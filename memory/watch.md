---
name: ai-toolkit-gaia-watch
description: "No FrostAura plugin is installed on the founder's own Mac, so 'live to every installed user' currently describes nobody here; the repo's own audit script is blind to the plugin skills it ships; the playwright MCP is unpinned."
type: watch
last_verified: 2026-09-11
---

# Watch list

**Dogfooding — the gap that matters most**

- **Nothing FrostAura publishes is installed on the founder's Mac**, so "live to every installed
  user" describes nobody there; the evidence and its tree-wide consequence are root's
  ([`../../../../memory/plugin-roster-drift.md`](../../../../memory/plugin-roster-drift.md)).
  **Dogfooding is a stated kill criterion for this program** (`../CLAUDE.md`) — which is why the
  product carries the consequence rather than only the machine that fails to consume it.
- **The repository and the running service may disagree and nothing here can tell you.**
  v13 removed the `tasks_*` / `memory_*` / `evolve_*` tools; whether the hosted image was ever
  rebuilt and redeployed is unverifiable from the repo — [`questions.md`](questions.md) §1. If
  it was not, the live service still advertises tools no shipped plugin calls. Practical risk
  is low; the credibility cost of a service contradicting its own public repo is not.

**Shipped defects in the product itself**

- **`context-audit.py` cannot validate a single plugin skill this repo ships.**
  `check_skill_indexes` only enters a `skills/` directory whose parent is `.claude` or
  `.github`, and plugin skills live at `plugins/<name>/skills/` — so the
  frontmatter↔directory-name check, the one invariant whose failure silently produces an
  uninvokable skill, never runs on any of them. The substitute is the by-hand loop in
  `../docs/development.md`; run it on every skill change.
- **`--group-dir` still defaults to `projects`.** A caller who does not pass
  `--group-dir packages` on a workspace monorepo gets CLEAN over directories the script never
  entered. Silent pass by default is the worst failure mode an audit tool has.
- **`fa-foundation-create-agent` and `fa-foundation-create-skill` still route global policy
  into `AGENTS.md`** — both tell the author to put global workflow policy there rather than in
  the instruction file, against the interop-pointer doctrine this repo applies to its own
  `AGENTS.md`. Shipped to users, so it teaches the wrong thing.
- **`npx @playwright/mcp@latest` is unpinned** in the engineering plugin's `plugin.json` (both
  copies) — a live supply-chain and reproducibility exposure on every invocation.
- **The Claude Desktop / claude.ai / Cowork install path in the README is documented by link,
  not by click-path**, and has never been confirmed in the live product.

**Repository hygiene**

- **v13 deliberately deleted 15 tracked files** — the 14 tasks/memory/evolve sources, models,
  stores, validator and schema docs, plus this store's own resolved `do-not-push.md` alert.
  Those deletions are in origin's history, so a range comparison against an older ref shows
  them. **Expected and enumerated; never turn it into a restore.** The number is fixed by that
  commit and does not move.
- **This repo is public and its context layer is not written as if it were.** `CLAUDE.md`
  names the owner; the decision topics name the parent organization's internal structure and a
  killed internal project. The committed layers are already pushed, so this is now a
  going-forward choice: genericize deliberately, or accept it deliberately.
- **A 2 MB `README.icon.png` sits committed at repo root**, referenced by absolute GitHub raw
  URL anyway. History is permanent; removing it now only stops it growing.
