<p align="center">
  <img src="https://github.com/frostaura/ai.toolkit.gaia/blob/main/README.icon.png?raw=true" alt="Gaia" width="300" />
</p>

<h1 align="center"><b>Gaia</b></h1>
<h3 align="center">full-stack apps. enterprise-grade. maintainable. customizable.</h3>
<p align="center"><i>A team of AI agents, available as a plugin for Claude Code, GitHub Copilot, and Claude Desktop.</i></p>

---

<p align="center">
  <a href="./CHANGELOG.md"><img src="https://img.shields.io/badge/Version-14.0.0-purple.svg" alt="Version 14.0.0" /></a>
  <a href="https://opensource.org/licenses/MIT"><img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT" /></a>
  <a href="https://github.com/features/copilot"><img src="https://img.shields.io/badge/GitHub-Copilot-blue.svg" alt="GitHub Copilot" /></a>
  <a href="https://www.claude.com/product/claude-code"><img src="https://img.shields.io/badge/Claude-Code-orange.svg" alt="Claude Code" /></a>
  <a href="https://gaia.frostaura.net"><img src="https://img.shields.io/badge/MCP-remote-green.svg" alt="Remote MCP" /></a>
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-MCP%20server-512BD4.svg" alt=".NET MCP server" /></a>
</p>

---

## What is Gaia?

Gaia is a **team of AI agents** that builds and evolves software using **spec-driven development**. You describe your goal; Gaia coordinates intake, architecture, planning, implementation, testing, and release — enforcing QA and release gates along the way.

It also does the part that comes first: giving your assistant a **context layer** it can actually work from — instruction and memory files at every directory that earns one, so an agent understands your whole tree rather than the one file it was pointed at.

Gaia ships as three composable plugins. The plugins are self-contained: your plan is your assistant's own todo list, and everything durable — memory, decisions, lessons — is written as files in your repository, so it versions with your code and there is nothing to sync.

The repository also hosts a small **.NET MCP server** ([`src/`](./src)) that exposes third-party consumer services as agent tools. It is a separate concern from the plugins and neither requires the other.

| Plugin | What it covers |
| --- | --- |
| **`foundation`** | The context layer — establishing, maintaining and auditing instruction and memory files across a tree — plus authoring new skills, agents and plans. Required by the other two. |
| **`engineering`** | Spec-driven delivery end-to-end: architecture, planning, implementation, UI, testing, containerization, deploy, and per-language repository baselines. |
| **`product`** | A money-gated lifecycle for consumer one-off / in-app-purchase products, from discovery through launch and live-ops. |

> There is deliberately **no skill list in this README** — it rots faster than anyone maintains it. The live roster is `/plugin` in your client; a dated snapshot lives in [`docs/catalog.md`](./docs/catalog.md).

---

## Install

Install only what you need. **Install `foundation` first** — `engineering` and `product` both declare it as a dependency in their `plugin.json` and call its skills by name, but nothing installs it for you.

### Claude Code

```bash
/plugin marketplace add frostaura/ai.toolkit.gaia

/plugin install foundation@frostaura
/plugin install engineering@frostaura
/plugin install product@frostaura
```

### GitHub Copilot

```bash
copilot plugin marketplace add frostaura/ai.toolkit.gaia

copilot plugin install foundation@frostaura
copilot plugin install engineering@frostaura
copilot plugin install product@frostaura
```

### Claude Desktop, claude.ai and Cowork

Add this marketplace from its repository URL — `https://github.com/frostaura/ai.toolkit.gaia` — then install the plugins you want. The exact path differs by surface and changes as the product moves, so follow [Anthropic's plugin documentation](https://code.claude.com/docs/en/plugins) rather than a click-path copied into a README.

> **Surface caveat.** Skills load on every surface. **Sub-agents and hooks run in Claude Code**, not in Desktop or claude.ai chat — which matters more for Gaia than for most plugins, because Gaia's method *is* the fan-out. Everything still works without them; it runs serially instead, and the skills say so where it changes what you get.

---

## Start here

The first thing to run on a new tree — a repo, a monorepo, or a folder of unrelated projects:

> **"Optimize this directory tree — establish the Gaia context layer across everything in it."**

That is `fa-foundation-optimize-directory-tree`, and it is **not a one-off setup step**. It does three jobs, and it decides which one your tree needs by reading disk rather than asking:

- **Establish** — put instruction and memory files at every directory that earns one, write the registry, and derive the design documentation your projects never had, from their source.
- **Maintain** — re-derive every claim from what the projects have actually become, so the files describe the system rather than last quarter's intentions.
- **Sweep** — prune what went stale, consolidate what converged, delete what died. **This is the one a healthy tree runs most often, and deletion is a normal outcome, not data loss.**

Before you run it:

- **`foundation` alone** establishes the context layer. Retrospective **design documentation additionally requires `engineering`** installed.
- **In Claude Code** it fans out one sub-agent per subtree. **In Desktop or claude.ai chat** it runs the same partition and the same brief serially — same result, slower.
- **It states a write plan before writing anything** — scope count, projects to be analysed, file estimate, the write allow-list — and waits for your go-ahead. Only a run carrying an explicit autonomy instruction, or one that cannot reach a human, skips the wait; the plan is stated either way. Nothing is ever committed for you.

---

## Use it

Open any project and prompt your assistant:

> **"Create a REST API for a blog with posts and comments."**

Gaia refines the request, drafts architecture into `docs/`, plans the work, implements it, tests it, and validates release gates — automatically.

### Headless (Copilot CLI)

```bash
copilot -p "Create a REST API for a blog with posts and comments" --yolo
```

### Product discovery (one-off / in-app purchases)

The `product` plugin runs a money-gated pipeline for consumer products that monetize via one-off or in-app / in-game purchases (not SaaS):

```bash
/fa-product-coordinator premium one-off puzzle game for iOS + Steam, ~$200k net in year one
```

…or describe the goal in natural language. The coordinator frames the brief, then runs 10 money-gated stages — each as fan-out → adversarial review → synthesis against one shared unit-economics model — and returns an Opportunity Thesis with a go / loop-back / kill decision.

---

## Docs

| Document | What is in it |
| --- | --- |
| [`docs/catalog.md`](./docs/catalog.md) | Dated snapshot of every plugin, skill and agent. The live roster is always `/plugin`. |
| [`docs/development.md`](./docs/development.md) | Working on Gaia itself: releases and the 12 version sites, the two marketplace manifests, local-clone installs, authoring invariants, the pre-ship checks, contributing. |
| [`CHANGELOG.md`](./CHANGELOG.md) | Every user-visible change, lockstep across the three plugins. |
| [`src/README.md`](./src/README.md) | The .NET MCP server — integration tools, transports, credentials. |
| [`docs/integrations/`](./docs/integrations/architecture.md) | How the integrations work: the credential model, the captured upstream contract, use cases, and the end-to-end QA procedure. |
| [`CLAUDE.md`](./CLAUDE.md) | How agents should work in this repository. |

---

## Data & privacy

**The plugins send nothing anywhere.** They are skill and agent definitions that run inside your assistant. Your plan lives in your assistant's own todo list, and your memory lives in `MEMORY.md` and `memory/` files inside your repository. There is no hosted store, no account, and nothing segregated by project name because nothing leaves your machine.

**The MCP server stores nothing either.** It is stateless by design: it holds no database and no credential store, so a compromise of the host leaks no standing access to anyone's account.

- **Never stored, logged, or echoed:** your provider credentials. You supply them per request — as HTTP headers over streamable HTTP, or in the process environment over stdio — and they live only for the duration of that one call.
- **Sent onward:** only what the tool you invoked requires, and only to that tool's provider.
- **Sessions are per-operation:** sign in, do the work, sign out. Sessions are never pooled or cached, so one caller's session can never serve another's call.

Because credentials travel in headers, **never expose this server over plain HTTP** — TLS is the only thing protecting them. The server source lives in [`src/Gaia.Mcp.Server`](./src/Gaia.Mcp.Server).

---

<p align="center">
  <i>"In Greek mythology, Gaia is the personification of Earth and the ancestral mother of all life."</i>
</p>
