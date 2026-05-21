# PRN232 – LAB 1: REST API Design & Implementation Specification

> **Môn:** Technical Requirements & Design Standards  
> **Project:** Learning Management System (LMS) – ASP.NET Core RESTful API  
> **Kiến trúc:** 3-Layer Architecture + Docker + Swagger

---

## 1. Cấu trúc Project (Solution Structure)

```
PRN232.LMS.sln
├── PRN232.LMS.API/                  ← Controller Layer (Entry point)
├── PRN232.LMS.Services/             ← Business Logic Layer
└── PRN232.LMS.Repositories/        ← Data Access Layer
```

### Quy tắc đặt tên
| Layer | Namespace |
|-------|-----------|
| API | `PRN232.[ProjectName].API` |
| Services | `PRN232.[ProjectName].Services` |
| Repositories | `PRN232.[ProjectName].Repositories` |

---

## 2. Database Schema

### Bảng bắt buộc

```sql
-- Semester
CREATE TABLE "Semester" (
    "SemesterId"   SERIAL PRIMARY KEY,
    "SemesterName" VARCHAR(100) NOT NULL,
    "StartDate"    TIMESTAMP NOT NULL,
    "EndDate"      TIMESTAMP NOT NULL
);

-- Course
CREATE TABLE "Course" (
    "CourseId"   SERIAL PRIMARY KEY,
    "CourseName" VARCHAR(100) NOT NULL,
    "SemesterId" INT NOT NULL REFERENCES "Semester"("SemesterId")
);

-- Subject
CREATE TABLE "Subject" (
    "SubjectId"   SERIAL PRIMARY KEY,
    "SubjectCode" VARCHAR(20) NOT NULL,
    "SubjectName" VARCHAR(100) NOT NULL,
    "Credit"      INT NOT NULL
);

-- Student
CREATE TABLE "Student" (
    "StudentId"   SERIAL PRIMARY KEY,
    "FullName"    VARCHAR(100) NOT NULL,
    "Email"       VARCHAR(100) NOT NULL,
    "DateOfBirth" TIMESTAMP NOT NULL
);

-- Enrollment
CREATE TABLE "Enrollment" (
    "EnrollmentId" SERIAL PRIMARY KEY,
    "StudentId"    INT NOT NULL REFERENCES "Student"("StudentId"),
    "CourseId"     INT NOT NULL REFERENCES "Course"("CourseId"),
    "EnrollDate"   TIMESTAMP NOT NULL,
    "Status"       VARCHAR(20) NOT NULL
);
```

> 💡 PostgreSQL dùng `SERIAL` thay `IDENTITY`, `VARCHAR` thay `NVARCHAR`, `TIMESTAMP` thay `DATETIME`.

### Dữ liệu mẫu tối thiểu
| Bảng | Số bản ghi |
|------|-----------|
| Semester | ≥ 5 |
| Student | ≥ 50 |
| Subject | ≥ 10 |
| Course | ≥ 20 |
| Enrollment | ≥ 500 |

---

## 3. 4 Loại Model (Model Types)

### 3.1 Entity Model — Ánh xạ database

```csharp
// PRN232.LMS.Repositories/Entities/Student.cs
public class Student
{
    public int StudentId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
}
```

### 3.2 Business Model — Xử lý logic nghiệp vụ

```csharp
// PRN232.LMS.Services/BusinessModels/StudentBM.cs
public class StudentBM
{
    public int StudentId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    public List<EnrollmentBM> Enrollments { get; set; }
}
```

### 3.3 Request Model — Input từ client

```csharp
// PRN232.LMS.API/Models/Requests/CreateStudentRequest.cs
public class CreateStudentRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}

// PRN232.LMS.API/Models/Requests/UpdateStudentRequest.cs
public class UpdateStudentRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}
```

### 3.4 Response Model — Output trả về API

```csharp
// PRN232.LMS.API/Models/Responses/StudentResponse.cs
public class StudentResponse
{
    public int StudentId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}
```

### Quy tắc sử dụng Model
| Layer | Entity | Business | Request | Response |
|-------|--------|----------|---------|----------|
| Repository | ✅ | ❌ | ❌ | ❌ |
| Service | ✅ (map sang BM) | ✅ | ❌ | ❌ |
| Controller | ❌ | ✅ (nhận từ Service) | ✅ | ✅ |

> ⚠️ **Không bao giờ** return Entity Model trực tiếp trong API response.  
> ⚠️ **Không dùng** Request/Response Model trong Repository Layer.

---

## 4. 3-Layer Architecture – Trách nhiệm từng layer

### 4.1 Repository Layer

```csharp
// Interface
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student> CreateAsync(Student student);
    Task<Student?> UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
}

// Implementation
public class StudentRepository : IStudentRepository
{
    private readonly LmsDbContext _context;
    public StudentRepository(LmsDbContext context) => _context = context;

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.StudentId == id);

    // ... các method khác
}
```

> ✅ Chỉ xử lý CRUD và query database  
> ❌ Không chứa business logic  
> ❌ Không dùng Request/Response Model

### 4.2 Service Layer

```csharp
// Interface
public interface IStudentService
{
    Task<StudentBM?> GetByIdAsync(int id);
    Task<PagedResult<StudentBM>> GetAllAsync(StudentQueryParams query);
    Task<StudentBM> CreateAsync(CreateStudentRequest request);
    Task<StudentBM?> UpdateAsync(int id, UpdateStudentRequest request);
    Task<bool> DeleteAsync(int id);
}

// Implementation
public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<StudentBM?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<StudentBM>(entity);
    }

    public async Task<StudentBM> CreateAsync(CreateStudentRequest request)
    {
        var entity = _mapper.Map<Student>(request);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<StudentBM>(created);
    }
}
```

> ✅ Chứa toàn bộ business logic  
> ✅ Mapping Entity ↔ BusinessModel  
> ❌ Không truy cập DB trực tiếp

### 4.3 Controller Layer

```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly IMapper _mapper;

    public StudentsController(IStudentService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var bm = await _service.GetByIdAsync(id);
        if (bm == null)
            return NotFound(ApiResponse<object>.Fail("Student not found"));

        var response = _mapper.Map<StudentResponse>(bm);
        return Ok(ApiResponse<StudentResponse>.Success(response));
    }
}
```

> ✅ Nhận Request, trả Response  
> ✅ Gọi Service  
> ❌ Không chứa business logic  
> ❌ Không truy cập Repository trực tiếp

---

## 5. RESTful API Design

### 5.1 Nguyên tắc đặt URL

| ✅ Đúng | ❌ Sai |
|---------|--------|
| `GET /api/students` | `GET /api/getStudents` |
| `GET /api/students/{id}` | `GET /api/student/{id}` |
| `POST /api/enrollments` | `POST /api/createEnrollment` |
| `PUT /api/courses/{id}` | `PUT /api/updateCourse/{id}` |
| `DELETE /api/semesters/{id}` | `DELETE /api/deleteSemester` |

### 5.2 Toàn bộ Endpoints

#### Semesters
```
GET    /api/semesters
GET    /api/semesters/{id}
POST   /api/semesters
PUT    /api/semesters/{id}
DELETE /api/semesters/{id}
```

#### Courses
```
GET    /api/courses
GET    /api/courses/{id}
POST   /api/courses
PUT    /api/courses/{id}
DELETE /api/courses/{id}
```

#### Subjects
```
GET    /api/subjects
GET    /api/subjects/{id}
POST   /api/subjects
PUT    /api/subjects/{id}
DELETE /api/subjects/{id}
```

#### Students
```
GET    /api/students
GET    /api/students/{id}
POST   /api/students
PUT    /api/students/{id}
DELETE /api/students/{id}
```

#### Enrollments
```
GET    /api/enrollments
GET    /api/enrollments/{id}
POST   /api/enrollments
PUT    /api/enrollments/{id}
DELETE /api/enrollments/{id}
```

---

## 6. Query Parameters – Search, Sort, Page, Fields, Expand

### 6.1 Query Params Model

```csharp
public class QueryParams
{
    public string? Search { get; set; }
    public string? Sort { get; set; }        // "fullName,-dateOfBirth"
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Fields { get; set; }      // "studentId,fullName,email"
    public string? Expand { get; set; }      // "course,student"
}
```

### 6.2 Ví dụ URL đầy đủ

```
GET /api/students?search=nguyen
GET /api/students?sort=fullName,-dateOfBirth
GET /api/students?page=2&size=10
GET /api/students?fields=studentId,fullName,email
GET /api/enrollments?expand=student,course
GET /api/enrollments?search=active&sort=-enrollDate&page=1&size=20&fields=enrollmentId,status&expand=student,course
```

### 6.3 Logic xử lý từng tính năng

#### Search
```csharp
if (!string.IsNullOrWhiteSpace(query.Search))
{
    var keyword = query.Search.ToLower();
    source = source.Where(s =>
        s.FullName.ToLower().Contains(keyword) ||
        s.Email.ToLower().Contains(keyword));
}
```

#### Sort (hỗ trợ multi-field, dấu `-` = descending)
```csharp
if (!string.IsNullOrWhiteSpace(query.Sort))
{
    var fields = query.Sort.Split(',');
    IOrderedQueryable<Student>? ordered = null;

    foreach (var field in fields)
    {
        var desc = field.StartsWith("-");
        var name = field.TrimStart('-');

        if (ordered == null)
            ordered = desc
                ? source.OrderByDescending(s => EF.Property<object>(s, name))
                : source.OrderBy(s => EF.Property<object>(s, name));
        else
            ordered = desc
                ? ordered.ThenByDescending(s => EF.Property<object>(s, name))
                : ordered.ThenBy(s => EF.Property<object>(s, name));
    }
    if (ordered != null) source = ordered;
}
```

#### Paging
```csharp
var totalItems = await source.CountAsync();
var totalPages = (int)Math.Ceiling(totalItems / (double) query.Size);
var items = await source.Skip((query.Page - 1) * query.Size).Take(query.Size).ToListAsync();
```

#### Field Selection (dynamic projection)
```csharp
// Dùng System.Reflection hoặc thư viện như Sieve / EF dynamic projection
// Hoặc return ExpandoObject
public static object SelectFields(object obj, string fields)
{
    var props = fields.Split(',').Select(f => f.Trim()).ToHashSet();
    var expando = new ExpandoObject() as IDictionary<string, object>;
    foreach (var prop in obj.GetType().GetProperties())
        if (props.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
            expando[prop.Name] = prop.GetValue(obj)!;
    return expando;
}
```

#### Expand (eager loading related entities)
```csharp
if (!string.IsNullOrWhiteSpace(query.Expand))
{
    var parts = query.Expand.Split(',');
    if (parts.Contains("student"))
        source = source.Include(e => e.Student);
    if (parts.Contains("course"))
        source = source.Include(e => e.Course);
}
```

---

## 7. Pagination Metadata & Response Format

### 7.1 Wrapper Response chung

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, object? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
```

### 7.2 Paged Result

```csharp
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public PaginationMeta Pagination { get; set; }
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
```

### 7.3 JSON mẫu – List response

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {
    "items": [
      { "studentId": 1, "fullName": "Nguyen Van A", "email": "a@fpt.edu.vn" }
    ],
    "pagination": {
      "page": 1,
      "pageSize": 10,
      "totalItems": 50,
      "totalPages": 5
    }
  },
  "errors": null
}
```

### 7.4 JSON mẫu – Single resource

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {
    "studentId": 1,
    "fullName": "Nguyen Van A",
    "email": "a@fpt.edu.vn",
    "dateOfBirth": "2003-01-15T00:00:00"
  },
  "errors": null
}
```

### 7.5 JSON mẫu – Error response

```json
{
  "success": false,
  "message": "Student not found",
  "data": null,
  "errors": null
}
```

---

## 8. HTTP Status Codes

| Code | Ý nghĩa | Khi nào dùng |
|------|---------|--------------|
| `200 OK` | Thành công | GET, PUT thành công |
| `201 Created` | Tạo mới thành công | POST thành công |
| `400 Bad Request` | Input sai | Dữ liệu không hợp lệ |
| `404 Not Found` | Không tìm thấy | GET/PUT/DELETE không có resource |
| `500 Internal Server Error` | Lỗi server | Exception không xử lý được |

### Ví dụ Controller áp dụng đúng status code

```csharp
// POST → 201
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
{
    var bm = await _service.CreateAsync(request);
    var response = _mapper.Map<StudentResponse>(bm);
    return CreatedAtAction(nameof(GetById), new { id = response.StudentId },
        ApiResponse<StudentResponse>.Success(response, "Student created successfully"));
}

// GET by ID → 200 hoặc 404
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var bm = await _service.GetByIdAsync(id);
    if (bm == null) return NotFound(ApiResponse<object>.Fail("Student not found"));
    return Ok(ApiResponse<StudentResponse>.Success(_mapper.Map<StudentResponse>(bm)));
}

// PUT → 200 hoặc 404
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
{
    var bm = await _service.UpdateAsync(id, request);
    if (bm == null) return NotFound(ApiResponse<object>.Fail("Student not found"));
    return Ok(ApiResponse<StudentResponse>.Success(_mapper.Map<StudentResponse>(bm)));
}

// DELETE → 200 hoặc 404
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var deleted = await _service.DeleteAsync(id);
    if (!deleted) return NotFound(ApiResponse<object>.Fail("Student not found"));
    return Ok(ApiResponse<object>.Success(null, "Student deleted successfully"));
}
```

---

## 9. Docker Deployment

### 9.1 Dockerfile (đặt tại root `PRN232.LMS.API/`)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"
RUN dotnet publish "PRN232.LMS.API/PRN232.LMS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]
```

### 9.2 docker-compose.yml

```yaml
version: "3.9"

services:
  db:
    image: postgres:16-alpine
    container_name: lms_db
    environment:
      POSTGRES_DB: lmsdb
      POSTGRES_USER: lmsuser
      POSTGRES_PASSWORD: YourStrong@Passw0rd
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
    networks:
      - lms_network

  api:
    build:
      context: .
      dockerfile: PRN232.LMS.API/Dockerfile
    container_name: lms_api
    depends_on:
      - db
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=lmsdb;Username=lmsuser;Password=YourStrong@Passw0rd
    ports:
      - "8080:80"
    networks:
      - lms_network

volumes:
  pgdata:

networks:
  lms_network:
```

### 9.3 Connection String trong `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=lmsdb;Username=lmsuser;Password=YourStrong@Passw0rd"
  }
}
```

### 9.4 Lệnh chạy

```bash
docker compose up --build -d
```

---

## 10. Swagger / OpenAPI Configuration

### 10.1 Cài đặt trong `Program.cs`

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PRN232 LMS API",
        Version = "v1",
        Description = "Learning Management System RESTful API"
    });
});

// Trong pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API V1");
        c.RoutePrefix = string.Empty; // Swagger tại root "/"
    });
}
```

### 10.2 Annotate Controller để Swagger hiển thị đầy đủ

```csharp
/// <summary>Get student by ID</summary>
/// <param name="id">Student ID</param>
/// <response code="200">Returns student data</response>
/// <response code="404">Student not found</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetById(int id) { ... }
```

---

## 11. Dependency Injection Setup (`Program.cs`)

```csharp
// DbContext — dùng Npgsql cho PostgreSQL
// NuGet: Npgsql.EntityFrameworkCore.PostgreSQL
builder.Services.AddDbContext<LmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

// Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
```

---

## 12. AutoMapper Profile

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → BusinessModel
        CreateMap<Student, StudentBM>();
        CreateMap<Enrollment, EnrollmentBM>();
        CreateMap<Course, CourseBM>();

        // BusinessModel → Response
        CreateMap<StudentBM, StudentResponse>();
        CreateMap<EnrollmentBM, EnrollmentResponse>();

        // Request → Entity
        CreateMap<CreateStudentRequest, Student>();
        CreateMap<UpdateStudentRequest, Student>();
        CreateMap<CreateEnrollmentRequest, Enrollment>();
    }
}
```

---

## 13. NuGet Packages cần cài

| Package | Layer | Mục đích |
|---------|-------|----------|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | Repositories | EF Core provider cho PostgreSQL |
| `Microsoft.EntityFrameworkCore.Tools` | Repositories | Migration CLI (`dotnet ef`) |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | Services | Object mapping |
| `Swashbuckle.AspNetCore` | API | Swagger/OpenAPI |

### Lệnh cài nhanh
```bash
# Trong project Repositories
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Trong project Services
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Trong project API
dotnet add package Swashbuckle.AspNetCore
```

### DbContext mẫu (PostgreSQL)
```csharp
public class LmsDbContext : DbContext
{
    public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PostgreSQL: map tên bảng thường (snake_case hoặc PascalCase tuỳ ý)
        modelBuilder.Entity<Student>().ToTable("Student");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Semester>().ToTable("Semester");
        modelBuilder.Entity<Subject>().ToTable("Subject");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
    }
}
```

### Migration
```bash
dotnet ef migrations add InitialCreate --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API
dotnet ef database update --project PRN232.LMS.Repositories --startup-project PRN232.LMS.API
```

---

## 14. Checklist Evaluation

| # | Tiêu chí | Mô tả |
|---|----------|-------|
| ✅ 1 | 3-layer architecture | API / Services / Repositories tách biệt |
| ✅ 2 | 4 model types | Entity, Business, Request, Response |
| ✅ 3 | RESTful URL | Plural nouns, resource-based |
| ✅ 4 | Search, Sort, Paging | Query params đầy đủ |
| ✅ 5 | Field Selection | `?fields=` param |
| ✅ 6 | Expansion | `?expand=` param |
| ✅ 7 | Pagination metadata | `page, pageSize, totalItems, totalPages` |
| ✅ 8 | Consistent response format | `ApiResponse<T>` wrapper |
| ✅ 9 | HTTP Status Codes | 200, 201, 400, 404, 500 |
| ✅ 10 | Docker deployment | Dockerfile + docker-compose.yml |
| ✅ 11 | Swagger/OpenAPI | Endpoint docs + test UI |

---

## 15. Out of Scope (KHÔNG cần làm)

- ❌ Authentication / Authorization
- ❌ JWT Security
- ❌ Advanced Validation (FluentValidation,...)
- ❌ Global Exception Handling Middleware
- ❌ Unit Testing / Integration Testing

---

## 16. Tóm tắt luồng dữ liệu

```
Client Request
     ↓
[Controller] – nhận Request Model, gọi Service
     ↓
[Service]    – xử lý logic, map Entity ↔ BusinessModel
     ↓
[Repository] – CRUD với Entity Model qua EF Core
     ↓
[Database]   – PostgreSQL chạy trong Docker
     ↑
[Repository] – trả Entity lên Service
     ↑
[Service]    – map sang BusinessModel
     ↑
[Controller] – map sang Response Model → ApiResponse<T>
     ↑
Client Response (JSON)
```
