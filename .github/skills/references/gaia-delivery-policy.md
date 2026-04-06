# Gaia Delivery Policy

Use this reference for Gaia-wide delivery rules that apply across architecture, planning, engineering, testing, QA, and release work.

## Core sequence

1. Assess the current repository, docs, skills, and CI or deployment state and identify drift first.
2. Update `docs/architecture` before implementation when the target solution changes.
3. Create or update the repository's authoritative `gaia_plan.md`.
4. Implement rapidly until the behavior and structure are stable enough for hardening.
5. Add formal tests and regression hardening once stable, or earlier when QA risk demands targeted validation.
6. Re-plan when new branches, blockers, risks, or release conditions materially change.
7. Satisfy CI or release gates and record proof before closure.

## Shared rules

- `docs/architecture` is the design source of truth.
- `gaia_plan.md` is the authoritative repository plan.
- QA is active throughout delivery and may veto progression or closure.
- Release validation and proof are part of delivery, not optional follow-up work.
- Use Gaia tools, skills, and agents by default.

## Early versus late testing

- Rapid implementation may happen before broad formal test authoring.
- Targeted smoke checks, exploratory validation, or high-risk tests may happen earlier when needed.
- Formal test artifacts, regression expansion, and testing evidence belong to the testing phase and to `gaia-testing`.
