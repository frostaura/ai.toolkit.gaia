---
name: gaia-process
description: Provides Gaia's end-to-end workflow orchestration guidance for intake, routing, QA checkpoints, release gates, and proof expectations. Use it by routing every meaningful request through the intake → architecture → planning → engineering → testing → release sequence and only completing tasks once the corresponding gates and proof are recorded. Use it for meaningful requests, workflow resets after drift or blockers, and maintenance work that changes Gaia's operating model.
---

# Gaia Process

## Scope and when to use

Use this skill to classify work, route it through the right Gaia roles, keep QA
in the loop, and close only after release gates and proof are satisfied.

Use this skill when:

- a request is new and needs the correct workflow path
- the current delivery path is unclear or likely misrouted
- a blocker suggests the workflow should reset upstream
- Gaia's own definitions are being revised and multiple roles are involved

Do not use this skill when:

- you only need local design work and already know architecture is the sole owner
- you only need direct implementation or direct validation on a stable branch
- you are trying to bypass the owning skills for architecture, planning, testing, or release

## Required inputs

- current repository state across docs, contract, agents, and skills
- the user request, goals, constraints, and non-goals
- available CI or release-gate context
- current tasks, blockers, memory, and any known drift

## Owned outputs

- complexity classification and rationale
- initial execution graph or direct next-owner routing
- explicit QA checkpoints and veto points
- closure conditions, release gates, and proof expectations

## Decision tree

- If the request is ambiguous, keep it in intake and clarify first.
- If the user specified stack preferences, honor them and treat Gaia defaults as non-applicable unless the user asks for comparison or migration.
- If the request leaves stack choice open and the repo has no approved override, resolve Gaia's default stack baseline before planning.
- If architecture drift exists, route to architecture before planning or delivery.
- If the target solution is current but execution order is missing, route to planning.
- If a planned branch is ready, route to engineering or testing as appropriate.
- If repeated bounce-backs suggest the wrong owner is carrying the work, re-open failure ownership and re-route.
- If closure is near, ensure release gates and proof are explicit before the workflow ends.

## Core workflow

1. Inspect the repo, docs, skills, and CI or deployment signals for blocking drift.
2. Resolve whether the user or repo already chose the stack, and use Gaia defaults only when neither did.
3. Decide whether the request is trivial, standard, or complex, and say why.
4. Route the work through architecture first when the target solution changes.
5. Require planning after architecture so the execution tree is explicit.
6. Keep QA active throughout delivery, not only at the end.
7. Re-plan when new branches, blockers, or gate conditions appear.
8. Treat proof recording as part of delivery, not optional cleanup.

## Workflow checkpoints

- confirm the current source of truth before delegating downstream work
- confirm the active stack baseline or explicit user override before delegating planning or implementation
- name the current owner and the next owner explicitly at each phase boundary
- expose QA checkpoints before implementation begins, not after it ends
- verify that release gates and proof expectations exist before calling work complete

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| request ambiguity | clarify scope and constraints | intake | stay upstream until stable |
| docs or definition drift | repair the correct source of truth first | architect or maintainer | block downstream delivery |
| repeated rework loop | re-open failure ownership and complexity | intake | re-classify the work |
| missing gate model | require planning update | planner | block release claims |

## Anti-patterns

- do not use process language to avoid real ownership
- do not substitute Gaia defaults for an explicit user stack choice
- do not let planning start while stack selection is still implicit
- do not skip architecture because the change "seems small"
- do not let QA or proof become implied final steps
- do not leave the workflow linear when safe parallel branches exist

## Parallelism and handoff

- allow parallel work only after architecture and acceptance criteria are stable
- name the merge gate whenever two branches or roles can proceed in parallel
- use forward, bounce-back, or escalation handoff language explicitly
- route blockers to the true owner instead of the most recent owner by habit

## Examples

- **Good fit:** route a new Gaia maintenance request across architecture, contract, agents, skills, QA review, and release validation.
- **Good fit:** reset a workflow after repeated tester and engineer bounce-backs.
- **Not a fit:** edit a single file when architecture, plan, and owner are already obvious.

## Completion checklist

- complexity and routing decisions are explicit
- current drift, blockers, and veto points are named
- downstream owners know which artifact to trust next
- release gates and proof expectations are not left implicit

## References

- [Gaia delivery policy](../references/gaia-delivery-policy.md)
- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
