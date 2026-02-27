---
name: integration-testing-http
description: Run curl-style integration checks against the docker-compose stack for HTTP APIs. Required for use-case changes.
---

# Integration Testing (HTTP via docker-compose + curl)

## When to use

Use when:

- A task is flagged as **use-case change** (new/change/remove)
- API behavior is modified (routes, auth, payloads)
- Drift fix chooses “code wins” and affects use cases

This is **required** for use-case changes.

## Inputs

- `docker-compose.yml` stack (must exist)
- `.env.example` + local `.env` (as needed)
- Use-case file(s): `/docs/use-cases/UC-###-*.md`
- API base URL/port (from compose/Make)

## Outputs

- Integration checks that validate use-case acceptance criteria at the HTTP boundary
- Evidence recorded as MCP proof args (paths + labels only)
- If automated integration tests are added: file paths recorded in `tests_added[]`
- Manual regression label recorded in `manual_regression[]` (label: `curl`)

## Rules

- Always run against the compose stack (dockerized platform prerequisite).
- Prefer adding automated integration tests where the stack supports it.
- If automation is not feasible quickly, perform manual curl regression and keep it repeatable by documenting exact requests (briefly).
- Do not paste long outputs in chat.

## Step 1 — Bring up the stack

Preferred:

- `make up`
  Confirm:
- API is reachable
- dependencies are healthy

If not reachable:

- fix compose/runtime issues first (blocking)

## Step 2 — Map use-case acceptance criteria to HTTP checks

For each affected UC:

- Identify endpoints involved
- Identify inputs (headers/auth/body)
- Identify observable outputs (status code, body shape, key fields)
  Create a small checklist:
- “Given X, when request Y, then response Z”

## Step 3 — Execute curl checks (manual regression)

Run the minimal set that proves the UC acceptance criteria:

- Happy path
- One key failure path (auth/validation) if relevant

Record:

- command snippets in your working notes (do not paste full outputs)
- final proof as MCP labels/paths only

If auth tokens/credentials are required and missing:

- call MCP `flag_needs_input` with exact questions
- continue other tasks; keep completion blocked

## Step 4 — Add automated integration tests (when feasible)

If the repo supports an integration test harness:

- Add tests that hit the running service boundary (HTTP)
- Ensure they run via `make test` (or `make test-integration` if repo uses a split)

Prefer:

- Existing test frameworks in repo
  Fallback:
- Use stack defaults (but avoid large migrations unless necessary)

## Step 5 — Wire to CI

- Ensure CI can run integration tests if they are included in `make test`.
- If CI cannot run docker-compose due to environment constraints:
  - keep integration automation local only, but still require manual regression
  - document the limitation in a task note (do not mark done unless allowed by gates; in Gaia it remains gated)

## Step 6 — Proof & completion (MCP)

- If you added test files: include them in `tests_added[]`
- Always include manual regression label: `curl` in `manual_regression[]` for use-case changes
- Reference UC ID in notes (do not create new index files)

## Suggested required_gates[]

- For use-case changes: `integration`, `manual-regression`, plus baseline `lint/build/ci`
