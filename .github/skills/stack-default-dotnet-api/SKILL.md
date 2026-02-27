---
name: stack-default-dotnet-api
description: "Opinionated baseline for .NET HTTP APIs: analyzers/formatting, unit+integration strategy, docker-compose, Makefile targets, and CI wiring."
---

# Stack Default: .NET HTTP API

## When to use

Use when:

- Repo is a .NET HTTP API and foundations are missing/incomplete
- Bootstrapping a new API
- Preparing for use-case work (docker-first required)

## Baseline outcomes

- Formatting/analyzers enforced (lint)
- Makefile provides canonical commands
- docker-compose exists (required for HTTP API use-case work)
- CI runs lint/build/test
- Docs updated for run/test

## Preferred tools (default choices)

- Format/lint: `.editorconfig` + `dotnet format` (or established analyzers)
- Tests: `dotnet test`
- Integration: HTTP boundary tests where feasible + curl regression via compose for use cases

Rule: Prefer existing repo conventions if present.

## Required files / locations

- `Makefile`
- `.editorconfig`
- `.github/workflows/ci.yml`
- `docker-compose.yml` + `.env.example` (required for HTTP API)
- `/docs/testing/how-to-run.md`

## Make targets (required)

- `make lint` (e.g., `dotnet format --verify-no-changes` or analyzer-based)
- `make build` (e.g., `dotnet build`)
- `make test` (e.g., `dotnet test`)
- `make up` / `make down` (docker-compose)

Optional:

- `make test-integration` (if repo splits)

## Step 1 — Lint/format baseline

- Add `.editorconfig` if missing
- Add analyzers/format step
- Ensure `make lint` fails on violations

## Step 2 — Build/test baseline

- Ensure `dotnet build` succeeds
- Ensure `dotnet test` runs
- If no tests: add minimal unit test project aligned to solution structure

## Step 3 — Docker baseline (required)

- Add Dockerfile for API if missing
- Add `docker-compose.yml` + `.env.example`
- Ensure `make up` brings API up and it responds

## Step 4 — CI wiring

- CI runs `make lint`, `make build`, `make test`
- Ensure SDK version is pinned via `global.json` if repo policy prefers it

## Step 5 — Docs alignment

Update `/docs/testing/how-to-run.md`:

- Make targets
- how to run compose
- how to run tests
- where API is reachable

## Suggested required_gates[]

- Baseline setup: `lint`, `build`, `ci`
- If adding tests: add `unit`
- If adding compose integration checks: add `integration`
