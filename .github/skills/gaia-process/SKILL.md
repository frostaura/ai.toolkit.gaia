---
name: gaia-process
description: End-to-end Gaia SDLC workflow (Repo Explorer → drift/CI fixes → task graph → gated delivery → QA veto → MCP proof). Use for any work in a repo.
---

# Gaia Process (SDLC Controller)

## When to use

Use this skill for **every request** to ensure docs/code/CI/skills stay in sync and work is delivered with the required quality gates.

> All rules, gates, roles, and MCP tool signatures live in `AGENTS.md`. This skill is the **procedural workflow only**.

## Inputs

- User request (feature/bug/refactor/docs)
- Current repo state (unknown until Repo Explorer runs)
- Available MCP tools: `tasks_*`, `memory_*`, `self_improve_*` (see `AGENTS.md §7`)

## Step 0 — Load context (fast)

1. Read `AGENTS.md` (canonical rules, roles, gates, MCP tools, proof, Definition of Done).
2. Call `memory_recall(project)` and `self_improve_list()` to load prior context and lessons.
3. Identify existing conventions (Make targets, CI workflows, test folders, docker-compose).

## Step 1 — Repo Explorer FIRST (always)

Delegate to **Repo Explorer** (`SKILL: repository-audit`) for a compact "Repo Survey":

- Stack(s) and build system
- `/docs` presence + gaps
- docs ↔ code drift signal
- CI presence + status
- lint/format tooling
- tests (unit/integration/e2e) status
- docker-compose status (if HTTP API)
- Makefile presence and targets
- skill drift signal (skills vs reality)

Repo Explorer stores discovered conventions via `memory_remember(project, key, value)`.
Orchestrator owns the actual MCP task graph.

## Step 2 — Hard blockers (resolve before feature work)

If any of the following are true, create blocking tasks and fix autonomously first:

- Docs ↔ code drift exists
- CI missing or failing
- Skill drift exists (skills don’t match repo reality)
- HTTP API without docker-compose (and request involves use cases)

Drift resolution direction:

- Decide case-by-case; if unsure, default to docs.
- If “code wins” implies behavior/use-case change → apply use-case gates.

## Step 3 — Build the Task Graph (orchestrator supremacy)

Call `tasks_create(project, title, requiredGates[])` for **all** work:

- Foundations (CI, lint/format, docker-compose, Makefile targets)
- Docs/spec (create/derive/update — use templates in `/docs/`)
- Implementation
- Tests (unit + integration/e2e as required)
- Manual regression (labels only)
- QA Gatekeeper review

In-flight discovery:

- If you uncover TODOs, gaps, or new risks: call `tasks_create` immediately.
- “No TODO left behind”: turn TODOs into MCP tasks or `blockers[]`.

## Step 4 — Decide gates per task (explicit)

For each MCP task, set `required_gates[]` explicitly per `AGENTS.md §10 Canonical gate vocabulary`.

If tests/regression cannot be run:

- call `tasks_flag_needs_input(project, id, questions[])` to block on human input
- do parallelizable work, but completion stays blocked

## Step 5 — Execute with delegation

Delegate by intent:

- Repo survey → Repo Explorer
- Architecture / doc structure → Architect
- Implementation → Developer
- Test authoring → Tester
- Independent verification → Quality Gatekeeper

Subagent requests must be **tight** (inputs + expected output + constraints) to avoid context bloat.

## Step 6 — Completion proof (MCP-enforced, low-context)

Call `tasks_mark_done(project, id, changedFiles[], testsAdded[], manualRegressionLabels[])`:

- `changedFiles[]` (paths)
- `testsAdded[]` (paths)
- `manualRegressionLabels[]` (labels like `curl`, `playwright-mcp`)

Do not paste logs; store only paths/labels.
Store discovered conventions via `memory_remember(project, key, value)`.
Log lessons learned via `self_improve_log(project, suggestion, category)`.

## Step 7 — QA Gatekeeper review (veto)

Before final completion:

- Ask Quality Gatekeeper to verify:
  - blockers empty
  - gates satisfied
  - CI present + green
  - docker-compose present if HTTP API
  - use-case triggers handled correctly
  - proof args are complete and consistent
    If vetoed → create/fix tasks until approved.

## Step 8 — Final response discipline

End with **one short paragraph**:

- docs touched
- code touched
- tests added/updated (paths)
- manual regression labels performed
- how to run locally (Make targets)

## References (read/consult)

- `AGENTS.md` (repo constitution — canonical rules, MCP tools, gates)
- `.github/agents/` (role definitions)
- `.github/skills/` (procedures)
- `Makefile` (canonical local commands)
- `.github/workflows/` (CI gates)
