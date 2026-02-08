---
name: documentation
description: A skill for generating and maintaining project documentation, including design documents, user guides, and API documentation. This skill ensures that all documentation is clear, comprehensive, and up-to-date to facilitate effective communication and knowledge sharing among team members and stakeholders and based on the repository structure.
---

<skill>
  <name>documentation</name>
  <description>
  A skill for generating and maintaining project documentation, including design documents, user guides, and API documentation. This skill ensures that all documentation is clear, comprehensive, and up-to-date to facilitate effective communication and knowledge sharing among team members and stakeholders. Documentation aligns with spec-driven design principles to ensure code and design remain synchronized.
  </description>
  <documentation-standards>
    <category name="Repository Structure">
      <standard name="Location">
        <value>docs/ directory</value>
        <description>All project documentation must reside in the docs/ directory at the repository root. This centralizes documentation and makes it easily discoverable.</description>
      </standard>
      <standard name="Format">
        <value>Markdown (.md)</value>
        <description>Use Markdown format for all documentation to ensure readability, version control compatibility, and universal accessibility.</description>
      </standard>
      <standard name="Naming">
        <value>lowercase-kebab-case.md</value>
        <description>File names should use lowercase with hyphens for readability and consistency across platforms.</description>
      </standard>
    </category>
    <category name="Design Documents">
      <document name="design.md">
        <value>docs/design.md</value>
        <description>Use cases, architecture overview, and system design. Created for Standard+ complexity tasks. Must include actors, user flows, and architectural decisions.</description>
      </document>
      <document name="api.md">
        <value>docs/api.md</value>
        <description>API contracts, endpoints, request/response schemas, and authentication requirements. Created when API changes are introduced.</description>
      </document>
      <document name="data.md">
        <value>docs/data.md</value>
        <description>Database schema, entity relationships, migration strategy, and data models. Created when database changes are required.</description>
      </document>
      <document name="security.md">
        <value>docs/security.md</value>
        <description>Authentication flows, authorization rules, security considerations, and threat mitigations. Created when security changes are made.</description>
      </document>
      <document name="deployment.md">
        <value>docs/deployment.md</value>
        <description>Deployment procedures, environment configurations, CI/CD pipeline details, and infrastructure requirements.</description>
      </document>
      <document name="testing.md">
        <value>docs/testing.md</value>
        <description>Testing strategy, coverage requirements, test scenarios, and quality gate definitions.</description>
      </document>
    </category>
    <category name="Quality Rules">
      <rule name="No Placeholders">
        <value>Zero tolerance for TODO/TBD</value>
        <description>Documentation must be complete at creation. No [TODO], [TBD], or placeholder sections allowed. If information is unknown, document the decision-making process instead.</description>
      </rule>
      <rule name="Consistent Terminology">
        <value>Single source of truth for terms</value>
        <description>Use consistent terminology across all documents. Define domain terms once and reference consistently. Maintain a glossary if needed.</description>
      </rule>
      <rule name="Traceability">
        <value>Link requirements to implementation</value>
        <description>Every design decision and requirement must be traceable to implementation. Use markdown file references with line numbers to link design to code.</description>
      </rule>
      <rule name="Version Control">
        <value>Document alongside code changes</value>
        <description>Documentation updates must be committed with related code changes. Design changes require doc updates in the same PR.</description>
      </rule>
      <rule name="Up-to-date">
        <value>Documentation reflects current state</value>
        <description>Documentation must accurately reflect the current system state. Outdated documentation is treated as a defect and must be corrected immediately.</description>
      </rule>
      <rule name="No Temporary Documentation">
        <value>Zero ephemeral docs in repository</value>
        <description>Checklists, temporary notes, progress reports, and development artifacts must NOT be committed to the repository. Use task and memory tools for temporary tracking. Clean up any temporary documentation before merging.</description>
      </rule>
      <rule name="Cleanup Required">
        <value>Remove development artifacts</value>
        <description>All TODO.md, CHECKLIST.md, NOTES.md, PROGRESS.md, and similar temporary files must be deleted before PR merge. Only permanent documentation in docs/ directory is allowed.</description>
      </rule>
    </category>
    <category name="On-Demand Creation">
      <principle name="Just-in-Time">
        <description>Create documentation only when needed, not as empty templates. Documents should be created when their corresponding features are being designed or implemented.</description>
      </principle>
      <principle name="Adaptive Depth">
        <description>Documentation depth should match task complexity. Trivial tasks need minimal docs, enterprise tasks need comprehensive documentation.</description>
      </principle>
      <principle name="Living Documents">
        <description>Documentation is a living artifact that evolves with the codebase. Regular reviews and updates are part of the development process.</description>
      </principle>
    </category>
    <category name="Spec-Driven Design">
      <principle name="Design Before Code">
        <description>For Complex+ tasks, design documents must be created and reviewed before implementation begins. This ensures alignment and reduces rework.</description>
      </principle>
      <principle name="Code Reflects Spec">
        <value>Implementation matches design</value>
        <description>Code implementation must match the documented design. Deviations require design document updates before merging.</description>
      </principle>
      <principle name="Bidirectional Sync">
        <value>Design ↔ Code synchronization</value>
        <description>When code changes, update docs. When requirements change, update design then code. Maintain continuous alignment between specification and implementation.</description>
      </principle>
      <principle name="Verification">
        <value>Quality gate for doc-code alignment</value>
        <description>Review process must verify that implementation matches documented design. Include doc review as part of quality gates for Standard+ complexity.</description>
      </principle>
    </category>
    <category name="Content Standards">
      <standard name="Structure">
        <value>Clear hierarchy with sections</value>
        <description>Use heading levels consistently (# for title, ## for main sections, ### for subsections). Include table of contents for documents over 200 lines.</description>
      </standard>
      <standard name="Diagrams">
        <value>Mermaid required for all visual documentation</value>
        <description>MUST use Mermaid.js for all architecture, sequence, entity-relationship, flow, and state diagrams. Mermaid diagrams are version-controlled, reviewable, and maintainable within markdown. No external image files for diagrams unless absolutely necessary (screenshots, photos).</description>
      </standard>
      <standard name="Diagram Types">
        <value>Use appropriate Mermaid diagram for context</value>
        <description>Architecture (C4/graph), Sequence (interactions), ERD (data models), Flowchart (processes), State (workflows), Gantt (timelines). Include diagram code blocks directly in markdown documentation.</description>
      </standard>
      <standard name="Code Examples">
        <value>Syntax-highlighted, runnable examples</value>
        <description>Include code examples with proper syntax highlighting. Examples should be copy-paste ready and include necessary imports/context.</description>
      </standard>
      <standard name="References">
        <value>Link to related documents and code</value>
        <description>Use relative links to reference other docs and absolute file paths with line numbers to reference code. Maintain linkability as primary concern.</description>
      </standard>
    </category>
    <category name="Documentation Types">
      <type name="Technical Design">
        <description>Architecture decisions, system design, technical specifications for developers and architects.</description>
      </type>
      <type name="API Documentation">
        <description>Endpoint contracts, request/response schemas, authentication requirements for API consumers.</description>
      </type>
      <type name="User Guides">
        <description>Feature explanations, how-to guides, and usage instructions for end-users.</description>
      </type>
      <type name="Developer Guides">
        <description>Setup instructions, development workflows, contribution guidelines for team members.</description>
      </type>
      <type name="Operations Guides">
        <description>Deployment procedures, monitoring setup, troubleshooting guides for operators.</description>
      </type>
    </category>
    <category name="Prohibited Documentation">
      <prohibited name="Temporary Files">
        <examples>TODO.md, CHECKLIST.md, NOTES.md, PROGRESS.md, TASKS.md</examples>
        <description>Temporary tracking files are prohibited in the repository. Use MCP task management and memory tools instead.</description>
      </prohibited>
      <prohibited name="Development Artifacts">
        <examples>scratch.md, test.md, debug-notes.md, wip.md</examples>
        <description>Development scratch files and work-in-progress notes must be deleted before commit. Keep local only.</description>
      </prohibited>
      <prohibited name="Meeting Notes">
        <examples>standup-notes.md, meeting-YYYY-MM-DD.md</examples>
        <description>Meeting notes and status reports should not be committed. Use external tools for ephemeral communications.</description>
      </prohibited>
      <prohibited name="Personal Checklists">
        <examples>my-tasks.md, dev-checklist.md</examples>
        <description>Individual developer checklists are not repository artifacts. Use personal tools or MCP task management.</description>
      </prohibited>
    </category>
  </documentation-standards>
</skill>
