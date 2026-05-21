# Tổng hợp Nguyên tắc RESTful API
> Nguồn tham khảo chính: [restfulapi.net](https://restfulapi.net)  
> Tác giả gốc: Lokesh Gupta | Dựa trên luận văn của Roy Thomas Fielding (2000)

---

## Mục lục
1. [REST là gì?](#1-rest-là-gì)
2. [6 Ràng buộc Kiến trúc REST](#2-6-ràng-buộc-kiến-trúc-rest)
3. [Đặt tên Resource (URI Naming)](#3-đặt-tên-resource-uri-naming)
4. [HTTP Methods](#4-http-methods)
5. [HTTP Status Codes](#5-http-status-codes)
6. [Statelessness (Phi trạng thái)](#6-statelessness-phi-trạng-thái)
7. [Caching (Bộ nhớ đệm)](#7-caching-bộ-nhớ-đệm)
8. [Compression (Nén dữ liệu)](#8-compression-nén-dữ-liệu)
9. [Content Negotiation](#9-content-negotiation)
10. [HATEOAS](#10-hateoas)
11. [Idempotence (Tính lũy đẳng)](#11-idempotence-tính-lũy-đẳng)
12. [Versioning (Phiên bản hóa)](#12-versioning-phiên-bản-hóa)
13. [Security (Bảo mật)](#13-security-bảo-mật)
14. [Pagination, Sorting & Filtering](#14-pagination-sorting--filtering)
15. [Rate Limiting](#15-rate-limiting)
16. [Richardson Maturity Model](#16-richardson-maturity-model)
17. [Best Practices tổng hợp](#17-best-practices-tổng-hợp)

---

## 1. REST là gì?

**REST** (Representational State Transfer) là một phong cách kiến trúc được Roy Fielding đặt ra năm 2000 trong luận văn tiến sĩ của ông. Đây **không phải là giao thức** mà là một tập hợp các ràng buộc kiến trúc để thiết kế ứng dụng kết nối mạng.

- Dữ liệu và chức năng được coi là **resource** (tài nguyên), truy cập qua **URI** (Uniform Resource Identifiers).
- Client và Server trao đổi **representations** của resource thông qua giao diện và giao thức chuẩn hóa (thường là HTTP).
- Mọi tương tác với server phải là **stateless** (phi trạng thái).
- REST và HTTP **không phải là một** — REST có thể hoạt động trên các giao thức khác, nhưng HTTP là phổ biến nhất.

---

## 2. 6 Ràng buộc Kiến trúc REST

Đây là **cốt lõi** của REST. Một web service chỉ được gọi là "thực sự RESTful" khi tuân thủ đầy đủ 6 ràng buộc này.

### 2.1 Uniform Interface (Giao diện Thống nhất)
Đây là ràng buộc quan trọng nhất, bao gồm 4 yếu tố con:

| Yếu tố | Mô tả |
|--------|-------|
| **Identification of resources** | Mỗi resource phải có một URI duy nhất để định danh |
| **Manipulation through representations** | Client dùng representation của resource để thay đổi trạng thái trên server |
| **Self-descriptive messages** | Mỗi message phải chứa đủ thông tin để mô tả cách xử lý nó |
| **HATEOAS** | Client chỉ cần biết URI gốc; server trả về hyperlink để điều hướng các hành động tiếp theo |

> Một resource chỉ nên có **một URI logic duy nhất**. Mọi resource nên có link (HATEOAS) trỏ đến các URI liên quan. Các representation trong toàn hệ thống phải tuân theo naming convention, link format và data format nhất quán.

### 2.2 Client–Server (Phân tách Client và Server)
- Client và Server phải có khả năng **phát triển độc lập** mà không phụ thuộc vào nhau.
- Client chỉ cần biết **URI của resource** — không cần biết gì thêm về server.
- Server và client có thể được thay thế hoặc phát triển riêng biệt, miễn là giao diện giữa chúng không thay đổi.
- Giúp cải thiện **tính di động** của UI trên nhiều nền tảng và **khả năng mở rộng** của server.

### 2.3 Stateless (Phi trạng thái)
- Server **không lưu trữ** bất kỳ thông tin nào về các HTTP request trước đó của client.
- Mỗi request đều được xử lý như một **request mới hoàn toàn** — không có session, không có history.
- Mọi thông tin cần thiết (bao gồm cả xác thực) **phải được gửi kèm trong từng request**.
- Giúp tăng khả năng **scale, cache và quản lý** dịch vụ.

### 2.4 Cacheable (Có thể Cache)
- Response phải tự **đánh dấu** là cacheable hoặc non-cacheable.
- Cache được áp dụng khi phù hợp — có thể triển khai ở phía client hoặc server.
- Cache được quản lý tốt sẽ **giảm bớt hoặc loại bỏ** một số tương tác client-server, cải thiện hiệu suất và khả năng mở rộng.

### 2.5 Layered System (Hệ thống Phân tầng)
- Client **không thể biết** mình đang kết nối trực tiếp với server đích hay qua một trung gian (proxy, load balancer, gateway...).
- Ví dụ: API deploy trên Server A, dữ liệu lưu ở Server B, xác thực ở Server C — client không cần quan tâm.
- Cho phép triển khai các **gateway bảo mật, bộ nhớ đệm, load balancer** một cách trong suốt.

### 2.6 Code on Demand *(Tùy chọn)*
- Server có thể trả về **code thực thi** (ví dụ: JavaScript) để client chạy.
- Đây là ràng buộc **duy nhất tùy chọn** trong 6 ràng buộc.
- Ví dụ: trả về widget rendering code để client hiển thị UI động.

---

## 3. Đặt tên Resource (URI Naming)

### 3.1 Nguyên tắc cơ bản
- URI phải trỏ đến **resource** (danh từ), **không phải hành động** (động từ).
- Resource là "thing" (đối tượng), không phải "action" (hành động).
- Mỗi resource nên có **một URI logic duy nhất**.

```
✅ Đúng:  /users
✅ Đúng:  /users/{id}
✅ Đúng:  /device-management/managed-devices
❌ Sai:   /getUsers
❌ Sai:   /createUser
❌ Sai:   /deleteUser/{id}
```

### 3.2 Quy tắc đặt tên URI

| Quy tắc | Ví dụ |
|---------|-------|
| Dùng **danh từ số nhiều** cho collection | `/users`, `/orders`, `/devices` |
| Dùng **chữ thường** (lowercase) | `/user-management` ✅ |
| Dùng **dấu gạch ngang** (-) thay dấu gạch dưới (_) | `/device-management` ✅ |
| **Không dùng** extension file trong URI | `/users` ✅, không phải `/users.json` ❌ |
| Dùng **query parameter** cho filter/sort | `/users?role=admin&sort=name` |
| **Không dùng trailing slash** (/) ở cuối | `/users` ✅, không phải `/users/` ❌ |

### 3.3 Hierarchy Resource
```
/device-management/managed-devices          → Collection
/device-management/managed-devices/{id}     → Singular resource
/device-management/managed-devices/{id}/configurations  → Sub-resource collection
```

### 3.4 Không nhúng động từ vào URI
Khi cần thực thi một hành động, hãy **tạo resource trạng thái** thay vì dùng động từ:
```
❌ Sai:   POST /scripts/{id}/execute
✅ Đúng:  POST /scripts/{id}/status   (với body: action=execute)
```

---

## 4. HTTP Methods

### 4.1 Bảng tổng hợp

| Method | Mục đích | Safe? | Idempotent? | Status thành công |
|--------|----------|-------|-------------|-------------------|
| **GET** | Lấy resource | ✅ | ✅ | 200 OK |
| **POST** | Tạo resource mới | ❌ | ❌ | 201 Created |
| **PUT** | Thay thế toàn bộ resource | ❌ | ✅ | 200 OK / 204 No Content |
| **PATCH** | Cập nhật một phần resource | ❌ | ❌ | 200 OK |
| **DELETE** | Xóa resource | ❌ | ✅ | 200 OK / 202 / 204 |

### 4.2 Chi tiết từng method

**GET**
- Chỉ dùng để **đọc** — không thay đổi trạng thái resource.
- Là safe method và idempotent.
- Nhiều request GET giống nhau phải trả về **cùng một kết quả**.

**POST**
- Dùng để **tạo resource mới** trong một collection.
- **Không safe, không idempotent** — hai POST giống nhau tạo ra hai resource khác nhau.
- Response nên trả về `201 Created` và header `Location` trỏ đến resource mới.

**PUT**
- Dùng để **thay thế hoàn toàn** một resource hiện có.
- Là **idempotent** — nhiều PUT giống nhau cho kết quả giống nhau.
- Khác POST: PUT biết trước URI của resource; POST để server tự quyết định.

**PATCH**
- Dùng để **cập nhật một phần** resource (áp dụng delta, không thay toàn bộ).
- Không idempotent theo chuẩn, nhưng có thể thiết kế để idempotent.

**DELETE**
- Dùng để **xóa** resource.
- Là **idempotent** — DELETE lần 2 trên resource đã xóa trả về `404 Not Found`.
- Response không cacheable.

### 4.3 Safe vs Idempotent

- **Safe methods**: GET, HEAD, OPTIONS, TRACE — không thay đổi trạng thái server.
- **Idempotent methods**: GET, PUT, DELETE — nhiều request giống nhau = một request.

---

## 5. HTTP Status Codes

| Nhóm | Ý nghĩa | Ví dụ phổ biến |
|------|---------|----------------|
| **2xx** | Thành công | 200 OK, 201 Created, 202 Accepted, 204 No Content |
| **3xx** | Redirect | 301 Moved Permanently, 304 Not Modified |
| **4xx** | Lỗi client | 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found, 409 Conflict, 429 Too Many Requests |
| **5xx** | Lỗi server | 500 Internal Server Error, 503 Service Unavailable |

**Nguyên tắc:** Luôn sử dụng **đúng status code** theo ngữ nghĩa — không bao giờ trả về `200 OK` cho một lỗi.

---

## 6. Statelessness (Phi trạng thái)

- Mỗi HTTP request phải **độc lập hoàn toàn** — server không nhớ request trước đó.
- Server không lưu **session state** của client.
- Client phải gửi **mọi thông tin cần thiết** trong mỗi request (bao gồm authentication token, credentials, context...).
- **Application state** (trạng thái ứng dụng) được giữ hoàn toàn ở phía **client**.
- **Resource state** (trạng thái resource) được lưu ở phía **server**.

### Lợi ích của Statelessness
- **Scalability**: Bất kỳ server nào cũng xử lý được bất kỳ request nào — không phụ thuộc session.
- **Simplicity**: Giảm phức tạp vì không cần đồng bộ trạng thái giữa các server.
- **Cacheability**: Dễ cache vì mỗi request tự mô tả hoàn toàn.
- **Reliability**: Hệ thống phân tán dễ phục hồi hơn.

---

## 7. Caching (Bộ nhớ đệm)

- Response phải khai báo rõ là **cacheable** hay **non-cacheable** thông qua HTTP headers.
- Cache có thể triển khai ở **client side, server side, hoặc trung gian** (CDN, proxy).
- Cache tốt giúp **giảm tải server** và **tăng hiệu suất** cho client.

### Headers quan trọng

| Header | Mô tả |
|--------|-------|
| `Cache-Control` | Quy định chính sách cache (max-age, no-cache, no-store...) |
| `ETag` | Token định danh phiên bản resource — dùng để validate cache |
| `Last-Modified` | Thời điểm resource được sửa đổi lần cuối |
| `Expires` | Thời điểm hết hạn cache |

---

## 8. Compression (Nén dữ liệu)

- Client gửi header `Accept-Encoding` để báo server biết các thuật toán nén nó hỗ trợ (ví dụ: `gzip`, `deflate`).
- Server dùng thuật toán phù hợp và trả về header `Content-Encoding` để thông báo.
- Nếu server không hỗ trợ encoding được yêu cầu → trả về `415 Unsupported Media Type`.
- Compression **tiết kiệm băng thông** đáng kể với chi phí xử lý tối thiểu.

---

## 9. Content Negotiation

Cơ chế cho phép client và server **thỏa thuận** định dạng dữ liệu phù hợp nhất.

| Header (Request) | Mục đích |
|------------------|---------|
| `Accept` | Loại media client muốn nhận (JSON, XML, HTML...) |
| `Accept-Language` | Ngôn ngữ client muốn nhận |
| `Accept-Encoding` | Encoding/compression client hỗ trợ |
| `Accept-Charset` | Charset client muốn |

| Header (Response) | Mục đích |
|-------------------|---------|
| `Content-Type` | Loại media server trả về |
| `Content-Language` | Ngôn ngữ của response |
| `Content-Encoding` | Encoding được áp dụng cho response |

---

## 10. HATEOAS

**HATEOAS** (Hypermedia as the Engine of Application State) là một ràng buộc của Uniform Interface.

### Nguyên tắc
- Client chỉ cần biết **URI gốc** của ứng dụng.
- Tất cả các URI và hành động khác được **khám phá động** thông qua hyperlinks trong response.
- Server trả về các link điều hướng tất cả các tương tác tiếp theo.

### Ví dụ Response có HATEOAS
```json
{
  "id": 1,
  "name": "Nguyen Van A",
  "links": [
    { "rel": "self",   "href": "/users/1",        "method": "GET" },
    { "rel": "update", "href": "/users/1",        "method": "PUT" },
    { "rel": "delete", "href": "/users/1",        "method": "DELETE" },
    { "rel": "orders", "href": "/users/1/orders", "method": "GET" }
  ]
}
```

### Richardson Maturity Model & HATEOAS
Theo Roy Fielding, **Level 3 (HATEOAS) là điều kiện tiên quyết** của REST thực sự.

---

## 11. Idempotence (Tính lũy đẳng)

> Một method là **idempotent** nếu nhiều request giống nhau có cùng tác động như một request duy nhất.

| Method | Idempotent? | Giải thích |
|--------|-------------|-----------|
| GET | ✅ | Chỉ đọc, không thay đổi |
| PUT | ✅ | Ghi đè resource — cùng data = cùng kết quả |
| DELETE | ✅ | Xóa lần 1 → gone; xóa lần 2 → 404; **tác động trên resource** như nhau |
| POST | ❌ | Mỗi POST tạo một resource mới |
| PATCH | ❌ | Phụ thuộc thiết kế |

> Một method là **safe** nếu nó **không thay đổi trạng thái** resource trên server (GET, HEAD, OPTIONS, TRACE).

---

## 12. Versioning (Phiên bản hóa)

API không bao giờ hoàn toàn ổn định — versioning giúp quản lý thay đổi mà không phá vỡ client hiện tại.

### 4 cách Version phổ biến

| Phương pháp | Ví dụ | Ưu điểm |
|-------------|-------|---------|
| **URI Path** | `/api/v1/users` | Rõ ràng, dễ test trên browser |
| **Query Parameter** | `/api/users?version=1` | Không thay đổi URI structure |
| **Custom Header** | `X-API-Version: 1` | URI sạch hơn |
| **Accept Header** (Content Negotiation) | `Accept: application/vnd.example.v1+json` | Chuẩn HTTP nhất |

### Nguyên tắc
- Luôn **duy trì backward compatibility** khi có thể.
- Deprecation nên được **thông báo trước** và có tài liệu đầy đủ.
- Xóa version cũ phải thực hiện **từ từ, có lộ trình**.

---

## 13. Security (Bảo mật)

### Xác thực & Phân quyền

| Cơ chế | Mô tả |
|--------|-------|
| **OAuth2** | Tiêu chuẩn ủy quyền mạnh nhất cho public API |
| **JWT (JSON Web Token)** | Token tự chứa thông tin, compact, phổ biến |
| **API Keys** | Đơn giản, dùng cho server-to-server |

### Các nguyên tắc bảo mật cốt lõi

- ✅ **Luôn dùng HTTPS** để mã hóa dữ liệu truyền tải — tránh man-in-the-middle attack.
- ✅ **Stateless authentication**: Gửi credentials/token trong mỗi request (không dùng server session).
- ✅ **Bearer token** trong `Authorization` header — không đặt token trong URL.
- ✅ **Least Privilege**: Chỉ cấp quyền tối thiểu cần thiết cho client.
- ✅ **RBAC** (Role-Based Access Control): Phân quyền theo role.
- ✅ **Validate & sanitize** mọi input — phòng chống SQL injection, XSS.
- ✅ **Mã hóa** dữ liệu nhạy cảm và che giấu thông tin cá nhân.
- ✅ Không để lộ **chi tiết lỗi nội bộ** trong response.

---

## 14. Pagination, Sorting & Filtering

Khi collection có kích thước lớn, phải hỗ trợ phân trang để tránh quá tải.

### Pagination

```
GET /users?page=1&size=20
GET /devices?startIndex=0&size=20
```

### Sorting

```
GET /users?sort=name&order=asc
GET /orders?sort=createdAt&order=desc
```

### Filtering

```
GET /users?role=admin&status=active
GET /products?category=electronics&price_max=500
```

### Best practices
- Trả về metadata trong response: `total`, `page`, `size`, `totalPages`.
- Dùng **cursor-based pagination** cho dữ liệu thời gian thực.
- Kết hợp HATEOAS: cung cấp link `next`, `prev`, `first`, `last` trong response.

---

## 15. Rate Limiting

Cơ chế giới hạn số lượng request client có thể thực hiện trong một khoảng thời gian nhất định.

### Mục đích
- Bảo vệ server khỏi **DoS/DDoS, brute force, data scraping**.
- Đảm bảo **công bằng** giữa các client.
- Quản lý **chi phí** với paid API.

### HTTP Headers

| Header | Mô tả |
|--------|-------|
| `X-RateLimit-Limit` | Giới hạn request tối đa |
| `X-RateLimit-Remaining` | Số request còn lại |
| `X-RateLimit-Reset` | Thời điểm reset limit (Unix timestamp) |
| `Retry-After` | Thời gian chờ trước khi retry |

### Response khi vượt giới hạn
```
HTTP/1.1 429 Too Many Requests
Retry-After: 60
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 0
```

---

## 16. Richardson Maturity Model

Mô hình đánh giá mức độ "thuần REST" của một web service, dựa trên 3 yếu tố: **URIs, HTTP Methods, HATEOAS**.

| Level | Tên | URI | HTTP Methods | HATEOAS |
|-------|-----|-----|--------------|---------|
| **0** | The Swamp of POX | ❌ Một endpoint | ❌ Chỉ POST | ❌ |
| **1** | Resources | ✅ Nhiều URI | ❌ Chỉ POST | ❌ |
| **2** | HTTP Verbs | ✅ | ✅ GET/POST/PUT/DELETE | ❌ |
| **3** | Hypermedia Controls | ✅ | ✅ | ✅ |

> **Level 2** là mức phổ biến nhất trong thực tế.  
> **Level 3** là điều kiện tiên quyết của REST thực sự theo Roy Fielding.  
> Ở Level 3, service **tự mô tả** và **dễ khám phá** — client không cần tài liệu bên ngoài.

---

## 17. Best Practices tổng hợp

### Thiết kế URI
- ✅ Dùng danh từ số nhiều: `/users`, `/orders`
- ✅ Phân cấp resource hợp lý: `/users/{id}/orders`
- ✅ Chữ thường, dùng gạch ngang: `/user-profiles`
- ❌ Không dùng động từ trong URI: `/getUser`, `/createOrder`
- ❌ Không dùng trailing slash, không dùng extension file

### HTTP Methods & Status Codes
- ✅ Dùng đúng method theo ngữ nghĩa (GET đọc, POST tạo, PUT/PATCH sửa, DELETE xóa)
- ✅ Trả về đúng status code (201 khi tạo, 404 khi không tìm thấy, 400 khi input sai)
- ✅ Response body nhất quán và tự mô tả

### Thiết kế Response
- ✅ Dùng JSON làm định dạng mặc định
- ✅ Cấu trúc response nhất quán trên toàn API
- ✅ Trả về thông báo lỗi có ý nghĩa, không lộ thông tin nội bộ
- ✅ Thêm HATEOAS links khi có thể

### Hiệu suất & Khả năng mở rộng
- ✅ Thiết kế stateless để dễ scale ngang
- ✅ Implement caching với headers phù hợp
- ✅ Dùng compression (gzip) để giảm băng thông
- ✅ Hỗ trợ pagination cho mọi collection lớn

### Bảo mật
- ✅ Luôn dùng HTTPS
- ✅ Xác thực mọi request (JWT/OAuth2)
- ✅ Validate và sanitize mọi input
- ✅ Rate limiting để chống abuse

### Bảo trì & Tiến hóa
- ✅ Version API ngay từ đầu
- ✅ Tài liệu hóa đầy đủ (OpenAPI/Swagger)
- ✅ Deprecation có lộ trình rõ ràng
- ✅ Backward compatibility là ưu tiên hàng đầu

---

## Thiết kế REST API theo quy trình

Theo restfulapi.net, quy trình thiết kế một REST API gồm 4 bước:

```
1. Identify Object Model
   → Xác định các đối tượng sẽ được biểu diễn thành resource

2. Create Resource URIs
   → Tạo URI cho từng resource, tuân thủ naming conventions

3. Determine Representations
   → Quyết định định dạng data (JSON/XML) và cấu trúc payload

4. Assign HTTP Methods
   → Map các hành động CRUD vào đúng HTTP method
```

---

*Tổng hợp từ [restfulapi.net](https://restfulapi.net) — Luận văn gốc: Roy T. Fielding, "Architectural Styles and the Design of Network-based Software Architectures" (2000)*
