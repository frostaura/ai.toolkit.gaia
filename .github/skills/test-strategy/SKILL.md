---
name: test-strategy
description: A skill for tailoring test depth to risk and defining coverage targets.
---

<skill>
  <name>test-strategy</name>
  <description>
  Tailors test depth to risk level, ensuring critical paths get comprehensive coverage while trivial changes get appropriate (lighter) validation. Balances confidence with cost.
  </description>

  <risk-assessment>
    <step number="1">Identify the change scope: files touched, components affected.</step>
    <step number="2">Assess blast radius: how many other components depend on this?</step>
    <step number="3">Evaluate reversibility: can this be easily rolled back if broken?</step>
    <step number="4">Check data sensitivity: does this touch user data, payments, auth?</step>
    <step number="5">Assign risk tier using criteria below.</step>
  </risk-assessment>

  <risk-tiers>
    <tier name="Low">
      <criteria>Single component, no behavior change, easily reversible, no sensitive data.</criteria>
      <examples>CSS fixes, typo corrections, internal refactors, dependency updates (non-breaking).</examples>
      <test-approach>Unit tests on touched code; spot manual check; no regression suite.</test-approach>
      <coverage-target>50% on touched lines (aim 100% where practical).</coverage-target>
    </tier>
    <tier name="Medium">
      <criteria>Behavior changes in one component/API, moderate blast radius, some data handling.</criteria>
      <examples>New endpoint, modified business logic, UI component changes, form validation.</examples>
      <test-approach>Unit tests + integration tests for affected API; targeted regression on feature.</test-approach>
      <coverage-target>70% on touched code, 100% on new logic paths.</coverage-target>
    </tier>
    <tier name="High">
      <criteria>Cross-component changes, data migrations, security-affecting, hard to rollback.</criteria>
      <examples>Auth flow changes, payment integration, schema migrations, permission model updates.</examples>
      <test-approach>Unit + integration + E2E; full regression on affected features; security review.</test-approach>
      <coverage-target>80% overall, 100% on security/data paths.</coverage-target>
    </tier>
    <tier name="Critical">
      <criteria>System-wide impact, irreversible changes, regulatory/compliance implications.</criteria>
      <examples>Major version migrations, encryption changes, GDPR features, multi-tenant isolation.</examples>
      <test-approach>All test types + manual QA + security audit + staged rollout with monitoring.</test-approach>
      <coverage-target>90%+ overall, 100% on critical paths, documented test plan.</coverage-target>
    </tier>
  </risk-tiers>

  <test-type-guidance>
    <type name="Unit Tests">
      <when>Always. Every change should have unit tests.</when>
      <focus>Business logic, pure functions, state transformations, edge cases.</focus>
      <tools>.NET: xUnit. JS/TS: Vitest. Python: pytest.</tools>
      <anti-pattern>Testing implementation details instead of behavior.</anti-pattern>
    </type>
    <type name="Integration Tests">
      <when>Medium+ risk. API changes, database operations, external service calls.</when>
      <focus>API contracts, data persistence, service interactions.</focus>
      <anti-pattern>Testing through UI when API test would suffice.</anti-pattern>
    </type>
    <type name="E2E Tests">
      <when>High+ risk. Critical user journeys, multi-step flows, cross-system features.</when>
      <focus>Happy paths, key user journeys, checkout/auth/signup flows.</focus>
      <anti-pattern>Duplicating unit test coverage in E2E; over-testing non-critical paths.</anti-pattern>
    </type>
    <type name="Regression Tests">
      <when>Medium+ risk on features with documented use cases.</when>
      <focus>All scenarios in use-cases.md for affected features.</focus>
      <reference>See regression-testing skill for execution protocol.</reference>
    </type>
    <type name="Manual Testing">
      <when>Visual changes, UX flows, exploratory testing, accessibility.</when>
      <focus>Things automation can't easily verify: feel, usability, edge interactions.</focus>
      <anti-pattern>Using manual testing as substitute for missing automated tests.</anti-pattern>
    </type>
  </test-type-guidance>

  <coverage-matrix>
    <matrix>
      | Risk Tier | Unit | Integration | E2E | Regression | Manual | Security Review |
      |-----------|------|-------------|-----|------------|--------|----------------|
      | Low       | Yes  | If API      | No  | No         | Spot   | No             |
      | Medium    | Yes  | Yes         | Key paths | Targeted | As needed | No    |
      | High      | Yes  | Yes         | Yes | Full       | Yes    | Yes            |
      | Critical  | Yes  | Yes         | Yes | Full       | Full QA | Audit          |
    </matrix>
  </coverage-matrix>

  <test-plan-template>
    <section name="Change Summary">What's being changed and why.</section>
    <section name="Risk Assessment">Tier assignment with justification.</section>
    <section name="Test Plan">
      <item>Unit tests: [list or "existing adequate"]</item>
      <item>Integration tests: [list or "N/A"]</item>
      <item>E2E tests: [list or "N/A"]</item>
      <item>Regression scope: [features/scenarios or "N/A"]</item>
      <item>Manual testing: [checklist or "spot check"]</item>
    </section>
    <section name="Coverage Targets">Expected coverage % with current baseline.</section>
    <section name="Test Data">Special data requirements or fixtures needed.</section>
    <section name="Environment">Where tests run (local, CI, staging).</section>
  </test-plan-template>

  <anti-patterns>
    <anti-pattern>Same test depth for all changes regardless of risk.</anti-pattern>
    <anti-pattern>Skipping tests for "simple" changes that touch shared code.</anti-pattern>
    <anti-pattern>100% coverage on trivial code, gaps on critical paths.</anti-pattern>
    <anti-pattern>E2E tests that could be unit tests (slow, brittle).</anti-pattern>
    <anti-pattern>No test plan for High/Critical changes.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="unit-testing">Provides platform-specific unit test guidance.</with-skill>
    <with-skill name="regression-testing">Provides manual regression protocol.</with-skill>
    <with-agent name="tester">Executes test plan and reports results.</with-agent>
    <with-agent name="developer">Writes unit/integration tests per strategy.</with-agent>
  </integration>

  <references>
    <reference>docs/testing.md for project test standards.</reference>
    <reference>docs/use-cases.md for regression scenario source.</reference>
  </references>
</skill>
