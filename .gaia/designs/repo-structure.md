[<< Back](../../README.md)

## Repo Structure

This document describes the preferred repository structure. This may be used for AI coding when scaffolding the solution from scratch, for example. This structure must be followed for all new repositories, and existing repositories should be migrated to this structure over time.

### Structure

- ROOT (The repository root)
  - /.gaia (The collection of repository documents and system designs etc. This directory should be documented in [README.md](../../README.md) too.)
    - /designs (System designs - COMPLETED FROM TEMPLATES BELOW)
      - <DESIGN_NAME>.md (A system design file)
    - /prompts (AI framework prompt collection)
      - gaia-plan.prompt.md (The main planning and execution prompt for building solutions)
      - gaia-improve.prompt.md (The prompt for improving the Gaia AI Toolkit framework)
      - gaia-no-plan.prompt.md (The prompt for working without the Gaia framework)
      - gaia-test.prompt.md (The prompt for testing purposes)
    - /instructions (Copilot instructions for enforcing rules)
      - <INSTRUCTION_NAME>.instructions.md (Instruction files)
    - <DOCUMENT_NAME>.md (a system documentation file)
  - /.github (GitHub-related content and AI state management)
    - /state (AI session and project state tracking)
      - Gaia.TaskPlanner.db.json (system/execution plan, milestones & current session progress tracking)
    - templates
      - /designs (System designs templates)
      - <DESIGN_NAME>.md (A system design file)
    - /workflows (GitHub action workflows)
      - <WORKFLOW_NAME>-workflow.yml
  - /.vscode (VS Code workspace settings & recommended extensions)
    - extensions.json (Recommended VSCode extensions)
    - mcp.json (Which MCP server(s) should Copilot Chat have access to)
    - settings.json (The VSCode workspace settings to inherit when working on this system)
  - /src (mandatory for all code and test projects to live inside here or deeper)
    - / backend (where .NET services go, a Dotnet example below)
      - / <PROJECT_NAME>.Api (ASP.NET Core Web API project)
        - Controllers/
        - Program.cs
        - appsettings.json
        - <PROJECT_NAME>.Api.csproj
      - / <PROJECT_NAME>.Api.Tests.csproj
        ...
      - / <PROJECT_NAME>.Core (Business logic etc)
        - / Services
          - / Managers (iDesign Manager layer)
            - UserManager.cs
              ...
          - / Engines (iDesign Engine layer)
            - CalculatorEngine.cs
              ...
          - / Data / Repositories (iDesign Data Access layer)
            - UserData.cs
            - ApplicationDbContext.cs
              ...
        - / Models (Domain models and DTOs)
          - / Entities
            ...
          - / Dtos
            ...
          - User.cs
          - UserDto.cs
            ...
        - / Interfaces (Shared interfaces)
          - / Managers
            - IUserManager.cs
          - / Engines
            - IAuthenticationEngine.cs
          - / Data
            - IUserData.cs
        - <PROJECT_NAME>.Core.csproj
      - / <PROJECT_NAME>.Core.Tests.csproj
        - ...
      - <PROJECT_NAME>.sln
    - / frontend (a ReactJS example below)
      - public/
      - src/
        - components/
        - pages/
        - store/ (Redux store)
        - services/ (API services)
        - types/
        - App.tsx
        - index.tsx
      - tests
      - t1
      - t2
      - integration
      - ...
      - package.json
      - tsconfig.json
      - tailwind.config.js
      - ...
    - ...
  - .gitignore
  - docker-compose.yml (Wire up the entire stack with all the dockerfiles)
  - Dockerfile.<PROJECT_NAME> (frontend / backend etc)
  - LICENSE
    - MIT
  - README.icon.png (The project thumbnail / icon)
  - README.md
    - Link to various documents in the .docs dir, in a neat table format.

# Description

This repository follows a structured approach designed for AI-enhanced software development. Each directory serves a specific purpose in the development lifecycle:

## Core Directories

### `.gaia/` - AI Framework Intelligence

The heart of the AI coding framework, containing the orchestration files that guide the AI through the development process:

- **prompts/**: AI coding framework prompt files
  - **gaia-plan.prompt.md**: The main planning and execution prompt for building solutions
  - **gaia-improve.prompt.md**: The prompt for improving the Gaia AI Toolkit framework
  - **gaia-no-plan.prompt.md**: The prompt for working without the Gaia framework
  - **gaia-test.prompt.md**: The prompt for testing purposes
- **instructions/**: Copilot instructions for enforcing common and file-specific rules
- **designs/**: System architecture and design documents

### `.github/` - CI/CD Pipeline & State Management

GitHub-related automation and state tracking:

- **state/**: AI planning, session and project state tracking
  - **Gaia.TaskPlanner.db.json**: Auto-generated project plan with milestones, deliverables & tracking of development progress and current status
- **workflows/**: Automated build, test, and deployment processes



### `src/` - Source Code Organization

Following clean architecture principles with clear separation of concerns:

- **backend/**: API and business logic layers with proper dependency injection
- **frontend/**: User interface applications (web, mobile, desktop)

## Configuration & DevOps

### `.github/` - CI/CD Pipeline

- **workflows/**: Automated build, test, and deployment processes
- Supports containerization, security scanning, and multi-environment deployments

### `.vscode/` - Development Environment

- **mcp.json**: Which MCP server(s) should Copilot Chat have access to
- **settings.json**: Consistent code formatting and linting rules
- **extensions.json**: Recommended extensions for optimal development experience

## Project Files

- **README.md**: Main project documentation with quick start guide
- **docker-compose.yml**: Multi-service container orchestration
- **Dockerfile.\***: Service-specific container definitions
- **.gitignore**: Version control exclusion patterns
- **LICENSE**: MIT license for open source distribution

This structure ensures that AI agents can navigate the codebase efficiently while maintaining clean separation between documentation, source code, and infrastructure components.

[<< Back](../../README.md)
