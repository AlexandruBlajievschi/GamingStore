# Gaming Store Backend

The backend is a .NET Web API for the Gaming Store project.

## Intended Architecture

This project will follow a simple MVC-style structure:

- `Controllers/` - HTTP endpoints and request/response handling.
- `Services/` - business logic and application workflows.
- `Repositories/` - database access once persistence is added.
- `Models/` or feature folders - domain objects, DTOs, and validation models as the project grows.
- `tests/` - unit and integration tests for backend behavior.

## Current Endpoints

```text
GET /api/health
GET /api/games
GET /api/games/{id}
POST /api/games
PUT /api/games/{id}
DELETE /api/games/{id}
GET /api/sellers
GET /api/sellers/{id}
POST /api/sellers
PUT /api/sellers/{id}
DELETE /api/sellers/{id}
GET /api/users
GET /api/users/{id}
POST /api/users
PUT /api/users/{id}
DELETE /api/users/{id}
```

The health endpoint lets the frontend confirm that the API is reachable. Games, sellers, and users provide the first full CRUD API surfaces.

## Testing

Run backend tests from the `backend/` folder:

```bash
dotnet test /p:UseAppHost=false
```

Run tests with coverage:

```bash
dotnet test /p:UseAppHost=false --collect:"XPlat Code Coverage" --results-directory TestResults
```

Unit tests cover service behavior, controller response mapping, entity invariants, and exception middleware mapping. Integration tests cover repository/database behavior.

Coverage is treated as feedback, not the goal by itself. The project aims for strong coverage of business rules and CRUD behavior without chasing 100% for trivial or framework-only code.
