---
name: documentation
description: A skill for generating and maintaining project documentation, including design documents, user guides, and API documentation. This skill ensures that all documentation is clear, comprehensive, and up-to-date to facilitate effective communication and knowledge sharing among team members and stakeholders and based on the repository structure.
---

<skill>
  <name>documentation</name>
  <description>
  Standards for generating and maintaining project documentation. Ensures docs are clear, complete, aligned with spec-driven design, and synchronized with code.
  </description>

  <structure>
    <location>docs/ directory at repository root</location>
    <format>Markdown (.md), lowercase-kebab-case filenames</format>
    <documents>
      <doc name="design.md">Use cases, architecture overview, system design. For Standard+ complexity.</doc>
      <doc name="api.md">API contracts, endpoints, request/response schemas, auth requirements.</doc>
      <doc name="data.md">Database schema, entity relationships, migration strategy.</doc>
      <doc name="security.md">Authentication flows, authorization rules, threat mitigations.</doc>
      <doc name="deployment.md">Deployment procedures, environments, CI/CD, infrastructure.</doc>
      <doc name="testing.md">Testing strategy, coverage requirements, quality gate definitions.</doc>
    </documents>
  </structure>

  <quality-rules>
    <rule>**No placeholders** — zero [TODO]/[TBD]. If unknown, document the decision process instead.</rule>
    <rule>**Consistent terminology** — define terms once, reference consistently. Glossary if needed.</rule>
    <rule>**Traceability** — link requirements to implementation with file:line references.</rule>
    <rule>**Version controlled** — doc updates committed with related code changes in same PR.</rule>
    <rule>**Current state only** — outdated docs are treated as defects and fixed immediately.</rule>
    <rule>**No temporary files** — no TODO.md, CHECKLIST.md, NOTES.md, PROGRESS.md, scratch.md, etc. Use MCP task/memory tools instead.</rule>
  </quality-rules>

  <creation-principles>
    <principle>**Just-in-time** — create docs when features are designed/implemented, not as empty templates.</principle>
    <principle>**Adaptive depth** — match documentation depth to task complexity.</principle>
    <principle>**Living documents** — evolve with the codebase; regular reviews are part of development.</principle>
  </creation-principles>

  <spec-driven-design>
    <rule>For Complex+ tasks, design docs must exist before implementation begins.</rule>
    <rule>Code must match documented design; deviations require doc updates before merge.</rule>
    <rule>Bidirectional sync: code changes → update docs; requirement changes → update design then code.</rule>
    <rule>Quality gates must verify doc-code alignment for Standard+ complexity.</rule>
  </spec-driven-design>

  <content-standards>
    <standard>**Headings** — clear hierarchy (# title, ## sections, ### subsections). TOC for docs >200 lines.</standard>
    <standard>**Diagrams** — Mermaid.js required for all visual documentation (architecture, sequence, ERD, flow, state). No external image files unless unavoidable.</standard>
    <standard>**Code examples** — syntax-highlighted, copy-paste ready with necessary imports.</standard>
    <standard>**References** — relative links to other docs; absolute paths with line numbers to code.</standard>
  </content-standards>

  <doc-types>
    <type name="Technical Design">Architecture decisions, system design for developers/architects.</type>
    <type name="API Documentation">Endpoint contracts, schemas for API consumers.</type>
    <type name="User Guides">Feature explanations, how-to instructions for end-users.</type>
    <type name="Developer Guides">Setup instructions, dev workflows, contribution guidelines.</type>
    <type name="Operations Guides">Deployment, monitoring, troubleshooting for operators.</type>
  </doc-types>

  <prohibited>
    <item>Temporary files: TODO.md, CHECKLIST.md, NOTES.md, PROGRESS.md, TASKS.md</item>
    <item>Dev artifacts: scratch.md, test.md, debug-notes.md, wip.md</item>
    <item>Meeting notes: standup-notes.md, meeting-*.md</item>
    <item>Personal checklists: my-tasks.md, dev-checklist.md</item>
  </prohibited>
</skill>
