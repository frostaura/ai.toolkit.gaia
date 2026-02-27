---
name: doc-derivation
description: Derive comprehensive `/docs` from an existing codebase when docs are missing/stale. This is blocking work before new features.
---

# Doc Derivation (Restore Docs-First Truth)

## When to use

Use when:

- Code exists but `/docs/` is missing or incomplete, OR
- Docs exist but are stale and docs↔code drift is confirmed, OR
- Orchestrator explicitly decides “code wins” and docs must be updated to match behavior.

This work is **blocking**: do it before feature work.

## Inputs

- Current repository (code, configs, tests, CI, docker)
- Existing documentation (if any)
- Repo Survey from Repo Explorer

## Outputs

A comprehensive `/docs/` set that reflects the real system:

- `/docs/use-cases/` — one use case per file; use `UC-000-template.md` (naming: `UC-NNN-short-title.md`)
- `/docs/architecture/` — system overview + key decisions; use `ARCH-000-template.md` (naming: `ARCH-NNN-short-title.md`)
- `/docs/testing/` — how tests are structured + how to run; use `TEST-000-template.md` (naming: `TEST-NNN-short-title.md`)
- `/docs/README.md` — docs index + source-of-truth statement

## Constraints

- Prefer updating existing docs vs rewriting everything.
- Keep docs factual and verifiable from the repo (code/tests/configs).
- Avoid huge prose; focus on accurate contracts, flows, and run instructions.

## Step 1 — Inventory behavior from code (fast but thorough)

Identify:

- Entry points (servers/CLIs/apps)
- Public interfaces:
  - HTTP endpoints (routes, controllers, minimal APIs)
  - CLI commands/flags
  - background jobs/queues
- Data stores and schemas (ORM models/migrations)
- Integrations (external APIs, auth providers)
- Observability hooks (logging/tracing/metrics)

Capture notes as you go (do not commit notes; commit only final docs).

## Step 2 — Derive use cases (UC files)

Create `/docs/use-cases/` if missing.

For each real user-facing flow, create a file:
`/docs/use-cases/UC-###-<kebab-title>.md`

Each file MUST include these headers:

- `# UC-###: <Title>`
- `## Goal`
- `## Actors`
- `## Preconditions`
- `## Main Flow`
- `## Variants / Edge Cases`
- `## Acceptance Criteria`
- `## Notes (Optional)`

Rules:

- Use cases describe observable outcomes, not internal implementation details.
- If an endpoint exists but no clear user goal: document as a “system use case” (still UC-###).

## Step 3 — Derive architecture docs

Create `/docs/architecture/` if missing.

Minimum set:

- `system-overview.md` (components, responsibilities, boundaries)
- `runtime-and-dependencies.md` (services, data stores, external deps)
- `api-surface.md` (routes/commands summary, auth, error patterns)
- `decisions.md` (key decisions found; record as bullets, not essays)

Keep it concise but complete enough that future work can be spec-driven.

## Step 4 — Derive testing docs + local run

Create `/docs/testing/` if missing.

Include:

- `testing-strategy.md` (what exists: unit/integration/e2e; what triggers what)
- `how-to-run.md` (canonical Make targets and what they do)
- `ci-overview.md` (what CI runs and where)

Rule:

- Prefer Makefile UX. If Makefile missing, create a task to add it (and add docs once it exists).

## Step 5 — Align with CI/lint/docker reality (no drift)

Cross-check that docs reflect:

- Lint command(s)
- Build command(s)
- Test command(s)
- Docker compose up/down
- Environment variables and `.env.example`

If any are missing in repo, create tasks to add them (blocking if needed per Gaia rules).

## Step 6 — Validation checklist (before declaring drift resolved)

- `/docs/` exists and is structured
- `/docs/use-cases/` exists and UC files have UC-### and required headers
- Run instructions in docs match Make targets
- Docs do not contradict code/tests/CI
- If behavior changes were made during drift fix, ensure use-case gates were applied

## References (read/consult)

- `AGENTS.md` (drift is blocking; case-by-case direction)
- `.github/copilot-instructions.md` (repo invariants)
- `.github/skills/gaia-process/SKILL.md` (workflow)
- `.github/skills/spec-consistency/SKILL.md` (anti-drift checks)
- `Makefile`, `.github/workflows/`, `docker-compose.yml`, tests folders
