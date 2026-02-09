---
name: performance-budgeting
description: A skill for defining and enforcing performance budgets to prevent regressions.
---

<skill>
  <name>performance-budgeting</name>
  <description>
  Defines measurable performance limits and enforces them as quality gates. Prevents creeping regressions by making performance visible, tracked, and non-negotiable.
  </description>

  <budget-categories>
    <category name="Latency">
      <metrics>
        <metric name="Time to First Byte">Server response start time. Target: <200ms.</metric>
        <metric name="First Contentful Paint">First render visible. Target: <1.5s.</metric>
        <metric name="Largest Contentful Paint">Main content visible. Target: <2.5s.</metric>
        <metric name="Time to Interactive">Page fully interactive. Target: <3.5s.</metric>
        <metric name="API Response Time">P50 and P95 latencies. Target: P50 <100ms, P95 <500ms.</metric>
      </metrics>
    </category>
    <category name="Payload Size">
      <metrics>
        <metric name="JavaScript Bundle">Total JS transferred. Target: <300KB gzipped.</metric>
        <metric name="CSS Bundle">Total CSS transferred. Target: <50KB gzipped.</metric>
        <metric name="Image Assets">Per-page image weight. Target: <500KB.</metric>
        <metric name="API Response Size">Typical response payload. Target: <100KB.</metric>
        <metric name="Total Page Weight">All resources. Target: <1.5MB.</metric>
      </metrics>
    </category>
    <category name="Resource Usage">
      <metrics>
        <metric name="Memory (Server)">Container/process memory. Target: <512MB typical.</metric>
        <metric name="Memory (Client)">Browser JS heap. Target: <100MB.</metric>
        <metric name="CPU (Server)">Container CPU utilization. Target: <70% at normal load.</metric>
        <metric name="Database Queries">Queries per request. Target: <10, no N+1.</metric>
      </metrics>
    </category>
    <category name="Throughput">
      <metrics>
        <metric name="Requests per Second">Sustained load capacity. Target: based on traffic estimates.</metric>
        <metric name="Concurrent Users">Simultaneous active sessions. Target: based on traffic estimates.</metric>
        <metric name="Error Rate">5xx errors under load. Target: <0.1%.</metric>
      </metrics>
    </category>
  </budget-categories>

  <baseline-capture>
    <step number="1">Identify key user flows (page loads, API calls, critical transactions).</step>
    <step number="2">Run performance tests under controlled conditions (consistent environment, data).</step>
    <step number="3">Record current values for each metric in budget categories.</step>
    <step number="4">Document measurement methodology (tools, environment, data set).</step>
    <step number="5">Store baseline as reference for regression detection.</step>
    <repeatable>Re-capture baseline after major releases or infrastructure changes.</repeatable>
  </baseline-capture>

  <budget-definition>
    <principle>Budgets = Baseline + Acceptable Variance (typically 10-20%).</principle>
    <format>
      | Metric | Baseline | Budget | Variance |
      |--------|----------|--------|----------|
      | LCP    | 2.1s     | 2.5s   | +20%     |
      | JS Bundle | 250KB | 300KB  | +20%     |
    </format>
    <ownership>Architect sets budgets; developer maintains; tester validates.</ownership>
    <review>Review budgets quarterly or when requirements change significantly.</review>
  </budget-definition>

  <enforcement>
    <gate name="Build Time">
      <check>Bundle size limits via build tools (webpack-bundle-analyzer, size-limit).</check>
      <action>Fail build if bundle exceeds budget.</action>
    </gate>
    <gate name="CI Pipeline">
      <check>Lighthouse CI, WebPageTest, or custom perf tests.</check>
      <action>Fail pipeline if metrics exceed budgets.</action>
    </gate>
    <gate name="Pre-Release">
      <check>Load testing against throughput/latency budgets.</check>
      <action>Block release if P95 latency or error rate exceeds budget.</action>
    </gate>
    <gate name="Production Monitoring">
      <check>Real user monitoring (RUM) and synthetic tests.</check>
      <action>Alert when metrics trend toward budget; incident if exceeded.</action>
    </gate>
  </enforcement>

  <regression-response>
    <detection>Any metric exceeding budget triggers investigation.</detection>
    <immediate>Identify the change that caused regression (recent PRs, deployments).</immediate>
    <options>
      <option>Revert the change if impact is severe.</option>
      <option>Optimize to bring back within budget.</option>
      <option>Request budget increase with justification (requires architect approval).</option>
    </options>
    <documentation>Record regression, root cause, and resolution in performance log.</documentation>
  </regression-response>

  <output-format>
    <section name="Budget Table">All metrics with baseline, budget, current value, status.</section>
    <section name="Trend Chart">Key metrics over time (optional, for reporting).</section>
    <section name="Violations">Any metrics exceeding budget with severity.</section>
    <section name="Actions">Required fixes or budget adjustment requests.</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Setting budgets without measuring baseline first.</anti-pattern>
    <anti-pattern>Budgets so loose they never catch regressions.</anti-pattern>
    <anti-pattern>Budgets so tight they block every change.</anti-pattern>
    <anti-pattern>Measuring only in CI, not production (RUM).</anti-pattern>
    <anti-pattern>Ignoring budget violations ("we'll fix it later").</anti-pattern>
    <anti-pattern>No ownership or accountability for performance.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="release-readiness">Performance within budget is release criteria.</with-skill>
    <with-skill name="chaos-readiness">Test performance under degraded conditions.</with-skill>
    <with-agent name="developer">Implements optimizations when budgets exceeded.</with-agent>
    <with-agent name="tester">Runs performance tests and reports violations.</with-agent>
  </integration>

  <references>
    <reference>docs/design.md for performance-sensitive flows.</reference>
    <reference>docs/testing.md for test methodology.</reference>
  </references>
</skill>
