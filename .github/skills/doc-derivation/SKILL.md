# SKILL: Doc Derivation (Comprehensive)

## When to use
When code exists but docs are missing/outdated, or docs/code drift is detected.

## Steps
1) Inventory external behavior
   - public APIs/endpoints/CLI commands
   - core use-cases and flows

2) Write/repair docs first
   - create/update `/docs/use-cases/UC-###-*.md`
   - include: Goal, Actors, Preconditions, Main Flow, Variants, Acceptance Criteria

3) Add architecture notes
   - `/docs/architecture/` minimal but comprehensive: components, boundaries, data flow

4) Reconcile drift
   - choose direction case-by-case (if unsure: docs win)
   - if “code wins” and behavior changes: treat as **use-case change** and run heavier verification

## Outputs
- comprehensive `/docs` aligned to reality

## Done when
- docs accurately describe current system behavior
- orchestrator can proceed with feature planning without ambiguity
