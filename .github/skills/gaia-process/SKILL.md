---
name: gaia-process
description: End-to-end Gaia SDLC workflow (Repo Explorer → drift/CI fixes → task graph → gated delivery → QA veto → MCP proof). Use for any work in a repo.
---

# Gaia Process (SDLC Controller)

## When to use

Use this skill for **every request** to ensure docs/code/CI/skills stay in sync and work is delivered with the required quality gates. Agent Skills can include instructions + referenced resources/scripts. :contentReference[oaicite:0]{index=0}

## Inputs

- User request (feature/bug/refactor/docs)
- Current repo state (unknown until Repo Explorer runs)
- Available MCP tools (tasks, memories, self-improve)

## Required outcomes

- Repo reality understood (survey)
- Drift resolved (docs ↔ code) **before** feature work
- CI exists and is green (or fixed) **before** feature work
- Task graph fully captured in MCP (with gates + blockers)
- Delivery meets gates; completion recorded via MCP proof args
- QA Gatekeeper approval (veto respected)

## Step 0 — Read the repo contracts (fast)

1. Read `AGENTS.md` (non-negotiables + roles + gating).
2. Read `.github/copilot-instructions.md` (always-on rules).
3. Identify existing conventions:
   - Make targets (preferred)
   - CI workflows
   - test folders and naming
   - docker-compose presence for HTTP APIs

## Step 1 — Repo Explorer FIRST (always)

Delegate to **Repo Explorer** and request a compact “Repo Survey”:

- Stack(s) and build system
- `/docs` presence + gaps
- docs ↔ code drift signal
- CI presence + status
- lint/format tooling
- tests (unit/integration/e2e) status
- docker-compose status (if HTTP API)
- Makefile presence and targets
- skill drift signal (skills vs reality)

Repo Explorer may include a **Suggested Task List**, but the orchestrator owns the actual MCP task graph.

## Step 2 — Hard blockers (resolve before feature work)

If any of the following are true, create blocking tasks and fix autonomously first:

- Docs ↔ code drift exists
- CI missing or failing
- Skill drift exists (skills don’t match repo reality)
- HTTP API without docker-compose (and request involves use cases)

Drift resolution direction:

- Decide case-by-case; if unsure, default to docs.
- If “code wins” implies behavior/use-case change → apply use-case gates (below).

## Step 3 — Build the Task Graph (orchestrator supremacy)

Create MCP tasks to cover **all** work:

- Foundations (CI, lint/format, docker-compose, Makefile targets)
- Docs/spec (create/derive/update)
- Implementation
- Tests (unit + integration/e2e as required)
- Manual regression (labels only)
- QA Gatekeeper review

In-flight discovery:

- If you uncover TODOs, gaps, or new risks: add tasks immediately.
- “No TODO left behind”: turn TODOs into MCP tasks or `blockers[]`.

## Step 4 — Decide gates per task (explicit)

For each MCP task, set `required_gates[]` explicitly (do not guess later):
Baseline gates (always):

- `lint`
- `build`
- `ci`

Use-case change gates (when adding/changing/removing a use case):

- `integration` (and/or `e2e` where applicable)
- `manual-regression`
  Rules:
- Web: require Playwright specs when none exist; otherwise follow existing test framework.
- Manual regression:
  - backend: curl-like checks against docker-compose stack
  - web: Playwright MCP manual walkthrough

If tests/regression cannot be run:

- call MCP to add `blockers[]` / “needs input” questions
- do parallelizable work, but completion stays blocked

## Step 5 — Execute with delegation

Delegate by intent:

- Repo survey → Repo Explorer
- Architecture / doc structure → Architect
- Implementation → Developer
- Test authoring → Tester
- Independent verification → Quality Gatekeeper

Subagent requests must be **tight** (inputs + expected output + constraints) to avoid context bloat. Clear personas and boundaries improve agent reliability. :contentReference[oaicite:1]{index=1}

## Step 6 — Completion proof (MCP-enforced, low-context)

To mark DONE for a task, call MCP `mark_done` with:

- `changed_files[]` (paths)
- `tests_added[]` (paths)
- `manual_regression[]` (labels like `curl`, `playwright-mcp`)
  Do not paste logs; store only paths/labels.
  MCP should refuse completion with clear error codes/messages when missing. :contentReference[oaicite:2]{index=2}

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

- `AGENTS.md` (repo constitution)
- `.github/copilot-instructions.md` (repo instructions)
- `.github/agents/` (role definitions)
- `.github/skills/` (procedures)
- `Makefile` (canonical local commands)
- `.github/workflows/` (CI gates)
