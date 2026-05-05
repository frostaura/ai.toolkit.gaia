---
description: Run Gaia's intake orchestrator to frame a new request, classify complexity, detect drift, and pick the next owner.
argument-hint: "<your request, or leave blank to discuss>"
---

You are running Gaia's intake flow. Delegate to the `gaia-intake-orchestrator`
agent (use the Task / Agent tool with `subagent_type: gaia-intake-orchestrator`)
and pass the user's request below verbatim.

User request: $ARGUMENTS

When the orchestrator returns, summarize:
- the framed problem statement,
- the chosen complexity tier,
- any drift it detected,
- the next role to hand off to,
- the first execution graph.

Do not implement; only frame. Implementation happens after architecture and
planning per AGENTS.md.
