# Gaia: Agent Workflow Contract (AGENTS.md)

This file defines **how Gaia runs agentic workflows** in this repository (Copilot orchestrator + Gaia agents).
It complements:
- `.github/copilot-instructions.md` (repo-wide Copilot context)
- `.github/agents/*.md` (individual agent personas)
- `.github/skills/**/SKILL.md` (tribal knowledge / best-practice playbooks)

## 0) Default Rule

For anything beyond a single obvious action, **start with `gaia-workload-orchestrator`**.
It decomposes the request, routes to the correct agent(s), and manages handoffs.

## 1) Spec-Driven Development

- `docs/` is the **single source of truth** for requirements, architecture, and use cases.
- **No drift**:
  - Spec says it exists → code must exist.
  - Code exists → spec must describe it.

## 2) Authority & Permissions (Hard Rules)

### Documentation (docs/)
- **Only `gaia-architect`** may create/modify/delete anything under `docs/`.
- Everyone else must request doc changes from the Architect (with concrete diffs/notes).

### Architecture / Tech Stack
- **Only `gaia-architect`** approves:
  - new dependencies / frameworks
  - architectural patterns
  - cross-cutting refactors
- Default stack reference:
  - `.github/skills/default-web-stack/SKILL.md`

### Code / Tests / Migrations / Infra
- **Only `gaia-developer`** may edit application code, tests, migrations, or infra configuration.
- Others may propose guidance or patches, but do not directly change code.

### Investigation / Analysis
- **`gaia-analyst`** handles debugging, root-cause analysis, performance investigations, and risk identification.
- Analyst provides findings + recommended approach; Developer implements.

### Validation / QA
- **`gaia-tester`** runs quality gates and validates behavior against specs/use cases (functional + visual where relevant).

## 3) Delegation Is Mandatory (Mesh, Not Silos)

**Never struggle alone. Delegate early.**

Delegation triggers:
- Architect → any spec/design/tech-stack decision; any `docs/` change.
- Developer → any code/test/migration/infra change.
- Analyst → ambiguous bugs, perf, deep investigation.
- Tester → validation, regression checks, security review, QA sign-off.
- Orchestrator → any multi-step or cross-agent workflow, or uncertainty about process.

Hard rule: **If you spend >2 minutes outside your domain, delegate.**

## 4) Gaia MCP Tools Are Mandatory

Gaia ships MCP servers in `.github/mcp-config.json` (e.g. `gaia`, `playwright`). Use them aggressively.

### Memory
- **Always call `gaia-recall` first** before starting work.
- **Always call `gaia-remember`** after:
  - decisions + rationale
  - reusable patterns
  - workarounds/edge cases
  - user preferences/context

Recommended categories: `pattern`, `decision`, `workaround`, `context`, `lesson`.

### Tasks
For any multi-step work:
- create tasks **before** starting (`gaia-update_task`)
- update progress **during**
- mark complete **after**
Also use tasks for cross-agent handoffs.

### Improvements
Log friction immediately with `gaia-log_improvement`:
- `PainPoint`, `MissingCapability`, `WorkflowImprovement`, `KnowledgeGap`

Minimum: for complex tasks, log at least one improvement **if any friction occurred**.

## 5) Skills Are Mandatory

Before domain work, check for relevant skills:
- `.github/skills/**/SKILL.md`

If a recurring need lacks a skill, log an improvement request.

## 6) Handoff Format

When handing work to another agent, include:

- **Objective** (what success looks like)
- **Context** (what you learned; links/paths; constraints)
- **Inputs** (files touched, commands, expected output)
- **Risks / open questions**
- **Next actions** (1–3 bullets)

## 7) Folder-Specific Rules (Optional)

If a subtree needs special rules, you may add a nested `AGENTS.md` inside that folder.
Nested rules may only add constraints/details for that area and must not contradict this root contract.
