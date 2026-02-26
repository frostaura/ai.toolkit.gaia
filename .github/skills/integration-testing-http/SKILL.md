# SKILL: HTTP Integration Testing (curl against compose)

## Goal
Prove use-cases via HTTP-level checks against the docker-compose stack.

## Steps
1) Start stack (`make up`)
2) Identify endpoints + expected responses from `/docs/use-cases/*`
3) Write integration checks (stack-appropriate):
   - curl-based scripts/tests or test runner that executes HTTP calls
4) Ensure tests can run in CI (or as close as feasible)

## Outputs
- integration test artifacts (paths recorded as proof)

## Done when
- checks validate the use-case contract at HTTP boundaries
