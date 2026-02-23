# Gaia: GitHub Copilot Instructions (Repository-Wide)

These instructions apply to **all Copilot usage in this repo** (VS Code Copilot Chat, Copilot CLI, and suggestions).

For agent workflow rules, permissions, delegation, and tool requirements, see **`AGENTS.md`**.

## North Star: Spec-Driven Design

- `docs/` is the single source of truth for requirements and architecture.
- Code and specs must match:
  - If you change behavior, ensure the corresponding spec exists and is current (via the Architect).
  - If a spec describes a feature, it must exist in code (via the Developer).

## Where to Look First

1. `docs/` — requirements, architecture, use cases.
2. `.github/skills/**/SKILL.md` — proven patterns and workflow playbooks.
3. Existing code/tests — repo conventions and patterns.
4. `.github/agents/*.md` — agent personas and boundaries.

## Default Quality Bar

- Keep changes small, cohesive, and reversible.
- Follow existing naming, folder structure, and patterns.
- Prefer tests for non-trivial behavior.
- Prefer explicit, simple solutions over clever abstractions.
- Don’t introduce new dependencies/frameworks without consulting the default stack:
  - `.github/skills/default-web-stack/SKILL.md`

## Routing Cheatsheet

- Spec/architecture/docs changes → **Architect**
- Code/tests/migrations/infra → **Developer**
- Bugs/perf/investigation → **Analyst**
- Validation/regression/security review → **Tester**
- Multi-step work/coordination → **Workload Orchestrator**

## Project-Scoped Tools

- All Gaia MCP tools (`recall`, `remember`, `update_task`, `read_tasks`, `clear_tasks`, `clear_memories`) require a **`projectName`** parameter. Memories and tasks are persisted per project.
- `log_improvement` accepts an optional `projectName` for context. Improvements are universal but should note which project triggered them.
- Derive the project name from the repository or workspace folder name and pass it consistently to all tool calls.
