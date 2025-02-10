# Employee Management System API

A secure .NET Core 8 Web API for managing employee data with JWT authentication and Entity Framework Core.

## Features
- **Employee CRUD Operations**
  - Create, Read, Update, Delete employees
  - Soft delete (mark inactive)
- **JWT Authentication**
  - Secure token-based authentication
  - Role-based access control
- **Database Integration**
  - SQL Server with Entity Framework Core
  - Code-first migrations
- **Security**
  - SQL injection prevention
  - Request validation
  - Data encryption in transit
- **API Documentation**
  - Swagger/OpenAPI support
  - Endpoint testing UI

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Git](https://git-scm.com/downloads)

## Setup Instructions

### 1. Clone Repository
git clone https://github.com/farhanbasysyaro/EmployeeSystemAPI.git

### 2. Configure Database
### Edit appsettings.json (use appsettings.Development.json as template):
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeDB;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key-here",
    "Issuer": "EmployeeSystemAPI",
    "Audience": "EmployeeSystemClient",
    "ExpireHours": 8
  }
}

### Generate JWT Key:
### Linux/macOS
openssl rand -base64 32

### Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { [byte](Get-Random -Minimum 0 -Maximum 255) }))

### 3. Run Migrations
dotnet ef database update --project EmployeeSystem.Infrastructure --startup-project EmployeeSystem.API

### 4. Start Application
dotnet run --project EmployeeSystem.API
