---
name: stack-default-web-ts
description: Opinionated baseline for JS/TS web repos: lint/format, tests, Playwright, docker (if HTTP API), Makefile targets, and CI wiring.
---

# Stack Default: Web (JS/TS)

## When to use

Use when:

- Repo is a JS/TS web project and lacks standard foundations, OR
- You’re bootstrapping from scratch, OR
- You need a consistent baseline before implementing use cases.

## Baseline outcomes

- Lint/format configured and enforced
- Makefile provides canonical commands
- CI runs lint/build/test
- Playwright present for web E2E (required for use-case changes)
- Docs updated for “how to run” and “how to test”

## Preferred tools (default choices)

- Lint: ESLint (or existing repo standard)
- Format: Prettier (or existing repo standard)
- E2E: Playwright (required if none exists)
- Unit: repo’s existing runner (Jest/Vitest/etc.)

Rule: If repo already uses different tools, prefer existing conventions.

## Required files / locations

- `Makefile` (repo root)
- `.github/workflows/ci.yml` (or equivalent)
- `/docs/testing/how-to-run.md`
- `/tests/e2e/` as default E2E location if no convention exists

## Make targets (required)

Provide:

- `make lint`
- `make build`
- `make test`
- `make up` / `make down` (if docker-compose exists / needed)

Optional:

- `make test-e2e` (if repo splits unit vs e2e)
- `make format`

## Step 1 — Lint/format baseline

- Add ESLint + config (if missing)
- Add Prettier + config (if missing)
- Ensure `make lint` runs the canonical lint/format checks

## Step 2 — Test baseline

- Ensure unit tests can run (`make test`)
- If no unit harness exists, add minimal harness aligned to framework (Next/Vite/etc.)

## Step 3 — Playwright baseline (if none exists)

- Add Playwright dependency/config
- Ensure headless run for CI
- Default location if none exists: `/tests/e2e/`
- Enforce UC ID in spec filename for use-case work (e.g., `uc-001-*.spec.ts`)

## Step 4 — CI wiring

- CI must run:
  - `make lint`
  - `make build` (if applicable)
  - `make test` (and `make test-e2e` if split)
- Add caching (node modules) only if stable; prefer reliability first.

## Step 5 — Docs alignment

Update `/docs/testing/how-to-run.md`:

- How to install deps
- Make targets
- How to run Playwright (if present)

## Suggested required_gates[]

- Baseline setup: `lint`, `build`, `ci`
- If adding tests: add `unit`
- If adding Playwright: add `e2e`
