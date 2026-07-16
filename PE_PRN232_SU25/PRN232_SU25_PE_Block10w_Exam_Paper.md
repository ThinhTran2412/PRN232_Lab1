# FPT University – PRN232 Practice Exam

**Course:** PRN232 - Building Cross-Platform Back-End Application With .NET

**Duration:** 85 minutes

---

## 1. General Information

- **Environment:** Visual Studio 2019+ & SQL Server 2012+
- **Architecture:** 3-layer (Presentation, Business Logic, Data Access)
- **Database:** SU25LeopardDB
- **Entity Framework Core:** Configure DbContext via `appsettings.json` (**hardcoded connection strings will receive 0 points**)
- **Structure:**
  - `PRN231_SU25_SE<YourStudentID>.api` – API Project
  - `PRN231_SU25_SE<YourStudentID>.json` – Postman Script
- **Notes:**
  - All data operations must go through the Web API
  - No direct DB access from frontend
  - No hardcoded SQL allowed

---

## 2. Implementation Tasks

### 2.1. Authentication & Authorization (1.0 point)

- Implement login using JWT (email + password).
- **Allowed Roles:**
  - `administrator` (RoleId=5), `moderator` (RoleId=6): Full access (CRUD + search)
  - `developer` (RoleId=7), `member` (RoleId=4): Read + search only
  - Other roles: No token issued

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

### 2.2. LeopardProfile API Endpoints (4.5 points)

- **GET** `/api/LeopardProfile`
  - List all LeopardProfile items
  - Roles: administrator, moderator, developer, member
  - Status: 200, 401, 403

- **GET** `/api/LeopardProfile/{id}`
  - Get LeopardProfile by ID
  - Roles: administrator, moderator, developer, member
  - Status: 200, 404, 401, 403

- **POST** `/api/LeopardProfile`
  - Create a new LeopardProfile
  - Roles: administrator, moderator
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

  - **Validation:**
    - LeopardName: Regex `^([A-Z0-9][a-zA-Z0-9-#]*\s*[A-Z0-9][a-zA-Z0-9-#]*)$`
    - Weight > 15
  - Status: 201, 400, 401, 403

- **PUT** `/api/LeopardProfile/{id}`
  - Update an existing LeopardProfile
  - Roles: administrator, moderator
  - Status: 200, 400, 404, 401, 403

- **DELETE** `/api/LeopardProfile/{id}`
  - Delete a LeopardProfile
  - Roles: administrator, moderator
  - Status: 200, 404, 401, 403

- **GET** `/api/LeopardProfile/search?LeopardName=...&Weight=...`
  - Search LeopardProfile by CheetahName and Weight (OData supported)
  - Roles: all roles with token
  - Status: 200, 401, 403

---

### 2.3. Error Code Format (1.0 point)

Always return JSON error messages in the following format:

```json
{
  "errorCode": "HB40001",
  "message": "modelName is required"
}
```

| errorCode | HTTP Status | Meaning |
|---|---|---|
| HB40001 | 400 | Missing/invalid input |
| HB40101 | 401 | Token missing/invalid |
| HB40301 | 403 | Permission denied |
| HB40401 | 404 | Resource not found |
| HB50001 | 500 | Internal server error |

---

### 2.4. Swagger (1.0 point)

- Must expose all endpoints listed above
- Must allow JWT token insertion for testing

---

### 2.5. Postman Test Cases (2.5 points)

Write and execute at least 6 test cases using Postman:

1. Login success
2. Login failed
3. Create LeopardProfile (authorized roles only)
4. Update LeopardProfile
5. Delete LeopardProfile
6. Get list / Get by ID

**Requirements:**

- Include JWT in Header
- Use correct method/body
- Validate response status & message

---

## 3. Submission Notes

- Syntax errors, uncompiled code = 0 points
- Any hardcoded/irrelevant code = 0 points
- Naming must strictly follow instruction format

---

**Good luck!**
