---
name: spec-consistency
description: Prevent and repair drift between `/docs` (source of truth), code, tests, CI, and runtime artifacts. Use before marking work done.
---

# Spec Consistency (Anti-Drift)

## When to use

Use whenever:

- Behavior changes (features/bugs/refactors that affect outputs)
- `/docs` changes (especially `/docs/use-cases/`)
- New endpoints/routes/commands are added
- QA Gatekeeper is reviewing completion
- Repo Explorer suspects drift

Drift is **blocking** in Gaia.

## Inputs

- `/docs/` (use-cases, architecture, testing docs)
- Code changes (API/CLI/UI)
- Tests (unit/integration/e2e)
- CI workflows
- Docker compose + Makefile

## Outputs

- Drift status: `none | suspected | confirmed`
- A concrete fix list (docs updates and/or code/test/CI alignment)
- Updated docs/tests/configs so docs and repo behavior match

## Core rule

If unsure about “source of truth” direction, default to **docs**.
Orchestrator may decide case-by-case, but must be able to justify in 1 line.

If “code wins” changes a use case: apply use-case gates (Playwright specs + manual regression).

## Checks (do these in order)

### 1) Use-case ↔ behavior

For each affected UC file:

- Does the described outcome exist in the system?
- Are inputs/outputs (status codes, payload shapes, UI states) consistent?
- Are acceptance criteria testable and still true?

If UC changed (new/change/remove): trigger use-case gates.

### 2) Public surface ↔ docs

Validate consistency between:

- Documented routes/commands and actual ones
- Auth requirements and actual behavior
- Error formats/codes and actual behavior
- Versioning / pagination / sorting rules (if applicable)

### 3) Tests ↔ use cases

- For use-case changes: ensure integration/e2e coverage exists.
  - Web: Playwright specs (follow repo convention; else standardize)
  - API: curl-like checks against docker-compose stack
- Ensure tests reference UC IDs where relevant (web specs include UC ID in filename).

### 4) CI ↔ reality

- CI must run lint/build/tests as applicable.
- Ensure CI uses the same commands as local Make targets (or vice versa).
- If CI failing: fix CI first (blocking).

### 5) Docker/Make ↔ docs

If HTTP API:

- docker-compose must exist and match documented run instructions
- `.env.example` exists and docs list required vars (high-level)
  Makefile:
- `make up/down/test/lint/build` (or best approximation) exists and docs reflect it

## Fix strategies

- Prefer minimal, surgical changes that restore consistency.
- If docs are correct: change code/tests/configs to match docs.
- If code is correct and docs outdated: update docs _comprehensively_.
  - If behavior/use-cases change as a result → apply use-case gates.

## Validation checklist (before declaring “no drift”)

- UC acceptance criteria matches observable behavior
- Playwright specs exist when web use-cases changed
- Manual regression labels completed for use-case changes (`curl`, `playwright-mcp`)
- CI exists and is green
- Make targets align with CI and docs
- Skills that mention commands/paths match reality (skill drift = blocking)

## References

- `AGENTS.md` (drift policy; blocking)
- `.github/skills/gaia-process/SKILL.md` (workflow)
- `.github/skills/doc-derivation/SKILL.md` (restore docs-first)
- `.github/workflows/`, `Makefile`, `docker-compose.yml`
- `/docs/use-cases/`, `/docs/architecture/`, `/docs/testing/`
