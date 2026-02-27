# GitHub Copilot — Repository Instructions (Gaia)

## Operating mode

- Follow Gaia’s SDLC. `/docs/` is the source of truth.
- ALWAYS delegate to **Repo Explorer** first to survey reality (docs/code/CI/lint/tests/docker/Makefile).
- If docs ↔ code drift exists: STOP feature work and fix drift autonomously first.
- If CI is missing or failing: fix CI first.
- If HTTP API and docker-compose missing: add docker-compose + `.env.example` + Make targets first.

## Planning & tasking (orchestrator is supreme)

- The **Workload Orchestrator** owns the plan and the task graph.
- Capture all work as tasks: foundations, docs, implementation, tests, QA review.
- Add tasks in-flight when new TODOs/risks/gaps appear.
- No “TODO left behind”: convert TODOs to MCP tasks or blockers.

## Quality gates (baseline + triggers)

Baseline (always):

- Lint/format + build must pass.
- CI must exist and run the relevant checks.

Use-case change trigger (new/change/remove use cases):

- Require integration/E2E coverage:
  - Web: add/update Playwright specs (prefer existing conventions; else standardize).
  - Manual regression is required:
    - Backend: curl-style checks against the docker-compose stack.
    - Web: Playwright MCP manual walkthrough.
- If required tests/regression cannot be run: task completion is blocked.

## Completion is MCP-gated (proof is link-only)

- Marking a task DONE must be done via MCP task tools.
- MCP completion proof args are required:
  - `changed_files[]` (paths)
  - `tests_added[]` (paths)
  - `manual_regression[]` (labels e.g. `curl`, `playwright-mcp`)
- Do NOT paste large logs. Proof is paths/labels only.

## Agents (use them)

- Use subagents/custom agents for separation of concerns:
  - Repo Explorer: repo survey + suggested tasks.
  - Architect: design + `/docs/architecture/`.
  - Developer: implement + keep conventions consistent.
  - Tester: write/update tests (unit/integration/e2e).
  - Quality Gatekeeper: independent verification; can veto.
  - Analyst: clarify acceptance criteria/risks when needed.

## Skills (procedures live in skills)

- Prefer using Skills for repeatable workflows (linting, CI baseline, dockerize, Playwright, regression, doc derivation).
- If repo conventions change, update all affected skills in the same change set.
- Skill drift is blocking (fix skills before proceeding).

## Output discipline (context hygiene)

- Keep responses concise and action-oriented.
- Summaries: 1 short paragraph max at the end (docs touched, code touched, tests, manual regression labels).
- Avoid dumping tool output; reference file paths and commands instead.
