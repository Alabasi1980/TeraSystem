# VERSION_REGISTRY.md

> **Purpose:** Central registry for all application versions, releases, patches, hotfixes, support status, and deferred features.
> **Source Protocol:** `tera-system/runtime/VERSION_LIFECYCLE_PROTOCOL.md`

---

## 1. Current Version

- **Active Version:** v1.0
- **Status:** Planned / In Development / Released / Maintenance / Closed
- **Release Type:** Initial / Hotfix / Patch / Minor / Major
- **Release Date:** YYYY-MM-DD / TBD
- **Support Status:** Not Started / Active / Maintenance Only / Deprecated / Ended
- **Final Application Status:** Open / Final Closure Planned / Final Closed

---

## 2. Version History

| Version | Type | Status | Start Date | Release Date | Git Tag | Commit Hash | GitHub Release | Scope Summary | Support Status |
|---|---|---|---|---|---|---|---|---|---|
| v1.0 | Initial | Planned | YYYY-MM-DD | TBD | v1.0 / Pending | Pending | Pending / Deferred / N/A | Core MVP / First approved release | Not Started |

---

## 3. Deferred Features by Version

| Feature / Item | Target Version | Source | Status | Notes |
|---|---|---|---|---|
|  | v1.1 / v2.0 / Unassigned | Phase 1 Roadmap / Client Request / Issue / Decision | Deferred / Planned / Cancelled / Delivered |  |

---

## 4. Maintenance / Hotfix History

| Version | Affected Version | Type | Status | Summary | Linked Task(s) | Release Notes Updated |
|---|---|---|---|---|---|---|
|  | v1.0 | Hotfix / Patch | Planned / Released / Cancelled |  |  | Yes / No / N/A |

---

## 5. Version Management Notes

- No released version may be modified without opening a Version Cycle, Maintenance Cycle, or Hotfix Cycle.
- Level 3 expansion (`project-control/versions/`, `/tera-new-version`, `/tera-hotfix`, `/tera-maintenance`) is intentionally deferred until a large project, frequent releases, or parallel version support requires it.

---

## 6. Git Release Tags

| Version | Git Tag | Commit Hash | Push Status | Tag Push Status | GitHub Release Status | GitHub Release URL | Approved By | Notes |
|---|---|---|---|---|---|---|---|---|
| v1.0 | v1.0 / Pending | Pending | Pending / Pushed / Deferred | Pending / Pushed / Deferred | Pending / Published / Deferred / N/A |  | User / Client / N/A |  |

Rules:

- Tera manages git status/diff/log review, commit preparation, remote verification, tag creation, and tag push after approval.
- Tera manages GitHub Release creation after the tag is pushed and after user approval.
- User/Majed approves push, tag push, and GitHub Release creation; user does not need to manage repository mechanics manually.
- No force push and no release tag rewrite without explicit emergency approval.
