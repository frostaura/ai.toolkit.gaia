---
name: release-readiness
description: A skill for ensuring safe and deliberate releases with clear go or no-go checks.
---

<skill>
  <name>release-readiness</name>
  <description>
  Ensures releases are deliberate and safe through structured go/no-go checks. Covers rollback planning, monitoring, staged rollout, and stakeholder communication. Reduces deployment risk and improves recovery speed.
  </description>

  <when-to-use>
    <trigger>Before any production deployment.</trigger>
    <trigger>Before major version releases or breaking changes.</trigger>
    <trigger>Before releases with database migrations.</trigger>
    <trigger>Before security-sensitive releases.</trigger>
    <required>All Standard+ complexity releases must complete readiness check.</required>
  </when-to-use>

  <readiness-checklist>
    <category name="Code Quality">
      <check status="required">All quality gates pass (build, lint, test, coverage).</check>
      <check status="required">No Critical or High severity bugs open.</check>
      <check status="required">Code review completed and approved.</check>
      <check status="recommended">Security review completed for auth/data changes.</check>
    </category>
    <category name="Testing">
      <check status="required">Unit tests pass with coverage targets met.</check>
      <check status="required">Integration tests pass.</check>
      <check status="conditional">E2E tests pass (required for High+ risk).</check>
      <check status="conditional">Regression tests pass (required for Medium+ risk).</check>
      <check status="conditional">Performance tests within budget (if perf-sensitive).</check>
      <check status="conditional">Chaos tests passed (if reliability-critical).</check>
    </category>
    <category name="Rollback Plan">
      <check status="required">Rollback procedure documented.</check>
      <check status="required">Rollback owner identified and available.</check>
      <check status="required">Rollback tested in staging (for major releases).</check>
      <check status="required">Database migrations are reversible or rollback script exists.</check>
      <check status="required">Maximum rollback time estimated.</check>
    </category>
    <category name="Monitoring & Alerting">
      <check status="required">Key metrics dashboards ready.</check>
      <check status="required">Alerts configured for error rates, latency, availability.</check>
      <check status="required">Log aggregation capturing new code paths.</check>
      <check status="recommended">Baseline metrics captured before deploy.</check>
    </category>
    <category name="Staged Rollout">
      <check status="conditional">Feature flags in place (if applicable).</check>
      <check status="conditional">Canary or percentage rollout configured.</check>
      <check status="required">Rollout stages defined (e.g., 1% → 10% → 50% → 100%).</check>
      <check status="required">Success criteria for each stage defined.</check>
      <check status="required">Bake time between stages documented.</check>
    </category>
    <category name="Communication">
      <check status="required">Stakeholders notified of release timing.</check>
      <check status="conditional">User-facing changelog prepared (if visible changes).</check>
      <check status="conditional">Support team briefed (if user-impacting).</check>
      <check status="required">On-call/incident owner identified.</check>
    </category>
    <category name="Documentation">
      <check status="required">Spec-code consistency verified.</check>
      <check status="conditional">API docs updated (if API changes).</check>
      <check status="conditional">User docs updated (if behavior changes).</check>
      <check status="conditional">Runbook updated (if operational changes).</check>
    </category>
  </readiness-checklist>

  <go-no-go-criteria>
    <go>All "required" checks pass; all "conditional" checks applicable to this release pass.</go>
    <conditional-go>Minor gaps with documented mitigations and owner commitment to address post-release.</conditional-go>
    <no-go>Any Critical gap: failing quality gates, missing rollback plan, no monitoring, unreviewed code.</no-go>
  </go-no-go-criteria>

  <rollback-plan-template>
    <section name="Trigger Conditions">When to initiate rollback (error rate >X%, latency >Yms, user reports).</section>
    <section name="Decision Maker">Who authorizes rollback.</section>
    <section name="Procedure">
      <step>Revert deployment to previous version (specific command/process).</step>
      <step>Run database rollback migration if applicable.</step>
      <step>Verify services healthy post-rollback.</step>
      <step>Notify stakeholders of rollback.</step>
    </section>
    <section name="Estimated Time">Total time from decision to recovery.</section>
    <section name="Post-Rollback">Root cause analysis, fix forward plan.</section>
  </rollback-plan-template>

  <post-release-validation>
    <timing>Perform within 15 minutes of deployment completion.</timing>
    <checks>
      <check>Application health checks pass.</check>
      <check>Key user flows functional (smoke test).</check>
      <check>Error rates at or below baseline.</check>
      <check>Latency within acceptable range.</check>
      <check>No unexpected alerts firing.</check>
      <check>Logs show expected behavior.</check>
    </checks>
    <escalation>If any check fails, consider immediate rollback or hotfix.</escalation>
  </post-release-validation>

  <output-format>
    <section name="Release Summary">Version, scope, risk level.</section>
    <section name="Checklist Results">Table: Category | Check | Status | Notes.</section>
    <section name="Rollback Plan">Using template above.</section>
    <section name="Rollout Plan">Stages with success criteria and timing.</section>
    <section name="Decision">Go | Conditional Go (with conditions) | No-Go (with blockers).</section>
    <section name="Post-Release Validation Plan">Checks to perform after deploy.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>"We'll figure out rollback if we need it."</anti-pattern>
    <anti-pattern>Deploying Friday afternoon without on-call coverage.</anti-pattern>
    <anti-pattern>Skipping staging validation to meet deadline.</anti-pattern>
    <anti-pattern>No monitoring before deploying new features.</anti-pattern>
    <anti-pattern>Big-bang deployments without staged rollout.</anti-pattern>
    <anti-pattern>Deploying irreversible database changes without testing rollback.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="test-strategy">Test coverage informs readiness.</with-skill>
    <with-skill name="performance-budgeting">Performance within budget is go criterion.</with-skill>
    <with-skill name="chaos-readiness">Chaos test results inform reliability confidence.</with-skill>
    <with-skill name="spec-consistency">Spec alignment verified before release.</with-skill>
    <with-agent name="developer">Prepares release, documents rollback.</with-agent>
    <with-agent name="tester">Confirms test gates pass.</with-agent>
    <with-agent name="architect">Reviews readiness for major releases.</with-agent>
  </integration>

  <references>
    <reference>docs/deployment.md for deployment procedures.</reference>
    <reference>docs/testing.md for test gate definitions.</reference>
  </references>
</skill>
