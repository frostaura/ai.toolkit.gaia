---
name: ai-toolkit-gaia-decisions-distribution
description: "Public distribution is Labs' one standing stealth exception; plugin refs float on ref:main not tags, so a breaking change lands on users at push time; versions move in lockstep; the README is deliberately not a catalog."
type: decision
last_verified: 2026-09-11
---

# Live decisions — distribution, versioning and the catalog

- **Public distribution is the one standing stealth exception in Labs.** Do not relitigate.
  It is also the division's only public repository, which raises the blast radius of
  everything shipped from here — a skill edit is a production change.
- **Plugin sources are pinned to `ref: main`, not to a tag** — a conscious choice whose
  consequence is that installs float with `main`, so any breaking change to a skill or agent
  contract reaches users the moment it is pushed. The `plugin.json` `version` field is what
  actually triggers a floating install to re-fetch, which is why bumps matter. Whether to
  move to tags is open — [`questions.md`](questions.md).
- **Plugin versions move in lockstep across all three plugins**, and `metadata.version` in
  the two marketplace manifests does **not** move for a plugin change — it tracks the
  catalog's shape, not the plugins'. Major when a contract breaks (tools removed, skill
  descriptions changed, the agent-schema rewrite); minor when purely additive. A bump touches
  twelve sites — six `plugin.json` files and the plugin row in each manifest — plus the README
  badge, and nothing in CI checks that they agree ([`gotchas.md`](gotchas.md)).
- **`"dependencies": ["foundation"]` stays in the two `plugin.json` copies and is
  deliberately not mirrored into the marketplace manifests.** The manifests are a discovery
  catalog, not a resolver; adding a field neither ecosystem resolves creates a fourth place
  to disagree. The README states the consequence instead: install `foundation` first, because
  nothing installs it for you.
- **The README is not a catalog, and will not become one again** (2026-08-21). Every README
  defect that release fixed — a badge stuck at `Version-11`, a skill table listing renamed
  and missing skills, a mis-framed `dependencies` note — arrived by the same mechanism:
  hand-maintained duplication with nothing checking agreement. Skills are listed only in
  `docs/catalog.md`, which dates itself and points at `/plugin` as the live roster; the
  README says *why* in one line so the next contributor does not helpfully re-add the table.
  A highlights list was rejected for the same reason — it rots just as fast and looks current
  while doing it.
- **`fa-foundation-optimize-directory-tree` ships with no agent definition of its own.**
  Founder rationale, verbatim: *"i dont want to commit to a process then llms get more clever
  and the dated process just ties them down."* The method lives in an editable pasted brief
  (`references/analysis-brief.md`) rather than a named role roster. Accepted cost: a brief can
  silently fail to arrive. Compensating controls: the version marker, the mandatory
  `files written` block, the positive write allow-list, and the explicit prohibition on
  dispatching `fa-foundation-context-auditor` in its place.
- **`context-audit.py` is genericized on purpose, in five places that were each a
  transcription of one organization's directory shape** rather than a general rule:
  `projects/` hardcoded as the only grouping directory (now `--group-dir`); `VENDORED_DIRS`
  skipped even under `--all`; `check_registry` building `on_disk` only from sub-scopes, so a
  **flat** tree — the modal shape for a stranger installing this plugin — fired a false
  `REGISTRY-PHANTOM` per project; registry name patterns that accepted only lowercase-dotted
  names, so `MyService.Api` could never round-trip; and `instruction_file()` preferring
  `CLAUDE.md`, so an `AGENTS.md`-convention tree was audited on its interop pointer forever
  (now `--instruction-file`). Each fix looks like polish and is the difference between the
  self-healing claim holding and not.
- **The marketplace owner byline is "FrostAura Technologies"** in both manifests. Byline
  only — directory placement remains open ([`questions.md`](questions.md)).
