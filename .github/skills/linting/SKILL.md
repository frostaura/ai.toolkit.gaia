---
name: linting
description: Add or extend lint/format tooling and ensure it runs locally (Makefile) and in CI. Lint/build are always required in Gaia.
---

# Linting (Add/Extend + Enforce)

## When to use

Use when:

- Repo has no lint/format tooling
- Tooling exists but isn’t enforced (no CI / no Make targets)
- New stack introduced (new language/runtime)
- QA Gatekeeper finds lint drift or regressions

Lint is **required** and **blocking**.

## Inputs

- Detected stack(s) and frameworks
- Existing config files (if any)
- Existing CI and Make targets

## Outputs

- Lint/format tool configured for the stack
- `make lint` exists and passes
- CI runs lint (and fails on violations)
- Repo conventions documented (briefly) in `/docs/testing/how-to-run.md` (or equivalent)

## Rules

- Prefer existing repo conventions if present.
- If none exist, use stack-default skills for the detected platform.
- Do not introduce multiple linters unless there is a clear need.
- Avoid noisy rules that cause churn; prioritize correctness and consistency.

## Step 1 — Detect current lint/format state

Look for:

- JS/TS: ESLint, Prettier, Biome
- .NET: dotnet format, analyzers, EditorConfig
- Python: ruff, black, isort
- Flutter/Dart: `dart format`, `dart analyze`, `flutter analyze`
- MAUI: .NET analyzers + formatting + EditorConfig

Record:

- Tool(s)
- Config file location
- How it’s invoked today

## Step 2 — Choose the canonical commands

Prefer Make targets:

- `make lint` (required)
- Optionally: `make format` (if repo supports auto-fix)
  If Makefile missing: create a task to add it (blocking if repo needs to run locally).

## Step 3 — Add/extend configuration (by stack)

### JS/TS (web)

- Ensure a single “lint” entrypoint exists
- Configure formatting either via Prettier/Biome
- Ensure lint fails on errors (and warnings if repo policy requires)

### .NET (API/MAUI)

- Ensure `.editorconfig` exists
- Use `dotnet format` (or analyzers) as the canonical lint/format step
- Enforce analyzers at build (as appropriate)

### Python

- Prefer ruff as a unified linter/formatter where feasible
- Ensure `python -m ...` commands are stable across environments

### Flutter

- Use `dart format` + `dart analyze` (or `flutter analyze`) as canonical steps

## Step 4 — Wire into CI

- Add lint to CI workflow(s)
- Ensure CI uses the same command as local Make targets (`make lint`)

## Step 5 — Validate and minimize churn

- Run lint locally
- Apply autofix only if repo policy supports it
- Avoid massive formatting-only diffs unless specifically requested or necessary

## Step 6 — Update docs + skills (skill drift is blocking)

- Update `/docs/testing/how-to-run.md` to include lint commands (Make targets).
- If you changed conventions used in any skill docs, update those skills in the same change set.

## Suggested required_gates[]

- Always include: `lint`, `ci`
- Usually includes: `build` (to ensure analyzers/build rules are compatible)
