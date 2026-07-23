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
