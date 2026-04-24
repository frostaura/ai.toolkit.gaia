name: gaia-rapid-development
description: Provides a fast local edit-run-observe loop for bounded, low-risk work using Gaia's default stack baseline, docker-backed shared dependencies, and browser-based verification. Use it when rapid iteration matters more than a full SDLC pass-through, and stop using it as soon as scope, risk, or gate requirements grow.
license: MIT
compatibility: Requires docker compose, latest stable .NET, Node/npm, and browser tooling for screenshot-based verification.
---

# Gaia Rapid Development

## Scope

Use this skill to move quickly through a small local implementation loop without
pretending the work needs Gaia's full intake-to-release path every time.

## Use when

- the task is bounded, local, and low-risk
- the fastest way to learn is to run the real app and iterate with HMR
- shared dependencies such as Postgres or MailHog still matter during the loop
- browser-visible behavior needs immediate screenshot-based verification

## Do not use when

- the work changes architecture, contract, README-level story, or release gates
- the task already needs explicit planning across multiple branches or owners
- formal regression evidence is the main need rather than fast convergence
- the loop has stopped being local and now needs broader QA or release review

## Required inputs

- the bounded objective, constraints, and obvious adjacent risk areas
- whether the user or repo overrides `gaia-default-tech-stack`
- the repo's local runtime commands and required environment variables
- the browser routes or user flows that prove the change worked
- the escalation point that sends the work back to the full Gaia path

## Owned outputs

- a converged local implementation loop with realistic runtime dependencies
- a local loop that either follows Gaia's default stack baseline or names the repo-specific override explicitly
- browser-review evidence based on screenshots, console output, and network behavior
- explicit escalation when the work grows past the shortcut's safe boundary
- a clean handoff into engineering, testing, or the full Gaia process when needed

## Decision tree

- If the task is not clearly bounded, route back to `gaia-process`.
- If architecture or contract behavior might change, route to `gaia-process`.
- If the user specified another stack, use that stack instead of inheriting Gaia defaults.
- If neither the user nor the repo has a stack override, inherit `gaia-default-tech-stack` before starting the local loop.
- If the work mainly needs fast feedback, start the local runtime loop.
- If the browser still shows defects, keep iterating while the work remains local.
- If the loop stabilizes, either finish the small task or hand off to `gaia-engineering` or `gaia-testing`.
- If new dependencies, acceptance gaps, or gate expectations appear, stop and re-route into the full Gaia process.

## Core workflow

1. Confirm whether the user or repo overrides `gaia-default-tech-stack`.
2. Start only the shared dependencies you need from `docker compose`.
3. Run the backend directly with `dotnet run` against the local dependency stack.
4. Run the frontend directly with the repo's HMR command so UI edits apply immediately.
5. For default-stack UI work, land semantic tokens and global styles before page-by-page primitive replacement.
6. Reproduce the target behavior in the live browser and capture screenshots of the real result.
7. Inspect browser console messages and network behavior instead of trusting the DOM alone.
8. Iterate on code, refresh the live page, and compare the next screenshot to the intended outcome.
9. Once the local loop converges, decide whether the work is truly done or now needs formal engineering/testing handoff.

## Local runtime model

- keep Docker focused on shared services such as Postgres and MailHog, not the active app-edit loop
- prefer repo-native dev commands over containerized backend/frontend runs so edits apply immediately
- when the repo does not say otherwise, assume the frontend is Vite + React + TypeScript + Redux Toolkit + Tailwind + shadcn/ui and the backend is .NET + EF Core + PostgreSQL with MCP exposure
- keep backend and frontend origins explicit when bypassing the frontend proxy
- stop and document environment blockers instead of silently switching to a different runtime model

## Browser verification rules

- use headed browser verification whenever user-visible behavior matters
- treat screenshots as evidence of layout, state, and visual regressions, not just decoration
- check console errors, failed requests, and obvious loading-state lies before calling the loop healthy
- iterate immediately when the screenshot or browser telemetry still contradicts the intended behavior

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| loop no longer bounded | stop using the shortcut | rapid developer | `gaia-process` |
| architecture or contract drift | route upstream before more edits | rapid developer | `gaia-process` |
| browser proof still fails | keep iterating while scope stays local | rapid developer | `gaia-testing` if broader evidence is needed |
| environment mismatch | repair the dev runtime or document the blocker | rapid developer | `gaia-engineering` |

## Anti-patterns

- do not use this skill to avoid architecture or planning work that is actually needed
- do not spin up full Dockerized frontend/backend containers when the goal is fast HMR-backed iteration
- do not skip the foundation-first UI order when standardizing onto Gaia's default frontend stack
- do not trust DOM structure alone when screenshots, console output, or network requests disagree
- do not keep the shortcut running once the work clearly needs broader QA or release gates

## Examples

- **Good fit:** tighten a local frontend workflow where screenshots and live requests are the fastest truth source.
- **Good fit:** debug a backend-plus-frontend slice against compose-backed Postgres and MailHog while both app surfaces run directly in dev mode.
- **Not a fit:** introduce a cross-cutting workflow change that needs architecture, planning, and release-gate updates first.

## References

- [Gaia default stack baseline](../gaia-default-tech-stack/references/default-stack-baseline.md)
- [Rapid local runtime loop](references/rapid-local-runtime-loop.md)
- [Gaia delivery policy](../references/gaia-delivery-policy.md)
- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
