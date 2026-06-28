# ISSUES_AND_GAPS.md

# المشاكل والفجوات والملاحظات

يسجل هذا الملف أي مشكلة، فجوة، ملاحظة، أو اقتراح يظهر أثناء العمل.

## حالات السجل

```text
Open
Planned
In Progress
Resolved
Deferred
Won't Fix
Closed
```

## السجل

| Issue ID | Title | Type | Status | Linked Task | Owner | Resolution / Next Action |
|---|---|---|---|---|---|---|
| ISSUE-0000 | Project control layer needed before implementation | Governance Gap | Closed | TASK-0000 | Tera Agent | Added `project-control/` and traceability protocol. |
| ISSUE-0001 | Preparation files had stale pending-state text | Documentation Sync | Closed | N/A | Tera Agent | Updated `08_TECHNICAL_ARCHITECTURE.md`, `TERA_PROJECT_DECISION.md`, and `GENERATED_AGENTS_MANIFEST.md`. |
| ISSUE-0002 | Sub-agent handback could remain only in chat | Traceability Gap | Closed | TASK-0001 | Tera Agent | Added mandatory handback recording rule: every sub-agent result must be recorded inside `project-control/tasks/[TASK-ID].md` before acceptance or closure. |
| ISSUE-0003 | Secret exposure in local config fallback and follow-up documentation | Security Incident | Resolved | TASK-0003 / TASK-0004 | Project Owner | A real secret was exposed during local config handling and then described unsafely in follow-up records. Cleaned by Project Owner: removed the fallback entirely, redacted documentation to `[REDACTED]`, and tightened Tera system rules so post-execution review also inspects `project-control` files, task records, reports, and incident descriptions for secret leakage. |
| ISSUE-0004 | SEC-001: User Enumeration via inactive account message (رسالة خطأ منفصلة تكشف وجود المستخدم) | Security Finding (Low) | Resolved | TASK-0007 | EngineeringAgent | تم الإصلاح في TASK-0007: توحيد رسالة الخطأ لجميع حالات الفشل. السبب الحقيقي يُسجل في console.log فقط. راجع TASK-0007.md. |
| ISSUE-0005 | SEC-002: Logout action cookie missing httpOnly/secure/sameSite properties | Security Finding (Info) | Resolved | TASK-0007 | EngineeringAgent | تم الإصلاح في TASK-0007: إضافة httpOnly/secure/sameSite إلى `cookieStore.set()` في `app/logout/actions.ts`. |
| ISSUE-0006 | Server-side validation missing in Banks/Parties Server Actions | Security / Validation Gap | Resolved | TASK-0011 | EngineeringAgent | Resolved in TASK-0011: added required/trim/max-length validation and safe optional field normalization in Banks and Parties Server Actions. Build PASS. |
| ISSUE-0007 | NaN amount bypass in check validation | Security Finding (Medium) | Resolved | TASK-0013 | EngineeringAgent | Resolved in TASK-0013: added `typeof amount !== 'number' || isNaN(amount)` guard before `<= 0` check in `validateCheckData()`. Build PASS. |
| ISSUE-0008 | Invalid date crash in check actions | Stability (Low) | Resolved | TASK-0014 | EngineeringAgent | Resolved in TASK-0014: replaced `dueDateStr()` helper with explicit `new Date()` parsing + `isNaN(getTime())` validation. Invalid dates return Arabic error 'تاريخ الإصدار غير صحيح' / 'تاريخ الاستحقاق غير صحيح'. Build PASS. |
