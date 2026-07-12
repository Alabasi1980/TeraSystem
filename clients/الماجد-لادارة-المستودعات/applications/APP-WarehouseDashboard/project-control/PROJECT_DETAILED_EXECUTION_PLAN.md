# PROJECT_DETAILED_EXECUTION_PLAN.md — WarehouseDashboard

> **Purpose:** Detailed execution plan breaking down the 6 implementation phases (A–F) into granular TASK-COD-* units with agent assignments, reference files, and acceptance criteria.
> **Status:** ✅ Approved (Phase 5 — Execution Planning)
> **Prepared by:** TeraAgent — 2026-07-12
> **Predecessor:** `PROJECT_MASTER_PLAN.md` (Monitor Condition 1 ✅)

---

## 1. Execution Structure

```
Phase 5 (هذا الملف) → EXECUTION_BATCH_PLAN.md → TASK-COD-* → Build Mode
```

### Key Principles
- كل `TASK-COD-*` = أصغر وحدة تنفيذ آمنة (لا تجمع UI + API + DB في مهمة واحدة)
- لا يُفعل Build Mode قبل اعتماد Client Approval Package (R4)
- كل TASK-COD-* يمر عبر Pre-Execution Gate → Post-Execution Review
- TASK-COD-001 إلزاماً = Oracle connection test (Monitor Condition 3)

---

## 2. Phase A — Foundation (التمهيد)

**الهدف:** إثبات الاتصال بـ Oracle + إنشاء قاعدة SQL Server + هيكل المشروع
**المدة المقدرة:** 8–16 ساعة
**التبعية:** لا توجد (أول مرحلة)

### A.1 — TASK-COD-001: Oracle Connection Test

| البند | القيمة |
|---|---|
| **الهدف** | إثبات أن Oracle متصل وقابل للقراءة عبر ODP.NET |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §1 |
| **قرارات التصميم** | D-BE-3 (API لا Auth)، D-BE-1 (AdminPassword) |
| **مخرجات المتوقع** | Console app أو اختبار وحدة يقرأ row واحد من Oracle |
| **قبول** | ✅ اتصال ناجح + قراءة أول row |
| **فشل** | 🔴 إبلاغ العميل فوراً — استبدال TNS/credentials |

### A.2 — TASK-COD-002: Create SQL Server Database

| البند | القيمة |
|---|---|
| **الهدف** | إنشاء قاعدة SQL Server + تشغيل EF Core Migrations للجداول الإعدادية |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `19_DATABASE_DESIGN.md` §1 (DashboardCards, CardDrillDownLevels, SyncSettings, AdminPassword) |
| **قرارات التصميم** | D-BE-1 (AdminPassword، لا AdminUsers)، D-BE-2 (لا AuditLog) |
| **مخرجات المتوقع** | SQL Server DB + EF Core migrations + جداول Config |
| **قبول** | ✅ الجداول الستة (Config + Logs) موجودة في SQL Server |

### A.3 — TASK-COD-003: Project Scaffolding + Technology Setup

| البند | القيمة |
|---|---|
| **الهدف** | إنشاء مشاريع .NET 8 (Web + Api) + إعداد Syncfusion + إعداد ODP.NET |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `dotnet-razorpages-adonet.md`، `08_TECHNICAL_ARCHITECTURE.md` §2 |
| **مخرجات المتوقع** | Solution مع 2 مشاريع (Web + Api)، إعداد Syncfusion UnlockKey، NuGet packages |
| **قبول** | ✅ `dotnet build` يمر بدون أخطاء |

---

## 3. Phase B — Data Layer: Sync Engine (طبقة البيانات)

**الهدف:** بناء محرك المزامنة Oracle → SQL Server مع API endpoints
**المدة المقدرة:** 100–160 ساعة
**التبعية:** Phase A (TASK-COD-001, 002, 003)

### B.1 — TASK-COD-004: Oracle Data Extraction Layer

| البند | القيمة |
|---|---|
| **الهدف** | بناء طبقة استخراج البيانات من Oracle (قراءة فقط) |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §2، `06_DATA_MODEL_PREPARATION.md` §3 |
| **مخرجات المتوقع** | OracleDataReader أو DataTable لكل جدول، نوع mapping |
| **قبول** | ✅ استخراج ناجح + تحويل أنواع صحيح |

### B.2 — TASK-COD-005: SqlBulkCopy Data Loading

| البند | القيمة |
|---|---|
| **الهدف** | بناء طبقة تحميل البيانات إلى SQL Server عبر SqlBulkCopy (ليس EF Core) |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `06_DATA_MODEL_PREPARATION.md` §5، `08_TECHNICAL_ARCHITECTURE.md` §3.2 |
| **مخرجات المتوقع** | SqlBulkCopy pipeline مع per-table handling |
| **قبول** | ✅ بيانات منتقلة + سجلات في SyncLogs |

### B.3 — TASK-COD-006: Sync API Endpoints

| البند | القيمة |
|---|---|
| **الهدف** | بناء API endpoints للمزامنة (`POST /api/sync/trigger` + `GET /api/sync/status`) |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `20_API_CONTRACTS.md` §1–§5 |
| **قرارات التصميم** | D-BE-3 (API لا Token auth في Phase 1) |
| **مخرجات المتوقع** | SyncController مع trigger/status/logs endpoints |
| **قبول** | ✅ الـ 4 endpoints تعمل كالمتوقع |

### B.4 — TASK-COD-007: Sync Scheduling + Background Service

| البند | القيمة |
|---|---|
| **الهدف** | إضافة BackgroundService مع PeriodicTimer (30 دقيقة) للمزامنة التلقائية |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `08_TECHNICAL_ARCHITECTURE.md` §5.5، `22_DEPLOYMENT.md` §5 |
| **مخرجات المتوقع** | SyncBackgroundService + SemaphoreSlim لتجنب التداخل |
| **قبول** | ✅ مزامنة تلقائية كل 30 دقيقة + منع تداخل |

---

## 4. Phase C — Config Layer: Admin Panel (لوحة الأدمن)

**الهدف:** بناء لوحة تحكم الأدمن (مسار مخفي `/admin-secure-panel/`)
**المدة المقدرة:** 80–120 ساعة
**التبعية:** Phase B (البيانات متوفرة للإدارة)

### C.1 — TASK-COD-008: Admin Login + Authentication

| البند | القيمة |
|---|---|
| **الهدف** | صفحة دخول الأدمن + BCrypt verification + Session |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `08_TECHNICAL_ARCHITECTURE.md` §8.1، `19_DATABASE_DESIGN.md` §1.4 |
| **قرارات التصميم** | D-BE-1 (AdminPassword singleton، لا AdminUsers) |
| **مخرجات المتوقع** | صفحة Login + AdminAuthMiddleware + Session |
| **قبول** | ✅ دخول ناجح + حماية المسار المخفي |

### C.2 — TASK-COD-009: DashboardCards CRUD (List + Editor)

| البند | القيمة |
|---|---|
| **الهدف** | صفحة قائمة البطاقات + صفحة تحرير/إضافة بطاقة |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `07_SCREENS_AND_UI_STRUCTURE.md` (Card List + Editor)، `19_DATABASE_DESIGN.md` §1.1 |
| **مخرجات المتوقع** | Card List (جدول) + Card Editor (نموذج: عنوان، ChartType، SqlQuery، GridPosition…) |
| **قبول** | ✅ CRUD كامل + Validation |

### C.3 — TASK-COD-010: Query Tester + Drill Down Config

| البند | القيمة |
|---|---|
| **الهدف** | صفحة اختبار استعلامات SQL + صفحة تكوين مستويات Drill Down |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `07_SCREENS_AND_UI_STRUCTURE.md` (Query Tester + DrillDown Config)، `19_DATABASE_DESIGN.md` §1.2 |
| **مخرجات المتوقع** | Query Tester (SQL input → result preview) + DrillDown Config |
| **قبول** | ✅ استعلام آمن (قراءة فقط) + تكوين Drill Down |

---

## 5. Phase D — Dashboard UI (الواجهة الرئيسية)

**الهدف:** بناء لوحة المعلومات الديناميكية مع ~20 كارت + Drill Down
**المدة المقدرة:** 100–160 ساعة
**التبعية:** Phase B (بيانات) + Phase C (تكوين البطاقات)

### D.1 — TASK-COD-011: Dashboard Main Page (Grid of Cards)

| البند | القيمة |
|---|---|
| **الهدف** | صفحة Dashboard الرئيسية تعرض شبكة من البطاقات الديناميكية |
| **الوكيل** | ui-designer + engineering-agent (UI + data binding) |
| **ملف التصميم** | `07_SCREENS_AND_UI_STRUCTURE.md` (Dashboard Main)، `28_UI_UX_GUIDELINES.md` |
| **مخرجات المتوقع** | Razor Page مع شبكة CSS Grid 12 عمود، بطاقات من DashboardCards، Syncfusion charts |
| **قبول** | ✅ بطاقات تظهر + ألوان زرقاء (11 لوناً) + تخطيط متجاوب |

### D.2 — TASK-COD-012: Drill Down Pages

| البند | القيمة |
|---|---|
| **الهدف** | صفحات Drill Down متعددة المستويات مع Breadcrumb |
| **الوكيل** | ui-designer + engineering-agent |
| **ملف التصميم** | `07_SCREENS_AND_UI_STRUCTURE.md` (Drill Down)، `13_REPORTS.md` §3 |
| **مخرجات المتوقع** | صفحة Drill Down مع Breadcrumb navigation + query parameters |
| **قبول** | ✅ Drill Down يعمل (card → level 1 → level 2 مع Breadcrumb) |

### D.3 — TASK-COD-013: Sync Status Bar + Manual Refresh Button

| البند | القيمة |
|---|---|
| **الهدف** | شريط حالة المزامنة في Dashboard + زر تشغيل يدوي |
| **الوكيل** | ui-designer + engineering-agent |
| **ملف التصميم** | `07_SCREENS_AND_UI_STRUCTURE.md` (Sync Status)، `20_API_CONTRACTS.md` §1–2 |
| **مخرجات المتوقع** | شريط (آخر مزامنة + الحالة) + زر تشغيل (Admin فقط) |
| **قبول** | ✅ حالة حية + زر يعمل |

### D.4 — TASK-COD-014: Filtering + Search

| البند | القيمة |
|---|---|
| **الهدف** | إضافة فلاتر عامة للـ Dashboard + بحث في جداول Drill Down |
| **الوكيل** | ui-designer + engineering-agent |
| **ملف التصميم** | `13_REPORTS.md` §4، `28_UI_UX_GUIDELINES.md` §3.4 |
| **مخرجات المتوقع** | فلاتر (تاريخ/فئة) + بحث نصي في الجداول |
| **قبول** | ✅ تصفية + بحث يعملان |

---

## 6. Phase E — Polish & Vitality (الصقل والحيوية)

**الهدف:** تحسين تجربة المستخدم
**المدة المقدرة:** 40–60 ساعة
**التبعية:** Phase D (الواجهات موجودة للتحسين)

### E.1 — TASK-COD-015: Loading States + Skeleton

| البند | القيمة |
|---|---|
| **الهدف** | إضافة Skeleton Loading / Shimmer لجميع البطاقات والجداول |
| **الوكيل** | ui-designer |
| **ملف التصميم** | `28_UI_UX_GUIDELINES.md` §7 (Vitality)، `07_SCREENS.md` (all screens) |
| **مخرجات المتوقع** | Skeleton/Shimmer أثناء تحميل البيانات |
| **قبول** | ✅ كل بطاقة/جدول يظهر Skeleton أثناء التحميل |

### E.2 — TASK-COD-016: Empty States + Error States

| البند | القيمة |
|---|---|
| **الهدف** | إضافة حالات فارغة (لا توجد بيانات) + حالات خطأ لكل قسم |
| **الوكيل** | ui-designer |
| **ملف التصميم** | `28_UI_UX_GUIDELINES.md` §7 |
| **مخرجات المتوقع** | رسائل "لا توجد بيانات" + "حدث خطأ" + أيقونات |
| **قبول** | ✅ كل قسم يعالج Empty + Error |

### E.3 — TASK-COD-017: Toast Notifications + Micro-animations

| البند | القيمة |
|---|---|
| **الهدف** | إضافة إشعارات (نجاح/فشل/تحذير) + تأثيرات Micro-animations |
| **الوكيل** | ui-designer |
| **ملف التصميم** | `28_UI_UX_GUIDELINES.md` §3.5 (Syncfusion Toast)، §3.6 (animations) |
| **مخرجات المتوقع** | Toast Notifications + Hover effects + Stagger entries |
| **قبول** | ✅ إشعارات حية + حركات سلسة |

### E.4 — TASK-COD-018: Connection Status Indicator

| البند | القيمة |
|---|---|
| **الهدف** | مؤشر اتصال حي (متصل/غير متصل) |
| **الوكيل** | ui-designer + engineering-agent |
| **ملف التصميم** | `28_UI_UX_GUIDELINES.md` §3.7 |
| **مخرجات المتوقع** | مؤشر في Dashboard يُظهر حالة الاتصال |
| **قبول** | ✅ مؤشر يعمل + تغيير الحالة تلقائياً |

---

## 7. Phase F — Deployment (النشر)

**الهدف:** نشر النظام على IIS وإعداد البيئة
**المدة المقدرة:** 16–24 ساعة
**التبعية:** Phases A–E

### F.1 — TASK-COD-019: IIS Setup + Environment Config

| البند | القيمة |
|---|---|
| **الهدف** | إعداد IIS site + app pool + Environment Variables |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `22_DEPLOYMENT.md` §1–2 |
| **مخرجات المتوقع** | IIS site مع app pool (No Managed Code) + appsettings + env vars |
| **قبول** | ✅ التطبيق يشتغل تحت IIS |

### F.2 — TASK-COD-020: Syncfusion License + Scheduled Sync

| البند | القيمة |
|---|---|
| **الهدف** | تسجيل Syncfusion UnlockKey + تفعيل المزامنة التلقائية (PeriodicTimer) |
| **الوكيل** | engineering-agent |
| **ملف التصميم** | `22_DEPLOYMENT.md` §3–5 |
| **مخرجات المتوقع** | Syncfusion مسجل + BackgroundService يعمل |
| **قبول** | ✅ مزامنة تلقائية بدون تدخل يدوي |

### F.3 — TASK-COD-021: UAT + Deployment Testing

| البند | القيمة |
|---|---|
| **الهدف** | اختبار قبول المستخدم + التحقق من كل الميزات |
| **الوكيل** | engineering-agent (Tera reviews) |
| **ملف التصميم** | جميع ملفات التحضير كمرجع |
| **مخرجات المتوقع** | تقرير اختبار + قائمة تحقق قبول |
| **قبول** | ✅ كل الميزات تعمل على البيئة الحية |

---

## 8. Agent Assignment Summary

| TASK-COD | المرحلة | الوكيل الأساسي | الوكيل المساعد |
|---|---|---|---|
| 001–007 | A + B | engineering-agent | — |
| 008–010 | C | engineering-agent | — |
| 011–014 | D | engineering-agent + ui-designer | — |
| 015–017 | E | ui-designer | — |
| 018 | E | ui-designer + engineering-agent | — |
| 019–021 | F | engineering-agent | Tera |

### Agent Readiness

| Agent | Status | Notes |
|---|---|---|
| **engineering-agent** | ⏳ Needs activation | أول استخدام في TASK-COD-001 |
| **ui-designer** | ✅ Activated | استُخدم في Batch D (07_SCREENS, 28_UI_UX) |
| **tera-software-designer** | ✅ Available | للتصميم التقني لكل TASK-COD عند الحاجة |

---

## 9. Dependencies & Sequencing

```
TASK-COD-001 (Oracle test)          ──┬──> 002 (SQL Server DB)
                                         │       │
                                         └──> 003 (Scaffolding)
                                                   │
                                                   └──> 004 (Extract) ──> 005 (BulkCopy)
                                                                              │
                                                                              └──> 006 (API) ──> 007 (Schedule)
                                                                                                      │
                                               008 (Admin Login) ──> 009 (Card CRUD) ──> 010 (Query Tester)
                                                                                                      │
                                                                                                      ├──> 011 (Dashboard) ──> 012 (Drill Down)
                                                                                                      │       │                    │
                                                                                                      │       └──> 013 (Status) ──┤
                                                                                                      │                             │
                                                                                                      └──> 014 (Filter/Search) ───┘
                                                                                                                                 │
                                                                                                    015–018 (Polish) <─────────────┘
                                                                                                                                 │
                                                                                                    019–021 (Deployment) <─────────┘
```

### Parallelization
- TASK-COD-002 و 003 يمكن أن يتوازيا
- TASK-COD-008 (Admin Login) يمكن أن يبدأ بعد 003 (لا ينتظر Sync Engine)
- 011 (Dashboard) و 014 (Filter/Search) يمكنهما التوازي
- 015–018 (Polish) يمكن أن تتوازى كلها

---

## 10. Quality Gates Per Task

كل TASK-COD-* يجب أن يشمل في ملف المهمة:

### Pre-Execution Gate Checklist
```
[ ] Reference design files loaded
[ ] Acceptance criteria defined
[ ] Allowed write targets documented
[ ] Agent identified and delegated
[ ] Technology profile applied
[ ] Task scope not overlapping
[ ] UI Vitality Checklist included (للواجهات)
```

### Post-Execution Review Checklist
```
[ ] Allowed Write Targets respected
[ ] No secrets in outputs
[ ] In scope
[ ] Acceptance criteria met
[ ] Handback recorded in task file
[ ] Activity log updated
```

### UI Vitality Checklist (إلزامي لكل واجهة)
```
[ ] Skeleton Loading / Shimmer
[ ] Toast Notifications
[ ] Connection Status Indicator
[ ] Search (في الجداول)
[ ] Micro-animations
[ ] Empty States
[ ] Realistic Data
```

---

## 11. Phase Transition Criteria

| إلى | شرط الانتقال |
|---|---|
| Phase B (Data Layer) | TASK-COD-001 ✅ + 002 ✅ + 003 ✅ |
| Phase C (Admin Panel) | TASK-COD-003 ✅ (يمكن بدء C-A بعد 003، لا ينتظر B) |
| Phase D (Dashboard) | TASK-COD-006 ✅ (Sync API) + 004 ✅ (بيانات) |
| Phase E (Polish) | TASK-COD-011 ✅ (Dashboard يعمل) |
| Phase F (Deployment) | جميع TASK-COD-* من A–E ✅ |
| Phase 7 (Closure) | ALL TASK-COD-* ✅ + Delivery Readiness |

---

## 12. Handoff to EXECUTION_BATCH_PLAN.md

التالي: إنشاء `EXECUTION_BATCH_PLAN.md` — يقسم الـ 21 TASK-COD إلى دفعات قابلة للتنفيذ (3–5 مهام لكل دفعة)، ويُعرض للمستخدم للاعتماد قبل Build Mode.

**الدفعة الأولى المقترحة (Batch 1):**
| TASK-COD | الوصف | الوكيل |
|---|---|---|
| 001 | Oracle Connection Test | engineering-agent |
| 002 | SQL Server Database | engineering-agent |
| 003 | Project Scaffolding | engineering-agent |

---

## 13. Document Control

| Version | Date | Author | Changes |
|---|---|---|---|
| 1.0 | 2026-07-12 | TeraAgent | Initial detailed execution plan (21 TASK-COD-* across 6 phases) |
