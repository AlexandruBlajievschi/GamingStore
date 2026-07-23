# Gaming Store Backend

The backend is a .NET Web API for the Gaming Store project.

## Intended Architecture

This project will follow a simple MVC-style structure:

- `Controllers/` - HTTP endpoints and request/response handling.
- `Services/` - business logic and application workflows.
- `Repositories/` - database access once persistence is added.
- `Models/` or feature folders - domain objects, DTOs, and validation models as the project grows.

## Current Endpoint

```text
GET /api/health
```

Returns a simple response so the frontend can confirm that the API is reachable.
