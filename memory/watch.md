---
name: ai-toolkit-gaia-watch
description: "Dogfooded on the Claude desktop bundle at 13.0.0 while origin now serves 14.3.1; an unenumerated fourteenth version site leaves the MCP server advertising 13.0.0; the shipped audit script is blind to the plugin skills this repo ships."
type: watch
last_verified: 2026-09-16
---

# Watch list

**Dogfooding — two channels, and only one of them consumes the product**

- **Dogfooded on the Claude desktop surface, at `13.0.0` — now a major version behind what is
  published.** Desktop sessions materialise `foundation`, `engineering` and `product` per session
  from a bundle reading `13.0.0`, author FrostAura Technologies, with the full 10 · 16 · 14 skill
  rosters and the three `foundation` auditor agents (observed 2026-09-16, **before** that evening's
  push); this tree's 2026-09-16 upkeep ran out of it, and **the stated kill criterion is not
  tripping** (`../CLAUDE.md`). The **Claude Code CLI** store holds no `frostaura` marketplace, so
  on that channel every `fa-*` name resolves to nothing —
  [`../../../../memory/plugin-roster-drift.md`](../../../../memory/plugin-roster-drift.md).
  **The live item is the new gap:** `origin/main` now serves `14.3.1` ([`state.md`](state.md)) and
  sources float on `ref: main`, so the next re-fetch carries this install across v14's breaking
  boundary in one step — and the first repository it would apply the derived-index rules to is this
  tree. **Whether it has already re-fetched was not re-checked after the push.**
- **Repo and running service may disagree and nothing here can tell you** — no redeploy is
  verifiable from here ([`questions.md`](questions.md) §1), and the v14 push built an image without
  deploying one. The credibility cost of a service contradicting its own public repo is the risk.

**Shipped defects in the product itself** — all of these are now in the *released* v14, not
merely in a local tree.

- **There is a fourteenth version site, it is the one that is wrong, and it is now published
  wrong.** `src/Gaia.Mcp.Server/Program.cs` sets the `Implementation.Version` the server advertises
  on every MCP `initialize`; it still reads `13.0.0`, last moved by the v13.0.0 release commit and
  left untouched across the whole v14 line that shipped on 2026-09-16. Neither
  [`state.md`](state.md)'s release loop nor `../docs/development.md`'s table names it. Two cheap
  exits, neither taken: fold it into the release loop, or decide the server versions independently,
  which `../CLAUDE.md` already holds as separate concerns.
- **The shipped `context-audit.py` passes silently over what it cannot reach.**
  `check_skill_indexes` enters a `skills/` directory only when its parent is `.claude` or
  `.github`, and this repo's own plugin skills live at `plugins/<name>/skills/` — so the
  frontmatter↔directory-name check, whose failure silently produces an uninvokable skill, never
  runs on any skill the product ships. `--group-dir` likewise defaults to `projects`, so a monorepo
  caller gets CLEAN over directories the script never entered. Substitute: the by-hand loop in
  `../docs/development.md`, on every skill change.
- **`fa-foundation-create-agent` and `fa-foundation-create-skill` still route global policy into
  `AGENTS.md`**, against the interop-pointer doctrine this repo applies to its own. It teaches
  users the wrong thing.
- **`npx @playwright/mcp@latest` is unpinned** in the engineering plugin's `plugin.json` (both
  copies), and the bundled `13.0.0` install carries the same argument, so the supply-chain exposure
  is live on a real install. It went out unchanged in the v14 push.
- **The README's marketplace-add path for Claude Desktop / claude.ai / Cowork has never been
  confirmed end to end.** Deliberately documented by repository URL plus a link to Anthropic's
  docs rather than a click-path. The plugins demonstrably *load and run* on that surface, but from
  a session-local bundle — the install mechanism is unconfirmed, not the surface.

**Repository hygiene**

- **v13 deliberately deleted 15 tracked files** — tasks/memory/evolve sources, stores,
  validator and schema docs, plus this store's resolved `do-not-push.md` alert. They sit in
  origin's history, so a range comparison against an older ref shows them. **Expected and
  enumerated; never turn it into a restore.**

- **This repo is public and its context layer is not written as if it were** — `CLAUDE.md` names
  the owner, the decision topics name the parent's internal structure and a killed internal
  project, all pushed. Genericize deliberately, or accept it deliberately.
- **A 2 MB `README.icon.png` sits committed at repo root**, referenced by absolute GitHub raw URL
  anyway. History is permanent; removing it now only stops it growing.
