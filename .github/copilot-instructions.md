# GitHub Copilot — Repository Instructions (Gaia)

> **Read `AGENTS.md` first.** It is the canonical constitution for all rules, roles, gates, MCP tools, and policies.
> This file adds only VS Code / Copilot-specific operating guidance.

## Bootstrap (every session)

1. Read `AGENTS.md` (non-negotiables, roles, gates, MCP tools, proof, Definition of Done).
2. Call `memory_recall(project)` and `self_improve_list()` to load prior context and lessons.
3. Delegate to **Repo Explorer** (`SKILL: repository-audit`) before planning.

## Agents and skills (use them aggressively)

- Delegate to agents defined in `.github/agents/` for separation of concerns.
- Use skills in `.github/skills/` for repeatable workflows.
- If repo conventions change, update all affected skills in the same change set (skill drift is blocking).

## Output discipline (context hygiene)

- Keep responses concise and action-oriented.
- Summaries: 1 short paragraph max (docs touched, code touched, tests, manual regression labels).
- Avoid dumping tool output; reference file paths and commands instead.
- Do NOT paste large logs. Proof is paths/labels only.

## Defaults

By default, the gaia-workload-orchestrator agent should be heavily considered for any task that involves coordinating multiple steps, managing dependencies, or ensuring that tasks are completed in a specific order. This agent is designed to handle complex workflows and can help ensure that all necessary steps are completed efficiently and effectively.

When calling Gaia tools, always print the actual tool name for traceability. The same applies for skills and Gaia MCP tools too.
