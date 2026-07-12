# 09_IMPLEMENTATION_PLAN.md — WarehouseDashboard

> **النوع:** Preliminary Implementation Plan — جسر بين Phase 4 (التحضير) و Phase 5 (تخطيط التنفيذ)
> **الحالة:** `Draft — للمراجعة قبل Phase 5`
> **Baseline Module:** WarehouseDashboard
> **تاريخ:** 2026-07-12
> **الجهة المعدّة:** TeraAgent (General Agent — TASK-PREP-016)
> **الملفات المرجعية:** `08_TECHNICAL_ARCHITECTURE.md`، `01_PROJECT_BRIEF.md`، `PREPARATION_PLAN.md`، `TERA_PROJECT_DECISION.md`

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Draft — Preliminary` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | قبل بدء Phase 5 (Execution Planning) |
| **Document Type** | Bridge Plan — ليس خطة تنفيذ نهائية |
| **Version** | 1.0.0-draft |

---

## 1. Implementation Phases (مراحل التنفيذ)

> هذا القسم **لا يكتب كود** — هو تخطيط أولي فقط. الخطة الرسمية تُنشأ في Phase 5 عبر `PROJECT_MASTER_PLAN.md` + `EXECUTION_BATCH_PLAN.md`.

### (A) Foundation — Oracle Connection Test + SQL Server Setup
- **الهدف:** إثبات إمكانية الاتصال بـ Oracle (ODP.NET Managed) قبل أي عمل آخر (تخفيفاً لخطر التواصل المتأخر — R1 / Decision #22).
- **المخرجات:**
  - `TASK-COD-001`: اختبار اتصال Oracle + قراءة جدول تجريبي واحد (أول مهمة تنفيذية).
  - تجهيز SQL Server (إنشاء Database `WarehouseDashboard` + Config Tables عبر EF Core Migrations).
- **الاعتمادات:** قرارات `08_TECHNICAL_ARCHITECTURE.md` مُعتمدة.

### (B) Data Layer — Sync Engine
- **الهدف:** بناء `WarehouseDashboard.Api` (Sync Engine).
- **المخرجات:**
  - `OracleExtractionService` (ODP.NET) + `SqlServerLoadService` (SqlBulkCopy) + `SyncEngineService` (BackgroundService + PeriodicTimer) + `SyncLogService` (EF Core).
  - وضع Full Refresh (DELETE + BulkInsert في Transaction واحدة).
  - REST endpoints: `/api/sync/trigger`, `/api/sync/status`, `/api/sync/logs`, `/api/sync/config`.
- **الاعتمادات:** اكتمال (A).

### (C) Config Layer — Admin Panel
- **الهدف:** بناء `WarehouseDashboard.Web` — Admin Panel.
- **المخرجات:**
  - `AdminAuthMiddleware` + صفحة Login (BCrypt).
  - Card CRUD + Card Editor + Query Tester + Drill Down Config.
  - DbContext (Config + Logs) + Migrations.
- **الاعتمادات:** اكتمال (A) و (B) جزئياً (SQL Server جاهز).

### (D) Dashboard UI
- **الهدف:** بناء واجهة Dashboard الديناميكية.
- **المخرجات:**
  - Dynamic Card Rendering (~20 بطاقة) عبر `DashboardService`.
  - Drill Down متعدد المستويات + Breadcrumb.
  - Sync Status Bar + زر مزامنة يدوية.
  - Blue Theme (11 لوناً) + Responsive Grid + Syncfusion Components.
- **الاعتمادات:** اكتمال (B) و (C).

### (E) Polish & Vitality
- **الهدف:** الصقل وجودة العرض.
- **المخرجات:**
  - State Management (Loading / Empty / Error / Success / Sync Running).
  - تطابق كامل مع الهوية البصرية الزرقاء.
  - اختبار سيناريوهات الأخطاء (Oracle down، SQL Server down).
- **الاعتمادات:** اكتمال (D).

### (F) Deployment
- **الهدف:** النشر المحلي على IIS.
- **المخرجات:**
  - `dotnet publish` للـ Api + Web.
  - Application Pools (WDApiPool / WDWebPool) + ربط `/api` و `/`.
  - تعيين Connection Strings (Environment Variables / مشفّر).
  - تثبيت .NET 8 Hosting Bundle + اختبار `http://localhost/`.
- **الاعتمادات:** اكتمال (E).

---

## 2. Monitor Condition Compliance (امتثال شروط المراقبة)

تأكيد صريح لشروط المراقبة الثلاثة المقبولة في `TERA_PROJECT_DECISION.md §2` و `PREPARATION_PLAN.md §1`:

| # | الشرط | كيفية الامتثال في هذه الخطة |
|:-:|-------|------------------------------|
| **1** | `PROJECT_MASTER_PLAN.md` يُنشأ **قبل** أي `TASK-COD-*` | في Phase 5، الخطوة الأولى هي إنشاء واعتماد `PROJECT_MASTER_PLAN.md`. لا يُنشأ أي `TASK-COD` قبل ذلك. هذا الملف (09) هو مجرد جسر تحضيري ولا يحتوي على أكواد مهام تنفيذية. |
| **2** | `PROJECT_STATE.md` يُملأ **قبل** أول `TASK-COD` | يُملأ `PROJECT_STATE.md` بالكامل (status, progress, open risks, decisions) قبل إنشاء `TASK-COD-001`. تكون حالته `current` عند بدء التنفيذ. |
| **3** | أول `TASK-COD` = اختبار اتصال Oracle | `TASK-COD-001` = **Oracle Connection Test** (المرحلة A). هذا يخفف خطر R1 ويتوافق مع قرار Majed "Oracle Testing Early" (#22). |

> **Tera Commitment:** تُنفَّذ الالتزامات أعلاه حرفياً في Phase 5 دون استثناء.

---

## 3. Key Milestones & Dependencies (المعالم الرئيسية والاعتماديات)

| المعلم (Milestone) | المرحلة | الاعتماديات | ملاحظة |
|--------------------|:-------:|------------|--------|
| M1: Oracle Connectivity Proven | A | لا شيء (يدخل أولاً) | حاسم — يفتح باقي العمل |
| M2: SQL Server Schema Ready | A | M1 | Config + Logs Migrations |
| M3: Sync Engine Functional | B | M2 | Full Refresh + API endpoints |
| M4: Admin Panel Operational | C | M2 | CRUD + Auth |
| M5: Dashboard Rendering Live | D | M3, M4 | بطاقات ديناميكية |
| M6: Polish & Vitality Passed | E | M5 | Blue theme + error states |
| M7: IIS Deployment Done | F | M6 | تسليم محلي |

**قواعد الاعتمادية (Dependencies):**
- لا تبدأ (B) أو (C) أو (D) قبل اكتمال (A).
- لا تبدأ (D) قبل (C) (التكوين من Admin مطلوب لعرض البطاقات).
- (E) و (F) متسلسلتان بعد (D).

---

## 4. Risk Mitigations (تخفيف المخاطر)

| الخطر | Severity | التخفيف المخطط له |
|-------|:--------:|-------------------|
| **Oracle connectivity unknown** (R1 / R2) | High / Medium | `TASK-COD-001` = اختبار الاتصال أولاً (المرحلة A). إذا فشل، يُبلغ العميل فوراً قبل أي تقدم. العميل متاح أثناء التنفيذ لتفاصيل الجداول (Q1/Q5). |
| **Type mapping** (Oracle Types → .NET) | Medium | `OracleExtractionService` يقوم Manual mapping موثّق في `08_TECHNICAL_ARCHITECTURE.md §4.2`. اختبار أنواع البيانات في المرحلة A/B مبكراً. SqlBulkCopy يعتمد على `DataTable` schema متطابق. |
| **Full Refresh بطيء مع بيانات كبيرة** (R1) | High | المحرك **مصمم لـ Incremental Sync** من البداية (§5.4) — غير مفعّل. يُفعّل فوراً إذا تجاوزت البيانات 100K صف (Q5). |
| **EF Core غير مناسب لـ Bulk** (R3) | Medium | ADO.NET SqlBulkCopy للبيانات + EF Core للتكوين فقط (قرار معتمد #21). |
| **Drill Down معقد تقنياً** (R5) | Medium | تصميم هيكلي واضح في `08` + اختبار مبكر في المرحلة D. |

---

## 5. Deferred to Phase 2 (مؤجل للمرحلة الثانية)

الميزات التالية **خارج نطاق Phase 1** وتُنقل إلى Phase 2 (حسب `01_PROJECT_BRIEF.md §5` و `TERA_PROJECT_DECISION.md §6`):

- **Data Editing Screens** — شاشات تعديل البيانات (وليست التكوين فقط).
- **RBAC / Users** — إدارة المستخدمين والأدوار (Admin/Viewer). Phase 1 يستخدم كلمة مرور + Hidden URL فقط.
- **Export Excel / PDF** — تصدير التقارير والبطاقات.
- **Advanced Analytics** — تحليلات متقدمة خارج نطاق Core MVP.

> هذه المؤجلات موثّقة أيضاً في `35_ROADMAP_AND_FUTURE_PHASES.md`.

---

## 6. Handoff to Phase 5 (تسليم إلى المرحلة الخامسة)

عند اكتمال Phase 4 (هذا الملف + كل ملفات التحضير الـ 19)، يتم التسليم إلى Phase 5 عبر:

1. **`PROJECT_MASTER_PLAN.md`** — يُنشأ أولاً في Phase 5 (قبل أي `TASK-COD`)، ويحوّل هذا الجسر إلى خطة تنفيذ رسمية مقسّمة إلى batches.
2. **`EXECUTION_BATCH_PLAN.md`** — يحدد ترتيب الـ batches وتوزيع `TASK-COD-*` مع الالتزام بـ:
   - `TASK-COD-001` = Oracle Connection Test.
   - `PROJECT_STATE.md` مملوء قبل `TASK-COD-001`.
   - المراحل A→F تُترجم إلى batches قابلة للتنفيذ.

> **ملاحظة:** هذا الملف (09) **تخطيطي مبدئي فقط** — لا يحل محل خطط Phase 5 الرسمية، ولا يحتوي على أكواد أو تفاصيل تنفيذية نهائية.

---

> **Prepared by:** TeraAgent — TASK-PREP-016
> **Date:** 2026-07-12
> **Status:** `preparation` — awaiting Phase 5
