---
name: strict-linting
description: Zero-warning linting configuration. Frontend uses ESLint --max-warnings 0 with TypeScript strict. Backend uses TreatWarningsAsErrors with StyleCop and Roslynator.
---

# Strict Linting

**Zero tolerance for warnings.** All warnings are errors.

## Frontend (React/TypeScript)

### ESLint Key Rules
```javascript
{
  "no-console": "error",
  "@typescript-eslint/no-unused-vars": "error",
  "@typescript-eslint/no-explicit-any": "error",
  "@typescript-eslint/explicit-function-return-type": "error",
  "react-hooks/rules-of-hooks": "error",
  "react-hooks/exhaustive-deps": "error"
}
```

### TypeScript Strict
```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true
  }
}
```

### Commands
```bash
npm run lint          # --max-warnings 0
npm run typecheck     # tsc --noEmit
npm run format:check  # prettier --check
npm run validate      # All above
```

## Backend (.NET)

### Directory.Build.props
```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <Nullable>enable</Nullable>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" />
  <PackageReference Include="Roslynator.Analyzers" />
  <PackageReference Include="SonarAnalyzer.CSharp" />
</ItemGroup>
```

### Commands
```bash
dotnet build --no-incremental
dotnet format --verify-no-changes --severity error
```

## Escape Hatches (Single Line Only)

### Frontend
```typescript
// eslint-disable-next-line @typescript-eslint/no-explicit-any -- API returns untyped
```

### Backend
```csharp
#pragma warning disable CA1062 // Validated by framework
#pragma warning restore CA1062
```

**Always include justification. Never disable globally.**
