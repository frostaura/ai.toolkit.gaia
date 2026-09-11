---
name: fa-foundation-memory-maintenance
description: Provides the deep-refresh procedure for one scope's memory store — the `memory/` topic files, plus the `MEMORY.md` index regenerated from them — re-established against inspected reality instead of against what the previous version claimed. Use it by reading the whole store end to end, verifying every status by running the command behind it, rewriting the state and decision topics, deleting dead topics, restamping `last_verified` only where you actually checked, and regenerating the index with `--fix-index` rather than hand-editing it. Use it when a session shipped, decided, discovered, abandoned or unblocked something; when a topic file's `last_verified` is older than the repository's freshness window; when an instruction file has accumulated a date, a status, a version pin or a "currently"; or before trusting any recorded status — a git status above all — for a consequential decision. It refreshes the facts in a store that already exists; standing stores up across a whole tree is `fa-foundation-optimize-directory-tree`'s.
license: MIT
---

# Gaia Memory Maintenance

## Scope and when to use

Use this skill to re-establish one scope's memory store — the `MEMORY.md` index and the `memory/` topic files beside it — against reality you inspected, not against what the last version of the file happened to say.

Use this skill when:

- a session shipped, decided, discovered, abandoned or unblocked something
- a topic file's `last_verified` is older than the repository's freshness window
- an instruction file has accumulated a date, a status, a version pin or a "currently" — that content is misfiled and belongs here
- you are about to trust a recorded status, especially a git status, for a consequential decision

Do not use this skill when:

- the scope has no `memory/` directory yet, or the split between the two context files is itself wrong — that is authoring, not maintenance
- every scope in the repository is suspect at once — that is a whole-repository sweep, run per scope with disjoint ownership
- you merely finished a routine session and need to leave a correct trace across several scopes

## Required inputs

- every topic file under `memory/`, and the `MEMORY.md` index they derive to
- the scope's instruction file, plus every instruction file above it in the cascade
- direct access to the underlying reality — working tree, build, test command, deployment target
- the current output of `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py`

## Owned outputs

- topic files whose every claim was verified in this pass, each carrying an honest `last_verified`
- topic `description:` lines that carry the state signal without the reader opening a file, and an index regenerated from them
- deletions — dead topics gone, converged topics merged
- discrepancies found outside this scope, reported to the caller rather than written into this store

## Decision tree

- If the scope has no `memory/` directory, stop and author the store first; maintenance assumes one exists.
- If a claim cannot be verified by running something, do not record the claim — record the open question and name its owner.
- If a fact holds across sibling scopes, write it one level up and reference it downward.
- If a topic's concern is dead, delete the file and regenerate the index; do not leave a tombstone in place.
- If what you are about to record is a count, apply *Topology over volatile integers* before writing a digit.

## Core workflow

1. Read the whole store end to end — *every* topic file, and the index they derive to — before editing any of it. A store is one artifact spread across a dozen files and is coherent or incoherent only as a whole, so the three defects this pass exists to catch are the three no search can surface: a topic whose concern died, two topics that have converged into one, and a `description:` that no longer describes the body it sits above. `ls memory/` locates the store; it never assesses it.
2. Verify by inspection, never by reading the previous `MEMORY.md` and adjusting it. Minimum sweep for a code scope: `git log -1 --format='%h %cd %s'`, the current branch, `git status --short`, whether a remote and an upstream exist, whether anything is unpushed, and whether the test command actually asserts something rather than passing vacuously. For a non-code scope: which files exist and when they last changed.
3. Rewrite the `state` topic as one tight file — stage, last real activity with a date, what works, what is half-built. No hedging, no aspiration in the present tense. Re-cut its `description:` — that line *is* the index hook, so the signal must show there without the file being opened, inside 240 characters.
4. Move every decision into the `decision` topic *with its why*. A decision recorded without its rationale is reversed by the next agent, who sees only its cost — this is the single highest-value content in the store. A red do-not-do-this item becomes its own `alert` topic, indexed near the top.
5. Promote up, never sideways. A fact that holds across sibling scopes belongs in the grouping level's topic file and is referenced from below; copied into each scope, the copies begin disagreeing on the next edit.
6. Prune, and treat pruning as the test of whether this pass actually happened. Resolved questions become decisions or disappear; cleared watch items go; a dead concern means deleting its file; two topics grown into each other become one. Keep each topic under **60 lines and 6,000 characters** — the line rule alone is met by paragraph-long lines, and the character rule is what closes that. **A refresh that only added has not been done** — addition is the one operation justifiable without having read anything, which is exactly why a never-pruned store is the signature of a never-read one.
7. Restamp `last_verified` only on topics you genuinely verified — editing a file is not verifying it — keep the six-line frontmatter intact so `head -8` still yields the full relevance signal plus the heading, then **regenerate the index**: `python3 ${CLAUDE_PLUGIN_ROOT}/scripts/context-audit.py --fix-index` — adding `--scope <your directory>` whenever other agents are working elsewhere in the same tree, so you never rewrite a sibling's index mid-flight. The index is derived from the topic files' `# H1`, `description:` and `type:`; hand-editing it is how a hook and its body start disagreeing, and the audit reports any hand-edit as `MEMORY-INDEX-STALE`. Then re-run the script and resolve what it reports.

## Topology over volatile integers

Ahead-counts, behind-counts, dirty-path counts and commit totals **go stale inside the session that writes them**. The closing context commit — the one this skill exists to produce — moves the branch pointer and dirties the tree, so the sentence is false before the session ends, sometimes with arithmetic that no longer adds up. The next agent reads it, believes it, and acts on a number that was true for about a minute.

Record the **topology** instead: the shape of the situation, which survives the commit and carries the signal a reader actually needs.

- *safe to push — ahead-only, no deletions in the range*
- *diverged fork; a plain push is rejected and `--force` destroys the remote*
- *no remote at all; this exists on one machine*
- *a large uncommitted body that has never been `git add`ed, so `git clean` deletes it with no reflog*
- *the deployment runs from a tag, not from the branch tip*

A number earns its place only when it is **itself the hazard and does not move**. "The unpushed range deletes 21 named tracked files" is a fact about what a commit *does* — true for as long as that commit exists, and the entire reason to look before pushing. "3 ahead" is a fact about where a pointer *currently sits*, and the audit script re-measures it in seconds, which makes any copy of it a second source that starts disagreeing immediately. If you quote a moving number anyway, date it in the same sentence and say plainly that it must be re-measured before anyone acts on it.

This is not cosmetic accuracy. Two of the ordinary ways a store gets someone's work destroyed are an agent force-pushing because a topic file said *ahead-only* when the branch had since diverged, and an agent running a cleanup because a topic file said the tree was clean when a body of never-added files was sitting in it. Topology would have been right in both cases; the integer was already wrong in the hour it was written.

## Failure recovery

| Failure mode | Recovery | Owner | Escalation |
| --- | --- | --- | --- |
| store read only in part | stop editing and read every topic end to end, then restart the pass | maintainer | none — this is not escalatable, it is redoable |
| a claim that cannot be verified | record it as a `question` topic naming its owner, not as state | maintainer | raise to the human owner if the answer gates a decision |
| a volatile integer already in the store | replace it with the topology it was standing in for | maintainer | re-run the audit to confirm nothing else quotes it |
| a claim about another scope's context file | delete it here and report the discrepancy to the caller | maintainer | the reconciling pass at the root is the only writer allowed across scopes |

## Anti-patterns

- do not record a status you did not inspect, or restamp `last_verified` for a file you only edited
- do not judge a topic file you are maintaining from its `description:` or the rest of its frontmatter; `head -8` is a relevance signal for a reader deciding whether to open a file it does not own
- do not hand-edit `MEMORY.md`; edit the topic files and regenerate
- do not record what a context file **outside this scope** currently says — in a fan-out a sibling agent is editing it, so the claim is stale before you finish the sentence
- do not write aspiration in the present tense; if a team, a customer or a deployment does not exist, say so
- do not delete an open question because it is uncomfortable — move it up the tree if it is not yours to answer
- do not let the store become a changelog; it records what is true now, not what happened in order

## Handoff and downstream impact

- tell the next session which topics were verified in this pass and which were left standing on an older stamp
- tell the caller about every discrepancy you saw outside this scope instead of writing it into this store
- tell the instruction-file maintainer about any state you moved out of a normative file, so the rule that remains reads cleanly
- tell whoever owns the repository's procedures when a trap you just recorded is really a recurring procedure, and belongs in a skill via `fa-foundation-create-skill`

## Examples

- **Good fit:** a delivery session ends with a merged branch, one reversed decision and a dead open question; the store is re-read, the `state` topic rewritten, the decision recorded with its why, the question's topic file deleted, and the index regenerated.
- **Good fit:** a topic file claims a scope is "2 commits ahead, clean"; inspection shows a diverged branch and a body of never-added files, and the claim is replaced with the topology plus the hazard it implies.
- **Not a fit:** a scope has an instruction file and no `MEMORY.md` at all — the pair is missing, which is an authoring job, and there is nothing here to refresh.

## Completion checklist

- every topic file in the store was read end to end before any of them was edited
- every recorded status was established by running something, and `last_verified` moved only where that happened
- at least one thing was deleted, merged or demoted, or the pass can explain why nothing had died
- no moving count appears anywhere in the store without a date and a re-measure warning
- every `description:` carries the signal inside 240 characters, and the index was regenerated rather than hand-written
- the audit script runs clean, or every remaining finding has a recorded reason to stay

## References

- [The context cascade](../../references/context-cascade.md)
- [Context audit finding codes](../../references/context-audit-findings.md)
