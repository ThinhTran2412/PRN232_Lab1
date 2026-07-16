# Hướng Dẫn Chi Tiết Giải Đề Thực Hành PE PRN232 - SU25

Tài liệu này hướng dẫn chi tiết cách giải đề thi **PRN232_SU25_PE_Block10w** áp dụng kiến trúc 3 lớp (3-layer architecture), đảm bảo code gọn gàng, tối ưu và đáp ứng đầy đủ yêu cầu (Authentication, CRUD, Validate, Custom Error, Swagger).

---

## Bước 1: Chuẩn bị Database
1. Mở **SQL Server Management Studio (SSMS)**.
2. Mở file `SU25LeopardDB.txt` trong thư mục đề thi.
3. Chạy toàn bộ script (F5) để tạo database `SU25LeopardDB` cùng các bảng (`LeopardAccount`, `LeopardProfile`, `LeopardType`) và dữ liệu mẫu.

---

## Bước 2: Tạo Solution và Kiến Trúc 3 Lớp với Visual Studio 2022

Mở Visual Studio 2022 -> **Create a new project** -> Chọn **Blank Solution** -> Đặt tên `PRN231_SU25_SE<MãSinhViên>_Solution` (hoặc tên theo đề yêu cầu).

Trong Solution, chuột phải **Add -> New Project** để tạo 3 project sau:

1. **`Repositories` (Lớp Data Access):**
   - Chọn loại: **Class Library** (.NET 8 hoặc phiên bản theo yêu cầu).
   - Project này chịu trách nhiệm tương tác với Database bằng Entity Framework Core.
2. **`Services` (Lớp Business Logic):**
   - Chọn loại: **Class Library**.
   - Thêm tham chiếu (Add Project Reference) tới project `Repositories`.
3. **`PRN231_SU25_SE<MãSinhViên>.api` (Lớp Presentation - Web API):**
   - Chọn loại: **ASP.NET Core Web API**. Bỏ chọn "Configure for HTTPS" nếu không cần thiết để test nhanh.
   - Thêm tham chiếu tới cả `Repositories` và `Services`.
   - Set project API này làm **Startup Project** (Chuột phải -> Set as Startup Project).

---

## Bước 3: Lớp Data Access (`Repositories`) - Scaffolding DB

Mở **Terminal** hoặc **Command Prompt** tại thư mục chứa project `Repositories`.

### 3.1 Cài đặt thư viện EF Core:
Chạy các lệnh sau:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```
*(Lưu ý: có thể thêm cờ `-v 8.0.x` (ví dụ `-v 8.0.6`) để tương thích với .NET 8. Và cài cùng phiên bản thư viện vào project API nếu bị lỗi)*

### 3.2 Scaffold DbContext (Tạo Model từ Database):
Thực thi lệnh sau tại thư mục project `Repositories` để tự động tạo Models và DbContext (yêu cầu máy đã cài đặt Entity Framework Core tools toàn cục bằng lệnh `dotnet tool install --global dotnet-ef`):
```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=SU25LeopardDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models --context SU25LeopardDBContext
```
*Lưu ý thay `Server=localhost` bằng tên Server SQL của bạn.*

> [!WARNING]
> **Tuyệt đối không để chuỗi kết nối hardcode trong DbContext** (như đề yêu cầu sẽ bị 0 điểm).
> Bạn phải tìm và **XÓA (hoặc COMMENT)** phương thức `OnConfiguring` tự động sinh ra trong file `Models/SU25LeopardDBContext.cs`.

**Ví dụ đoạn code cần xử lý trong `SU25LeopardDBContext.cs`:**
```csharp
    // XÓA HOẶC COMMENT TOÀN BỘ ĐOẠN NÀY LẠI:
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // #warning To protect potentially sensitive information in your connection string...
    //     => optionsBuilder.UseSqlServer("Server=localhost;Database=SU25LeopardDB;Trusted_Connection=True;TrustServerCertificate=True;");
```

---

## Bước 4: Cấu hình `appsettings.json` trong project API

Mở `appsettings.json` trong project API và thêm chuỗi kết nối và cấu hình JWT:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SU25LeopardDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SE182004_TranThaiThinh_SecretKey_DungDeThiPE!",
    "Issuer": "SE182004",
    "Audience": "SE182004_API"
  }
}
```

---

## Bước 5: Cấu hình Lớp API (`Program.cs`)

Xóa mọi thứ trong `Program.cs` và cấu hình siêu gọn như sau:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Repositories.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext
builder.Services.AddDbContext<SU25LeopardDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký DI cho Services/Repositories
// Ví dụ: builder.Services.AddScoped<ILeopardProfileService, LeopardProfileService>();

// 3. Cấu hình JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// 4. Cấu hình Swagger hỗ trợ nhập Token
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, Description = "Nhập token", Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[]{} }
    });
});

var app = builder.Build();

// 5. Middleware xử lý Custom Error Format (HB50001, HB40101,...)
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == 401)
    {
        await response.WriteAsJsonAsync(new { errorCode = "HB40101", message = "Token bị thiếu/không hợp lệ" });
    }
    else if (response.StatusCode == 403)
    {
        await response.WriteAsJsonAsync(new { errorCode = "HB40301", message = "Không đủ quyền truy cập" });
    }
    // Bổ sung 404 nếu cần
});

// Thêm global exception handler cho mã lỗi HB50001
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { errorCode = "HB50001", message = "Lỗi server nội bộ" });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Bước 6: Viết API Controller cho Đăng Nhập (`AuthController`)

Tạo `AuthController.cs` để xử lý xác thực và trả về Token. Trong lớp Service, bạn viết logic check email/password từ bảng `LeopardAccount`.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Repositories.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SU25LeopardDBContext _context;
    private readonly IConfiguration _config;

    public AuthController(SU25LeopardDBContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.LeopardAccounts.FirstOrDefault(x => x.Email == request.Email && x.Password == request.Password);
        
        if (user == null) return Unauthorized(new { errorCode = "HB40101", message = "Sai tài khoản hoặc mật khẩu" });
        
        // Vai trò cho phép: 4, 5, 6, 7
        if (user.RoleId != 4 && user.RoleId != 5 && user.RoleId != 6 && user.RoleId != 7)
            return Unauthorized(new { errorCode = "HB40301", message = "Role không được cấp phép" });

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token), role = user.RoleId.ToString() });
    }
}

public class LoginRequest { public string Email { get; set; } public string Password { get; set; } }
```

---

## Bước 7: Viết Generic Repository (Siêu Nhanh, Bỏ qua Interface)

Để tiết kiệm tối đa thời gian trong 85 phút, chúng ta sẽ dùng **Generic Repository** thay vì tạo Repo cho từng bảng.

Trong project `Repositories`, tạo duy nhất class `GenericRepo.cs`. Nó sẽ lo vụ CRUD cơ bản cho MỌI BẢNG (Profile, Account, Type...).
```csharp
using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace Repositories;

public class GenericRepo<T> where T : class
{
    private readonly SU25LeopardDBContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepo(SU25LeopardDBContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public List<T> GetAll() => _dbSet.ToList();
    public T? GetById(object id) => _dbSet.Find(id);
    public void Create(T entity) { _dbSet.Add(entity); _context.SaveChanges(); }
    public void Update(T entity) { _dbSet.Update(entity); _context.SaveChanges(); }
    public void Delete(T entity) { _dbSet.Remove(entity); _context.SaveChanges(); }
    
    // Hàm này hỗ trợ việc viết Search bằng LINQ một cách dễ dàng
    public IQueryable<T> Query() => _dbSet.AsQueryable(); 
}
```

---

## Bước 8: Viết Service trực tiếp (Không dùng Interface)

Trong project `Services`, tạo `LeopardProfileService.cs` (Bỏ qua interface). Tiêm thẳng `GenericRepo<LeopardProfile>` vào để xài.

```csharp
using Repositories;
using Repositories.Models;
using System.Text.RegularExpressions;

namespace Services;

public class LeopardProfileService
{
    private readonly GenericRepo<LeopardProfile> _repo;

    public LeopardProfileService(GenericRepo<LeopardProfile> repo) => _repo = repo;

    public List<LeopardProfile> GetAll() => _repo.GetAll();

    public (bool IsSuccess, string ErrorMessage, LeopardProfile? Data) GetById(int id)
    {
        var profile = _repo.GetById(id);
        return profile == null ? (false, "Không tìm thấy", null) : (true, "", profile);
    }

    public (bool IsSuccess, string ErrorMessage, LeopardProfile? Data) Create(LeopardProfile model)
    {
        if (model.Weight <= 15) return (false, "Weight phải lớn hơn 15", null);
        if (!Regex.IsMatch(model.LeopardName, @"^([A-Z0-9][a-zA-Z0-9-#]*\s*[A-Z0-9][a-zA-Z0-9-#]*)$"))
            return (false, "LeopardName không đúng định dạng", null);

        _repo.Create(model);
        return (true, "", model);
    }

    public (bool IsSuccess, string ErrorMessage, LeopardProfile? Data) Update(int id, LeopardProfile model)
    {
        var existing = _repo.GetById(id);
        if (existing == null) return (false, "Không tìm thấy", null);

        if (model.Weight <= 15) return (false, "Weight phải lớn hơn 15", null);
        
        existing.LeopardName = model.LeopardName;
        existing.Weight = model.Weight;
        existing.Characteristics = model.Characteristics;
        existing.CareNeeds = model.CareNeeds;
        
        _repo.Update(existing);
        return (true, "", existing);
    }

    public (bool IsSuccess, string ErrorMessage) Delete(int id)
    {
        var profile = _repo.GetById(id);
        if (profile == null) return (false, "Không tìm thấy");

        _repo.Delete(profile);
        return (true, "");
    }

    public List<LeopardProfile> Search(string? name, double? weight)
    {
        var query = _repo.Query();
        if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.LeopardName.Contains(name));
        if (weight.HasValue) query = query.Where(x => x.Weight == weight.Value);
        return query.ToList();
    }
}
```

---

## Bước 9: Cập nhật Controller và đăng ký DI

Trong project API, tạo Controller gọi Service.
*(Lưu ý: Bạn phải đăng ký Dependency Injection ở `Program.cs` bằng 2 dòng code siêu gọn: `builder.Services.AddScoped(typeof(GenericRepo<>));` và `builder.Services.AddScoped<LeopardProfileService>();`)*

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;
using Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Bắt buộc có token
public class LeopardProfileController : ControllerBase
{
    private readonly LeopardProfileService _service;

    public LeopardProfileController(LeopardProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "4,5,6,7")]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "4,5,6,7")]
    public IActionResult GetById(int id)
    {
        var result = _service.GetById(id);
        if (!result.IsSuccess) return NotFound(new { errorCode = "HB40401", message = result.ErrorMessage });
        return Ok(result.Data);
    }

    [HttpPost]
    [Authorize(Roles = "5,6")] // Chỉ administrator và moderator
    public IActionResult Create([FromBody] LeopardProfile model)
    {
        var result = _service.Create(model);
        if (!result.IsSuccess) return BadRequest(new { errorCode = "HB40001", message = result.ErrorMessage });
        return StatusCode(201, result.Data);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "5,6")]
    public IActionResult Update(int id, [FromBody] LeopardProfile model)
    {
        var result = _service.Update(id, model);
        if (!result.IsSuccess) 
        {
            if (result.ErrorMessage == "Không tìm thấy") return NotFound(new { errorCode = "HB40401", message = result.ErrorMessage });
            return BadRequest(new { errorCode = "HB40001", message = result.ErrorMessage });
        }
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "5,6")]
    public IActionResult Delete(int id)
    {
        var result = _service.Delete(id);
        if (!result.IsSuccess) return NotFound(new { errorCode = "HB40401", message = result.ErrorMessage });
        return Ok(new { message = "Xóa thành công" });
    }

    [HttpGet("search")]
    [Authorize(Roles = "4,5,6,7")]
    public IActionResult Search([FromQuery] string? LeopardName, [FromQuery] double? Weight)
    {
        return Ok(_service.Search(LeopardName, Weight));
    }
}
```

---

## Bước 10: Viết script test Postman

1. Mở Postman, tạo 1 Collection mới `PRN231_SU25_SE<MãSinhViên>`.
2. Tạo 6 requests theo yêu cầu (Login Success, Login Fail, Create, Update, Delete, Get).
3. **Mẹo:** Trong request Login Success, tab **Tests**, viết script để lấy token tự động:
```javascript
var jsonData = pm.response.json();
if(jsonData.token){
    pm.environment.set("jwt_token", jsonData.token);
}
```
4. Ở các request khác (Create, Get...), vào tab **Authorization** -> Chọn loại **Bearer Token** -> Chèn giá trị `{{jwt_token}}`.
5. Bấm chuột phải vào Collection chọn **Export** -> Ra file `.json` để nộp bài.

---

### Một số lưu ý quan trọng để tránh 0 điểm:
- Không hardcode Connection String ở `SU25LeopardDBContext`. Phải lấy từ `appsettings.json` thông qua Dependency Injection trong `Program.cs`.
- Đảm bảo Code Build Success. Nếu lỗi đỏ, comment đoạn đó lại để chạy được.
- Đặt tên Project và tên Script Postman tuyệt đối chính xác theo MSSV.
- Mã lỗi trả về phải chuẩn format đề bài: `{"errorCode": "HB...", "message": "..."}`. Tránh để lộ nguyên stacktrace lỗi của .NET Core.
