# SKILL: Linting + Formatting

## Goal
Add or extend lint/format for the detected stack and enforce it via CI.

## Defaults
- Prefer auto-fixable formatters.
- Keep the configuration minimal and conventional for the stack.

## Outputs
- lint/format config files
- Make targets (when applicable): `make lint`
- CI step(s) that run lint

## Done when
- lint exists
- lint is run in CI
- new work does not regress lint quality
