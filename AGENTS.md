# Agent Instructions

## Commit Messages

Use Conventional Commits for every commit message prepared in this project.

Format:

```text
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

The description must be short, written in the imperative mood, and start with a lowercase letter unless it begins with a proper noun.

Preferred types:

- `feat`: a new feature
- `fix`: a bug fix
- `docs`: documentation-only changes
- `style`: formatting or styling changes that do not affect behavior
- `refactor`: code changes that neither fix a bug nor add a feature
- `perf`: performance improvements
- `test`: adding or changing tests
- `build`: build system, dependencies, or project configuration
- `ci`: continuous integration changes
- `chore`: maintenance work that does not fit another type

Preferred scopes:

- `root`: repository-level files
- `backend`: backend-wide changes
- `frontend`: frontend-wide changes
- `api`: API controllers, services, repositories, or contracts
- `data`: EF Core, entities, migrations, database configuration, or seed data
- `docs`: documentation structure or content
- `auth`: authentication and authorization

Examples:

```text
feat(frontend): add health check panel
feat(data): add initial game store entities
fix(api): return seeded games from database
docs(root): document project structure
build(backend): add PostgreSQL EF provider
```

Breaking changes must be marked with `!` after the type or scope, or with a `BREAKING CHANGE:` footer.

Examples:

```text
feat(api)!: replace game response contract

BREAKING CHANGE: game responses now return seller details in a nested object.
```

When a change covers multiple unrelated purposes, prefer separate commits instead of one broad commit.
