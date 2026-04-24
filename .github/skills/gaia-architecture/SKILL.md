name: gaia-architecture
description: Provides Gaia architecture documentation guidance that keeps docs, README messaging, and design decisions aligned to the current operating model. Use it when system structure, trust boundaries, interfaces, workflows, ownership, or architectural assumptions change, or when architecture drift blocks planning or delivery.
license: MIT
---

# Gaia Architecture

## Scope and when to use

Use this skill to keep `docs/architecture` current and concrete enough that the
rest of Gaia can plan and deliver against a stable target solution.

Use this skill when:

- the operating model, system structure, or trust boundaries change
- `docs/architecture` is missing or stale
- README messaging must be aligned to architecture changes
- planning or testing expose a design ambiguity that blocks progress

Do not use this skill when:

- the issue is only execution order or branch sequencing
- the problem is purely local implementation work
- the only remaining question is final release readiness

## Required inputs

- the current request and approved constraints
- existing `docs/architecture` content or evidence that it is absent
- relevant contract, README, agent, and skill definitions
- architecture templates and shared ownership rules

## Owned outputs

- current architecture docs under `docs/architecture`
- explicit assumptions, invariants, and out-of-scope statements
- README alignment when the high-level system story changed
- clear downstream impact notes for planners and maintainers

## Decision tree

- If no baseline exists, document the current implemented system first.
- If only the story changed but not the design, update README and architecture summaries without inventing a new delta.
- If planning cannot proceed because architecture is vague, make the missing decision explicit before handoff.
- If implementation or validation defects do not imply a design change, route them back to the true owner instead of redesigning the system.

## Core workflow

1. Assess the repo, docs, README, and operating definitions for drift.
2. Decide whether the change is a real architecture delta or a justified no-op.
3. Update the relevant architecture artifacts using the provided templates.
4. Keep the architecture docs specific enough for planning and QA to act on.
5. Align README language when the user-facing Gaia story changed materially.
6. Hand off any agent or skill maintenance implied by the new design.

## Artifact selection order

- start with system components when the runtime or ownership model changes
- add or update use cases when workflow behavior or actor responsibilities change
- update class or data views only when structural detail improves the design basis
- align README after the architecture set is internally consistent

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
|---|---|---|---|
| missing baseline | create the baseline from current repo behavior | architect | block planning until complete |
| unclear request | obtain clearer scope or constraints | intake | stay upstream |
| downstream role requests design workaround | restate the design or reject the workaround | architect | escalate unresolved contradictions |
| README drift after design change | update README after architecture stabilizes | architect | block release messaging claims |

## Anti-patterns

- do not treat architecture as optional background material
- do not solve code defects by silently changing the design
- do not leave README claims inconsistent with the target solution
- do not push planning forward on implied architecture

## Handoff and downstream impact

- tell planning which artifacts are now authoritative
- tell engineering which invariants or boundaries must not be violated
- tell testing which behaviors, actors, or flows changed materially
- tell maintainers which agent or skill definitions must align with the new design

## Examples

- **Good fit:** create the initial `docs/architecture` baseline for the Gaia plugin repository before rewriting its operating definitions.
- **Good fit:** update system boundaries after the contract adds explicit delegation and failure routing.
- **Not a fit:** publish a branch plan or implement the definition changes directly.

## Completion checklist

- affected architecture artifacts are current or explicitly no-op
- assumptions, invariants, and out-of-scope items are named
- README drift is resolved when the system story changed
- downstream roles know what changed and what artifact to read first

## References

- [Gaia ownership and conventions](../references/gaia-ownership-and-conventions.md)
- [System Components Template](references/system-components-template.md)
- [Use Cases Template](references/use-cases-template.md)
- [Class Diagrams Template](references/class-diagrams-template.md)
- [User Interface Template](references/user-interface-template.md)
