#!/usr/bin/env python3
"""Context-layer audit.

Mechanical drift checks over the instruction-file / MEMORY.md / skills layer of
a consuming repository. Catches the classes of drift that do not need
judgement, so agent attention is spent on the ones that do.

Usage:  python3 context-audit.py [--root PATH] [--scope NAME ...]
                                 [--group-dir NAME ...] [--instruction-file NAME]
                                 [--registry PATH] [--max-age DAYS]
                                 [--no-git] [--all] [--fix-index]

A *scope* is a directory carrying its own context pair — an instruction file
(`CLAUDE.md`, or `AGENTS.md` where that is the repository's convention) beside a
`MEMORY.md`. Pass `--scope` once per scope, or pass none and let the audit
discover every immediate subdirectory that carries the pair.

A *grouping directory* is one whose only job is holding scopes. Its name varies
by ecosystem — `projects/`, `packages/`, `apps/`, `services/`, `crates/` — so
pass `--group-dir` once per name in use; the default is `projects`. Pass
`--instruction-file AGENTS.md` where that, rather than `CLAUDE.md`, is the
tree's real instruction file and the other name is only an interop pointer.

Exit code 0 = clean, 1 = findings. Findings are advisory, not policy.
Spec: ../references/context-cascade.md
Finding codes: ../references/context-audit-findings.md
"""
import argparse
import datetime as dt
import os
import re
import sys
from pathlib import Path

# The instruction half of the context pair. `CLAUDE.md` wins where both exist —
# a repository that also ships `AGENTS.md` for other tooling usually keeps it as
# a pointer, and auditing the pointer as if it were the instruction file emits a
# page of findings against a file that is deliberately thin.
INSTRUCTION_FILES = ("CLAUDE.md", "AGENTS.md")
# ...which is the wrong way round on an `AGENTS.md`-convention tree, where the
# `CLAUDE.md` is the three-line interop pointer. `--instruction-file NAME`
# reorders this so every check routed through `instruction_file()` audits the
# file that actually carries the rules.
def set_instruction_file(name):
    """Put `name` at the front of INSTRUCTION_FILES; the rest stay as fallbacks."""
    global INSTRUCTION_FILES
    INSTRUCTION_FILES = (name,) + tuple(n for n in INSTRUCTION_FILES if n != name)


# A *grouping directory* holds scopes and nothing else. `projects/` is one
# convention among many — `packages/`, `apps/`, `services/`, `crates/`, `libs/`
# are the same idea under different ecosystems — so the names are configurable
# with a repeatable `--group-dir`. Hard-coding one of them is how this script
# used to print CLEAN over every directory a `packages/`-shaped tree holds.
GROUP_DIRS = ("projects",)
SKIP_DIRS = {".git", "node_modules", "worktrees", "bin", "obj", "dist",
             "build", ".venv", "venv", "__pycache__", ".tmp"}
# Imported/vendored trees. Their link debt is upstream engineering debt, not
# context drift in the consuming repository, so they are excluded unless --all
# is passed.
VENDORED_DIRS = {".github", "plugins", "references"}
STATE_TOKENS = re.compile(
    r"\b(currently|as of|so far|last verified|we decided|is now|dormant|"
    r"in progress|not yet (?:built|shipped|deployed))\b", re.I)
DATE_TOKEN = re.compile(r"\b20\d{2}-\d{2}-\d{2}\b")
LINK = re.compile(r"\[[^\]]*\]\(([^)]+)\)")
CLOUD_DUP = re.compile(r" 2\.[A-Za-z0-9]+$")

# --- Scratch discipline -----------------------------------------------------
# The scratch rule: every temporary artefact lives in a `.tmp/` at the scope
# being worked in, and that directory is emptied before the session ends. These
# checks are the mechanical half of the rule — a populated `.tmp/`, a committed
# one, and debris that never made it into one. The judgement half stays with the
# session-close and audit skills.
#
# `.tmp` is in SKIP_DIRS so no other check descends into scratch: a working copy
# of a memory topic parked there would otherwise emit a page of frontmatter
# findings against a file nobody is maintaining.
TMP_DIR = ".tmp"
# Filesystem noise. A `.tmp/` holding only these is empty in every sense that
# matters, and flagging it teaches agents to ignore the finding.
TMP_NOISE = re.compile(r"^(\.DS_Store|\._.*|\.localized|Thumbs\.db|desktop\.ini)$")
# Tool-residue directories: workspace output that is never committed. A
# 163-entry `.playwright-mcp/` survived a QA session at a tree root once already.
RESIDUE_DIRS = {".playwright-mcp", ".casetest"}
# Unambiguous scratch file extensions.
DEBRIS_EXT = {".log", ".tmp", ".bak", ".zip"}
IMAGE_EXT = {".png", ".jpg", ".jpeg"}
# Directory names inside a `docs/` tree that legitimately hold binaries — brand
# assets, design references, QA capture sets. An image *outside* one of these is
# a screenshot dropped into the reference layer. Keep this list generous: a
# false positive on a curated asset set is how this check gets ignored, and the
# failure it exists to catch (a capture landing in `docs/operating/`) is not
# subtle enough to need a tight net.
ASSET_DIRS = {"assets", "brand", "design", "images", "img", "media", "mockups",
              "screenshots", "shots", "qa-shots", "qa-audio", "reference-shots",
              "diagrams", "figures", "icons", "logos"}
# A dated filename in `docs/` is a session report or a run log wearing a doc's
# clothes. Filenames in a reference layer describe content, never chronology.
DATED_FILENAME = re.compile(r"20\d{2}-\d{2}-\d{2}")

# A fenced code block opener/closer: any indent, then >=3 backticks or tildes.
FENCE = re.compile(r"^\s*(`{3,}|~{3,})\s*(\S*)")
# An inline code span, delimited by a run of 1+ backticks. Applied per line.
INLINE_CODE = re.compile(r"(`+)(?:(?!\1).)*?\1")


def instruction_file(dirpath, filenames):
    """The one instruction file to audit in this directory, or None."""
    for name in INSTRUCTION_FILES:
        if name in filenames:
            return dirpath / name
    return None


def strip_code(text, unclosed=None):
    """Blank out fenced code blocks and inline code spans.

    Markdown links inside a code block are *illustrations of syntax*, not
    references — a cascade spec showing a specimen memory index containing
    `[Current state](memory/state.md)` resolves from nowhere and is not meant
    to. Reporting those as broken links trains agents to ignore the link check
    entirely, which is the real cost.

    Line structure is preserved so any future line-numbered check can reuse this.

    An *unclosed* fence blanks every remaining line, which silently exempts the
    rest of the file from every check built on this function. That is a
    suppression, not a pass. Pass a list as `unclosed` and the opener's line
    number is appended to it so the caller can report it.
    """
    out = []
    fence = None  # (delimiter char, run length, opener line no) while inside
    for lineno, line in enumerate(text.split("\n"), 1):
        m = FENCE.match(line)
        if fence is None:
            if m:
                fence = (m.group(1)[0], len(m.group(1)), lineno)
                out.append("")
                continue
        else:
            # A closer matches the opener's char, is at least as long, and
            # carries no info string.
            if m and m.group(1)[0] == fence[0] and len(m.group(1)) >= fence[1] \
                    and not m.group(2):
                fence = None
            out.append("")
            continue
        out.append(INLINE_CODE.sub(" ", line))
    if fence is not None and unclosed is not None:
        unclosed.append(fence[2])
    return "\n".join(out)


def walk(root, include_vendored=False):
    skip = SKIP_DIRS if include_vendored else SKIP_DIRS | VENDORED_DIRS

    def keep(dirpath, d):
        if d in skip:
            return False
        # `.claude/skills/` is the third cascading context artifact and IS in
        # scope. `.claude/worktrees/` and other `.claude/` internals are not.
        if d == ".claude":
            try:
                return "skills" in os.listdir(os.path.join(dirpath, d))
            except OSError:
                return False
        return True

    for dirpath, dirnames, filenames in os.walk(root):
        dirnames[:] = [d for d in dirnames if keep(dirpath, d)]
        yield Path(dirpath), filenames


# --- Scope resolution -------------------------------------------------------
# The audit used to carry a hard-coded list of top-level directory names, which
# made it useless in any repository but the one it was written for. Scopes are
# now either named explicitly with repeated `--scope` arguments, or discovered:
# any immediate subdirectory that already carries the context pair is, by
# definition, a scope.

def has_pair(path):
    return (path / "MEMORY.md").is_file() and \
        any((path / n).is_file() for n in INSTRUCTION_FILES)


def discover_scopes(root, include_vendored=False):
    """Immediate subdirectories carrying the pair.

    `--all` reaches here too. Discovery used to skip VENDORED_DIRS
    unconditionally, so `--all` widened every *other* check while the scope list
    it fed them stayed narrow — and a repository whose real scopes are named
    `plugins/` or `references/` was invisible to the audit under every
    combination of flags.
    """
    skip = SKIP_DIRS if include_vendored else SKIP_DIRS | VENDORED_DIRS
    found = []
    try:
        entries = sorted(root.iterdir())
    except OSError:
        return found
    for p in entries:
        if not p.is_dir() or p.name in skip:
            continue
        if p.name.startswith("."):
            continue
        if has_pair(p):
            found.append(p)
    return found


def resolve_scopes(root, named, out, include_vendored=False):
    """Named scopes win; discovery is the fallback, never a supplement.

    A named scope that does not exist is MISSING-SCOPE rather than a silent
    no-op: the usual cause is a scope that was renamed or removed without the
    caller (a skill, a CI job) being updated, and swallowing it means the audit
    reports CLEAN for a tree it never looked at.
    """
    if not named:
        return discover_scopes(root, include_vendored)
    scopes = []
    for name in named:
        p = Path(name)
        p = p if p.is_absolute() else (root / name)
        if not p.is_dir():
            out.append(("MISSING-SCOPE", str(p),
                        "named as a --scope but no such directory exists"))
            continue
        scopes.append(p.resolve())
    return scopes


def sub_scopes(scope, group_dirs=GROUP_DIRS):
    """Scopes nested one level down, under any of `<scope>/<group_dir>/`."""
    found = []
    for group in group_dirs:
        proj = scope / group
        if not proj.is_dir():
            continue
        try:
            entries = sorted(proj.iterdir())
        except OSError:
            continue
        found += [p for p in entries if p.is_dir()
                  and p.name not in SKIP_DIRS and p.name != "memory"]
    return found


def all_scopes(root, scopes, group_dirs=GROUP_DIRS):
    """Every directory the cascade requires a context pair in."""
    required = [root]
    for scope in scopes:
        required.append(scope)
        for group in group_dirs:
            proj = scope / group
            if proj.is_dir():
                required.append(proj)
        required += sub_scopes(scope, group_dirs)
    return required


def rel(root, path):
    try:
        return str(Path(path).relative_to(root))
    except ValueError:
        return str(path)


def check_pairs(root, scopes, out, group_dirs=GROUP_DIRS):
    """Every scope, every grouping dir, every nested scope needs both files."""
    for path in all_scopes(root, scopes, group_dirs):
        if not (path / "MEMORY.md").is_file():
            out.append(("MISSING-PAIR", rel(root, path / "MEMORY.md"),
                        "required by the cascade spec"))
        if not any((path / n).is_file() for n in INSTRUCTION_FILES):
            out.append(("MISSING-PAIR", rel(root, path / INSTRUCTION_FILES[0]),
                        "required by the cascade spec"))


REGISTRY_HEADING = re.compile(r"^(#{2,})[ \t]+(.*\bregistr\w*\b.*)$", re.M | re.I)
# A backticked registry name. The shipped pattern was `([a-z0-9][a-z0-9.\-]*)`
# — a transcription of one organisation's lowercase-dotted convention, under
# which `MyService.Api` is permanently REGISTRY-UNLISTED and no PascalCase or
# snake_case phantom is ever caught. Widening some of the call sites and not
# others is worse than widening none, so this constant is the only pattern and
# all three sites use it.
REGISTRY_NAME = re.compile(r"`([A-Za-z0-9][A-Za-z0-9._\-]*)`")


def check_registry(root, registry_path, scopes, out, group_dirs=GROUP_DIRS):
    """A registry file vs what is actually on disk, both directions.

    Opt-in: pass `--registry PATH`. A repository with no central registry is not
    in violation of anything, and assuming one exists is how this check used to
    hard-fail outside the tree it was written for.
    """
    registry = Path(registry_path)
    registry = registry if registry.is_absolute() else (root / registry_path)
    if not registry.is_file():
        out.append(("REGISTRY", rel(root, registry), "--registry path is not a file"))
        return
    text = registry.read_text(encoding="utf-8", errors="replace")
    m = REGISTRY_HEADING.search(text)
    if not m:
        out.append(("REGISTRY", rel(root, registry),
                    "no '## ...registry...' heading to anchor on"))
        return
    level = len(m.group(1))
    tail = text[m.end():]
    nxt = re.search(r"^#{1,%d}[ \t]+\S" % level, tail, re.M)
    section = tail[:nxt.start()] if nxt else tail
    retired = section.split("**Retired", 1)[1] if "**Retired" in section else ""
    live_section = section.split("**Retired", 1)[0]
    listed = set(REGISTRY_NAME.findall(live_section))
    retired_names = set(REGISTRY_NAME.findall(retired))
    on_disk = {}
    # A named scope that is itself a leaf is a registry row in its own right.
    # Without this seed, `on_disk` is built only by descending into grouping
    # directories, so on a *flat* tree — a root holding three project
    # directories and no `projects/` — it is empty: every registry row fires
    # REGISTRY-PHANTOM and REGISTRY-UNLISTED can never fire at all.
    #
    # The leaf condition is load-bearing, not tidiness. A caller passes every
    # scope at every depth as its own `--scope`, and a registry lists
    # *projects* — owning scope is an attribute of a row, and a grouping
    # directory is never a row. Seeding unconditionally would just trade one
    # false-finding class for another: REGISTRY-UNLISTED against `packages`
    # itself, and one against every mid-level scope of a two-level tree.
    for scope in scopes:
        if not sub_scopes(scope, group_dirs) and scope.name not in group_dirs:
            on_disk[scope.name] = rel(root, scope)
    for scope in scopes:
        for p in sub_scopes(scope, group_dirs):
            on_disk[p.name] = rel(root, p)
    for name, where in sorted(on_disk.items()):
        if name not in listed:
            out.append(("REGISTRY-UNLISTED", where,
                        "exists on disk, absent from the registry"))
        if name in retired_names:
            out.append(("REGISTRY-CONTRADICTION", name,
                        "listed as retired but the directory exists"))
    # Registry entries are recognised by their backticked name appearing in a
    # bullet that also carries a `/` slug or a `—` gloss. Both dotted namespaced
    # IDs (`org.product`) and bare brand slugs (`some-product`) must be caught —
    # a dot-only test is blind to every bare slug by construction.
    entries = set()
    for line in live_section.splitlines():
        if not line.lstrip().startswith("-"):
            continue
        names = REGISTRY_NAME.findall(line)
        if names:
            entries.add(names[0])
    for name in sorted(entries):
        if name not in on_disk and name not in retired_names:
            out.append(("REGISTRY-PHANTOM", name,
                        "named in the registry, no directory on disk"))


def check_links(root, out, include_vendored=False):
    """Relative markdown links in the context layer must resolve."""
    for dirpath, filenames in walk(root, include_vendored):
        for fn in filenames:
            if not fn.endswith(".md"):
                continue
            f = dirpath / fn
            try:
                text = f.read_text(encoding="utf-8", errors="replace")
            except OSError:
                out.append(("UNREADABLE", rel(root, f),
                            "file could not be read — open it with the agent's "
                            "own read tool"))
                continue
            unclosed = []
            stripped = strip_code(text, unclosed)
            if unclosed:
                out.append(("UNCLOSED-FENCE",
                            f"{rel(root, f)}:{unclosed[0]}",
                            "code fence opened here is never closed — every line "
                            "after it is blanked before the link check, so the rest "
                            "of this file is silently unchecked. Close the fence."))
            for target in LINK.findall(stripped):
                t = target.split("#", 1)[0].strip()
                if not t or t.startswith(("http://", "https://", "mailto:", "<")):
                    continue
                from urllib.parse import unquote
                t = unquote(t)
                base = root if t.startswith("/") else dirpath
                resolved = (base / t.lstrip("/")).resolve()
                if not resolved.exists():
                    out.append(("BROKEN-LINK", rel(root, f), target))


# The four required frontmatter keys, in order, with no extras — six lines total,
# so `head -7` of any topic file yields the complete relevance signal plus the
# first body line. This is deliberately a line-by-line parse and not one regex:
# a single re.S regex lets `name: .+` match across newlines, so any number of
# extra keys slips through a check the doctrine advertises as exact.
MEM_FM_KEYS = ("name", "description", "type", "last_verified")
MEM_FM_KV = re.compile(r"^([A-Za-z_][A-Za-z0-9_-]*):[ \t]*(.*)$")


def parse_topic_frontmatter(body):
    """Validate a memory topic file's six-line frontmatter block.

    Returns ((name, description, type, last_verified_date), None) on success,
    or (None, reason).
    Spec: ../references/context-cascade.md, "Memory layout — the topic store".
    """
    lines = body.split("\n")
    if not lines or lines[0].strip() != "---":
        return None, "no opening '---' on line 1"
    if len(lines) < 6:
        return None, "shorter than the required six-line frontmatter block"
    values = {}
    for i, key in enumerate(MEM_FM_KEYS, start=1):
        m = MEM_FM_KV.match(lines[i])
        if not m:
            return None, (f"line {i + 1} is not a `key: value` pair — expected "
                          f"`{key}:`, got {lines[i].strip()[:40]!r}")
        if m.group(1) != key:
            return None, (f"line {i + 1} is `{m.group(1)}:`, expected `{key}:` — the "
                          f"four keys are {' / '.join(MEM_FM_KEYS)}, in that order")
        if not m.group(2).strip():
            return None, f"`{key}:` is empty"
        values[key] = m.group(2).strip()
    if lines[5].strip() != "---":
        return None, (f"line 6 is {lines[5].strip()[:40]!r}, expected the closing "
                      f"'---' — extra frontmatter keys are not allowed")
    raw = values["last_verified"].strip("\"'")
    try:
        verified = dt.date.fromisoformat(raw)
    except ValueError:
        return None, f"`last_verified: {raw}` does not parse as a YYYY-MM-DD date"
    return (values["name"].strip("\"'"), values["description"].strip("\"'"),
            values["type"].strip("\"'"), verified), None


# The closed vocabulary from the cascade spec. Singular, always — the *file* may
# be `decisions.md`, the *type* is `decision`. Plural types are the common typo
# and they defeat any tooling that filters on type.
# The order here is the index order: alerts first because they are read at every
# level passed through, then the scope's own state, then everything an agent
# must not relitigate, then the traps, then what is open, then the debts, then
# the record-keeping types.
TOPIC_TYPE_ORDER = ("alert", "state", "decision", "gotcha", "question", "watch",
                    "kill-record", "evidence", "log", "reference")
TOPIC_TYPES = set(TOPIC_TYPE_ORDER)
# The canonical basename for each recurring type. Within a type, the canonical
# file sorts ahead of its split siblings (`decisions.md` before
# `decisions-sync.md`), so the index reads in the order a person would write it.
CANONICAL_STEMS = {"state", "decisions", "gotchas", "questions", "watch",
                   "kill-records", "alerts", "log", "evidence", "reference"}
# Past this a topic file is almost always two topics wearing one filename.
TOPIC_MAX_LINES = 60
# The line rule alone is gameable: a 57-line file can carry 22 KB because every
# line is a paragraph. Sixty lines of ~100 characters is the envelope the line
# rule always meant, so the body is capped in characters too.
TOPIC_MAX_CHARS = 6000
# The description is the index hook. It has to fit on one line of an index a
# reader skims; past this it is a summary of the body, not a relevance signal.
DESCRIPTION_MAX_CHARS = 240
TOPIC_NAME = re.compile(r"^[a-z0-9]+(?:-[a-z0-9]+)*$")
# Volatile git-shaped integers. The spec says record the topology, not the
# count: these go stale inside the session that writes them. Advisory — a
# count that is itself the hazard ("deletes 21 tracked files") is legitimate,
# and this regex is deliberately narrow so it never fires on that.
VOLATILE_COUNT = re.compile(
    r"\b\d[\d,]*\s+(?:uncommitted|dirty|unpushed|untracked)\b"
    r"|\b\d[\d,]*\s+commits?\s+(?:ahead|behind)\b"
    r"|\b(?:ahead|behind)\s+(?:by\s+)?\d[\d,]*\b"
    r"|\b\d[\d,]*\s+ahead\b|\b\d[\d,]*\s+behind\b", re.I)
H1 = re.compile(r"^#\s+(.+?)\s*$")


def scope_slug(root, scope, group_dirs=GROUP_DIRS):
    """The `name:` prefix a scope's topic files carry.

    The root is `root`. Otherwise the scope's own directory name, lowercased,
    with dots, underscores and spaces turned into dashes — except that a
    *grouping* directory is prefixed by its parent's slug, because a tree
    usually has several and `projects-state` would collide once per scope.
    """
    if scope == root:
        return "root"
    own = scope.name.lower().replace(".", "-").replace("_", "-").replace(" ", "-")
    if scope.name in group_dirs and scope.parent != root:
        return f"{scope_slug(root, scope.parent, group_dirs)}-{own}"
    return own


def index_heading(root, scope):
    """`# MEMORY — <scope path relative to the root>`; the root uses its own
    directory name, which is the only name it has."""
    rel_path = Path(scope).relative_to(root)
    return f"# MEMORY — {root.name if str(rel_path) == '.' else rel_path.as_posix()}"


def read_topics(memdir):
    """Parse every topic file in a memory/ directory.

    Returns (topics, problems): topics is a list of dicts for files whose
    frontmatter parsed; problems is a list of (file, reason) for those that
    did not. Order is by filename.
    """
    topics, problems = [], []
    for p in sorted(memdir.glob("*.md")):
        try:
            body = p.read_text(encoding="utf-8", errors="replace")
        except OSError:
            problems.append((p, "unreadable"))
            continue
        parsed, reason = parse_topic_frontmatter(body)
        if not parsed:
            problems.append((p, reason))
            continue
        name, desc, ttype, verified = parsed
        lines = body.split("\n")
        title = None
        for line in lines[6:]:
            if not line.strip():
                continue
            m = H1.match(line)
            title = m.group(1) if m else None
            break
        topics.append({"path": p, "name": name, "description": desc,
                       "type": ttype, "verified": verified, "title": title,
                       "body": "\n".join(lines[6:]), "text": body})
    return topics, problems


def render_index(root, scope, topics):
    """The one correct MEMORY.md for a scope, derived from its topic files.

    Title = the topic's H1; hook = its `description`; order = type rank, then
    canonical-file-first, then filename. Nothing in the index is authored by
    hand, so a hook cannot drift from the body it points at, and the whole file
    is regenerable with --fix-index.
    """
    rank = {t: i for i, t in enumerate(TOPIC_TYPE_ORDER)}
    ordered = sorted(topics, key=lambda t: (rank.get(t["type"], len(rank)),
                                            0 if t["path"].stem in CANONICAL_STEMS else 1,
                                            t["path"].name))
    lines = [index_heading(root, scope), ""]
    for t in ordered:
        title = (t["title"] or t["path"].stem).replace("[", "").replace("]", "")
        lines.append(f"- [{title}](memory/{t['path'].name}) — {t['description']}")
    return "\n".join(lines) + "\n"


def check_memory_freshness(root, max_age, out, fix_index=False,
                           group_dirs=GROUP_DIRS):
    """Memory is a topic store: MEMORY.md is a pure index *derived* from the
    memory/*.md topic files, whose first 6 lines are frontmatter
    (name / description / type / last_verified). head -7 of any topic file
    must be enough to judge relevance, and the index must equal exactly what
    render_index() produces from those files."""
    today = dt.date.today()
    seen_names = {}
    for dirpath, filenames in walk(root):
        if "MEMORY.md" not in filenames:
            continue
        f = dirpath / "MEMORY.md"
        r = rel(root, f)
        memdir = dirpath / "memory"
        try:
            text = f.read_text(encoding="utf-8", errors="replace")
        except OSError:
            continue
        if not memdir.is_dir():
            # Legacy monolith: still enforce the stamp, and flag for migration.
            m = re.search(r"_Last verified:\s*(\d{4}-\d{2}-\d{2})", text[:800])
            if not m:
                out.append(("NO-VERIFIED-STAMP", r, "missing the _Last verified:_ line"))
            else:
                age = (today - dt.date.fromisoformat(m.group(1))).days
                if age > max_age:
                    out.append(("STALE-MEMORY", r, f"last verified {age} days ago"))
            out.append(("MEMORY-UNMIGRATED", r,
                        "monolithic MEMORY.md — split into the memory/ topic store "
                        "(spec: ../references/context-cascade.md)"))
            continue
        topics, problems = read_topics(memdir)
        for p, reason in problems:
            out.append(("MEMORY-FRONTMATTER", rel(root, p),
                        f"{reason} — the block is exactly six lines: '---', "
                        f"name, description, type, last_verified, '---'"))
        # 1. Every topic file: known type, name shape, heading, size, freshness.
        slug = scope_slug(root, dirpath, group_dirs)
        for t in topics:
            prel = rel(root, t["path"])
            if t["type"] not in TOPIC_TYPES:
                out.append(("MEMORY-TOPIC-TYPE", prel,
                            f"type: {t['type']!r} is outside the vocabulary "
                            f"({', '.join(TOPIC_TYPE_ORDER)})"))
            expected = f"{slug}-{t['path'].stem}"
            if not TOPIC_NAME.match(t["name"]) or t["name"] != expected:
                out.append(("MEMORY-TOPIC-NAME", prel,
                            f"name: {t['name']!r} — expected {expected!r} "
                            f"(<scope slug>-<file stem>, kebab-case)"))
            if t["name"] in seen_names:
                out.append(("MEMORY-TOPIC-NAME", prel,
                            f"name: {t['name']!r} is also used by {seen_names[t['name']]}"))
            seen_names.setdefault(t["name"], prel)
            if not t["title"]:
                out.append(("MEMORY-TOPIC-NO-HEADING", prel,
                            "the first body line is not a `# Heading` — the index "
                            "title is derived from it"))
            elif t["type"] == "alert" and not t["title"].startswith("Red — "):
                out.append(("MEMORY-ALERT-HEADING", prel,
                            f"an alert's heading begins `Red — ` so the hazard reads "
                            f"as one in the index; got {t['title'][:50]!r}"))
            if len(t["description"]) > DESCRIPTION_MAX_CHARS:
                out.append(("MEMORY-DESCRIPTION-LONG", prel,
                            f"description is {len(t['description'])} chars (max "
                            f"{DESCRIPTION_MAX_CHARS}) — it is the index hook, one "
                            f"line of signal, not a summary of the body"))
            n_lines = len(t["text"].rstrip("\n").split("\n"))
            if n_lines > TOPIC_MAX_LINES:
                out.append(("MEMORY-TOPIC-LONG", prel,
                            f"{n_lines} lines (max ~{TOPIC_MAX_LINES}) — this is "
                            f"probably two topics; split it or prune it"))
            n_chars = len(t["body"])
            if n_chars > TOPIC_MAX_CHARS:
                out.append(("MEMORY-TOPIC-HEAVY", prel,
                            f"body is {n_chars} chars (max {TOPIC_MAX_CHARS}) — "
                            f"the line rule is being met with paragraphs; split "
                            f"it or prune it"))
            age = (today - t["verified"]).days
            if age > max_age:
                out.append(("STALE-MEMORY", prel, f"last verified {age} days ago"))
            if t["verified"] > today:
                out.append(("MEMORY-FRONTMATTER", prel,
                            f"last_verified {t['verified']} is in the future"))
            for m in VOLATILE_COUNT.finditer(strip_code(t["body"])):
                out.append(("MEMORY-VOLATILE-COUNT", prel,
                            f"{m.group(0)!r} — record the topology, not the "
                            f"integer; the audit re-measures counts"))
        # 2. The index must be exactly what the topic files derive to.
        expected_index = render_index(root, dirpath, topics)
        if text != expected_index:
            linked = set(re.findall(r"\((memory/[^)]+\.md)\)", text))
            on_disk = {f"memory/{p.name}" for p in memdir.glob("*.md")}
            for miss in sorted(linked - on_disk):
                out.append(("MEMORY-LINK-BROKEN", r, f"index links {miss}, not on disk"))
            for orph in sorted(on_disk - linked):
                out.append(("MEMORY-ORPHAN-TOPIC", r, f"{orph} exists but is not indexed"))
            if fix_index and not problems:
                f.write_text(expected_index, encoding="utf-8")
                out.append(("MEMORY-INDEX-REWRITTEN", r,
                            "regenerated from the topic files' frontmatter"))
            else:
                why = ("cannot regenerate while a topic has malformed frontmatter"
                       if problems else
                       "run with --fix-index to regenerate it")
                exp_lines = expected_index.splitlines()
                got_lines = text.splitlines()
                first = next((i for i, (a, b) in enumerate(zip(got_lines, exp_lines))
                              if a != b), min(len(got_lines), len(exp_lines)))
                out.append(("MEMORY-INDEX-STALE", r,
                            f"index differs from what the topic files derive to "
                            f"(first difference at line {first + 1}) — {why}"))


MEMORY_REF = re.compile(r"\[[^\]]*\]\([^)]*MEMORY\.md[^)]*\)|\bMEMORY\.md\b")


def check_split(root, out):
    """State asserted inside an instruction file is a misfiling defect.

    The `MEMORY.md` exemption is narrow on purpose. A line that merely *points*
    at `MEMORY.md` usually has to name the thing it is routing ("anything with a
    date, a status or a 'currently' goes there"), and flagging those trains
    agents to ignore this finding. But such an exemption easily swallows the
    whole line, so a **date** — which is state under any reading — hides behind a
    mention of that filename. One tree's root instruction file carried exactly
    that for a month. So: the state-word branch is exempted, the date branch
    never is.
    """
    for dirpath, filenames in walk(root):
        f = instruction_file(dirpath, filenames)
        if f is None:
            continue
        try:
            lines = f.read_text(encoding="utf-8", errors="replace").splitlines()
        except OSError:
            continue
        for i, line in enumerate(lines, 1):
            if line.lstrip().startswith(("|", ">")):
                continue
            bare = re.sub(r"`[^`]*`", "", line)
            # Strip the routing reference itself, not the line that carries it.
            bare = MEMORY_REF.sub("", bare)
            hit = DATE_TOKEN.search(bare) or \
                ("MEMORY.md" not in line and STATE_TOKENS.search(bare))
            if hit:
                out.append(("POSSIBLE-STATE-IN-INSTRUCTIONS",
                            f"{rel(root, f)}:{i}", line.strip()[:100]))


def check_cloud_dupes(root, out):
    n = 0
    for dirpath, filenames in walk(root):
        for fn in filenames:
            if CLOUD_DUP.search(fn):
                n += 1
    if n:
        out.append(("SYNC-CONFLICT-DUPES", str(root.name),
                    f"{n} ' 2.ext' sync-conflict duplicate files in the context layer "
                    "(cloud-sync tools create these; the copy is usually the stale one)"))


def check_upkeep(root, out):
    """Every instruction file in the cascade must carry the standing upkeep duty."""
    for dirpath, filenames in walk(root):
        f = instruction_file(dirpath, filenames)
        if f is None:
            continue
        try:
            text = f.read_text(encoding="utf-8", errors="replace")
        except OSError:
            continue
        if not re.search(r"^##+ Upkeep", text, re.M):
            out.append(("UPKEEP-MISSING", rel(root, f),
                        "no '## Upkeep' section — the standing duty to keep the "
                        "instruction file / MEMORY.md / skills current is not "
                        "stated here"))


# Supporting material a skill set may carry, which is not itself a skill.
SKILL_SUPPORT_DIRS = ("references", "assets", "scripts", "templates")
SKILL_FM_KV = re.compile(r"^([A-Za-z_][A-Za-z0-9_-]*):[ \t]*(.*)$")


def _slug(text):
    return re.sub(r"[^a-z0-9]+", " ", (text or "").lower()).strip()


def parse_skill_frontmatter(body):
    """Return (name, description) from a SKILL.md, or (None, reason).

    Handles a folded/literal scalar (`description: >-` and friends) by taking
    the indented continuation lines, because that shape is legal YAML and a
    parser that rejects it produces false SKILL-MALFORMED findings — which is
    how a check ends up ignored.
    """
    lines = body.split("\n")
    if not lines or lines[0].strip() != "---":
        return None, ("no opening '---' on line 1 — the frontmatter delimiter is "
                      "missing, so the loader never registers this skill")
    values = {}
    i, closed = 1, False
    while i < len(lines):
        line = lines[i]
        if line.strip() == "---":
            closed = True
            break
        m = SKILL_FM_KV.match(line)
        if m:
            key, val = m.group(1), m.group(2).strip()
            if val in (">", ">-", ">+", "|", "|-", "|+"):
                parts, i = [], i + 1
                while i < len(lines) and lines[i].strip() != "---" \
                        and (not lines[i].strip() or lines[i][:1] in (" ", "\t")):
                    parts.append(lines[i].strip())
                    i += 1
                val = " ".join(p for p in parts if p).strip()
                values[key] = val
                continue
            values[key] = val
        i += 1
    if not closed:
        return None, "frontmatter block is never closed by a '---' line"
    if "name" not in values:
        return None, "no `name:` key in the frontmatter"
    if not values["name"]:
        return None, "`name:` is empty"
    if "description" not in values:
        return None, ("no `description:` key — without one the model has nothing "
                      "to match a request against and the skill never fires")
    if not values["description"]:
        return None, ("`description:` is empty — without one the model has nothing "
                      "to match a request against and the skill never fires")
    return (values["name"], values["description"]), None


def check_skill_indexes(root, out, include_vendored=False):
    """Every skills directory needs a README.md index; every skill needs a
    well-formed SKILL.md.

    Two failures this used to be blind to by construction:

    * It tested only that a skill directory *contained* a `SKILL.md` and never
      opened the file. A `SKILL.md` whose frontmatter delimiter is missing, or
      whose `name` disagrees with its directory, does not load — and looked
      perfect to the audit. Corrupt frontmatter was undetectable, not merely
      unreported.
    * It called `walk(root)` without the vendored flag, so every
      `.github/skills/` tree was skipped even under `--all`. A repository that
      mirrors its skills for a second tooling ecosystem had exactly half of them
      checked, and the mirror is where drift lands first.
    """
    for dirpath, filenames in walk(root, include_vendored):
        if dirpath.name != "skills" or dirpath.parent.name not in (".claude", ".github"):
            continue
        if not (dirpath / "README.md").is_file():
            out.append(("SKILLS-INDEX-MISSING", rel(root, dirpath),
                        "skills directory with no README.md index"))
        for sub in sorted(dirpath.iterdir()):
            if not sub.is_dir() or sub.name in SKILL_SUPPORT_DIRS:
                continue
            skill = sub / "SKILL.md"
            if not skill.is_file():
                out.append(("SKILL-MALFORMED", rel(root, sub),
                            "skill directory with no SKILL.md"))
                continue
            try:
                body = skill.read_text(encoding="utf-8", errors="replace")
            except OSError:
                out.append(("UNREADABLE", rel(root, skill),
                            "file could not be read — open it with the agent's "
                            "own read tool"))
                continue
            parsed, reason = parse_skill_frontmatter(body)
            if not parsed:
                out.append(("SKILL-MALFORMED", rel(root, skill), reason))
                continue
            name, description = parsed
            if name != sub.name:
                out.append(("SKILL-MALFORMED", rel(root, skill),
                            f"`name: {name}` does not match its directory "
                            f"{sub.name!r} — the two must be identical or the "
                            f"skill resolves under a name nothing references"))
            if _slug(description) == _slug(name) or (
                    _slug(name) in _slug(description)
                    and len(description) < len(name) + 12):
                out.append(("SKILL-MALFORMED", rel(root, skill),
                            "`description:` only restates the name — a description "
                            "must say what the skill provides, how it is used and "
                            "when it fires, or the model has nothing to match on"))


def check_scratch(root, out, include_vendored=False):
    """A `.tmp/` that still holds something, and tool residue that never got one.

    TMP-NOT-EMPTY is the finding that catches an unclosed session: the work is
    done, the scratch is not. Anything in there that still mattered was never
    temporary — promote it (memory store / instruction file / skill / docs), then
    delete the file. Promote first, delete second; outside the git repositories
    there is no undo.
    """
    skip = (SKIP_DIRS if include_vendored else SKIP_DIRS | VENDORED_DIRS) - {TMP_DIR}
    for dirpath, dirnames, _ in os.walk(root):
        keep = []
        for d in dirnames:
            p = Path(dirpath) / d
            if d == TMP_DIR:
                try:
                    live = [e for e in os.listdir(p) if not TMP_NOISE.match(e)]
                except OSError:
                    continue
                if live:
                    out.append(("TMP-NOT-EMPTY", rel(root, p),
                                f"{len(live)} item(s) left in scratch — the session that "
                                f"wrote them is not closed. Promote anything that still "
                                f"matters, then empty it"))
                continue  # never descend into scratch
            if d in RESIDUE_DIRS:
                out.append(("STRAY-ARTIFACT", rel(root, p),
                            "tool-residue directory — belongs in a .tmp/ and should "
                            "not have survived the session"))
                continue
            if d not in skip:
                keep.append(d)
        dirnames[:] = keep


def check_stray_artifacts(root, scopes, out, include_vendored=False,
                          group_dirs=GROUP_DIRS):
    """Loose temporary artefacts sitting in the tree instead of in a `.tmp/`.

    Deliberately narrow. Three surfaces only — the root, scope roots, nested
    scope roots — and every `docs/` tree, because those are where debris is
    *invisible*: a `.log` under a project's `src/` is that project's build output
    and its own `.gitignore`'s problem, while a `.log` at a scope root is residue
    nobody will ever look for again.

    Images are checked at the root and at scope roots, but **not** at nested
    scope roots — repositories legitimately carry a `README.icon.png` there.
    """
    def flag(p, why):
        out.append(("STRAY-ARTIFACT", rel(root, p), why))

    def top_level(d, images=True):
        if not d.is_dir():
            return
        try:
            entries = sorted(d.iterdir())
        except OSError:
            return
        for p in entries:
            if not p.is_file():
                continue
            ext = p.suffix.lower()
            if ext in DEBRIS_EXT or (images and ext in IMAGE_EXT):
                flag(p, f"loose {ext} artefact outside a .tmp/ — put it in "
                        f"{d.name}/.tmp/ or promote it and delete it")

    top_level(root)
    for scope in scopes:
        top_level(scope)
        for p in sub_scopes(scope, group_dirs):
            top_level(p, images=False)

    for dirpath, filenames in walk(root, include_vendored):
        parts = dirpath.relative_to(root).parts
        if "docs" not in parts:
            continue
        below = parts[parts.index("docs") + 1:]
        curated = any(part in ASSET_DIRS for part in below)
        for fn in filenames:
            f = dirpath / fn
            ext = f.suffix.lower()
            if ext in DEBRIS_EXT:
                flag(f, f"loose {ext} artefact in the reference layer — docs/ is "
                        f"for reference content, not run output")
            elif ext in IMAGE_EXT and not curated:
                flag(f, "image in docs/ outside a curated asset directory — a "
                        "capture belongs in .tmp/, a real asset in an assets/ "
                        "or design/ subdirectory")
            elif ext == ".md" and DATED_FILENAME.search(fn):
                flag(f, "dated filename in docs/ — this is a session report, and "
                        "docs/ is a reference layer, not an archive. A report is "
                        "not a memory: promote the findings to the memory store, "
                        "then delete it")


def check_tmp_tracked(root, scopes, out, group_dirs=GROUP_DIRS):
    """A `.tmp/` path committed into a repository.

    Means that repository's `.gitignore` is missing the rule, so scratch is being
    pushed to a remote rather than emptied. An outer tree's `.gitignore` enforces
    nothing on a nested repository, so this is only detectable per repository.
    """
    for repo in _repos(root, scopes, group_dirs):
        listing = _git(repo, "ls-files")
        if not listing:
            continue
        tracked = [p for p in listing.splitlines()
                   if TMP_DIR in Path(p).parts]
        if tracked:
            out.append(("TMP-TRACKED", rel(root, repo),
                        f"{len(tracked)} .tmp/ path(s) tracked by git — e.g. "
                        f"{tracked[0]}. Add `.tmp/` to this repo's .gitignore and "
                        f"`git rm --cached` them"))


def _git(repo, *args):
    import subprocess
    try:
        r = subprocess.run(("git", "-C", str(repo)) + args, capture_output=True,
                           text=True, timeout=25)
        return r.stdout.strip() if r.returncode == 0 else None
    except Exception:
        return None


def _repos(root, scopes, group_dirs=GROUP_DIRS):
    seen = set()
    candidates = [root]
    for scope in scopes:
        candidates.append(scope)
        candidates.extend(sub_scopes(scope, group_dirs))
    for p in candidates:
        key = str(p)
        if key in seen:
            continue
        seen.add(key)
        if (p / ".git").exists():
            yield p


def check_git_health(root, scopes, out, destructive_threshold=8,
                     group_dirs=GROUP_DIRS):
    """Repo-durability checks. Every one of these has fired for real.

    LOCK-DEBRIS      stale .git/*.lock silently blocks every commit
    GIT-DIVERGED     local branch has commits the remote does not AND vice versa
                     — a plain push is rejected and --force destroys the remote
    GIT-UNPUSHED     work that exists on exactly one machine
    GIT-DESTRUCTIVE  an unpushed commit that deletes many tracked files
    GIT-DIRTY        uncommitted working tree
    """
    for repo in _repos(root, scopes, group_dirs):
        r = rel(root, repo)
        locks = sorted((repo / ".git").glob("*.lock"))
        if locks:
            out.append(("GIT-LOCK-DEBRIS", r,
                        "stale " + ", ".join(l.name for l in locks) +
                        " — blocks every commit until removed"))
        dirty = _git(repo, "status", "--porcelain")
        if dirty:
            out.append(("GIT-DIRTY", r, f"{len(dirty.splitlines())} uncommitted path(s)"))
        upstream = _git(repo, "rev-parse", "--abbrev-ref", "--symbolic-full-name", "@{u}")
        if not upstream:
            if _git(repo, "remote"):
                out.append(("GIT-NO-UPSTREAM", r,
                            "remote configured but the branch tracks nothing — never pushed"))
            else:
                out.append(("GIT-NO-REMOTE", r,
                            "no remote — this repo exists on one machine only"))
            continue
        counts = _git(repo, "rev-list", "--left-right", "--count", f"{upstream}...HEAD")
        if counts:
            behind, ahead = (int(x) for x in counts.split())
            if behind and ahead:
                out.append(("GIT-DIVERGED", r,
                            f"{ahead} ahead / {behind} behind {upstream} — a plain push is "
                            f"rejected and --force destroys the remote; do NOT run a generic "
                            f"push runbook against this repo"))
            elif ahead:
                out.append(("GIT-UNPUSHED", r, f"{ahead} commit(s) ahead of {upstream}"))
            if ahead:
                stat = _git(repo, "diff", "--name-status", f"{upstream}..HEAD")
                dels = [l for l in (stat or "").splitlines() if l.startswith("D")]
                if len(dels) >= destructive_threshold:
                    out.append(("GIT-DESTRUCTIVE-UNPUSHED", r,
                                f"unpushed commits delete {len(dels)} tracked file(s) — "
                                f"verify this is intended before any push"))


def main():
    ap = argparse.ArgumentParser(
        description="Mechanical drift checks over a repository's context layer.")
    ap.add_argument("--root", default=None,
                    help="tree root (default: the current working directory)")
    ap.add_argument("--scope", action="append", default=[], metavar="PATH",
                    help="a directory carrying its own context pair; repeat once "
                         "per scope. Omit to auto-discover every immediate "
                         "subdirectory that already carries the pair")
    ap.add_argument("--group-dir", action="append", default=[], metavar="NAME",
                    help="name of a directory whose only job is holding scopes; "
                         "repeat once per name in use. Default: "
                         + ", ".join(GROUP_DIRS))
    ap.add_argument("--instruction-file", default=None, metavar="NAME",
                    help="the filename that actually carries this tree's rules "
                         "(default order: " + ", ".join(INSTRUCTION_FILES) +
                         "). Pass AGENTS.md on a tree where CLAUDE.md is only "
                         "an interop pointer, or the pointer is audited instead")
    ap.add_argument("--registry", default=None, metavar="PATH",
                    help="markdown file holding a project registry to reconcile "
                         "against what is on disk; omit to skip the check")
    ap.add_argument("--max-age", type=int, default=60)
    ap.add_argument("--no-git", action="store_true",
                    help="skip the repo-durability checks (git subprocess calls)")
    ap.add_argument("--all", action="store_true",
                    help="also check vendored/imported trees "
                         "(" + ", ".join(sorted(VENDORED_DIRS)) + ")")
    ap.add_argument("--fix-index", action="store_true",
                    help="rewrite every MEMORY.md that differs from what its "
                         "memory/ topic files derive to (title = the topic's H1, "
                         "hook = its description, order = type rank). The index "
                         "is generated, never hand-written")
    args = ap.parse_args()
    root = Path(args.root).resolve() if args.root else Path.cwd().resolve()
    if not root.is_dir():
        ap.error(f"--root is not a directory: {root}")
    if args.instruction_file:
        set_instruction_file(args.instruction_file)
    group_dirs = tuple(args.group_dir) if args.group_dir else GROUP_DIRS

    out = []
    scopes = resolve_scopes(root, args.scope, out, args.all)
    check_pairs(root, scopes, out, group_dirs)
    if args.registry:
        check_registry(root, args.registry, scopes, out, group_dirs)
    check_links(root, out, args.all)
    check_memory_freshness(root, args.max_age, out, args.fix_index, group_dirs)
    check_split(root, out)
    check_cloud_dupes(root, out)
    check_upkeep(root, out)
    check_skill_indexes(root, out, args.all)
    check_scratch(root, out, args.all)
    check_stray_artifacts(root, scopes, out, args.all, group_dirs)
    if not args.no_git:
        check_git_health(root, scopes, out, group_dirs=group_dirs)
        check_tmp_tracked(root, scopes, out, group_dirs)

    if not out:
        print(f"CLEAN — context layer passes all mechanical checks ({root}).")
        return 0

    by_kind = {}
    for kind, where, detail in out:
        by_kind.setdefault(kind, []).append((where, detail))
    print(f"Context audit — {root}\n")
    for kind in sorted(by_kind):
        items = by_kind[kind]
        print(f"## {kind} ({len(items)})")
        for where, detail in items[:40]:
            print(f"  {where}\n      {detail}")
        if len(items) > 40:
            print(f"  ... and {len(items) - 40} more")
        print()
    print(f"{len(out)} finding(s). Advisory — POSSIBLE-STATE-IN-INSTRUCTIONS in "
          f"particular needs a human or agent read, not a blind fix. GIT-DIVERGED "
          f"and GIT-DESTRUCTIVE-UNPUSHED are the two that can lose work: read "
          f"them first.")
    return 1


if __name__ == "__main__":
    sys.exit(main())
