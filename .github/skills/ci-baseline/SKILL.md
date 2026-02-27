---
name: ci-baseline
description: Ensure GitHub Actions CI exists and is green. CI must run lint/format, build, and tests as applicable. Failing CI is blocking.
---

# CI Baseline (GitHub Actions)

## When to use

Use when:

- `.github/workflows/` is missing or incomplete
- CI exists but is failing
- A new stack is introduced (new language/runtime)
- QA Gatekeeper is enforcing “CI must be green”

CI is **required** and **blocking** in Gaia.

## Inputs

- Detected stack(s) (from Repo Explorer)
- Existing workflows (if any)
- Lint/build/test commands (prefer Make targets)
- Repo structure (monorepo vs single project)

## Outputs

- At least one workflow that runs on PR + main branch pushes
- CI runs: lint/format, build, tests (as applicable)
- CI uses Make targets where available (`make lint`, `make build`, `make test`)
- CI is green (or has a clear blocker task if environment secrets are required)

## Rules

- Prefer Makefile as the canonical interface.
- If Makefile missing and repo is meant to be runnable: create a task to add it.
- Keep CI simple and reliable before optimizing.

## Step 1 — Inventory existing CI

- Check `.github/workflows/*.yml`
- Identify triggers: `pull_request`, `push`
- Identify jobs for: lint, build, test
- If failing: capture the failure cause (1 line) and fix first.

## Step 2 — Decide CI entrypoints

Preferred (if Makefile exists):

- `make lint`
- `make build` (if applicable)
- `make test`

Fallback (if no Makefile yet):

- Use stack defaults (see stack-default skills) and create a task to add Make targets.

## Step 3 — Implement baseline workflow

Create/update a workflow (e.g. `.github/workflows/ci.yml`) with:

- Runs on: PRs + pushes to main
- Uses cache where standard for the ecosystem
- Runs in order:
  1. lint/format
  2. build
  3. tests

Monorepo:

- Split jobs by package/project only if necessary.
- Prefer one workflow; multiple workflows only if it improves reliability.

## Step 4 — Validate locally + in CI

- Ensure commands work locally (via Make targets)
- Push/run CI and ensure green
- If CI needs secrets/credentials:
  - add MCP blockers/questions via “needs input”
  - keep tasks blocked until resolved

## Step 5 — Keep docs and skills consistent

- Ensure `/docs/testing/how-to-run.md` reflects CI entrypoints.
- If CI commands differ from skills: update affected skills (skill drift is blocking).

## Recommended files to reference

- `.github/workflows/*.yml`
- `Makefile`
- Stack config: `package.json`, `pyproject.toml`, `.csproj`, `pubspec.yaml`
- `/docs/testing/`

## Suggested required_gates[]

- Always include: `ci`
- Usually includes: `lint`, `build`, `unit` (if tests exist)
