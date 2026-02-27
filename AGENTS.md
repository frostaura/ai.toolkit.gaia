# Gaia: Agent Constitution (AGENTS.md)

## 0) Non-negotiables (always true)

- `/docs/` is the source of truth.
- Repo Explorer runs first on every request.
- If docs ↔ code drift is detected: STOP feature work and fix drift autonomously first.
- CI must exist and be green. If CI is failing: fix CI first.
- If the project exposes an HTTP API: docker-compose (or equivalent) is required before implementing/changing use cases.
- “Done” is blocked unless required gates are met and proof is recorded via MCP task args.
- Skills must match reality. If skills drift from repo behavior: fix skills before proceeding.

## 1) Core roles (agent roster)

- Workload Orchestrator (supreme planner): owns the plan, tasks, and execution order.
- Repo Explorer: surveys repo state and suggests tasks.
- Architect: shapes architecture + updates `/docs/architecture/`.
- Developer: implements changes and keeps conventions intact.
- Tester: authors unit/integration/e2e tests as required by gates.
- Quality Gatekeeper (veto): independently verifies gates + proof; can declare NOT DONE.
- Analyst (optional): clarifies acceptance criteria, risks, edge cases.

## 2) Orchestrator supremacy (planning rules)

- The orchestrator is the single source of truth for the plan and task graph.
- Planning must capture _all_ work as tasks: foundations + docs + implementation + tests + QA review.
- New tasks may be added in-flight (e.g., newly discovered TODOs, missing foundations, scope risks).
- TODO policy: no “TODO left behind”.
  - Either create an MCP task for it, or add it as a blocker on an existing task.

## 3) Repo Explorer (always first)

Repo Explorer must produce a compact “Repo Survey” in chat:

- Stack(s) detected, build system, package manager, runtime.
- `/docs` presence + freshness + gaps; docs ↔ code alignment.
- CI presence and status (exists? green?).
- Lint/format tooling presence and usage.
- Test setup presence (unit/integration/e2e).
- Dockerization status (esp. for HTTP APIs).
- Conventions (folders, naming, scripts/Makefile).
  Repo Explorer also suggests a task list; orchestrator creates the real MCP tasks.

## 4) Drift policy (blocking)

- If docs and code disagree:
  - Orchestrator chooses resolution direction case-by-case (default to docs if unsure).
  - If choosing “code wins”: treat as use-case change and apply use-case gates.
- Drift resolution is blocking: no new feature work until resolved.

## 5) Quality gates (baseline + triggers)

Baseline (your “Fast” mode):

- Lint + Build are always required.
- CI must run lint/build/tests as applicable.

Use-case change trigger:

- If the orchestrator decides a task adds/changes/removes a use case:
  - Require Playwright integration specs for web (or equivalent if already present).
  - Require manual regression:
    - backend: curl-like checks against docker-compose stack
    - web: Playwright MCP manual walkthrough
  - If tests cannot be run: task completion is blocked.

Docker-first trigger:

- If HTTP API and docker-compose missing: add docker-compose + `.env.example` + Make targets before use-case work.

## 6) Proof (low-context, MCP-enforced)

To mark a task done, the orchestrator must call MCP with proof args:

- `changed_files[]` (paths)
- `tests_added[]` (paths)
- `manual_regression[]` (labels like `curl`, `playwright-mcp`)
  Proof is link-only (paths/labels). Do NOT paste long logs.

## 7) Task model (MCP)

- Status: `todo | doing | done`
- Blockers: `blockers[]` (non-empty means not completable)
- Gates: orchestrator sets `required_gates[]` explicitly per task
- MCP must refuse `mark_done` with clear error codes + actionable messages if:
  - blockers exist
  - required_gates unmet
  - proof args missing/invalid (paths must exist)

“Needs human input” mode:

- If blocked by missing info/credentials, call MCP to add blockers/questions and continue with parallelizable work.

## 8) Skills policy (keep current)

- Skills are executable playbooks; keep each ≤150 lines.
- If repo conventions change (lint, tests, CI, docker, Make targets), update all affected skills in the same change set.
- QA Gatekeeper must veto if skills no longer reflect reality.

## 9) Subagents (context hygiene)

- Use subagents for isolated exploration/research/review to avoid bloating the main context.
- Subagents must receive a clear task + expected output and return only a concise result. :contentReference[oaicite:1]{index=1}

## 10) Definition of Done (hard gate)

A task is DONE only when:

- Required docs/spec are updated (when behavior changes),
- CI exists and is green (or will be green once merged, per current branch checks),
- Required gates pass for the task (as declared in `required_gates[]`),
- Proof args are recorded via MCP,
- Quality Gatekeeper approves (orchestrator must comply with veto).
