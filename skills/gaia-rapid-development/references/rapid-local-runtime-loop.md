# Rapid Local Runtime Loop

Use this reference when `gaia-rapid-development` needs repo-accurate commands
for the fast local edit-run-observe cycle.

## Dependency stack

Start only the shared services you need from Compose:

```bash
docker compose up -d postgres mailhog
```

If the loop needs the full Compose app surface instead, it no longer belongs in
`gaia-rapid-development`; route back to the full Gaia process.

## Backend loop

Run the API directly from the repo so code edits apply immediately:

```bash
cd src/backend
dotnet run --project LifeOS.Api
```

- Prefer ASP.NET user-secrets on `src/backend/LifeOS.Api` for local secrets.
- If you bypass the frontend proxy, keep the backend origin explicit so browser
  requests still target the direct API process.

## Frontend loop

Run the frontend with the existing HMR command:

```bash
cd src/frontend
npm run dev
```

This repo uses Vite, so `npm run dev` is the correct fast-feedback command even
if another project might use `npm start`.

## Browser verification

1. Open the live page served by the direct frontend dev server.
2. Reproduce the changed flow.
3. Capture and inspect screenshots for visible regressions.
4. Review console errors and failed network requests.
5. Iterate immediately while the loop remains local and bounded.

## Exit criteria

Leave `gaia-rapid-development` when any of the following becomes true:

- the task now changes architecture, contract, or release expectations
- the work needs multi-branch planning or cross-role coordination
- the evidence needed is formal regression rather than rapid screenshot review
- the direct dev runtime is no longer enough to prove the change safely

## Related repo references

- `README.md` local development section
- `docs/testing/TEST-001-manual-regression.md`
