---
name: gaia-engineering
description: Use this skill to execute planned implementation work, keep a branch coherent while it changes, and stabilize behavior before formal hardening and release evaluation. Use it when a current plan branch is ready for code or definition-file edits, when implementation-side cleanup is needed to preserve the branch, or when targeted early QA support is useful during delivery. Do not use it to redefine architecture, replace planning, or make broad release decisions. Its output is implemented work that respects the approved target solution, leaves a clean testing handoff, and calls out any upstream design or planning mismatch instead of masking it locally.
license: MIT
---

# Gaia Engineering

## Scope and when to use

Use this skill to deliver the branch that the plan already made explicit.

Use this skill when:

- a planned branch is ready for code or configuration changes
- Gaia definition files need direct edits after the operating model is approved
- localized stabilization is needed before formal QA
- the work needs practical implementation standards and execution discipline

Do not use this skill when:

- architecture or plan prerequisites are missing
- the main problem is broad regression strategy or final release approval
- the change requires inventing a new target solution instead of implementing the approved one

## Required inputs

- current architecture basis and plan branch
- repo structure and toolchain constraints
- local runtime, validation, and build expectations
- adjacent risk areas or known downstream QA concerns

## Owned outputs

- implemented branch changes aligned to the plan
- localized stabilization and branch-coherence fixes
- concise testing and rollout handoff notes
- explicit upstream escalation when the true issue is not local implementation

## Decision tree

- If the branch is ready, implement the smallest complete set of changes that satisfies the plan.
- If the work reveals a design contradiction, stop and route to architecture.
- If the work reveals a missing dependency or acceptance model, route to planning.
- If the branch is stable enough for targeted early QA, involve testing deliberately rather than waiting blindly.
- If the branch is stable enough for formal validation, hand it off with clear notes.

## Core workflow

1. Review the active branch, acceptance criteria, and architecture invariants.
2. Implement the required code or definition-file changes.
3. Stabilize the branch by addressing tightly coupled issues caused by the change.
4. Run the existing build or validation commands needed to keep the branch coherent.
5. Prepare a direct testing handoff with known risk areas and assumptions.

## Implementation guardrails

- keep edits aligned with the approved architecture and current plan branch
- fix tightly coupled breakage caused by the change, but do not expand scope casually
- preserve explicit configuration and toolchain conventions already used in the repo
- stop and escalate when the branch cannot be completed without redesign or re-planning

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| design mismatch | stop and surface the contradiction | engineer | send to architect |
| sequencing gap | request a plan update | engineer | send to planner |
| unstable branch | continue local stabilization | engineer | block formal QA handoff |
| release-only blocker | surface the concern for gate review | engineer | involve release after QA |

## Anti-patterns

- do not silently redesign the system during implementation
- do not patch around a planning problem with hidden local assumptions
- do not hand off an unstable branch just to move work forward
- do not grow branch scope without reflecting it back into the plan

## Handoff and downstream impact

- tell testing what changed, what is risky, and what still needs evidence
- tell release about any remaining gate or rollout concerns that are not code defects
- tell planning when the branch revealed a hidden dependency or acceptance gap
- tell architecture when the requested change conflicts with the documented target solution

## Examples

- **Good fit:** rewrite Gaia's agent or skill files to the new template after the architecture and contract are approved.
- **Good fit:** make the local definition edits and run the existing build to ensure the repo stays healthy.
- **Not a fit:** decide whether Gaia should adopt a new role or source-of-truth order.

## Completion checklist

- the branch matches the approved architecture and plan
- local stabilization is complete enough for formal QA
- upstream mismatches are surfaced instead of hidden
- the testing handoff is explicit and actionable

## References

- [Gaia delivery policy](../references/gaia-delivery-policy.md)
- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
- [Repo structure reference](references/repo-structure.md)
