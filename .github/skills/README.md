# Gaia Skills Index

This folder contains **small, executable playbooks** used by Gaia agents.

- Keep each skill ≤150 lines.
- Skills must match repo reality. **Skill drift is blocking**: fix skills before proceeding.
- Prefer existing repo conventions; if none exist, apply the closest stack default.

## Core workflow

- **gaia-process** — End-to-end SDLC controller (Repo Explorer → drift/CI fixes → task graph → gated delivery → QA veto → MCP proof).
- **repository-audit** — Repo Explorer survey (stack, docs/code/skills drift, CI/lint/tests/docker/Makefile) + suggested tasks.
- **tasking-and-proof** — MCP task graph rules, required gates, blockers, and link-only proof args.
- **spec-consistency** — Anti-drift checks across docs, code, tests, CI, docker, Make targets.
- **doc-derivation** — Comprehensive docs derivation when code exists without trustworthy `/docs`.

## Quality foundations

- **ci-baseline** — Ensure GitHub Actions exists and is green; CI is required and blocking if failing/missing.
- **linting** — Add/extend lint/format; ensure `make lint` exists and CI enforces it.

## Runtime foundations (HTTP APIs)

- **dockerize-http-api** — Add `docker-compose.yml` + `.env.example` + Make targets; required before use-case work.
- **integration-testing-http** — Curl-style integration checks against compose stack; required for API use-case changes.

## Web testing & regression

- **playwright-e2e** — Add/extend Playwright specs; UC ID required in spec filename; follow repo conventions or standardize to `/tests/e2e/`.
- **manual-regression-web** — Manual web regression using Playwright MCP tools (label: `playwright-mcp`).
- **manual-regression-api** — Manual API regression via curl against compose stack (label: `curl`).

## Stack defaults (opinionated baselines)

- **stack-default-web-ts** — JS/TS web baseline: lint/format, tests, Playwright, Makefile, CI.
- **stack-default-dotnet-api** — .NET HTTP API baseline: analyzers/format, compose, Makefile, CI.
- **stack-default-python** — Python baseline: ruff + pytest, Makefile, CI.
- **stack-default-flutter** — Flutter baseline: format/analyze + tests, Makefile, CI.
- **stack-default-dotnet-maui** — .NET MAUI baseline: analyzers/format, build validation, unit tests where feasible, Makefile, CI.
