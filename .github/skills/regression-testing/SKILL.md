---
name: regression-testing
description: A skill for providing agent guidance on performing manual regression testing using browser automation tools. The agent must test all pages at 4 breakpoints, all interactive states, and monitor console for errors. Use interactive testing tools only - NOT npm/npx commands or spec files for manual regression.
---

<skill>
  <name>regression-testing</name>
  <description>
  Agent guidance for manual regression testing via interactive browser automation tools. Test across breakpoints and states, capture evidence, report findings. No spec files or npx commands for manual regression.
  </description>

  <tool-rules>
    <rule>**MUST** use interactive browser automation tools (e.g. Playwright MCP) for all test interactions.</rule>
    <rule>**MUST NOT** run npm/npx browser automation commands or create/execute spec files.</rule>
    <rule>Run headed mode locally; headless mode in CI — but always use interactive tools.</rule>
  </tool-rules>

  <testing-scope>
    <rule>Test ALL affected features and pages.</rule>
    <rule>Test at ALL breakpoints: 320px (mobile), 768px (tablet), 1024px (desktop), 1440px+ (large).</rule>
    <rule>Test ALL interactive states: default, hover, focus, active, disabled, loading, error.</rule>
    <rule>Monitor and report ALL console errors, warnings, and failed network requests.</rule>
  </testing-scope>

  <functional-testing>
    <step>Navigate to target page/component using browser automation tools.</step>
    <step>Interact with elements — click buttons, type inputs, select dropdowns, trigger behaviors.</step>
    <step>Verify expected behavior — correct results, state changes, navigation.</step>
    <step>Check console for errors/warnings.</step>
    <step>Test edge cases — error states, form validation, boundary conditions.</step>
  </functional-testing>

  <visual-testing>
    <breakpoint name="Mobile (320px)">Touch targets, readability, mobile layouts. Capture full-page screenshot.</breakpoint>
    <breakpoint name="Tablet (768px)">Medium layouts, touch interactions, responsive transitions. Capture screenshot.</breakpoint>
    <breakpoint name="Desktop (1024px)">Standard layouts, hover states, typical viewport. Capture screenshot.</breakpoint>
    <breakpoint name="Large (1440px+)">Max-width constraints, spacing, wide layouts. Capture screenshot.</breakpoint>
  </visual-testing>

  <validation-checklist>
    <functional>
      <check>Navigation works between pages/views</check>
      <check>Forms submit with valid data; validation catches invalid data</check>
      <check>Data displays correctly from API responses</check>
      <check>Buttons/links respond to interaction</check>
      <check>Error states show correct messages and styling</check>
      <check>Loading states display during async operations</check>
      <check>Auth/authorization works as expected</check>
      <check>No console errors or warnings</check>
    </functional>
    <visual>
      <check>Screenshots captured at all 4 breakpoints</check>
      <check>No layout breaks or visual glitches</check>
      <check>Text readable at all breakpoints</check>
      <check>Spacing and alignment consistent</check>
    </visual>
  </validation-checklist>

  <collaboration>
    <rule>Share test results (pass/fail, features tested, anomalies) with Architect.</rule>
    <rule>Provide screenshots and console logs as evidence.</rule>
    <rule>Report bugs with clear reproduction steps — Architect documents formally.</rule>
    <rule>Do NOT create formal documentation directly — delegate to Architect.</rule>
  </collaboration>

  <references>
    <reference>docs/use-cases.md — required regression scenarios</reference>
    <reference>docs/frontend.md — UI patterns and expected interactions</reference>
  </references>
</skill>
