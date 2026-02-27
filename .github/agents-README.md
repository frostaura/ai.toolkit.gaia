# Gaia Agents Index

This folder defines Gaia’s **custom agents (subagents)** used for spec-driven delivery.

- Keep each agent doc ≤150 lines.
- The **Workload Orchestrator** is the supreme planner and owns the MCP task graph.
- The **Quality Gatekeeper** has veto power.
- The **Repo Explorer** runs first on every request.

## Agents

- **gaia-workload-orchestrator** — Supreme planner/controller: runs Repo Explorer first, fixes blockers first, creates MCP tasks + gates, delegates, enforces QA veto, records MCP proof.
- **gaia-repo-explorer** — First step always: surveys repo reality (stack/docs drift/CI/lint/tests/docker/Makefile) and suggests tasks (chat-only).
- **gaia-quality-gatekeeper** — Independent QA authority with veto: verifies gates, proof, CI green, docker-first, and drift resolution.
- **gaia-architect** — Architecture + docs: designs structure/contracts and updates `/docs/architecture/`.
- **gaia-developer** — Implementation: builds features per spec, preserves conventions, keeps lint/build/tests passing, updates skills/docs when conventions change.
- **gaia-tester** — Tests: authors unit/integration/e2e; for web use-case changes adds Playwright specs with UC ID in filename.
- **gaia-analyst** — Clarity: acceptance criteria, edge cases, risks; proposes MCQ/yes-no questions for “needs input”.

## How they work together

1. Orchestrator delegates to Repo Explorer (always).
2. Orchestrator creates the task graph (MCP) and sets `required_gates[]`.
3. Architect/Developer/Tester execute tasks using Skills.
4. Quality Gatekeeper reviews every task and can veto.
5. Orchestrator marks completion via MCP with link-only proof args.
