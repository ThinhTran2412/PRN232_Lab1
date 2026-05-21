using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;

// Allow Npgsql to accept DateTime.Kind=Unspecified (required for seed data)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ─── DbContext ───────────────────────────────────────────────────────────────
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Repositories ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IStudentRepository,    StudentRepository>();
builder.Services.AddScoped<ICourseRepository,     CourseRepository>();
builder.Services.AddScoped<ISemesterRepository,   SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository,    SubjectRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// ─── Services ────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IStudentService,    StudentService>();
builder.Services.AddScoped<ICourseService,     CourseService>();
builder.Services.AddScoped<ISemesterService,   SemesterService>();
builder.Services.AddScoped<ISubjectService,    SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// ─── AutoMapper (scans all assemblies) ───────────────────────────────────────
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ─── Controllers + JSON ──────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ─── Swagger / OpenAPI ───────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "PRN232 LMS API",
        Version     = "v1",
        Description = "Learning Management System RESTful API – PRN232 LAB 1"
    });
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ─── Auto-migrate với retry (chờ PostgreSQL sẵn sàng trong Docker) ───────────
using (var scope = app.Services.CreateScope())
{
    var db     = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<LmsDbContext>>();
    var maxRetries = 10;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("✅ Database migration applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            if (attempt == maxRetries) throw;
            logger.LogWarning("⏳ DB not ready (attempt {A}/{M}): {Msg}. Retrying in 3s...",
                attempt, maxRetries, ex.Message);
            Thread.Sleep(3000);
        }
    }
}

// ─── Pipeline ────────────────────────────────────────────────────────────────
// Swagger luôn bật (Development + Docker Production đều dùng được)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API V1");
    c.RoutePrefix = string.Empty; // Swagger tại "/"
});

// app.UseHttpsRedirection(); // Tắt: Docker chạy HTTP-only (port 80)
app.UseAuthorization();
app.MapControllers();

app.Run();
