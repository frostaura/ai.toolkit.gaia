# Context Templates — the pair, level by level

Four instruction-file templates (root · scope · grouping directory · project), one mirror-file template, and a worked body for each memory archetype. Fill these in for a consuming tree; they are not descriptions of Gaia's own repository.

Everything here is **structure-neutral by construction**. The only labels are root, scope, grouping directory and project; no org-chart noun appears anywhere, because the same four templates have to serve a solo developer's folder of three side projects, a single workspace monorepo, and a tree of research notes or operational runbooks that never builds anything. Where a template section only makes sense for one of those, it is marked **include-when** and the trigger is named.

## Three nouns, defined once

- A **scope** is any directory that carries the pair. A directory earns one when it has working rules an agent would otherwise get wrong — never because it exists.
- A **grouping directory** is a directory whose only job is holding scopes: `projects/`, `packages/`, `apps/`, `services/`. It carries rules about its *children*, not about itself. The name is a parameter, not a constant.
- **The pair** is the instruction file plus the `MEMORY.md` beside it. The instruction file is `CLAUDE.md` or `AGENTS.md` — whichever the tree chose; the other becomes a pointer that never accumulates policy of its own.

Two more used constantly below. A **delta** is what only this level can say; anything the cascade already delivers is a restatement, and the failure of a restatement is never verbosity — it is two copies that later disagree. A **topic** is one frontmattered file in `memory/` covering one concern, indexed from `MEMORY.md`.

## The selection rule — how many templates a tree actually needs

**A flat tree uses root + project only.** That is the modal shape and it is not a degenerate case: a folder holding three unrelated projects, or a single repository with no sub-projects at all, is correctly served by two files at root and one pair per project.

Add the other two only on their triggers:

| Template | Emit it when | Do not emit it when |
|---|---|---|
| **Root** | Always. Even at N=1. | — |
| **Project** | Always, for each scope with a deliverable. | — |
| **Scope** | A middle layer has rules **of its own** that narrow root and bind its children. | The middle layer is a folder. |
| **Grouping directory** | A directory's only job is holding children, and there are rules about *how a child is structured*. | There is one child, or the schema is nothing but "same as root". |

**An empty intermediate scope file is worse than no scope at all.** It reads as a completed artifact, so the next agent trusts it, finds nothing, and stops looking upward — while every future session pays to load it. The same test that creates a scope deletes one: has this directory working rules an agent would otherwise get wrong? If the honest answer is no, the correct output is no file.

The same discipline governs sections *inside* a template. An emitted heading with nothing true under it reads as a completed section; a stranger cannot tell "N/A" from "nobody filled this in". **Omit the heading entirely** rather than emitting it empty.

## What is deliberately not in this file

Frontmatter shape and key order, the derived-`MEMORY.md` index discipline, the closed `type:` vocabulary, the read and write protocols, the topic size caps, and the topology-over-integers rule are canonical in [the context cascade](../../../references/context-cascade.md) and are **not repeated here**. This file supplies what that document does not: filled-in *shapes* for the four instruction levels, and a worked *body* for each memory archetype. Finding codes referenced below are defined in [the context audit findings](../../../references/context-audit-findings.md).

---

## The upkeep clause — written once, carried by every level

Every one of the four templates ends with `## Upkeep`, and every one carries **this** clause, parameterised. It is the single most restated paragraph in any tree that has one, so it is written here once and each template points at it.

**Two things about it are non-obvious and both are load-bearing.**

First, **the clause names all three artifacts — the instruction file, the memory store, and the skills that fire at this level — not just the file it sits in.** A clause that mentions only its own file teaches the reader that the other two are somebody else's problem, and they then become nobody's.

Second, **it states at every level that `MEMORY.md` is required exactly as the instruction file is, and cascades downward on identical terms.** This is the one sanctioned exception to *never restate a parent*, and it must be annotated as such or an executor applying the delta rule uniformly deletes it from every child. The reason it is exempt: an agent entering the tree at this depth may never read the level that states it. A rule about *how to read the cascade* has to be stated in the child's own file, or it does not reach the reader who needs it. Rules the cascade already *delivers* are never restated — only the rules governing the cascade itself.

The canonical text, with `<children>` the only token to substitute (`every project below`, `every package here`, `every child scope`; at a project with no children, drop that clause):

```markdown
## Upkeep

This file, `MEMORY.md`, its `memory/` topic store, and the skills that fire at this level are kept current **as changes land**, not in a later cleanup pass. `MEMORY.md` is required here exactly as this file is, and it cascades downward on identical terms — <children> carries its own pair too, and an instruction file with no `MEMORY.md` beside it is a defect, not a shortcut: the rules are stated and reality is left unstated, so the next agent infers status instead of reading it. Both files, or neither.

- **Memory — always.** Update the topic files at the end of any session that shipped, decided, discovered, abandoned or unblocked something; re-cut the `description:` of any topic whose signal moved, then regenerate `MEMORY.md` from the topic files rather than editing it; restamp `last_verified:` only for what you actually re-inspected. Record decisions *with their why* — a decision without its rationale is reversed by the next agent, who sees only its cost.
- **This file — only when a rule changed.** A new convention, command or invariant earns an edit. A status change does not; it belongs in the memory store.
- **Skills — when *how* the work is done changed.** Fix or delete a skill whose commands or thresholds no longer exist; a stale skill is worse than a missing one because it fires with authority. A procedure that would be correct in a repository with nothing to do with this tree belongs upstream in a plugin, never forked into this tree.
- **Scratch — always.** Empty `.tmp/` before the session ends, promoting anything that still mattered first.

None of these four are assessed by search. Read the scope's files end to end before changing any of them. Drift is not caused by neglect over months; it is caused by good sessions that ended without a close-out.
```

**Root's copy adds one line and nothing else** — the mechanical check, named by purpose rather than by path:

```markdown
The mechanical check is the context-audit script shipped with the installed `foundation` plugin; invoke it through the audit capability (e.g. `fa-foundation-context-audit`) rather than by path, and run it before and after any maintenance pass. One of its finding classes is permanently judgement-required: `POSSIBLE-STATE-IN-INSTRUCTIONS` cannot tell a locked product rule from a status line, because to a regex they are identical. An agent that drives that finding to zero has probably deleted a rule.
```

**Never emit a plugin-relative path into a consuming tree's own files** — not into an instruction file, not into a memory topic, not into a mirror file. A path variable that a plugin expands in its own context is an ordinary literal string once it has been pasted into somebody else's tree: it expands to nothing, resolves to nothing, and the command sitting in their root instruction file is permanently broken with no link checker able to say so. This is the same rule this document already binds on skill names — **name the capability by purpose and let the capability resolve its own path** — and it binds harder here, because a wrong skill name merely fails to fire while a wrong path looks runnable.

Emit the clause **in full** at every level. Do not emit a pointer to another level's copy: the reader who most needs it is the one who entered here and will never read the other file.

---

## Template — the root instruction file

Ordered. **U** = unconditional, **IW** = include-when, with the trigger stated. Seven sections survive all three tree shapes unchanged; the rest are conditional, and on a single-repository tree most of them are correctly absent.

```markdown
# <Tree name>

<One sentence: what this tree is and who works in it.> This file is canonical for **how to work here** — where any other document in this tree conflicts with it, this file wins.

<IW — a superseded document exists: `<path>` is kept for provenance and is **not** canonical; <what replaced it> supersedes it in every conflict.>

## The context cascade — read this first

Every directory here that carries meaning carries **two** files, and you read both, all the way up the chain, before acting.

| | this file | `MEMORY.md` |
| --- | --- | --- |
| Answers | What is this, and how do I work here? | What is actually true here right now? |
| Nature | Normative — rules, conventions, standards | Observational — state, dates, decisions, gotchas |
| Changes | Rarely; a change is a policy change | Often; every session that changed reality |
| If wrong | The rule was wrong | The world moved |

The instruction file outranks `MEMORY.md` on *how to work*. `MEMORY.md` outranks it on *what is currently true*. **If an instruction file asserts a status, a date or a version, that is a defect** — move it to the memory store. The test: a sentence carrying a date, a status, a version, a "currently" or a "we decided" is memory. Navigation is instruction; condition is memory.

Alongside the pair sits a **third artifact: skills** — how a recurring job is done here. Generic procedure arrives from installed plugins and is not a file in this tree; anything confidential or shaped by this tree specifically lives in a local `skills/` directory and cascades exactly as the pair does.

**Write only the delta.** A child never restates its parent — link instead. Restatement is how one paragraph ends up at four depths and then disagrees with itself. **Read whole files:** search *locates* a scope, it never *assesses* one; deletion and consolidation are normal outcomes of maintenance and neither is visible in a search result.

## Local memory is canonical

The tree's `MEMORY.md` and `memory/` topic files are the source of truth for system state. Hosted or persistent memory tooling may be used alongside them, never instead of them. Any session that records something durable in a hosted store records it locally in the same pass. Local memory cascades, is reviewable in a diff, survives losing access to the service, and is the only copy a fresh clone gets.

<IW — more than one scope>
## Scopes

One line per child: name, then a one-sentence identity. Reuse that sentence verbatim wherever this child is described, so it is not described three different ways in three files. Nothing else lives here — no status, no roster, no rules the child owns.

- `<name>/` — <one-sentence identity>

<IW — more than one scope>
## Routing — where does X go?

First matching rule wins.

- <kind of work that stays at root> -> **stays here**
- <kind of work> -> `<scope>/`

If a request genuinely spans two scopes, pick the one that owns the *primary deliverable* and reference the other from that file.

<IW — more than one project>
## Project registry

<Identifier · owning scope · one-line identity, one row per project. No status: live status lives in <the owning grouping's `MEMORY.md`>, which is the only level that can observe it, and root would be wrong about it within a week.>

<Where a machine-readable manifest owns membership — a workspace file, a solution file, a members list — point at it and record only what it cannot express (owning scope, identity line, retired entries). Never re-list names the manifest owns; that is a second source of truth that starts disagreeing the same day.>

**Retired — do not reason about these as live assets.** <identifier> — <retired date, and where the reusable capability actually went>. Record a retirement *before* deleting anything, never after.

<IW — a convention governs more than one name>
## Naming conventions

<Rules that make a directory name itself information.>

<IW — rules exist that an agent would otherwise get wrong>
## House rules

Headlines only — each links to the document that governs it. Do not re-derive these from memory; read the linked document when the decision is real, and apply the document rather than your recollection of it.

- **<Rule>.** <One line.> -> [`<path>`](<path>)

<Where a refusal is absolute rather than weighed, mark it as absolute: it is not traded off against cost or convenience.>

<IW — a `docs/` layer exists>
## Reference index

Read the relevant document before producing detailed output in that domain. Reference these documents; do not inline them.

- [`<path>`](<path>) — <what it covers, and when to read it>

<A superseded document is labelled as superseded here **and** in the document itself, with what replaced it. A folder of speculative or pre-decision material says plainly that an entry in it is not evidence the thing exists.>

## Skills at this level

<Name each capability **by purpose**, with the skill name given as an example only — a plugin skill is not a file in this tree, so no link checker can detect a rename.>

- <Closing out a session that changed reality> — e.g. `<skill-name>`
- <Authoring or refreshing a scope's pair> — e.g. `<skill-name>`

Generic procedure comes from the installed plugins: it is installed, versioned and consumed, never edited from here. **Never fork a plugin skill into this tree** — a stale generic procedure is fixed upstream and reinstalled. Local skills live in the narrowest directory that needs them and are **never mirrored downward**.

## Fan out by default

<Paste the fan-out block verbatim — see below. Do not paraphrase it.>

## Scratch — `.tmp/`

Nothing temporary is written into the tree proper. Every log, capture, downloaded asset, intermediate file and session report goes in a `.tmp/` at the scope being worked in — not in `docs/`, not beside the file you are editing. `.tmp/` is listed in the `.gitignore` of every repository that hosts one.

**Emptying it is part of finishing.** A session that leaves `.tmp/` populated is not closed. Anything still worth having was never temporary: **promote it, then delete the file** — a durable fact to the memory store, a rule to an instruction file, a procedure to a skill, reference material to `docs/`. One carve-out: a report *deliberately promoted* into `docs/` **and referenced** from an instruction file or a memory topic has become evidence; an unreferenced one is debris.

<IW — this tree carries scripts of its own>
## Scripts

<What each script checks, and how to read its output.>

<IW — both instruction filenames exist in this tree>
## Interoperability

`<canonical filename>` carries the content at every level. `<mirror filename>` is a **pointer, not a second source of truth**, and it never accumulates policy of its own — if it starts explaining rules instead of pointing at them, delete the explanation. A mirror file inside an imported repository is that project's upstream engineering artifact, and that project's own instruction file wins on every conflict.

## Upkeep

<The canonical upkeep clause, plus root's mechanical-check line. Emit both in full.>
```

**What root deliberately does not say.** It fixes each child's identity string and stops — it does not restate any child's rules, conventions, roster or team. It carries the registry's *identity* columns and pushes *status* down to the level that can observe it. It names capabilities it does not own and forbids the local copy that would let them drift. It carries the scratch *rule* and links the *justification*. Each of those is a place a root file grows into a second copy of every child's file if the boundary is not held deliberately.

**Which sections survive which tree.** Sections 1–3, skills, fan-out, scratch and upkeep survive all three shapes unchanged — that is the unconditional set, and it is seven sections, not twenty. On a **single repository**: scopes, routing and registry are vacuous and must be omitted; naming conventions often survives (package naming); scripts often survives. On a **solo developer's folder of three side projects**: scopes, routing and registry survive but thin; naming conventions usually fails; house rules often fails; reference index fails. On a **research or runbook tree**: house rules survives with different content (data-handling rules, never mutate the raw inputs); the registry survives as a dataset or runbook index; reference index usually survives; **nothing may assume a build or a deployable artifact exists.**

### The fan-out block, pasted verbatim

This block lands permanently in the consuming tree's root file. **Paste it; never paraphrase it** — a paraphrase is a second standard the moment it lands. Use "subagent" throughout and never "branch": this artifact lives in a stranger's tree forever, and "branch" means a git branch to most of its readers.

```markdown
## Fan out by default

Any work that creates, generates, analyses or reviews is fanned out to parallel subagents and workflows unless it is genuinely one indivisible thing. A single pass is the exception and needs a stated reason; the reason is never "it seemed quicker."

- **Prefer a workflow or an existing skill to an ad-hoc pass** — a brief written from memory drifts from the documented method immediately, and nothing announces that it has.
- **Split into disjoint scopes before launching anything, and write the partition down** — two agents in one subtree overwrite each other; last writer wins and neither report says so.
- **Launch every independent subagent concurrently, in a single dispatch** — a fan-out run one at a time costs what doing it yourself costs and returns none of the advantage.
- **Give every subagent the same brief** — then differences between reports are real signal.
- **Analysis, review and QA get independent, dispassionate reviewers — more than one where the call is consequential.** An agent reviewing its own output defends what it just wrote.
- **Reconcile at the parent last, once, from finished reports.**
- **A subagent writes inside its own scope and returns a report to its caller; it never writes above its scope and never writes a report into the tree.**
- **Serializing independent work is a defect, not a style choice.** *Where the tool running this file has no subagents, the partition-first, one-brief and reconcile-last rules still bind and the work runs serially — that is the exception, and it needs no separate reason.*
```

The final clause is not optional. This block lands in trees whose owners work in surfaces where subagents do not exist; without it, their root file instructs every future session, forever, to do something impossible and calls doing otherwise a defect.

### Template — the mirror file

Where the tree carries both filenames, the one it did not choose gets **this and nothing else**, about fifteen lines. Its whole job is to survive the next well-meaning agent who wants to be helpful.

```markdown
# <Tree name>

This file is a **pointer, not a source of truth**. The rules for working in this tree live in [`<canonical filename>`](<canonical filename>) beside it; read that file, then the `MEMORY.md` beside it, then the same pair at every level down to where you are working.

The rules themselves are stated once in `<canonical filename>` and specified in full in the documents it links. They are deliberately **not** repeated here; a second copy is how the two drift apart.

## Where the rules actually live

- [`<canonical filename>`](<canonical filename>) — how to work here
- [`MEMORY.md`](MEMORY.md) — what is true here right now
- <the cascade standard> · <the local skills index> · <the mechanical check, named by the capability that runs it — never by a plugin-relative path>

A mirror file inside an imported repository is that project's own upstream artifact; that project's instruction file wins on every conflict.
```

The refusal in the second paragraph, with its reason attached, is the load-bearing line. Written that way, a future agent tempted to helpfully expand this file reads *why not* before it reads the empty space.

---

## Template — a scope instruction file

Emit only where a middle layer has rules of its own.

```markdown
# <Scope name>

## What this scope is

<One paragraph placing this scope against its parent **and its siblings** — the job it does that the others do not. Close with one bolded sentence stating what success here means.>

## What this scope owns

- <category of work>

<IW — siblings exist> <A closing paragraph routing adjacent work to whoever actually owns it. Name only the boundaries an agent working *here* will actually hit — never restate the parent's whole routing table.>

<IW — this scope directly holds children and no grouping directory owns the roster>
## Children

Live status, dates and health live in [`<grouping>/MEMORY.md`](<grouping>/MEMORY.md), never here.

| Child | Identity |
|---|---|
| `<name>` | <one sentence> |

<Where a workspace manifest owns membership, point at the manifest and list only what it cannot express.>

<IW — a grouping directory sits beneath this scope>
## How a child here works

<Two to four rules that make the child schema binding — "no <X>, no <child>". The schema itself lives in the grouping directory's own file and is linked, never inlined.>

## Conventions

These are scope-wide defaults. A child may narrow them in its own instruction file; it may not silently ignore them.

| Rule | Why it exists |
|---|---|
| <rule> | <the reason, stated so a future agent does not overturn it on cost alone> |

<IW>
## Non-obvious rules

<Timeless traps only. A trap that already cost hours and may now be fixed is a `gotcha` topic in memory, not a rule here.>

<IW>
## Never do this here

- Never <action an agent would otherwise take reasonably, at this altitude>

<IW — a docs layer or a parent standard exists>
## Where things live

- [`<path>`](<path>) — <what it covers>

<Where a pointer's target can disagree with reality, say what to do about it — "treat a mismatch between <document> and what is actually here as an escalation, not something to resolve locally." That is the one thing the pointed-at document cannot contain.>

## Skills at this level

<Capabilities that fire on work inside this scope, named by purpose.> <Where there is no local set, say so in one line — "this scope has no local skills of its own" — rather than omitting the heading; an absent section reads as an oversight and the next agent goes hunting.>

## Upkeep

<The canonical upkeep clause, in full.>
```

**What a scope deliberately does not say.** It names only the routing *edges* its own work crosses, never the map. It delegates the child schema downward and keeps only the rules that make the schema binding. It records a *local condition* rather than a parent rule — "several repos here carry both filenames, and every mirror in this scope is a short pointer" is the delta; restating which filename outranks which is not, and it goes stale the moment root is reworded. Where the parent's absolute prohibitions touch this scope's work, one clause at the collision point beats a restated policy section — and where there is no collision, silence is correct.

**Two sections were deliberately dropped from this template** and should not be re-added: a roster of role framings or personas (inert for one person, meaningless in a research tree, and it reads as a staff list), and brand or asset direction (a `docs/` concern the scope should point at).

---

## Template — a grouping directory instruction file

The level whose entire reason to exist is the child schema. **A grouping directory with no child schema is an empty file** — delete it and let the children inherit from the scope above.

```markdown
# <Parent scope> — <children>

Each subdirectory here is one <child noun>. This file defines **how a <child> is structured**, not what exists. Parent context in [`../<instruction file>`](../<instruction file>) applies to everything here unless a <child> explicitly narrows it.

<IW — true> This is the only place a <child> is created; nothing <child>-shaped is created elsewhere under `../`.

The live roster — what exists, and each child's status, dates and health — lives in [`MEMORY.md`](MEMORY.md). **Do not list <children> in this file**: a roster written into a normative file rots the moment a child is added or retired.

## <Child> instruction-file schema

<Child> files inherit this scope's context through the cascade — do not restate it. A <child> file exists to capture what is <child>-specific. Sections, roughly in this order:

1. **Title** — display name plus the identifier the registry and roster use.
2. **What this <child> is** — one paragraph: what it is, who it serves, the primary use case.
3. **How it is organised & key concepts** — the shape, plus the vocabulary an agent must load before touching anything.
4. **Conventions & non-obvious rules** — the contracts that must not be broken and the invariants that look optional and are not.
5. **Build, test, run** — the real commands, with ports and required environment. <For a <child> with no executable surface, this is replaced by "how the work here is produced and checked". "No build step — this is a document set, not a system" is correct content when true; a blank or an invented command is a defect.>
6. **Docs** — <where its reference lives, and which parts are authoritative versus input only.>
7. **Never do here** — actions that break the <child> or its trust posture.
8. **Cross-<child> dependencies** — shared infrastructure, identity, internal APIs, upstream data. **An explicit "none" is a required answer, not an omission.**
<IW — this grouping has a domain question worth forcing>
9. **<The one question this <child> answers in its own words>** — and inability to state it is itself a placement signal, not something to invent an answer for.

Anything carrying a date, a status, a version, a count, a "currently" or a "we decided" goes in that <child>'s `MEMORY.md` instead. **An instruction file asserting a state is a defect.**

## Rules for this level

Lifecycle and coordination only; nothing that belongs to a single <child>.

- **Adding a <child>:** scaffold the pair, then register it <in this level's roster and in the root registry>. Registered in only one place, it drifts immediately.
- **Retiring a <child>:** write the retirement record into [`MEMORY.md`](MEMORY.md) here — the date, what triggered it, and where the reusable capability actually went — **before** the directory is removed. "Where it went" names a surviving document or repository, **never a staging directory**: a record citing a staged copy as the only copy is a defect at the moment it is written, and it must be fixed then, while the files still exist.
- **Verify state by inspection before recording it.** Run the command; do not repeat an inherited status sentence. Inherited status text at this level has repeatedly been wrong.
<IW — the children share a real surface>
- **Work touching several <children>** — shared identity, shared billing, shared infrastructure — is decided at [`../<instruction file>`](../<instruction file>), never forked into each child.
<IW — a machine-readable manifest owns membership>
- **Membership is owned by [`<manifest>`](<manifest>).** The roster records only children whose status differs from live and points at the manifest rather than re-listing names.
<IW — above roughly forty children>
- **The roster splits by sub-grouping** (`roster-<area>.md`) rather than growing past the topic line cap. The same split rule binds every enumerating topic at this level.
<IW — a root-owned file is canonical for a cross-cutting fact>
- **<Root> is canonical for <that fact>.** Read it before asserting <the fact>; the copies down here have been observed stale.

## Skills at this level

<Only what fires when adding, moving or retiring a child — never the full menu. Named by purpose, with the skill name as an example.> <Where there is no local set, say so in one line.>

## Upkeep

<The canonical upkeep clause, in full.>
```

**What a grouping directory deliberately does not say.** It does not name its children — that is the roster, one file away, and it decays fastest. It states the delta rule at the exact level that enforces it and does not re-describe the parent's context in order to do so. It applies the instruction/state split per schema *item*, at the one or two items where authors actually get it wrong, rather than restating the principle. It replaces an entire restatement of the parent with nine words of pointer.

---

## Template — a project instruction file

**Governing rule, stated above the sections:** where a grouping directory declares its own child schema, the project conforms to **that** schema; this template is the default only where no schema is declared. A template that overrides a declared local schema has created a second standard immediately.

**Second rule:** this file captures only what is project-specific. A project file that re-explains an inherited rule has made a copy that will disagree with its original.

**The executable-surface gate** decides two whole sections: a project has an executable surface if it has a build file, an entrypoint, routes, a schema, or a deployable artifact. Where it does not, sections 5 and 6 below are replaced by **one** section — "how the work here is produced and checked".

```markdown
# <Display name> — `<identifier>`

<IW — the project is an experiment rather than a deliverable, in which case this goes FIRST, above "what this is": for a bet, the question outranks the description>
## The question this tests

<One falsifiable hypothesis, stated so it can come back false. If there is genuinely no question, only a product opinion, say that plainly rather than inventing one.>

## What this project is

<One paragraph: what it is, who it serves, the primary use case. The only section that must be readable by someone who has never opened the directory. If it is a document set and not a system, say so in one honest sentence.>

## How it is organised & key concepts

<The shape of the thing, plus the domain vocabulary an agent needs loaded before touching anything. Prefer a directive heading where a real obligation attaches — "The retrieval design — read this before touching search" beats "Architecture".>

<IW — executable surface>
## Stack & repo

<Languages, frameworks, datastore, repo URL and branch — noting **only** what departs from or sharpens the inherited default. Every departure is a *declared deviation*: name what is overridden, by whose authority, and why. A silent divergence is indistinguishable from a mistake six months later. "No repo yet" is a valid answer.>

<IW — executable surface>
## Build, test, run

<The actual commands, with ports, required environment, and the one gate that must be green before claiming done. Name variables and where they are sourced; **never a secret value.** Any shape — a target table, a fenced block, prose — is fine.>

<Where there is no executable surface, this and the section above are replaced by:>
## How the work here is produced and checked

<The real process. "No build step — this is a document set, not a system" is correct content when true.>

## Conventions & non-obvious rules

The filter: things that look like a bug and are not. Each entry says *why*, or the next contributor "fixes" it.

- <invariant that looks optional and is not>

## Never do here

- Never <action that breaks the product or its trust posture>

<Where a rule exists because it was already broken once, say so and point at the memory topic that carries the incident.>

## Cross-project dependencies

<Shared infrastructure, identity, internal APIs, upstream data — or the single word **none**, which is a required answer rather than an omission.>

<Deliberate **non**-couplings belong here too: "that project mirrors this one's environment-key names for consistency — a convention, not a coupling, and the names should not be cleaned up." Nothing else in the tree records that, and without it a well-meaning refactor breaks a deliberate parallel.>

<IW — a docs layer exists>
## Docs

<Where this project's reference lives, and — the part only this level knows — which parts are authoritative and which are input only.>

<IW — a decision class is reserved to someone other than whoever is reading>
## Ownership & decision rights

<Which decisions are **not** settled here. Never a staff list: a name and a role are state and belong in the memory store.>

<IW — a licence, legal, safety or data-use constraint binds>
## Compliance & risk

<Stated as a gate, not a footnote.>

<IW — the layout is non-obvious>
## Repo layout

<Top level only. A deeper tree rots on the first refactor and nothing mechanical catches it.>

<IW — one exists>
## Blocking gates that are not code

<Things that gate shipping and cannot be closed by writing more code — a name clearance, a licence, an external approval, a capture nobody has taken. Name the gate, never its status.>

<IW — experimental; these four travel as a set>
## Milestones
<2–5 falsifiable gates. Definitions, never statuses — whether one has been met is the memory store's answer.>
## Ends if
<The explicit conditions under which this work stops. The cheapest thing a template can force someone to write down.>
## Where it goes if it works
<Destination, and what must be true first.>

## Upkeep

<The canonical upkeep clause, in full.>
```

**What a project deliberately does not say.** The cleanest delta at this level is *the deviation, not the rule*: "no frontend — the parent's frontend convention does not apply to a headless service", and "no database and no ORM: the service is stateless by design; a deliberate departure recorded as a decision in the memory store." Three moves in two sentences — name what is overridden, say why, push the decision record to memory — and at no point restate what the parent default is.

The second-cleanest is *pointing at state instead of carrying it*: "the current test count lives in `MEMORY.md`, not here" · "which providers are actually live is recorded in `MEMORY.md`, never here". The instruction file says **where**; the memory file says **what**. Neither repeats the other's sentence.

And the one that only exists at this level: **record an unresolved conflict with the parent rather than hiding it.** "This does not use what the parent convention requires. That gap is *not* a declared deviation — read `MEMORY.md` before writing any code in this area." Delta discipline is not only omitting inherited rules; it includes naming, in the child's own file, where the child is knowingly out of compliance — instead of diverging silently or restating the rule as though it were being followed.

---

## Two counter-examples, kept deliberately

The delta rule is learned from violations, not from statements of it. Both of these were written in good faith by competent authors, which is exactly why they are worth showing.

**A roster inlined into an instruction file.** A grouping directory grows a section listing each child with a one-line identity, hedged with "identity only — live status is in `MEMORY.md`". The hedge does not rescue it. The same identity lines now exist in three places — this file, the scope file one level up, and the roster — and two of them rot on every add or retirement. Identity lines drift too, and nothing checks the three against each other. **The correct form is the pointer with its reason attached**, so the next author reads *why not* before re-adding the list.

**A parent default restated before the exception.** A child writes: "the parent's stack convention is `<A>` + `<B>` + `<C>`; this project's accepted exception is `<D>`." Everything before the word *exception* already exists one level up. The moment the parent is reworded this file is wrong, and nothing announces it. **The delta-correct form is the exception alone, plus a link.**

A third, subtler failure is the one a sweep exists to catch: **restatement within a single level.** A root file that states the both-files-or-neither rule at length in its cascade section *and* again in its upkeep section has two copies, in one file, that have already begun to diverge in wording. State each rule once per file. The one exemption is the upkeep clause's pair-required sentence, and it is exempted across *levels*, never twice within one file.

---

## Skills by level

Each level names the capabilities that fire on work *at that level* — never the full menu. A file that lists everything is read as a catalogue and skipped.

| Level | Names | Does not name |
|---|---|---|
| **Root** | Tree-wide capabilities: closing out a session, authoring or refreshing a pair, sweeping for drift, verifying a registry, judging repository durability. | Anything that fires only inside one child. |
| **Scope** | What governs work inside this scope, and the plain sentence "this scope has no local skills of its own" where that is true. | Root's list, restated. |
| **Grouping directory** | What fires when **adding, moving or retiring a child** — and nothing else. | The child's own build or stack capabilities. |
| **Project** | The stack, containerization, deploy and testing capabilities for *its* language and shape. | Every other language's. |

Three rules bind all four rows.

**Name a capability by purpose, with the skill name as an example, not as the identifier.** A plugin skill is not a file in this tree, so no link checker can detect a rename — and skills do get renamed. "Closing out a session that changed reality — e.g. `<name>`" survives a rename with a stale example; a bare list of names silently stops resolving and nothing says so. Write the example as *"e.g."*, never as *"currently"*: the word marks the name as state, and a state word in an instruction file is exactly what the audit's state-in-instructions finding exists to catch. The same applies to a path — a capability is named, never located.

**A local `skills/` directory is earned, not assumed.** A scope earns one when a procedure there is *repeated, ordered and trap-bearing* and nothing installed covers it. Anything that would be correct in a repository with nothing to do with this tree belongs **upstream in a plugin**, never forked down. A stale local skill is worse than a missing one, because it fires with authority.

**Never mirror a root skill downward, and never park a project-specific procedure at root** — the first drifts into N copies, the second taxes every session in the tree forever.

---

## Memory bodies — one worked archetype each

What follows is the **body** of a topic file. Frontmatter, the derived index, the `type:` vocabulary and the size caps are in [the context cascade](../../../references/context-cascade.md) and are not repeated. Examples use a placeholder tree with an `api`, a `web` and an `ingest` project.

Six devices recur across the archetypes and are worth naming once, because each closes a specific failure:

- **A provenance stamp** under the H1 — when this was verified, by what method, and explicitly *what was not re-checked this pass*. It is what makes a single `last_verified:` honest across a file whose contents are of mixed age, and it is the only thing that prevents a blanket restamp from destroying the freshness signal.
- **A seam declaration** where a topic has siblings — this file's remit, named, with the siblings linked and saying the reciprocal. A split that does not announce its seam from both sides produces two halves nobody can name.
- **A ranking declaration** where the body is a list of more than about four items — one line saying what the order means, so the next editor inserts in the right place instead of appending.
- **Bold group labels** instead of `##` subsections. Reach for `##` only where compartments have genuinely different verification dates or different owners; the `state` archetype usually does, `decision` and `gotcha` usually do not.
- **A deliberate-omission note** where a volatile figure was left out on purpose, naming the check that re-measures it. Without it the next agent helpfully fills it back in.
- **A closing structural cause** — *why this recurs* — where an item has recurred or is structurally expected to. It is what makes the next reader fix the mechanism rather than the instance.

Prefer an H1 that is a **claim** rather than a label: "Red — there is no safe blanket push here" beats "Push notes". The reader gets the finding before the body.

### `state` — compartmented by decay rate

```markdown
# Where this tree stands

*Verified <date> by inspection — commands re-run, manifests read. The build section below was measured <earlier date> and nothing has changed there since, so it still holds. Nothing else in this file was re-checked this pass.*

This is the tree-wide state file. Per-scope state lives in each scope's own `MEMORY.md`; per-project state in each project's. Read down, do not duplicate.

## Durability — inspected <date>, decays fast

- `api` — safe to push: ahead-only, no deletions in the range.
- `ingest` — **diverged fork.** A plain push is rejected and `--force` destroys the remote. Nobody pushes this without reading `memory/push-hazards.md` first.
- `web` — no remote at all; exists on one machine.

Per-repo ahead- and dirty-counts are deliberately absent: every pass moves them, including the pass that would record them. The context-audit capability (e.g. `fa-foundation-context-audit`) re-measures them in seconds and is the single source.

## What the tree is right now

- Three projects; `ingest` is dormant — last real commit <date>, no owner assigned.
- The registry lists all three. Membership is owned by `<manifest>`; the registry adds only the identity lines.

## Awaiting a human

- Whether `ingest` is retired or paused. Nothing proceeds on it until that is answered — see `questions.md`.
```

### `decision` — each entry carries its *why*

The highest-value archetype, because **a decision recorded without its rationale is reversed by the next agent, who sees only its cost.** Each entry also names what it superseded and on whose authority.

```markdown
# Decisions an agent must not relitigate

**Ordered by blast radius.** Newest within a group first.

- **`ingest` writes no schema migrations; the `api` owns the schema.** <date>. Two services migrating one database produced a race that took a week to diagnose. The cost of this decision is an extra cross-team step on every schema change, and that cost is *known and accepted* — it is not new information and does not reopen the decision. Supersedes the shared-migrations arrangement of <earlier date>.
- **The registry points at `<manifest>` rather than listing member names.** <date>. Two lists of the same membership disagreed within a day of being written. Consequence accepted: mechanical reconciliation is delegated to the package manager, so the audit's registry check is not run against member names here.
- **No hosted-only memory.** <date>. Anything durable recorded in a hosted store is recorded here in the same pass — hosted state does not cascade, is not visible in a diff, and is not what a fresh clone gets.

Full evidence for the second entry, including what was measured: [`evidence-registry.md`](evidence-registry.md) — the decision lives here, the evidence lives there, because the evidence is the argument.
```

### `gotcha` — traps that already cost real hours

Each entry carries **the tell** that identifies it, not just the fix. A trap you cannot recognise is a trap you hit again.

```markdown
# Traps that have already cost hours

**Ranked by how long each one cost.** The shared build procedure these sit under is the installed build capability; everything below is what that procedure does *not* cover because it is specific to this tree.

- **A green test run here proves nothing about `ingest`.** *Tell:* the suite finishes in under a second. There are no test projects wired for it, so the runner exits zero over an empty set. Check what actually ran before quoting a pass.
- **An upstream `200` with an error body.** *Tell:* the client succeeds and the parsed object is empty. Never trust the status alone — inspect the body. Cost: two days, twice, eighteen months apart.
- **A pinned dependency version here is usually load-bearing.** *Tell:* the pin has a comment. Read the comment before any upgrade sweep; two of these pins hold back a breaking major that the code compensates for elsewhere.

## Added <date> — <a new cluster>

<Append a dated cluster rather than opening a new file for every session. When the cluster grows large enough to split, it already has its seam.>
```

### `watch` — known drift and unpaid debts

Not `alert` (nothing breaks on the next action) and not `question` (nobody has to rule on it yet). An item leaves this file in one of three directions: fixed, escalated to `alert`, or handed to `question`.

```markdown
# Known drift and unpaid debts

**Grouped by consequence class, worst first.**

**Durability — work that exists in exactly one place**
- `web` has no remote. A disk failure loses it entirely. Cheapest unblock: create the remote and push once; nothing else about the project has to be decided first.
- An uncommitted body under `ingest/` has never been `git add`ed, so `git clean` deletes it with no reflog.

**Documentation against reality**
- `api`'s README describes an endpoint that was removed. Docs-to-code drift is blocking here: fix the doc in the same change as the code.

**Standing**
- No version control on `<path>`. Change history for that scope does not exist and cannot be reconstructed; mtimes are not a substitute, because on a synced volume they record sync events rather than edits.

**Why this recurs.** Nothing in the cascade obliges a reference document to be re-read when a project changes underneath it. The pattern is structural, not neglect — which is why the fix is a rule in the upkeep clause, not a resolution to try harder.
```

### `question` — every item names its owner

```markdown
# Open questions

**Roughly by consequence.** An item leaves this file into a `decision` topic, with its rationale, or it is deleted.

1. **Is `ingest` retired or paused?** Owner: `unassigned`. Blocks: whether its dependencies are maintained, and whether it appears in the registry as live. Cheapest resolution: one sentence from whoever last worked on it.
2. **Does the registry keep identity lines once `<manifest>` gains a description field?** Owner: `unassigned`. Not urgent; becomes urgent the first time the two disagree.

Where no human is named, the owner field is written literally as `unassigned` and the topic's `description:` — which is what the index shows — says so. **Never synthesize a name** — an invented owner reads as an assignment, and the real person never learns they have one.

*Distinct from `watch.md`: these need a ruling from a person. A watch item needs work, not a decision.*
```

### `kill-record` — what was retired, and where the capability went

Three mandatory contents: the **date**, **what triggered it**, and **where the reusable capability actually went**.

```markdown
# Retired — do not reason about these as live assets

- **`<identifier>` — retired <date>.** Trigger: <the specific thing that ended it, not "deprioritised">. What was learned: <the durable finding>. **The reusable capability — <what it was> — now lives at [`<surviving path or repository>`](<path>).**

**The rule that makes this archetype worth having:** "where it went" names a surviving document or repository, **never a staging directory**. A record citing a staged copy as the only copy is a defect at the moment it is written, and it must be fixed *then*, while the files still exist. Staging is a courtesy delay, not preservation — more than one genuinely reusable component has been lost exactly this way, with a correct-looking record pointing at a directory somebody later emptied.

And record the retirement **before** deleting anything. A record written afterwards is written from memory, which is where the trigger and the destination go missing.
```

### Two further archetypes worth a note

**`alert`** carries a read protocol the others do not: read always, at every level passed through, and indexed near the top. Where an alert needs a human call, it **enumerates the options with their costs and marks the recommended one — and does not take the decision.** That posture is the whole point of the type; an alert that resolves itself was a `decision`.

**`log`** is a session index and never a changelog: it records *what happened and where the durable output went*, never the blow-by-blow. Every rule a session produced lives in the `decision` and `gotcha` topics; current status lives in `state`. Older sections are replaced by a `— PRUNED` stub naming the commits that carry the detail. Without both of those rules it is the fastest-growing file in any store; with them it is the only place a session's provenance survives.

### Where a topic belongs, and when it splits

Canonical basenames for the recurring five — `state.md`, `decisions.md`, `gotchas.md`, `watch.md`, `questions.md`. A **concern-named** basename where the topic is one specific thing rather than a category. `<parent>-<facet>.md` for a split child.

**Split on the real seam, never at the size cap.** A topic split to satisfy the line or character cap produces two halves nobody can name, and each half's `description:` — its index hook — degrades into a label instead of a signal. If a file is long and has only one concern, it is one topic that needs pruning — deletion is the expected outcome of maintenance, not an exceptional one.

**Promote up; never duplicate sideways.** A fact that matters across a whole grouping belongs in that level's topic and is referenced from the children — not copy-pasted into each. The reciprocal rule is what makes an apparent duplication legitimate: the parent records the *pattern* and the decision, the child records the *instance* and the evidence, and each names the other. Anything else is two copies waiting to disagree.
