---
name: unit-testing
description: A skill for writing comprehensive unit tests across multiple platforms and frameworks. This skill provides platform-agnostic guidance for achieving high test coverage, writing effective test cases, and following testing best practices for .NET (xUnit), JavaScript/TypeScript (Vitest, Jest), Python (pytest), and other platforms.
---

<skill>
  <name>unit-testing</name>
  <description>
  Platform-agnostic guidance for writing comprehensive unit tests targeting 100% coverage. Covers test strategy, patterns, coverage tiers, and platform-specific execution.
  </description>

  <principles>
    <principle>**Target 100% coverage** — all code paths, edge cases, error scenarios. Use tiered minimums when impractical.</principle>
    <principle>**Test behavior, not implementation** — focus on public APIs and observable outcomes.</principle>
    <principle>**Independent and isolated** — no shared state, no execution order dependency.</principle>
    <principle>**Fast and deterministic** — &lt;1s per test, no random data or timing dependencies.</principle>
  </principles>

  <coverage-tiers>
    <tier name="Trivial" target="Manual verification">Simple scripts, prototypes, static content</tier>
    <tier name="Simple" target="50% touched (aim 100%)">CLI tools, simple APIs, basic utilities</tier>
    <tier name="Standard" target="70% touched (aim 100%)">Web apps, REST APIs, business logic</tier>
    <tier name="Complex" target="80% all code (aim 100%)">Financial, healthcare, e-commerce</tier>
    <tier name="Enterprise" target="100% all code">Payment processing, medical, aviation</tier>
  </coverage-tiers>

  <what-to-test>
    <area>Happy path — primary successful execution with valid inputs</area>
    <area>Edge cases — boundaries, empty/null inputs, extreme values</area>
    <area>Error paths — exception handling, failure scenarios</area>
    <area>Business logic — rules, calculations, decision logic</area>
    <area>State transitions — state changes in stateful components</area>
    <area>Integration points — dependency interactions (via mocks/stubs)</area>
  </what-to-test>

  <test-structure>
    <pattern>**Arrange-Act-Assert** — setup → execute → verify (preferred)</pattern>
    <pattern>**Given-When-Then** — BDD-style for scenario-based tests</pattern>
    <naming>Descriptive names: `should_return_user_when_valid_id_provided`</naming>
    <organization>Mirror code structure: UserService → UserServiceTests</organization>
  </test-structure>

  <platforms>
    <platform name=".NET">
      <framework>xUnit (recommended) — [Fact], [Theory], [InlineData]</framework>
      <run>dotnet test</run>
      <coverage>dotnet test --collect:"XPlat Code Coverage"</coverage>
      <assertions>Assert (built-in), FluentAssertions (recommended)</assertions>
      <mocking>Moq, NSubstitute</mocking>
    </platform>
    <platform name="JavaScript/TypeScript">
      <framework>Vitest (recommended) — describe(), it(), beforeEach()</framework>
      <run>npm test</run>
      <coverage>npm test -- --coverage</coverage>
      <assertions>expect (built-in) — .toBe(), .toEqual(), .toThrow()</assertions>
      <mocking>vi.mock(), vi.fn()</mocking>
    </platform>
    <platform name="Python">
      <framework>pytest (recommended) — @pytest.fixture, @pytest.mark.parametrize</framework>
      <run>pytest</run>
      <coverage>pytest --cov=src --cov-report=html</coverage>
      <assertions>assert (built-in)</assertions>
      <mocking>unittest.mock, pytest-mock</mocking>
    </platform>
    <platform name="Java">
      <framework>JUnit 5 — @Test, @BeforeEach, @ParameterizedTest</framework>
      <run>mvn test</run>
      <coverage>mvn test jacoco:report</coverage>
      <assertions>JUnit assertions, AssertJ (recommended)</assertions>
      <mocking>Mockito</mocking>
    </platform>
  </platforms>

  <best-practices>
    <practice>**Single responsibility** — each test verifies one specific behavior.</practice>
    <practice>**No test logic** — avoid conditionals, loops, or complex logic in tests.</practice>
    <practice>**Mock external dependencies** — databases, APIs, file systems. Don't over-mock internals.</practice>
    <practice>**DRY via fixtures** — extract common setup into helpers/factories.</practice>
    <practice>**Run frequently** — after every change; use watch mode during development.</practice>
    <practice>**Fix immediately** — never commit failing tests; coverage must never decrease.</practice>
  </best-practices>

  <anti-patterns>
    <anti-pattern>Testing implementation details — makes tests brittle on refactoring.</anti-pattern>
    <anti-pattern>Shared state between tests — fragile, order-dependent, hard to debug.</anti-pattern>
    <anti-pattern>Slow tests (network/DB calls) — mock external dependencies for speed.</anti-pattern>
    <anti-pattern>Over-mocking — tests end up testing mocks, not real behavior.</anti-pattern>
    <anti-pattern>Ignoring coverage gaps — use reports to find and fill untested paths.</anti-pattern>
  </anti-patterns>

  <coverage-metrics>
    <metric>Line coverage — % of code lines executed</metric>
    <metric>Branch coverage — % of decision branches (if/else) covered</metric>
    <metric>Function coverage — % of functions invoked</metric>
    <note>100% coverage ≠ bug-free, but 0% coverage = untested. Focus on meaningful logic, not trivial getters.</note>
  </coverage-metrics>
</skill>
