---
name: chaos-readiness
description: A skill for validating failure modes through safe fault injection.
---

<skill>
  <name>chaos-readiness</name>
  <description>
  Validates system resilience by safely injecting faults in controlled environments. Reveals hidden dependencies, tests recovery mechanisms, and builds confidence before production incidents occur.
  </description>

  <safety-guardrails>
    <rule name="Environment">NEVER run chaos tests in production. Use staging, QA, or dedicated chaos environments only.</rule>
    <rule name="Blast Radius">Start with smallest scope; expand only after validating containment.</rule>
    <rule name="Rollback Ready">Have documented rollback steps before running any test.</rule>
    <rule name="Monitoring Active">Confirm observability is working before injecting faults.</rule>
    <rule name="Team Aware">Notify relevant team members before running chaos experiments.</rule>
    <rule name="Time-Boxed">Set maximum duration; auto-revert if not manually stopped.</rule>
  </safety-guardrails>

  <when-to-use>
    <trigger>Before major releases to validate resilience claims.</trigger>
    <trigger>After adding new dependencies or integrations.</trigger>
    <trigger>When SLOs are defined but not validated.</trigger>
    <trigger>Post-incident to verify fixes prevent recurrence.</trigger>
    <trigger>Quarterly as part of operational readiness review.</trigger>
    <skip>Early development phases where basic functionality isn't stable.</skip>
  </when-to-use>

  <fault-scenarios>
    <category name="Network Faults">
      <scenario name="Latency Injection">
        <description>Add artificial delay to network calls.</description>
        <test-values>100ms, 500ms, 2s, 10s.</test-values>
        <expected>Timeouts trigger gracefully; UX shows loading states; no cascading failures.</expected>
      </scenario>
      <scenario name="Packet Loss">
        <description>Drop percentage of network packets.</description>
        <test-values>1%, 5%, 20%, 50%.</test-values>
        <expected>Retries work; circuit breakers engage; no data corruption.</expected>
      </scenario>
      <scenario name="DNS Failure">
        <description>Simulate DNS resolution failures.</description>
        <expected>Fallback to cached values or graceful degradation.</expected>
      </scenario>
    </category>
    <category name="Dependency Faults">
      <scenario name="Service Outage">
        <description>Make dependent service completely unavailable.</description>
        <targets>Database, cache, auth service, third-party APIs.</targets>
        <expected>Circuit breakers open; fallbacks activate; clear error messages.</expected>
      </scenario>
      <scenario name="Slow Dependency">
        <description>Dependency responds but very slowly.</description>
        <test-values>5s, 30s response times.</test-values>
        <expected>Timeouts prevent thread exhaustion; requests don't queue indefinitely.</expected>
      </scenario>
      <scenario name="Intermittent Failures">
        <description>Dependency fails randomly (50% error rate).</description>
        <expected>Retries with backoff; circuit breaker after threshold.</expected>
      </scenario>
    </category>
    <category name="Resource Faults">
      <scenario name="CPU Exhaustion">
        <description>Consume CPU to simulate contention.</description>
        <test-values>50%, 80%, 95% utilization.</test-values>
        <expected>Graceful degradation; no OOM; priority tasks still complete.</expected>
      </scenario>
      <scenario name="Memory Pressure">
        <description>Consume memory to approach limits.</description>
        <expected>GC handles pressure; no crash; alerts fire appropriately.</expected>
      </scenario>
      <scenario name="Disk Full">
        <description>Fill disk to near capacity.</description>
        <expected>Writes fail gracefully; alerts fire; no data corruption.</expected>
      </scenario>
    </category>
    <category name="Application Faults">
      <scenario name="Exception Injection">
        <description>Force specific exceptions in code paths.</description>
        <targets>Database operations, API calls, business logic.</targets>
        <expected>Error handling works; user sees friendly message; logs capture details.</expected>
      </scenario>
      <scenario name="Process Crash">
        <description>Kill application process unexpectedly.</description>
        <expected>Orchestrator restarts; no data loss; recovery within SLO.</expected>
      </scenario>
    </category>
  </fault-scenarios>

  <experiment-protocol>
    <step number="1">Define hypothesis: "When X fails, the system should Y."</step>
    <step number="2">Select scenario from catalog above (or define new one).</step>
    <step number="3">Document: environment, blast radius, duration, rollback steps.</step>
    <step number="4">Verify guardrails: non-prod, monitoring active, team notified.</step>
    <step number="5">Establish baseline: capture current metrics before injection.</step>
    <step number="6">Inject fault using appropriate tooling.</step>
    <step number="7">Observe: check dashboards, logs, alerts, user-facing behavior.</step>
    <step number="8">Record findings: expected vs actual, surprises, gaps.</step>
    <step number="9">Rollback/recover and verify system returns to baseline.</step>
    <step number="10">Document learnings; create issues for gaps found.</step>
  </experiment-protocol>

  <output-format>
    <section name="Hypothesis">[When X fails, system should Y]</section>
    <section name="Environment">[Staging/QA, duration, blast radius]</section>
    <section name="Baseline Metrics">[Key metrics before injection]</section>
    <section name="Observations">[What actually happened]</section>
    <section name="Result">Pass (matched hypothesis) | Fail (unexpected behavior) | Partial</section>
    <section name="Gaps Found">[Issues, missing alerts, unexpected failures]</section>
    <section name="Actions">[Tickets created, fixes needed]</section>
  </output-format>

  <anti-patterns>
    <anti-pattern>Running chaos experiments without monitoring in place.</anti-pattern>
    <anti-pattern>Skipping rollback planning ("we'll figure it out").</anti-pattern>
    <anti-pattern>Testing in production without explicit approval and safeguards.</anti-pattern>
    <anti-pattern>Running experiments without hypothesis (random breaking).</anti-pattern>
    <anti-pattern>Not documenting findings or creating follow-up actions.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="release-readiness">Chaos results inform go/no-go decisions.</with-skill>
    <with-skill name="performance-budgeting">Validate performance under degraded conditions.</with-skill>
    <with-agent name="developer">Implements fixes for gaps discovered.</with-agent>
    <with-agent name="tester">May execute experiments as part of validation.</with-agent>
  </integration>

  <references>
    <reference>docs/deployment.md for environment details.</reference>
    <reference>docs/testing.md for testing scope.</reference>
  </references>
</skill>
