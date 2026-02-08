---
name: unit-testing
description: A skill for writing comprehensive unit tests across multiple platforms and frameworks. This skill provides platform-agnostic guidance for achieving high test coverage, writing effective test cases, and following testing best practices for .NET (xUnit), JavaScript/TypeScript (Vitest, Jest), Python (pytest), and other platforms.
---

<skill>
  <name>unit-testing</name>
  <description>
  Platform-agnostic guidance for writing comprehensive unit tests targeting 100% coverage where possible. This skill covers test strategy, best practices, coverage requirements, and platform-specific execution details for multiple technology stacks.
  </description>

  <testing-philosophy>
    <principle name="Coverage Target">
      <value>Target 100% test coverage where possible</value>
      <description>Aim for complete coverage of all code paths, edge cases, and error scenarios. Use tiered minimums only when comprehensive coverage is impractical.</description>
    </principle>
    <principle name="Test Quality">
      <value>Tests should be maintainable, readable, and fast</value>
      <description>Write clear assertions, descriptive test names, and avoid brittle tests that break with refactoring.</description>
    </principle>
    <principle name="Test Independence">
      <value>Each test should be independent and isolated</value>
      <description>Tests should not depend on execution order, shared state, or external resources where avoidable.</description>
    </principle>
    <principle name="Meaningful Tests">
      <value>Test behavior, not implementation</value>
      <description>Focus on public APIs and observable behavior rather than internal implementation details.</description>
    </principle>
  </testing-philosophy>

  <coverage-tiers>
    <tier name="Trivial" target="Manual verification">
      <description>Simple scripts or prototypes where automated testing adds minimal value.</description>
      <example>Hello world apps, one-off scripts, static content</example>
    </tier>
    <tier name="Simple" target="50% on touched code (aim for 100%)">
      <description>Small projects or utilities with straightforward logic.</description>
      <example>CLI tools, simple APIs, basic utilities</example>
    </tier>
    <tier name="Standard" target="70% on touched code (aim for 100%)">
      <description>Typical applications with moderate complexity.</description>
      <example>Web applications, REST APIs, standard business logic</example>
    </tier>
    <tier name="Complex" target="80% all code (aim for 100%)">
      <description>Mission-critical applications requiring high reliability.</description>
      <example>Financial systems, healthcare apps, e-commerce platforms</example>
    </tier>
    <tier name="Enterprise" target="100% all code">
      <description>Systems where failures have severe consequences.</description>
      <example>Payment processing, medical devices, aviation software</example>
    </tier>
  </coverage-tiers>

  <test-strategy>
    <category name="What to Test">
      <area name="Happy Path">
        <description>Test the primary successful execution path with valid inputs.</description>
      </area>
      <area name="Edge Cases">
        <description>Test boundary conditions, empty inputs, null values, and extreme values.</description>
      </area>
      <area name="Error Paths">
        <description>Test error handling, exceptions, and failure scenarios.</description>
      </area>
      <area name="Business Logic">
        <description>Test all business rules, calculations, and decision logic.</description>
      </area>
      <area name="State Transitions">
        <description>Test state changes and transitions in stateful components.</description>
      </area>
      <area name="Integration Points">
        <description>Test interactions with dependencies (using mocks/stubs).</description>
      </area>
    </category>
    <category name="Test Structure">
      <pattern name="Arrange-Act-Assert">
        <description>Organize tests with clear setup, execution, and verification phases.</description>
        <arrange>Set up test data, mocks, and preconditions</arrange>
        <act>Execute the code under test</act>
        <assert>Verify the expected outcome</assert>
      </pattern>
      <pattern name="Given-When-Then">
        <description>BDD-style structure for scenario-based tests.</description>
        <given>Given a specific context or state</given>
        <when>When an action or event occurs</when>
        <then>Then expect a specific outcome</then>
      </pattern>
    </category>
    <category name="Naming Conventions">
      <convention name="Descriptive Names">
        <description>Test names should clearly describe what is being tested and expected outcome.</description>
        <example>should_return_user_when_valid_id_provided</example>
        <example>shouldThrowExceptionWhenEmailIsInvalid</example>
      </convention>
      <convention name="Test Organization">
        <description>Group related tests in test classes/suites that mirror the code structure.</description>
        <example>UserService tests → UserServiceTests class</example>
      </convention>
    </category>
  </test-strategy>

  <platform-implementations>
    <platform name=".NET">
      <framework>xUnit (recommended), NUnit, MSTest</framework>
      <command name="Run Tests">dotnet test</command>
      <command name="Coverage">dotnet test --collect:"XPlat Code Coverage"</command>
      <command name="Watch Mode">dotnet watch test</command>
      <attributes>
        <attribute>[Fact] - Single test</attribute>
        <attribute>[Theory] - Parameterized test</attribute>
        <attribute>[InlineData] - Test data</attribute>
      </attributes>
      <assertions>
        <library>Assert (xUnit), FluentAssertions (recommended)</library>
        <example>Assert.Equal(expected, actual)</example>
        <example>result.Should().Be(expected)</example>
      </assertions>
      <mocking>
        <library>Moq, NSubstitute, FakeItEasy</library>
        <example>var mock = new Mock&lt;IUserService&gt;()</example>
      </mocking>
    </platform>
    <platform name="JavaScript/TypeScript">
      <framework>Vitest (recommended), Jest, Mocha</framework>
      <command name="Run Tests">npm test</command>
      <command name="Coverage">npm test -- --coverage</command>
      <command name="Watch Mode">npm test -- --watch</command>
      <functions>
        <function>describe() - Test suite</function>
        <function>it() or test() - Individual test</function>
        <function>beforeEach() - Setup</function>
        <function>afterEach() - Teardown</function>
      </functions>
      <assertions>
        <library>expect (built-in)</library>
        <example>expect(result).toBe(expected)</example>
        <example>expect(fn).toThrow()</example>
        <example>expect(result).toEqual(expected)</example>
      </assertions>
      <mocking>
        <library>vi.mock() for Vitest, jest.mock() for Jest</library>
        <example>vi.mock('./userService')</example>
        <example>const spy = vi.fn()</example>
      </mocking>
    </platform>
    <platform name="Python">
      <framework>pytest (recommended), unittest</framework>
      <command name="Run Tests">pytest</command>
      <command name="Coverage">pytest --cov=src --cov-report=html</command>
      <command name="Watch Mode">pytest-watch or ptw</command>
      <decorators>
        <decorator>@pytest.fixture - Test fixtures</decorator>
        <decorator>@pytest.mark.parametrize - Parameterized tests</decorator>
      </decorators>
      <assertions>
        <library>assert (built-in)</library>
        <example>assert result == expected</example>
        <example>assert len(items) > 0</example>
      </assertions>
      <mocking>
        <library>unittest.mock, pytest-mock</library>
        <example>from unittest.mock import Mock, patch</example>
      </mocking>
    </platform>
    <platform name="Java">
      <framework>JUnit 5 (recommended), TestNG</framework>
      <command name="Run Tests">mvn test or gradle test</command>
      <command name="Coverage">mvn test jacoco:report</command>
      <annotations>
        <annotation>@Test - Test method</annotation>
        <annotation>@BeforeEach - Setup</annotation>
        <annotation>@ParameterizedTest - Parameterized test</annotation>
      </annotations>
      <assertions>
        <library>JUnit assertions, AssertJ (recommended)</library>
        <example>assertEquals(expected, actual)</example>
        <example>assertThat(result).isEqualTo(expected)</example>
      </assertions>
      <mocking>
        <library>Mockito, EasyMock</library>
        <example>@Mock private UserService userService;</example>
      </mocking>
    </platform>
  </platform-implementations>

  <best-practices>
    <category name="Test Design">
      <practice name="Single Responsibility">
        <description>Each test should verify one specific behavior or outcome.</description>
      </practice>
      <practice name="Fast Execution">
        <description>Tests should run quickly (&lt; 1s per test ideally) to enable rapid feedback.</description>
      </practice>
      <practice name="Deterministic">
        <description>Tests should produce the same result every time, avoid random data or timing dependencies.</description>
      </practice>
      <practice name="Self-Validating">
        <description>Tests should clearly pass or fail without manual inspection.</description>
      </practice>
      <practice name="Avoid Test Logic">
        <description>Tests should not contain complex logic, conditionals, or loops.</description>
      </practice>
    </category>
    <category name="Mocking Strategy">
      <practice name="Mock External Dependencies">
        <description>Mock databases, APIs, file systems, and external services.</description>
      </practice>
      <practice name="Don't Over-Mock">
        <description>Avoid mocking internal implementation details; test real behavior when possible.</description>
      </practice>
      <practice name="Use Test Doubles Appropriately">
        <description>Choose mocks, stubs, spies, or fakes based on testing needs.</description>
      </practice>
    </category>
    <category name="Test Maintenance">
      <practice name="DRY Principle">
        <description>Extract common setup into fixtures, helpers, or factory functions.</description>
      </practice>
      <practice name="Avoid Brittle Tests">
        <description>Don't test internal implementation; focus on public contracts and behavior.</description>
      </practice>
      <practice name="Keep Tests Updated">
        <description>Update tests when requirements change; failing tests should indicate real issues.</description>
      </practice>
    </category>
  </best-practices>

  <common-pitfalls>
    <pitfall name="Testing Implementation">
      <description>Testing how code works instead of what it does makes tests brittle and hard to maintain.</description>
      <solution>Focus on public APIs and observable outcomes, not internal methods or state.</solution>
    </pitfall>
    <pitfall name="Shared State">
      <description>Tests that depend on shared state or execution order are fragile and hard to debug.</description>
      <solution>Ensure each test is isolated with proper setup and teardown.</solution>
    </pitfall>
    <pitfall name="Slow Tests">
      <description>Slow test suites discourage running tests frequently, delaying bug discovery.</description>
      <solution>Mock external dependencies, avoid database/network calls, optimize test data.</solution>
    </pitfall>
    <pitfall name="Insufficient Coverage">
      <description>Missing tests for edge cases and error paths leaves code vulnerable to bugs.</description>
      <solution>Use coverage tools to identify gaps, write tests for all code paths.</solution>
    </pitfall>
    <pitfall name="Over-Mocking">
      <description>Excessive mocking can make tests test the mocks rather than real behavior.</description>
      <solution>Mock only external dependencies, use real implementations for internal code.</solution>
    </pitfall>
  </common-pitfalls>

  <execution-guidelines>
    <guideline name="Run Tests Frequently">
      <description>Run tests after every change to catch issues early.</description>
    </guideline>
    <guideline name="Fix Failing Tests Immediately">
      <description>Never commit code with failing tests; fix or skip them explicitly.</description>
    </guideline>
    <guideline name="Monitor Coverage Trends">
      <description>Track coverage over time; ensure it increases or stays high, never decreases.</description>
    </guideline>
    <guideline name="Review Test Output">
      <description>Read test failure messages carefully; they should clearly indicate what went wrong.</description>
    </guideline>
    <guideline name="Use Watch Mode During Development">
      <description>Enable watch mode to automatically re-run tests as you code.</description>
    </guideline>
  </execution-guidelines>

  <coverage-analysis>
    <metric name="Line Coverage">
      <description>Percentage of code lines executed by tests.</description>
    </metric>
    <metric name="Branch Coverage">
      <description>Percentage of decision branches (if/else) covered by tests.</description>
    </metric>
    <metric name="Function Coverage">
      <description>Percentage of functions/methods invoked by tests.</description>
    </metric>
    <metric name="Statement Coverage">
      <description>Percentage of statements executed by tests.</description>
    </metric>
    <interpretation>
      <note>100% coverage does not guarantee bug-free code, but lack of coverage guarantees untested code.</note>
      <note>Focus on meaningful coverage - test logic, not trivial code like getters/setters.</note>
      <note>Use coverage reports to identify gaps, then write targeted tests.</note>
    </interpretation>
  </coverage-analysis>
</skill>
