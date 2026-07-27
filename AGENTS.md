# Agent Instructions

## Project Direction

Gaming Store is a learning-friendly full-stack e-commerce project for selling games.

Current frontend state:

- The repository currently contains a Vite + React + TypeScript frontend.
- The intended public-release frontend direction is Next.js App Router + React + TypeScript so the storefront can build strong SEO foundations early.

When planning larger frontend changes, prefer the intended Next.js direction unless the task is clearly about maintaining the current Vite app.

## Frontend Stack Decisions

Use both Next.js and React.

Next.js is the application framework. It provides routing, layouts, rendering strategies, metadata, image optimization, and other production web features.

React is the UI library used inside Next.js. Components, hooks, state, props, and JSX are still React concepts. In practical terms, the production frontend should be understood as:

```text
Next.js App Router + React + TypeScript
```

Next.js is not a replacement for React; it is a framework built on React.

## SEO Foundations

SEO is a core requirement for the public storefront.

Prefer Server Components and server-rendered content for SEO-critical pages and data, including:

- Home page content
- Product listings
- Product detail pages
- Categories and brands
- Search result pages
- Product titles, descriptions, prices, availability, images, reviews, and breadcrumbs
- Page metadata and structured data

Google and other crawlers should be able to understand important public pages before client-side JavaScript runs.

Avoid fetching SEO-critical public content only inside `useEffect`. Use client-side fetching for browser-only behavior, personalization, or interactions that do not need to define the initial searchable page content.

## Next.js Rendering Rules

Default to Server Components.

Use Client Components only when browser interactivity is required, such as:

- Add to cart
- Wishlist button
- Quantity selector
- Product gallery controls
- Search filters
- Login/register form behavior
- Checkout form behavior
- Cart drawer
- Theme switch
- Browser storage

Keep `"use client"` boundaries as small as reasonably possible. Do not mark an entire page as a Client Component when only one nested control needs interactivity.

Server Components may import Client Components. Client Components should not directly import Server Components.

Keep backend business logic in the ASP.NET Core API. The frontend should focus on rendering, routing, SEO, user interaction, and calling backend endpoints.

## Tailwind CSS

Use Tailwind CSS for frontend styling when building or migrating the production frontend.

Tailwind is preferred because it works well with Next.js, supports fast responsive UI development, encourages consistent design tokens, and reduces scattered custom CSS.

Tailwind does not replace CSS knowledge. Use semantic HTML, accessible markup, responsive layout primitives, and clear component structure. Reach for custom CSS only when Tailwind utilities become awkward or the styling belongs in a reusable base layer.

## Usability Heuristics

Use Nielsen's 10 usability heuristics as a frontend design and review lens. They are not a replacement for user research, accessibility checks, or tests, but they should guide everyday interface decisions:

- Visibility of system status: keep users informed about loading, empty, success, and error states.
- Match between the system and the real world: use familiar storefront language, prices, product names, and actions.
- User control and freedom: make navigation, cancellation, clearing inputs, and recovery paths obvious.
- Consistency and standards: follow platform, browser, and e-commerce conventions unless there is a strong reason not to.
- Error prevention: prefer constraints, validation, and clear affordances before users make mistakes.
- Recognition rather than recall: expose useful labels, hints, visible options, and remembered context instead of relying on memory.
- Flexibility and efficiency of use: support efficient repeated use with search, keyboard behavior, sensible defaults, and fast paths.
- Aesthetic and minimalist design: keep pages focused on the current task and remove diagnostic or decorative clutter when it stops helping.
- Help users recognize, diagnose, and recover from errors: write specific, plain-language messages and preserve the user's path forward.
- Help and documentation: document product behavior and project conventions where future contributors will need them.

When reviewing or building a user-facing frontend change, consider whether any violated heuristic points to a concrete improvement. Prioritize fixes that affect task completion, trust, accessibility, or purchase confidence.

## Code Quality

Use ESLint and Prettier consistently.

- Run ESLint before finishing frontend changes.
- Run Prettier formatting before finishing changes once Prettier is configured.
- Keep formatting-only changes separate from behavioral changes when practical.
- Add or update npm scripts for `lint`, `format`, and `format:check` when setting up or migrating frontend tooling.
- Prefer project-level configuration files so future agents and editors share the same rules.

## Backend Domain Model

Use entity factory methods to protect domain invariants.

Entities should not be freely created with invalid state from application code. Prefer private constructors plus static `Create(...)` methods when an entity has business rules such as required names, valid email addresses, non-empty foreign keys, length limits, or non-negative prices.

Keep validation that defines whether an entity can exist close to the entity. Keep workflow validation and cross-entity orchestration in services.

EF Core may still need private parameterless constructors and anonymous seed objects for materialization and `HasData`. That is acceptable; normal application code should use the entity factories.

## Backend API Layers

Keep controllers thin. Controllers should translate HTTP input into service calls and translate service outcomes into HTTP responses. They should not contain EF Core queries or business workflows.

Services own use-case orchestration and business decisions. Services should call entity factory/update methods, coordinate repository calls, enforce cross-entity checks such as "seller exists before creating a game", and raise clear application/domain exceptions for invalid operations.

Repositories own EF Core data access. Keep LINQ queries, `Include`, ordering, tracking decisions, adds, deletes, and `SaveChangesAsync` behind repository methods so services can be unit-tested without mocking `DbContext` or `DbSet`.

Use centralized API exception middleware for expected service/domain exceptions. Controllers should not repeat local `try/catch` blocks for `DomainValidationException` or `ResourceNotFoundException`; they should let middleware map those exceptions to standardized ProblemDetails responses.

For EF Core repositories, default read-only queries to `AsNoTracking()`. Use tracked queries only when an entity will be modified or deleted before `SaveChangesAsync`. Prefer method names that make intent clear, such as `GetByIdAsync` for read-only access and `GetTrackedByIdAsync` or `GetByIdForUpdateAsync` for mutation flows.

## Backend Testing

Add tests with every backend feature.

- Unit tests should cover service behavior, controller success-path HTTP mapping, entity invariants, and exception middleware response mapping.
- Unit tests should use fakes/stubs for repository abstractions instead of mocking EF Core `DbSet` query behavior.
- Integration tests should cover real repository/database behavior with EF Core.
- Prefer tests against the production database provider for high-risk persistence behavior. SQLite in-memory is acceptable for early isolated relational integration tests when PostgreSQL test infrastructure is not yet available.
- Avoid EF Core's non-relational in-memory provider for repository/query tests.
- Coverage is a signal, not the goal. Prioritize meaningful assertions for business rules, validation, service branches, controller success mapping, repository persistence behavior, and expected exception mapping.
- Aim for very high coverage on newly implemented domain/use-case code when practical, but do not chase 100% for trivial DTOs, EF-only constructors, framework-generated code, or defensive branches that do not represent normal product behavior.
- End-to-end tests should wait until a full user-facing feature chain exists across frontend and backend.

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
