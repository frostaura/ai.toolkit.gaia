---
name: gaia-planning
description: Provides execution planning guidance that turns approved architecture into a branch-aware plan with dependencies, QA checkpoints, release gates, and proof expectations. Use it by translating the current architecture into MCP tasks (tasks_create) sequenced by branch, with explicit required_gates and blockers, then keeping the plan current as new work is discovered. Use it after architecture is current, when work needs explicit sequencing instead of informal next steps, or when new branches, blockers, or gate definitions require re-planning.
license: MIT
---

# Gaia Planning

## Scope and when to use

Use this skill to publish the execution tree that tells Gaia what can start,
what must wait, and how the work will be validated and closed.

Use this skill when:

- architecture is current and the work needs sequencing
- delivery spans multiple branches, dependencies, or owners
- QA, release, or proof expectations need to be explicit before implementation
- execution reveals new work streams that materially change the plan

Do not use this skill when:

- design is still missing or stale
- the branch only needs local implementation against an already current plan
- the only remaining work is release-ready interpretation

## Required inputs

- the approved architecture basis
- the request summary, constraints, and non-goals
- current repo, CI, deployment, and testing constraints
- any existing plan, tasks, blockers, or re-plan triggers

## Owned outputs

- a current execution plan with branch boundaries and dependencies
- explicit acceptance criteria, QA checkpoints, and release gates
- safe parallelism and merge-gate decisions
- proof expectations and re-plan triggers

## Decision tree

- If architecture is stale, send the work back before planning.
- If the work is simple but still risky, keep the plan concise but make QA and proof explicit.
- If branches are independent, mark them as parallel-safe and define the merge gate.
- If criteria or gate ownership are missing, block plan publication until they exist.
- If execution exposes new branches or blocker types, re-publish the plan instead of patching around it informally.

## Core workflow

1. Restate the target solution from architecture in a short, testable summary.
2. Break the work into delivery branches by capability, dependency, or ownership.
3. For each branch, define outcome, dependencies, owner roles, skills, and acceptance criteria.
4. Mark which branches can run in parallel and what gate recombines them.
5. Attach QA checkpoints, release gates, and proof expectations directly to the plan.
6. Name blockers, assumptions, open questions, and re-plan triggers.

## Parallelism and merge rules

- parallelize only when branch outputs do not compete for the same artifact or decision
- mark every parallel branch with the condition that allows it to start
- define the merge gate that recombines parallel work into one validated branch
- re-plan immediately when a supposedly independent branch gains a new dependency

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| stale architecture | stop and request design clarification | planner | send to architect |
| missing acceptance criteria | write or request testable outcomes | planner | block downstream work |
| dependency loop | split or re-sequence the work | planner | escalate if no clean branch model exists |
| gate ambiguity | assign gate ownership and evidence | planner | involve release if needed |

## Anti-patterns

- do not use a linear checklist when the work has real dependencies
- do not hide QA under a generic future test task
- do not delegate implementation from an undocumented target solution
- do not treat proof recording as a nice-to-have

## Handoff and downstream impact

- give engineering branch-level acceptance criteria and dependency context
- give testing the QA checkpoints and evidence model before validation starts
- give release the gates, proof expectations, and ready-state definition
- give intake clear re-plan triggers for future workflow resets

## Examples

- **Good fit:** split a Gaia definition overhaul into architecture baseline, contract rewrite, agent rewrite, skill rewrite, and validation branches.
- **Good fit:** re-plan after validation shows the current acceptance criteria are incomplete.
- **Not a fit:** decide what the architecture should be in the first place.

## Completion checklist

- each branch has an owner, outcome, and acceptance criteria
- dependencies and parallel-safe work are explicit
- QA, release, and proof work are embedded rather than implied
- blockers and re-plan triggers are visible to downstream roles

## References

- [Gaia delivery policy](../references/gaia-delivery-policy.md)
- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
- [Plan template](references/plan-template.md)
