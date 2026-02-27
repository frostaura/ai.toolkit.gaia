---
name: repository-audit
description: Survey repo reality (stack, docs/code/skills drift, CI/lint/tests/docker/Makefile) and produce a compact Repo Survey + suggested tasks for the orchestrator.
---

# Repository Audit (Repo Explorer)

## When to use

Run on **every request** before planning. Output is **chat-only** (no repo files). Keep it compact.

## Inputs

- Working tree contents (paths + key files)
- Recent changes (if available)
- CI status signals (workflows present; failures if visible)
- `/docs` contents and relevance to current code

## Output (exact format)

Return a **Repo Survey** with these sections (bullets, short lines):

1. **Stack & Tooling**
2. **Docs State**
3. **Drift Checks (Docs↔Code, Skills↔Reality)**
4. **Quality Infrastructure**
5. **Suggested Tasks (titles + gates)**

## Step 1 — Identify stack & tooling

Look for (examples, not exhaustive):

- Languages: `.csproj`, `package.json`, `pyproject.toml`, `requirements.txt`, `go.mod`, `pubspec.yaml`
- Build: `Makefile`, `docker-compose.yml`, `*.sln`, `gradle`, `mvnw`, `nx.json`, `turbo.json`
  Report:
- Primary stack(s)
- Build entrypoints (Make targets preferred)
- Package manager(s)

## Step 2 — Check docs presence & shape

Inspect `/docs`:

- Does it exist?
- Is there `/docs/use-cases/`?
- Are use cases “one per file” and do they have `UC-###`?
- Is there `/docs/architecture/`?
  Report:
- What’s present
- What’s missing
- Any obvious staleness/duplication

## Step 3 — Drift checks (blocking signals)

### Docs ↔ Code drift (blocking)

Look for mismatches such as:

- Documented endpoints that don’t exist
- Implemented endpoints/flows not documented
- Behavior described differently than code/tests imply
  Return: `drift_docs_code: none | suspected | confirmed` with 1–2 bullet reasons.

### Skills ↔ Reality drift (blocking)

Compare repo reality against skills assumptions:

- Lint tool differs from skill
- Test runner differs
- CI structure differs
- Docker/Make conventions differ
  Return: `drift_skills: none | suspected | confirmed` with 1–2 bullet reasons.

## Step 4 — Quality infrastructure inventory

### CI (blocking if missing/failing)

- Check `.github/workflows/`
- Identify primary workflow(s): lint/build/test
- If evidence of failing CI exists, report it
  Return:
- `ci: missing | present | failing`

### Lint/format

- Identify lint tools and where configured
  Return:
- `lint: missing | present` and the tool

### Tests

- Identify unit/integration/e2e presence
- Note Playwright presence for web
  Return:
- `tests: none | unit | unit+integration | unit+e2e | full` (best match)

### Dockerization (required for HTTP APIs)

- Check for `docker-compose.yml` and `.env.example`
  Return:
- `docker: missing | present` and whether it includes the API

### Makefile (preferred local UX)

- Check for `Makefile`
- List key targets if present: `up/down/test/lint/build`
  Return:
- `make: missing | present` and key targets

## Step 5 — Suggested Tasks (orchestrator will create real tasks)

Provide 5–12 suggested tasks max, each as:

- **Title**
- **Why** (one short line)
- **Suggested required_gates[]** (explicit list)

Rules:

- If `drift_docs_code != none`: include “Resolve docs↔code drift” as the top task (blocking).
- If `ci != present`: include “Add/Fix CI” as a top task (blocking).
- If HTTP API and `docker: missing`: include “Add docker-compose stack” (blocking for use-case work).
- If `drift_skills != none`: include “Update affected skills to match reality” (blocking).

## References (read/consult)

- `AGENTS.md` (non-negotiables)
- `.github/copilot-instructions.md` (repo instructions)
- `.github/skills/gaia-process/SKILL.md` (workflow contract)
- `.github/workflows/` (CI)
- `/docs/` (source of truth)
- `Makefile` (local commands)
