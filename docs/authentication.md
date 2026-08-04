# Authentication and Authorization

This document describes the authentication and authorization behavior currently implemented in Gaming Store.

## Identity Model

The backend uses ASP.NET Core Identity with `Guid` keys and Entity Framework Core's PostgreSQL store. The domain `User` inherits from `IdentityUser<Guid>` and adds:

- first name
- last name
- creation timestamp

The Identity migration extends the `Users` table with password, security-stamp, email-confirmation, lockout, phone, and two-factor fields. It also creates the Identity role, claim, external-login, user-role, and token tables.

The seeded `alex.player@gamingstore.local` user has no password and cannot use password login. Users created through registration receive an Identity password hash.

## Current Sign-In Method

Gaming Store currently accepts local email-and-password registration and login.

Registration requires:

- first and last names of at most 100 characters each
- a syntactically valid, unique email of at most 320 characters
- a password between 15 and 128 characters

Email addresses are trimmed, normalized to lowercase in the domain model, and normalized by Identity for lookup and uniqueness. Registration checks email format but does not prove that the mailbox exists or belongs to the registrant. A successful registration immediately creates the browser session.

The configured password rules allow passphrases without mandatory uppercase letters, digits, or symbols. ASP.NET Core Identity hashes passwords with PBKDF2-HMAC-SHA512, a unique random salt, and 220,000 iterations. The database stores only Identity's encoded `PasswordHash`, never the original password.

## Browser Session

Both registration and login issue the same ASP.NET Core Identity application cookie. The cookie is:

- HttpOnly
- SameSite Lax
- host-only
- valid for an eight-hour sliding window
- non-persistent because the current sign-in actions do not enable “remember me”

During HTTP development the cookie is named `GamingStore.Auth` and uses the request's security level. Outside development it is named `__Host-GamingStore.Auth` and requires HTTPS.

The Next.js rewrite forwards browser requests from `/api/*` to the ASP.NET Core API, so the browser treats authentication calls as same-origin requests. Authentication credentials are not stored in `localStorage` or `sessionStorage`.

## Antiforgery Protection

Registration, login, and logout validate an antiforgery token in addition to their antiforgery cookie. The frontend first requests:

```text
GET /api/auth/antiforgery-token
```

It then sends the returned token in the `X-CSRF-TOKEN` header of the modifying request. The token and password are not written to browser console logs.

## Authentication Routes

| Method | Route | Access | Behavior |
| --- | --- | --- | --- |
| `GET` | `/api/auth/antiforgery-token` | Anonymous | Issues the antiforgery cookie and returns its matching request token. |
| `POST` | `/api/auth/register` | Anonymous + antiforgery | Creates a local user and signs them in. |
| `POST` | `/api/auth/login` | Anonymous + antiforgery | Verifies the password and creates a session. |
| `GET` | `/api/auth/me` | Authenticated | Returns the current user's ID, name, and email. |
| `POST` | `/api/auth/logout` | Authenticated + antiforgery | Ends the current session. |

Unknown accounts, incorrect passwords, and locked accounts receive the same `Invalid email or password.` response. Duplicate registration receives a generic registration failure rather than confirming that an email already exists.

New accounts are locked for 15 minutes after five failed password attempts. The antiforgery-token, registration, and login routes share a fixed limit of ten requests per remote IP per minute and return HTTP 429 when that limit is exceeded.

## Current Authorization Boundary

Authentication proves which user owns the current cookie. Authorization currently protects only:

- `GET /api/auth/me`
- `POST /api/auth/logout`

Those routes return HTTP 401 when no valid session exists. The Identity role tables and role services are present, but no seller or administrator role policy is currently applied.

The games, sellers, and users CRUD controllers currently remain anonymous, including their modifying endpoints. The frontend account state does not grant additional catalog permissions.

## Frontend Behavior

The storefront header orders its main controls as search, account, and cart.

The account control has three visible states:

- pulsing gray icon while the current session is being checked
- neutral outlined icon with a gray badge when logged out
- cyan icon with a green check badge when logged in

Clicking the logged-out icon opens `/login`. Clicking the authenticated icon opens a menu containing the current user's name, email, and logout action. Registration is available at `/register`; its back action returns to login, while login's back action returns to the store.

In development, browser console messages prefixed with `[Gaming Store auth]` report the current user and authentication outcomes. These messages are disabled in production and do not include passwords, cookies, or antiforgery tokens.

## Automated Verification

Backend coverage includes entity, service, controller, middleware, repository, Identity password-hash, and HTTP authentication tests. The HTTP integration test runs the real ASP.NET Core pipeline and verifies:

```text
antiforgery token -> registration -> Identity cookie -> current-user response
```

The HTTP test uses SQLite in memory and ephemeral Data Protection keys so it is isolated from the development PostgreSQL database and Windows user profile.
