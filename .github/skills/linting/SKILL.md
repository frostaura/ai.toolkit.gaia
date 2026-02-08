---
name: linting
description: A skill for enforcing code quality standards through linting across multiple platforms and languages. This skill provides platform-agnostic guidance for configuring, running, and maintaining linters with zero-warning tolerance for .NET, JavaScript/TypeScript, Python, and other platforms.
---

<skill>
  <name>linting</name>
  <description>
  Platform-agnostic guidance for enforcing code quality standards through automated linting. This skill covers linter configuration, execution, zero-warning policies, and platform-specific tooling for maintaining consistent, high-quality code across technology stacks.
  </description>

  <linting-philosophy>
    <principle name="Zero Warnings">
      <value>Enforce zero-warning tolerance</value>
      <description>All warnings must be addressed before code is merged. Warnings indicate potential issues that should be fixed, not ignored.</description>
    </principle>
    <principle name="Consistency">
      <value>Maintain consistent code style across the codebase</value>
      <description>Linters ensure uniform formatting, naming conventions, and code structure that improves readability and maintainability.</description>
    </principle>
    <principle name="Automation">
      <value>Automate code quality checks</value>
      <description>Use linters as part of CI/CD pipelines to catch issues early and prevent low-quality code from reaching production.</description>
    </principle>
    <principle name="Prevention">
      <value>Catch bugs and code smells before runtime</value>
      <description>Linters detect potential bugs, security issues, and anti-patterns during development rather than at runtime.</description>
    </principle>
  </linting-philosophy>

  <linting-strategy>
    <category name="What to Lint">
      <area name="Code Style">
        <description>Formatting, indentation, spacing, line length, and visual consistency.</description>
      </area>
      <area name="Best Practices">
        <description>Language-specific idioms, design patterns, and recommended approaches.</description>
      </area>
      <area name="Error Prevention">
        <description>Unused variables, unreachable code, potential null references, and type errors.</description>
      </area>
      <area name="Security">
        <description>Unsafe patterns, exposed secrets, vulnerable dependencies, and security anti-patterns.</description>
      </area>
      <area name="Performance">
        <description>Inefficient algorithms, unnecessary computations, and performance anti-patterns.</description>
      </area>
      <area name="Maintainability">
        <description>Code complexity, naming conventions, documentation, and code smells.</description>
      </area>
    </category>
    <category name="When to Lint">
      <timing name="During Development">
        <description>Enable real-time linting in IDE/editor for immediate feedback as you type.</description>
      </timing>
      <timing name="Before Commit">
        <description>Run linters in pre-commit hooks to catch issues before they enter version control.</description>
      </timing>
      <timing name="In CI/CD Pipeline">
        <description>Run linters in automated pipelines to enforce quality gates before merge/deployment.</description>
      </timing>
      <timing name="On Demand">
        <description>Run linters manually when refactoring or reviewing code quality.</description>
      </timing>
    </category>
    <category name="Fix Strategy">
      <approach name="Auto-Fix">
        <description>Use auto-fix capabilities for formatting and simple style issues.</description>
        <caution>Review auto-fixes to ensure they don't change logic or introduce bugs.</caution>
      </approach>
      <approach name="Manual Fix">
        <description>Manually address complex warnings that require understanding context and intent.</description>
      </approach>
      <approach name="Suppression">
        <description>Suppress specific warnings only when absolutely necessary with clear justification.</description>
        <caution>Avoid suppressing warnings as a shortcut; fix the underlying issue instead.</caution>
      </approach>
    </category>
  </linting-strategy>

  <platform-implementations>
    <platform name=".NET">
      <linter name="dotnet format">
        <description>Built-in .NET code formatter and style analyzer.</description>
        <command name="Check">dotnet format --verify-no-changes</command>
        <command name="Fix">dotnet format</command>
        <configuration>.editorconfig, .globalconfig</configuration>
      </linter>
      <linter name="StyleCop Analyzers">
        <description>C# style and consistency analyzers.</description>
        <package>StyleCop.Analyzers NuGet package</package>
        <configuration>stylecop.json, .editorconfig</configuration>
      </linter>
      <linter name="Roslyn Analyzers">
        <description>Microsoft code quality and security analyzers.</description>
        <enablement>Enabled by default in .NET 5+</enablement>
        <configuration>.editorconfig, .globalconfig</configuration>
      </linter>
      <settings>
        <setting name="TreatWarningsAsErrors">
          <value>true</value>
          <description>Treat all warnings as errors to enforce zero-warning policy.</description>
          <location>csproj file: &lt;TreatWarningsAsErrors&gt;true&lt;/TreatWarningsAsErrors&gt;</location>
        </setting>
        <setting name="EnforceCodeStyleInBuild">
          <value>true</value>
          <description>Enable code style analysis during build.</description>
          <location>csproj file: &lt;EnforceCodeStyleInBuild&gt;true&lt;/EnforceCodeStyleInBuild&gt;</location>
        </setting>
      </settings>
    </platform>
    <platform name="JavaScript/TypeScript">
      <linter name="ESLint">
        <description>Pluggable linting utility for JavaScript and TypeScript.</description>
        <command name="Check">npm run lint or npx eslint .</command>
        <command name="Fix">npm run lint -- --fix or npx eslint . --fix</command>
        <configuration>.eslintrc.js, .eslintrc.json, eslint.config.js (flat config)</configuration>
      </linter>
      <linter name="TypeScript Compiler">
        <description>TypeScript static type checking.</description>
        <command name="Check">npx tsc --noEmit</command>
        <configuration>tsconfig.json</configuration>
      </linter>
      <linter name="Prettier">
        <description>Opinionated code formatter.</description>
        <command name="Check">npx prettier --check .</command>
        <command name="Fix">npx prettier --write .</command>
        <configuration>.prettierrc, prettier.config.js</configuration>
      </linter>
      <settings>
        <setting name="max-warnings">
          <value>0</value>
          <description>Enforce zero-warning policy in ESLint.</description>
          <usage>npm run lint -- --max-warnings 0</usage>
        </setting>
        <setting name="strict">
          <value>true</value>
          <description>Enable strict TypeScript checking.</description>
          <location>tsconfig.json: "strict": true</location>
        </setting>
      </settings>
      <integration>
        <tool name="eslint-config-prettier">
          <description>Disable ESLint formatting rules that conflict with Prettier.</description>
        </tool>
        <tool name="eslint-plugin-prettier">
          <description>Run Prettier as an ESLint rule.</description>
        </tool>
      </integration>
    </platform>
    <platform name="Python">
      <linter name="Ruff">
        <description>Fast Python linter written in Rust (modern choice).</description>
        <command name="Check">ruff check .</command>
        <command name="Fix">ruff check --fix .</command>
        <configuration>pyproject.toml, ruff.toml</configuration>
      </linter>
      <linter name="Flake8">
        <description>Popular Python linting tool (traditional choice).</description>
        <command name="Check">flake8 .</command>
        <configuration>.flake8, setup.cfg, tox.ini</configuration>
      </linter>
      <linter name="Pylint">
        <description>Comprehensive Python code analysis tool.</description>
        <command name="Check">pylint src/</command>
        <configuration>.pylintrc, pyproject.toml</configuration>
      </linter>
      <linter name="Black">
        <description>Uncompromising Python code formatter.</description>
        <command name="Check">black --check .</command>
        <command name="Fix">black .</command>
        <configuration>pyproject.toml</configuration>
      </linter>
      <linter name="mypy">
        <description>Static type checker for Python.</description>
        <command name="Check">mypy src/</command>
        <configuration>mypy.ini, pyproject.toml</configuration>
      </linter>
      <settings>
        <setting name="max-complexity">
          <description>Limit cyclomatic complexity (e.g., 10).</description>
          <location>Ruff: max-complexity = 10</location>
        </setting>
        <setting name="line-length">
          <description>Enforce maximum line length (e.g., 88 for Black, 100 for others).</description>
          <location>pyproject.toml: line-length = 88</location>
        </setting>
      </settings>
    </platform>
    <platform name="Java">
      <linter name="Checkstyle">
        <description>Code style checker for Java.</description>
        <command name="Check">mvn checkstyle:check or gradle checkstyleMain</command>
        <configuration>checkstyle.xml</configuration>
      </linter>
      <linter name="PMD">
        <description>Source code analyzer for Java.</description>
        <command name="Check">mvn pmd:check or gradle pmdMain</command>
        <configuration>pmd-ruleset.xml</configuration>
      </linter>
      <linter name="SpotBugs">
        <description>Static analysis tool to find bugs in Java code.</description>
        <command name="Check">mvn spotbugs:check or gradle spotbugsMain</command>
        <configuration>spotbugs-exclude.xml</configuration>
      </linter>
      <settings>
        <setting name="failOnViolation">
          <value>true</value>
          <description>Fail build on linting violations.</description>
        </setting>
      </settings>
    </platform>
    <platform name="Go">
      <linter name="golangci-lint">
        <description>Fast Go linters runner (combines multiple linters).</description>
        <command name="Check">golangci-lint run</command>
        <command name="Fix">golangci-lint run --fix</command>
        <configuration>.golangci.yml</configuration>
      </linter>
      <linter name="gofmt">
        <description>Official Go formatter.</description>
        <command name="Check">gofmt -l .</command>
        <command name="Fix">gofmt -w .</command>
      </linter>
      <settings>
        <setting name="enable-all">
          <description>Enable all available linters for maximum coverage.</description>
          <location>.golangci.yml: linters.enable-all: true</location>
        </setting>
      </settings>
    </platform>
  </platform-implementations>

  <best-practices>
    <category name="Configuration">
      <practice name="Start Strict">
        <description>Begin with strict rules and selectively relax them if truly necessary.</description>
      </practice>
      <practice name="Team Agreement">
        <description>Ensure the team agrees on linting rules to avoid conflicts and frustration.</description>
      </practice>
      <practice name="Version Control">
        <description>Commit linter configurations to version control for consistency across environments.</description>
      </practice>
      <practice name="Document Exceptions">
        <description>Document why specific rules are disabled or suppressed in comments or documentation.</description>
      </practice>
    </category>
    <category name="Execution">
      <practice name="Fast Feedback">
        <description>Run linters in watch mode or on file save for immediate feedback during development.</description>
      </practice>
      <practice name="Pre-Commit Hooks">
        <description>Use tools like husky, pre-commit, or lint-staged to run linters before commits.</description>
      </practice>
      <practice name="CI/CD Integration">
        <description>Run linters in CI/CD pipelines with exit code 1 on failure to block merges.</description>
      </practice>
      <practice name="Incremental Adoption">
        <description>When adding linters to legacy projects, fix existing issues incrementally or use baseline configs.</description>
      </practice>
    </category>
    <category name="Maintenance">
      <practice name="Keep Tools Updated">
        <description>Regularly update linters to benefit from new rules and bug fixes.</description>
      </practice>
      <practice name="Review New Rules">
        <description>When updating, review new rules and warnings; address them or explicitly disable if inappropriate.</description>
      </practice>
      <practice name="Monitor Rule Effectiveness">
        <description>Periodically review which rules catch real issues vs. noise; adjust configuration accordingly.</description>
      </practice>
    </category>
  </best-practices>

  <common-pitfalls>
    <pitfall name="Ignoring Warnings">
      <description>Allowing warnings to accumulate creates technical debt and hides real issues.</description>
      <solution>Enforce zero-warning policy; fix warnings immediately or suppress with justification.</solution>
    </pitfall>
    <pitfall name="Over-Configuration">
      <description>Excessive customization makes linter rules hard to understand and maintain.</description>
      <solution>Use standard configurations (e.g., eslint:recommended) as a base; customize minimally.</solution>
    </pitfall>
    <pitfall name="Inconsistent Environments">
      <description>Different linter versions or configurations across team members cause confusion.</description>
      <solution>Lock linter versions in package.json/requirements.txt, commit configurations to git.</solution>
    </pitfall>
    <pitfall name="Disabling Rules Without Reason">
      <description>Turning off rules to "make it work" undermines code quality.</description>
      <solution>Understand why a rule exists; fix the issue rather than disable the rule.</solution>
    </pitfall>
    <pitfall name="No Auto-Fix">
      <description>Manually fixing trivial formatting issues wastes time.</description>
      <solution>Use auto-fix capabilities for safe, mechanical changes; reserve manual effort for complex issues.</solution>
    </pitfall>
  </common-pitfalls>

  <execution-guidelines>
    <guideline name="Run Before Committing">
      <description>Always run linters before committing code to catch issues early.</description>
    </guideline>
    <guideline name="Fix First">
      <description>Use auto-fix to handle formatting and simple issues, then manually address complex warnings.</description>
    </guideline>
    <guideline name="Zero Tolerance">
      <description>Never commit code with linting errors or warnings; always achieve a clean exit code.</description>
    </guideline>
    <guideline name="Understand Warnings">
      <description>Read warning messages carefully to understand the issue before fixing or suppressing.</description>
    </guideline>
    <guideline name="Consistent Results">
      <description>Ensure linters produce the same results locally as in CI/CD pipelines.</description>
    </guideline>
  </execution-guidelines>

  <quality-gate-integration>
    <gate name="Lint Gate">
      <description>Binary pass/fail gate: exit code 0 = pass, non-zero = fail.</description>
      <enforcement>Block merges and deployments on linting failures.</enforcement>
      <reporting>Provide clear output showing which files/lines have issues and how to fix them.</reporting>
    </gate>
    <ci-configuration>
      <step name="Install Dependencies">
        <description>Install linter tools and dependencies as first step.</description>
      </step>
      <step name="Run Linter">
        <description>Execute linter with appropriate command for platform.</description>
      </step>
      <step name="Check Exit Code">
        <description>CI should fail build if linter exits with non-zero code.</description>
      </step>
      <step name="Report Results">
        <description>Display linter output in CI logs; optionally generate reports for dashboards.</description>
      </step>
    </ci-configuration>
  </quality-gate-integration>

  <ide-integration>
    <editor name="VS Code">
      <extension>ESLint, Pylance, C# Dev Kit, Prettier</extension>
      <configuration>settings.json: enable format on save, auto-fix on save</configuration>
    </editor>
    <editor name="JetBrains IDEs">
      <builtin>IntelliJ IDEA, PyCharm, Rider have built-in linters and formatters</builtin>
      <configuration>Enable inspections, configure code style, enable auto-format</configuration>
    </editor>
    <editor name="Vim/Neovim">
      <plugin>ALE, coc.nvim, null-ls for linter integration</plugin>
    </editor>
    <benefit>
      <description>IDE integration provides real-time feedback, reducing linting failures in CI/CD.</description>
    </benefit>
  </ide-integration>
</skill>
