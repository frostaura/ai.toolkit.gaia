---
name: privacy-review
description: A skill for reviewing data handling, retention, and access to reduce privacy risk.
---

<skill>
  <name>privacy-review</name>
  <description>
  Reviews data handling practices to reduce privacy risk and support compliance. Maps data flows, identifies PII, validates retention, and recommends minimization. Runs before features collecting or processing personal data.
  </description>

  <when-to-use>
    <trigger>New feature collecting user data (forms, tracking, uploads).</trigger>
    <trigger>Changes to data storage, retention, or deletion logic.</trigger>
    <trigger>Third-party data sharing or integrations.</trigger>
    <trigger>Analytics, logging, or telemetry additions.</trigger>
    <trigger>New user-facing consent or preference features.</trigger>
    <trigger>Cross-border data transfer changes.</trigger>
    <scope>Run before implementation; revisit when data handling changes.</scope>
  </when-to-use>

  <data-classification>
    <category name="PII (Personal Identifiable Information)">
      <examples>Name, email, phone, address, IP address, device ID, SSN, passport.</examples>
      <handling>Encrypt in transit and at rest; strict access controls; retention limits.</handling>
      <risk>High - regulatory impact, user harm if breached.</risk>
    </category>
    <category name="Sensitive PII">
      <examples>Health data, financial data, biometrics, race, religion, sexual orientation.</examples>
      <handling>Highest protection; explicit consent; consider not storing.</handling>
      <risk>Critical - severe regulatory and reputational impact.</risk>
    </category>
    <category name="Behavioral Data">
      <examples>Browsing history, click patterns, usage analytics, preferences.</examples>
      <handling>Anonymize where possible; clear consent; retention limits.</handling>
      <risk>Medium - can identify users when combined with other data.</risk>
    </category>
    <category name="Business Data">
      <examples>Transactions, contracts, internal communications.</examples>
      <handling>Access controls; backup; retention per business needs.</handling>
      <risk>Medium-Low - competitive/legal risk, not privacy-focused.</risk>
    </category>
  </data-classification>

  <review-checklist>
    <category name="Data Inventory">
      <check>What data is collected? (List all fields/attributes)</check>
      <check>What is the data classification? (PII, Sensitive, Behavioral, Business)</check>
      <check>Is each data element necessary? (Minimization check)</check>
      <check>Can any data be anonymized or pseudonymized?</check>
    </category>
    <category name="Collection">
      <check>Is consent obtained before collection?</check>
      <check>Is the purpose clearly communicated to users?</check>
      <check>Is collection limited to stated purpose?</check>
      <check>Are there legitimate grounds for processing? (Consent, contract, legal, vital, public, legitimate interest)</check>
    </category>
    <category name="Storage">
      <check>Where is data stored? (Region, provider, on-prem vs cloud)</check>
      <check>Is data encrypted at rest?</check>
      <check>Are backups encrypted and access-controlled?</check>
      <check>Is storage location compliant with data residency requirements?</check>
    </category>
    <category name="Access">
      <check>Who can access the data? (Roles, services, third parties)</check>
      <check>Is access logged and auditable?</check>
      <check>Is principle of least privilege applied?</check>
      <check>Are there controls for privileged access?</check>
    </category>
    <category name="Retention">
      <check>How long is data retained?</check>
      <check>Is retention period documented and justified?</check>
      <check>Is there automated deletion after retention period?</check>
      <check>Are backups included in retention policies?</check>
    </category>
    <category name="Deletion">
      <check>Can users request data deletion?</check>
      <check>Is deletion complete (including backups, logs, caches)?</check>
      <check>Is deletion verified and auditable?</check>
      <check>How are deletion requests handled for shared data?</check>
    </category>
    <category name="Sharing">
      <check>Is data shared with third parties?</check>
      <check>Are Data Processing Agreements in place?</check>
      <check>Is cross-border transfer compliant (SCCs, adequacy)?</check>
      <check>Can users opt out of sharing?</check>
    </category>
  </review-checklist>

  <data-flow-mapping>
    <step number="1">Identify data entry points (forms, APIs, imports, tracking).</step>
    <step number="2">Map data movement: entry → processing → storage → output.</step>
    <step number="3">Note each system/service that touches the data.</step>
    <step number="4">Identify data exits: exports, APIs, reports, third-party sharing.</step>
    <step number="5">Document retention and deletion points in flow.</step>
    <format>Use simple diagram or table: Source → Process → Storage → Access → Exit.</format>
  </data-flow-mapping>

  <output-format>
    <section name="Scope">Feature/component being reviewed.</section>
    <section name="Data Inventory">Table: Data Element | Classification | Purpose | Necessary?</section>
    <section name="Data Flow">Diagram or description of data movement.</section>
    <section name="Checklist Results">Pass/Fail/N-A per category with notes.</section>
    <section name="Risks Found">Privacy risks with severity and likelihood.</section>
    <section name="Recommendations">Minimization, anonymization, control improvements.</section>
    <section name="Compliance Notes">Relevant regulations (GDPR, CCPA, etc.) and status.</section>
    <section name="Actions">Required changes before release, assigned to developer.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Collecting data "just in case" without specific purpose.</anti-pattern>
    <anti-pattern>No retention policy (data kept forever).</anti-pattern>
    <anti-pattern>Logging PII in application logs.</anti-pattern>
    <anti-pattern>Third-party sharing without DPA.</anti-pattern>
    <anti-pattern>Assuming anonymization is sufficient without verification.</anti-pattern>
    <anti-pattern>Privacy review after launch (too late).</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="threat-modeling">Run together; threat model covers attack vectors, privacy covers data handling.</with-skill>
    <with-skill name="release-readiness">High privacy risks may block release.</with-skill>
    <with-agent name="developer">Implements minimization and controls.</with-agent>
    <with-agent name="architect">Reviews data flow design decisions.</with-agent>
  </integration>

  <references>
    <reference>docs/data.md for data model details.</reference>
    <reference>docs/security.md for access control model.</reference>
  </references>
</skill>
