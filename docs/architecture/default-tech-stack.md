# Gaia Default Tech Stack

This document defines Gaia's default application stack when the user request or
the target repository does not specify another approved platform.

## Selection rule

- If the user explicitly chooses another stack, that choice overrides this default.
- If the repository already has a mandated platform, Gaia should treat this document as migration guidance instead of a forced rewrite.
- If no explicit stack exists, Gaia should declare this baseline before planning or implementation proceeds.

## Frontend baseline

- latest stable React with TypeScript
- Vite as the default frontend toolchain
- Redux Toolkit for centralized client state
- Tailwind CSS for styling
- shadcn/ui as the default component primitive layer
- a centralized semantic design system built on CSS variables for color, status, radius, and shadow tokens

## Default UI standardization phases

### Phase 1: Foundation and configuration

- initialize shadcn/ui for Vite + React + Tailwind + TypeScript
- standardize Tailwind theme tokens around semantic CSS variables
- define semantic color tokens, status tokens, radius, and elevation shadows

### Phase 2: Component replacement

- replace bespoke buttons with shadcn `Button`
- replace bespoke modals with shadcn `Dialog`
- replace custom badges and pills with shadcn `Badge`
- refactor cards onto shadcn `Card` primitives

### Phase 3: Anti-pattern removal

- remove arbitrary typography values in favor of the Tailwind scale
- remove arbitrary spacing values in favor of the standard spacing grid
- replace rigid fixed heights with flexible or minimum-height layouts
- ensure consistent hover, focus-visible, and disabled states across interactive elements

## Backend baseline

- latest stable ASP.NET Core API on the latest stable .NET runtime
- EF Core for data access
- PostgreSQL as the default relational database
- API capabilities exposed through an MCP server surface, typically an `/mcp` endpoint

## Invariants

- The default stack is the fallback, not a license to ignore explicit project constraints.
- MCP exposure is part of Gaia's default API expectation whenever Gaia owns the backend stack choice.
- Detailed execution guidance belongs in the `gaia-default-tech-stack` skill.
