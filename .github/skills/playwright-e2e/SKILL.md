---
name: playwright-e2e
description: Add/extend Playwright E2E/integration specs for web use-case changes. UC ID must be in spec filename. Prefer existing repo conventions; else standardize.
---

# Playwright E2E / Integration Specs (Web)

## When to use

Use when:

- Orchestrator flags a task as **use-case change** and the repo has a web surface, OR
- Drift fix (“code wins”) changes a use case and the web is affected, OR
- QA Gatekeeper requires web integration coverage.

Required for web use-case changes.

## Inputs

- Use-case doc(s): `/docs/use-cases/UC-###-*.md`
- Repo’s existing E2E conventions (if any)
- Local run entrypoints (Makefile preferred)
- If docker-compose exists: base URLs/services from compose

## Outputs

- Playwright specs added/updated
- Spec filename includes UC ID (required):
  - `uc-###-<kebab-title>.spec.(ts|js)` (default)
- Specs follow repo convention if present; otherwise standardize to `/tests/e2e/`
- CI can run Playwright (or explicit MCP blockers if missing credentials/env)

## Rules

- Prefer existing framework/location if already present.
- If no E2E framework exists, standardize on Playwright.
- UC ID must be in the spec filename (required by Gaia).
- Keep specs stable: prefer role/text selectors; avoid brittle CSS selectors.
- Do not paste large test outputs in chat.

## Step 1 — Determine test location & runner

1. If repo already has E2E folder/convention: use it.
2. Else use default: `/tests/e2e/` (Gaia standard).

Confirm how tests are invoked:

- Prefer `make test` (or `make test-e2e` if repo splits).

If no Makefile: create a task to add it (and do not proceed to “done” without it if gated).

## Step 2 — Install/configure Playwright (if missing)

- Add Playwright dependency and config per stack conventions.
- Ensure a single command can run tests headless in CI.
- Ensure base URL configuration is environment-driven (no hardcoded hostnames).

## Step 3 — Translate UC acceptance criteria into specs

For each affected UC:

- Cover the main happy path.
- Cover at least one critical edge/validation/auth case when relevant.
- Assert observable outcomes (UI states, navigation, key content, API-driven UI updates).

Name specs:

- `uc-###-<kebab-title>.spec.ts`

## Step 4 — Data + environment strategy

- Prefer test IDs / seed data that can be created deterministically.
- If the app needs auth:
  - Use Playwright storage state if supported.
  - If credentials required and unavailable: add MCP blockers/questions and keep completion gated.

## Step 5 — Wire to Makefile + CI

- Add Make targets (preferred):
  - `make test` includes unit + e2e only if repo policy allows
  - or `make test-e2e` and CI calls it explicitly
- Update CI to run Playwright tests where feasible.

If CI cannot run E2E due to environment constraints:

- Add MCP blockers/questions.
- Still perform manual regression via Playwright MCP (separate skill).

## Step 6 — Proof (MCP, link-only)

When completing the task:

- Add spec file paths to `tests_added[]`
- If manual walkthrough was done (use-case change): include `playwright-mcp` in `manual_regression[]`
- Add any config paths changed to `changed_files[]`

## Suggested required_gates[]

- For web use-case change: `e2e`, `manual-regression`, plus baseline `lint/build/ci`
