# Gaia Skills Index

Skills are short playbooks (≤200 lines) that agents follow.

- **gaia-process** — end-to-end SDLC flow (repo survey → task graph → gates → delivery)
- **repository-audit** — determine repo state, detect drift, inventory CI/lint/tests/docker
- **doc-derivation** — derive comprehensive `/docs` from existing code (drift repair)
- **spec-consistency** — prevent doc/code drift across API/data/UI/tests
- **tasking-and-proof** — MCP task graph rules, required_gates, blockers, proof args
- **ci-baseline** — add/repair GitHub Actions CI (lint/build/tests)
- **linting** — add/extend lint+format per stack + CI integration
- **dockerize-http-api** — docker-compose + .env.example + Makefile targets
- **integration-testing-http** — curl-level checks against docker stack
- **playwright-e2e** — add/extend Playwright and structure specs (UC ID naming)
- **manual-regression-api** — manual curl regression protocol (labels only)
- **manual-regression-web** — Playwright MCP manual regression protocol (labels only)
- **stack-defaults/** — first-class baselines for Web TS, .NET API, Python, Flutter, .NET MAUI
