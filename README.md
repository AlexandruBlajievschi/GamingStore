# Gaming Store

Gaming Store is a learning-friendly full-stack project for an online store focused on games. The backend is built with .NET and the frontend is built with Next.js, React, TypeScript, and Tailwind CSS.

The first version will keep the scope simple: visitors can browse the store, registered users can interact with the platform as customers, and sellers can create and manage game listings. A guest does not need to be a stored role at first; it can simply mean a visitor who is not logged in yet.

## Project Layout

- `backend/` - .NET API using a simple MVC-style structure.
- `frontend/` - Next.js App Router frontend using React, TypeScript, Tailwind CSS, and Feature-Sliced Design.
- `docs/` - project documentation for shared engineering and product conventions.

## Health Check

The backend exposes a small health endpoint at:

```text
GET http://localhost:5215/api/health
```

This endpoint is kept for backend diagnostics, deployment checks, and future monitoring. The public frontend health page was removed because the storefront itself now exercises real API behavior through the game catalog.

## Usability

Frontend work should use Nielsen's 10 usability heuristics as a design and review lens. See `docs/usability-heuristics.md`.

## Early Domain Notes

- `Guest` - anonymous visitor; usually not a database role.
- `User` - registered customer account.
- `Seller` - registered account that can create and manage game listings.

## Commit Style

Project commits follow the Conventional Commits format described in `AGENTS.md`.

More detailed setup, environment, and contribution instructions can be added once the first real features are chosen.
