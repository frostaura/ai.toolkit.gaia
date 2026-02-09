---
name: self-improvement
description: A skill for continuous improvement by capturing friction, proposing changes, and verifying impact using Gaia improvement tools.
---

<skill>
  <name>self-improvement</name>
  <description>
  Institutionalizes learning by capturing friction, proposing experiments, and measuring impact. Agents must use Gaia improvement tools exclusively—no temporary files, no informal notes. Small wins compound into significant velocity gains.
  </description>

  <philosophy>
    <principle name="Tool Mandate">MUST use Gaia improvement tools (log_frustration, propose_improvement, verify_impact). No docs/, no scratch files, no TODOs.</principle>
    <principle name="Atomic Changes">One friction point → one proposed fix → one measurable outcome. Never bundle unrelated improvements.</principle>
    <principle name="Close the Loop">Every logged frustration requires a follow-up within 5 sessions to verify if the change helped or needs iteration.</principle>
    <principle name="Blame Process, Not People">Focus on systemic issues (tooling, docs, patterns) rather than individual mistakes.</principle>
  </philosophy>

  <friction-detection>
    <trigger name="Repetition">Same issue encountered twice in one session or three times across sessions.</trigger>
    <trigger name="Workarounds">Manual steps required to complete what should be automated or streamlined.</trigger>
    <trigger name="Context Loss">Information had to be re-discovered because it wasn't captured or was hard to find.</trigger>
    <trigger name="Tooling Gaps">Missing capability forced fallback to slower or less reliable approach.</trigger>
    <trigger name="Unclear Guidance">Ambiguity in skill, agent, or doc caused decision paralysis or wrong path.</trigger>
    <severity-threshold>Log if issue cost >5 minutes or will recur. Skip trivial one-offs.</severity-threshold>
  </friction-detection>

  <logging-protocol>
    <step number="1">Identify the friction using triggers above.</step>
    <step number="2">Log via Gaia tool: category (tooling|process|docs|skill|agent), description, impact estimate.</step>
    <step number="3">Propose concrete fix: what changes, who owns it, success criteria, effort estimate (S/M/L).</step>
    <step number="4">Tag with priority: P1 (blocks work), P2 (slows work), P3 (annoyance).</step>
    <format>
      Category: [tooling|process|docs|skill|agent]
      Friction: [What happened, why it hurt]
      Impact: [Time lost, risk introduced, or frequency]
      Proposed Fix: [Specific change]
      Owner: [architect|developer|orchestrator|self]
      Success Criteria: [How we know it worked]
      Effort: [S: <1h, M: 1-4h, L: >4h]
      Priority: [P1|P2|P3]
    </format>
  </logging-protocol>

  <verification-protocol>
    <timing>Review logged items every 5 sessions or when similar friction recurs.</timing>
    <questions>
      <question>Did the proposed fix get implemented?</question>
      <question>Did it reduce or eliminate the friction?</question>
      <question>Any unintended side effects?</question>
      <question>Should we iterate, close, or escalate?</question>
    </questions>
    <outcomes>
      <outcome name="Verified">Fix worked, close the loop, optionally share as pattern.</outcome>
      <outcome name="Partial">Some improvement, propose iteration.</outcome>
      <outcome name="Failed">No improvement, analyze root cause, try different approach.</outcome>
      <outcome name="Superseded">Problem no longer relevant, close without action.</outcome>
    </outcomes>
  </verification-protocol>

  <escalation-rules>
    <rule>P1 items: escalate to orchestrator immediately if not resolved in current session.</rule>
    <rule>Recurring P2 items (3+ occurrences): escalate to architect for systemic fix.</rule>
    <rule>Skill/agent gaps: propose skill update or new skill via skill-maturity process.</rule>
    <rule>Cross-agent friction: orchestrator coordinates resolution across agents.</rule>
  </escalation-rules>

  <anti-patterns>
    <anti-pattern name="Silent Struggling">Working around issues without logging them. Always log—even if you fix it yourself.</anti-pattern>
    <anti-pattern name="Vague Proposals">"Improve the docs" is not actionable. Specify which doc, what change, why.</anti-pattern>
    <anti-pattern name="Over-Engineering">Proposing L-effort fixes for P3 annoyances. Match effort to impact.</anti-pattern>
    <anti-pattern name="Orphaned Logs">Logging without follow-up. The loop must close.</anti-pattern>
    <anti-pattern name="File-Based Tracking">Creating TODO.md, NOTES.md, or similar. Use Gaia tools only.</anti-pattern>
  </anti-patterns>

  <success-metrics>
    <metric>Friction logged within session of occurrence (no backlog drift).</metric>
    <metric>80%+ of logged items have verified outcomes within 10 sessions.</metric>
    <metric>Recurring friction decreases over time (measured by duplicate log entries).</metric>
    <metric>Zero temporary tracking files in repository.</metric>
    <metric>Improvement proposals are specific, actionable, and appropriately sized.</metric>
  </success-metrics>

  <integration>
    <with-skill name="memory">Store successful fixes as patterns for future recall.</with-skill>
    <with-skill name="skill-maturity">Skill gaps identified here feed into maturity assessments.</with-skill>
    <with-agent name="orchestrator">P1 escalations route through orchestrator for prioritization.</with-agent>
    <with-agent name="architect">Systemic process/doc issues route to architect for resolution.</with-agent>
  </integration>
</skill>
