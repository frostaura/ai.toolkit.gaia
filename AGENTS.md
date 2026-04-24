# Gaia: Agent Constitution (AGENTS.md)

## 0) Non-negotiables (always true)

- `/docs/` is the source of truth.
- Repo Explorer runs first on every request.
- If the user explicitly states stack preferences, those preferences override Gaia's default stack.
- If the request leaves the application stack unspecified and the repo has no approved override, Gaia's default tech stack must be declared before planning.
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
- The orchestrator must resolve stack choice before planning when the request and repo leave it implicit.
- Planning must capture _all_ work as tasks: foundations + docs + implementation + tests + QA review.
- New tasks may be added in-flight (e.g., newly discovered TODOs, missing foundations, scope risks).
- TODO policy: no “TODO left behind”.
  - Either create an MCP task for it, or add it as a blocker on an existing task.

## 3) Repo Explorer (always first)

Repo Explorer must produce a compact “Repo Survey” in chat:

- Stack(s) detected, build system, package manager, runtime.
- Whether the user explicitly requested a stack that overrides Gaia defaults.
- Whether the repo overrides Gaia's default stack baseline or should inherit it.
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

## 7) Task model (MCP tools)

Task tools (`tasks_*`):

- `tasks_create(project, title, requiredGates[])` — create a new task.
- `tasks_list(project)` — list all tasks and their state.
- `tasks_update(project, id, ...)` — update status/gates/blockers.
- `tasks_complete(project, id, changedFiles[], testsAdded[], manualRegressionLabels[])` — complete with proof.
- `tasks_request_input(project, id, questions[])` — block on human input.
- `tasks_delete(project, id)` / `tasks_clear(project)` — cleanup.

Memory tools (`memory_*`):

- `memory_remember(project, key, value)` — persist a stable fact.
- `memory_recall(project, key?)` — recall facts (call at session start).
- `memory_forget(project, key)` / `memory_clear(project)` — cleanup.

Evolution tools (`evolve_*`) — agents call these to continuously upgrade themselves:

- `evolve_log(project, suggestion, category?)` — log a lesson learned.
- `evolve_list(project?, category?)` — review lessons (call at session start).
- `evolve_apply(id)` / `evolve_clear(project?)` — manage backlog.

Task fields:

- `status`: `todo | doing | done`
- `blockers[]`: non-empty means not completable
- `required_gates[]`: set explicitly per task by orchestrator
- `gates_satisfied[]`: updated as gates pass
- `proof`: `changed_files[]`, `tests_added[]`, `manual_regression[]`

`tasks_complete` refuses with error codes when:

- `GAIA_TASKS_ERR_BLOCKERS_UNRESOLVED` — blockers exist
- `GAIA_TASKS_ERR_NEEDS_INPUT_UNRESOLVED` — human input pending
- `GAIA_TASKS_ERR_GATES_UNSATISFIED` — required gates not met
- `GAIA_TASKS_ERR_MISSING_PROOF_ARGS` — proof arrays empty

"Needs human input" mode:

- Call `tasks_request_input(project, id, questions[])` to block on human input.
- Continue parallelizable work while waiting.

## 8) Skills policy (keep current)

- Skills are executable playbooks; keep each ≤150 lines.
- If repo conventions change (lint, tests, CI, docker, Make targets), update all affected skills in the same change set.
- QA Gatekeeper must veto if skills no longer reflect reality.

## 9) Subagents (context hygiene)

- Use subagents for isolated exploration/research/review to avoid bloating the main context.
- Subagents must receive a clear task + expected output and return only a concise result.
- Subagents can use `tasks_*` tools for their own isolated task tracking when delegated complex work.

## 10) Canonical gate vocabulary

Use these labels consistently in `required_gates[]` and `gates_satisfied[]`:

| Gate                | Meaning                                |
| ------------------- | -------------------------------------- |
| `lint`              | Lint/format passes                     |
| `build`             | Project builds successfully            |
| `ci`                | CI workflow is green                   |
| `unit`              | Unit tests pass                        |
| `integration`       | Integration tests pass (HTTP boundary) |
| `e2e`               | E2E / Playwright specs pass            |
| `manual-regression` | Manual regression performed            |
| `docs-updated`      | Docs reflect behavior changes          |
| `docker-ready`      | docker-compose stack runs              |

Manual regression labels (for `manual_regression[]` proof):

- `curl` — API regression via curl against compose stack
- `playwright-mcp` — Web regression via Playwright MCP tools

## 11) Definition of Done (hard gate)

A task is DONE only when:

- Required docs/spec are updated (when behavior changes),
- CI exists and is green (or will be green once merged, per current branch checks),
- Required gates pass for the task (as declared in `required_gates[]`),
- Proof args are recorded via `tasks_complete`,
- Quality Gatekeeper approves (orchestrator must comply with veto).

## 12) Memory and evolution (use aggressively)

- **Every session start**: call `memory_recall(project)` and `evolve_list()` to load context.
- **Repo Explorer**: `memory_remember` discovered conventions (build commands, env vars, stack details).
- **After mistakes/inefficiencies**: call `evolve_log` to record lessons for future sessions.
- **After applying a lesson**: call `evolve_apply` to close the loop.
