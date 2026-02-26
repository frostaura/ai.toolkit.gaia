# SKILL: Playwright E2E (Web)

## Goal
Add or extend Playwright as the default web E2E/integration framework.

## Location rule
- Follow existing repo convention if present; otherwise use `/tests/e2e/`.

## UC ID rule
- Spec files must include UC ID in filename by default:
  - `uc-001-<slug>.spec.ts`

## Steps
1) Install/configure Playwright
2) Add base config + reliable selectors strategy
3) Add/extend specs aligned to `/docs/use-cases/*`
4) Wire into CI

## Outputs
- Playwright config
- specs under chosen location

## Done when
- specs exist for use-case changes
- CI runs the Playwright suite (or explicit blocker if environment prevents it)
