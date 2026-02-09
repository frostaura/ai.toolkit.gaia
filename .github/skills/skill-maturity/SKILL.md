---
name: skill-maturity
description: A skill for assessing skill coverage, identifying gaps, and prioritizing new skills to add.
---

<skill>
  <name>skill-maturity</name>
  <description>
  Keeps capability growth intentional by assessing current skills, identifying gaps, and prioritizing additions. Prevents ad-hoc skill sprawl while ensuring the system evolves with project needs.
  </description>

  <assessment-dimensions>
    <dimension name="Coverage">
      <question>What percentage of recurring agent tasks have a supporting skill?</question>
      <scoring>0-40% = Critical gap, 41-70% = Moderate gap, 71-90% = Good, 91-100% = Excellent</scoring>
      <method>List top 20 recurring tasks across agents; count how many have dedicated skill guidance.</method>
    </dimension>
    <dimension name="Depth">
      <question>Do existing skills provide actionable steps, not just descriptions?</question>
      <scoring>Thin (<50 lines, no steps) = Weak, Adequate (50-100 lines, some steps) = Fair, Dense (100-150 lines, full protocol) = Strong</scoring>
      <method>Sample 5 skills; rate each for actionability, specificity, and anti-pattern coverage.</method>
    </dimension>
    <dimension name="Alignment">
      <question>Do skills match current tech stack, workflows, and project goals?</question>
      <scoring>Outdated/misaligned = Weak, Partially aligned = Fair, Fully aligned = Strong</scoring>
      <method>Compare skill content against default-web-stack and repository-structure; flag mismatches.</method>
    </dimension>
    <dimension name="Consistency">
      <question>Do skills follow a uniform structure and quality bar?</question>
      <scoring>Inconsistent formats = Weak, Mostly consistent = Fair, Uniform structure = Strong</scoring>
      <method>Compare skill file structures; identify outliers in format, depth, or naming.</method>
    </dimension>
    <dimension name="Integration">
      <question>Do skills reference each other and connect to agents appropriately?</question>
      <scoring>Isolated skills = Weak, Some cross-refs = Fair, Well-integrated = Strong</scoring>
      <method>Check for integration or references sections; verify links are valid.</method>
    </dimension>
  </assessment-dimensions>

  <gap-identification>
    <sources>
      <source>Self-improvement logs: recurring friction pointing to missing guidance.</source>
      <source>Agent failures: tasks where agents struggled due to lack of skill support.</source>
      <source>New project needs: tech or workflow changes requiring new capabilities.</source>
      <source>Industry standards: common practices not yet codified (observability, accessibility, etc.).</source>
    </sources>
    <prioritization-criteria>
      <criterion name="Frequency">How often is this capability needed? (Daily/Weekly/Monthly/Rarely)</criterion>
      <criterion name="Impact">What's the cost of not having it? (Blocks work / Slows work / Minor friction)</criterion>
      <criterion name="Effort">How hard is it to create? (S: 1-2h, M: 2-4h, L: 4h+)</criterion>
      <criterion name="Dependencies">Does it require other skills or changes first?</criterion>
    </prioritization-criteria>
    <priority-formula>Priority = (Frequency × Impact) / Effort. Adjust for dependencies.</priority-formula>
  </gap-identification>

  <skill-quality-checklist>
    <item>Has clear description (2-3 sentences, not vague).</item>
    <item>Defines philosophy/principles (why, not just what).</item>
    <item>Provides actionable steps or protocols (how).</item>
    <item>Includes anti-patterns (what not to do).</item>
    <item>Lists success metrics (how to measure).</item>
    <item>Integrates with related skills and agents.</item>
    <item>References relevant docs where applicable.</item>
    <item>100-150 lines of dense, useful content.</item>
    <item>No placeholders, TODOs, or thin sections.</item>
  </skill-quality-checklist>

  <assessment-protocol>
    <step number="1">Inventory: List all skills in .github/skills/; note line counts and structure.</step>
    <step number="2">Score: Rate each dimension (Coverage, Depth, Alignment, Consistency, Integration) 1-5.</step>
    <step number="3">Gaps: Identify missing skills from sources; apply prioritization criteria.</step>
    <step number="4">Quality: Run checklist against each existing skill; flag those needing improvement.</step>
    <step number="5">Recommend: Propose top 3 new skills and top 3 skills needing enhancement.</step>
    <step number="6">Report: Summarize findings with scores, gaps, and recommendations.</step>
  </assessment-protocol>

  <output-format>
    <section name="Summary">Overall maturity score (1-5) with brief rationale.</section>
    <section name="Dimension Scores">Table of dimensions with scores and notes.</section>
    <section name="Gap Analysis">Prioritized list of missing skills with Frequency/Impact/Effort.</section>
    <section name="Enhancement Needs">Existing skills requiring depth or alignment fixes.</section>
    <section name="Recommendations">Top 3 new skills to add, top 3 existing to enhance.</section>
  </output-format>

  <cadence>
    <regular>Run full assessment quarterly or after major project changes.</regular>
    <triggered>Run partial assessment when self-improvement logs show recurring skill gaps.</triggered>
    <ad-hoc>Run before adding new skills to check for overlap or sequencing issues.</ad-hoc>
  </cadence>

  <anti-patterns>
    <anti-pattern>Adding skills reactively without assessing existing coverage first.</anti-pattern>
    <anti-pattern>Creating thin placeholder skills instead of dense, actionable ones.</anti-pattern>
    <anti-pattern>Duplicating guidance already in another skill or agent definition.</anti-pattern>
    <anti-pattern>Ignoring skill quality in favor of quantity.</anti-pattern>
    <anti-pattern>Never revisiting or enhancing existing skills as needs evolve.</anti-pattern>
  </anti-patterns>

  <integration>
    <with-skill name="self-improvement">Friction logs feed gap identification.</with-skill>
    <with-skill name="documentation">New skills may require doc updates via architect.</with-skill>
    <with-agent name="architect">Architect approves new skill proposals and structural changes.</with-agent>
    <with-agent name="orchestrator">Orchestrator routes skill gaps to appropriate owners.</with-agent>
  </integration>
</skill>
