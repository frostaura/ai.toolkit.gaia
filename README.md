<p align="center">
  <img src="https://github.com/frostaura/fa.templates.vibe-coding/blob/main/README.icon.png?raw=true" alt="Gaia" width="300" />
</p>

<h1 align="center"><b>Gaia</b></h1>
<p align="center"><i>research scientist edition</i></p>
<h3 align="center">full-stack apps. enterprise-grade. maintainable. customizable.</h3>

---

[![Version 7](https://img.shields.io/badge/Version-7-purple.svg)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub Copilot](https://img.shields.io/badge/GitHub-Copilot-blue.svg)]()
[![GitHub Copilot CLI](https://img.shields.io/badge/GitHub-Copilot%20CLI-blue.svg)]()

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

### Advanced Mode
The below may be used to simulate a workflow execution bu running copilot cli in headless mode.

*Basic chaining for workflows*
```bash
copilot -p "prompt 1" --yolo && copilot -p "prompt 2" --yolo && copilot -p "prompt 3" --yolo
```

*Recursive chaining for continuous workflows*
```bash
while true; do copilot -p "prompt 1" --yolo && copilot -p "prompt 2" --yolo && copilot -p "prompt 3" --yolo; done
```

The above commands are exceptional in getting Gaia to recursively reflect on the code state vs the system spec, and iterate until your desired completion rate before starting the human polishing phase.

*Simple recursive process for ensuring feature parity with the system spec and comprehensively test the system using the test spec / plan.*
*Recursive chaining for continuous workflows. In the below example, we use multiple models. Claude Opus to do the assessments and Codex 5.3 to implement the deltas, recursively*
```bash
while true; do copilot --yolo --model "claude-opus-4.6" -p "Assess the docs/specs/system_spec.md and the state of the code repository. Assess the docs/specs/test_spec.md for a comprehensive test plan for the system. Determine the the completion rate for all features, items, outstanding todos, ourstanding static, sample or mock data, that data and config is well-abstracted and live in the config or appsettings files and not statically coded anywhere. Integration tests implementation completion rates, based on the test spec and the state of the repo. This should be a dispassionate report. Dont take the role of a developer or tester. Instead you are a dispassionate auditor. A feature parity, quality and visual excellence police. Detective Gaia, if you will. create a detailed report for the overall system completion and save it to COMPLETION_REPORT.md (overwrite)" && copilot --yolo --model "gpt-5.3-codex" -p "[COMPLETION_REPORT.md] - Now implement all the non 100% completed or perfect items systematically. Always leave any code you encounter in a better state where appropriate. Never leave things like pre-existing gaps, errors, todos and so on, undone. You are a responsible software architect and elite software engineer. Always implement 100% of all missing things. No less is exceptable. And only after the 100% completion mark, continue to the tests. As tests are written, ensure the code that it tests is bug-free. After all thats the point of tests. Don't just blindly taylor tests to the state of the code. The code should be taylored to delivering on features which the tests should objectively assert. Remove the report after."; done
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
