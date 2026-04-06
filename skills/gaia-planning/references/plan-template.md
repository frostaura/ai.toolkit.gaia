# Gaia Plan Template

> Use this template for the repository's single `gaia_plan.md`. This file is the authoritative implementation plan for the repository. Keep it concise, current, and aligned to the latest architecture decision.

## Document Control

| Field | Value |
|---|---|
| Request | {{request_name}} |
| Complexity | Trivial / Standard / Complex |
| Status | Draft / In Progress / Blocked / Complete |
| Owner | {{owner_or_team}} |
| Last Updated | {{yyyy-mm-dd}} |
| Architecture Reference | {{docs/architecture/... or no-op architecture decision}} |
| Related Specs | {{docs/specs/... or N/A}} |

## 1. Objective and Scope

**Objective**  
{{What this work will deliver.}}

**In Scope**
- {{deliverable or change}}
- {{deliverable or change}}

**Out of Scope**
- {{explicit non-goal}}
- {{explicit non-goal}}

## 2. Architecture Basis

**Target Solution Summary**  
{{Restate the approved architecture delta in 2-4 sentences.}}

**Constraints and Assumptions**
- {{constraint or assumption}}
- {{constraint or assumption}}

## 3. Execution Plan

| Branch / Task | Outcome | Estimate | Dependencies | Agents / Skills | Acceptance Criteria |
|---|---|---|---|---|---|
| {{branch_1}} | {{expected result}} | {{size or time}} | {{deps or none}} | {{roles}} | {{completion test}} |
| {{branch_2}} | {{expected result}} | {{size or time}} | {{deps or none}} | {{roles}} | {{completion test}} |
| {{branch_3}} | {{expected result}} | {{size or time}} | {{deps or none}} | {{roles}} | {{completion test}} |

**Sequencing / Parallelism**
- {{what must happen first}}
- {{what can run in parallel}}

## 4. QA Strategy

| QA Checkpoint | Coverage | Veto Point | Evidence Required |
|---|---|---|---|
| {{checkpoint}} | {{tests, review, validation}} | {{what blocks progress}} | {{proof}} |
| {{checkpoint}} | {{tests, review, validation}} | {{what blocks progress}} | {{proof}} |

## 5. CI / Release Gates

| Gate | Requirement | Owner | Evidence |
|---|---|---|---|
| {{ci_or_release_gate}} | {{must-pass condition}} | {{owner}} | {{artifact or result}} |
| {{ci_or_release_gate}} | {{must-pass condition}} | {{owner}} | {{artifact or result}} |

## 6. Risks, Blockers, and Open Questions

**Risks**
- {{risk and impact}}
- {{risk and impact}}

**Blockers**
- {{current blocker or none}}

**Open Questions**
- {{question needing resolution}}

## 7. Definition of Done

- {{implemented outcome}}
- {{qa expectation met}}
- {{ci or deployment gate passed}}
- {{required proof recorded}}

## 8. Proof of Completion

| Proof Item | Location / Reference |
|---|---|
| {{test results}} | {{path, PR note, artifact, or log}} |
| {{ci run or deployment validation}} | {{path, PR note, artifact, or log}} |
| {{qa sign-off}} | {{path, PR note, artifact, or log}} |

## 9. Re-Plan Triggers

- Re-publish this plan when architecture changes.
- Re-publish this plan when QA discovers a blocking issue or new branch of work.
- Re-publish this plan when dependencies, estimates, or release gates materially change.
