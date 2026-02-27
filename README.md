<p align="center">
  <img src="https://github.com/frostaura/fa.templates.vibe-coding/blob/main/README.icon.png?raw=true" alt="Gaia" width="300" />
</p>

<h1 align="center"><b>Gaia</b></h1>
<h3 align="center">full-stack apps. enterprise-grade. maintainable. customizable.</h3>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub Copilot](https://img.shields.io/badge/GitHub-Copilot-blue.svg)]()
[![AI Agents](https://img.shields.io/badge/AI-5%20Agents-purple.svg)]()

---

## What is Gaia?

Gaia is a **team of AI agents** designed to build and evolve software using **spec-driven development**.
You describe your goal; Gaia coordinates architecture, implementation, testing, and documentation.

---

## How Gaia Works (Spec-Driven)

- `docs/` is the **source of truth** for requirements and architecture.
- **No drift**:
  - If a spec describes a feature, it must exist in code.
  - If code changes behavior, the spec must be updated.

**Ownership:**

- Architect owns `docs/` and architecture decisions.
- Developer owns code/tests/migrations/infra.
- Tester validates behavior and quality gates.
- Analyst investigates bugs/perf and provides recommendations.
- Orchestrator coordinates everything.

The workflow contract lives in **`AGENTS.md`**.

---

## Using Gaia

### In VS Code (recommended)

1. Open your project folder in VS Code
2. Enable GitHub Copilot
3. Start a chat and describe what you want

Example:

```
Build me a task management app with user login
```

### In the Terminal (Copilot CLI)

```bash
npm i -g @github/copilot && copilot -p "<your project request>" --yolo
```

Example:

```bash
copilot -p "Create a REST API for a blog with posts and comments"  --yolo
```

---

## Repository Layout

Gaia’s configuration lives in `.github/`:

- `.github/copilot-instructions.md` — repo-wide Copilot context
- `AGENTS.md` — agent workflow contract (permissions, delegation, tools)
- `.github/agents/` — individual agent personas
- `.github/skills/` — playbooks and best practices
- `.github/mcp-config.json` — MCP servers (e.g. `gaia`, `playwright`)

Application projects typically include:

```
docs/           ← Specifications and design docs (Architect-owned)
src/            ← Application code (Developer-owned)
tests/          ← Automated tests (Developer-owned)
```

---

## Notes for Gaia Maintainers

When scaffolding a new project, the **Architect** should replace this README with a project-specific README describing the actual application being built.

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life."</i>
</p>
