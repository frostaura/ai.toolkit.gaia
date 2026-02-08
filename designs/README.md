# Gaia 6 Design Documents

Design documents are created **on-demand** based on project complexity.

## Documents

| Document | Purpose | When Required |
|----------|---------|---------------|
| `design.md` | Use cases + architecture | Standard+ complexity |
| `api.md` | API contracts | When API changes |
| `data.md` | Database schema + models | When DB changes |
| `security.md` | Auth + access control | When security changes |

## Complexity Tiers

- **Trivial/Simple**: No design docs required
- **Standard**: `design.md` only
- **Complex**: `design.md` + affected area docs
- **Enterprise**: All applicable documents

## Rules

- ✅ Create documents when needed, not upfront
- ✅ Update incrementally
- ✅ Keep consistent terminology across docs
- ❌ Don't create empty templates
- ❌ Don't rewrite entire documents for small changes
