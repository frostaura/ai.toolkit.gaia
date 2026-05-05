# Claude Code custom agents — frontmatter reference

This is the canonical spec Gaia agent files follow. Source of truth for fields,
shapes, and examples is the [Claude Code plugin reference](https://code.claude.com/docs/en/plugins-reference).

## File location

Agent definitions live at `agents/<agent-name>.md` at the plugin root.
Claude Code auto-discovers them; the manifest at `.claude-plugin/plugin.json`
does not need to list them explicitly.

## YAML frontmatter

| Field             | Type                | Required | Purpose                                                                                                                          |
| ----------------- | ------------------- | -------- | -------------------------------------------------------------------------------------------------------------------------------- |
| `name`            | string              | yes      | Display name and routing key (e.g. `gaia-intake-orchestrator`).                                                                  |
| `description`     | string              | yes      | When to use this agent. Used for both auto-discovery and UI listings. Describe TRIGGER conditions and OUTPUT, not just identity. |
| `tools`           | array of strings    | no       | Allow-list of tool names. Omit to inherit all available tools. See *Tool names* below.                                           |
| `disallowedTools` | array of strings    | no       | Deny-list applied after `tools`.                                                                                                 |
| `model`           | string              | no       | One of `opus`, `sonnet`, `haiku`. If unset, inherits the parent's model.                                                         |
| `effort`          | string              | no       | `low` \| `medium` \| `high`. Reasoning depth budget.                                                                             |
| `maxTurns`        | number              | no       | Hard cap on tool-use turns for this agent.                                                                                       |
| `skills`          | array of strings    | no       | Skills this agent should preload.                                                                                                |
| `memory`          | object              | no       | Per-agent memory configuration.                                                                                                  |
| `background`      | boolean             | no       | Run as a background agent (does not block parent on long jobs).                                                                  |
| `isolation`       | string              | no       | Only `"worktree"` is supported — runs the agent in a temporary git worktree.                                                     |

The agent's behavior, role contract, and instructions live in the markdown body
below the frontmatter.

## Tool names

Built-in tool names are case-sensitive: `Read`, `Edit`, `Write`, `MultiEdit`,
`Bash`, `Grep`, `Glob`, `Task`, `WebFetch`, `WebSearch`, `NotebookEdit`,
`TodoWrite`, etc.

MCP tools follow `mcp__<server>__<tool>`, with double underscore separators.
Wildcards are supported on the tool segment, e.g. `mcp__gaia-remote__tasks_*`
enables every tool whose name starts with `tasks_` on the `gaia-remote` server.

## Gaia conventions

- One agent per Gaia role (`intake-orchestrator`, `solutions-architect`,
  `implementation-planner`, `software-engineer`, `tester`, `release-engineer`).
- Each agent's `tools` array is the **minimal** set required for its role
  (read-only roles do not get `Edit`/`Write`/`Bash`).
- `model: opus` for orchestrator, architect, planner; `model: sonnet` for
  engineer, tester, release engineer.
- The body must declare: mission, when-to-use, when-not-to-use, expected output
  shape, and handoff target.

## Example

```yaml
---
name: gaia-tester
description: |
  Use for formal validation, regression coverage, and pass/fail/blocked
  decisions after a branch is stable enough to evaluate. Output: explicit
  evidence + clear routing to the actual failure owner.
tools:
    - Read
    - Edit
    - Write
    - Bash
    - Grep
    - Glob
    - Task
    - mcp__gaia-remote__tasks_*
    - mcp__playwright__*
model: sonnet
effort: medium
---
```
