# Standard Repo Structure

Use a role-based structure instead of a web-biased `frontend` and `backend` split. The top-level repo should separate runnable applications from reusable packages, then organize each project by responsibility.

## Core Principles

- Name top-level folders by runtime role, not by language or framework.
- Separate deployable apps from reusable libraries.
- Keep the same architectural message across platforms: presentation, application, domain, and infrastructure concerns remain distinct.
- Reserve `coordinators`, `engines`, and `io` for consistent meanings across all runtimes.

## Recommended Root Structure

- `.github`
  - `workflows` - CI/CD workflows
  - `actions` - reusable composite actions or shared workflow actions
- `docs` - product, architecture, engineering, and operational documentation
- `src` - runnable or deployable applications by platform or role
  - `web` - browser applications such as React, Angular, Vue, Blazor, etc.
  - `mobile` - mobile apps such as MAUI, React Native, Flutter, etc.
  - `desktop` - desktop apps such as MAUI, WPF, Electron, Tauri, etc.
  - `api` - HTTP, GraphQL, or gRPC services
  - `worker` - background jobs, queue processors, schedulers, daemons
  - `cli` - command-line tools and automation entry points
  - `tests` - cross-project unit, integration, e2e, and performance tests
  - `infra` - deployment and environment infrastructure, IaC, hosting configs
  - `scripts` - repo automation scripts
  - `tools` - local developer tooling, generators, linting helpers
- `examples` - optional sample apps or reference implementations

## Standard Internal Project Structure

Inside any app or package, use the platform's normal project layout, but prefer these responsibility boundaries:

- `src` - source code if the ecosystem expects a source folder
  - `components` - reusable UI components when the project has a visual surface
    - `atoms` - smallest, indivisible components that do not depend on other components
    - `molecules` - composed of atoms, but still reusable and not specific to a single page or feature
    - `organisms` - composed of molecules and atoms, but more
    ...
  - `services` - service layer where the stack prefers service-oriented organization
    - `coordinators` - orchestrate multi-step flows across modules or dependencies
    - `engines` - complex calculations, rules, or logic that do not fit naturally in a single component or entity
    - `io` - input and output boundaries such as API clients, file handling, storage, serialization, or transport

Not every project needs every folder. Use only the folders that match the project role.

## Role Mapping for Common Stacks

- React or web SPA: `apps/web` plus shared code in `packages/presentation`, `packages/application`, or `packages/domain`
- Node.js API: `apps/api` plus shared code in `packages/application`, `packages/domain`, and `packages/infrastructure`
- .NET service: `apps/api` or `apps/worker` with shared libraries in `packages/domain`, `packages/application`, and `packages/infrastructure`
- MAUI app: `apps/mobile` or `apps/desktop` with shared libraries in `packages/presentation`, `packages/application`, and `packages/domain`
- Full-stack monorepo: multiple entries under `apps`, shared code under `packages`, tests under `tests`, and environment definitions under `infra`

## Mapping from the Old Structure

- Old `src/frontend` maps to `apps/web` or another app role such as `apps/mobile` or `apps/desktop`
- Old `src/backend/main api project` maps to `apps/api`
- Old `src/backend/core project` maps to `packages/domain` and sometimes `packages/application`
- Old `src/backend/data project` maps to `packages/infrastructure` or `packages/infrastructure/persistence`

## Prescribed Meanings

- `apps/*` are entry points, deployables, or runnable hosts
- `packages/*` are reusable libraries that should avoid owning runtime bootstrapping
- `presentation` handles delivery to users or external callers
- `application` coordinates use cases and workflows
- `domain` owns business meaning and invariants
- `infrastructure` handles external technology concerns
- `coordinators` orchestrate
- `engines` calculate or evaluate rules
- `io` crosses system boundaries