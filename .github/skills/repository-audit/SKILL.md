# SKILL: Repository Audit

## Goal
Determine repo reality and identify blockers before planning.

## Checklist
- **Docs**: is `/docs/` present, structured, and current?
- **Use-cases**: `/docs/use-cases/*.md` one per UC, each with UC-### ID
- **Drift**: docs vs code mismatch? (behavior, endpoints, flows)
- **Skills drift**: do skills match actual commands/paths/conventions?
- **CI**: workflows exist? obvious failures?
- **Lint/format**: configured? enforced in CI?
- **Tests**: unit/integration/e2e present?
- **HTTP API + docker**: API present? compose present? Makefile targets?

## Outputs
- short Repo Survey
- suggested tasks + suggested `required_gates`

## Done when
- blockers are clearly identified
- suggested tasks cover foundations and drift repairs
