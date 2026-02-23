---
name: linting
description: A skill for enforcing code quality standards through linting across multiple platforms and languages. This skill provides platform-agnostic guidance for configuring, running, and maintaining linters with zero-warning tolerance for .NET, JavaScript/TypeScript, Python, and other platforms.
---

<skill>
  <name>linting</name>
  <description>
  Platform-agnostic guidance for enforcing code quality through automated linting. Covers zero-warning policies, platform tooling, and CI integration.
  </description>

  <principles>
    <principle>**Zero warnings** — all warnings addressed before merge. TreatWarningsAsErrors / --max-warnings 0.</principle>
    <principle>**Consistency** — uniform style, naming, and structure across the codebase.</principle>
    <principle>**Automation** — lint in IDE, pre-commit hooks, and CI/CD pipelines.</principle>
    <principle>**Prevention** — catch bugs, security issues, and anti-patterns at dev time, not runtime.</principle>
  </principles>

  <what-to-lint>
    <area>Code style — formatting, indentation, spacing, line length</area>
    <area>Best practices — language idioms, design patterns</area>
    <area>Error prevention — unused vars, null refs, type errors, unreachable code</area>
    <area>Security — unsafe patterns, exposed secrets, vulnerable dependencies</area>
    <area>Maintainability — complexity, naming, code smells</area>
  </what-to-lint>

  <when-to-lint>
    <timing>During development — real-time IDE feedback</timing>
    <timing>Before commit — pre-commit hooks (husky, lint-staged, pre-commit)</timing>
    <timing>In CI/CD — quality gate blocking merge on failure</timing>
  </when-to-lint>

  <fix-strategy>
    <approach>**Auto-fix first** — use for formatting and simple style issues. Review changes for correctness.</approach>
    <approach>**Manual fix** — for complex warnings requiring context understanding.</approach>
    <approach>**Suppress only with justification** — document why; fix underlying issue when possible.</approach>
  </fix-strategy>

  <platforms>
    <platform name=".NET">
      <tools>dotnet format, StyleCop Analyzers, Roslyn Analyzers, SonarAnalyzer</tools>
      <check>dotnet format --verify-no-changes</check>
      <fix>dotnet format</fix>
      <config>.editorconfig, .globalconfig, stylecop.json</config>
      <key-settings>TreatWarningsAsErrors=true, EnforceCodeStyleInBuild=true in csproj</key-settings>
    </platform>
    <platform name="JavaScript/TypeScript">
      <tools>ESLint (strict), Prettier, TypeScript compiler</tools>
      <check>npx eslint . --max-warnings 0 &amp;&amp; npx prettier --check . &amp;&amp; npx tsc --noEmit</check>
      <fix>npx eslint . --fix &amp;&amp; npx prettier --write .</fix>
      <config>eslint.config.js, .prettierrc, tsconfig.json (strict: true)</config>
      <integration>eslint-config-prettier to avoid rule conflicts</integration>
    </platform>
    <platform name="Python">
      <tools>Ruff (preferred), Black, mypy</tools>
      <check>ruff check . &amp;&amp; black --check . &amp;&amp; mypy src/</check>
      <fix>ruff check --fix . &amp;&amp; black .</fix>
      <config>pyproject.toml, ruff.toml</config>
    </platform>
    <platform name="Go">
      <tools>golangci-lint, gofmt</tools>
      <check>golangci-lint run &amp;&amp; gofmt -l .</check>
      <fix>golangci-lint run --fix &amp;&amp; gofmt -w .</fix>
      <config>.golangci.yml</config>
    </platform>
    <platform name="Java">
      <tools>Checkstyle, PMD, SpotBugs</tools>
      <check>mvn checkstyle:check pmd:check spotbugs:check</check>
      <config>checkstyle.xml, pmd-ruleset.xml</config>
      <key-settings>failOnViolation=true</key-settings>
    </platform>
  </platforms>

  <best-practices>
    <practice>**Start strict** — begin with recommended configs; relax selectively with documented reasons.</practice>
    <practice>**Lock versions** — pin linter versions in package.json/requirements.txt for consistency.</practice>
    <practice>**Commit configs** — version control all linter configurations.</practice>
    <practice>**Incremental adoption** — for legacy projects, fix issues incrementally or use baseline.</practice>
    <practice>**Keep updated** — regularly update linters; review new rules when upgrading.</practice>
  </best-practices>

  <ci-integration>
    <step>Install dependencies → run linter → check exit code (0=pass) → block merge on failure.</step>
    <rule>Ensure local and CI results are identical (same versions, same configs).</rule>
  </ci-integration>

  <anti-patterns>
    <anti-pattern>Accumulating warnings — enforce zero-warning from day one.</anti-pattern>
    <anti-pattern>Over-customizing — use standard configs as base, customize minimally.</anti-pattern>
    <anti-pattern>Disabling rules without reason — understand the rule before suppressing.</anti-pattern>
    <anti-pattern>Skipping auto-fix — use it for mechanical changes, save manual effort for complex issues.</anti-pattern>
  </anti-patterns>
</skill>
