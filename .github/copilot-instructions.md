# Gaia: GitHub Copilot Instructions (Repository-Wide)

These instructions apply to **all Copilot usage in this repo** (VS Code Chat, CLI, and suggestions).
For agent workflow rules, permissions, delegation, and tools, see **`AGENTS.md`**.

## North Star: Spec-Driven Design

- `docs/` is the single source of truth for requirements and architecture.
- Code ↔ spec must stay in sync: behavior changes need specs; specs need implementations.

## Where to Look First

1. `docs/` — requirements, architecture, use cases
2. `.github/skills/**/SKILL.md` — proven patterns and playbooks
3. Existing code/tests — repo conventions
4. `.github/agents/*.md` — agent personas and boundaries

## Default Quality Bar

- Small, cohesive, reversible changes.
- Follow existing naming, structure, and patterns.
- Tests for non-trivial behavior.
- Explicit > clever. No new dependencies without consulting `.github/skills/default-web-stack/SKILL.md`.

## Routing Cheatsheet

| Need | Route to |
|------|----------|
| Spec/architecture/docs | **Architect** |
| Code/tests/migrations/infra | **Developer** |
| Bugs/perf/investigation | **Analyst** |
| Validation/regression/security | **Tester** |
| Multi-step/coordination | **Orchestrator** |

## Project-Scoped Tools

- All Gaia MCP tools require **`projectName`** (derive from repo/workspace folder name).
- Common agent behaviors (recall, remember, log improvements) are defined in `AGENTS.md §5`.
