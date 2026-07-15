# VERSION_LIFECYCLE_PROTOCOL.md

> **Purpose:** Official Teranoo protocol for application version, release, maintenance, and hotfix lifecycle management.
> **Layer:** Tera Version Management Layer.
> **Status:** Active runtime protocol.

---

## 1. Core Rule

```text
No released version may be modified without opening a Version Cycle, Maintenance Cycle, or Hotfix Cycle.
```

Arabic operational wording:

> لا يتم تعديل أي نسخة مسلّمة إلا من خلال دورة نسخة جديدة، أو دورة صيانة، أو دورة إصلاح عاجل.

---

## 2. Release Types

| Type | Meaning | Scope Rule |
|---|---|---|
| `Initial` | First delivered application version, usually `v1.0` | Uses full 7-phase workflow |
| `Hotfix` | Urgent production fix after release | No new features allowed |
| `Patch` | Small bug fixes or safe improvements | No major workflow or module changes |
| `Minor` | New features within the same product direction | Requires version decision and planning |
| `Major` | New module, major workflow, breaking change, or new product scope | Requires updated project decision and fuller planning |

---

## 3. Required Project-Control Records

For projects using version management, Tera must maintain:

```text
project-control/VERSION_REGISTRY.md
project-control/RELEASE_NOTES.md
project-control/NEXT_VERSION_HANDOFF.md
project-control/PROJECT_STATE.md
project-control/TASK_REGISTRY.md
project-control/tasks/TASK_TEMPLATE.md
```

Rules:

- No new version starts without updating `VERSION_REGISTRY.md`.
- Every task must record `Target Version` and `Release Type`.
- Every release, patch, or hotfix must update `RELEASE_NOTES.md`.
- Phase 7 must produce or update `NEXT_VERSION_HANDOFF.md` unless the application reaches Final Application Closure.
- Deferred features must be linked to a target version or explicitly marked `Unassigned`.

---

## 4. Version Cycle Entry Rules

### 4.0 Version Scope Proposal Ownership

During Phase 2, Tera is responsible for proposing what enters each application version based on:

- Phase 1 discovery information.
- `MVP_DEFINITION_PROTOCOL.md` classification.
- Client/user priorities.
- Risks, dependencies, and implementation complexity.
- Anti-bloat and smallest useful version rules.

The user/client is responsible for approving, rejecting, or modifying the proposal.

```text
Client provides needs -> Tera proposes version scope -> User/client approves -> Tera documents and executes.
```

No version scope becomes official until it is approved and recorded in the relevant project-control records, especially `VERSION_REGISTRY.md` and the Phase 2 decision record.

### 4.0.1 Version Scope Change Requests

If the user/client asks to move a feature, screen, workflow, module, or requirement from one version to another, Tera must classify it as a `Version Scope Change`.

Examples:

```text
Move Feature X from v1.1 to v1.0.
Move reports from v2.0 to v1.1.
Move mobile support out of v1.0 to a later version.
```

Rules:

- Tera is responsible for updating the version documentation and execution records after approval.
- If the target version is not released yet, Tera may update that version's approved scope, plans, task registry, and release notes preparation after user/client approval.
- If the target version is already released, Tera must not rewrite the released version history or tag. Instead, Tera must open the appropriate `Patch`, `Hotfix`, `Minor`, or `Major` cycle.
- Moving a feature into an earlier unreleased version requires checking MVP impact, schedule impact, risks, dependencies, preparation files, and client approval needs.
- Moving a feature out of a version requires updating deferred items and the target version in `VERSION_REGISTRY.md`.
- The original version history must remain traceable.

Required records when applicable:

```text
VERSION_REGISTRY.md
PROJECT_STATE.md
TASK_REGISTRY.md
RELEASE_NOTES.md
PROJECT_MASTER_PLAN.md / PROJECT_DETAILED_EXECUTION_PLAN.md
Phase 2 decision record / VERSION_DECISION.md when used
```

### 4.1 First Version (`v1.0`)

Default path:

```text
Phase 1 -> Phase 2 -> Phase 3 -> Phase 4 -> Phase 5 -> Phase 6 -> Phase 7
```

The first version must define:

- `Current App Version`
- `Release Type: Initial`
- Core MVP scope
- Deferred features and target versions where known

### 4.2 New Minor / Major Version

Before starting `v1.1`, `v2.0`, or later:

1. Read `VERSION_REGISTRY.md`.
2. Read `NEXT_VERSION_HANDOFF.md` if it exists.
3. Classify release type: `Minor` or `Major`.
4. Update or create a version decision record in project-control files.
5. Update `PROJECT_STATE.md` and `TASK_REGISTRY.md` version fields.
6. Continue with the smallest sufficient phase path:
   - Minor: usually Phase 2 update -> Phase 3/5 as needed -> Phase 6 -> Phase 7.
   - Major: usually updated Phase 2 -> Phase 3 -> Phase 4 if needed -> Phase 5 -> Phase 6 -> Phase 7.

### 4.3 Hotfix / Patch Cycle

Before starting a hotfix or patch:

1. Classify the request as Bug / Hotfix / Patch / Minor Feature / Major Version.
2. Confirm affected released version.
3. Confirm no new feature is included in hotfix scope.
4. Create a versioned fix task using `TASK-COD-FIX-vX.Y.Z-001` or equivalent approved format.
5. Run Pre-Execution Gate.
6. Execute in Phase 6.
7. Run Post-Execution Review.
8. Update `RELEASE_NOTES.md`, `VERSION_REGISTRY.md`, and `PROJECT_STATE.md`.
9. Run compact Phase 7 closure for the hotfix/patch.

---

## 5. Versioned TASK-ID Rule

Default versioned format:

```text
TASK-COD-v1.0-001
TASK-PREP-v1.0-001
TASK-REV-v1.0-001
TASK-COD-FIX-v1.0.1-001
```

Rules:

- Keep the prefix family (`TASK-COD-*`, `TASK-PREP-*`, `TASK-COD-FIX-*`) compatible with existing Teranoo gates.
- Do not create duplicate task IDs for the same target version.
- Read the last used task ID for the target version before creating the next one.
- `TASK-COD-FIX-*` is for accepted fix work returning from Phase 7 or production maintenance.

---

## 6. Phase 7 Closure Types

Phase 7 must distinguish between:

| Closure Type | Meaning |
|---|---|
| `Version Closure` | A delivered version such as `v1.0` is accepted/released |
| `Maintenance Closure` | Patch or non-urgent maintenance cycle is closed |
| `Hotfix Closure` | Urgent fix version such as `v1.0.1` is released |
| `Final Application Closure` | The whole application lifecycle is closed with no planned next version |

Rules:

- Version Closure is not automatically Final Application Closure.
- No Final Application Closure with hidden open issues or undocumented deferred items.
- If a next version is likely, update `NEXT_VERSION_HANDOFF.md`.

---

## 7. Git Release Tagging + GitHub Releases Protocol

Git release tagging is the official code-level anchor for a delivered application version. GitHub Releases are the official repository-facing release record when the project remote is hosted on GitHub.

### 7.1 Responsibility Model

| Responsibility | Owner |
|---|---|
| Inspecting git status/diff/log before release commit | Tera |
| Preparing commit message and local commit | Tera |
| Confirming remote URL from `project-control/GIT_REMOTE.md` | Tera |
| Asking for push/tag approval | Tera |
| Approving or rejecting push/tag | User / Majed / authorized client owner |
| Executing `git push`, `git tag`, and tag push after approval | Tera |
| Creating GitHub Release after tag push approval | Tera |
| Recording git/tag result in project-control files | Tera |

User responsibility is approval. Tera responsibility is safe repository handling, release tagging, GitHub Release creation, logging, and version traceability.

### 7.2 Release Tag Preconditions

Before creating a Git release tag:

- Phase 7 Delivery Readiness is `READY` or the closure decision allows release.
- `VERSION_REGISTRY.md` is updated for the target version.
- `RELEASE_NOTES.md` contains the target version entry.
- `PROJECT_CLOSURE_REPORT.md` records Closure Type and Closed Version.
- No secrets are staged or committed.
- `project-control/GIT_REMOTE.md` matches the intended repository.
- User has explicitly approved push/tag action.
- If GitHub Release is requested, GitHub remote is confirmed and release notes are ready.

### 7.3 Default Git Release Flow

```text
review status/diff/log
-> git add .
-> git commit -m "Release vX.Y"
-> ask user approval to push
-> git push
-> ask user approval to create/push tag
-> git tag -a vX.Y -m "Release vX.Y"
-> git push origin vX.Y
-> ask user approval to create GitHub Release
-> gh release create vX.Y --title "Release vX.Y" --notes-file [release-notes-file]
-> update VERSION_REGISTRY / RELEASE_NOTES / PROJECT_CLOSURE_REPORT / PROJECT_ACTIVITY_LOG
```

For hotfixes:

```text
git checkout -b hotfix/vX.Y.Z vX.Y
```

Only after the user approves the hotfix cycle and Tera confirms the target base tag.

### 7.4 Tag Rules

- Tag names should match the application version: `v1.0`, `v1.0.1`, `v1.1`, `v2.0`.
- No force-push.
- No deleting or rewriting release tags without explicit emergency approval.
- No tag if Phase 7 is blocked.
- If tag creation is deferred, record why in `VERSION_REGISTRY.md` and `PROJECT_CLOSURE_REPORT.md`.
- Git tags restore code state, not database state, secrets, uploaded files, or server configuration. Release notes must record migration/deployment notes when relevant.

### 7.5 GitHub Release Rules

- GitHub Release creation is optional per release but recommended for every delivered version after the Git tag is pushed.
- No GitHub Release without a matching Git tag.
- No GitHub Release without explicit user approval.
- GitHub Release notes must come from `project-control/RELEASE_NOTES.md` or a generated release-note excerpt for the target version.
- If `gh` CLI is unavailable or unauthenticated, record GitHub Release as `Deferred` and keep the Git tag as the source of truth.
- If the repository is not hosted on GitHub, record GitHub Release as `N/A`.
- GitHub Releases must not contain secrets, credentials, private environment values, or unapproved client-sensitive information.

---

## 8. Deferred Level 3 Expansion

The following expansion is intentionally deferred until needed:

```text
project-control/versions/
/tera-new-version
/tera-hotfix
/tera-maintenance
deprecation / end-of-life workflows
parallel version support
```

Activation triggers:

- Large or ERP project.
- Multiple active versions.
- Frequent hotfix or maintenance cycles.
- Need to archive each version separately.
- Client requires formal version-by-version approval records.

Until then, `VERSION_REGISTRY.md`, `RELEASE_NOTES.md`, `NEXT_VERSION_HANDOFF.md`, and versioned task fields are the official lightweight layer.

---

## 9. Anti-Bloat Rule

Do not create per-version folders or slash commands until the Level 3 activation trigger exists and the user approves it.
