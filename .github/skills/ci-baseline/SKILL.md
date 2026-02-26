# SKILL: CI Baseline (GitHub Actions)

## Goal
Ensure CI exists and is green. Missing/failing CI is blocking.

## Steps
1) Add workflow(s) that run on PR + main
2) Run:
   - lint/format
   - build
   - tests (as available)
3) Keep commands consistent with Make targets if present

## Outputs
- `.github/workflows/*.yml`

## Done when
- CI exists and is expected to pass for the repo baseline
