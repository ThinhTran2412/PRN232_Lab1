# FPT University – Đề Thi Thực Hành PRN232

**Môn học:** PRN232 - Xây Dựng Ứng Dụng Back-End Đa Nền Tảng Với .NET

**Thời gian làm bài:** 85 phút

---

## 1. Thông Tin Chung

- **Môi trường:** Visual Studio 2019+ & SQL Server 2012+
- **Kiến trúc:** 3 lớp (Presentation, Business Logic, Data Access)
- **Cơ sở dữ liệu:** SU25LeopardDB
- **Entity Framework Core:** Cấu hình DbContext thông qua `appsettings.json` (**hardcode chuỗi kết nối sẽ bị 0 điểm**)
- **Cấu trúc:**
  - `PRN231_SU25_SE<MãSinhViên>.api` – Dự án API
  - `PRN231_SU25_SE<MãSinhViên>.json` – Postman Script
- **Lưu ý:**
  - Mọi thao tác dữ liệu phải thông qua Web API
  - Không được truy cập trực tiếp DB từ frontend
  - Không được hardcode SQL

---

## 2. Các Yêu Cầu Thực Hiện

### 2.1. Xác Thực & Phân Quyền (1.0 điểm)

- Thực hiện đăng nhập bằng JWT (email + mật khẩu).
- **Các vai trò được phép:**
  - `administrator` (RoleId=5), `moderator` (RoleId=6): Toàn quyền (CRUD + tìm kiếm)
  - `developer` (RoleId=7), `member` (RoleId=4): Chỉ đọc + tìm kiếm
  - Các vai trò khác: Không cấp token

**Endpoint:**
```
POST /api/auth
```

**Request:**
```json
{
  "email": "administrator@leopard.com",
  "password": "@1"
}
```

**Response:**
```json
{
  "token": "<JWT token>",
  "role": "5"
}
```

---

### 2.2. Các Endpoint API LeopardProfile (4.5 điểm)

- **GET** `/api/LeopardProfile`
  - Liệt kê tất cả các LeopardProfile
  - Vai trò: administrator, moderator, developer, member
  - Trạng thái: 200, 401, 403

- **GET** `/api/LeopardProfile/{id}`
  - Lấy LeopardProfile theo ID
  - Vai trò: administrator, moderator, developer, member
  - Trạng thái: 200, 404, 401, 403

- **POST** `/api/LeopardProfile`
  - Tạo mới một LeopardProfile
  - Vai trò: administrator, moderator
  - Body:

```json
{
  "LeopardProfileId": 0,
  "LeopardTypeId": 1,
  "LeopardName": "Panthera tigris tigris",
  "Weight": 35,
  "Characteristics": "The leopard possesses a tawny or rusty yellow-colored coat with close-set rosettes and dark spots",
  "CareNeeds": "These animals are classified as endangered by the IUCN",
  "ModifiedDate": "2025-06-20T00:00:00"
}
```

  - **Kiểm tra hợp lệ (Validation):**
    - LeopardName: Regex `^([A-Z0-9][a-zA-Z0-9-#]*\s*[A-Z0-9][a-zA-Z0-9-#]*)$`
    - Weight > 15
  - Trạng thái: 201, 400, 401, 403

- **PUT** `/api/LeopardProfile/{id}`
  - Cập nhật một LeopardProfile hiện có
  - Vai trò: administrator, moderator
  - Trạng thái: 200, 400, 404, 401, 403

- **DELETE** `/api/LeopardProfile/{id}`
  - Xóa một LeopardProfile
  - Vai trò: administrator, moderator
  - Trạng thái: 200, 404, 401, 403

- **GET** `/api/LeopardProfile/search?LeopardName=...&Weight=...`
  - Tìm kiếm LeopardProfile theo CheetahName và Weight (hỗ trợ OData)
  - Vai trò: tất cả vai trò có token
  - Trạng thái: 200, 401, 403

---

### 2.3. Định Dạng Mã Lỗi (1.0 điểm)

Luôn trả về thông báo lỗi dạng JSON theo định dạng sau:

```json
{
  "errorCode": "HB40001",
  "message": "modelName is required"
}
```

| errorCode | HTTP Status | Ý nghĩa |
|---|---|---|
| HB40001 | 400 | Thiếu/không hợp lệ dữ liệu đầu vào |
| HB40101 | 401 | Token bị thiếu/không hợp lệ |
| HB40301 | 403 | Không đủ quyền truy cập |
| HB40401 | 404 | Không tìm thấy tài nguyên |
| HB50001 | 500 | Lỗi server nội bộ |

---

### 2.4. Swagger (1.0 điểm)

- Phải hiển thị tất cả các endpoint đã liệt kê ở trên
- Phải cho phép nhập JWT token để kiểm thử

---

### 2.5. Các Test Case Postman (2.5 điểm)

Viết và thực thi ít nhất 6 test case bằng Postman:

1. Đăng nhập thành công
2. Đăng nhập thất bại
3. Tạo LeopardProfile (chỉ với vai trò được phép)
4. Cập nhật LeopardProfile
5. Xóa LeopardProfile
6. Lấy danh sách / Lấy theo ID

**Yêu cầu:**

- Đính kèm JWT trong Header
- Sử dụng đúng phương thức/body
- Kiểm tra đúng trạng thái phản hồi & thông báo

---

## 3. Lưu Ý Khi Nộp Bài

- Lỗi cú pháp, code không biên dịch được = 0 điểm
- Bất kỳ đoạn code hardcode/không liên quan = 0 điểm
- Đặt tên phải tuân thủ đúng định dạng theo hướng dẫn

---

**Chúc bạn làm bài tốt!**
