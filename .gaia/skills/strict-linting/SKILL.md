---
name: strict-linting
description: Guide for enforcing strict linting that fails builds on any violation. Zero warnings allowed. Use when setting up new projects or validating code quality.
---

# Strict Linting Configuration

## Philosophy

**Zero tolerance for lint violations.** All warnings are errors. Builds MUST fail on any lint issue.

## Frontend (React/TypeScript)

### ESLint Configuration

Create `.eslintrc.cjs` in frontend root:

```javascript
module.exports = {
  root: true,
  env: { browser: true, es2020: true, node: true },
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/strict-type-checked',
    'plugin:@typescript-eslint/stylistic-type-checked',
    'plugin:react/recommended',
    'plugin:react/jsx-runtime',
    'plugin:react-hooks/recommended',
    'plugin:jsx-a11y/strict',
    'plugin:import/recommended',
    'plugin:import/typescript',
    'prettier',
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs', 'vite.config.ts', 'coverage'],
  parser: '@typescript-eslint/parser',
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
    project: ['./tsconfig.json', './tsconfig.node.json'],
    tsconfigRootDir: __dirname,
  },
  plugins: ['react-refresh', '@typescript-eslint', 'import'],
  settings: {
    react: { version: 'detect' },
    'import/resolver': {
      typescript: { alwaysTryTypes: true },
    },
  },
  rules: {
    // STRICT: All warnings become errors
    'no-console': 'error',
    'no-debugger': 'error',
    'no-alert': 'error',
    'no-var': 'error',
    'prefer-const': 'error',
    'no-unused-vars': 'off',
    '@typescript-eslint/no-unused-vars': ['error', { argsIgnorePattern: '^_' }],
    '@typescript-eslint/explicit-function-return-type': 'error',
    '@typescript-eslint/no-explicit-any': 'error',
    '@typescript-eslint/no-non-null-assertion': 'error',
    '@typescript-eslint/strict-boolean-expressions': 'error',
    '@typescript-eslint/no-floating-promises': 'error',
    '@typescript-eslint/await-thenable': 'error',
    '@typescript-eslint/no-misused-promises': 'error',
    '@typescript-eslint/consistent-type-imports': 'error',
    '@typescript-eslint/consistent-type-definitions': ['error', 'interface'],
    '@typescript-eslint/naming-convention': [
      'error',
      { selector: 'interface', format: ['PascalCase'] },
      { selector: 'typeAlias', format: ['PascalCase'] },
      { selector: 'enum', format: ['PascalCase'] },
      { selector: 'enumMember', format: ['UPPER_CASE'] },
      { selector: 'variable', format: ['camelCase', 'UPPER_CASE', 'PascalCase'] },
      { selector: 'function', format: ['camelCase', 'PascalCase'] },
      { selector: 'parameter', format: ['camelCase'], leadingUnderscore: 'allow' },
    ],
    
    // React strict rules
    'react/prop-types': 'off',
    'react/jsx-no-leaked-render': 'error',
    'react/jsx-curly-brace-presence': ['error', { props: 'never', children: 'never' }],
    'react/self-closing-comp': 'error',
    'react/jsx-boolean-value': ['error', 'never'],
    'react-refresh/only-export-components': ['error', { allowConstantExport: true }],
    'react-hooks/rules-of-hooks': 'error',
    'react-hooks/exhaustive-deps': 'error',
    
    // Import organization
    'import/order': [
      'error',
      {
        groups: ['builtin', 'external', 'internal', 'parent', 'sibling', 'index'],
        'newlines-between': 'always',
        alphabetize: { order: 'asc', caseInsensitive: true },
      },
    ],
    'import/no-duplicates': 'error',
    'import/no-unresolved': 'error',
    'import/no-cycle': 'error',
    
    // Accessibility
    'jsx-a11y/alt-text': 'error',
    'jsx-a11y/anchor-has-content': 'error',
    'jsx-a11y/click-events-have-key-events': 'error',
    'jsx-a11y/no-static-element-interactions': 'error',
  },
};
```

### Prettier Configuration

Create `.prettierrc` in frontend root:

```json
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": true,
  "printWidth": 100,
  "tabWidth": 2,
  "useTabs": false,
  "bracketSpacing": true,
  "bracketSameLine": false,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

### TypeScript Configuration (tsconfig.json strict settings)

Ensure these are in `tsconfig.json`:

```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "strictFunctionTypes": true,
    "strictBindCallApply": true,
    "strictPropertyInitialization": true,
    "noImplicitThis": true,
    "useUnknownInCatchVariables": true,
    "alwaysStrict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "exactOptionalPropertyTypes": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true,
    "noUncheckedIndexedAccess": true,
    "noImplicitOverride": true,
    "noPropertyAccessFromIndexSignature": true,
    "forceConsistentCasingInFileNames": true
  }
}
```

### Package.json Scripts

```json
{
  "scripts": {
    "lint": "eslint . --max-warnings 0",
    "lint:fix": "eslint . --fix --max-warnings 0",
    "format": "prettier --write \"src/**/*.{ts,tsx,css,json}\"",
    "format:check": "prettier --check \"src/**/*.{ts,tsx,css,json}\"",
    "typecheck": "tsc --noEmit",
    "validate": "npm run typecheck && npm run lint && npm run format:check",
    "build": "npm run validate && vite build"
  }
}
```

### Required Dependencies

```bash
npm install -D eslint @typescript-eslint/eslint-plugin @typescript-eslint/parser \
  eslint-plugin-react eslint-plugin-react-hooks eslint-plugin-react-refresh \
  eslint-plugin-jsx-a11y eslint-plugin-import eslint-config-prettier prettier \
  eslint-import-resolver-typescript
```

## Backend (.NET)

### .editorconfig (Root of solution)

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.{cs,csx}]
# .NET naming conventions
dotnet_naming_rule.public_members_must_be_pascal_case.symbols = public_symbols
dotnet_naming_rule.public_members_must_be_pascal_case.style = pascal_case_style
dotnet_naming_rule.public_members_must_be_pascal_case.severity = error

dotnet_naming_rule.private_fields_must_be_camel_case.symbols = private_fields
dotnet_naming_rule.private_fields_must_be_camel_case.style = camel_case_underscore_style
dotnet_naming_rule.private_fields_must_be_camel_case.severity = error

dotnet_naming_symbols.public_symbols.applicable_kinds = property,method,field,event,delegate
dotnet_naming_symbols.public_symbols.applicable_accessibilities = public,internal,protected

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.pascal_case_style.capitalization = pascal_case
dotnet_naming_style.camel_case_underscore_style.capitalization = camel_case
dotnet_naming_style.camel_case_underscore_style.required_prefix = _

# Code style
csharp_style_var_for_built_in_types = true:error
csharp_style_var_when_type_is_apparent = true:error
csharp_style_var_elsewhere = true:error
csharp_prefer_braces = true:error
csharp_style_expression_bodied_methods = when_on_single_line:error
csharp_style_expression_bodied_properties = true:error
csharp_style_expression_bodied_accessors = true:error
csharp_prefer_simple_using_statement = true:error
csharp_style_prefer_switch_expression = true:error
csharp_style_prefer_pattern_matching = true:error
csharp_style_prefer_not_pattern = true:error
csharp_style_prefer_null_check_over_type_check = true:error

# Formatting
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_indent_case_contents = true
csharp_space_after_cast = false
csharp_space_after_keywords_in_control_flow_statements = true

[*.{csproj,props,targets}]
indent_size = 2
```

### Directory.Build.props (Root of solution)

```xml
<Project>
  <PropertyGroup>
    <!-- Treat warnings as errors -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <NoWarn />
    
    <!-- Enable all analyzers -->
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <AnalysisMode>All</AnalysisMode>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    
    <!-- Nullable reference types -->
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- Documentation -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn> <!-- Allow missing XML comments for now -->
  </PropertyGroup>
  
  <ItemGroup>
    <!-- StyleCop Analyzers -->
    <PackageReference Include="StyleCop.Analyzers" Version="1.1.118">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    
    <!-- Additional Analyzers -->
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    
    <PackageReference Include="Roslynator.Analyzers" Version="4.12.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    
    <PackageReference Include="SonarAnalyzer.CSharp" Version="9.23.0.88079">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

### stylecop.json (Root of solution)

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json",
  "settings": {
    "documentationRules": {
      "companyName": "FrostAura",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "documentInterfaces": true,
      "documentExposedElements": true,
      "documentInternalElements": true,
      "documentPrivateElements": false,
      "documentPrivateFields": false,
      "fileNamingConvention": "metadata"
    },
    "indentation": {
      "indentationSize": 4,
      "tabSize": 4,
      "useTabs": false
    },
    "layoutRules": {
      "newlineAtEndOfFile": "require"
    },
    "maintainabilityRules": {},
    "namingRules": {
      "allowCommonHungarianPrefixes": false,
      "allowedHungarianPrefixes": []
    },
    "orderingRules": {
      "usingDirectivesPlacement": "outsideNamespace",
      "systemUsingDirectivesFirst": true
    },
    "readabilityRules": {},
    "spacingRules": {}
  }
}
```

### .globalconfig (Root of solution - Analyzer severity overrides)

```ini
is_global = true

# All diagnostics are errors by default
dotnet_analyzer_diagnostic.severity = error

# Specific rule configurations
dotnet_diagnostic.CA1062.severity = error  # Validate arguments of public methods
dotnet_diagnostic.CA1303.severity = none   # Do not pass literals as localized parameters (too strict)
dotnet_diagnostic.CA1848.severity = error  # Use LoggerMessage delegates
dotnet_diagnostic.CA2007.severity = none   # ConfigureAwait (not needed in ASP.NET Core)
dotnet_diagnostic.CA1852.severity = error  # Seal internal types
dotnet_diagnostic.CA1851.severity = error  # Possible multiple enumerations
dotnet_diagnostic.CA1860.severity = error  # Avoid using 'Enumerable.Any()' extension method
dotnet_diagnostic.CA1861.severity = error  # Avoid constant arrays as arguments
dotnet_diagnostic.CA2254.severity = error  # Template should be a static expression

# StyleCop rules
dotnet_diagnostic.SA1101.severity = none   # Prefix local calls with this (personal preference)
dotnet_diagnostic.SA1309.severity = none   # Field names should not begin with underscore
dotnet_diagnostic.SA1633.severity = none   # File should have header (optional)
dotnet_diagnostic.SA1600.severity = error  # Elements should be documented
dotnet_diagnostic.SA1601.severity = error  # Partial elements should be documented
dotnet_diagnostic.SA1602.severity = error  # Enumeration items should be documented
```

## Gate Commands (Build Fails on Lint)

### Frontend

```bash
# Validate (runs before build)
npm run validate

# Or individually
npm run typecheck   # TypeScript strict checking
npm run lint        # ESLint with --max-warnings 0
npm run format:check # Prettier format check
```

### Backend

```bash
# Build (automatically runs analyzers as errors)
dotnet build --no-incremental

# Format check
dotnet format --verify-no-changes --severity error

# Full validation
dotnet build --no-incremental && dotnet format --verify-no-changes --severity error
```

## CI/CD Integration

### GitHub Actions (lint.yml)

```yaml
name: Lint

on: [push, pull_request]

jobs:
  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json
      - run: npm ci
        working-directory: frontend
      - run: npm run validate
        working-directory: frontend

  backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
        working-directory: backend
      - run: dotnet build --no-restore --no-incremental
        working-directory: backend
      - run: dotnet format --verify-no-changes --severity error
        working-directory: backend
```

## Key Rules

| Rule | Frontend | Backend |
|------|----------|---------|
| Warnings allowed | ❌ NO | ❌ NO |
| `any` type allowed | ❌ NO | N/A |
| Implicit types | N/A | ✅ Required (`var`) |
| Unused variables | ❌ Error | ❌ Error |
| Console.log/Debug | ❌ Error | ❌ Error |
| Null safety | ✅ Strict | ✅ Enabled |
| Naming conventions | ✅ Enforced | ✅ Enforced |
| Import order | ✅ Enforced | ✅ Enforced |
| Accessibility (a11y) | ✅ Strict | N/A |

## When Adding New Files

1. **Frontend**: Run `npm run lint:fix && npm run format` immediately
2. **Backend**: Run `dotnet format` immediately
3. **Before commit**: Run full validation (`npm run validate` / `dotnet build`)

## Escape Hatches (Use Sparingly)

### Frontend (single line only)

```typescript
// eslint-disable-next-line @typescript-eslint/no-explicit-any -- API returns untyped data
const response = data as any;
```

### Backend (single line only)

```csharp
#pragma warning disable CA1062 // Null check handled by framework
public void Method(string input) { }
#pragma warning restore CA1062
```

**Never disable rules globally. Each escape must have a justification comment.**
