---
name: fa-foundation-context-authoring
description: Provides cascade-aware authoring for a scope's instruction file (CLAUDE.md or AGENTS.md) and its MEMORY.md topic store, keeping timeless rules strictly separate from dated state and holding every scope to its own delta over its parent. Use it by reading every context file in the scope end to end first, sorting each sentence into instruction or memory with the date/status/version test, then writing frontmattered memory/ topic files and generating MEMORY.md from them rather than hand-writing it. Use it when creating, rewriting or restructuring either file at a single scope whose surrounding cascade already exists, when a nested scope restates content it already inherits, when an instruction file has started asserting status, versions or progress, or when a memory store has no memory/ directory, carries malformed topic frontmatter, or has a hand-written index instead of one generated from its topic files.
license: MIT
---

# Context Authoring

## Scope and when to use

Use this skill when the *structure* of a scope's context pair is wrong or absent — you are creating a file, rewriting one, or moving content across the instruction/`MEMORY.md` line.

Use this skill when:

- creating or rewriting a scope's instruction file or `MEMORY.md`
- an instruction file has started asserting dates, statuses, versions or progress
- a nested scope restates rules it already inherits from a parent
- a memory store is a single prose file rather than an index over `memory/` topics, or the audit reports a stale index, malformed frontmatter or an invalid `type`

Do not use this skill when:

- there is no surrounding cascade yet — the whole tree is unbootstrapped, which is `fa-foundation-optimize-directory-tree`'s job, not a scope-by-scope authoring pass
- the structure is sound and only the *facts* are stale — that is a memory refresh
- the content is an ordered procedure with traps in it — that is a skill, per `fa-foundation-create-skill`
- the material is reference documentation belonging in the consuming repo's own `docs/` layer

## Required inputs

- every context file already present in the scope, read end to end
- the pair at every scope above this one, so the delta can be computed
- the shipped cascade standard and the audit's finding codes
- verified reality for anything you intend to record as state

## Owned outputs

- an instruction file that is timeless: mandate, architecture, conventions, commands, prohibitions
- `memory/` topic files with valid frontmatter, and a `MEMORY.md` generated from them
- content moved to the correct side of the split, with the wrong-side copy deleted
- a scope that states only its own delta and links upward for everything else

## Decision tree

- If a sentence carries a date, a status, a version, a "currently" or a "we decided", it is state — it goes in a `memory/` topic, never in the instruction file.
- If it would still be true after a year of work, it is instruction.
- If a parent already says it, delete it and link upward. **Write only the delta, never restate a parent** — duplication is exactly how a cascade drifts against itself, because the copies get edited on different days and nobody notices which one is lying.
- If it is an ordered procedure with traps, it is neither file — it is a skill.
- If you cannot verify a fact by inspection right now, do not record it at all.

## Core workflow

1. Read the whole scope before writing a line of it — both context files, every `memory/` topic, every local `SKILL.md`, plus the pair at each parent scope. Editing at a located insertion point is how a file ends up asserting one thing in its conventions section and the opposite three paragraphs down, and how a "new" rule gets added beside the weaker version of itself that was already there.
2. Sort every existing sentence into instruction or memory with the test above. Misfiled state is the commonest defect and the most damaging: **one status line rots the whole instruction file**, because a reader who catches it stale stops trusting the rules printed beside it.
3. Apply the refinement that settles the hard cases. A *structural inventory* is not state — "this scope owns these packages", "the API exposes these pillars", "the stack is .NET, EF Core and PostgreSQL" are facts about the shape of the thing and belong in the instruction file, because routing and navigation depend on them. What does not belong is *condition*: patch-level pins, progress, phase, dormancy, dates, counts that move on the next commit, credentials. The test is not "could this ever change" — everything can. It is "does an agent need this to find its way around, or to know how things are going right now?" **Navigation is instruction; condition is memory.**
4. Compute the delta against the parents and delete everything inherited.
5. Write the instruction file in this shape: what this is, in two or three sharp sentences · architecture and key concepts · conventions and non-obvious rules · how to build, test and run · where its docs live · what an agent must never do here.
6. Write the memory store to the topic-store shape below — one topic file per concern, then generate the index from them.
7. Run `${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py` and clear every mechanical finding before handing off.

## The topic-store shape

`MEMORY.md` is a **pure index, and it is derived — never authored.** Content lives in a `memory/` directory beside it, one topic file per concern, so a reader lists the index, skims frontmatter, and deep-reads only what the task needs. Every line of the index is computed from those topic files: the heading from the scope's path, each title from the topic's `# H1`, each hook from its `description:`, the order from its `type:`. You write topic files and then run the generator; you do not write `MEMORY.md`.

```bash
python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --fix-index [--scope <your directory>]
```

It refuses to regenerate a store whose topics have malformed frontmatter, and refuses when `memory/` holds no topic at all rather than truncating the index to a bare heading. Only `*.md` topics live in `memory/` — anything else there is `MEMORY-STORE-DEBRIS`.

```markdown
# MEMORY — packages/ingest

- [Red — there is no safe blanket push here](memory/alerts.md) — the description of alerts.md, verbatim
- [Current state](memory/state.md) — the description of state.md, verbatim
```

The heading is the scope's path relative to the repository root; the root's own index uses the repository directory's name. Order is by type — `alert`, `state`, `decision`, `gotcha`, `question`, `watch`, `kill-record`, `evidence`, `log`, `reference` — canonical file before its split siblings, then alphabetical. Any byte of difference between the file on disk and that derivation is `MEMORY-INDEX-STALE`, and the fix is always to regenerate, never to edit the index.

Every topic file opens with exactly this block — four keys, in this order, no extras, then a blank line and the heading — so `head -8` returns the complete relevance signal plus that heading:

```markdown
---
name: <scope slug>-<file stem>
description: "one line, at most 240 characters — the index hook"
type: state
last_verified: YYYY-MM-DD
---

# <Topic heading — becomes the index title>
```

`name` is `<scope slug>-<file stem>`: the scope's directory name kebab-cased, a grouping directory prefixed by its parent, the repository root `root` — so `packages/ingest/memory/gotchas.md` is `ingest-gotchas`. **`description` is the index hook, and it carries the signal, not a label** — "deployment notes" is a wasted line; "deploy decided, unpushed range still local, kill clock void once live" leaves a reader informed even if they stop at the index. It is capped at **240 characters**: past that it is narrating the body, or the topic is two topics. **The first body line is a `# Heading`**, because the index title comes from it, and an `alert`'s heading begins `Red — `. `type` is one of exactly ten values, **always singular** — the file may be `decisions.md`, the type is `decision`, and a plural silently defeats any tooling that filters on type: `state` · `decision` (recorded with its *why*, or the next agent reverses it on cost alone) · `gotcha` · `question` (naming its owner) · `watch` · `kill-record` · `alert` (read at every level passed through, and sorted to the top) · `log` (a pointer index, never a changelog) · `evidence` · `reference`. Keep a topic under **60 lines and 6,000 characters**; past either it is two topics. Restamp `last_verified` only for what you actually inspected, never for what you merely edited, and never date it in the future. Topic files carry no `## Upkeep` section. Relative links inside a topic resolve from `memory/`, one level below the scope — prefix `../`.

**Never describe a directory's file inventory from inside a file you are adding to that directory.** One pass wrote "this directory contains exactly one file" into four `MEMORY.md` files it was in the act of adding to those directories — false before the write completed. Describe dormancy and last real activity instead of file counts and mtimes, and warn explicitly when a context file's own mtime must not be read as scope activity.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| status found in an instruction file | move the sentence into a `memory/` topic, delete the original | author | re-read the whole file for siblings of it |
| scope restates a parent | delete the restatement and link upward | author | fix the parent instead, if the parent is what is wrong |
| `MEMORY.md` is prose, not an index | split it into `memory/` topics, put each hook in that topic's `description:`, then regenerate the index | author | audit sibling scopes for the same shape |
| invalid `type` or malformed frontmatter | rewrite the four-key block; singularize the type | author | re-run the context audit |
| a fact cannot be verified now | omit it, or record it as a `question` with an owner | author | escalate to the human owner |

## Anti-patterns

- do not edit at a located insertion point without reading the file to the end; a purely additive edit is the tell
- do not write "follow best practices"; write the pin and the reason it exists, and never write aspiration in the present tense — if a team, customer or deployment does not exist yet, say so
- do not record volatile integers such as ahead-counts or dirty-path counts; record the topology that stays true
- do not leave an instruction file without a `MEMORY.md` beside it — both files, or neither

## Handoff and downstream impact

- tell the next agent which claims were verified by inspection and which were carried over unverified
- tell skill maintainers when content you moved out was really a procedure and needs a skill
- tell the human owner about any contradiction between scopes you could not resolve locally, and leave the audit clean so the next session's findings are genuinely new

## Examples

- **Good fit:** a scope's instruction file lists pinned patch versions and a "currently blocked on" line; split it, moving condition into `memory/state.md` while the structural stack statement stays put.
- **Good fit:** a newly created nested scope whose file is 80% a copy of its parent's conventions; reduce it to its own delta.
- **Not a fit:** every fact in a well-structured memory store is six months old and needs re-verification — that is a memory refresh, not a restructure.

## Completion checklist

- no date, status, version, "currently" or "we decided" survives in the instruction file
- structural inventory stayed put; condition moved to memory
- nothing in this scope restates a parent
- `MEMORY.md` was generated rather than written, every `description:` carries signal rather than a label inside 240 characters, and every topic file has the four-key frontmatter, an `# H1`, a singular `type` and an honest `last_verified`
- the context audit runs clean, or every remaining finding is a deliberate judgement call

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit findings](../../references/context-audit-findings.md)
- [Ownership and conventions](../../references/ownership-and-conventions.md)
