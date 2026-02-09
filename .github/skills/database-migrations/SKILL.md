---
name: database-migrations
description: A skill for managing database schema changes using Entity Framework Code First migrations. Ensures production databases always reflect the migration history with no manual schema changes.
---

<skill>
  <name>database-migrations</name>
  <description>
  Manages database schema evolution using EF Core Code First migrations. The architect owns all migration creation; developers implement entity changes. Production databases must always match migration history—no manual DDL, no schema drift.
  </description>

  <philosophy>
    <principle name="Migrations Are Source of Truth">The migration history is the authoritative record of database schema. Production must match migrations exactly.</principle>
    <principle name="Code First Always">Schema changes start in C# entities, never directly in the database. Migrations are generated from entity changes.</principle>
    <principle name="Architect Owns Migrations">Only the architect creates migration files. Developers modify entities; architect generates and reviews migrations.</principle>
    <principle name="No Manual DDL">Never run manual ALTER/CREATE/DROP statements in production. All changes go through migrations.</principle>
    <principle name="Reversibility">Every migration should have a working Down() method. If a rollback isn't possible, document why.</principle>
  </philosophy>

  <workflow>
    <step number="1" owner="Developer">Modify entity classes (add/remove properties, relationships, constraints).</step>
    <step number="2" owner="Developer">Update DbContext configuration if needed (fluent API, indexes, etc.).</step>
    <step number="3" owner="Developer">Request migration from architect with summary of entity changes.</step>
    <step number="4" owner="Architect">Review entity changes against design docs.</step>
    <step number="5" owner="Architect">Generate migration: `dotnet ef migrations add [MigrationName] --project [DbProject]`.</step>
    <step number="6" owner="Architect">Review generated migration for correctness and safety.</step>
    <step number="7" owner="Architect">Test migration locally: `dotnet ef database update`.</step>
    <step number="8" owner="Architect">Test rollback: `dotnet ef database update [PreviousMigration]`.</step>
    <step number="9" owner="Architect">Commit migration files to version control.</step>
    <step number="10" owner="Developer">Apply migration in CI/CD pipeline or deployment process.</step>
  </workflow>

  <naming-conventions>
    <format>[YYYYMMDD]_[DescriptiveAction]</format>
    <examples>
      <example>20260209_AddUserEmailIndex</example>
      <example>20260209_CreateOrdersTable</example>
      <example>20260209_AddProductCategoryRelationship</example>
      <example>20260209_RemoveDeprecatedUserFields</example>
    </examples>
    <rules>
      <rule>Use PascalCase for migration names.</rule>
      <rule>Start with date prefix for chronological ordering.</rule>
      <rule>Be specific about what changes (Add, Create, Remove, Rename, Alter).</rule>
      <rule>Reference the affected table or entity in the name.</rule>
    </rules>
  </naming-conventions>

  <migration-commands>
    <command name="Add Migration">
      <syntax>dotnet ef migrations add [Name] --project [DbProject] --startup-project [WebProject]</syntax>
      <when>After entity changes are complete and reviewed.</when>
    </command>
    <command name="Apply Migration">
      <syntax>dotnet ef database update --project [DbProject] --startup-project [WebProject]</syntax>
      <when>To apply pending migrations to local database.</when>
    </command>
    <command name="Rollback Migration">
      <syntax>dotnet ef database update [TargetMigration] --project [DbProject]</syntax>
      <when>To revert to a specific migration state.</when>
    </command>
    <command name="Generate SQL Script">
      <syntax>dotnet ef migrations script [From] [To] --idempotent --output migration.sql</syntax>
      <when>For production deployments or DBA review. Always use --idempotent.</when>
    </command>
    <command name="Remove Last Migration">
      <syntax>dotnet ef migrations remove --project [DbProject]</syntax>
      <when>Only if migration hasn't been applied. Never remove applied migrations.</when>
    </command>
    <command name="List Migrations">
      <syntax>dotnet ef migrations list --project [DbProject]</syntax>
      <when>To see pending and applied migrations.</when>
    </command>
  </migration-commands>

  <safety-checklist>
    <item>Does the Up() method correctly implement the schema change?</item>
    <item>Does the Down() method correctly reverse the change?</item>
    <item>Are default values provided for new non-nullable columns?</item>
    <item>Are data migrations handled separately from schema migrations?</item>
    <item>Is the migration idempotent (safe to run multiple times)?</item>
    <item>Are indexes created for foreign keys and frequently queried columns?</item>
    <item>Are cascade delete rules intentional and documented?</item>
    <item>Has the migration been tested with realistic data volumes?</item>
    <item>Is there a rollback plan if the migration fails in production?</item>
  </safety-checklist>

  <dangerous-operations>
    <operation name="Dropping Columns/Tables">
      <risk>Data loss if column contains data.</risk>
      <mitigation>Create backup migration or soft-delete first. Stage over multiple releases.</mitigation>
    </operation>
    <operation name="Renaming Columns/Tables">
      <risk>EF may generate drop+create instead of rename.</risk>
      <mitigation>Use migrationBuilder.RenameColumn() or RenameTable() explicitly.</mitigation>
    </operation>
    <operation name="Changing Column Types">
      <risk>Data truncation or conversion errors.</risk>
      <mitigation>Test with production-like data. Consider multi-step migration.</mitigation>
    </operation>
    <operation name="Adding Non-Nullable Column">
      <risk>Fails if existing rows have no value.</risk>
      <mitigation>Add as nullable, populate default, then alter to non-nullable.</mitigation>
    </operation>
    <operation name="Modifying Primary Keys">
      <risk>Breaks foreign key relationships.</risk>
      <mitigation>Coordinate with all dependent tables. Usually requires multi-step approach.</mitigation>
    </operation>
  </dangerous-operations>

  <production-deployment>
    <rule>Generate SQL script for production: `dotnet ef migrations script --idempotent`.</rule>
    <rule>Have DBA or senior developer review script before execution.</rule>
    <rule>Run during maintenance window for breaking changes.</rule>
    <rule>Always have rollback script ready (Down migration or backup restore).</rule>
    <rule>Monitor application logs immediately after migration.</rule>
    <rule>Never apply migrations directly via `dotnet ef database update` in production.</rule>
  </production-deployment>

  <anti-patterns>
    <anti-pattern name="Manual Schema Changes">Running DDL directly in production. All changes must go through migrations.</anti-pattern>
    <anti-pattern name="Editing Applied Migrations">Modifying migration files after they've been applied. Create new migration instead.</anti-pattern>
    <anti-pattern name="Skipping Down Method">Leaving Down() empty. Always implement rollback logic.</anti-pattern>
    <anti-pattern name="Data and Schema in One Migration">Mixing data seeding with schema changes. Keep them separate.</anti-pattern>
    <anti-pattern name="Giant Migrations">One migration with dozens of changes. Break into logical, reviewable chunks.</anti-pattern>
    <anti-pattern name="Developer Creates Migrations">Developers should change entities; architect creates and reviews migrations.</anti-pattern>
    <anti-pattern name="Ignoring Migration Conflicts">Multiple developers creating migrations simultaneously. Coordinate to avoid merge issues.</anti-pattern>
  </anti-patterns>

  <success-metrics>
    <metric>Production database schema matches latest migration snapshot exactly.</metric>
    <metric>All migrations have working Down() methods tested locally.</metric>
    <metric>Zero manual DDL statements in production history.</metric>
    <metric>Migration reviews catch issues before production deployment.</metric>
    <metric>Rollback can be executed within 5 minutes if needed.</metric>
  </success-metrics>

  <integration>
    <with-agent name="architect">Architect creates all migrations after reviewing entity changes.</with-agent>
    <with-agent name="developer">Developer modifies entities and requests migration generation.</with-agent>
    <with-skill name="spec-consistency">Schema changes must align with data.md design docs.</with-skill>
    <with-skill name="release-readiness">Migration deployment is part of release checklist.</with-skill>
    <with-skill name="architecture-decision-records">Major schema changes may warrant ADR.</with-skill>
  </integration>

  <references>
    <reference>docs/data.md for database schema design.</reference>
    <reference>Microsoft EF Core Migrations documentation.</reference>
  </references>
</skill>
