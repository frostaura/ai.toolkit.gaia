# Gaia: Agent Workflow Contract (AGENTS.md)

This file defines **how Gaia runs agentic workflows** in this repository.
It complements `.github/copilot-instructions.md`, `.github/agents/*.md`, and `.github/skills/**/SKILL.md`.

## 0) Default Rule

For anything beyond a single obvious action, **start with `gaia-workload-orchestrator`**.
It classifies the request into one of three tiers:
- **Rapid** — trivial/straightforward: skip ceremony, just execute.
- **Standard** — moderate: recall context, delegate to one agent, remember learnings.
- **Full** — complex/cross-cutting: full multi-agent orchestration with tasks, specs, and handoffs.

## 1) Spec-Driven Development

- `docs/` is the **single source of truth** for requirements, architecture, and use cases.
- **No drift**: spec ↔ code must stay in sync at all times.

## 2) Authority & Permissions

| Domain | Owner | Others |
|--------|-------|--------|
| `docs/`, architecture, tech stack | **gaia-architect** | Request changes via Architect |
| Code, tests, migrations, infra | **gaia-developer** | Propose guidance only |
| Debugging, root-cause, perf | **gaia-analyst** | Findings → Developer implements |
| Quality gates, validation | **gaia-tester** | Reports → Developer/Architect fix |

Default stack: `.github/skills/default-web-stack/SKILL.md`

## 3) Delegation Is Mandatory

**Never struggle alone. Delegate early.** If you spend >2 minutes outside your domain, hand off.

- Architect → specs, design, tech-stack, `docs/` changes
- Developer → code, tests, migrations, infra
- Analyst → ambiguous bugs, perf, deep investigation
- Tester → validation, regression, security review
- Orchestrator → multi-step or cross-agent coordination

## 4) Gaia MCP Tools Are Mandatory

All tools require **`projectName`** (derived from repo/workspace name). Use them aggressively.

| Tool | When |
|------|------|
| `gaia-recall` | **Always first** — before starting any work |
| `gaia-remember` | After decisions, patterns, workarounds, user preferences |
| `gaia-update_task` | Before/during/after multi-step work; cross-agent handoffs |
| `gaia-log_improvement` | **Immediately** on any friction — don't wait. Over-log > under-log |

Memory categories: `pattern`, `decision`, `workaround`, `context`, `lesson`.
Improvement types: `PainPoint`, `MissingCapability`, `WorkflowImprovement`, `KnowledgeGap`, `Enhancement`.

## 5) Common Agent Behaviors

Every agent **must**:
1. Pass `projectName` consistently to all Gaia MCP tool calls.
2. Call `gaia-recall` at task start; `gaia-remember` for significant learnings.
3. Log friction immediately via `gaia-log_improvement` with `projectName`.
4. Check `.github/skills/**/SKILL.md` for relevant skills before domain work.
5. Use the structured handoff format (§7) when delegating.

## 6) Skills Are Mandatory

Before domain work, check for relevant skills in `.github/skills/**/SKILL.md`.
If a recurring need lacks a skill, log an improvement request.

## 7) Handoff Format

When handing work to another agent, include:
- **Project name** (always)
- **Objective** (what success looks like)
- **Context** (what you learned; links/paths; constraints)
- **Inputs** (files touched, commands, expected output)
- **Risks / open questions**
- **Next actions** (1–3 bullets)

## 8) Folder-Specific Rules (Optional)

Subtrees may add a nested `AGENTS.md` for additional constraints. Nested rules must not contradict this root contract.
