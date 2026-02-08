---
name: regression-testing
description: A skill for providing agent guidance on performing manual regression testing using browser automation tools. The agent must test all pages at 4 breakpoints, all interactive states, and monitor console for errors. Use interactive testing tools only - NOT npm/npx commands or spec files for manual regression.
---

<skill>
  <name>regression-testing</name>
  <description>
  Agent guidance for performing manual regression testing using browser automation tools directly. The agent must conduct interactive testing across multiple breakpoints and states, verify functional behavior, capture visual evidence, and report console errors. This skill defines the agent's responsibility for comprehensive manual regression validation without using automated spec files or npx commands.
  </description>

  <agent-responsibilities>
    <category name="Tool Usage">
      <requirement name="Interactive Testing Tools">
        <value>MUST use interactive browser automation tools directly</value>
        <description>Use browser automation tools for all test interactions and navigation.</description>
      </requirement>
      <requirement name="No NPM Commands">
        <value>MUST NOT run npm/npx browser automation commands</value>
        <description>Avoid automated test execution via command line.</description>
      </requirement>
      <requirement name="No Spec Files">
        <value>MUST NOT create or execute spec files for regression testing</value>
        <description>Manual regression requires interactive testing, not automated test scripts.</description>
      </requirement>
      <requirement name="No Command Line Automation">
        <value>MUST NOT run browser automation via command line or npx. Only use interactive tools.</value>
        <description>Prohibit use of command line or npx for browser automation; enforce exclusive use of interactive tools for manual regression testing.
        When running locally, run tests in headed mode. When not possible like in the build pipeline or a headless environment, run in headless mode but still use interactive tools for test execution and reporting.</description>
      </requirement>
      <rationale>
        <description>Manual regression requires direct interaction through browser automation tools to validate UI behavior interactively, not automated test suites.</description>
      </rationale>
    </category>
    <category name="Testing Scope">
      <requirement name="Features">
        <value>Test ALL affected features and pages</value>
        <description>Ensure complete coverage of changed functionality.</description>
      </requirement>
      <requirement name="Breakpoints">
        <value>Test at ALL required breakpoints</value>
        <description>Validate responsive behavior at 320px, 768px, 1024px, and 1440px+.</description>
      </requirement>
      <requirement name="States">
        <value>Test ALL interactive states for each element</value>
        <description>Verify default, hover, focus, active, disabled, loading, and error states.</description>
      </requirement>
      <requirement name="Console">
        <value>Monitor and report ALL console errors</value>
        <description>Track JavaScript errors, warnings, and failed network requests.</description>
      </requirement>
      <rationale>
        <description>Comprehensive coverage ensures visual and functional regressions are detected across all user scenarios.</description>
      </rationale>
    </category>
    <category name="Reporting & Collaboration">
      <requirement name="Collaborate with Architect">
        <value>MUST collaborate with Architect for all documentation</value>
        <description>Report test findings, screenshots, and issues to Architect who will handle formal documentation.</description>
      </requirement>
      <requirement name="Provide Test Evidence">
        <value>Capture screenshots and console logs as evidence</value>
        <description>Collect visual evidence and error logs during testing to share with architect for documentation purposes.</description>
      </requirement>
      <requirement name="Communicate Findings">
        <value>Report test results and issues to Architect</value>
        <description>Share pass/fail status, visual discrepancies, console errors, and reproduction steps with Architect.</description>
      </requirement>
      <requirement name="No Direct Documentation">
        <value>MUST NOT create formal documentation directly</value>
        <description>Avoid creating design docs, test reports, or formal documentation. Delegate to Architect instead.</description>
      </requirement>
      <rationale>
        <description>Separation of concerns ensures testing agent focuses on validation while architect maintains comprehensive documentation.</description>
      </rationale>
    </category>
  </agent-responsibilities>

  <testing-methodology>
    <category name="Functional Testing">
      <step name="Navigate">
        <value>Navigate to page/component</value>
        <description>Use MCP navigation tools to reach the target feature or page being tested.</description>
      </step>
      <step name="Interact">
        <value>Interact with elements</value>
        <description>Click buttons, type in inputs, select dropdowns, and trigger all interactive behaviors.</description>
      </step>
      <step name="Verify">
        <value>Verify expected behavior</value>
        <description>Confirm that each interaction produces the correct result, state change, or navigation.</description>
      </step>
      <step name="Console">
        <value>Check for console errors</value>
        <description>Monitor browser console for JavaScript errors, warnings, or failed network requests.</description>
      </step>
      <step name="Edge Cases">
        <value>Test error states</value>
        <description>Validate error messages, form validation, edge cases, and boundary conditions.</description>
      </step>
    </category>
    <category name="Visual Testing">
      <breakpoint name="Mobile">
        <value>320px</value>
        <description>Small phones - test touch targets, readability, and mobile-specific layouts. Capture full-page screenshot.</description>
      </breakpoint>
      <breakpoint name="Tablet">
        <value>768px</value>
        <description>Tablets/iPad - test medium layouts, touch interactions, and responsive transitions. Capture full-page screenshot.</description>
      </breakpoint>
      <breakpoint name="Desktop">
        <value>1024px</value>
        <description>Laptops - test standard desktop layouts, hover states, and typical user viewport. Capture full-page screenshot.</description>
      </breakpoint>
      <breakpoint name="Large">
        <value>1440px+</value>
        <description>Large monitors - test max-width constraints, spacing, and wide-screen layouts. Capture full-page screenshot.</description>
      </breakpoint>
    </category>
    <category name="Interactive States">
      <state name="Default">
        <value>Normal appearance</value>
        <description>Normal appearance of the element in its resting state. Verify styling, positioning, and content display correctly.</description>
      </state>
      <state name="Hover">
        <value>Mouse over</value>
        <description>Visual feedback when mouse cursor is over the element. Trigger hover and verify color, border, shadow, or other visual changes.</description>
      </state>
      <state name="Focus">
        <value>Keyboard focus</value>
        <description>Accessibility indication when element receives keyboard focus. Tab to element and verify focus ring or outline is visible.</description>
      </state>
      <state name="Active">
        <value>Being clicked</value>
        <description>Visual feedback during the moment of click or interaction. Click/touch and verify pressed state styling appears.</description>
      </state>
      <state name="Disabled">
        <value>Not interactive</value>
        <description>Non-interactive state preventing user interaction. Verify disabled styling and ensure element cannot be interacted with.</description>
      </state>
      <state name="Loading">
        <value>Async operation</value>
        <description>Indication that an asynchronous operation is in progress. Trigger async action and verify spinner, skeleton, or loading indicator appears.</description>
      </state>
      <state name="Error">
        <value>Validation/error</value>
        <description>Visual indication of validation failure or error condition. Trigger error condition and verify error styling and helpful message displays.</description>
      </state>
    </category>
  </testing-methodology>

  <validation-checklist>
    <category name="Functional Checks">
      <check name="Navigation">
        <description>Navigation works correctly between pages and views</description>
      </check>
      <check name="Forms">
        <description>Forms submit successfully with valid data</description>
      </check>
      <check name="Data Display">
        <description>Data displays properly from API responses</description>
      </check>
      <check name="Interactions">
        <description>Buttons and links respond to user interaction</description>
      </check>
      <check name="Error States">
        <description>Error states show correct messages and styling</description>
      </check>
      <check name="Console">
        <description>No console errors or warnings appear</description>
      </check>
      <check name="Loading States">
        <description>Loading states display during async operations</description>
      </check>
      <check name="Auth">
        <description>Authentication and authorization work as expected</description>
      </check>
    </category>
    <category name="Visual Checks">
      <check name="Mobile Screenshot">
        <description>Screenshot captured at 320px (Mobile)</description>
      </check>
      <check name="Tablet Screenshot">
        <description>Screenshot captured at 768px (Tablet)</description>
      </check>
      <check name="Desktop Screenshot">
        <description>Screenshot captured at 1024px (Desktop)</description>
      </check>
      <check name="Large Screenshot">
        <description>Screenshot captured at 1440px+ (Large)</description>
      </check>
      <check name="Baseline Comparison">
        <description>Compare screenshots with baseline or design specifications</description>
      </check>
      <check name="Layout Integrity">
        <description>Verify no layout breaks or visual glitches</description>
      </check>
      <check name="Readability">
        <description>Confirm text is readable at all breakpoints</description>
      </check>
      <check name="Spacing">
        <description>Validate spacing and alignment consistency</description>
      </check>
    </category>
  </validation-checklist>

  <collaboration-guidelines>
    <category name="Architect Collaboration">
      <guideline name="Test Findings">
        <description>Share test results with Architect including pass/fail status, specific features tested, and any anomalies observed.</description>
      </guideline>
      <guideline name="Visual Evidence">
        <description>Provide screenshots captured at each breakpoint to Architect for inclusion in documentation or issue tracking.</description>
      </guideline>
      <guideline name="Console Errors">
        <description>Report all console errors, warnings, and failed network requests to Architect with context about when they occurred.</description>
      </guideline>
      <guideline name="Reproduction Steps">
        <description>When bugs are found, communicate clear reproduction steps to Architect who will document them appropriately.</description>
      </guideline>
      <guideline name="Documentation Requests">
        <description>If documentation is needed, request Architect to create or update relevant documents rather than doing it directly.</description>
      </guideline>
    </category>
  </collaboration-guidelines>
</skill>
