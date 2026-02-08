---
name: web-research
description: Two-tier web research using web fetching as primary for known URLs and headless browser automation as fallback for searches. Requires 3+ sources, prioritizes official docs, includes versions/dates.
---

<skill>
  <name>web-research</name>
  <description>
  A skill for conducting thorough web research using a two-tier approach: web fetching as the primary tool for known URLs, and headless browser automation as a fallback for searches and dynamic content. This skill ensures research quality through multiple sources, prioritizes official documentation, and includes versioning and publication dates.
  </description>
  <research-approach>
    <tier name="Primary" tool="Web Fetching">
      <use-case>Known URLs (official docs, blogs, GitHub, API references)</use-case>
      <description>Fast and efficient for direct URL access without JavaScript rendering requirements.</description>
      <example>Use web fetching with URLs array and query string for known documentation URLs</example>
      <strengths>
        <strength>Fast execution</strength>
        <strength>Efficient for static content</strength>
        <strength>Ideal for known documentation URLs</strength>
      </strengths>
      <limitations>
        <limitation>No JavaScript rendering</limitation>
        <limitation>Cannot perform searches</limitation>
      </limitations>
    </tier>
    <tier name="Fallback" tool="Browser Automation">
      <use-case>Search queries and dynamic content requiring JavaScript rendering</use-case>
      <description>Headless browser automation for complex interactions and search engine queries.</description>
      <example>Use browser automation tools for navigation, element interaction, keyboard input, and content extraction in interactive searches</example>
      <mode>Headless (no visible browser window)</mode>
      <strengths>
        <strength>Full JavaScript support</strength>
        <strength>Search engine integration</strength>
        <strength>Dynamic content handling</strength>
      </strengths>
    </tier>
  </research-approach>
  <quality-standards>
    <standard name="Sources">
      <requirement>Minimum 3 sources for all claims</requirement>
      <priority>Official documentation preferred over third-party sources</priority>
    </standard>
    <standard name="Versioning">
      <requirement>Include version numbers for all technologies</requirement>
      <format>v[X.Y.Z] or [Major].[Minor]</format>
    </standard>
    <standard name="Dates">
      <requirement>Include publication or last-updated dates</requirement>
      <format>ISO 8601 or readable format (e.g., "January 2026")</format>
    </standard>
    <standard name="Caching">
      <requirement>Cache findings using memory storage for future reference</requirement>
      <scope>ProjectWide for architectural decisions</scope>
    </standard>
  </quality-standards>
  <output-format>
    <template>Research report includes topic, recommendation with version and date, bulleted key findings, and full source URLs</template>
    <requirements>
      <requirement>Clear topic identification</requirement>
      <requirement>Explicit recommendation with version</requirement>
      <requirement>Bulleted key findings</requirement>
      <requirement>Full source URLs</requirement>
    </requirements>
  </output-format>
  <memory-integration>
    <workflow>
      <step name="Before Research">
        <action>Recall topic from memory</action>
        <purpose>Retrieve existing knowledge to avoid redundant research</purpose>
      </step>
      <step name="After Research">
        <action>Store research findings with sources as ProjectWide memory</action>
        <purpose>Cache findings for future reference and consistency</purpose>
      </step>
    </workflow>
  </memory-integration>
  <rules>
    <required>
      <rule>Try web fetching first for known URLs</rule>
      <rule>Cite all sources with full URLs</rule>
      <rule>Cache key findings via memory integration</rule>
    </required>
    <forbidden>
      <rule>Do not skip to browser automation for known URLs</rule>
      <rule>Do not present unverified information</rule>
      <rule>Do not cite fewer than 3 sources</rule>
    </forbidden>
  </rules>
</skill>
