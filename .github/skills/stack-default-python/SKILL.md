---
name: stack-default-python
description: Opinionated baseline for Python projects: ruff-based lint/format, pytest, optional dockerization for services, Makefile targets, and CI wiring.
---

# Stack Default: Python

## When to use

Use when:

- Repo is Python and foundations are missing/incomplete
- Bootstrapping a new Python project/service
- Preparing for spec-driven feature work

## Baseline outcomes

- Lint/format enforced
- Makefile provides canonical commands
- Tests run reliably
- CI runs lint/build/test as applicable
- Docs updated for run/test

## Preferred tools (default choices)

- Lint/format: ruff (prefer single-tool baseline)
- Tests: pytest
- Packaging: prefer existing (poetry/uv/pip) — do not force migration

Rule: Prefer existing repo conventions if present.

## Required files / locations

- `Makefile`
- `.github/workflows/ci.yml`
- `/docs/testing/how-to-run.md`

## Make targets (required)

- `make lint`
- `make test`
  Optional:
- `make build` (if packaging/build step exists)
- `make up/down` (if docker-compose service)

## Step 1 — Lint/format baseline

- Add ruff config (if missing)
- Ensure `make lint` runs ruff checks (and format check if used)

## Step 2 — Test baseline

- Ensure pytest runs via `make test`
- If no tests: add minimal test scaffold

## Step 3 — Service dockerization (only if HTTP API/service)

If Python exposes HTTP API:

- apply `dockerize-http-api` skill (compose required before use-case work)

## Step 4 — CI wiring

- CI runs `make lint` and `make test` (and `make build` if applicable)
- Pin Python version as appropriate

## Step 5 — Docs alignment

Update `/docs/testing/how-to-run.md`:

- env setup
- Make targets
- test invocation

## Suggested required_gates[]

- Baseline setup: `lint`, `ci`
- If build step exists: add `build`
- If adding tests: add `unit`
