# Gaia Operating Contract (AGENTS)

This file is the **source of truth** for how Gaia agents collaborate.

## Non-negotiables

1) **Docs-first truth**
- `/docs/` is authoritative.
- Docs/code drift is **blocking**. Gaia fixes drift autonomously before feature work.

2) **Repo Explorer always first**
- Every request starts with a Repo Explorer survey.
- Orchestrator does not plan until survey completes.

3) **Orchestrator supremacy**
- Workload Orchestrator owns the plan and the canonical MCP task graph.
- Other agents may suggest tasks; orchestrator creates/edits the real tasks.

4) **CI is mandatory and must be green**
- Missing CI or failing CI is **blocking** and fixed first.

5) **HTTP API work is docker-first**
- If the project exposes an HTTP API and docker-compose is missing, add it **before** implementing/changing use cases.
- Standard: `docker-compose.yml` at repo root + `.env.example` + `Makefile` targets.

6) **Use-case changes require heavier verification**
When work adds/changes/removes a use case:
- Add/update web integration specs (Playwright) where applicable.
- Run manual regression:
  - backend via curl against docker stack
  - web via Playwright MCP tools

7) **Completion is enforced by MCP**
A task is only “done” if:
- docs updated when behavior changes
- required gates satisfied
- proof args recorded in MCP `mark_done` call (paths/labels only)
- blockers resolved

8) **Skill drift is blocking**
If repo conventions change, update all affected skills in the same effort.

## Use-case change decision
- Orchestrator decides whether change is a use-case change.
- If uncertain: default to YES.
- Record a 1-line rationale.

## Task graph rules
- Planning must comprehensively capture all required work as tasks.
- New tasks may be added in-flight for discovered TODOs/risks.
- No orphan TODOs: convert into tasks or blockers.

## Proof policy (low-context)
MCP `mark_done` requires:
- `changed_files[]` (paths)
- `tests_added[]` (paths)
- `manual_regression[]` (labels like `curl`, `playwright-mcp`)

## Completion summary
Orchestrator writes one paragraph max:
- docs touched
- code touched
- tests paths
- manual regression labels
- how to run locally (1–2 commands)
