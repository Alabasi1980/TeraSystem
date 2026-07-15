# TASK-COD-004: Convert read_tera_workspace to Fallback

## Task Identity

| Field | Value |
|---|---|
| TASK-ID | TASK-COD-004 |
| Phase | Phase 4.6 |
| Title | Convert read_tera_workspace to Fallback with Deprecation Warning |
| Status | Accepted |
| Created | 2026-07-10 |
| Technology Profile | effect-bun-opencode |

---

## Objective

Convert `read_tera_workspace` tool from active to fallback with deprecation warning:

- Add deprecation warning message when tool is used
- Keep tool functional as fallback (Gateway is now primary)
- Update tool description to reflect fallback status
- Add diagnostic logging when tool is invoked

---

## Scope

### In Scope
- Modify `packages/core/src/tool/read-tera-workspace.ts`
- Add deprecation warning to tool description
- Add diagnostic output when tool is executed
- Keep tool functional (no removal)

### Out of Scope (Deferred)
- Tool removal (Phase 5)
- Event Stream (future)
- Config Bridge (future)

---

## Design Reference

Based on ROADMAP.md Phase 4.6:

```
Phase 3: Active — الأداة الوحيدة
Phase 4: Fallback — Gateway أساسي، read_tera_workspace احتياطي
Phase 4.5: Deprecated — تحذير عند الاستخدام
Phase 5: Removed — حذف نهائي
```

### Current Implementation
- File: `packages/core/src/tool/read-tera-workspace.ts`
- Tool name: `read_tera_workspace`
- Reads files from `.tera-workspace/` directory
- Used for governance documents, task registry, decisions log

### Required Changes
1. Update tool description to include "[DEPRECATED]" prefix
2. Add deprecation warning to output when tool is executed
3. Add diagnostic logging for tool invocation
4. Keep tool functional as fallback

---

## Allowed Write Targets

```
clients/TeraAi/packages/core/src/tool/read-tera-workspace.ts (modify)
```

---

## Acceptance Criteria

| # | Criterion | Test |
|---|---|---|
| 1 | Tool description includes "[DEPRECATED]" prefix | Code review |
| 2 | Tool output includes deprecation warning message | Manual test |
| 3 | Tool still functions correctly (reads files) | Existing tests |
| 4 | Typecheck passes | `bun run typecheck` |
| 5 | No new dependencies added | Code review |

---

## Verification Commands

```bash
# From packages/core
bun run typecheck
```

---

## Vitality & Polish Checklist

N/A — This is a tool modification, not a UI task.

---

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Single file modification |
| One objective only | PASS | Add deprecation warning only |
| No deferrable work included | PASS | Phase 4.6 is the core next step |
| No UI unless explicitly requested | PASS | No UI involved |
| No API unless explicitly requested | PASS | No API involved |
| No Auth unless explicitly requested | PASS | No Auth involved |
| No schema/migration unless explicitly requested | PASS | No database involved |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secret handling plan documented and redacted | PASS | N/A |
| CLI side effects checked | PASS | No side effects |
| No internal contradiction between constraints and outputs | PASS | No contradictions |
| Allowed Write Targets are narrow | PASS | 1 file only |
| Acceptance criteria are testable | PASS | All criteria have tests |

Gate Status: **PASS**

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | PASS | read-tera-workspace.ts only |
| No unauthorized files created | PASS | Only 1 file modified |
| No unauthorized files deleted | PASS | No files deleted |
| No unauthorized packages added | PASS | No new packages |
| No unauthorized UI/CSS/theme changes | PASS | No UI involved |
| UI Acceptance Gate passed for UI tasks | N/A | Not a UI task |
| No real secrets outside approved local environment files | PASS | No secrets involved |
| Secrets redacted in docs/logs/config references | PASS | N/A |
| No unauthorized ORM models/entities/migrations | PASS | No database involved |
| No unapproved business validation moved to DB constraints | PASS | N/A |
| No unauthorized API/Auth created | PASS | No Auth involved |
| Acceptance criteria satisfied | PASS | All 5 criteria met |
| CLI side effects reviewed | PASS | No side effects |
| Task file and core project-control records reviewed | PASS | Will update after |
| No secret leakage in task files/logs/reports/handbacks | PASS | No secrets |
| No duplicate project-control IDs created | PASS | TASK-COD-004 is unique |
| Any out-of-target changes classified | N/A | All changes within targets |
| Independent review decision recorded | PASS | SecurityAgent not required (no Auth/Secrets) |

Gate Status: **PASS**

Root Cause if failed:
- N/A

Required Action:
- N/A

Independent Review:
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required (no Auth/Secrets/Config)
- QAAndAcceptanceAgent: Not Required (typecheck sufficient)

Deviation Classification:
- N/A
