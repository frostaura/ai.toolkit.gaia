---
name: tasking-and-proof
description: How the orchestrator must create/manage MCP tasks (todo/doing/done), set required_gates, handle blockers/questions, and record low-context proof for completion.
---

# Tasking & Proof (MCP Contract)

## When to use

Use for every planned unit of work. This skill defines the **task graph**, **gate enforcement**, and **proof** rules required to mark tasks DONE.

## Task model (required)

- `status`: `todo | doing | done`
- `required_gates[]`: explicit per task (set by orchestrator at creation)
- `blockers[]`: non-empty means task cannot be marked done
- `proof` args (required at completion):
  - `changed_files[]` (paths)
  - `tests_added[]` (paths)
  - `manual_regression[]` (labels like `curl`, `playwright-mcp`)

Rule: proof is **link-only** (paths/labels). Do NOT paste logs.

## Orchestrator supremacy

- Only the **Workload Orchestrator** owns the authoritative task graph.
- Other agents may suggest tasks, but orchestrator creates/updates MCP tasks.

## Step 1 — Create a complete task graph (planning)

For each request, ensure tasks exist for:

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

Baseline gates (always):

- `lint`
- `build` (if applicable to the repo)
- `ci`

Use-case change gates (new/change/remove a use case):

- `integration` and/or `e2e` (as applicable)
- `manual-regression`
  Manual regression labels:
- backend: `curl`
- web: `playwright-mcp`

Docker-first gate condition:

- If HTTP API and compose missing → create dockerize task and gate use-case tasks on it.

## Step 3 — In-flight task creation (mandatory)

When you discover:

- TODOs
- missing foundations
- new scope requirements
- risky unknowns
  Create a new MCP task immediately or add to blockers.

“No TODO left behind”:

- Do not leave TODO comments without a corresponding MCP task or blocker.

## Step 4 — Blockers + “needs input” mode

Use blockers when:

- secrets/credentials missing
- environment cannot run
- unclear requirements (must ask user)
  Call MCP to flag needs input with `questions[]` (can be MCQ or free text).

Rules:

- A task with blockers cannot be marked done.
- Continue parallelizable work while waiting on input.

## Step 5 — Completion proof (MCP args only)

When marking a task done, provide:

- `changed_files[]`: all files modified for this task (paths only)
- `tests_added[]`: new/updated test files (paths only)
- `manual_regression[]`: labels performed for this task

Path validation:

- Only include paths that exist in the repo at completion time.

## Step 6 — Enforced failures (expected MCP behavior)

MCP `mark_done` should refuse with clear error codes + messages when:

- blockers exist (e.g., `ERR_NEEDS_INPUT_UNRESOLVED`)
- required gates not satisfied (e.g., `ERR_REQUIRED_GATES_UNMET`)
- proof args missing/invalid paths (e.g., `ERR_MISSING_PROOF_PATHS`, `ERR_PROOF_PATH_NOT_FOUND`)

Agents must treat these errors as instructions for next actions.

## Step 7 — QA Gatekeeper coupling

Before final completion:

- QA Gatekeeper reviews tasks for gate satisfaction + proof consistency.
- If vetoed: create/fix tasks until approved.

## References

- `AGENTS.md`
- `.github/copilot-instructions.md`
- `.github/skills/gaia-process/SKILL.md`
- `.github/agents/gaia-quality-gatekeeper.md`
