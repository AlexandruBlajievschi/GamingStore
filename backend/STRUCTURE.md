# Backend Structure

The backend should stay simple until the domain becomes clearer.

## Recommended Folders

- `Controllers/` - API controllers. Keep them thin.
- `Services/` - business rules and orchestration.
- `Repositories/` - database queries and persistence logic.
- `Models/` - domain models and shared DTOs.
- `Data/` - database context, migrations, and seed data when added.

## Flow

```text
Controller -> Service -> Repository -> Database
```

Controllers should not talk directly to the database. Services should own the main business decisions. Repositories should focus on data access.

## Layer Responsibilities

Controllers:

- expose HTTP routes
- accept request DTOs
- call services
- return status codes and response DTOs
- avoid repeated exception `try/catch`; expected service/domain exceptions are mapped by middleware

Services:

- coordinate use cases
- enforce business workflow rules
- call entity factory/update methods
- check cross-entity requirements, such as a seller existing before a game is created
- decide when to call repository persistence methods

Repositories:

- own EF Core queries and persistence
- use `AsNoTracking` for read-only lists/details where tracking is not needed
- use tracking queries when an entity will be updated or deleted
- keep `Include`, ordering, filtering, add/remove, and `SaveChangesAsync` details out of services/controllers

## EF Core Tracking

Default to no-tracking queries for read-only API flows.

Examples:

- `GET /api/games` should use `AsNoTracking()`.
- `GET /api/games/{id}` should use `AsNoTracking()` when returning a DTO.
- `PUT /api/games/{id}` should load a tracked entity before applying changes.
- `DELETE /api/games/{id}` may load a tracked entity before removing it.

Repository method names should make this intent visible. In this project, `GetByIdAsync` means read-only/no-tracking, while `GetTrackedByIdAsync` means the service intends to update or delete the entity.

## Exception Handling

Use `ApiExceptionHandlingMiddleware` for expected API exception mapping.

- `DomainValidationException` maps to `400 Bad Request`.
- `ResourceNotFoundException` maps to `404 Not Found`.
- Controllers should let these exceptions bubble to middleware instead of catching them locally.
- Services and entities should throw these exceptions when domain or use-case rules fail.

## Entity Creation

Use static factory methods on entities when creating domain objects from application code.

Factories protect basic invariants before an object exists, such as:

- required text must not be empty
- email values must be valid
- prices must not be negative
- foreign key identifiers must not be empty
- string values must fit configured database limits

Keep entity constructors private unless EF Core needs them for materialization. EF seed data can use anonymous objects in configuration classes so seed rows stay explicit without reopening public setters.

## Testing

Backend features should include tests in the solution.

- `tests/GamingStore.Api.UnitTests/` covers services and controller HTTP mapping.
- `tests/GamingStore.Api.IntegrationTests/` covers repository/database behavior.
- Unit tests should fake repository interfaces rather than mocking EF Core `DbSet`.
- Integration tests should use a relational provider. SQLite in-memory is acceptable for early tests, but PostgreSQL-backed tests are preferred once test database infrastructure is available.
- Coverage is useful feedback, not the definition of test quality.
- New CRUD behavior should have strong coverage across service logic, controller success mapping, entity invariants, exception middleware mapping, and repository persistence behavior.
- Do not chase 100% for trivial DTOs, EF-only constructors, framework-generated code, or defensive branches unless they represent meaningful product risk.
- End-to-end tests should be added only once a complete frontend-to-backend feature flow exists.

Useful commands:

```bash
dotnet test /p:UseAppHost=false
dotnet test /p:UseAppHost=false --collect:"XPlat Code Coverage" --results-directory TestResults
```
