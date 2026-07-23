# Gaming Store Frontend

The frontend is a React TypeScript app created for the Gaming Store project.

## Architecture

The folder structure follows Feature-Sliced Design at a practical starting size:

- `src/app/` - app shell, global providers, routing, and styles.
- `src/pages/` - route-level screens when routing is added.
- `src/widgets/` - larger page sections composed from features and entities.
- `src/features/` - user-facing actions and workflows.
- `src/entities/` - domain objects such as games, users, sellers, carts, and orders.
- `src/shared/` - reusable API clients, UI primitives, utilities, and config.

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
