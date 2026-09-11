# The Context Cascade — instruction file + `MEMORY.md`

Every directory in a consuming repository that carries meaning carries **two** context files: an **instruction file** (`CLAUDE.md` for Claude Code, `AGENTS.md` for tools that look for that name) and a `MEMORY.md`. A **scope** is any directory that carries this pair. Both files cascade downward: the repository root applies everywhere, a scope narrows it, a scope nested inside that one narrows it further. An agent entering at any depth reads its own pair plus every pair above it.

## The split

| | instruction file | `MEMORY.md` |
| --- | --- | --- |
| **Answers** | What is this, and how do I work here? | What is actually true here right now? |
| **Nature** | Normative — rules, mandate, conventions, standards | Observational — state, dates, decisions, gotchas |
| **Changes** | Rarely. A change is a policy change. | Often. Every meaningful session should touch it. |
| **Tense** | Timeless present ("projects use lowercase names") | Dated past/present ("as of 2026-07-19, dormant since 2026-05-02") |
| **If wrong** | The rule was wrong | The world moved |

The failure this split exists to prevent: instructions and reality decay at completely different rates, and mixing them into one file means **the whole file rots at the speed of the fastest-moving line**. Keeping state out of the instruction file is what lets the instruction file stay stable and trustworthy.

**Rule of thumb:** if a sentence contains a date, a status, a version, a "currently", or a "we decided" — it belongs in `MEMORY.md`. If it would still be true after a year of work, it belongs in the instruction file.

**The one refinement worth stating precisely.** A *structural inventory* is not state. "This scope owns these projects", "the API exposes these pillars", "the stack is .NET + EF Core + PostgreSQL" are facts about the shape of the thing, and they belong in the instruction file because routing and navigation depend on them. What does not belong is anything about *condition*: patch-level version pins, progress, phase, dormancy, dates, counts that move on the next commit, or credentials. The test is not "could this ever change" — everything can. It is "does an agent need this to find its way around, or to know how things are going right now?" **Navigation is instruction; condition is memory.**

## The third artifact — skills

A scope's `skills/` directory answers a third question: **how is a recurring job done here?** It cascades exactly as the pair does — root skills fire for any session inside the repository, a nested scope's own skills fire only when a session is rooted at or inside that directory.

The division of labour is clean once stated. A *fact* is `MEMORY.md`. A *rule* is the instruction file. A *procedure with an order and traps in it* is a skill. "The tuned strategy returns 18.1%/yr" is memory. "Strategies live in `user_data/strategies/`" is instruction. "Run the campaign loop, and without `--analyze-per-epoch` the optimizer silently fake-optimizes" is a skill.

Two rules keep the layer healthy. **Never mirror a root skill downward** — one root skill referenced everywhere, not one copy per scope drifting apart. **Never park a scope-specific procedure at root**, where it taxes every session in the repository forever.

**A stale skill is worse than a missing one.** It fires with authority and misleads. Delete without ceremony.

## Agents — the workers, not a fourth context artifact

Agent definitions are pre-written subagent roles. They are not a fourth cascading artifact — they carry no rules of their own that the instruction file does not already carry. They exist so that the *method* for a recurring fan-out lives in one editable place instead of being re-typed from memory into every brief, which is how method drifts. When how a sweep is run changes, **edit the agent definition**; a brief is not a place to keep policy.

**A session report is not a context artifact either, and must never become a fourth one.** There are three, and a report is none of them: nothing cascades it and the audit does not check it, so a finding parked in one is invisible to every mechanism this document describes. **Promote the findings into the memory store of the scope they concern, then delete the report** — in that order. A subagent's report is returned to its caller and never written into the tree; the pass's working files live in a `.tmp/` at that scope and go with it.

## Where the pair is required

A tree carrying none of this yet acquires the cascade in one pass rather than a file at a time: `fa-foundation-optimize-directory-tree` partitions the tree, writes the pair at every scope that earns one, and reconciles the root last.

- **The repository root** — required.
- **Every scope passed to the audit as a `--scope` argument** — required.
- **Every directory that groups scopes** (a `projects/` or `packages/` directory) — required. The instruction file holds the schema each child follows; `MEMORY.md` holds the live roster with per-child status.
- **Every scope inside it** — required.

**A directory with an instruction file and no `MEMORY.md` is a defect, not a shortcut.** It states the rules and leaves reality unstated, so the next agent infers status instead of reading it — precisely the failure the split exists to prevent. **Both files or neither.**

- **Reference (`docs/`) directories** — instruction file only, and only where the reference layer itself needs rules. Doctrine, not state.
- **`skills/` directories** — a `README.md` index, wherever one exists at any scope.
- **Deeper than a scope root** — at your discretion. Add a pair to a subdirectory only when it has its own working rules an agent would otherwise get wrong (a Python sidecar with a purity contract, a research folder with a one-shot holdout). Do not add them mechanically; an unnecessary context file is noise that costs every future agent tokens.

## Memory layout — the topic store

`MEMORY.md` is a **pure index**; the content lives in a `memory/` directory beside it, one topic file per concern. This mirrors how agent memory systems themselves work: list the index, skim each topic's frontmatter, deep-read only what the task needs.

**The index is derived, never authored.** Every line of `MEMORY.md` is computed from the topic files: the heading from the scope's path, each entry's title from the topic's `# H1`, its hook from the topic's `description:`, and the order from the topic's `type:`. Nothing in the index is written by hand, so a hook cannot disagree with the body it points at — the defect that produced a 1,500-character index line contradicting itself in one repository. After editing topic files, regenerate the index:

```bash
python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --no-git --fix-index
```

The result is exactly this and nothing else:

```markdown
# MEMORY — packages/ingest

- [Red — there is no safe blanket push here](memory/alerts.md) — diverged fork; a plain push is rejected and --force destroys the remote …
- [Current state](memory/state.md) — the description of state.md, verbatim
- [Live decisions](memory/decisions.md) — the description of decisions.md, verbatim
```

The heading is `# MEMORY — <path of the scope relative to the repository root>`; the root's own index uses the repository directory's name. Entries are ordered by type — `alert`, `state`, `decision`, `gotcha`, `question`, `watch`, `kill-record`, `evidence`, `log`, `reference` — with the canonical file of a type (`decisions.md`) before its split siblings (`decisions-sync.md`), then alphabetically. The audit reports `MEMORY-INDEX-STALE` whenever the file on disk differs from that derivation by a byte, and `--fix-index` rewrites it — but never invents one: it refuses while any topic in the store has malformed frontmatter, and refuses when the store holds no topic at all (`MEMORY-STORE-EMPTY`), rather than truncating an existing index to a bare heading. **Never hand-edit the index** — a hand-cut hook is the next `MEMORY-INDEX-STALE`, and the next regeneration silently discards it. Only `*.md` topic files live in `memory/`; a `.bak`, a stray note or a subdirectory is `MEMORY-STORE-DEBRIS`.

**Topic files** open with exactly this frontmatter — six lines, then a blank, then the heading, so `head -8` of any topic file returns the complete relevance signal plus that heading:

```markdown
---
name: <scope slug>-<file stem>
description: "one line, at most 240 characters — the index hook, enough to judge relevance without opening the body"
type: state
last_verified: YYYY-MM-DD
---

# <Topic heading — becomes the index title>

body…
```

- **`name`** is `<scope slug>-<file stem>`, kebab-case, and unique by construction. The scope slug is the scope's own directory name lowercased, with dots, underscores and spaces as dashes (`fa-reel`, `my-service`, `ingest`); a *grouping* directory is prefixed by its parent (`repo-packages`), because a tree usually has several and a bare `packages-state` collides once per scope; the repository root is `root`. So `packages/ingest/memory/gotchas.md` is `ingest-gotchas`. The audit reports `MEMORY-TOPIC-NAME` on anything else, and on any duplicate.
- **`description`** is the index hook, and it is the *signal*, not a summary: what a reader who stops at the index must leave knowing ("diverged fork, plain push rejected, force-push destroys the remote"). Hard ceiling **240 characters** (`MEMORY-DESCRIPTION-LONG`) and a floor of 40, with the heading itself never accepted as the description (`MEMORY-DESCRIPTION-THIN`). If the signal does not fit, the topic is two topics or the description is narrating the body; if it fits in a label, it carries no signal. **A description may not contain a markdown link** (`MEMORY-DESCRIPTION-LINK`): it is copied verbatim into the index, one directory above `memory/`, where a relative link resolves from the wrong place — name the file in backticks instead.
- **`type`** is one of exactly ten, always singular — the *file* may be `decisions.md`, the *type* is `decision`, and a plural defeats any tooling that filters on type: `state` (the scope's current state), `decision` (live decisions with their *why*), `gotcha`, `question` (open questions, naming the owner), `watch` (known drift and unpaid debts), `kill-record`, `alert` (red do-not-do-this items — sorted to the top and read at every level passed through), `log` (a dated session index pruned to pointers, never a changelog), `evidence` (assembled facts for a pending human-owner call), `reference`.
- **`last_verified`** moves only when the topic's claims were re-established by inspection in that session — editing is not verifying. A date in the future is a `MEMORY-FRONTMATTER` finding.
- **The heading** is the first body line and is what the index shows as the title (`MEMORY-TOPIC-NO-HEADING`). `alert` topics begin theirs with `Red — ` so the hazard reads as one at a glance (`MEMORY-ALERT-HEADING`).

**Size.** A topic file stays under **60 lines *and* 6,000 characters of body** (`MEMORY-TOPIC-LONG`, `MEMORY-TOPIC-HEAVY`). The line rule alone was met with 22 KB files of paragraph-long lines; the character rule closes that. Past either, it is two topics — split by concern (`decisions-sync.md`, `gotchas-build.md`), never by date. A small scope may carry just `state.md` and one or two others; **never pad to a canonical set** — a `watch.md` whose only content is "the schema has never been exercised" is a line in `state.md`, not a file.

**One home per fact.** A fact lives in exactly one topic file, at the lowest scope that owns it; every other mention is a link. Promote *up* when a fact holds across siblings (a grouping-wide trap goes in the grouping's topic, referenced from below), and point *down* when a higher scope must carry a hazard a reader needs on entry: a root or grouping `alert` is a short pointer — what, why it is red, where the detail lives — of at most ~15 lines, never a copy of the child's topic. Copies begin disagreeing on the next edit; one sweep found the same repository described as pushed in one scope and unpushed in two others.

**Read protocol.** Entering a scope: read its index and every index above it — indexes cascade exactly as the instruction file does. Then `head -8` any topic whose hook looks relevant; deep-read only those that are. `alert` topics are read always, at every level passed through.

**Write protocol.** New concern → new topic file, then regenerate the index. Changed concern → edit the topic file, restamp its `last_verified` if you verified it, and regenerate. Dead concern → delete the file and regenerate. **Never hand-edit the index**; a hand-cut hook is the next `MEMORY-INDEX-STALE`. Relative links inside a topic file resolve from `memory/`, one level below the scope — prefix `../`. Topic files carry no `## Upkeep` section (`MEMORY-TOPIC-UPKEEP`) — the write protocol *is* their upkeep, and a "delete this topic once X" condition is the topic's last sentence. **In a fan-out, pass `--scope <your directory>`** so `--fix-index` regenerates only your own indexes and never a sibling's mid-flight.

Delete anything that stops being true: memory is not a changelog and never grows without bound.

**Do not record volatile integers in prose.** Ahead-counts, behind-counts, dirty-path counts and commit totals go stale *inside the session that writes them* — the context commit that closes the session moves them, so the recording agent invalidates its own sentence before it finishes, sometimes with arithmetic that no longer closes. The audit re-measures every one of these in seconds and is the single source; a number copied into a topic file is a second source that starts disagreeing with it immediately. It flags the git-shaped ones as `MEMORY-VOLATILE-COUNT`, advisory because a count that is *itself* the hazard is legitimate.

**Record the topology, not the integer.** "Safe to push — ahead-only, no deletions in the range" · "diverged fork; a plain push is rejected and `--force` destroys the remote" · "no remote at all, exists on one machine" · "a large uncommitted body that has never been `git add`ed, so `git clean` deletes it with no reflog" — these stay true across sessions and carry the signal a reader actually needs. A number is worth writing down only when it is **itself the hazard and does not move**: "the unpushed range deletes 21 named tracked files" is a fact about what a commit *does*; "3 ahead" is a fact about where a branch pointer *currently sits*. If a count is worth quoting at all, say when it was measured and that it must be re-measured before it is acted on.

## Maintenance duties

1. **Read up the chain before acting.** Root → grouping directory → scope. Later files narrow earlier ones; they never silently contradict them. A real contradiction is a bug — fix it, don't route around it.
2. **Write only the delta.** Never restate a parent's content. Link to it. Duplication is how a tree drifts against itself.
3. **Update the memory store at the end of any session that changed reality** — shipped, decided, discovered, abandoned. Edit the topic files, regenerate the index with `--fix-index`, and restamp `last_verified` only where you actually checked, not merely edited.
4. **Prune.** Resolved questions move into `decision` topics or disappear. Cleared watch items disappear. Keep each topic file under 60 lines and 6,000 characters; delete topic files whose concern died, and let the index regeneration drop their lines.
5. **Promote up, don't duplicate sideways.** A fact that matters across a whole grouping belongs in that level's topic file, referenced by the scopes below — not copy-pasted into each.
6. **Facts beat inference.** Establish state by inspection (`git log`, manifests, tests) before recording it. Never record a status you did not verify. Git claims specifically — committed, pushed, backed up, deploy-ready — decay faster than anything else here and are never repeated from a context file without re-running the command behind them.
7. **Maintain the skills alongside the pair.** If a session changed *how* the work is done, the local skills are as stale as an unupdated `MEMORY.md` would be. Fix or delete them in the same session.
8. **Assess by reading, never by searching.** Read every context file in a scope end to end before changing any of it — the pair, every `memory/` topic, every local `SKILL.md`. Duties 4 and 5 are the ones a grep cannot serve at all: nothing in a search result tells you that a topic file's concern has died, or that the same fact now sits in three scopes and belongs one level up. Deletion and consolidation are normal outcomes here, not exceptional ones.
9. **Never describe a directory's file inventory from inside a file you are adding to that directory.** One pass wrote "this directory contains exactly one file" into four `MEMORY.md` files it was in the act of adding to those directories — false before the write completed. Describe *dormancy and last real activity*, not file counts and mtimes, and warn explicitly when a context file's own mtime should not be read as scope activity.

## Automation

Everything here that does not require judgement is enforced mechanically by `${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py` — run it rather than checking by hand, and `--fix-index` regenerates every stale `MEMORY.md` from its topic files. Its finding codes, what each means, and which ones need a human read: [`context-audit-findings.md`](context-audit-findings.md).

## Precedence

The instruction file outranks `MEMORY.md` on *how to work*. `MEMORY.md` outranks the instruction file on *what is currently true*. If an instruction file asserts a state at all, that is a defect — move it.
