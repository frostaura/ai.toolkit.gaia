---
description: Run Gaia's Repo Explorer to produce a compact Repo Survey and seed the initial task graph.
---

You are running Gaia's Repo Explorer kickoff (per AGENTS.md §3).

Produce a compact "Repo Survey" in your reply that covers:
- Stack(s) detected, build system, package manager, runtime.
- Whether the user explicitly requested a stack that overrides Gaia's defaults.
- Whether the repo overrides Gaia's default stack baseline.
- `/docs` presence + freshness + gaps; docs ↔ code alignment.
- CI presence and status (exists? green?).
- Lint/format tooling presence and usage.
- Test setup (unit/integration/e2e).
- Dockerization status (especially for HTTP APIs).
- Conventions (folder layout, naming, scripts/Makefile).

Then:
1. Call `mcp__gaia-remote__memory_recall` to load any persisted facts.
2. Call `mcp__gaia-remote__evolve_list` to load lessons.
3. Suggest the first 3–5 tasks the orchestrator should create — but do not
   create them; the orchestrator owns task creation.

Stop after the survey + suggestions. The orchestrator decides ownership.
