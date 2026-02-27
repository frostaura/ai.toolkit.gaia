---
name: dockerize-http-api
description: Add docker-compose for HTTP APIs (required before implementing/changing use cases). Provide .env.example + Make targets: up/down/test/lint/build.
---

# Dockerize HTTP API (Compose-First)

## When to use

Use when:

- The repo exposes an HTTP API and docker-compose is missing or incomplete
- Use-case work is planned (new/change/remove use cases)
- QA Gatekeeper enforces docker-first for HTTP APIs

This is **blocking** for use-case work.

## Inputs

- Detected API stack (.NET / Node / Python / etc.)
- Existing Dockerfile(s) or containerization
- Runtime dependencies (DB, cache, queues)
- Environment variables and secrets requirements

## Outputs

- `docker-compose.yml` at repo root
- `.env.example` at repo root
- Make targets:
  - `make up` / `make down`
  - `make build` (if applicable)
  - `make test` (runs tests appropriate to repo; at least unit)
  - `make lint`
- Docs updated: `/docs/testing/how-to-run.md` reflects compose + Make usage

## Rules

- Prefer repo conventions if present; otherwise standardize.
- Compose must be runnable by default with minimal setup.
- Do not hardcode secrets; use `.env.example`.
- Keep services minimal; add only what the API needs to run and be tested.

## Step 1 — Define the runtime stack

Identify:

- App service (the HTTP API)
- Required dependencies (DB/cache/etc.)
- Ports, health endpoints, migrations/seed steps (if needed)

## Step 2 — Add container build/run for the API

Prefer:

- A Dockerfile for the API service (if missing, add one)
- Deterministic builds (pin base images when appropriate)

Ensure:

- container starts the API reliably
- logs go to stdout/stderr
- healthcheck exists if feasible

## Step 3 — Create docker-compose.yml (root)

Include:

- `api` service
- dependency services as needed
- volumes only when necessary
- exposed ports for local dev/testing
- network defaults (keep simple)

## Step 4 — Create `.env.example` (root)

Include:

- non-secret defaults where safe
- placeholders for secrets (`CHANGE_ME`)
- ports and connection strings with compose service names

## Step 5 — Add Make targets (required UX)

Create/extend `Makefile`:

- `up`: start compose
- `down`: stop compose + cleanup as appropriate
- `lint`: run canonical lint
- `build`: build the project (if applicable)
- `test`: run canonical tests (at least unit)
  Optional:
- `logs`: follow logs
- `smoke`: quick curl check if repo supports it

## Step 6 — Validate compose stack

- `make up` brings system up
- API responds on expected port
- Dependency services reachable
  If credentials/secrets required:
- add MCP blockers/questions via “needs input”
- do parallelizable work, but keep completion blocked

## Step 7 — Align docs + skills (blocking if drift)

- Update `/docs/testing/how-to-run.md` with:
  - `make up/down`
  - env setup
  - where API is reachable
- If this changes any baseline conventions, update affected skills (skill drift is blocking).

## Suggested required_gates[]

- Always include: `build`, `ci`
- Usually includes: `lint`
- If compose is used for integration tests: include `integration`
