---
name: stack-default-flutter
description: Opinionated baseline for Flutter apps: format/analyze, tests, Makefile targets, and CI wiring (no docker unless backend exists separately).
---

# Stack Default: Flutter

## When to use

Use when:

- Repo is Flutter and foundations are missing/incomplete
- Bootstrapping a new Flutter app
- Preparing for use-case work affecting UI flows

## Baseline outcomes

- Formatting + static analysis enforced
- Makefile provides canonical commands
- Tests run reliably
- CI runs lint/build/test as applicable
- Docs updated for run/test

## Preferred tools (default choices)

- Format: `dart format`
- Analyze: `dart analyze` or `flutter analyze`
- Tests: `flutter test`

Rule: Prefer existing repo conventions if present.

## Required files / locations

- `Makefile`
- `.github/workflows/ci.yml`
- `/docs/testing/how-to-run.md`

## Make targets (required)

- `make lint` (format check + analyze)
- `make test` (`flutter test`)
  Optional:
- `make build` (if CI builds artifacts)
- `make run` (if useful and stable)

## Step 1 — Lint baseline

- Ensure formatter is applied and CI checks formatting (no drift)
- Ensure analyzer runs and fails on issues
- Implement via `make lint`

## Step 2 — Test baseline

- Ensure `flutter test` runs via `make test`
- Add minimal widget/unit test scaffold if none exists

## Step 3 — CI wiring

- CI runs `make lint` and `make test`
- If building artifacts, add `make build` and wire in

## Step 4 — Docs alignment

Update `/docs/testing/how-to-run.md`:

- Flutter SDK prerequisites
- Make targets
- how to run tests

## Suggested required_gates[]

- Baseline setup: `lint`, `ci`
- If adding tests: add `unit`
