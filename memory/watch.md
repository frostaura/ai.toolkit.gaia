---
name: ai-toolkit-gaia-watch
description: "Dogfooded on the Claude desktop bundle at 13.0.0 and nowhere in the CLI; an unenumerated fourteenth version site leaves the MCP server advertising 13.0.0; the shipped audit script is blind to the plugin skills this repo ships."
type: watch
last_verified: 2026-09-16
---

# Watch list

**Dogfooding — two channels, and only one of them consumes the product**

- **The product *is* dogfooded, on the Claude desktop surface, at `13.0.0`** (verified
  2026-09-16). Desktop sessions materialise `foundation`, `engineering` and `product` per
  session from a bundle reading `13.0.0`, author FrostAura Technologies, with the full
  10 · 16 · 14 skill rosters and the three `foundation` auditor agents — exactly what
  `origin/main` serves; this tree's own 2026-09-16 upkeep ran out of it. The **Claude Code
  CLI** store holds no `frostaura` marketplace and no Gaia plugin, so on that channel every
  `fa-*` name resolves to nothing. Tree-wide consequence, founder's options:
  [`../../../../memory/plugin-roster-drift.md`](../../../../memory/plugin-roster-drift.md).
  **The stated kill criterion is not tripping** (`../CLAUDE.md`), and "a skill edit ships to
  every installed user" has a referent at last — one live install, at `13.0.0`, which a v14
  push upgrades across a breaking boundary in one step.
- **Repo and running service may disagree and nothing here can tell you** — v13 removed the
  `tasks_*` / `memory_*` / `evolve_*` tools and no redeploy is verifiable from here
  ([`questions.md`](questions.md) §1). Low practical risk; the credibility cost of a service
  contradicting its own public repo is not.

**Shipped defects in the product itself**

- **There is a fourteenth version site, and it is the one that is wrong.**
  `src/Gaia.Mcp.Server/Program.cs` sets the `Implementation.Version` the server advertises on
  every MCP `initialize`, and it still reads `13.0.0` — last moved by the v13.0.0 release
  commit, then left exactly where it was across the whole v14 line. Neither [`state.md`](state.md)'s release
  loop nor `../docs/development.md`'s table names it. Two cheap exits, neither taken: fold it
  into the release loop, or decide the server versions independently, which `../CLAUDE.md`
  already holds as separate concerns. Until then, a reader checking "the version sites agree"
  reads the surface as consistent while the service reports a major version behind.
- **The shipped `context-audit.py` passes silently over what it cannot reach.**
  `check_skill_indexes` enters a `skills/` directory only when its parent is `.claude` or
  `.github`, and this repo's own plugin skills live at `plugins/<name>/skills/` — so the
  frontmatter↔directory-name check, the one invariant whose failure silently produces an
  uninvokable skill, never runs on any skill the product ships. `--group-dir` likewise still
  defaults to `projects`, so a monorepo caller who does not pass `--group-dir packages` gets
  CLEAN over directories the script never entered. Substitute for the first: the by-hand loop
  in `../docs/development.md`, on every skill change.
- **`fa-foundation-create-agent` and `fa-foundation-create-skill` still route global policy
  into `AGENTS.md`** — against the interop-pointer doctrine this repo applies to its own
  `AGENTS.md`. Shipped to users, so it teaches the wrong thing.
- **`npx @playwright/mcp@latest` is unpinned** in the engineering plugin's `plugin.json` (both
  copies) — and the bundled `13.0.0` install carries the same unpinned argument, so the
  supply-chain and reproducibility exposure is live on a real install, not only in source.
- **The README's marketplace-add path for Claude Desktop / claude.ai / Cowork has never been
  confirmed end to end.** Documented by repository URL plus a link to Anthropic's docs rather
  than a click-path, deliberately. The plugins demonstrably *load and run* on that surface, but
  from a session-local bundle — so the install mechanism is unconfirmed, not the surface.

**Repository hygiene**

- **v13 deliberately deleted 15 tracked files** — tasks/memory/evolve sources, stores,
  validator and schema docs, plus this store's resolved `do-not-push.md` alert. They sit in
  origin's history, so a range comparison against an older ref shows them. **Expected and
  enumerated; never turn it into a restore.**
- **This repo is public and its context layer is not written as if it were** — `CLAUDE.md`
  names the owner, the decision topics name the parent's internal structure and a killed
  internal project, all already pushed. Genericize deliberately, or accept it deliberately.
- **A 2 MB `README.icon.png` sits committed at repo root**, referenced by absolute GitHub raw
  URL anyway. History is permanent; removing it now only stops it growing.
