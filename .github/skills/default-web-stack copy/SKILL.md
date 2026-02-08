---
name: default-web-stack
description: Default web stack for projects. This dictates which technologies to leverage across projects, unless specified otherwise by the user. This includes backend, frontend, database, security, and so on. When no defaults are provided in this skill and a decision is made, the architect should update this skill accordingly.
---

<skill>
  <name>default-web-stack</name>
  <description>
  Default web stack for projects. This dictates which technologies to leverage across projects, unless specified otherwise by the user. This includes backend, frontend, database, security, and testing technologies, as well as architectural principles.
  </description>
  <web-stack>
    <category name="Backend">
      <technology name="Framework">
        <value>ASP.NET Core (.NET 10+)</value>
        <description>Primary backend framework providing web API capabilities, dependency injection, and middleware pipeline.</description>
      </technology>
      <technology name="ORM">
        <value>Entity Framework Core</value>
        <description>Object-relational mapper for database access with code-first migrations and LINQ support.</description>
      </technology>
      <technology name="Architecture">
        <value>iDesign Architecture</value>
        <description>Separates concerns into layers: Managers, Engines, Data for maintainability and testability.</description>
      </technology>
      <technology name="Linting">
        <value>StyleCop + Roslynator + SonarAnalyzer</value>
        <description>Code quality analyzers enforcing coding standards with TreatWarningsAsErrors enabled.</description>
      </technology>
    </category>
    <category name="Frontend">
      <technology name="Framework">
        <value>React 18+ with TypeScript 5+</value>
        <description>Modern UI library with hooks, concurrent features, and strict type safety.</description>
      </technology>
      <technology name="State">
        <value>Redux Toolkit + RTK Query</value>
        <description>State management with built-in best practices and efficient data fetching capabilities.</description>
      </technology>
      <technology name="Linting">
        <value>ESLint (strict) + Prettier</value>
        <description>Code quality and formatting tools with --max-warnings 0 enforcement and TypeScript strict mode.</description>
      </technology>
    </category>
    <category name="Database">
      <technology name="Primary">
        <value>PostgreSQL 15+</value>
        <description>Advanced open-source relational database with strong ACID compliance and extensive feature set.</description>
      </technology>
      <technology name="ORM">
        <value>EF Core with migrations</value>
        <description>Entity Framework Core for schema management with code-first migrations.</description>
      </technology>
      <technology name="Caching">
        <value>Redis</value>
        <description>In-memory data structure store for caching and session management.</description>
      </technology>
    </category>
    <category name="Security">
      <technology name="Auth">
        <value>JWT (15min access, 7day refresh)</value>
        <description>JSON Web Tokens for stateless authentication with short-lived access tokens and longer refresh tokens.</description>
      </technology>
      <technology name="Storage">
        <value>httpOnly cookies preferred</value>
        <description>Secure token storage using httpOnly cookies to prevent XSS attacks.</description>
      </technology>
      <technology name="Access">
        <value>Role-based (RBAC)</value>
        <description>Role-based access control for authorization and permission management.</description>
      </technology>
      <technology name="Dev Admin">
        <value>admin@system.local / Admin123!</value>
        <description>Default development administrator credentials for local testing only.</description>
      </technology>
    </category>
    <category name="Testing">
      <technology name="E2E/Visual">
        <value>Playwright MCP tools</value>
        <description>End-to-end and visual regression testing using Playwright for comprehensive UI validation.</description>
      </technology>
      <technology name="Unit">
        <value>xUnit (.NET), Vitest (React)</value>
        <description>Unit testing frameworks for backend and frontend with fast execution and modern features.</description>
      </technology>
      <technology name="Regression">
        <value>PlayWright spec files & CULR tests</value>
        <description>Regression testing using Playwright specification files and CULR tests to cover all use cases, CRUD operations and user flows.</description>
      </technology>
      <technology name="Coverage">
        <value>Tiered by complexity</value>
        <description>Coverage requirements adjusted based on project complexity and risk assessment. 100% coverage is preferred for unit tests and use cases.</description>
      </technology>
    </category>
    <category name="Architecture Principles">
      <principle name="iDesign">
        <description>Service-oriented component design methodology for scalable and maintainable architecture.</description>
      </principle>
      <principle name="API Design">
        <description>RESTful conventions with proper HTTP methods, status codes, and resource naming.</description>
      </principle>
      <principle name="Error Handling">
        <description>Structured error responses with consistent format, appropriate status codes, and actionable messages.</description>
      </principle>
    </category>
  </web-stack>
</skill>
