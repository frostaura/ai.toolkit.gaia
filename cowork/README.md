# Gaia in Claude CoWork

Claude CoWork is Anthropic's separate desktop product for knowledge work. It
does **not** load Claude Code plugins, but it speaks MCP. This directory is
Gaia's "starter pack" for CoWork: an MCP connector config plus a project
instructions document that gives a CoWork project the same role contract,
workflow, and proof obligations that the Claude Code surface enforces via
agents/skills/commands/hooks.

## What's in here

| File              | Purpose                                                                                   |
| ----------------- | ----------------------------------------------------------------------------------------- |
| `connector.json`  | MCP server config to register `gaia-remote` (and optionally `gaia-local`) in CoWork.      |
| `instructions.md` | Project instructions you paste into CoWork's "Instructions" / system-prompt field.        |
| `build.sh`        | Regenerates `instructions.md` from `AGENTS.md` + `agents/*.md` + `skills/*/SKILL.md` so the CoWork and Claude Code surfaces don't drift. |

## Setup (one-time, ~3 minutes)

1. **Open Claude CoWork** and create or open a project.
2. **Add the MCP connector**:
    - In CoWork's connector / MCP settings, add a new HTTP MCP server.
    - URL: `https://gaia.frostaura.net/mcp`
    - Or paste the snippet from `cowork/connector.json` if your build of
      CoWork accepts JSON config directly.
    - You should now see Gaia's `tasks_*`, `memory_*`, and `evolve_*` tools
      available.
3. **Paste the project instructions**:
    - Open `cowork/instructions.md` from this repo.
    - Copy the entire file contents (everything below the first `---`).
    - Paste it into CoWork's project Instructions / System Prompt field.
4. **Verify** by typing: *"Run a Gaia repo survey on this project."* You
   should see the assistant adopt the `[role: intake-orchestrator]` header
   and call `memory_recall` + `evolve_list` before producing the survey.

## Keeping CoWork in sync

When `AGENTS.md`, `agents/*.md`, or `skills/*/SKILL.md` change in this repo,
re-generate the CoWork instructions file:

```bash
./cowork/build.sh
```

Then re-paste `cowork/instructions.md` into your CoWork project. The script
emits a stable, deterministic output so re-paste is cheap.

## What CoWork loses vs Claude Code

CoWork is missing some of Gaia's Claude Code surface — these aren't
fixable from the plugin side, just things to know:

- **No slash commands.** `/gaia-init`, `/gaia-intake`, etc. don't exist in
  CoWork. Instead, ask in plain English: *"Run /gaia-init"* — the
  instructions file teaches the model to interpret these as role
  invocations.
- **No hooks.** The `SessionStart` / `PreToolUse` / etc. hooks that
  enforce policy in Claude Code do not run in CoWork. Policy enforcement
  is advisory only.
- **No subagent isolation.** Claude Code uses the `Task` tool to spawn
  fresh-context role agents. CoWork keeps a single conversation, so role
  switching is announced via the `[role: ...]` header instead.

The MCP-backed task / memory / evolution surface is identical between the
two products — that's where the real anchoring happens.
