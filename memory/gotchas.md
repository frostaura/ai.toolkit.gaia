---
name: ai-toolkit-gaia-gotchas
description: "MSBuildEnableWorkloadResolver=false is mandatory on this volume; the MCP session mode silently governs whether header credentials resolve; Woolworths returns HTTP 200 for every kind of failure; a green dotnet test before v13 meant nothing."
type: gotcha
last_verified: 2026-09-11
---

# Gotchas

- **`MSBuildEnableWorkloadResolver=false` is mandatory on this volume.** The workload
  resolver crashes under the iCloud-synced path and nothing here uses workloads. Export it
  before any `dotnet build` / `test` / `run`, or read a stack trace instead of a build.
- **A green `dotnet test` before v13.0.0 meant nothing.** `src/Gaia.Mcp.Tests` is now a real
  xUnit project in `Gaia.slnx`, but CI ran an empty gate for months — so any note, commit
  message or report written before that release treating green as evidence is wrong.
- **The MCP session mode silently governs whether header credentials work at all.** The
  integration tools read credentials from the inbound request headers via
  `IHttpContextAccessor`; `fa.integrations` ran stateless partly for this reason, while Gaia
  runs `StatefulForInitializeClients` so the sampling demo keeps working. Under the hybrid
  mode a tool call *does* still execute inside the caller's HTTP request — verified by probe,
  not reasoned about. **If the mode changes, re-verify:** send only `X-Woolworths-Username`
  and check that only the password is reported missing. The failure mode looks exactly like a
  caller-side configuration problem, which is why it costs hours.
- **Woolworths returns HTTP 200 for every kind of failure** — bad credentials, unavailable
  products, a logout that did not happen. Always inspect the body (`errorMessage`,
  `formexceptions`, `loggedInStatus`). Two traps in the same family: cart adds must be **one
  product per request**, because a batch containing one unavailable item adds *nothing* and
  still returns a success-shaped 200; and the search `filters[visibility]` parameter must be
  sent **twice** (`all` and `web and app`) or food results come back empty while non-food does
  not — which reads convincingly as a location-gated catalogue and is not. Full captured
  contract: `../docs/integrations/woolworths.md`.
- **The MCP SDK replaces arbitrary exception text with a generic message.** Anything a caller
  must act on has to be thrown as `McpException`. Only the *missing*-credential path was
  wrapped when this code arrived, so a *wrong* password produced "an error occurred" — at
  least as common a case. Found by probing the running server, not by reading the code.
- **Nothing in CI checks the byte-identical `plugin.json` pair invariant**, or that the two
  marketplace manifests agree. The `personal` plugin's pair had silently drifted before that
  plugin was removed. The three surviving pairs were `cmp`-verified identical on 2026-09-11;
  without a check they will drift again. The by-hand loop is in `../docs/development.md`.
- **Probing a health endpoint: `ping` was removed in MCP spec 2026-07-28** and SDK 2.2.0
  rejects it with a 400. Probe with a legacy `initialize` POST instead — which is what the
  Dockerfile's HEALTHCHECK does.
- **MCP sampling, roots and logging are spec-deprecated as of 2026-07-28** (12-month window,
  SEP-2577). `sampling_summarize` exists to demonstrate sampling anyway, so MCP9005 is
  suppressed file-wide in `SamplingTools.cs` — deliberately, and only there.
