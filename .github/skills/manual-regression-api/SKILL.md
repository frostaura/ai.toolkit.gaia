---
name: manual-regression-api
description: Manual backend regression via curl-like HTTP checks against the docker-compose stack. Required for API use-case changes (label: curl).
---

# Manual Regression (API via docker-compose + curl)

## When to use

Use when:

- Orchestrator flags **use-case change** affecting an HTTP API, OR
- Drift fix (“code wins”) changes behavior/use-cases, OR
- Automated integration tests are not yet feasible but validation is required.

Manual regression is **required** for API use-case changes.

## Inputs

- Use-case docs: `/docs/use-cases/UC-###-*.md`
- Running docker-compose stack (required)
- API base URL/port
- Auth credentials/tokens (if needed)

## Outputs

- Manual verification that UC acceptance criteria hold at the HTTP boundary
- MCP proof label recorded: `curl`
- Any automated integration tests added recorded under `tests_added[]` (if created)

## Rules

- Must run against docker-compose stack (docker-first prerequisite).
- Do not paste response bodies/logs in chat.
- If secrets/credentials are missing: add MCP blockers/questions and keep completion gated.

## Step 1 — Start the stack

Preferred:

- `make up`
  Confirm:
- API reachable (health endpoint if available)
- dependencies healthy

If not reachable:

- fix compose/runtime first (blocking)

## Step 2 — Create a UC checklist (compact)

For each affected UC:

- list 3–8 checks max:
  - happy path request/response
  - auth failure or validation failure (if relevant)
  - key state change (if UC implies persistence)

## Step 3 — Execute curl-style checks

Perform minimal calls that prove acceptance criteria:

- Use the correct headers/auth
- Validate status codes
- Validate presence of key fields (not full payload diffs)

If you discover undocumented behavior:

- trigger spec-consistency work (drift is blocking)

## Step 4 — Handle missing info (needs input)

If you cannot complete checks due to missing:

- base URL/ports
- credentials
- seed data requirements
  Call MCP “needs input” and continue with other tasks.

## Step 5 — Completion proof (MCP)

When marking task done:

- `manual_regression[]` must include: `curl`
- `tests_added[]` includes any new integration test files created
- `changed_files[]` includes any config/doc changes

## Suggested required_gates[]

- For API use-case change: `manual-regression` (and typically `integration`) plus baseline `lint/build/ci`
