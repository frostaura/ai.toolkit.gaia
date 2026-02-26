# Copilot Instructions (Repo-Level)

## Always-on rules
- Start with **Repo Explorer**.
- `/docs/` is source of truth; any drift is **blocking** and fixed first.
- Orchestrator owns the plan and MCP task graph.
- CI must exist and be green; missing/failing CI is **blocking**.
- HTTP APIs are docker-first (compose + Makefile) before use-case changes.
- Use-case changes require Playwright specs (web) + manual regression (curl + Playwright MCP).
- Completion is enforced by MCP `mark_done` proof args.
- Skill drift is blocking.

## Routing
- Planning + task graph → Workload Orchestrator
- Repo survey → Repo Explorer
- Enforcement / veto → Quality Gatekeeper
- Architecture → Architect
- Implementation → Developer
- Tests → Tester
- Requirements analysis → Analyst
