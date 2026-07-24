# Gaming Store Frontend

The frontend is a Next.js App Router, React, TypeScript, and Tailwind CSS app created for the Gaming Store project.

## Architecture

The folder structure keeps the Next.js `src/app/` router and uses Feature-Sliced Design for the rest of the application:

- `src/app/` - Next.js routes, layouts, metadata, and global styles.
- `src/pages/` - route-level screens when routing is added.
- `src/widgets/` - larger page sections composed from features and entities.
- `src/features/` - user-facing actions and workflows.
- `src/entities/` - domain objects such as games, users, sellers, carts, and orders.
- `src/shared/` - reusable API clients, UI primitives, utilities, and config.

Prefer Server Components for public pages and SEO-critical data. Use Client Components only for browser interaction.

## Local Development

Install dependencies:

```bash
npm install
```

Start the frontend:

```bash
npm run dev
```

The app expects the backend to run at `http://localhost:5215`.

You can override the backend URL with:

```bash
API_BASE_URL=http://localhost:5215 npm run dev
```

## Code Quality

Run checks before finishing frontend work:

```bash
npm run lint
npm run format:check
```

Format files with:

```bash
npm run format
```
