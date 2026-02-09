---
name: system-evolution
description: A skill for assessing change impact across components before implementation.
---

<skill>
  <name>system-evolution</name>
  <description>
  Assesses ripple effects of changes before implementation. Maps dependencies, identifies risks, and ensures changes don't introduce hidden regressions or create technical debt.
  </description>

  <when-to-use>
    <trigger>API contract changes (new endpoints, modified schemas, deprecated fields).</trigger>
    <trigger>Database schema changes (new tables, column modifications, migrations).</trigger>
    <trigger>Cross-component changes affecting more than one service or layer.</trigger>
    <trigger>Security model changes (auth flows, permissions, access controls).</trigger>
    <trigger>Third-party integration changes (new providers, API version upgrades).</trigger>
    <trigger>Performance-sensitive changes (caching, indexing, query optimization).</trigger>
    <skip>Isolated changes within a single component that don't affect interfaces.</skip>
  </when-to-use>

  <impact-analysis-protocol>
    <step number="1">Describe the proposed change in one paragraph.</step>
    <step number="2">Identify directly affected components (code, data, config).</step>
    <step number="3">Map dependencies: what consumes/depends on affected components?</step>
    <step number="4">Assess each impact area using the checklist below.</step>
    <step number="5">Identify migration/rollout requirements.</step>
    <step number="6">List risks with likelihood and severity.</step>
    <step number="7">Propose mitigations for high-severity risks.</step>
    <step number="8">Summarize go/no-go recommendation.</step>
  </impact-analysis-protocol>

  <impact-areas>
    <area name="API Contracts">
      <questions>
        <question>Do request/response schemas change?</question>
        <question>Are endpoints added, modified, or deprecated?</question>
        <question>Do existing clients break (backward compatibility)?</question>
        <question>Is API versioning required?</question>
      </questions>
      <risk-indicators>Breaking changes, missing versioning, undocumented modifications.</risk-indicators>
    </area>
    <area name="Data Layer">
      <questions>
        <question>Do table structures or relationships change?</question>
        <question>Is data migration required?</question>
        <question>Are indexes affected (performance implications)?</question>
        <question>Is there data loss risk during migration?</question>
      </questions>
      <risk-indicators>Irreversible migrations, missing rollback scripts, data integrity gaps.</risk-indicators>
    </area>
    <area name="Security">
      <questions>
        <question>Do authentication or authorization flows change?</question>
        <question>Are new attack surfaces introduced?</question>
        <question>Is sensitive data handling affected?</question>
        <question>Do audit/logging requirements change?</question>
      </questions>
      <risk-indicators>Privilege escalation paths, exposed endpoints, missing access checks.</risk-indicators>
    </area>
    <area name="Performance">
      <questions>
        <question>Are query patterns or data volumes changing?</question>
        <question>Is caching impacted?</question>
        <question>Are there new latency-sensitive paths?</question>
        <question>Do resource requirements (CPU, memory) increase?</question>
      </questions>
      <risk-indicators>N+1 queries, cache invalidation gaps, unbounded growth.</risk-indicators>
    </area>
    <area name="Testing">
      <questions>
        <question>Do existing tests cover the change?</question>
        <question>Are new test scenarios required?</question>
        <question>Do integration tests need updating?</question>
        <question>Is regression testing scope affected?</question>
      </questions>
      <risk-indicators>Untested paths, stale test fixtures, missing edge cases.</risk-indicators>
    </area>
    <area name="Observability">
      <questions>
        <question>Are logs, metrics, or traces affected?</question>
        <question>Do alerts need updating?</question>
        <question>Is debugging capability maintained?</question>
      </questions>
      <risk-indicators>Silent failures, missing metrics, unclear error attribution.</risk-indicators>
    </area>
  </impact-areas>

  <risk-assessment>
    <severity-levels>
      <level name="Critical">Data loss, security breach, complete outage. Blocks release.</level>
      <level name="High">Significant functionality loss, security degradation. Requires mitigation.</level>
      <level name="Medium">Partial functionality impact, performance regression. Should mitigate.</level>
      <level name="Low">Minor inconvenience, cosmetic issues. Can defer mitigation.</level>
    </severity-levels>
    <likelihood-levels>
      <level name="Certain">Will happen without mitigation.</level>
      <level name="Likely">Probable in normal operation.</level>
      <level name="Possible">Could happen under specific conditions.</level>
      <level name="Unlikely">Rare edge case only.</level>
    </likelihood-levels>
    <priority>Priority = Severity × Likelihood. Address Critical/Certain first.</priority>
  </risk-assessment>

  <output-format>
    <section name="Change Summary">One paragraph describing the change.</section>
    <section name="Affected Components">List with brief impact notes.</section>
    <section name="Dependency Map">What depends on affected components.</section>
    <section name="Impact Assessment">Checklist results per area with findings.</section>
    <section name="Risk Register">Table: Risk | Severity | Likelihood | Mitigation.</section>
    <section name="Migration Steps">Required sequencing for safe rollout.</section>
    <section name="Recommendation">Go / Conditional Go / No-Go with rationale.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Skipping analysis for "small" changes that touch shared code.</anti-pattern>
    <anti-pattern>Identifying risks without proposing mitigations.</anti-pattern>
    <anti-pattern>Ignoring downstream consumers when changing APIs.</anti-pattern>
    <anti-pattern>Assuming tests will catch everything without verifying coverage.</anti-pattern>
    <anti-pattern>Rolling out without migration plan or rollback strategy.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="spec-consistency">Changes may require doc updates; flag for architect.</with-skill>
    <with-skill name="architecture-decision-records">Major changes may warrant ADR.</with-skill>
    <with-skill name="release-readiness">Impact findings feed rollout planning.</with-skill>
    <with-agent name="analyst">Analyst gathers codebase data for dependency mapping.</with-agent>
    <with-agent name="developer">Developer implements mitigations identified here.</with-agent>
  </integration>

  <references>
    <reference>docs/design.md for architecture context.</reference>
    <reference>docs/api.md for contract details.</reference>
    <reference>docs/data.md for schema information.</reference>
    <reference>docs/security.md for access control model.</reference>
  </references>
</skill>
