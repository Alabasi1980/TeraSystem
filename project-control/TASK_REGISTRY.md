# TASK_REGISTRY.md

# سجل المهام

هذا الملف هو الفهرس الرسمي لكل مهام المشروع.

كل مهمة تنفيذ أو تحليل موجهة لعميل فرعي يجب أن يكون لها `TASK-ID`.

## حالات المهمة

```text
Draft
Approved
Assigned
In Progress
Submitted
Accepted
Needs Fix
Blocked
Deferred
Cancelled
Closed
```

## سجل المهام

| Task ID | Title | Assigned To | Phase | Status | Linked Files | Last Update |
|---|---|---|---|---|---|---|
| TASK-0000 | Initialize Project Control Layer | Tera Agent | System Setup | Closed | `project-control/` | 2026-06-27 |
| TASK-0001 | Scaffold Next.js + TypeScript + Prisma + .env.example | EngineeringAgent | Phase 1 — Technical Foundation | Closed | `project-control/tasks/TASK-0001.md` | 2026-06-27 |
| TASK-0002 | إنشاء Prisma Data Models (users, banks, parties, checks) | EngineeringAgent | Phase 2 — Data Model | Closed | `project-control/tasks/TASK-0002.md` | 2026-06-27 |
| TASK-0003 | إنشاء قاعدة البيانات checks_management + أول Migration | EngineeringAgent | Phase 3 — Database Setup | Closed | `project-control/tasks/TASK-0003.md` | 2026-06-27 |
| TASK-0004 | إعداد المصادقة والصلاحيات (Authentication + Roles) | EngineeringAgent | Phase 3 — Authentication | Closed | `project-control/tasks/TASK-0004.md` | 2026-06-27 |
| TASK-0005 | المراجعة الأمنية المستقلة لـ TASK-0004 (Authentication + Roles) | SecurityAgent | Phase 3 — Security Review | Closed | `project-control/tasks/TASK-0005.md` | 2026-06-27 |
| TASK-0006 | تحضير مواصفات التنفيذ — Workflow Rules + Screen Specs | BusinessWorkflowAgent + UIUXStructureAgent | Phase 4 — Preparation | Closed | `project-control/tasks/TASK-0006.md` | 2026-06-27 |
| TASK-0007 | SEC fixes + شاشة إدارة البنوك (S03) | EngineeringAgent | Phase 4 — Banks Screen | Closed | `project-control/tasks/TASK-0007.md` | 2026-06-27 |
| TASK-0008 | شاشة إدارة الجهات (Parties Screen S04) | EngineeringAgent + FrontendAgent | Phase 4 — Parties Screen | Closed | `project-control/tasks/TASK-0008.md` | 2026-06-27 |
| TASK-0009 | الصفحة الرئيسية Navigation Hub | FrontendAgent | Phase 4 — Main Page | Closed | `project-control/tasks/TASK-0009.md` | 2026-06-27 |
| TASK-0010 | Periodic Quality Review after TASK-0009 | QualityReviewCoordinatorAgent | Phase 4 — Quality Review | Closed | `project-control/tasks/TASK-0010.md` | 2026-06-28 |
