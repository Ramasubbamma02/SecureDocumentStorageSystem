# SecureDocumentStorageSystem

# ?? Secure Document Storage System

A secure, user-authenticated document management API built with **ASP.NET Core Web API**, **Entity Framework Core**, and **JWT Authentication**. Supports private file uploads, version control, and secure downloads.

---

## ? Features

- ?? **JWT Authentication** (Register/Login)
- ?? **Document Upload with Versioning**
- ?? **Download Latest or Specific Revision**
- ?? **Private Access Only** – No cross-user access
- ?? **Database BLOB Storage** via EF Core
- ?? **Unit Testing** with xUnit
- ?? Built using clean architecture and SOLID principles
- ?? **Swagger** UI for testing

---

## ?? How to Run the Project

### 1. Clone Repository

```bash
1) git clone https://github.com/Ramasubbamma02/SecureDocumentStorageSystem.git
cd secure-document-storage

2. Configure Database
By default, the app uses SQLite.

If you prefer SQL Server or PostgreSQL, update appsettings.json accordingly.

3. Apply Migrations
bash
Copy
Edit
dotnet ef database update
4. Run the API
bash
Copy
Edit
dotnet run
Swagger UI: https://localhost:5001/swagger

?? API Usage (Postman / Swagger)
?? Register
POST /api/auth/register

json
Copy
Edit
{
  "username": "testuser",
  "password": "Password123!"
}
?? Login
POST /api/auth/login

Returns:

json
Copy
Edit
{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
?? Use Token for Secure Access
Add to headers in Postman or Swagger:

makefile
Copy
Edit
Authorization: Bearer <your-token>
?? Upload Document
POST /api/files/upload

Form field: file (binary file input)

Auto-increments version if file name already exists

?? Download Document
GET /api/files/sample.pdf

Retrieves latest version

GET /api/files/sample.pdf?revision=0

Retrieves specific version (e.g., first version)

?? Access Control
Each file is tied to the uploading user

No other user can view/download your files

?? How to Run Unit Tests
bash
Copy
Edit
dotnet test
Covers:

Authentication flow

File versioning logic

User-based access restrictions

