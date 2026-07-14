# ISSUES_AND_GAPS.md

> **Purpose:** Track open issues, gaps, and risks during the project.

## Required Gap Format

```md
## GAP-XXXX - [Short Title]

- Source Task:
- Discovered By:
- Severity: Critical / High / Medium / Low
- Status: Open / Deferred / In Progress / Resolved / Cancelled
- Description:
- Impact:
- Recommended Action:
- Target Task / Phase:
- Owner:
```

**Severity:** Critical / High / Medium / Low
**Status:** Open / Deferred / In Progress / Resolved / Cancelled

**Phase 7 Rule:** No hidden open issues. Any unresolved issue must be documented as Resolved / Deferred / Won't Fix / Requires TASK-COD-FIX before project closure.

## Open Issues and Gaps

## GAP-0001 - Package typecheck blocked by plugin type mismatch

- Source Task: TASK-COD-001
- Discovered By: EngineeringAgent / Tera Review
- Severity: Medium
- Status: Resolved
- Description: `bun run typecheck` from `clients/TeraAi/packages/opencode` fails in `src/plugin/index.ts` due a plugin type mismatch between local `packages/plugin/src/index` and installed `@opencode-ai/plugin` types.
- Impact: TASK-COD-001 scoped tests pass, but package-wide typecheck cannot be used as a clean acceptance signal until this unrelated issue is fixed.
- Recommended Action: Resolved by `TASK-COD-FIX-001`; keep fix narrow and do not expand TASK-COD-001.
- Target Task / Phase: TASK-COD-FIX-001 / Phase 4 follow-up before broader Gateway work
- Owner: Tera / EngineeringAgent after separate approval

### Resolution

- Resolved By: TASK-COD-FIX-001
- Resolution Date: 2026-07-10
- Verification:
  - `bun run typecheck` from `clients/TeraAi/packages/opencode` — PASS
  - `bun test test/gateway/context-api.test.ts` — PASS, 7/7

## GAP-0002 - EngineeringAgent أنشأ كود التطبيق في المسار الخطأ

- Source Task: TASK-COD-001
- Discovered By: Majed
- Severity: **Critical**
- Status: Resolved (الملفات منقولة)
- Description: EngineeringAgent أنشأ مشروع TeraQuotation في `D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\src\TeraQuotation\` (جذر المنظومة) بدلاً من مسار العميل الصحيح داخل `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/source/`. السبب: TeraAgent لم يحدد Allowed Write Targets بالكامل عند التفويض.
- Impact: تلويث جذر المنظومة بملفات تطبيق لا تنتمي إليها. لو استمر، كان سيؤدي إلى التزام كود العميل مع كود TeraSystem في Git history دون فصل.
- Recommended Action: 
  1. ✅ نقل الملفات إلى `clients/.../source/TeraQuotation/` (تم)
  2. 🔄 تصحيح سلوك EngineeringAgent بإضافة قاعدة صارمة عن Allowed Write Targets (مطلوب عبر Hares)
  3. 🔄 تحديث قالب تفويض TeraAgent ليشمل المسار الكامل للـ Client
- Target Task / Phase: AFTER TASK-COD-001 — تصحيح سلوك الوكيل
- Owner: TeraAgent (نقل) + Hares (تصحيح سلوك EngineeringAgent)
