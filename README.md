# FlowDesk Task Board API

A lightweight REST API for managing tasks within projects, built with ASP.NET Core (.NET 8), Entity Framework Core, and JWT authentication.

## Tech Stack
- ASP.NET Core 8 Web API
- EF Core + SQLite (easily swappable to SQL Server)
- ASP.NET Core Identity + JWT Bearer
- AutoMapper, Serilog, Swagger/OpenAPI
- xUnit + Moq + FluentAssertions (tests)

## Prerequisites
- .NET 8 SDK
- (Optional) SQL Server if switching from SQLite

## Running Locally

1. Clone the repo and navigate into it
2. `cd FlowDesk.API && dotnet run`
3. Open https://localhost:5001/swagger
4. Register: POST /api/auth/register
5. Login: POST /api/auth/login — copy the token
6. Click "Authorize" in Swagger, enter: Bearer {token}

## Seeded Admin Account
- Email: admin@flowdesk.com
- Password: Admin@123

## Key Design Decisions
- **Clean Architecture**: Core → Infrastructure → API layers with no upward dependencies
- **Repository + Service pattern**: testable business logic, swappable data layer
- **Status state machine**: enforces valid transitions (Todo→InProgress, InProgress→Done, etc.)
- **Soft delete (archive)**: tasks and projects are never permanently deleted unless explicitly requested
- **Pagination + filtering** on all list endpoints via query parameters

## Running Tests
```
cd FlowDesk.Tests && dotnet test
```
