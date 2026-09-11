---
name: fa-foundation-session-close
description: Provides the end-of-session close-out that brings a scope's instruction file, MEMORY.md topic store and local skills back into line with what actually changed, and empties the session's scratch directory. Use it by walking every touched scope in order — memory store, then rules, then skills, then registries, then scratch — promoting anything durable before deleting it, running the context audit, and committing the context edits locally. Use it when a session shipped, decided, discovered, abandoned, unblocked or reorganized anything; before handing work to another agent or a future session; whenever a `.tmp/` still holds files, logs or screenshots; whenever a session report is waiting to be written; and any time you are about to end a turn having changed reality without touching a single `MEMORY.md`. It closes out what this session touched; a periodic drift sweep over every scope is `fa-foundation-context-audit`'s.
license: MIT
---

# Gaia Session Close

## Scope and when to use

Close-out is the pass that makes a session's work legible to whoever arrives next. It runs at **every scope whose reality you touched** — the scope you worked in, the grouping directory above it, and the repository root if the change is repo-wide. Write only the delta at each level; promote upward rather than duplicating sideways.

Use this skill when:

- a session shipped, decided, discovered, abandoned, unblocked, renamed or moved something
- work is being handed to another agent or to a future session
- a `.tmp/` scratch directory still holds files, logs, screenshots or a session report
- you are about to end a turn having changed reality without touching any `MEMORY.md`

Do not use this skill when:

- the session only read, searched or answered questions — nothing changed
- one scope's recorded facts are actively distrusted and need line-by-line re-verification
- the whole repository needs sweeping rather than the handful of scopes you touched

## Required inputs

- the list of scopes whose reality this session changed
- each of those scopes' instruction file and every `memory/` topic file, read end to end
- every decision made this session, and the reason behind each one
- the full contents of the session's `.tmp/`
- an honest split between what you actually inspected and what you merely edited

## Owned outputs

- updated `memory/` topic files at every touched scope, with each `MEMORY.md` regenerated from them
- instruction-file edits, but only where a rule changed
- local skills added, fixed or deleted where the *method* changed
- an empty `.tmp/`, with everything durable promoted first
- a context-audit run whose findings were read, with kept ones recorded as deliberate
- a session report delivered to the caller in the reply — never a file in the repository

## Decision tree

- If the fact is about condition — status, date, progress, a decision — it belongs in the memory store; see `fa-foundation-memory-maintenance`.
- If a rule, convention, command or invariant changed, edit the instruction file via `fa-foundation-context-authoring`. A status change never earns that edit.
- If *how* the work is done changed, fix or delete the local skill — `fa-foundation-create-skill`.
- If a scope was added, killed, renamed or moved, update every registry that names it in the same pass; registered in one place and not the others is drift on day one — `fa-foundation-registry-audit`.
- If you inspected nothing at a scope, do not restamp its `last_verified`. An unverified stamp is worse than a stale one, because it silences the staleness check for another cycle.
- If `.tmp/` still holds anything, the session is not closed — go to the scratch section below before doing anything else.

## Core workflow

1. Open every context file in each touched scope end to end — index, topics, instruction file. **Do not search them.** A close-out decides what to delete and what to merge as much as what to add, and neither is visible in a grep hit.
2. Update the memory store first; this is the step that is never optional. Rewrite the `state` topic if reality moved, and re-cut its `description:` — that line *is* the index hook, capped at 240 characters, so the new signal shows in the index once it is regenerated. Record every decision in the `decision` topic **with its rationale** — a decision stored without its *why* gets reversed by the next agent, who sees only its cost and none of the reasoning that paid for it. Anything that bit you goes in a `gotcha` topic; a red do-not-do-this gets an `alert` topic near the top of the index.
3. Prune. Resolved questions become decisions or disappear; dead concerns lose their topic file, and the index regeneration drops the line; topic files stay under **60 lines and 6,000 characters**. Memory is not a changelog and never grows without bound.
4. Restamp `last_verified` only on topics whose claims you actually inspected this session. Editing a file is not verifying it, and the two are indistinguishable from the outside once the stamp moves.
5. Edit the instruction file only if a rule changed. If you found yourself explaining the same non-obvious rule twice this session, that rule belongs there.
6. Put three questions to the local skills: did one describe a workflow you just discovered is wrong (fix or delete it — a stale skill fires with authority and misleads), did you hit a repeatable procedural trap (that is a skill, not a `gotcha`), did you repeat a multi-step workflow nothing covers (only then add one).
7. Regenerate every touched index — `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --fix-index --scope <your directory>`, carrying `--scope` every time rather than only when other agents are working in the same tree — because `MEMORY.md` is derived from the topic files and is never hand-written; a hand-cut index line is reported as `MEMORY-INDEX-STALE`. Empty `.tmp/` per the section below, re-run the same script without `--fix-index` over the touched scopes, then make the edits durable: **commit** the context edits inside a repository, separately from any product change, with a message naming them as context maintenance. **Never push** — that is the human owner's call in every case. And never `reset`, `rebase`, `checkout` a tracked path, `clean`, `stash` or `restore` to tidy a dirty tree: each of those silently destroys uncommitted work that exists on exactly one disk with no reflog entry to recover it from. Scopes outside a repository have no undo at all — prefer surgical edits to rewrites there.

## Scratch discipline and the session report

Everything temporary a session produces — logs, screenshots, captures, downloaded assets, scraped pages, intermediate files, and the session report itself — lives in a `.tmp/` at the scope being worked in, and nowhere else. Emptying it is a step of the close-out carrying the same weight as the memory store, not a courtesy to the next agent.

**A session that ends with a populated `.tmp/` is not closed.** The audit says so mechanically as `TMP-NOT-EMPTY`, and it is right: files left sitting there are a claim that someone will come back for them, which no one ever does.

**Promote first, delete second — in that order only.** Walk the directory item by item and route each one: a durable fact to the memory store, a rule to the instruction file, a procedure to a skill, lasting reference material to the repository's own reference layer. *Then* delete the file. The order is the whole rule, because both ways of breaking it fail badly and quietly. Delete before promoting and the finding is gone permanently — there is no undo above the repositories, and outside a repository there is none at all. Promote and leave the file sitting there and the next agent finds two copies of the same claim with no way to tell which is authoritative, which is how a memory store starts disagreeing with itself. Both halves, in that order, or the step is not done.

**Your own session report is the case that catches people.** A report is not one of the three context artifacts: nothing cascades it and no audit reads it, so a finding parked in one is invisible to every mechanism that exists. Promote its findings into the memory store of the scope each one concerns, delete the file, and **deliver the report to your caller in your reply — never write it into the repository.** A dated `.md` dropped into a reference directory is a session report wearing a disguise; the audit flags it as `STRAY-ARTIFACT`, and the fix is the same promote-then-delete.

Two scratch rules that only bite later: `.tmp/` belongs in every repository's `.gitignore`, or scratch gets pushed instead of emptied (`TMP-TRACKED`); and if a file in there feels too valuable to delete, that is the signal it was never temporary — promote it now, while you still remember what it was for.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| decision recorded without its why | reopen the topic and add the rationale before closing | closing agent | ask the deciding agent while the reasoning still exists |
| `last_verified` restamped without inspection | re-inspect now, or roll the stamp back to its previous date | closing agent | mark the topic suspect in its index hook |
| `.tmp/` non-empty at close | promote each item, then delete; never defer it to "next session" | closing agent | hand unroutable items to the human owner in the reply |
| dirty tree after context edits | commit the context edits at the scope that owns them | closing agent | escalate; never reset, clean, stash or restore to clear it |
| audit finding you believe is correct as written | record the judgement in the memory store so the next sweep does not relitigate it | closing agent | escalate `POSSIBLE-STATE-IN-INSTRUCTIONS` when it may be a binding rule |

## Anti-patterns

- do not restamp `last_verified` for a topic you edited but did not verify
- do not record a status you inferred rather than inspected
- do not describe a directory's file inventory from inside a file you are adding to that directory — the count is wrong before you save it
- do not edit a `MEMORY.md` by hand at all, least of all by appending to it; edit the topic files and regenerate
- do not write the session report into the repository, least of all as a dated file in a reference directory
- do not push, and do not reset, rebase, checkout a tracked path, clean, stash or restore to tidy up
- do not decide a file needs no change without having read it
- do not drive the audit count to zero; some findings are correct as written

## Handoff and downstream impact

- give the next agent a memory store that answers "what is true here now" without re-derivation
- tell the human owner what is committed and unpushed, and leave the push decision to them — `fa-foundation-repo-durability`
- tell skill and agent maintainers what changed in method, via `fa-foundation-create-skill` and `fa-foundation-create-agent`
- hand a multi-scope drift sweep to `fa-foundation-context-audit` rather than widening this pass
- return the session report in the reply, naming explicitly anything you could not route

## Examples

- **Good fit:** a session that shipped a feature, hit two build traps and made one architectural call — record the decision with its why, the traps as gotchas, fix the build skill, empty `.tmp/`.
- **Good fit:** handing an unfinished investigation to another agent, where the entire handover is what the memory store and the reply now say.
- **Not a fit:** a scope whose recorded facts you actively distrust and want re-verified claim by claim; that is a deep memory refresh, not a delta close-out.

## Completion checklist

- every touched scope's memory store reflects reality, and every decision carries its rationale
- every `description:` re-cut wherever the signal moved, dead topics deleted, and every touched index regenerated rather than hand-edited
- `last_verified` restamped only where inspection actually happened
- the instruction file changed only if a rule changed
- local skills fixed, deleted or added wherever the method moved
- `.tmp/` is empty, everything durable promoted first, report delivered to the caller
- the context audit has been run and its findings read rather than blind-fixed
- context edits committed locally at the scope that owns them, and nothing pushed

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
- [Foundation ownership and conventions](../../references/ownership-and-conventions.md)
