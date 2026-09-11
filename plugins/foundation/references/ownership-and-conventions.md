# Foundation Ownership and Conventions

Use this reference when deciding which foundation skill or agent owns a change, which context-layer artifact should be updated, or which naming convention to apply.

**The layer boundary:** the foundation plugin owns the **context layer** — the instruction files, the memory topic store, the skill and agent definitions, the registries, and the durability of the repository that holds them. The engineering plugin owns the **delivery layer** — architecture, plans, code, UI, tests, releases. When a change touches both, the context edit is foundation's and the code edit is engineering's; neither plugin carries a copy of the other's procedure.

## Skill ownership (foundation plugin)

| Skill | Primary ownership |
|---|---|
| `fa-foundation-memory-maintenance` | The memory store — `memory/` topic files, their frontmatter shape and `last_verified` stamps, pruning and promotion, and regenerating the derived `MEMORY.md` index from them |
| `fa-foundation-context-authoring` | Instruction files (`CLAUDE.md` / `AGENTS.md`) at one scope: the cascade delta, the state-vs-instruction split, upkeep sections |
| `fa-foundation-create-skill` | `SKILL.md` definitions — naming, description quality, scope sections, references, reuse-versus-new decisions |
| `fa-foundation-create-agent` | Agent definition files — role contracts, tool scopes, delegation rules, overlap control |
| `fa-foundation-create-plan` | Multi-step plan research and synthesis, and the on-disk plan file it hands off |
| `fa-foundation-session-close` | The close-out a session owes once it changed reality — the memory-store edits, the emptied scratch, and the local commit that makes them durable |
| `fa-foundation-registry-audit` | Registries that name scopes or projects, reconciled against what is on disk in both directions |
| `fa-foundation-repo-durability` | Repository git state — what is committed, pushed, diverged or unrecoverable, established by inspection rather than by claim |
| `fa-foundation-context-audit` | The whole-repository sweep: running the audit script, fanning out the auditors, and reconciling the root scope last |
| `fa-foundation-optimize-directory-tree` | A whole tree's context layer as one artifact — the partition it is written from, the pair at every scope that earns one, the root registry, and the project analysis its design documentation is derived from |

## Agent roster (foundation plugin)

| Agent | Role |
|---|---|
| `fa-foundation-context-auditor` | One scope's context layer — pair, memory topics, local skills — reconciled against inspected reality. **One agent per scope**; two agents in the same subtree overwrite each other |
| `fa-foundation-repo-durability-auditor` | "Is this repository safe, and is it safe to push?" across one or many repositories. Read-only on git |
| `fa-foundation-skills-auditor` | One definition directory — its skills, agents and scripts — audited for staleness, sprawl, dead pointers and descriptions that never fire |

## Context-layer artifacts

These paths refer to the **consuming repository** — the project Gaia is operating on, not this plugin.

- Instruction files (`CLAUDE.md`, `AGENTS.md`) at one scope inside an existing cascade -> `fa-foundation-context-authoring`
- `MEMORY.md` and its `memory/` topic files -> `fa-foundation-memory-maintenance`
- `SKILL.md` files, and the skills `README.md` index -> `fa-foundation-create-skill`
- Agent definition files -> `fa-foundation-create-agent`
- Scope and project registries -> `fa-foundation-registry-audit`
- Git state claims of any kind -> `fa-foundation-repo-durability`
- A session that shipped, decided, discovered, abandoned or unblocked something -> `fa-foundation-session-close`
- A drift sweep spanning more than one scope -> `fa-foundation-context-audit`
- An entire tree with no context layer, and the tree-wide project analysis its design docs were never written from -> `fa-foundation-optimize-directory-tree`
- Architecture docs, code, UI, tests, release gates -> the **engineering plugin**

## Naming conventions

- Every skill and agent is `fa-<plugin>-<name>`, where `<plugin>` is `foundation`, `engineering` or `product`: `fa-foundation-memory-maintenance`, `fa-foundation-context-auditor`.
- A skill's directory name must equal its frontmatter `name`; an agent's filename minus `.md` must equal its frontmatter `name`. A mismatch is how a definition silently stops resolving.
- Prefer role or domain names over implementation details; the name should survive a refactor of how the job is done.
- Refer to another skill or agent by bare backticked name, never by path — paths break the moment a plugin tree is restructured.

These are Gaia conventions layered on top of any lower-level format constraints required by the underlying skill or agent specification. The doctrine they enforce is in [`context-cascade.md`](context-cascade.md).
