# Frontend Structure

This project uses Feature-Sliced Design so the UI can grow without turning into one large components folder.

## Layers

- `app` - application bootstrap, providers, routing, and global styles.
- `pages` - complete route screens.
- `widgets` - composed interface blocks used by pages.
- `features` - user-facing actions, such as signing in or creating a game listing.
- `entities` - business entities such as game, user, seller, cart, and order.
- `shared` - generic code with no business ownership.

## Import Direction

Code should generally import only from layers below it:

```text
app -> pages -> widgets -> features -> entities -> shared
```

For now the app is intentionally small, so only the layers needed for the first connection check have files.
