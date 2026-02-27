---
name: manual-regression-web
description: Perform manual web regression using Playwright MCP tools (interactive). Required for web use-case changes. Keep evidence low-context (labels + paths only).
---

# Manual Regression (Web via Playwright MCP)

## When to use

Use when:

- Orchestrator flags **use-case change** affecting web UI, OR
- QA Gatekeeper requires manual regression, OR
- Automated E2E cannot be run in CI yet but validation is still required.

Manual regression is **required** for web use-case changes.

## Inputs

- UC docs: `/docs/use-cases/UC-###-*.md`
- Running environment (local dev server and/or docker-compose)
- Playwright MCP tool access

## Outputs

- Manual validation of UC acceptance criteria using Playwright MCP tools
- MCP proof label recorded: `playwright-mcp`
- Any added/updated Playwright specs recorded under `tests_added[]` (if created)

## Rules

- Do not paste tool logs or long transcripts.
- Validate observable outcomes aligned to UC acceptance criteria.
- If the environment cannot be run (missing secrets/ports/credentials), create blockers via MCP and keep completion gated.

## Step 1 — Ensure system is running

Preferred:

- `make up` (if docker-compose stack)
- or `make dev` / `make run` (repo convention)
  Confirm:
- Web app reachable
- Backend reachable if required for the flow

If not reachable:

- fix runtime/compose issues first (blocking)

## Step 2 — Create a UC checklist (compact)

For each affected UC:

- list 3–8 checkpoints max:
  - navigation start
  - key input(s)
  - key output(s)
  - one critical error/edge path (if relevant)

## Step 3 — Execute manual regression with Playwright MCP

Using Playwright MCP tools:

- Navigate the flow
- Capture failures precisely (what step, what expected, what happened)
- If failure found:
  - create or update automated Playwright spec if feasible
  - otherwise create a task/blocker describing the failure and required fix

Keep interaction tight:

- Prefer direct steps; avoid exploratory wandering
- Repeat only the minimal flow necessary to confirm fix

## Step 4 — Record outcomes (low-context)

Do NOT store logs.
Do:

- note which UC IDs were manually validated (in task notes if needed)
- record proof label `playwright-mcp` in MCP `manual_regression[]`

If the manual regression depended on specific local setup:

- add a short note in `/docs/testing/how-to-run.md` (only if it’s stable and reusable)

## Step 5 — Completion proof (MCP)

When marking task done:

- `manual_regression[]` must include: `playwright-mcp`
- `tests_added[]` includes any new/updated spec file paths
- `changed_files[]` includes any config/doc paths changed

## Suggested required_gates[]

- For web use-case change: `manual-regression` (and typically `e2e`) plus baseline `lint/build/ci`
