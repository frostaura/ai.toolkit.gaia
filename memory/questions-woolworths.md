---
name: ai-toolkit-gaia-questions-woolworths
description: "Founder-owned: the reverse-engineered Woolworths contract is published under FrostAura's name with no agreement, terms or notice channel. Unpublishing is no remedy — the levers left are framing, a vendor talk, removal on request."
type: question
last_verified: 2026-09-11
---

# Open — the published Woolworths integration

**This is the single home for the Woolworths exposure.** Other topics link here; none of them
restate it. Owner throughout: **the founder** — every lever is a brand, legal or vendor call,
which `decision-rights.md` places at parent level, not with an agent.

**What is already decided and cannot be undone.** The founder authorised the v13.0.0 push on
2026-08-21 with the exposure stated, so the work is on a public, MIT-licensed repository under
the company's name. `fa.integrations` was private; this repo is not. Unpublishing is not a
remedy once something is public — the remaining levers are framing, a vendor conversation, and
being ready to remove the integration if asked.

**What is published, precisely:**

- `WoolworthsSearchClient` carries a Constructor.io key lifted from the Woolworths storefront
  bundle. Not a secret leak — that key is served to every browser. What changes is attribution
  and convenience: a public repo under FrostAura's name hands anyone a working, documented
  proxy for a search contract **Woolworths pays for and we do not**. Abuse lands on their
  bill, which makes it worse rather than better.
- `docs/integrations/woolworths.md` is a reverse-engineered contract for undocumented
  endpoints. As a private engineering note that is documentation; published under a company
  name it reads as an invitation.
- There is no agreement, no terms and no notice channel with Woolworths, and there never was.
  Tolerable for a private tool a founder ran against his own account; a different proposition
  attached to a public repository.

**The live decisions, all founder-owned:**

1. **Does the README take a stance?** It currently takes none. A deliberate paragraph —
   personal-use tool, no affiliation with or endorsement by Woolworths, no warranty — is the
   cheapest move available and is not yet made.
2. **Is Woolworths approached, or not?** Time-sensitive rather than preventable now.
3. **Should the write path require explicit confirmation?**
   `woolworths_add_shopping_list_to_cart` adds roughly a full shopping list in one call with
   no confirmation. It stops short of checkout, but an agent calling it silently mutates a
   real cart — more pressing now the tools are reachable from a hosted server.

**Two operational unknowns, inherited from `fa.integrations` and never closed:**

- **Does a headless-filled cart appear in the Woolworths mobile app?** It is account-bound and
  survived sign-out in testing, so it should, but it has never been confirmed on a phone. The
  Siri Shortcut flow ends with "open the cart", so this matters to a real user path.
- **Will Woolworths tolerate this traffic pattern?** Unknowable without asking. Treat the
  integration as something that can break without warning, at their discretion.
