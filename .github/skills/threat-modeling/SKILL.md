---
name: threat-modeling
description: A skill for identifying security risks using lightweight, repeatable prompts.
---

<skill>
  <name>threat-modeling</name>
  <description>
  Surfaces security risks early using repeatable prompts based on STRIDE. Lightweight enough for feature work, thorough enough to catch real threats. Drives mitigations before code ships.
  </description>

  <when-to-use>
    <trigger>New feature handling user data, authentication, or authorization.</trigger>
    <trigger>API endpoints exposed to external consumers.</trigger>
    <trigger>Third-party integrations or data exchanges.</trigger>
    <trigger>Changes to security-critical flows (login, payment, admin).</trigger>
    <trigger>New data storage or transmission mechanisms.</trigger>
    <scope>Run before implementation for Standard+ complexity; revisit during major changes.</scope>
  </when-to-use>

  <stride-framework>
    <category name="Spoofing">
      <question>Can an attacker pretend to be a legitimate user or system?</question>
      <examples>Credential theft, session hijacking, API key compromise, forged tokens.</examples>
      <mitigations>Strong authentication, MFA, secure token storage, certificate validation.</mitigations>
    </category>
    <category name="Tampering">
      <question>Can an attacker modify data in transit or at rest?</question>
      <examples>Man-in-the-middle, SQL injection, parameter manipulation, file tampering.</examples>
      <mitigations>Input validation, parameterized queries, HTTPS, checksums, signed requests.</mitigations>
    </category>
    <category name="Repudiation">
      <question>Can an attacker deny performing an action without trace?</question>
      <examples>Deleting logs, unsigned transactions, no audit trail.</examples>
      <mitigations>Comprehensive logging, audit trails, signed events, non-repudiation tokens.</mitigations>
    </category>
    <category name="Information Disclosure">
      <question>Can an attacker access data they shouldn't see?</question>
      <examples>Verbose errors, debug endpoints, exposed secrets, IDOR, data in URLs.</examples>
      <mitigations>Least privilege, encryption, sanitized errors, access controls, data masking.</mitigations>
    </category>
    <category name="Denial of Service">
      <question>Can an attacker degrade or prevent legitimate access?</question>
      <examples>Resource exhaustion, unbounded queries, no rate limits, amplification attacks.</examples>
      <mitigations>Rate limiting, pagination, timeouts, resource quotas, CDN/WAF.</mitigations>
    </category>
    <category name="Elevation of Privilege">
      <question>Can an attacker gain unauthorized capabilities?</question>
      <examples>Privilege escalation, missing auth checks, insecure direct object references.</examples>
      <mitigations>RBAC, authorization on every request, principle of least privilege.</mitigations>
    </category>
  </stride-framework>

  <analysis-protocol>
    <step number="1">Identify assets: What needs protection? (User data, credentials, business logic)</step>
    <step number="2">Map entry points: Where do requests/data enter? (APIs, forms, file uploads, queues)</step>
    <step number="3">Map data flows: How does data move through the system? (Client → API → DB)</step>
    <step number="4">Apply STRIDE: For each entry point, ask each STRIDE question.</step>
    <step number="5">Identify threats: List realistic attack scenarios.</step>
    <step number="6">Assess risk: Rate each threat by likelihood and impact.</step>
    <step number="7">Propose mitigations: Specific countermeasures for high-risk threats.</step>
    <step number="8">Document residuals: Risks accepted or deferred with rationale.</step>
  </analysis-protocol>

  <risk-rating>
    <impact-levels>
      <level name="Critical">Data breach, compliance violation, complete compromise.</level>
      <level name="High">Significant data exposure, authentication bypass.</level>
      <level name="Medium">Limited data exposure, functionality abuse.</level>
      <level name="Low">Minimal impact, information leakage.</level>
    </impact-levels>
    <likelihood-levels>
      <level name="High">Easy to exploit, publicly known, no special access needed.</level>
      <level name="Medium">Requires some skill or specific conditions.</level>
      <level name="Low">Difficult, requires insider access or chained exploits.</level>
    </likelihood-levels>
    <priority>Risk = Impact × Likelihood. Address Critical/High-High first.</priority>
  </risk-rating>

  <output-format>
    <section name="Scope">Feature/component being analyzed.</section>
    <section name="Assets">What needs protection.</section>
    <section name="Entry Points">List with brief description.</section>
    <section name="Threat Matrix">
      | Threat | STRIDE | Entry Point | Impact | Likelihood | Mitigation |
      |--------|--------|-------------|--------|------------|------------|
    </section>
    <section name="Residual Risks">Accepted risks with rationale and owner.</section>
    <section name="Action Items">Mitigations to implement, assigned to developer.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Threat modeling after implementation (too late to influence design).</anti-pattern>
    <anti-pattern>Generic threats without specific attack scenarios.</anti-pattern>
    <anti-pattern>Identifying threats but no mitigations.</anti-pattern>
    <anti-pattern>Ignoring residual risks instead of documenting acceptance.</anti-pattern>
    <anti-pattern>One-time exercise never revisited as system evolves.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="privacy-review">Overlaps on data handling; run together for data-heavy features.</with-skill>
    <with-skill name="release-readiness">High residual risks may block release.</with-skill>
    <with-agent name="developer">Implements mitigations identified here.</with-agent>
    <with-agent name="tester">Security tests validate mitigations work.</with-agent>
    <with-agent name="architect">Reviews threat model for architectural implications.</with-agent>
  </integration>

  <references>
    <reference>docs/security.md for security architecture.</reference>
    <reference>docs/api.md for entry points and contracts.</reference>
  </references>
</skill>
