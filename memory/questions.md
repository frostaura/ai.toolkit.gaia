---
name: ai-toolkit-gaia-questions
description: "Founder-owned and unresolved: one repo ships two unrelated products; placement reads as Technologies, not Labs; floating ref:main installs vs tags; and nothing in the repo can say whether the hosted service was ever redeployed."
type: question
last_verified: 2026-09-11
---

# Open questions

Every item here is **founder-owned**. Placement, brand and public-distribution calls are
parent-controlled under `decision-rights.md`; an agent recommends and escalates, it does not
decide. The Woolworths exposure has its own home and is not restated here:
[`questions-woolworths.md`](questions-woolworths.md).

1. **Is the hosted service running a v13 image?** Owner: founder — he holds the Docker Hub
   and Portainer access, and nothing in this repository can answer it. CI builds and pushes
   the image on every merge to `main` but **contains no deploy step**, so redeploy is a manual
   act outside the repo. Until someone checks the running `tools/list`, the correct posture is
   that the version is *unknown*, not that it is stale. The evidence trail is in
   [`state.md`](state.md); the consequence if it was never redeployed is in
   [`watch.md`](watch.md).

2. **This repository ships two unrelated products.** A developer-delivery plugin suite and a
   grocery integration gateway share a repo, a CHANGELOG, a version line and a hostname — and
   since `foundation` stopped wiring the server, nothing else. Three honest exits: leave it
   and say so in the README; split the server into its own repository; or reframe Gaia
   deliberately as both. **The unpushed re-wiring quietly picks a fourth option** — coupling
   them again without deciding — and now rides inside the v14 release, so the push that ships
   v14 also ships this ([`state.md`](state.md)). Owner: founder.

3. **Placement still reads as `Technologies/`, not Labs.** This is a versioned, publicly
   distributed, multi-ecosystem developer product with a live hosted service, CI/CD, a support
   address and no research character left. A credential-handling gateway is an operations
   concern. It is also the only Labs program with a granted stealth exception — the signature
   of something that already graduated. Owner: founder; parent-controlled, so this program
   recommends and does not move itself.

4. **`ref: main` or tags for plugin refs?** Floating installs are dangerous for a public user
   base: a breaking change reaches every fetching install at push time, with no version a user
   can pin to and — because the release path creates no tags at all ([`state.md`](state.md)) —
   nothing to roll back to. Owner: founder. Decided either way, the README should say which.

5. **Should the integration be wired into any plugin at all?** If yes, it wants its own opt-in
   plugin rather than the context layer — that was the v13.0.0 reasoning, and the unpushed
   commit contradicts it without arguing with it. Owner: founder.
