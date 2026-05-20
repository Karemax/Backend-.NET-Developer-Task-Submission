# Project & Task Management API

A .NET 9 Clean Architecture backend for managing users, projects, and tasks.

## Submission Package

- **GitHub Repository**: this repository contains the complete solution.
- **README File**: this file documents setup, architecture, and running instructions.
- **Database Migration Files**: located under `src/Project.Infrastructure/Migrations`.
- **Swagger Documentation**: available at `/swagger` when running the API.
- **Postman Collection**: included at `postman_collection.json`.
- **Important Notes**: included in `SUBMISSION.md`.

## Overview

This repository implements a layered REST API for project and task management.
It follows Clean Architecture principles and uses CQRS with MediatR for command/query separation.

### Solution Layers

- `src/Project.API` — ASP.NET Core Web API entry point, controllers, middleware, and startup composition.
- `src/Project.Application` — commands, queries, MediatR handlers, DTOs, validation rules, and application interfaces.
- `src/Project.Domain` — domain entities, enums, primitives, and business rules.
- `src/Project.Infrastructure` — EF Core data access, repositories, JWT authentication, in-memory caching, and concrete implementations.
- `tests/Project.UnitTests` — unit tests for domain and application behavior.

## Architecture Overview

### Core Principles

- **Clean Architecture**: inner layers do not depend on outer layers.
- **Dependency Rule**: `Project.API -> Project.Application -> Project.Domain` and `Project.Infrastructure -> Project.Application -> Project.Domain`.
- **CQRS + MediatR**: commands handle writes, queries handle reads.
- **FluentValidation**: validates requests before handlers execute.
- **JWT Authentication**: protects endpoints and derives the executing user from token claims.
- **In-memory Caching**: selected read queries use `IMemoryCache`.
- **Global Error Handling**: standardized `ProblemDetails` responses for validation and runtime errors.

### Request Flow

1. Client sends an HTTP request.
2. Controller maps request data to a command or query.
3. MediatR dispatches the request.
4. FluentValidation validates the payload.
5. The handler executes business logic.
6. Infrastructure reads/writes data and cache.
7. Controller returns a standardized response.

## Current Features

- JWT-based authentication with bearer tokens.
- Owner-based authorization checks for project/task mutation.
- Project and task CRUD operations.
- API documentation via Swagger.
- Global exception handling with `ProblemDetails` output.
- In-memory caching support for selected read queries.

## Requirements

- .NET 9 SDK
- SQL Server or LocalDB for persistence

## Local Setup

1. Clone the repository.
2. Restore NuGet packages:

```powershell
dotnet restore
```

3. Clean build outputs (optional but recommended):

```powershell
dotnet clean Project.sln
```

4. Configure `src/Project.API/appsettings.json` or use environment variables.

5. Run the API:

```powershell
dotnet run --project src\Project.API\Project.API.csproj
```

5. Open Swagger in Development mode:

```text
https://localhost:<port>/swagger
```

6. Use `postman_collection.json` as a ready-made Postman collection for API testing.

## Configuration

### Database

Update `src/Project.API/appsettings.json` with your SQL Server connection string.

```json
"ConnectionStrings": {
  "Database": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### JWT Settings

```json
"Jwt": {
  "Issuer": "ProjectApi",
  "Audience": "ProjectApiUsers",
  "SecretKey": "SuperSecretKeyThatIsAtLeast32BytesLongForSecurityPurposes123!",
  "ExpirationInMinutes": 60
}
```

## Testing

Run unit tests from the repository root:

```powershell
dotnet test tests\Project.UnitTests\Project.UnitTests.csproj
```

## Notes

- The solution currently contains unit tests only.
- Integration tests and Docker deployment artifacts are not included in the current state.
- API versioning and policy-based role authorization are not yet configured.

