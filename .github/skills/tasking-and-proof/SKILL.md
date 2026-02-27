---
name: tasking-and-proof
description: How the orchestrator must create/manage MCP tasks (todo/doing/done), set required_gates, handle blockers/questions, and record low-context proof for completion.
---

# Tasking & Proof (MCP Contract)

## When to use

Use for every planned unit of work. This skill defines the **procedural workflow** for task management and proof recording.

> Task model, MCP tool signatures, error codes, and canonical gate vocabulary live in `AGENTS.md` §7, §10, §12. This skill is the procedure only.

## Orchestrator supremacy

- Only the **Workload Orchestrator** owns the authoritative task graph.
- Other agents may suggest tasks, but orchestrator calls `tasks_create`.
- Subagents can use `tasks_*` tools for their own isolated task tracking when delegated complex work.

## Step 1 — Create a complete task graph (planning)

Call `tasks_create(project, title, requiredGates[])` for each unit of work:

- Repo drift fixes (docs↔code) (blocking if present)
- Skill drift fixes (blocking if present)
- CI fixes/additions (blocking if missing/failing)
- Dockerize HTTP API (blocking for use-case work if HTTP API)
- Docs/spec changes (use-cases, architecture, testing)
- Implementation
- Tests (unit/integration/e2e as required)
- Manual regression (as required)
- QA Gatekeeper review (always)

Keep tasks small but complete; prefer multiple tasks over one mega task.

## Step 2 — Set required_gates[] explicitly (no ambiguity)

Use gate labels from `AGENTS.md §10 Canonical gate vocabulary`:

- Baseline (always): `lint`, `build`, `ci`
- Use-case change: add `integration` / `e2e` / `manual-regression` as applicable
- Docker-first: if HTTP API and compose missing → create dockerize task and gate use-case tasks on it

## Step 3 — In-flight task creation (mandatory)

When you discover TODOs, missing foundations, new scope requirements, or risky unknowns:

- Call `tasks_create` immediately or add to `blockers[]` via `tasks_update`.
- “No TODO left behind”: do not leave TODO comments without a corresponding MCP task or blocker.

## Step 4 — Blockers + “needs input” mode

Use `tasks_flag_needs_input(project, id, questions[])` when:

- secrets/credentials missing
- environment cannot run
- unclear requirements (must ask user)

Rules:

- A task with blockers cannot be marked done.
- Continue parallelizable work while waiting on input.

## Step 5 — Completion proof (MCP args only)

Call `tasks_mark_done(project, id, changedFiles[], testsAdded[], manualRegressionLabels[])`:

- `changedFiles[]`: all files modified for this task (paths only)
- `testsAdded[]`: new/updated test files (paths only)
- `manualRegressionLabels[]`: labels performed for this task (e.g. `curl`, `playwright-mcp`)

Rule: proof is **link-only** (paths/labels). Do NOT paste logs.

## Step 6 — Enforced failures (expected MCP behavior)

`tasks_mark_done` refuses with these error codes (see `AGENTS.md §7`):

- `GAIA_TASKS_ERR_BLOCKERS_UNRESOLVED` — blockers exist
- `GAIA_TASKS_ERR_NEEDS_INPUT_UNRESOLVED` — human input pending
- `GAIA_TASKS_ERR_GATES_UNSATISFIED` — required gates not met
- `GAIA_TASKS_ERR_MISSING_PROOF_ARGS` — proof arrays empty

Agents must treat these errors as instructions for next actions.

## Step 7 — QA Gatekeeper coupling

Before final completion:

- QA Gatekeeper reviews tasks for gate satisfaction + proof consistency.
- If vetoed: create/fix tasks until approved.

## References

- `AGENTS.md` (§7 Task model, §10 Gate vocabulary, §12 Memory/self-improvement)
- `.github/skills/gaia-process/SKILL.md`
- `.github/agents/gaia-quality-gatekeeper.md`
