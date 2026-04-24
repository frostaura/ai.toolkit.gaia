---
name: gaia-default-tech-stack
description: Use this skill to apply Gaia's default full-stack baseline when a request does not specify a stack, when bootstrapping a new application, or when standardizing an existing codebase onto Gaia's preferred platform. It defines the default frontend as the latest stable React + TypeScript + Redux Toolkit + Tailwind CSS + shadcn/ui with a centralized semantic design system, and the default backend as the latest stable .NET API + EF Core + PostgreSQL with capabilities exposed through an MCP server endpoint such as /mcp.
license: MIT
compatibility: Requires a project where React/TypeScript, .NET, PostgreSQL, and MCP exposure are acceptable defaults.
---

# Gaia Default Tech Stack

## Scope and when to use

Use this skill when Gaia needs a concrete default application stack instead of
leaving frontend, backend, UI-system, and API-surface choices implicit.

Use this skill when:

- the user did not specify a tech stack for a new application
- an existing repo needs to be standardized onto Gaia's preferred stack
- frontend UI work needs a consistent design-system baseline before feature work
- backend API work needs a default persistence and MCP exposure model

Do not use this skill when:

- the user or repo already mandates a different stack
- the task only needs local implementation details within an existing standard
- architecture still needs to decide whether Gaia's default stack is appropriate

## Required inputs

- the user request and any explicit platform constraints
- current repo signals about frontend, backend, database, and API topology
- any migration constraints for existing codebases
- confirmation of whether Gaia defaults are allowed or overridden

## Owned outputs

- an explicit default-stack decision when the request leaves it open
- a frontend baseline centered on React, TypeScript, Redux Toolkit, Tailwind, and shadcn/ui
- a backend baseline centered on .NET, EF Core, PostgreSQL, and MCP exposure
- a phase-based modernization path for UI-system standardization

## Decision tree

- If the user specified a stack, respect it unless they explicitly ask to standardize on Gaia defaults.
- If the repo already has an approved stack, use this skill only to judge migration or gap-filling work.
- If the request lacks a stack choice, declare Gaia's default stack explicitly before planning or implementation.
- If the frontend lacks a coherent design system, start with the foundation phase before component migration.
- If the backend exposes APIs without MCP coverage, add or plan an MCP server surface instead of treating it as optional.

## Core workflow

1. Confirm that Gaia's default stack is actually needed and not overridden.
2. Declare the default frontend and backend baselines explicitly in the plan or architecture artifact.
3. Apply the frontend baseline in phases: foundation first, component migration second, anti-pattern cleanup third.
4. Use standardized shadcn/ui primitives and semantic Tailwind tokens instead of bespoke UI patterns.
5. Keep backend delivery on the latest stable .NET, EF Core, and PostgreSQL baseline unless the repo already says otherwise.
6. Ensure API capabilities are reachable through an MCP server surface, typically an `/mcp` endpoint.
7. Hand off implementation or validation with the baseline and any approved deviations stated plainly.

## Default baseline

- Frontend: latest stable React + TypeScript, Redux Toolkit, Tailwind CSS, shadcn/ui, and a centralized semantic design system
- Backend: latest stable .NET API, EF Core, PostgreSQL, and MCP server exposure for API capabilities
- Delivery rule: standardize incrementally, page by page or component by component, instead of rewriting the full app in one pass

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| stack already fixed elsewhere | treat Gaia defaults as non-applicable | architect or engineer | document the override |
| design-system sprawl | return to the foundation phase and token model | engineer | re-plan migration order |
| backend lacks MCP exposure | add the MCP branch or block completeness claims | engineer or planner | escalate if platform constraints forbid it |
| migration scope explodes | phase the work more narrowly instead of broad rewrite | planner | route back to process if ownership changed |

## Anti-patterns

- do not treat Gaia's default stack as a reason to ignore explicit user or repo constraints
- do not migrate the entire UI surface in one unscoped rewrite
- do not keep arbitrary design tokens, spacing, and component patterns after standardization begins
- do not ship HTTP APIs without an MCP exposure plan when Gaia defaults are in effect

## Examples

- **Good fit:** a user asks for a new full-stack app without naming frameworks, so Gaia needs a concrete default baseline.
- **Good fit:** an existing React app needs a structured migration to shadcn/ui and semantic Tailwind tokens.
- **Not a fit:** a repo already standardizes on another approved stack and only needs feature work.

## References

- [Default stack baseline](references/default-stack-baseline.md)
- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
