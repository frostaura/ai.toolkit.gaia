---
name: ai-toolkit-gaia-decisions
description: "The server's task/memory/evolve tools were removed rather than deprecated, so the completion contract is now enforced by nothing but the skills; plugins are server-independent; fa.integrations folded in whole, keeping REST alongside MCP."
type: decision
last_verified: 2026-09-11
---

# Live decisions — product shape

- **The `tasks_*`, `memory_*` and `evolve_*` tools were removed, not deprecated** (founder,
  2026-08-21): use native todos, the repository's own `MEMORY.md` + `memory/` store, and log
  improvements as memories. **Why it is a better trade than it looks:** a todo list and a
  repo's memory store version with the code, are reviewed in the same pull request, need no
  account and survive a machine change with nothing to sync. The hosted store did none of
  that, and its records for this very project had gone stale enough to describe a repository
  generation that no longer existed.
- **The accepted cost: the completion contract is enforced by nothing.** The server used to
  refuse a completion carrying unresolved blockers or missing proof. It cannot now. This was
  raised before the work started and reaffirmed. The compensating move is that the policy
  files say the contract is unenforced and must therefore be applied *more* literally — do
  not soften it to unblock a stuck workflow.
- **No persistence at all, anywhere in the service.** Supersedes the older flat-JSON-over-EF
  decision, which is moot. The deviation from the mandated stack is now larger, not smaller,
  and is stated plainly in `CLAUDE.md` rather than hidden.
- **`foundation` declared no MCP server** (v13.0.0): wiring the remaining Woolworths tools
  into every `foundation` install would push a grocery integration onto people who installed
  a context-layer plugin. If the server is ever wired again it should be opt-in and its own
  plugin. **An unpushed commit reverses this with no recorded why**, and it now sits inside
  the unpushed v14 range ([`state.md`](state.md)) — treat the reversal as unratified until the
  founder says otherwise, and do not let a v14 push ratify it by accident.
- **`fa.integrations` was migrated in whole and killed, not kept alongside** — one hosted
  service, one deploy chain, one place to add the next integration. The `IIntegration` seam
  moved with it, so adding a provider is still a class plus three registration lines.
- **Four projects folded into one.** The seam that earns its keep is the `IIntegration`
  *type*, not an assembly boundary; a ~900-LOC service does not need four csproj files. The
  provider-neutral half still depends on nothing Woolworths-specific.
- **Both surfaces kept — REST as well as MCP.** Dropping `/api` would have silently killed
  the Siri Shortcut use case, which cannot speak MCP. The endpoints call the same services
  the tools do, so the cost is near zero and the capability survived the move intact.
- **Credentials over stdio are derived from the header names**
  (`X-Woolworths-Password` → `GAIA_WOOLWORTHS_PASSWORD`) so the two cannot drift. Gaia serves
  stdio and `fa.integrations` never did; without this three tools would be dead on that
  transport. The environment is consulted **only** when the transport supplied no headers at
  all — an HTTP caller who omits one is told so, never silently served the host's environment.
- **Agent definitions are Claude-Code-canonical, one tree** (founder, 2026-08-21). Copilot
  frontmatter keys dropped; `tools:` omitted by default so an agent inherits everything
  including MCP, because install-context MCP prefixes are too fragile to hardcode;
  `disallowedTools:` expresses deliberate restriction. **Why:** the old Copilot-schema agents
  could not launch in Claude Code at all, and dual trees would double the duplication defect.
- **Parallel-first is product content, not style.** Plans must declare parallel branches and
  true dependency edges — serializing independent work is a defect; coordinators fan out
  concurrently; concurrent edits take disjoint scopes or worktrees; and the completion
  contract is never softened to make parallelism easier.

Distribution, versioning and catalog decisions: [`decisions-distribution.md`](decisions-distribution.md).
