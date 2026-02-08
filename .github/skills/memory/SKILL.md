---
name: memory
description: A skill for managing agent memory and knowledge persistence. Agents should use memory tools by default for storing and recalling information. If memory tools are unavailable, fall back to docs/memories.json with strict JSON validity requirements.
---

<skill>
  <name>memory</name>
  <description>
  Platform-agnostic guidance for managing agent memory, knowledge persistence, and information recall. This skill defines the strategy for storing learnings, recalling context, and maintaining valid memory structures across sessions.
  </description>

  <memory-philosophy>
    <principle name="Default to MCP Tools">
      <value>Prefer memory tools with recall and remember capabilities as primary mechanism</value>
      <description>Memory tools provide structured, queryable memory with session and persistent scopes. Use these when available for optimal memory management.</description>
    </principle>
    <principle name="Fallback to JSON">
      <value>Use docs/memories.json only when memory tools are unavailable</value>
      <description>When memory tools cannot be accessed, fall back to the JSON file but maintain strict validity requirements.</description>
    </principle>
    <principle name="Selective Storage">
      <value>Store significant learnings, not every detail</value>
      <description>Memory should capture meaningful patterns, solutions, decisions, and gotchas—not routine operations or trivial details.</description>
    </principle>
    <principle name="Retrievability">
      <value>Structure memories for easy recall and search</value>
      <description>Use clear categories, descriptive keys, and searchable content to enable effective future retrieval.</description>
    </principle>
  </memory-philosophy>

  <primary-approach>
    <tool name="Memory Tools (Default)">
      <description>Memory system with recall and remember functions</description>
      <availability>Available through memory integration</availability>
      <recall>
        <function>recall</function>
        <description>Search memories using fuzzy matching across session and persistent storage</description>
        <usage>Call at task START to check for relevant context from past work</usage>
        <parameters>query string and optional maximum results</parameters>
        <example>Search for authentication bugs to find related past issues</example>
        <example>Search for API design patterns to find architectural decisions</example>
      </recall>
      <remember>
        <function>remember</function>
        <description>Store knowledge with categorization and duration control</description>
        <usage>Call for SIGNIFICANT learnings, patterns, solutions, decisions</usage>
        <parameters>category, key, value, and duration</parameters>
        <duration-session>SessionLength - temporary context for current session</duration-session>
        <duration-persistent>ProjectWide - permanent knowledge for entire project</duration-persistent>
      </remember>
      <categories>
        <category name="fix">
          <description>Bug solutions, workarounds, and resolution approaches</description>
          <duration>ProjectWide</duration>
          <example>Store CORS issue fix with proxy configuration details</example>
        </category>
        <category name="pattern">
          <description>Reusable approaches, architectural patterns, design decisions</description>
          <duration>ProjectWide</duration>
          <example>Store Result pattern for API error handling with type parameters</example>
        </category>
        <category name="config">
          <description>Working configurations, environment setups, tooling settings</description>
          <duration>ProjectWide</duration>
          <example>Store test coverage configuration with threshold values</example>
        </category>
        <category name="decision">
          <description>Architectural choices, technology selections, trade-off decisions</description>
          <duration>ProjectWide</duration>
          <example>Store state management library choice with scalability rationale</example>
        </category>
        <category name="warning">
          <description>Gotchas, caveats, things to avoid, known limitations</description>
          <duration>ProjectWide</duration>
          <example>Store database JSON query indexing limitations</example>
        </category>
        <category name="context">
          <description>Current task state, temporary notes, WIP context</description>
          <duration>SessionLength</duration>
          <example>Store current feature implementation status for session continuity</example>
        </category>
      </categories>
      <best-practices>
        <practice name="Recall First">
          <description>Always recall at the start of a task to leverage past learnings</description>
        </practice>
        <practice name="Remember Significant">
          <description>Store significant learnings only—not every sub-task or routine operation</description>
        </practice>
        <practice name="Clear Keys">
          <description>Use descriptive keys that will be searchable and meaningful later</description>
        </practice>
        <practice name="Context in Value">
          <description>Include enough context in the value so it's useful without referring back to code</description>
        </practice>
        <practice name="Right Duration">
          <description>Use SessionLength for temporary context, ProjectWide for permanent knowledge</description>
        </practice>
      </best-practices>
    </tool>
  </primary-approach>

  <fallback-approach>
    <file name="docs/memories.json">
      <description>JSON file-based memory storage when memory tools are unavailable</description>
      <location>docs/memories.json in repository root</location>
      <requirement name="Valid JSON">
        <value>MUST always maintain valid, parseable JSON</value>
        <critical>Invalid JSON breaks memory system—validate before saving</critical>
      </requirement>
      <requirement name="Atomic Edits">
        <value>Read entire file, modify in memory, write complete valid JSON</value>
        <critical>Never write partial JSON or leave file in invalid state</critical>
      </requirement>
      <requirement name="Backup on Edit">
        <value>When editing directly, validate JSON structure first</value>
        <recommendation>Use JSON schema validation before writing</recommendation>
      </requirement>
      <structure>
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
      "metadata": {
        "agent": "planner|developer|tester|operator|analyst",
        "tags": ["tag1", "tag2"]
      }
    }
  ],
  "version": "1.0",
  "lastUpdated": "ISO 8601 date"
}
        </schema>
      </structure>
      <operations>
        <operation name="Read">
          <description>Parse JSON file to access all memories</description>
          <validation>Verify JSON is valid before processing</validation>
        </operation>
        <operation name="Search">
          <description>Filter memories by category, key, value text, or tags</description>
          <approach>Simple text matching or regex on key/value fields</approach>
        </operation>
        <operation name="Add">
          <description>Append new memory to memories array</description>
          <process>Read → Parse → Add object → Validate → Write</process>
        </operation>
        <operation name="Update">
          <description>Modify existing memory by ID</description>
          <process>Read → Parse → Find by ID → Update → Validate → Write</process>
        </operation>
        <operation name="Clean">
          <description>Remove session-scoped memories when session ends</description>
          <process>Read → Parse → Filter out session memories → Validate → Write</process>
        </operation>
      </operations>
      <validation-steps>
        <step number="1">Read entire file contents</step>
        <step number="2">Parse JSON to validate structure (try/catch)</step>
        <step number="3">Make modifications in memory representation</step>
        <step number="4">Serialize back to JSON with proper formatting</step>
        <step number="5">Validate serialized JSON parses correctly</step>
        <step number="6">Write to file only after validation passes</step>
      </validation-steps>
      <error-handling>
        <error name="Invalid JSON">
          <description>File contains unparseable JSON</description>
          <recovery>Report error, do not write, attempt to load last valid backup</recovery>
        </error>
        <error name="Schema Mismatch">
          <description>JSON structure doesn't match expected schema</description>
          <recovery>Add missing fields with defaults, preserve unknown fields</recovery>
        </error>
        <error name="Write Failure">
          <description>Unable to write file</description>
          <recovery>Report error, keep memory changes in-memory until resolved</recovery>
        </error>
      </error-handling>
    </file>
  </fallback-approach>

  <usage-protocol>
    <when-to-recall>
      <timing name="Task Start">
        <description>Beginning new feature, bug fix, or complex task</description>
        <query>Search for related features, similar bugs, relevant patterns</query>
      </timing>
      <timing name="Problem Encountered">
        <description>Hitting an error or issue that may have been seen before</description>
        <query>Search for error messages, problem symptoms, solution patterns</query>
      </timing>
      <timing name="Design Decision">
        <description>Making architectural or technology choices</description>
        <query>Search for past decisions, patterns, architectural guidelines</query>
      </timing>
      <timing name="Configuration Needed">
        <description>Setting up tools, environments, or configurations</description>
        <query>Search for working configurations, setup steps, known issues</query>
      </timing>
    </when-to-recall>
    <when-to-remember>
      <timing name="Bug Resolution">
        <description>Successfully fixed a bug or resolved an issue</description>
        <category>fix</category>
        <what-to-store>Problem description, root cause, solution approach</what-to-store>
      </timing>
      <timing name="Pattern Discovery">
        <description>Identified a reusable approach or design pattern</description>
        <category>pattern</category>
        <what-to-store>Pattern name, use case, implementation notes</what-to-store>
      </timing>
      <timing name="Configuration Success">
        <description>Found working configuration after experimentation</description>
        <category>config</category>
        <what-to-store>Configuration details, purpose, caveats</what-to-store>
      </timing>
      <timing name="Architectural Decision">
        <description>Made significant technology or design choice</description>
        <category>decision</category>
        <what-to-store>Decision, rationale, alternatives considered, trade-offs</what-to-store>
      </timing>
      <timing name="Gotcha Discovered">
        <description>Encountered unexpected behavior, limitation, or caveat</description>
        <category>warning</category>
        <what-to-store>What to avoid, why it fails, proper approach</what-to-store>
      </timing>
    </when-to-remember>
    <when-not-to-remember>
      <scenario>Routine operations with no learning value</scenario>
      <scenario>Standard code that follows existing patterns</scenario>
      <scenario>Trivial changes or formatting fixes</scenario>
      <scenario>Temporary debugging or exploratory code</scenario>
      <scenario>Information already well-documented in code comments or docs</scenario>
    </when-not-to-remember>
  </usage-protocol>

  <best-practices>
    <category name="Memory Hygiene">
      <practice name="Clean Keys">
        <description>Use descriptive, searchable keys with consistent naming conventions</description>
        <good>authentication_refresh_token_flow</good>
        <bad>temp123, fix_v2, todo</bad>
      </practice>
      <practice name="Contextual Values">
        <description>Include enough detail so future recall is actionable without code review</description>
        <good>"JWT refresh tokens stored in httpOnly cookies. Access token expires 15min, refresh 7 days. POST /auth/refresh to renew."</good>
        <bad>"Fixed the auth thing"</bad>
      </practice>
      <practice name="Appropriate Scope">
        <description>Choose duration based on information lifespan</description>
        <session>Current WIP, temporary notes, task-specific context</session>
        <persistent>Solutions, patterns, decisions, warnings, configurations</persistent>
      </practice>
      <practice name="Regular Review">
        <description>Periodically review persistent memories to remove outdated entries</description>
      </practice>
    </category>
    <category name="Search Strategy">
      <practice name="Broad Then Narrow">
        <description>Start with broad queries, then narrow based on results</description>
        <example>Search for auth, then narrow to auth refresh token</example>
      </practice>
      <practice name="Multiple Queries">
        <description>Try different phrasings or related terms if first search yields nothing</description>
        <example>Try database migration, then schema change, then specific tool names</example>
      </practice>
      <practice name="Category Filters">
        <description>When using JSON fallback, filter by category for focused results</description>
      </practice>
    </category>
    <category name="JSON Integrity">
      <practice name="Always Validate">
        <description>Never write to docs/memories.json without validating JSON structure</description>
        <tool>Use JSON.parse() and try/catch before writing</tool>
      </practice>
      <practice name="Atomic Operations">
        <description>Complete read-modify-write cycle in one operation to avoid partial updates</description>
      </practice>
      <practice name="Pretty Print">
        <description>Format JSON with proper indentation for human readability</description>
        <formatting>JSON.stringify(obj, null, 2)</formatting>
      </practice>
      <practice name="Preserve Unknown Fields">
        <description>If schema evolves, preserve fields you don't recognize</description>
      </practice>
    </category>
  </best-practices>

  <integration-with-agents>
    <agent name="All Agents">
      <guideline>Use recall at task start to check for relevant past work</guideline>
      <guideline>Use remember for significant learnings specific to their domain</guideline>
      <guideline>Prefer memory tools; fall back to JSON only when unavailable</guideline>
      <guideline>Don't over-store—focus on high-value, reusable knowledge</guideline>
    </agent>
    <agent name="Planner">
      <focus>Store architectural decisions, design patterns, planning strategies</focus>
      <categories>decision, pattern, warning</categories>
    </agent>
    <agent name="Developer">
      <focus>Store bug fixes, implementation patterns, configuration solutions</focus>
      <categories>fix, pattern, config, warning</categories>
    </agent>
    <agent name="Tester">
      <focus>Store test patterns, security issues, coverage strategies</focus>
      <categories>fix, pattern, warning</categories>
    </agent>
    <agent name="Operator">
      <focus>Store deployment solutions, infrastructure patterns, operational fixes</focus>
      <categories>fix, config, warning</categories>
    </agent>
    <agent name="Analyst">
      <focus>Store analysis patterns, investigation techniques, codebase insights</focus>
      <categories>pattern, context, warning</categories>
    </agent>
  </integration-with-agents>

  <common-pitfalls>
    <pitfall name="Over-Storing">
      <description>Storing every minor detail clutters memory and reduces signal-to-noise ratio</description>
      <solution>Only store significant learnings that will be valuable in future tasks</solution>
    </pitfall>
    <pitfall name="Vague Keys">
      <description>Generic keys like "bug_fix" or "update" are not searchable or meaningful</description>
      <solution>Use specific, descriptive keys that capture the essence of the memory</solution>
    </pitfall>
    <pitfall name="Insufficient Context">
      <description>Storing minimal information requires referring back to code to understand</description>
      <solution>Include enough context so the memory is actionable on its own</solution>
    </pitfall>
    <pitfall name="Invalid JSON">
      <description>Writing malformed JSON breaks the entire memory system</description>
      <solution>Always validate JSON structure before writing to file</solution>
    </pitfall>
    <pitfall name="Ignoring Recall">
      <description>Starting tasks without checking for past learnings wastes time re-solving problems</description>
      <solution>Make recall a habit at the beginning of every task</solution>
    </pitfall>
    <pitfall name="Wrong Duration">
      <description>Storing temporary context as ProjectWide or vice versa</description>
      <solution>SessionLength for WIP/temporary, ProjectWide for permanent knowledge</solution>
    </pitfall>
  </common-pitfalls>

  <migration-guidance>
    <scenario name="Memory Tools Available to File Fallback">
      <description>When memory tools become unavailable mid-session</description>
      <action>Continue with docs/memories.json, maintain same categories and structure</action>
    </scenario>
    <scenario name="File Fallback to Memory Tools Available">
      <description>When memory tools become available after using file fallback</description>
      <action>Optionally migrate recent memories from JSON to memory tools for better search</action>
    </scenario>
    <scenario name="Cross-Session Continuity">
      <description>Starting new session with memories from previous session</description>
      <action>SessionLength memories won't persist; only ProjectWide memories carry over</action>
    </scenario>
  </migration-guidance>

  <success-metrics>
    <metric>Agents consistently recall relevant context at task start</metric>
    <metric>Significant learnings captured with clear, searchable keys</metric>
    <metric>docs/memories.json always maintains valid JSON structure</metric>
    <metric>No duplicate problem-solving due to forgotten past solutions</metric>
    <metric>Memory queries return relevant results efficiently</metric>
    <metric>Minimal noise—stored memories are high-value and reusable</metric>
    <metric>Proper duration scoping—temporary context vs permanent knowledge</metric>
  </success-metrics>
</skill>
