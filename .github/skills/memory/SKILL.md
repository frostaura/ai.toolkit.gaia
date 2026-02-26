---
name: memory
description: A skill for managing agent memory and knowledge persistence. Agents should use memory tools by default for storing and recalling information. If memory tools are unavailable, fall back to docs/memories.json with strict JSON validity requirements.
---

<skill>
  <name>memory</name>
  <description>
  Guidance for managing agent memory and knowledge persistence. Prefer MCP memory tools (recall/remember); fall back to docs/memories.json only when tools are unavailable.
  </description>

  <principles>
    <principle>**Default to MCP tools** — structured, queryable, session/persistent scopes.</principle>
    <principle>**Fallback to JSON** — docs/memories.json only when tools unavailable. Always validate JSON before writing.</principle>
    <principle>**Selective storage** — capture meaningful patterns, decisions, and gotchas, not routine operations.</principle>
    <principle>**Retrievability** — use clear categories, descriptive keys, and searchable content.</principle>
  </principles>

  <mcp-tools>
    <recall>
      <usage>Call at task START to check for relevant context from past work.</usage>
      <tip>Start broad, then narrow. Try different phrasings if first query yields nothing.</tip>
    </recall>
    <remember>
      <usage>Call for SIGNIFICANT learnings: bug fixes, patterns, decisions, configurations, warnings.</usage>
      <duration-session>SessionLength — temporary context for current session only.</duration-session>
      <duration-persistent>ProjectWide — permanent knowledge across sessions.</duration-persistent>
    </remember>
  </mcp-tools>

  <categories>
    <category name="fix">Bug solutions and workarounds (ProjectWide)</category>
    <category name="pattern">Reusable approaches and architectural patterns (ProjectWide)</category>
    <category name="config">Working configurations and environment setups (ProjectWide)</category>
    <category name="decision">Architecture choices with rationale (ProjectWide)</category>
    <category name="warning">Gotchas, caveats, and known limitations (ProjectWide)</category>
    <category name="context">Current task state and WIP notes (SessionLength)</category>
  </categories>

  <when-to-recall>
    <timing>Task start — search for related features, similar bugs, relevant patterns</timing>
    <timing>Problem encountered — search for error messages, symptoms, past solutions</timing>
    <timing>Design decision — search for past decisions and architectural guidelines</timing>
    <timing>Configuration needed — search for working configs and known issues</timing>
  </when-to-recall>

  <when-to-remember>
    <timing>Bug fixed — store problem, root cause, solution approach</timing>
    <timing>Pattern discovered — store pattern name, use case, implementation notes</timing>
    <timing>Config found — store config details, purpose, caveats</timing>
    <timing>Decision made — store decision, rationale, alternatives, trade-offs</timing>
    <timing>Gotcha found — store what to avoid, why it fails, proper approach</timing>
  </when-to-remember>

  <when-not-to-remember>
    <scenario>Routine operations with no learning value</scenario>
    <scenario>Standard code following existing patterns</scenario>
    <scenario>Trivial changes or formatting fixes</scenario>
    <scenario>Information already well-documented in code or docs</scenario>
  </when-not-to-remember>

  <best-practices>
    <practice>**Clean keys** — use descriptive, searchable keys (e.g., `authentication_refresh_token_flow`, not `fix_v2`).</practice>
    <practice>**Contextual values** — include enough detail to be actionable without referring back to code.</practice>
    <practice>**Right duration** — SessionLength for WIP/temporary; ProjectWide for permanent knowledge.</practice>
    <practice>**Recall first** — always check for existing knowledge before re-solving problems.</practice>
  </best-practices>

  <json-fallback>
    <location>docs/memories.json</location>
    <rules>
      <rule>MUST maintain valid, parseable JSON at all times.</rule>
      <rule>Read entire file → modify in memory → validate → write complete JSON.</rule>
      <rule>Format with proper indentation (JSON.stringify(obj, null, 2)).</rule>
      <rule>Preserve unknown fields if schema evolves.</rule>
    </rules>
    <schema>
{
  "memories": [
    {
      "id": "unique-id",
      "category": "fix|pattern|config|decision|warning|context",
      "key": "descriptive_key",
      "value": "detailed description with context",
      "timestamp": "ISO 8601 date",
      "duration": "session|persistent",
      "metadata": { "agent": "agent-name", "tags": ["tag1"] }
    }
  ],
  "version": "1.0",
  "lastUpdated": "ISO 8601 date"
}
    </schema>
  </json-fallback>

  <integration-with-agents>
    <agent name="All">Recall at task start; remember significant learnings; prefer MCP tools over JSON.</agent>
    <agent name="Architect">Focus: decisions, patterns, warnings.</agent>
    <agent name="Developer">Focus: fixes, patterns, configs, warnings.</agent>
    <agent name="Tester">Focus: test patterns, security issues, warnings.</agent>
    <agent name="Analyst">Focus: analysis patterns, investigation insights, warnings.</agent>
  </integration-with-agents>
</skill>
