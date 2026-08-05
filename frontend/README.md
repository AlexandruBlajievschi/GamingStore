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

## Authentication UI

The storefront header displays an account icon beside search and before the cart icon. A neutral account icon means the visitor is logged out; a cyan icon with a green check means an account is authenticated. The account menu shows the current user's name and email and provides logout.

Local account routes are available at `/login` and `/register`. Both screens can start Google authentication, and signed-in password users can connect Google from the account menu. Browser requests use the same-origin `/api/*` rewrite and an HttpOnly session cookie. Local form actions use an antiforgery token; Google's redirect flow uses OAuth state and correlation protection. Development-only console messages prefixed with `[Gaming Store auth]` show the current user and successful or failed authentication events without logging passwords, cookies, antiforgery tokens, or Google credentials.

See [`../docs/authentication.md`](../docs/authentication.md) for the complete current behavior.
See [`../docs/google-authentication-setup.md`](../docs/google-authentication-setup.md) for local Google setup.

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
