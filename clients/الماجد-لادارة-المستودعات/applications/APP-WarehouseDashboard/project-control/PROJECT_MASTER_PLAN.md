# PROJECT_MASTER_PLAN.md — WarehouseDashboard

> **Purpose:** Formal Master Plan for Phase 5 execution. The primary reference for all TASK-COD-* creation and delegation.
> **Status:** ✅ Approved (Phase 5 Entry Gate)
> **Prepared by:** TeraAgent — 2026-07-12
> **Last Updated:** 2026-07-12

---

## 1. Project Overview

| Item | Value |
|---|---|
| **Project** | WarehouseDashboard — نظام لوحات معلومات إدارة المستودعات |
| **Client** | الماجد لادارة المستودعات |
| **Tech Stack** | .NET 8, ASP.NET Core Razor Pages, Syncfusion, ODP.NET (Oracle), ADO.NET SqlBulkCopy, EF Core (config/logs), SQL Server, IIS |
| **Profile** | `dotnet-razorpages-adonet` (custom, approved) |
| **Pricing** | Time & Material @ 4 JOD/hour, open budget |
| **Size** | Medium (45.95% Scorecard) |
| **Estimated Hours** | 430–625 hours Phase 1 |

### Scope Summary

| Scope | Count | Reference |
|---|---|---|
| **In Scope (Phase 1)** | 12 features, 33 sub-components | `02_SCOPE_AND_BOUNDARIES.md`, `03_MODULES_AND_FEATURES.md` |
| **Deferred (Phase 2)** | 5 features: Data Editing, RBAC, Export, Advanced Analytics, Shared Library | `35_ROADMAP_AND_FUTURE_PHASES.md` |
| **Out of Scope** | 5 items | `02_SCOPE_AND_BOUNDARIES.md` |

### Key Architecture Decisions

| Decision | Reference |
|---|---|
| ADO.NET SqlBulkCopy for data (not EF Core) | `08_TECHNICAL_ARCHITECTURE.md` §3 |
| EF Core limited to config + logs | `08_TECHNICAL_ARCHITECTURE.md` §3.3 |
| Incremental Sync ready but not activated in Phase 1 | `08_TECHNICAL_ARCHITECTURE.md` §5 |
| Full Refresh default sync mode (Phase 1) | `08_TECHNICAL_ARCHITECTURE.md` §5.2 |
| Admin Panel: BCrypt password + hidden URL | `08_TECHNICAL_ARCHITECTURE.md` §8.1 |
| AdminPassword singleton (Phase 1); AdminUsers deferred | **D-BE-1** |
| AuditLog deferred to Phase 2; Phase 1 = SyncLogs + ErrorLogs only | **D-BE-2** |
| Phase 1 API = no app-level auth; internal network only | **D-BE-3** |
| Blue theme (11 colors) — 28_UI_UX_GUIDELINES.md | `28_UI_UX_GUIDELINES.md` |

---

## 2. Execution Strategy

### Principle

يمنع بدء أي `TASK-COD-*` قبل إنشاء `PROJECT_MASTER_PLAN.md` (هذا الملف — شرط المراقب #1 ✅).

### Execution Order

```
Phase 5 (Execution Planning) ──┬── 1. PROJECT_MASTER_PLAN.md ── هذا الملف ✅
                               ├── 2. PROJECT_DETAILED_EXECUTION_PLAN.md
                               ├── 3. EXECUTION_BATCH_PLAN.md
                               └── 4. First TASK-COD-* batch
                                   └── يجب أن يكون TASK-COD-001 = Oracle connection test

Phase 6 (Implementation) ──┬── التنفيذ حسب الدفعات المعتمدة
                            ├── Post-Execution Review لكل TASK-COD
                            └── Pre-Execution Gate قبل كل تفويض

Phase 7 (Delivery & Closure) ──┬── Delivery Readiness
                                ├── Client Handover
                                └── Project Closure
```

### Monitor Conditions

| # | Condition | Status | Verified |
|---|---|---|---|
| 1 | `PROJECT_MASTER_PLAN.md` before any TASK-COD-* | ✅ Done (this file) | Tera |
| 2 | `PROJECT_STATE.md` populated before first TASK-COD | ✅ Done | Tera |
| 3 | First TASK-COD = Oracle connection test | ⏳ Pending execution | Tera |

---

## 3. Implementation Phases

Based on `09_IMPLEMENTATION_PLAN.md` §2. ستة مراحل تنفيذ متسلسلة مع إمكانية التداخل المحدود.

### Phase A — Foundation (التمهيد)

| الهدف | تسليمات | التقدير |
|---|---|---|
| إثبات الاتصال بـ Oracle + إعداد SQL Server | Oracle connection test (TASK-COD-001)، إنشاء قاعدة SQL Server، تشغيل EF Core migrations للجداول الإعدادية | 8–16 ساعة |

**المخاطرة:** Oracle connectivity غير معروفة — تم التخفيف بالاختبار في أول 3 أيام.

### Phase B — Data Layer (طبقة البيانات — Sync Engine)

| الهدف | تسليمات | التقدير |
|---|---|---|
| بناء محرك المزامنة: Oracle → SQL Server | API endpoints (`POST /api/sync/trigger`, `GET /api/sync/status`)، SqlBulkCopy pipeline، Full Refresh، SyncLogs/ErrorLogs | 100–160 ساعة |

**التبعية:** Phase A (بدون Oracle اتصال، لا مزامنة)

### Phase C — Config Layer (طبقة الإعدادات — Admin Panel)

| الهدف | تسليمات | التقدير |
|---|---|---|
| لوحة تحكم الأدمن | Admin Login (BCrypt)، Card List، Card Editor، Query Tester، Drill Down Config، DashboardCards CRUD | 80–120 ساعة |

**التبعية:** Phase B (data pipeline موجود)
**ملاحظة:** المسار المخفي `/admin-secure-panel/` — غير موجود في التنقل العام.

### Phase D — Dashboard UI (واجهة المستخدم الرئيسية)

| الهدف | تسليمات | التقدير |
|---|---|---|
| لوحة المعلومات + Drill Down | ~20 كارت ديناميكي، 6 أنواع رسوم (Bar/Line/Pie/KPI/Table/Gauge)، Drill Down متعدد المستويات، Sync Status bar، Syncfusion integration | 100–160 ساعة |

**التبعية:** Phase B (data pipeline موجود)، Phase C (Admin configures cards)
**ملاحظة:** تطبق الهوية الزرقاء (11 لوناً) حسب `28_UI_UX_GUIDELINES.md`

### Phase E — Polish & Vitality (الصقل والحيوية)

| الهدف | تسليمات | التقدير |
|---|---|---|
| تحسين تجربة المستخدم والحيوية | Skeleton Loading، Toast Notifications، Empty States، Connection Status Indicator، Micro-animations، Search (جداول)، Error States | 40–60 ساعة |

**التبعية:** Phase D (واجهات موجودة للتحسين)

### Phase F — Deployment (النشر)

| الهدف | تسليمات | التقدير |
|---|---|---|
| نشر النظام على IIS | IIS site + app pool config، Environment Variables، Syncfusion license، Scheduled Sync (periodic timer)، اختبار قبول | 16–24 ساعة |

**التبعية:** Phases A–E (النظام كامل)

### تقدير إجمالي: 344–540 ساعة (ضمن النطاق المعتمد 430–625)

---

## 4. Milestones & Timeline

| Milestone | Delivery | Depends On |
|---|---|---|
| M0 — Phase 5 Plan Approved | PROJECT_MASTER_PLAN.md | — |
| M1 — Oracle Connected | TASK-COD-001 ✅ | M0 |
| M2 — Sync Engine Operational | API + Full Refresh | M1 |
| M3 — Admin Panel Ready | All admin screens + BCrypt | M2 |
| M4 — Dashboard Live | ~20 dynamic cards visible | M2, M3 |
| M5 — UI Polished | Skeleton, toast, animations | M4 |
| M6 — Deployed on IIS | Live on client server | M1–M5 |
| M7 — Delivery Accepted | Client sign-off, handover | M6 |

---

## 5. Dependencies & Critical Path

### Critical Path
```
M0 → M1 (Oracle) → M2 (Sync Engine) → M3 (Admin) + M4 (Dashboard) → M5 (Polish) → M6 (Deploy) → M7 (Delivery)
```

### Key Dependencies

| Task | Depends On | Risk if Blocked |
|---|---|---|
| Oracle connection test | Oracle credentials + server access | ⛔ Blocks ALL downstream work |
| SqlBulkCopy pipeline | Oracle tables identified | 🔴 Blocks data sync |
| DashboardCards CRUD | DB schema ready | 🟡 Delays admin panel |
| Dashboard UI | Sync Engine working | 🟡 Cannot test with fake data |
| Drill Down | DashboardCards configured | 🟡 Delays full functionality |
| Deployment | All code complete | 🔴 No delivery |

### Parallelization Opportunities
- Phase C (Admin Panel) can overlap with Phase D (Dashboard UI) — both depend on Phase B
- Phase E (Polish) starts after Phase D, but individual files can be polished independently

---

## 6. Risk Register

| ID | Risk | Likelihood | Impact | Mitigation | Owner |
|---|---|---|---|---|---|
| R1 | **Oracle connection fails** — wrong credentials, TNS issues, firewall | Medium | Critical | First TASK-COD = test (R1 in PROJECT_STATE.md) | Tera |
| R2 | **Oracle table structures unknown** — no table docs until execution | High | High | Adaptable approach documented in `14_INTEGRATIONS` | EngineeringAgent |
| R3 | **Full Refresh slow** — large Oracle tables | Medium | Medium | Incremental Sync ready (architecturally, not activated) | EngineeringAgent |
| R4 | **Client Approval Package missing** — Build Mode blocked | High | High | Create before Phase 6 entry | Tera |
| R5 | **Scope creep** — client requests Phase 2 features early | Medium | Medium | Change Request process (`25_CHANGE_REQUESTS.md`) | Tera + Majed |
| R6 | **Oracle RAC / HA** — app pool recycle kills background sync | Low | Medium | IIS app pool config (AlwaysRunning, Idle Timeout=0) | EngineeringAgent |

---

## 7. Quality Gates

| Gate | When | Requires |
|---|---|---|
| **Pre-Execution Gate** | Before every TASK-COD delegation | `tera-system/TeraPreExecutionGate.md` PASS |
| **Post-Execution Review** | After every TASK-COD submission | Acceptance criteria met, Vitality checklist complete |
| **Design Source Review** | Before UI implementation | Design Source Decision confirmed (`28_UI_UX_GUIDELINES.md`) |
| **UI Acceptance Gate** | Before UI task closure | `tera-system/design-system/UI_ACCEPTANCE_GATE.md` PASS |
| **Delivery Readiness** | Phase 7 entry | All TASK-COD-* closed, client acceptance |

### UI Vitality Checklist (إلزامي لكل TASK-COD-* للواجهات)

```
[ ] Skeleton Loading / Shimmer
[ ] Toast Notifications (نجاح، فشل، تحذير)
[ ] Connection Status Indicator
[ ] Search في الجداول
[ ] Micro-animations
[ ] Empty States
[ ] Realistic Data (أسماء، أرقام)
```

---

## 8. Handoff to Detailed Planning

### Next Steps (Immediate)

| Step | File | Owner | Status |
|---|---|---|---|
| 1 | `PROJECT_MASTER_PLAN.md` (هذا الملف) | Tera | ✅ Complete |
| 2 | `PROJECT_DETAILED_EXECUTION_PLAN.md` | Tera | ⏳ Next |
| 3 | `EXECUTION_BATCH_PLAN.md` | Tera | ⏳ After 2 |
| 4 | First TASK-COD-001 batch | Tera | ⏳ After 3 |

### Resource Requirements

| Agent | Phase 6 Role | Ready |
|---|---|---|
| **tera-software-designer** | Technical design for each TASK-COD | ✅ |
| **engineering-agent** | Backend code (Sync Engine, API, DB) | ⏳ |
| **ui-designer** | UI/UX implementation (Razor Pages, Syncfusion) | ✅ (activated) |
| **TeraAgent** | Orchestration, review, decisions | ✅ |

### Client Approval Package (R4)

مطلوب قبل Build Mode (Phase 6):
- `client-approval/SCOPE_AGREEMENT.md` (from `02_SCOPE_AND_BOUNDARIES.md`)
- `client-approval/PRICING_AGREEMENT.md` (T&M @ 4 JOD/hour)
- `client-approval/DESIGN_DIRECTION.md` (blue theme approval from `28_UI_UX_GUIDELINES.md`)
- `client-approval/EXECUTION_AUTHORIZATION.md` (formal go-ahead)

---

## 9. Document Control

| Version | Date | Author | Changes |
|---|---|---|---|
| 1.0 | 2026-07-12 | TeraAgent | Initial master plan — Phase 5 entry |
