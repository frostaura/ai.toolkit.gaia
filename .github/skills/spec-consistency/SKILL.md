---
name: spec-consistency
description: A skill for keeping code and specs aligned in a spec-driven workflow.
---

<skill>
  <name>spec-consistency</name>
  <description>
  Ensures bidirectional alignment between design documents and implementation. Detects drift early, routes corrections to appropriate owners, and maintains spec-driven design integrity throughout the SDLC.
  </description>

  <core-principle>
    <statement>Code reflects spec; spec reflects code. Neither drifts without the other updating.</statement>
    <enforcement>Every PR affecting behavior must verify spec alignment. Every spec change must precede or accompany code change.</enforcement>
  </core-principle>

  <when-to-check>
    <trigger>Before implementing any feature or fix (verify spec exists and is current).</trigger>
    <trigger>After completing implementation (verify code matches spec).</trigger>
    <trigger>During code review (reviewer checks spec alignment).</trigger>
    <trigger>When requirements change (update spec first, then code).</trigger>
    <trigger>When refactoring (ensure behavior described in spec is preserved).</trigger>
  </when-to-check>

  <alignment-checklist>
    <category name="Behavior Alignment">
      <check>Does the implementation match all behaviors described in specs?</check>
      <check>Are there implemented behaviors not captured in specs?</check>
      <check>Are there specified behaviors not yet implemented?</check>
      <check>Do edge cases in code match edge cases in spec?</check>
    </category>
    <category name="API Alignment">
      <check>Do endpoint paths, methods, and parameters match api.md?</check>
      <check>Do request/response schemas match documented contracts?</check>
      <check>Are status codes and error responses as documented?</check>
      <check>Is authentication/authorization as specified?</check>
    </category>
    <category name="Data Alignment">
      <check>Do entity structures match data.md schemas?</check>
      <check>Are relationships and constraints as documented?</check>
      <check>Do migrations reflect documented schema evolution?</check>
    </category>
    <category name="UI Alignment">
      <check>Do components match frontend.md patterns?</check>
      <check>Are user flows as documented in use-cases.md?</check>
      <check>Are interactive states as specified?</check>
    </category>
    <category name="Test Alignment">
      <check>Do test scenarios cover all use cases in specs?</check>
      <check>Are test descriptions traceable to requirements?</check>
      <check>Do regression tests match documented behaviors?</check>
    </category>
  </alignment-checklist>

  <drift-detection-protocol>
    <step number="1">Identify the change scope (which components, which behaviors).</step>
    <step number="2">Locate relevant spec documents (design.md, api.md, data.md, use-cases.md).</step>
    <step number="3">Compare implementation against spec using checklist above.</step>
    <step number="4">Document mismatches with specific file:line references.</step>
    <step number="5">Classify each mismatch: Code wrong | Spec outdated | Both need update.</step>
    <step number="6">Route corrections: Code issues → developer; Spec issues → architect.</step>
    <step number="7">Verify corrections before marking complete.</step>
  </drift-detection-protocol>

  <mismatch-classification>
    <type name="Code Wrong">
      <description>Implementation doesn't match valid, current spec.</description>
      <action>Developer fixes code to match spec.</action>
      <example>API returns 200 but spec says 201 on create.</example>
    </type>
    <type name="Spec Outdated">
      <description>Spec doesn't reflect intentional, approved code changes.</description>
      <action>Architect updates spec to match code.</action>
      <example>New endpoint added but not documented in api.md.</example>
    </type>
    <type name="Both Need Update">
      <description>Requirements changed; neither code nor spec is correct yet.</description>
      <action>Architect updates spec first; developer updates code to match.</action>
      <example>Business rule changed; old behavior in code, old rule in spec.</example>
    </type>
    <type name="Intentional Deviation">
      <description>Code differs from spec for valid reason (tech constraint, interim state).</description>
      <action>Document deviation in spec with rationale; create ADR if architectural.</action>
      <example>Spec says async but sync implemented due to deadline; noted as tech debt.</example>
    </type>
  </mismatch-classification>

  <output-format>
    <section name="Scope">Components and behaviors checked.</section>
    <section name="Alignment Status">Pass / Fail per checklist category.</section>
    <section name="Mismatches Found">Table: Location | Type | Description | Action Owner.</section>
    <section name="Spec Updates Needed">List for architect with specific changes.</section>
    <section name="Code Fixes Needed">List for developer with specific changes.</section>
    <section name="Verification Plan">How to confirm corrections are complete.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Implementing without checking spec first ("code and document later").</anti-pattern>
    <anti-pattern>Updating code without updating spec in same PR/session.</anti-pattern>
    <anti-pattern>Spec changes without corresponding code implementation.</anti-pattern>
    <anti-pattern>Vague specs that can't be verified against implementation.</anti-pattern>
    <anti-pattern>Ignoring "small" mismatches that accumulate into major drift.</anti-pattern>
    <anti-pattern>Developer updating docs directly instead of routing to architect.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-agent name="architect">Receives spec update requests; only architect modifies docs/.</with-agent>
    <with-agent name="developer">Fixes code mismatches; flags spec issues for architect.</with-agent>
    <with-agent name="tester">Uses spec for test case validation; reports mismatches found in testing.</with-agent>
    <with-skill name="documentation">Follows documentation standards for spec files.</with-skill>
    <with-skill name="architecture-decision-records">Major deviations may require ADR.</with-skill>
  </integration>

  <references>
    <reference>docs/README.md for documentation structure.</reference>
    <reference>docs/design.md for system behavior specs.</reference>
    <reference>docs/api.md for API contracts.</reference>
    <reference>docs/data.md for data model specs.</reference>
    <reference>docs/use-cases.md for user flow specs.</reference>
    <reference>docs/frontend.md for UI component specs.</reference>
  </references>
</skill>
