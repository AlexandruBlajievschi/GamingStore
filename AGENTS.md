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

## Code Quality

Use ESLint and Prettier consistently.

- Run ESLint before finishing frontend changes.
- Run Prettier formatting before finishing changes once Prettier is configured.
- Keep formatting-only changes separate from behavioral changes when practical.
- Add or update npm scripts for `lint`, `format`, and `format:check` when setting up or migrating frontend tooling.
- Prefer project-level configuration files so future agents and editors share the same rules.

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
