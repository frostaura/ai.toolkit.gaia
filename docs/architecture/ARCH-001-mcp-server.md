# ARCH-001 — MCP Server Architecture

## Summary

The Gaia MCP Server uses a dual-server architecture with HTTP and stdio transports, backed by thread-safe JSON file storage.

## Context & Problem

AI agent orchestration requires persistent, concurrent-safe storage for tasks, project memory, and improvement suggestions. The storage must survive server restarts and support multiple agents accessing the same data simultaneously.

## Decision

- **HTTP MCP Server** (`Gaia.Mcp.Server`): ASP.NET Core web app exposing tasks, memory, and self-improvement tools via HTTP transport at `/mcp`.
- **Stdio MCP Server** (`Gaia.Workflows.Server`): Console app exposing workflow execution tools via stdio transport.
- **Storage**: `ThreadSafeJsonStore<T>` uses per-key `SemaphoreSlim` locking with atomic writes (temp-file-then-move). No database required.
- **Security**: Project names are sanitized to prevent path traversal. Status values are validated against an allow-list.
- **Error handling**: Structured error responses with namespaced codes (`GAIA_TASKS_ERR_*`) for all failure paths.

## Alternatives Considered

- **SQLite**: Rejected — JSON files are simpler for the current scale and easier to inspect/debug.
- **Single server**: Rejected — workflows need stdio for local CLI integration; tasks/memory need HTTP for remote access.

## Consequences

- Simple deployment: single Docker container for HTTP server, local binary for workflows.
- No migration tooling needed — JSON schema changes are backward-compatible.
- Performance limited by full-file read/write on every mutation (acceptable at current scale).

## Affected Components

- `Gaia.Mcp.Server/Storage/ThreadSafeJsonStore.cs`
- `Gaia.Mcp.Server/Tools/TasksTool.cs`
- `Gaia.Mcp.Server/Tools/MemoryTool.cs`
- `Gaia.Mcp.Server/Tools/SelfImproveTool.cs`
- `Gaia.Mcp.Server/Validation/CompletionValidator.cs`

## Notes

Data directory is configurable via `GAIA_DATA_DIR` environment variable, defaulting to `{AppContext.BaseDirectory}/data`.
