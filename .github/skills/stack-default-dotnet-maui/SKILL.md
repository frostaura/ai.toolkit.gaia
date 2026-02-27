---
name: stack-default-dotnet-maui
description: Opinionated baseline for .NET MAUI apps: analyzers/format, tests where feasible, Makefile targets, and CI wiring (build validation).
---

# Stack Default: .NET MAUI

## When to use

Use when:

- Repo is .NET MAUI and foundations are missing/incomplete
- Bootstrapping a MAUI app baseline
- Preparing for UI use-case work

## Baseline outcomes

- Formatting/analyzers enforced
- Build validation in CI (platform constraints acknowledged)
- Makefile provides canonical commands
- Docs updated for run/test

## Preferred tools (default choices)

- Format/lint: `.editorconfig` + analyzers + (optional) `dotnet format`
- Build: `dotnet build`
- Tests: unit tests for shared logic/projects (UI automation may be constrained)

Rule: Prefer existing repo conventions if present.

## Required files / locations

- `Makefile`
- `.editorconfig`
- `.github/workflows/ci.yml`
- `/docs/testing/how-to-run.md`

## Make targets (required)

- `make lint`
- `make build`
- `make test` (for unit tests where feasible)

## Step 1 — Lint baseline

- Add `.editorconfig` if missing
- Ensure analyzers are enforced
- Implement `make lint` (verify-no-changes if format is used)

## Step 2 — Build baseline

- Ensure `make build` runs the correct MAUI build for CI constraints
- If full MAUI build is not feasible in CI:
  - at least build shared projects + core libraries
  - add blockers/questions only if truly required for correctness

## Step 3 — Test baseline

- Ensure `make test` runs unit tests for non-UI logic
- Add minimal unit tests if none exist

## Step 4 — CI wiring

- CI runs `make lint`, `make build`, `make test`
- Document any CI limitations clearly (short, factual)

## Step 5 — Docs alignment

Update `/docs/testing/how-to-run.md`:

- prerequisites (SDK workloads)
- Make targets
- how to run tests/build

## Suggested required_gates[]

- Baseline setup: `lint`, `build`, `ci`
- If tests exist: add `unit`
