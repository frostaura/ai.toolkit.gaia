# Gaia Default Stack Baseline

Use this reference when Gaia needs to apply or migrate toward the default stack
for a user project.

## Default selection rule

- If the user or repository does not specify another approved stack, default to the baseline below.
- If the user explicitly names another stack, that override wins.
- If the existing repo has a mandated stack, use this baseline only for migration or comparison work.

## Frontend default

- latest stable React with TypeScript
- Vite for the default web application toolchain
- Redux Toolkit for centralized client state
- Tailwind CSS for styling
- shadcn/ui for the standardized component primitive layer
- a centralized design system built on semantic CSS variables

## Frontend UI phases

### Phase 1: Foundation and configuration

1. Initialize `shadcn/ui` in the project and ensure `components.json` matches a Vite + React + Tailwind + TypeScript environment.
2. Rewrite the Tailwind theme to use semantic CSS-variable tokens instead of ad-hoc color values.
3. Standardize the token model around `background`, `surface`, `foreground`, `primary`, `secondary`, `muted`, `accent`, and `border`.
4. Add status tokens for `success`, `warning`, and `destructive`.
5. Define a shared radius token, such as `0.5rem`, and standardized shadow elevations from `shadow-sm` through `shadow-xl`.

### Phase 2: Component replacement

1. Replace bespoke clickable controls with shadcn/ui primitives instead of preserving custom button implementations.
2. Replace raw `<button>` usage with the shadcn `Button` component and choose variants and sizes intentionally.
3. Replace custom modal implementations with the shadcn `Dialog` component so overlay, padding, and close behavior are consistent.
4. Replace custom badges, pills, and metric indicators with the shadcn `Badge` component using the semantic status tokens.
5. Refactor dashboard and layout containers onto `Card`, `CardHeader`, `CardTitle`, and `CardContent`.

### Phase 3: Eradicating prescriptive anti-patterns

1. Replace arbitrary typography values such as `text-[30px]` with the standard Tailwind type scale.
2. Replace arbitrary margin, padding, and gap values with spacing that follows the Tailwind 4pt or 8pt grid.
3. Remove rigid fixed heights from cards and containers unless there is a documented requirement, favoring `min-h-*`, flex layouts, and content-driven sizing.
4. Ensure every interactive element has `hover:`, `focus-visible:`, and `disabled:` behavior, relying on shadcn/ui defaults where possible.

## Execution rules

- Process the refactor page by page or component by component.
- Start with `tailwind.config.js` and global CSS so token and primitive foundations land first.
- Ask for confirmation before installing new shadcn/ui primitives via CLI.
- If a component needs a highly specific custom layout, wrap it in a standard layout container first and preserve the inner visual logic only when necessary.

## Backend default

- latest stable ASP.NET Core API on the latest stable .NET runtime
- EF Core as the default ORM and persistence abstraction
- PostgreSQL as the default relational database
- all API capabilities must also be exposed through an MCP server surface, typically an `/mcp` endpoint or equivalent Model Context Protocol boundary

## Backend invariants

- MCP exposure is part of the default API baseline, not an optional afterthought.
- Data access should align to EF Core and PostgreSQL unless the user explicitly overrides the backend platform.
- API planning should assume both human-facing HTTP use and agent-facing MCP capability exposure.
