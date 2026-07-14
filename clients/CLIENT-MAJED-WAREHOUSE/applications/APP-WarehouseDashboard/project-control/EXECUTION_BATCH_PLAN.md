# EXECUTION_BATCH_PLAN.md — WarehouseDashboard

> **Purpose:** Batch plan for Phase 6 implementation. Organizes 21 TASK-COD-* into executable batches. This is the final Phase 5 document before Build Mode.
> **Status:** ✅ Approved (Phase 5 — Execution Planning)
> **Prepared by:** TeraAgent — 2026-07-12
> **Predecessor:** `PROJECT_MASTER_PLAN.md` + `PROJECT_DETAILED_EXECUTION_PLAN.md`

---

## 1. Batch Overview

| Batch | TASK-COD-* | المرحلة | المهام | الحالة |
|---|---|---|---|---|
| **B1** | 001, 002, 003 | A — Foundation | Oracle test + DB + Scaffolding | ⏳ Next |
| **B2** | 004, 005 | B — Data Core | Oracle extraction + SqlBulkCopy | ⏳ |
| **B3** | 006, 007, 008 | B/C — API + Admin Login | Sync endpoints + BackgroundService + Admin Auth | ⏳ |
| **B4** | 009, 010 | C — Admin Screens | Card CRUD + Query Tester + Drill Config | ⏳ |
| **B5** | 011, 012, 013, 014 | D — Dashboard UI | Main page + Drill Down + Status + Filter | ⏳ |
| **B6** | 015, 016, 017, 018 | E — Polish | Skeleton + Empty/Error + Toast + Animations + Connection | ⏳ |
| **B7** | 019, 020, 021 | F — Deployment | IIS + License + UAT | ⏳ |

---

## 2. Batch Details

### B1 — Foundation (التمهيد)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 001 | Oracle Connection Test | engineering-agent | 2–4h | — |
| 002 | SQL Server Database + EF Migrations | engineering-agent | 3–6h | — (مستقل) |
| 003 | Project Scaffolding (.NET 8 + Syncfusion) | engineering-agent | 3–6h | — (مستقل) |

**إستراتيجية التنفيذ:** 3 مهام **بالتوازي** (001 + 002 + 003 معاً) — لا توجد تبعية بينها.
**مخاطرة:** إذا فشل 001 (Oracle)، باقي المهام لا تتأثر — لكن Phase B لا يمكن أن تبدأ.
**بعد القبول:** الـ 3 مهام يمكن أن تُفوَّض لـ engineering-agent في نفس الدفعة.

### B2 — Data Core (لب البيانات)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 004 | Oracle Data Extraction Layer | engineering-agent | 40–60h | 001 ✅ |
| 005 | SqlBulkCopy Data Loading | engineering-agent | 40–80h | 004 ✅ |

**إستراتيجية التنفيذ:** 005 يعتمد على 004 — **تسلسلي**. يُفوَّض 004 أولاً، وبعد قبوله يُفوَّض 005.
**مخاطرة:** R1/R2 — Oracle table structures غير معروفة.

### B3 — API + Admin Auth (الخدمات)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 006 | Sync API Endpoints | engineering-agent | 20–30h | 005 ✅ |
| 007 | Sync Scheduling (BackgroundService) | engineering-agent | 10–15h | 006 ✅ |
| 008 | Admin Login + BCrypt + Session | engineering-agent | 10–15h | 003 ✅ |

**إستراتيجية التنفيذ:** 006 + 008 **بالتوازي** (لا تبعية بينهما). 007 بعد 006.
**ملاحظة:** D-BE-3 — API لا يحتاج Token auth في Phase 1 (IIS IP restriction).

### B4 — Admin Screens (شاشات الأدمن)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 009 | DashboardCards CRUD (List + Editor) | engineering-agent | 30–40h | 008 ✅ |
| 010 | Query Tester + Drill Down Config | engineering-agent | 20–30h | 008 ✅ |

**إستراتيجية التنفيذ:** 009 + 010 **بالتوازي** (كلاهما يعتمد على Admin Auth فقط).

### B5 — Dashboard UI (الواجهة الرئيسية)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 011 | Dashboard Main Page (Grid of Cards) | ui-designer + engineering-agent | 40–60h | 006 ✅ + 009 ✅ |
| 012 | Drill Down Pages | ui-designer + engineering-agent | 20–30h | 011 ✅ |
| 013 | Sync Status Bar + Manual Refresh | ui-designer + engineering-agent | 10–15h | 006 ✅ |
| 014 | Filtering + Search | ui-designer + engineering-agent | 10–15h | 011 ✅ |

**إستراتيجية التنفيذ:** 011 + 013 **بالتوازي** (013 لا يعتمد على 011). 012 يعتمد على 011. 014 يعتمد على 011.

### B6 — Polish & Vitality (الصقل)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 015 | Skeleton Loading / Shimmer | ui-designer | 10–15h | 011 ✅ |
| 016 | Empty States + Error States | ui-designer | 8–10h | 011 ✅ |
| 017 | Toast Notifications + Micro-animations | ui-designer | 10–15h | 011 ✅ |
| 018 | Connection Status Indicator | ui-designer + engineering-agent | 5–8h | 006 ✅ |

**إستراتيجية التنفيذ:** جميع المهام الأربع **بالتوازي** (لا تبعية بينها — كلها تعتمد على Dashboard موجود).

### B7 — Deployment (النشر)

| TASK-COD | الوصف | الوكيل | التقدير | التبعية |
|---|---|---|---|---|
| 019 | IIS Setup + Environment Config | engineering-agent | 6–8h | 003 ✅ |
| 020 | Syncfusion License + Scheduled Sync | engineering-agent | 4–6h | 006 ✅ + 007 ✅ |
| 021 | UAT + Deployment Testing | engineering-agent + Tera | 6–10h | ALL ✅ |

**إستراتيجية التنفيذ:** 019 + 020 **بالتوازي**. 021 بعد كل شيء.

---

## 3. Dependency Map

```
B1 ──┬── 001 (Oracle)
     ├── 002 (SQL DB)
     └── 003 (Scaffolding)
           │
B2 ──┬── 004 (Extract) ← 001
     └── 005 (BulkCopy) ← 004
           │
B3 ──┬── 006 (API) ← 005
     │    └── 007 (Schedule) ← 006
     └── 008 (Admin Auth) ← 003
           │
B4 ──┬── 009 (Card CRUD) ← 008
     └── 010 (Query Tester) ← 008
           │
B5 ──┬── 011 (Dashboard) ← 006 + 009
     │    ├── 012 (Drill Down) ← 011
     │    └── 014 (Filter) ← 011
     └── 013 (Status Bar) ← 006
           │
B6 ──┬── 015 (Skeleton) ← 011
     ├── 016 (Empty) ← 011
     ├── 017 (Toast) ← 011
     └── 018 (Connection) ← 006
           │
B7 ──┬── 019 (IIS) ← 003
     ├── 020 (License) ← 006 + 007
     └── 021 (UAT) ← ALL
```

---

## 4. Build Mode Entry Protocol

### قبل Build Mode:

| شرط | الحالة | المرجع |
|---|---|---|
| PROJECT_MASTER_PLAN.md موجود | ✅ | Monitor #1 |
| PROJECT_STATE.md مكتمل | ✅ | Monitor #2 |
| أول TASK-COD = Oracle test | ⏳ | Monitor #3 (B1) |
| Client Approval Package معتمد | ⏳ **مطلوب** | R4 |
| التصميم المعتمد موجود | ✅ | 28_UI_UX_GUIDELINES.md |

### Build Mode Flow:

```
1. إنشاء TASK-COD ملفات (001–003)
2. Pre-Execution Gate لكل مهمة
3. تفويض لـ engineering-agent (3 مهام بالتوازي)
4. انتظار الـ Handback
5. Post-Execution Review لكل مهمة
6. قبول / رفض / إصلاح
7. التالي: B2
```

### Client Approval Package (R4 — مطلوب قبل Build Mode)

| ملف الاعتماد | المصدر | الحالة |
|---|---|---|
| SCOPE_AGREEMENT.md | `02_SCOPE_AND_BOUNDARIES.md` | ⏳ يُنشأ |
| PRICING_AGREEMENT.md | T&M @ 4 JOD/hour | ⏳ يُنشأ |
| DESIGN_DIRECTION.md | `28_UI_UX_GUIDELINES.md` (Blue theme) | ⏳ يُنشأ |
| EXECUTION_AUTHORIZATION.md | Formal go-ahead from client | ⏳ يُنشأ |

---

## 5. Resource Planning

### Agent Availability

| الوكيل | B1 | B2 | B3 | B4 | B5 | B6 | B7 |
|---|---|---|---|---|---|---|---|
| **engineering-agent** | ✅ | ✅ | ✅ | ✅ | مساعد | ❌ | ✅ |
| **ui-designer** | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| **tera-software-designer** | دعم | دعم | دعم | دعم | دعم | دعم | دعم |
| **TeraAgent** | إشراف | إشراف | إشراف | إشراف | إشراف | إشراف | إشراف |

### Engineering-Agent Activation

لا حاجة لتوليد ملف وكيل جديد — `engineering-agent` متوفر كنوع وكيل مدمج في المنصة. يُفوَّض مباشرة عبر `subagent_type: "engineering-agent"`.

---

## 6. Delivery Acceptance Criteria

| Check | Description |
|---|---|
| ✅ Oracle connectivity proved | TASK-COD-001 ✅ |
| ✅ Data flowing from Oracle to SQL Server | TASK-COD-005 ✅ |
| ✅ Sync API functional (trigger + status) | TASK-COD-006 ✅ |
| ✅ Auto-sync every 30 min | TASK-COD-007 ✅ |
| ✅ Admin can login + manage cards | TASK-COD-008 + 009 ✅ |
| ✅ Dashboard displays ~20 dynamic cards | TASK-COD-011 ✅ |
| ✅ Drill Down works (2+ levels) | TASK-COD-012 ✅ |
| ✅ UI polished (skeleton, toast, empty, animations) | TASK-COD-015→018 ✅ |
| ✅ Deployed on IIS | TASK-COD-019→021 ✅ |
| ✅ Client approval | Phase 7 ✅ |

---

## 7. Estimated Timeline

| Batch | Hours | Realistic |
|---|---|---|
| B1 Foundation | 8–16 | 1 day |
| B2 Data Core | 80–140 | 5–10 days |
| B3 API + Admin Auth | 40–60 | 3–5 days |
| B4 Admin Screens | 50–70 | 3–5 days |
| B5 Dashboard UI | 80–120 | 5–8 days |
| B6 Polish | 33–48 | 2–3 days |
| B7 Deployment | 16–24 | 1–2 days |
| **Total** | **307–478** | **20–34 days** |

> ضمن النطاق المعتمد (430–625 ساعة / ~8–12 أسبوع لوتيرة طبيعية).

---

## 8. Document Control

| Version | Date | Author | Changes |
|---|---|---|---|
| 1.0 | 2026-07-12 | TeraAgent | Initial batch plan — 7 batches, 21 TASK-COD-* |
