---
name: database-migrations
description: A skill for managing database schema changes using Entity Framework Code First migrations. Ensures production databases always reflect the migration history with no manual schema changes.
---

<skill>
  <name>database-migrations</name>
  <description>
  EF Core Code First migration management. Architect owns migration creation; developers modify entities. Production must match migration history exactly — no manual DDL.
  </description>

  <principles>
    <principle>**Migrations are source of truth** — production schema must match migration history exactly.</principle>
    <principle>**Code First always** — schema changes start in C# entities, never directly in the database.</principle>
    <principle>**Architect owns migrations** — developers modify entities; architect generates and reviews migrations.</principle>
    <principle>**No manual DDL** — never run ALTER/CREATE/DROP in production outside migrations.</principle>
    <principle>**Reversibility** — every migration needs a working Down(). Document if rollback isn't possible.</principle>
  </principles>

  <workflow>
    <step owner="Developer">Modify entity classes and DbContext configuration.</step>
    <step owner="Developer">Request migration from architect with change summary.</step>
    <step owner="Architect">Review entity changes against design docs.</step>
    <step owner="Architect">Generate: `dotnet ef migrations add [Name] --project [DbProject]`</step>
    <step owner="Architect">Review generated migration for correctness/safety.</step>
    <step owner="Architect">Test: apply (`dotnet ef database update`) and rollback (`dotnet ef database update [Previous]`).</step>
    <step owner="Architect">Commit migration files.</step>
    <step owner="Developer">Apply in CI/CD pipeline or deployment.</step>
  </workflow>

  <naming>
    <format>[YYYYMMDD]_[DescriptiveAction] (PascalCase)</format>
    <examples>20260209_AddUserEmailIndex, 20260209_CreateOrdersTable, 20260209_RemoveDeprecatedUserFields</examples>
    <rule>Be specific: Add/Create/Remove/Rename/Alter + affected table/entity.</rule>
  </naming>

  <commands>
    <command name="Add">dotnet ef migrations add [Name] --project [DbProject] --startup-project [WebProject]</command>
    <command name="Apply">dotnet ef database update --project [DbProject] --startup-project [WebProject]</command>
    <command name="Rollback">dotnet ef database update [TargetMigration] --project [DbProject]</command>
    <command name="Script">dotnet ef migrations script [From] [To] --idempotent --output migration.sql</command>
    <command name="Remove">dotnet ef migrations remove --project [DbProject] (only if unapplied)</command>
    <command name="List">dotnet ef migrations list --project [DbProject]</command>
  </commands>

  <safety-checklist>
    <check>Up() correctly implements change; Down() correctly reverses it.</check>
    <check>Default values for new non-nullable columns.</check>
    <check>Data migrations separate from schema migrations.</check>
    <check>Indexes on FKs and frequently queried columns.</check>
    <check>Cascade delete rules intentional and documented.</check>
    <check>Tested with realistic data volumes; rollback plan ready.</check>
  </safety-checklist>

  <dangerous-operations>
    <op name="Drop columns/tables">Risk: data loss. Mitigation: backup or soft-delete first; stage over releases.</op>
    <op name="Rename columns/tables">Risk: EF generates drop+create. Mitigation: use RenameColumn()/RenameTable() explicitly.</op>
    <op name="Change column types">Risk: data truncation. Mitigation: test with prod-like data; multi-step migration.</op>
    <op name="Add non-nullable column">Risk: fails on existing rows. Mitigation: add nullable → populate → alter to non-nullable.</op>
  </dangerous-operations>

  <production-deployment>
    <rule>Generate idempotent SQL script: `dotnet ef migrations script --idempotent`.</rule>
    <rule>DBA/senior dev review before execution. Maintenance window for breaking changes.</rule>
    <rule>Always have rollback script ready. Monitor logs after migration.</rule>
    <rule>Never use `dotnet ef database update` directly in production.</rule>
  </production-deployment>

  <anti-patterns>
    <anti-pattern>Manual schema changes in production.</anti-pattern>
    <anti-pattern>Editing applied migrations — create new migration instead.</anti-pattern>
    <anti-pattern>Empty Down() — always implement rollback.</anti-pattern>
    <anti-pattern>Mixing data seeding with schema changes — keep separate.</anti-pattern>
    <anti-pattern>Giant migrations — break into reviewable chunks.</anti-pattern>
    <anti-pattern>Developer creating migrations — architect creates/reviews.</anti-pattern>
  </anti-patterns>

  <references>
    <reference>docs/data.md — database schema design</reference>
    <reference>Microsoft EF Core Migrations documentation</reference>
  </references>
</skill>
