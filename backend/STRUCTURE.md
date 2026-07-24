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

## Entity Creation

Use static factory methods on entities when creating domain objects from application code.

Factories protect basic invariants before an object exists, such as:

- required text must not be empty
- email values must be valid
- prices must not be negative
- foreign key identifiers must not be empty
- string values must fit configured database limits

Keep entity constructors private unless EF Core needs them for materialization. EF seed data can use anonymous objects in configuration classes so seed rows stay explicit without reopening public setters.
