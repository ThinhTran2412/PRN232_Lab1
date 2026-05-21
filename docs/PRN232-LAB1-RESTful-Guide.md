# PRN232 – LAB 1: RESTful API Design Guide
> Kết hợp chuẩn RESTful API (restfulapi.net) áp dụng vào yêu cầu LAB 1 – Learning Management System (LMS)

---

## Mục lục
1. [Tổng quan REST & LMS](#1-tổng-quan-rest--lms)
2. [6 Ràng buộc Kiến trúc REST – Áp dụng vào LAB1](#2-6-ràng-buộc-kiến-trúc-rest--áp-dụng-vào-lab1)
3. [Kiến trúc 3 tầng & 4 Model Types](#3-kiến-trúc-3-tầng--4-model-types)
4. [Database Schema & Seed Data](#4-database-schema--seed-data)
5. [Thiết kế URI – Resource Naming](#5-thiết-kế-uri--resource-naming)
6. [HTTP Methods – Áp dụng vào LMS](#6-http-methods--áp-dụng-vào-lms)
7. [HTTP Status Codes](#7-http-status-codes)
8. [Response Format chuẩn](#8-response-format-chuẩn)
9. [GET by ID – Quy tắc và ví dụ](#9-get-by-id--quy-tắc-và-ví-dụ)
10. [GET Collection – Search, Sort, Page, Select, Expand](#10-get-collection--search-sort-page-select-expand)
11. [Statelessness trong LMS](#11-statelessness-trong-lms)
12. [Caching](#12-caching)
13. [Content Negotiation & Compression](#13-content-negotiation--compression)
14. [HATEOAS](#14-hateoas)
15. [Idempotence](#15-idempotence)
16. [Versioning](#16-versioning)
17. [Swagger / OpenAPI](#17-swagger--openapi)
18. [Docker Deployment](#18-docker-deployment)
19. [Richardson Maturity Model – Mục tiêu LAB1](#19-richardson-maturity-model--mục-tiêu-lab1)
20. [Evaluation Checklist](#20-evaluation-checklist)
21. [Out of Scope](#21-out-of-scope)

---

## 1. Tổng quan REST & LMS

**REST** (Representational State Transfer) là phong cách kiến trúc do Roy Fielding đặt ra năm 2000. Đây **không phải là giao thức** mà là tập hợp các ràng buộc để thiết kế ứng dụng kết nối mạng.

Trong LAB1, hệ thống LMS sẽ được xây dựng dưới dạng ASP.NET Core RESTful API với các đặc điểm:

- Dữ liệu (Semester, Course, Subject, Student, Enrollment) là các **resource**, truy cập qua **URI**.
- Client và Server trao đổi qua **JSON representations** thông qua HTTP.
- Mọi tương tác phải là **stateless** — mỗi request tự chứa đủ thông tin.
- REST và HTTP không phải là một — nhưng LAB1 sử dụng HTTP làm giao thức nền.

---

## 2. 6 Ràng buộc Kiến trúc REST – Áp dụng vào LAB1

### 2.1 Uniform Interface (Giao diện Thống nhất)
Là ràng buộc quan trọng nhất, bao gồm 4 yếu tố:

| Yếu tố | Áp dụng trong LMS |
|--------|-------------------|
| **Identification of resources** | Mỗi resource có URI duy nhất: `/api/students/1`, `/api/enrollments/10` |
| **Manipulation through representations** | Client dùng representation của resource để tương tác (GET) |
| **Self-descriptive messages** | Response chứa `Content-Type: application/json`, status code rõ ràng |
| **HATEOAS** | Response có thể chứa `links` trỏ đến các hành động liên quan |

> Toàn bộ API trong LMS phải dùng **JSON** làm định dạng chuẩn. Naming convention, link format và cấu trúc response phải **nhất quán** trên toàn hệ thống.

### 2.2 Client–Server (Phân tách Client và Server)
- **API Layer** (Controllers) là phần server — client chỉ cần biết URI, không cần biết logic bên trong.
- Frontend/Postman/Swagger là client — có thể phát triển hoàn toàn độc lập với backend.
- Miễn là contract (URI, request/response format) không đổi, cả hai bên có thể thay đổi nội bộ tự do.

### 2.3 Stateless (Phi trạng thái)
- Server **không lưu session** giữa các request.
- Mỗi HTTP request đến LMS API phải **tự chứa đủ thông tin** để xử lý (resource ID, query params, body...).
- Không dùng server-side session để lưu trạng thái client.
- *(Authentication/Authorization là Out of Scope — xem mục 21)*

### 2.4 Cacheable (Có thể Cache)
- Response phải khai báo rõ là cacheable hay non-cacheable qua HTTP headers.
- Áp dụng với các GET endpoint ít thay đổi: danh sách Semester, Subject.

### 2.5 Layered System (Hệ thống Phân tầng)
- Kiến trúc 3 tầng của LAB1 (API → Service → Repository) chính là biểu hiện của Layered System.
- Client chỉ giao tiếp với **API Layer** — không biết Service hay Repository tồn tại.
- Docker Compose cho phép tách API container và Database container — client không cần biết DB ở đâu.

### 2.6 Code on Demand *(Tùy chọn — Không áp dụng trong LAB1)*
- Ràng buộc tùy chọn, không yêu cầu trong LAB1.

---

## 3. Kiến trúc 3 tầng & 4 Model Types

### 3.1 Project Structure

```
PRN232.[ProjectName].API/           ← API Layer (Controllers, Swagger, Docker)
PRN232.[ProjectName].Services/      ← Service Layer (Business Logic)
PRN232.[ProjectName].Repositories/  ← Repository Layer (Data Access, Entity Models)
```

**Nguyên tắc tách biệt trách nhiệm:**

| Tầng | Trách nhiệm | Không được làm |
|------|-------------|----------------|
| **API Layer** | Nhận request, trả response, map Request/Response Models | Chứa business logic |
| **Service Layer** | Xử lý business logic, dùng Business Models | Truy cập DB trực tiếp |
| **Repository Layer** | Truy cập DB, làm việc với Entity Models | Chứa business logic |

### 3.2 4 Model Types

| Model | Vị trí | Mục đích | Ví dụ |
|-------|--------|---------|-------|
| **Entity Model** | Repository Layer | Mapping trực tiếp với bảng DB | `StudentEntity`, `EnrollmentEntity` |
| **Business Model** | Service Layer | Xử lý logic nghiệp vụ | `StudentBusiness`, `EnrollmentBusiness` |
| **Request Model** | API Layer (input) | Nhận query params từ client (search, sort, page...) | `StudentQueryRequest`, `EnrollmentQueryRequest` |
| **Response Model** | API Layer (output) | Trả dữ liệu về cho client | `StudentResponse`, `EnrollmentResponse` |

**Quy tắc bắt buộc:**
- ❌ **Không** trả Entity Model trực tiếp trong API response.
- ❌ **Không** dùng Request/Response Model trong Repository Layer.
- ✅ Map Entity → Business Model tại Service Layer.
- ✅ Map Business Model → Response Model tại API Layer.

---

## 4. Database Schema & Seed Data

### 4.1 Bảng bắt buộc

```sql
Semester   (SemesterId int, SemesterName nvarchar(100), StartDate datetime, EndDate datetime)
Course     (CourseId int, CourseName nvarchar(100), SemesterId int)
Subject    (SubjectId int, SubjectCode varchar(20), SubjectName nvarchar(100), Credit int)
Student    (StudentId int, FullName nvarchar(100), Email varchar(100), DateOfBirth datetime)
Enrollment (EnrollmentId int, StudentId int, CourseId int, EnrollDate datetime, Status varchar(20))
```

> Students may add additional tables if needed.

### 4.2 Seed Data tối thiểu

| Bảng | Số lượng tối thiểu |
|------|--------------------|
| Semester | 5 |
| Student | 50 |
| Subject | 10 |
| Course | 20 |
| Enrollment | 500 |

---

## 5. Thiết kế URI – Resource Naming

### 5.1 Nguyên tắc cốt lõi
- URI trỏ đến **resource** (danh từ), **không phải hành động** (động từ).
- Dùng **danh từ số nhiều** cho collection.
- Mỗi resource có **một URI logic duy nhất**.

### 5.2 URI chuẩn cho LMS

| Resource | Collection URI | Singular URI |
|----------|---------------|--------------|
| Student | `GET /api/students` | `GET /api/students/{id}` |
| Course | `GET /api/courses` | `GET /api/courses/{id}` |
| Subject | `GET /api/subjects` | `GET /api/subjects/{id}` |
| Semester | `GET /api/semesters` | `GET /api/semesters/{id}` |
| Enrollment | `GET /api/enrollments` | `GET /api/enrollments/{id}` |

### 5.3 Ví dụ đúng / sai

```
✅ Đúng:  /api/students
✅ Đúng:  /api/students/{id}
✅ Đúng:  /api/enrollments/{id}
✅ Đúng:  /api/courses/{id}/enrollments    ← sub-resource

❌ Sai:   /api/getStudents
❌ Sai:   /api/createEnrollment
❌ Sai:   /api/students/                   ← trailing slash
❌ Sai:   /api/students.json               ← extension file
```

### 5.4 Quy tắc đặt tên URI

| Quy tắc | Ví dụ |
|---------|-------|
| Chữ thường (lowercase) | `/api/students` ✅ |
| Dùng dấu gạch ngang (-) thay gạch dưới (_) | `/api/student-enrollments` ✅ |
| Không dùng trailing slash | `/api/students` ✅ |
| Không dùng extension file | `/api/students` ✅ |
| Query parameter cho filter/sort/page | `/api/students?search=nguyen&page=1` ✅ |

---

## 6. HTTP Methods – Áp dụng vào LMS

### 6.1 Bảng tổng hợp

| Method | Mục đích | Safe? | Idempotent? | Status trả về |
|--------|----------|-------|-------------|---------------|
| **GET** | Lấy danh sách hoặc chi tiết resource | ✅ | ✅ | 200 OK |

### 6.2 Mapping vào LMS

```
GET /api/students              → Lấy danh sách students (có search/sort/page)
GET /api/students/{id}         → Lấy chi tiết 1 student

GET /api/enrollments           → Lấy danh sách enrollments
GET /api/enrollments/{id}      → Lấy chi tiết 1 enrollment

GET /api/courses               → Lấy danh sách courses
GET /api/courses/{id}          → Lấy chi tiết 1 course

GET /api/subjects              → Lấy danh sách subjects
GET /api/subjects/{id}         → Lấy chi tiết 1 subject

GET /api/semesters             → Lấy danh sách semesters
GET /api/semesters/{id}        → Lấy chi tiết 1 semester
```

### 6.3 Safe vs Idempotent

- **Safe method**: GET — không thay đổi trạng thái server.
- **Idempotent**: GET — nhiều request giống nhau luôn trả về cùng kết quả.

---

## 7. HTTP Status Codes

Theo yêu cầu LAB1, sử dụng đúng các status code sau:

| Code | Ý nghĩa | Khi nào dùng trong LMS |
|------|---------|------------------------|
| **200 OK** | Thành công | GET thành công |
| **400 Bad Request** | Request không hợp lệ | Query param sai format |
| **404 Not Found** | Không tìm thấy resource | GET student không tồn tại |
| **500 Internal Server Error** | Lỗi server | Exception không xử lý được |

**Nguyên tắc:** Không bao giờ trả về `200 OK` cho một lỗi.

```
✅ Student tồn tại        → 200 OK + data
✅ Student không tồn tại  → 404 Not Found + message
✅ Query param sai        → 400 Bad Request + errors
✅ Lỗi server             → 500 Internal Server Error
```

---

## 8. Response Format chuẩn

Tất cả API phải trả về **cùng một cấu trúc response**.

### 8.1 Cấu trúc chuẩn

```json
{
  "success": true,
  "message": "Request processed successfully",
  "data": {},
  "errors": null
}
```

### 8.2 Ví dụ theo từng trường hợp

**Thành công – GET single:**
```json
{
  "success": true,
  "message": "Student retrieved successfully",
  "data": {
    "studentId": 1,
    "fullName": "Nguyen Van A",
    "email": "vana@email.com",
    "dateOfBirth": "2002-01-15"
  },
  "errors": null
}
```

**Thành công – GET collection (có pagination):**
```json
{
  "success": true,
  "message": "Students retrieved successfully",
  "data": {
    "items": [...],
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

**Lỗi – 404:**
```json
{
  "success": false,
  "message": "Student not found",
  "data": null,
  "errors": ["Student with id 99 does not exist"]
}
```

**Lỗi – 400:**
```json
{
  "success": false,
  "message": "Bad request",
  "data": null,
  "errors": ["Invalid query parameter: sort value 'xyz' is not a valid field"]
}
```

---

## 9. GET by ID – Quy tắc và ví dụ

### 9.1 Nguyên tắc
- Trả về **đầy đủ thông tin liên quan** của resource.
- **Tránh circular references** và infinite recursion khi include related entity.
- Trả về **HTTP 404** nếu resource không tồn tại.

### 9.2 Ví dụ

```
GET /api/students/1
GET /api/enrollments/10
GET /api/courses/5
```

**Response GET /api/enrollments/10:**
```json
{
  "success": true,
  "message": "Enrollment retrieved successfully",
  "data": {
    "enrollmentId": 10,
    "enrollDate": "2024-09-01",
    "status": "Active",
    "student": {
      "studentId": 3,
      "fullName": "Le Van C",
      "email": "vanc@email.com"
    },
    "course": {
      "courseId": 2,
      "courseName": "Web Development",
      "semester": {
        "semesterId": 1,
        "semesterName": "Fall 2024"
      }
    }
  },
  "errors": null
}
```

> ⚠️ Lưu ý: Khi include nested object (student → enrollments → student...), phải dừng đệ quy tại một cấp nhất định để tránh infinite loop.

---

## 10. GET Collection – Search, Sort, Page, Select, Expand

Tất cả List API phải hỗ trợ đầy đủ 5 tính năng sau:

### 10.1 Searching – Tìm kiếm theo keyword

```
GET /api/students?search=nguyen
GET /api/enrollments?search=active
```

### 10.2 Sorting – Sắp xếp

- Prefix `-` = descending (giảm dần)
- Không có prefix = ascending (tăng dần)

```
GET /api/students?sort=fullName
GET /api/students?sort=fullName,-dateOfBirth
GET /api/enrollments?sort=-enrollDate
```

### 10.3 Paging – Phân trang

```
GET /api/students?page=2&size=10
GET /api/enrollments?page=1&size=20
```

**Pagination Metadata bắt buộc trong response:**
```json
"pagination": {
  "page": 1,
  "pageSize": 10,
  "totalItems": 50,
  "totalPages": 5
}
```

### 10.4 Selection – Chọn fields

```
GET /api/students?fields=studentId,fullName,email
GET /api/enrollments?fields=enrollmentId,status
```

### 10.5 Expansion – Include related entities

```
GET /api/enrollments?expand=student,course
GET /api/courses?expand=semester
```

### 10.6 Kết hợp tất cả

```
GET /api/enrollments?search=active&sort=-enrollDate&page=1&size=20&fields=enrollmentId,status&expand=student,course
```

---

## 11. Statelessness trong LMS

- Server **không lưu session** — mỗi request phải tự chứa đủ thông tin.
- Không dùng server-side session để nhớ kết quả query trước đó.
- Mỗi request đến `/api/students?page=2` phải mang đầy đủ params — server không nhớ client đang ở page nào.

**Lợi ích trong LMS:**
- Scale ngang dễ dàng khi có nhiều request đồng thời.
- Mỗi Docker container API có thể xử lý bất kỳ request nào — không phụ thuộc session.
- Dễ cache response vì mỗi request mô tả hoàn toàn kết quả mong muốn.

---

## 12. Caching

Response nên khai báo caching thông qua HTTP headers khi phù hợp.

### Headers quan trọng

| Header | Mô tả | Áp dụng trong LMS |
|--------|-------|-------------------|
| `Cache-Control` | Chính sách cache | `Cache-Control: max-age=300` cho GET /semesters |
| `ETag` | Version token của resource | Dùng cho GET /students/{id} |
| `Last-Modified` | Lần sửa đổi cuối | Dùng cho các resource ít thay đổi |

> Các endpoint như `GET /api/semesters` (dữ liệu ít thay đổi) nên được cache. Các endpoint như `GET /api/enrollments` (dữ liệu thay đổi thường xuyên) nên có `Cache-Control: no-cache`.

---

## 13. Content Negotiation & Compression

### Content Negotiation
- Client gửi `Accept: application/json` — server trả về JSON.
- Server luôn trả về `Content-Type: application/json` trong response header.

### Compression
- Client gửi `Accept-Encoding: gzip` để yêu cầu nén.
- Server trả về `Content-Encoding: gzip` nếu hỗ trợ.
- Giúp giảm băng thông khi trả về danh sách 500 enrollments.

---

## 14. HATEOAS

**HATEOAS** (Hypermedia as the Engine of Application State) — client chỉ cần biết URI gốc, server trả về links để điều hướng các hành động tiếp theo.

### Ví dụ Response có HATEOAS cho Enrollment

```json
{
  "success": true,
  "message": "Enrollment retrieved successfully",
  "data": {
    "enrollmentId": 10,
    "status": "Active",
    "links": [
      { "rel": "self",    "href": "/api/enrollments/10",    "method": "GET" },
      { "rel": "student", "href": "/api/students/3",        "method": "GET" },
      { "rel": "course",  "href": "/api/courses/2",         "method": "GET" }
    ]
  },
  "errors": null
}
```

### Ví dụ Response có HATEOAS cho Collection

```json
{
  "data": {
    "items": [...],
    "pagination": {
      "page": 2,
      "pageSize": 10,
      "totalItems": 50,
      "totalPages": 5
    },
    "links": [
      { "rel": "self",  "href": "/api/students?page=2&size=10" },
      { "rel": "first", "href": "/api/students?page=1&size=10" },
      { "rel": "prev",  "href": "/api/students?page=1&size=10" },
      { "rel": "next",  "href": "/api/students?page=3&size=10" },
      { "rel": "last",  "href": "/api/students?page=5&size=10" }
    ]
  }
}
```

---

## 15. Idempotence

| Method | Idempotent? | Áp dụng trong LMS |
|--------|-------------|-------------------|
| GET | ✅ | `GET /api/students/1` — gọi 100 lần vẫn trả về cùng data |

---

## 16. Versioning

Dùng **URI Path versioning** — rõ ràng, dễ test trên Swagger.

```
/api/v1/students
/api/v1/enrollments
/api/v1/courses
```

> LAB1 chỉ cần 1 version. Tuy nhiên nên đặt `/api/v1/` ngay từ đầu để dễ mở rộng sau này mà không phá vỡ client cũ.

---

## 17. Swagger / OpenAPI

Swagger/OpenAPI là bắt buộc trong LAB1.

### Yêu cầu

- ✅ Liệt kê đầy đủ tất cả endpoints
- ✅ Cho phép test API trực tiếp
- ✅ Tài liệu hóa Response schema
- ✅ Tài liệu hóa HTTP status codes

### Cấu hình trong ASP.NET Core

```csharp
// Program.cs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LMS API",
        Version = "v1",
        Description = "Learning Management System RESTful API"
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1");
});
```

### Annotate Controller để Swagger hiển thị status codes

```csharp
/// <summary>Get student by ID</summary>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetById(int id) { ... }
```

---

## 18. Docker Deployment

### Yêu cầu

- Database chạy trong Docker Desktop.
- API chạy trong Docker container.
- Project phải có `Dockerfile` và `docker-compose.yml`.

### Dockerfile (API)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]
```

### docker-compose.yml

```yaml
version: "3.9"

services:
  db:
    image: postgres:16-alpine
    container_name: lms_db
    environment:
      POSTGRES_DB: lmsdb
      POSTGRES_USER: lmsuser
      POSTGRES_PASSWORD: 12345
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
      - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=lmsdb;Username=lmsuser;Password=12345
    ports:
      - "8080:8080"
    networks:
      - lms_network

volumes:
  pgdata:

networks:
  lms_network:
```

### Khởi chạy

```bash
docker-compose up --build
```

> Phải demo được cả API container và Database container chạy thành công cùng lúc qua Docker Compose.

---

## 19. Richardson Maturity Model – Mục tiêu LAB1

| Level | Mô tả | LAB1 |
|-------|-------|------|
| **0** | Một endpoint, chỉ POST | ❌ |
| **1** | Nhiều URI, chỉ POST | ❌ |
| **2** | Nhiều URI + đúng HTTP Methods (GET) | ✅ **Tối thiểu** |
| **3** | Level 2 + HATEOAS | ✅ **Nên đạt** |

> LAB1 yêu cầu tối thiểu **Level 2**. Thêm HATEOAS links vào response để đạt **Level 3**.

---

## 20. Evaluation Checklist

- [ ] Đúng kiến trúc 3 tầng: API / Service / Repository
- [ ] Đúng 4 model types: Entity / Business / Request / Response
- [ ] Không trả Entity Model trong API response
- [ ] Không dùng Request/Response Model trong Repository
- [ ] URI dùng danh từ số nhiều, không dùng động từ
- [ ] Đúng HTTP methods (GET)
- [ ] Đúng HTTP status codes (200/400/404/500)
- [ ] Response format nhất quán (`success`, `message`, `data`, `errors`)
- [ ] GET by ID trả đầy đủ data liên quan, trả 404 nếu không tìm thấy
- [ ] List API hỗ trợ Search, Sort, Paging, Selection, Expansion
- [ ] Pagination metadata đầy đủ (`page`, `pageSize`, `totalItems`, `totalPages`)
- [ ] Docker: Dockerfile và docker-compose.yml hoạt động
- [ ] Swagger/OpenAPI tích hợp và tài liệu hóa đầy đủ
- [ ] Seed data: 5 semesters, 50 students, 10 subjects, 20 courses, 500 enrollments

---

## 21. Out of Scope

Các tính năng sau **KHÔNG yêu cầu** trong LAB1:

| Tính năng | Lý do |
|-----------|-------|
| Authentication / Authorization | Out of scope |
| JWT Security | Out of scope |
| Advanced Validation | Out of scope |
| Global Exception Handling | Out of scope |
| Unit Testing / Integration Testing | Out of scope |

> Mặc dù Security và Validation là nguyên tắc quan trọng trong RESTful API thực tế, LAB1 tập trung vào kiến trúc và thiết kế API cơ bản trước.

---

*Tài liệu tham khảo: [restfulapi.net](https://restfulapi.net) | Roy T. Fielding (2000) | Microsoft REST API Guidelines | PRN232 LAB1 Requirements*
