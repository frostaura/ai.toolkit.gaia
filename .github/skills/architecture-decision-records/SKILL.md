---
name: architecture-decision-records
description: A skill for capturing key architectural decisions and their tradeoffs in ADRs.
---

<skill>
  <name>architecture-decision-records</name>
  <description>
  Captures significant architectural decisions with context, tradeoffs, and consequences. Prevents design drift, repeated debates, and institutional memory loss. Only the architect agent creates ADRs in docs/.
  </description>

  <when-to-create-adr>
    <trigger>Technology selection (framework, database, library) affecting multiple components.</trigger>
    <trigger>Architectural pattern choice (microservices vs monolith, event-driven, CQRS).</trigger>
    <trigger>Data model changes affecting storage, relationships, or migration strategy.</trigger>
    <trigger>Security architecture decisions (auth flow, encryption, access control model).</trigger>
    <trigger>API design choices (REST vs GraphQL, versioning strategy, contract structure).</trigger>
    <trigger>Integration patterns (sync vs async, queue selection, third-party API approach).</trigger>
    <trigger>Reversing or significantly modifying a previous ADR.</trigger>
    <skip>Routine implementation details, coding style choices, or decisions already covered by default-web-stack.</skip>
  </when-to-create-adr>

  <adr-structure>
    <section name="Title">
      <format>ADR-[NNN]: [Short Decision Title]</format>
      <example>ADR-001: Use PostgreSQL as Primary Database</example>
    </section>
    <section name="Status">
      <values>Proposed | Accepted | Deprecated | Superseded by ADR-XXX</values>
      <guidance>New ADRs start as Proposed; architect marks Accepted after review.</guidance>
    </section>
    <section name="Context">
      <purpose>Why is this decision needed? What problem or opportunity prompted it?</purpose>
      <include>Business drivers, technical constraints, team capabilities, timeline pressures.</include>
      <length>3-5 sentences providing sufficient background.</length>
    </section>
    <section name="Decision">
      <purpose>What is the decision and why was it chosen?</purpose>
      <include>The choice made, key reasons, how it addresses the context.</include>
      <guidance>Be specific: "We will use X because Y" not "We decided on something."</guidance>
    </section>
    <section name="Consequences">
      <purpose>What are the results of this decision?</purpose>
      <positive>Benefits, capabilities enabled, risks mitigated.</positive>
      <negative>Tradeoffs accepted, limitations introduced, costs incurred.</negative>
      <neutral>Changes to workflow, skills needed, monitoring requirements.</neutral>
    </section>
    <section name="Alternatives Considered">
      <purpose>What other options were evaluated?</purpose>
      <include>Each alternative with brief pros/cons and why it was rejected.</include>
      <minimum>At least 2 alternatives for significant decisions.</minimum>
    </section>
    <section name="References">
      <purpose>Links to supporting evidence, docs, or related ADRs.</purpose>
      <include>Research sources, related design docs, superseded ADRs.</include>
    </section>
  </adr-structure>

  <file-conventions>
    <location>docs/adr/ directory (architect creates if missing).</location>
    <naming>adr-NNN-kebab-case-title.md (e.g., adr-001-use-postgresql.md).</naming>
    <numbering>Sequential, never reused. Deprecated ADRs keep their numbers.</numbering>
    <index>Maintain docs/adr/README.md listing all ADRs with status and one-line summary.</index>
  </file-conventions>

  <workflow>
    <step number="1">Agent identifies decision needing ADR (using triggers above).</step>
    <step number="2">Agent drafts ADR content following structure; routes to architect.</step>
    <step number="3">Architect reviews, refines, and creates ADR file in docs/adr/.</step>
    <step number="4">Architect updates docs/adr/README.md index.</step>
    <step number="5">Architect marks status as Accepted (or requests changes).</step>
    <step number="6">Implementation proceeds; ADR linked from relevant code/docs.</step>
    <step number="7">If decision changes later, create new ADR that supersedes; update old status.</step>
  </workflow>

  <quality-criteria>
    <criterion>Context explains why, not just what.</criterion>
    <criterion>Decision is specific and actionable.</criterion>
    <criterion>Consequences include both pros and cons (no one-sided ADRs).</criterion>
    <criterion>Alternatives show due diligence in evaluation.</criterion>
    <criterion>Length: 50-100 lines; dense but readable.</criterion>
    <criterion>No TODOs or placeholders—complete at creation.</criterion>
  </quality-criteria>

  <anti-patterns>
    <anti-pattern>Writing ADRs after the fact to justify decisions already made without analysis.</anti-pattern>
    <anti-pattern>Creating ADRs for trivial decisions that don't affect architecture.</anti-pattern>
    <anti-pattern>Vague context: "We needed a database" without explaining constraints.</anti-pattern>
    <anti-pattern>Missing alternatives: implies no evaluation was done.</anti-pattern>
    <anti-pattern>One-sided consequences: only listing benefits, hiding tradeoffs.</anti-pattern>
    <anti-pattern>Orphaned ADRs: not linked from code or design docs that depend on them.</anti-pattern>
  </anti-patterns>

  <lifecycle-management>
    <review>Revisit ADRs when underlying assumptions change (new requirements, tech shifts).</review>
    <deprecation>Mark as Deprecated when decision no longer applies (project pivot, tech sunset).</deprecation>
    <supersession>Create new ADR with "Supersedes ADR-XXX"; update old ADR status.</supersession>
    <cleanup>Never delete ADRs—they're historical record. Mark status, don't remove.</cleanup>
  </lifecycle-management>

  <integration>
    <with-agent name="architect">Only architect writes to docs/adr/. Other agents propose content.</with-agent>
    <with-skill name="documentation">ADRs follow documentation standards (Markdown, no placeholders).</with-skill>
    <with-skill name="spec-consistency">Major implementation changes may require new ADRs.</with-skill>
    <with-skill name="system-evolution">Impact assessments may trigger ADR for mitigation decisions.</with-skill>
  </integration>

  <references>
    <reference>docs/README.md for documentation structure.</reference>
    <reference>docs/templates/adr.md for the ADR template (created via architect).</reference>
  </references>
</skill>
