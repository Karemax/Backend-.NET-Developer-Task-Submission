# Submission Notes

## Included Deliverables

- `README.md` — updated repository documentation with setup, architecture, and API guidance.
- `postman_collection.json` — ready-made Postman collection for API endpoints and authentication flows.
- `src/Project.Infrastructure/Migrations` — EF Core migration files for database schema.
- `src/Project.API` — ASP.NET Core Web API with Swagger support.
- `src/Project.Infrastructure` — data access, JWT auth, and in-memory caching.
- `tests/Project.UnitTests` — unit tests for validation and business logic.

## Build & Run Status

- Verified build with:

```powershell
dotnet build Project.sln
```

- The API is configured to run from:

```powershell
dotnet run --project src\Project.API\Project.API.csproj
```

- Swagger documentation is available at `/swagger` when the API is running.

## Notes for Reviewers

- Redis has been removed from runtime requirements; the project now uses `IMemoryCache` for caching.
- Authentication is JWT-based and claims-based user identification is used throughout controller authorization.
- The solution is structured in Clean Architecture layers.
- Please ensure the repository is published to GitHub and provide access permissions if the repo is private.

## Recommended Validation Steps

1. Restore packages:

```powershell
dotnet restore
```

2. Clean and build solution:

```powershell
dotnet clean Project.sln
dotnet build Project.sln
```

3. Run API and verify Swagger at `/swagger`.
4. Apply migrations if using SQL Server:

```powershell
dotnet ef database update --project src\Project.Infrastructure\Project.Infrastructure.csproj
```

5. Use `postman_collection.json` to exercise authentication, projects, and tasks endpoints.
