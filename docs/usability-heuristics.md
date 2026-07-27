# Usability Heuristics

Gaming Store uses Nielsen's 10 usability heuristics as a shared design and review checklist for the storefront experience.

These heuristics are not formal requirements by themselves. They are a practical lens for noticing usability risks early, especially before the project has real user research, analytics, and mature end-to-end test coverage.

## How We Use Them

- Use them during frontend planning, implementation, and review.
- Prefer concrete fixes over abstract scoring.
- Combine them with accessibility checks, responsive layout testing, and product-flow testing.
- Treat them as guidance for user-facing behavior, not backend architecture.

## The 10 Heuristics

1. Visibility of system status

   The interface should keep users informed about what is happening. Use clear loading, empty, success, and error states.

2. Match between system and the real world

   The store should use familiar language and concepts: games, prices, featured products, search results, carts, checkout, sellers, and accounts.

3. User control and freedom

   Users should be able to recover from mistakes and move around freely. Examples include clearing search, leaving a product page, removing cart items, and cancelling flows.

4. Consistency and standards

   Follow common web and e-commerce patterns unless there is a strong reason to diverge. Buttons, links, cards, search, forms, and navigation should behave predictably.

5. Error prevention

   Prevent avoidable mistakes through validation, constraints, disabled impossible actions, clear labels, and confirmation only when the consequence is meaningful.

6. Recognition rather than recall

   Users should not have to remember hidden information. Show visible choices, product names, prices, images, helpful placeholders, and context where it matters.

7. Flexibility and efficiency of use

   Support both casual browsing and faster repeated use. Search, keyboard navigation, good defaults, and efficient flows help experienced users move quickly.

8. Aesthetic and minimalist design

   Keep screens focused on the user's current task. Remove test panels, diagnostic UI, and visual clutter when they no longer serve the storefront experience.

9. Help users recognize, diagnose, and recover from errors

   Error messages should be specific, readable, and useful. They should explain what happened and leave the user with a clear next step.

10. Help and documentation

    Document project conventions and user-facing behavior when future contributors need the context to make consistent decisions.

## Current Project Examples

- The search dropdown shows only the cover, title, and price so users can recognize results quickly without extra noise.
- Featured games remain stable while searching, so paid ads or curated placements are not unexpectedly changed by the search input.
- The frontend health page was removed from the public app because it was developer diagnostics, not useful storefront content.
- The backend health endpoint stays because it supports deployment, monitoring, and operational checks.
