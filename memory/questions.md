---
name: ai-toolkit-gaia-questions
description: "Founder-owned and unresolved: one repo ships two unrelated products; placement reads as Technologies, not Labs; floating ref:main installs vs tags; and nothing in the repo can say whether the hosted service was ever redeployed."
type: question
last_verified: 2026-09-16
---

# Open questions

Every item here is **founder-owned**. Placement, brand and public-distribution calls are
parent-controlled under `decision-rights.md`; an agent recommends and escalates, it does not
decide. The Woolworths exposure has its own home and is not restated here:
[`questions-woolworths.md`](questions-woolworths.md).

1. **What image is the hosted service running?** Owner: founder — he holds the Docker Hub and
   Portainer access, and nothing in this repository can answer it. CI builds and pushes the image
   on every merge to `main` but **contains no deploy step** (re-read 2026-09-16: two jobs, `test`
   and `build-and-push`), so the 2026-09-16 push built a v14 image and deployed nothing. Until
   someone checks the running `tools/list`, the version is *unknown*, not stale — and the obvious
   shortcut does **not** work: the server advertises `13.0.0` on `initialize` from source, so a
   freshly built `14.3.1` image reports `13.0.0` too ([`watch.md`](watch.md)). Only the tool set
   distinguishes them.

2. **This repository ships two unrelated products.** A developer-delivery plugin suite and a
   grocery integration gateway share a repo, a CHANGELOG, a version line and a hostname. Three
   honest exits: leave it and say so in the README; split the server into its own repository; or
   reframe Gaia deliberately as both. **The 2026-09-16 v14 push picked a fourth option by
   accident** — the `foundation` MCP re-wiring rode inside the release, so the two are coupled
   again in the *published* product without anyone deciding it ([`state.md`](state.md)). Owner:
   founder; this is now a question about what to do with a shipped coupling, not a pending one.

3. **Placement still reads as `Technologies/`, not Labs.** This is a versioned, publicly
   distributed, multi-ecosystem developer product with a live hosted service, CI/CD, a support
   address and no research character left. A credential-handling gateway is an operations
   concern. It is also the only Labs program with a granted stealth exception — the signature
   of something that already graduated. **Strengthened 2026-09-16:** it is now demonstrably in
   live use on the founder's own desktop surface ([`watch.md`](watch.md)), which retires the
   last argument that it had not graduated in practice. Owner: founder; parent-controlled, so
   this program recommends and does not move itself.

4. **`ref: main` or tags for plugin refs?** Floating installs are dangerous for a public user
   base: a breaking change reaches every fetching install at push time, with no version a user
   can pin to and — because the release path creates no tags at all ([`state.md`](state.md)) —
   nothing to roll back to. Owner: founder. Decided either way, the README should say which.

5. **Should the integration be wired into any plugin at all?** It now is: the re-wiring commit
   published in v14 on 2026-09-16, contradicting the v13.0.0 reasoning without ever arguing with
   it. If the answer is yes it wants its own opt-in plugin rather than the context layer; if no,
   the revert is a further release. Either way the README's "the plugins send nothing anywhere"
   disclosure is currently untrue for `foundation`. Owner: founder.
