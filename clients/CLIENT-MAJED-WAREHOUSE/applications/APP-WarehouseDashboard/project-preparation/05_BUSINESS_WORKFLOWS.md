# 05_BUSINESS_WORKFLOWS.md — WarehouseDashboard

**Status:** `preparation`
**Client:** الماجد لادارة المستودعات
**Application:** WarehouseDashboard
**Prepared by:** TeraAgent — Task TASK-PREP-006
**Date:** 2026-07-12
**Source Documents:** 01_PROJECT_BRIEF.md, 02_SCOPE_AND_BOUNDARIES.md, 04_USERS_ROLES_PERMISSIONS.md, APPLICATION_BLUEPRINT.md, CLIENT_DECISION_LOG.md

---

## 1. Overview

يُوثّق هذا المستند سير العمل (Workflows) الرئيسية لتطبيق WarehouseDashboard. يغطي أربعة تدفقات أساسية مع حالات الحالة (Status Transitions) ومعالجة الأخطاء (Error Handling) لكل منها.

### Workflows الرئيسية

| # | الـ Workflow | الاختصار | المشروع | الأولوية |
|:-:|:------------:|:---------:|:-------:|:--------:|
| 1 | **Sync Workflow** — Full Refresh | SW-01 | API | P1 |
| 2 | **Admin Panel Workflow** | AP-01 | Web (Razor) | P1 |
| 3 | **Dashboard Viewing Workflow** | DV-01 | Web (Razor) | P1 |
| 4 | **Drill Down Workflow** | DD-01 | Web (Razor) | P1 |

---

## 2. Workflow 1: Sync Workflow — Full Refresh

### 2.1 الغرض

مزامنة البيانات من Oracle (read-only) إلى SQL Server (destination) باستخدام أسلوب Full Refresh — حذف جميع الصفوف في كل جدول ثم إدخال مجمّع عبر SqlBulkCopy.

### 2.2 المحفزات (Triggers)

| # | نوع المحفز | الآلية | التوقيت |
|:-:|:-----------:|--------|---------|
| T1 | **Auto-Sync** | `IHostedService` + `PeriodicTimer` | كل 30 دقيقة (افتراضي — قابل للتكوين عبر `SyncSettings.IntervalMinutes`) |
| T2 | **Manual Sync** | `POST /api/sync/trigger` (يدوي من Dashboard) | عند الطلب — بشرط ألا تكون مزامنة قيد التشغيل حالياً |
| T3 | **Application Start** | تنفيذ مزامنة فور بدء التطبيق (اختياري — يُحدد أثناء التطوير) | عند تشغيل API |

### 2.3 التدفق الكامل (Flow)

```
┌─────────────────────────────────────────────────────────────────────┐
│                        SYNC WORKFLOW                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────┐                                                    │
│  │   Trigger    │                                                    │
│  │ (Auto/Manual)│                                                    │
│  └──────┬───────┘                                                    │
│         ▼                                                            │
│  ┌──────────────┐     ┌──────────────────┐     ┌──────────────────┐ │
│  │  Check Lock   │────▶│   No Sync in     │────▶│  Set Status =    │ │
│  │  (IsRunning)  │     │   Progress       │     │  "Running"       │ │
│  └──────┬───────┘     └──────────────────┘     └────────┬─────────┘ │
│         │                                                │          │
│         ▼ (Running)                                      │          │
│  ┌──────────────┐                                        │          │
│  │  Return 409   │                                        │          │
│  │  Conflict     │                                        │          │
│  └──────────────┘                                        ▼          │
│                                                  ┌──────────────────┐│
│                                                  │  For Each Table  ││
│                                                  │  in Config:      ││
│                                                  └────────┬─────────┘│
│                                                           ▼          │
│                                          ┌──────────────────────────┐│
│                                          │  1. Extract from Oracle  ││
│                                          │     (ODP.NET — SELECT *) ││
│                                          │  2. Data Type Mapping    ││
│                                          └────────────┬─────────────┘│
│                                                       ▼              │
│                                          ┌──────────────────────────┐│
│                                          │  3. DELETE All Rows      ││
│                                          │     (SQL Server)         ││
│                                          └────────────┬─────────────┘│
│                                                       ▼              │
│                                          ┌──────────────────────────┐│
│                                          │  4. SqlBulkCopy Insert   ││
│                                          │     (ADO.NET)            ││
│                                          └────────────┬─────────────┘│
│                                                       │              │
│                                                       ▼              │
│                                          ┌──────────────────────────┐│
│                                          │  5. Log Result           ││
│                                          │     (SyncLogs)           ││
│                                          └────────────┬─────────────┘│
│                                                       │              │
│                                                       ▼              │
│                                          ┌──────────────────────────┐│
│                                          │  Next Table? ──── YES ──▶│
│                                          │              │           │
│                                          │              ▼ NO        │
│                                          │         ┌──────────────┐  │
│                                          │         │ Done         │  │
│                                          │         │ Set Status = │  │
│                                          │         │ "Idle"       │  │
│                                          │         └──────────────┘  │
│                                          └──────────────────────────┘│
└─────────────────────────────────────────────────────────────────────┘
```

### 2.4 خطوات التفصيلية

| الخطوة | الإجراء | التقنية | الوصف |
|:------:|---------|:-------:|-------|
| 1 | **التحقق من القفل** | In-Memory Lock (`ConcurrentDictionary` / `SemaphoreSlim`) | التأكد من عدم وجود مزامنة قيد التشغيل. إذا كانت Running → رفض المحفز الجديد (HTTP 409 للـ Manual، تجاهل للـ Auto) |
| 2 | **تحديث الحالة** | Sync Status (In-Memory) | تعيين `SyncStatus.IsRunning = true`، تسجيل `StartTime` |
| 3 | **قراءة إعدادات الجداول** | EF Core → جدول `SyncSettings` | قراءة قائمة الجداول المراد مزامنتها وترتيبها |
| 4 | **لكل جدول:** استخراج من Oracle | ODP.NET (`OracleCommand` + `DataReader`) | تنفيذ `SELECT * FROM {TableName}` مع Data Type Mapping |
| 5 | **لكل جدول:** حذف كامل | ADO.NET (`SqlCommand` + `DELETE`) | تنفيذ `DELETE FROM {DestinationTable}` ضمن `SqlTransaction` |
| 6 | **لكل جدول:** إدخال مجمّع | ADO.NET (`SqlBulkCopy`) | استخدام `SqlBulkCopy` مع `DataTable` أو `IDataReader` لنقل الدفعة |
| 7 | **تسجيل النتيجة** | EF Core → جدول `SyncLogs` | تسجيل: TableName, StartTime, EndTime, Status, RecordCount, Duration, ErrorMessage |
| 8 | **تحديث الحالة** | Sync Status (In-Memory) | تعيين `SyncStatus.IsRunning = false`، `SyncStatus.LastSyncTime = DateTime.UtcNow` |

### 2.5 حالات الحالة (Status Transitions)

```
                  ┌──────────────────┐
                  │      Idle        │
                  │ (ready for sync) │
                  └────────┬─────────┘
                           │ Trigger received
                           ▼
                  ┌──────────────────┐
                  │    Running       │
                  │ (sync in progress)│
                  └────────┬─────────┘
                          / \
                         /   \
                        ▼     ▼
               ┌──────────┐ ┌──────────┐
               │ Completed │ │  Failed  │
               │ (success) │ │ (error)  │
               └─────┬─────┘ └─────┬────┘
                     │              │
                     └──────┬───────┘
                            ▼
                   ┌──────────────────┐
                   │      Idle        │
                   └──────────────────┘
```

| الحالة الحالية | الحدث | الحالة التالية | الإجراء |
|:--------------:|:-----:|:--------------:|---------|
| Idle | Trigger Auto/Manual | Running | بدء المزامنة |
| Running | Trigger آخر | Idle (no action) | رفض (409 للـ Manual) |
| Running | Sync completes successfully | Idle | تحديث LastSyncTime + تسجيل Success |
| Running | Sync fails (table-level) | Idle | تسجيل Failure + ErrorMessage + التراجع عن الجدول الفاشل فقط (استمرار بقية الجداول أو إيقاف — يُحدد أثناء التطوير) |
| Running | Sync fails (critical) | Idle | تسجيل Failure + إيقاف المزامنة بالكامل |

### 2.6 معالجة الأخطاء (Error Handling)

| # | السيناريو | آلية المعالجة | السلوك |
|:-:|-----------|---------------|--------|
| E1 | **فشل الاتصال بـ Oracle** | `try/catch` حول OracleCommand | تسجيل الخطأ في SyncLogs، إيقاف المزامنة، إعادة Idle |
| E2 | **فشل DELETE من SQL Server** | `try/catch` داخل SqlTransaction | التراجع عن الجدول الفاشل فقط (Rollback ضمن transaction) — بقية الجداول تُكمل |
| E3 | **فشل SqlBulkCopy** | `try/catch` داخل SqlTransaction | التراجع عن الجدول الفاشل، تسجيل العدد الذي تم إدخاله قبل الفشل |
| E4 | **Timeout** | `SqlCommand.CommandTimeout` و `OracleCommand.CommandTimeout` | التقاط `SqlException` / `OracleException` مع رمز Timeout — إيقاف المزامنة |
| E5 | **خطأ في Data Type Mapping** | عمود غير متوافق | تسجيل تحذير + تجاهل العمود أو إيقاف حسب القرار التقني |
| E6 | **محاولة مزامنة أثناء Running** | التحقق من lock قبل البدء | Manual → HTTP 409 Conflict. Auto → تجاهل وتسجيل في log |

---

## 3. Workflow 2: Admin Panel Workflow

### 3.1 الغرض

إدارة تكوين البطاقات في Dashboard بشكل ديناميكي — إنشاء، تعديل، حذف، وتكوين مصادر البيانات، أنواع الرسوم البيانية، ومستويات Drill Down.

### 3.2 التدفق الكامل

```
┌─────────────────────────────────────────────────────────────────────┐
│                     ADMIN PANEL WORKFLOW                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────┐                                               │
│  │  Open /Admin URL  │                                               │
│  │  (Hidden — not   │                                               │
│  │   in nav menu)   │                                               │
│  └────────┬─────────┘                                               │
│           ▼                                                         │
│  ┌──────────────────┐     ┌──────────────────┐                     │
│  │  Has Session?     │────▶│  No → Show Login  │                     │
│  │  (Valid Cookie)   │     │  Page            │                     │
│  └────────┬─────────┘     └────────┬─────────┘                     │
│           │                        │                                │
│           │ Yes                    │ Enter Password                 │
│           ▼                        ▼                                │
│  ┌──────────────────┐     ┌──────────────────┐                     │
│  │  Card List Page   │◀────│  Verify BCrypt    │                     │
│  │  (DashboardCards) │     │  Hash            │                     │
│  └────────┬─────────┘     └──────────────────┘                     │
│           │                                                        │
│  ┌────────┼────────┐                                               │
│  │        │        │                                               │
│  ▼        ▼        ▼                                               │
│  New    Edit    Delete                                              │
│  Card   Card    Card                                                │
│  │       │       │                                                 │
│  │       │       └──▶ Soft Delete / Hard Delete                    │
│  │       │           (Confirm → Remove from DB)                    │
│  ▼       ▼                                                         │
│  ┌─────────────────────────────────────────────┐                   │
│  │          Card Editor Page                     │                   │
│  ├─────────────────────────────────────────────┤                   │
│  │  • Title, Description                        │                   │
│  │  • Data Source (SQL Query / View Name)       │                   │
│  │  • Chart Type (Bar, Line, Pie, KPI, Table,   │                   │
│  │    Gauge)                                    │                   │
│  │  • Drill Down Levels (0–N levels)            │                   │
│  │  • Size & Position in Grid                   │                   │
│  │  • Refresh Interval (optional)               │                   │
│  └──────────────────────┬──────────────────────┘                   │
│                         │                                           │
│                         ▼                                           │
│  ┌─────────────────────────────────────────────┐                   │
│  │           Query Tester (Preview)             │                   │
│  ├─────────────────────────────────────────────┤                   │
│  │  • Execute SQL Query against SQL Server      │                   │
│  │  • Show results in a preview table           │                   │
│  │  • Show row count + execution time           │                   │
│  │  • If error → show error message             │                   │
│  └──────────────────────┬──────────────────────┘                   │
│                         │                                           │
│                         ▼                                           │
│  ┌─────────────────────────────────────────────┐                   │
│  │  Save / Publish                              │                   │
│  ├─────────────────────────────────────────────┤                   │
│  │  • Validate required fields                  │                   │
│  │  • Save to DashboardCards (EF Core)          │                   │
│  │  • Save DrillDownLevels (if any)            │                   │
│  │  • Show success message                      │                   │
│  │  • Redirect to Card List                     │                   │
│  └─────────────────────────────────────────────┘                   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 3.3 خطوات التفصيلية

| الخطوة | الإجراء | التقنية | الوصف |
|:------:|---------|:-------:|-------|
| 1 | **فتح الرابط** | Browser → `/Admin` | الرابط مخفي — غير موجود في قائمة التنقل الرئيسية |
| 2 | **التحقق من الجلسة** | ASP.NET Core Session Middleware | إذا كانت Session صالحة → Card List. إذا لا → Login Page |
| 3 | **تسجيل الدخول** | `Admin/Login` — POST | إدخال كلمة المرور → التحقق من `BCrypt.Verify(password, hash)` |
| 4 | **إنشاء الجلسة** | `HttpContext.Session.SetString` | تخزين `IsAuthenticated = true` في Session + HttpOnly Cookie |
| 5 | **عرض قائمة البطاقات** | `Admin/Index` | قراءة `DashboardCards` من DB وعرضها في جدول مع أزرار Edit/Delete |
| 6 | **إنشاء/تعديل بطاقة** | `Admin/Edit/{id?}` | نموذج إدخال يشمل: title, SQL query, chart type, dimensions, etc. |
| 7 | **تكوين Drill Down** | `Admin/Edit` — قسم Drill Down | إضافة مستويات Drill Down مع ربط كل مستوى ببطاقة أخرى أو باستعلام مخصص |
| 8 | **اختبار الاستعلام** | `Admin/TestQuery` — AJAX | إرسال SQL query إلى API داخلي → تنفيذه على SQL Server → عرض النتائج في جدول مسبق |
| 9 | **الحفظ** | `Admin/Edit` — POST | التحقق من صحة البيانات → حفظ/تحديث في `DashboardCards` + `CardDrillDownLevels` |
| 10 | **حذف بطاقة** | `Admin/Delete/{id}` | تأكيد الحذف → إزالة السجل من DB (مع حذف مستويات Drill Down المرتبطة) |

### 3.4 حالات الحالة (Status Transitions)

```
  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
  │ Unauthed  │───▶│  Login   │───▶│  Authed   │───▶│  Idle    │
  │ (no sess) │    │  Page    │    │ (session) │    │ (Card    │
  └──────────┘    └──────────┘    └──────────┘    │  List)   │
                                                   └────┬─────┘
                                                        │
                                          ┌─────────────┼─────────────┐
                                          │             │             │
                                          ▼             ▼             ▼
                                   ┌──────────┐ ┌──────────┐ ┌──────────┐
                                   │ Creating  │ │  Editing  │ │ Deleting  │
                                   │  Card    │ │  Card    │ │  Card    │
                                   └────┬─────┘ └────┬─────┘ └────┬─────┘
                                        │            │            │
                                        ▼            ▼            ▼
                                   ┌──────────┐ ┌──────────┐ ┌──────────┐
                                   │ Testing  │ │ Testing  │ │ Confirm  │
                                   │ Query    │ │ Query    │ │ Delete   │
                                   └────┬─────┘ └────┬─────┘ └────┬─────┘
                                        │            │            │
                                        ▼            ▼            │
                                   ┌──────────┐ ┌──────────┐      │
                                   │  Saving   │ │  Saving   │      │
                                   │ (success) │ │ (success) │      │
                                   └────┬─────┘ └────┬─────┘      │
                                        │            │            │
                                        └──────┬──────┘            │
                                               │                   │
                                               ▼                   ▼
                                        ┌──────────────────────────┐
                                        │          Idle            │
                                        │      (Card List)         │
                                        └──────────────────────────┘
```

| الحالة الحالية | الحدث | الحالة التالية | الإجراء |
|:--------------:|:-----:|:--------------:|---------|
| Unauthed | Open `/Admin` | Login Page | عرض صفحة الدخول |
| Login Page | إدخال كلمة سر صحيحة | Authed (Session) | إنشاء Session + توجيه إلى Card List |
| Login Page | إدخال كلمة سر خاطئة | Login Page (with error) | عرض رسالة خطأ + إعادة المحاولة |
| Authed | Session timeout | Unauthed | مسح Session + توجيه إلى Login |
| Authed | Logout | Unauthed | مسح Session + توجيه إلى Login |
| Authed | Click "New Card" | Creating Card | عرض نموذج فارغ |
| Authed | Click "Edit" | Editing Card | عرض نموذج مع البيانات الحالية |
| Creating/Editing | Click "Test Query" | Testing Query | تنفيذ AJAX Query Tester |
| Testing Query | Success | Creating/Editing | عرض النتائج المسبقة |
| Testing Query | Error | Creating/Editing | عرض رسالة الخطأ |
| Creating/Editing | Click "Save" | Saving | حفظ البيانات في DB |
| Saving | Success | Idle (Card List) | عرض رسالة نجاح + تحديث القائمة |
| Saving | Validation Error | Creating/Editing | عرض رسالة خطأ + بقاء النموذج مفتوحاً |
| Authed | Click "Delete" | Confirming Delete | عرض تأكيد الحذف |
| Confirming Delete | Confirm | Deleting | حذف السجل من DB |
| Deleting | Success | Idle (Card List) | عرض رسالة نجاح |

### 3.5 معالجة الأخطاء

| # | السيناريو | آلية المعالجة | السلوك |
|:-:|-----------|---------------|--------|
| E1 | **كلمة مرور خاطئة** | `BCrypt.Verify` يعيد `false` | عرض رسالة "كلمة المرور غير صحيحة" + إعادة المحاولة |
| E2 | **جلسة منتهية** | Session Middleware | إعادة توجيه تلقائي إلى Login |
| E3 | **SQL Query غير صالح** | Query Tester → `try/catch` | عرض رسالة الخطأ من SQL Server في واجهة الاختبار |
| E4 | **فشل الحفظ في DB** | EF Core → `try/catch` | عرض رسالة "فشل الحفظ، يرجى المحاولة مرة أخرى" — لا فقدان للبيانات المدخلة |
| E5 | **Duplicate title** | التحقق من الـ Unique Constraint | عرض رسالة "يوجد بطاقة بنفس الاسم" |
| E6 | **بطاقة غير موجودة للحذف** | التحقق من الوجود قبل الحذف | عرض رسالة "البطاقة غير موجودة" |

---

## 4. Workflow 3: Dashboard Viewing Workflow

### 4.1 الغرض

عرض Dashboard ديناميكي يحتوي على ~20 بطاقة تُبنى تلقائياً من التكوين المخزن في SQL Server. كل بطاقة تنفذ استعلام SQL الخاص بها وتعرض النتيجة باستخدام مكونات Syncfusion.

### 4.2 التدفق الكامل

```
┌─────────────────────────────────────────────────────────────────────┐
│                    DASHBOARD VIEWING WORKFLOW                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────┐                                               │
│  │  Page Load       │                                               │
│  │  (GET /)         │                                               │
│  └────────┬─────────┘                                               │
│           ▼                                                         │
│  ┌──────────────────┐                                               │
│  │  Read Active      │                                               │
│  │  Cards from DB    │                                               │
│  │  (DashboardCards  │                                               │
│  │   WHERE IsActive  │                                               │
│  │   = true)         │                                               │
│  └────────┬─────────┘                                               │
│           │                                                         │
│           ▼                                                         │
│  ┌──────────────────┐     ┌──────────────────┐                     │
│  │  For Each Card:   │────▶│  Execute Card's  │                     │
│  │  Parallel /       │     │  SQL Query       │                     │
│  │  Sequential       │     │  against SQL     │                     │
│  └──────────────────┘     │  Server          │                     │
│                           └────────┬─────────┘                     │
│                                    │                                │
│                                    ▼                                │
│  ┌──────────────────────────────────────────────────────────┐      │
│  │                 Render Dashboard Page                     │      │
│  ├──────────────────────────────────────────────────────────┤      │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐ │      │
│  │  │ Card #1  │  │ Card #2  │  │ Card #3  │  │ Card #4  │ │      │
│  │  │ (Bar)    │  │ (Line)   │  │ (KPI)    │  │ (Pie)    │ │      │
│  │  └──────────┘  └──────────┘  └──────────┘  └──────────┘ │      │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐               │      │
│  │  │ Card #5  │  │ Card #6  │  │ Card #7  │               │      │
│  │  │ (Table)  │  │ (Gauge)  │  │ ...      │               │      │
│  │  └──────────┘  └──────────┘  └──────────┘               │      │
│  └──────────────────────┬───────────────────────────────────┘      │
│                         │                                           │
│                         ▼                                           │
│  ┌──────────────────────────────────────────────────────────┐      │
│  │                Sync Status Bar                            │      │
│  ├──────────────────────────────────────────────────────────┤      │
│  │  • Last Sync: 12:30 PM                                   │      │
│  │  • Status: ✅ Synced / ⏳ Syncing / ❌ Failed            │      │
│  │  • [🔄 Sync Now] button (Admin only)                    │      │
│  └──────────────────────────────────────────────────────────┘      │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────┐      │
│  │                User Interactions                          │      │
│  ├──────────────────────────────────────────────────────────┤      │
│  │  • Click chart element → Drill Down (Workflow 4)        │      │
│  │  • Click "Refresh" → Reload current card data            │      │
│  │  • Hover → Tooltip with data values                      │      │
│  │  • Resize browser → Responsive grid reflow               │      │
│  └──────────────────────────────────────────────────────────┘      │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 4.3 خطوات التفصيلية

| الخطوة | الإجراء | التقنية | الوصف |
|:------:|---------|:-------:|-------|
| 1 | **تحميل الصفحة** | Razor Page `GET /` | طلب HTTP عادي للصفحة الرئيسية |
| 2 | **قراءة البطاقات النشطة** | EF Core → `DashboardCards.Where(c => c.IsActive)` | ترتيب حسب `Position` (الصف/العمود في الـ Grid) |
| 3 | **تجميع قائمة الاستعلامات** | تكوين قائمة SQL Queries من البطاقات | جمع جميع الاستعلامات في قائمة واحدة للتنفيذ المتوازي |
| 4 | **تنفيذ الاستعلامات** | ADO.NET `SqlCommand` إلى SQL Server | لكل بطاقة: تنفيذ استعلامها وجلب `DataTable`. يُفضل بالتوازي (`Task.WhenAll`) للبطاقات المستقلة |
| 5 | **ربط النتائج** | Server-side rendering | كل `DataTable` تُربط بمكون Syncfusion المناسب (Chart, Grid, KPI, Gauge) |
| 6 | **تطبيق الموضوع الأزرق** | Syncfusion Theme + CSS Variables | تطبيق الهوية البصرية (11 لوناً أزرق) على جميع المكونات |
| 7 | **عرض Sync Status** | `GET /api/sync/status` (AJAX) | جلب حالة المزامنة الحالية وعرضها في الشريط العلوي |
| 8 | **عرض الصفحة** | Response HTML | إرسال الصفحة المكتملة إلى المتصفح مع جميع البطاقات |

### 4.4 حالات الحالة

| الحالة الحالية | الحدث | الحالة التالية | الإجراء |
|:--------------:|:-----:|:--------------:|---------|
| Loading | Page requested | Reading Cards | بدء تحميل الصفحة |
| Reading Cards | Cards fetched | Executing Queries | بدء تنفيذ استعلامات البطاقات |
| Executing Queries | All queries done | Rendering | بناء HTML مع النتائج |
| Executing Queries | One query fails | Partial Render | عرض البطاقات الناجحة + رسالة خطأ في البطاقة الفاشلة |
| Rendering | Render complete | Displayed | عرض Dashboard كامل للمستخدم |
| Displayed | Refresh clicked | Executing Queries | إعادة تنفيذ استعلام بطاقة محددة |
| Displayed | Drill Down clicked | Drill Down Workflow | الانتقال إلى Workflow 4 |

### 4.5 معالجة الأخطاء

| # | السيناريو | آلية المعالجة | السلوك |
|:-:|-----------|---------------|--------|
| E1 | **فشل استعلام بطاقة** | `try/catch` حول كل استعلام | عرض بطاقة فارغة مع رسالة "تعذر تحميل البيانات" — لا يؤثر على بقية البطاقات |
| E2 | **Timeout** | `CommandTimeout` = 30 ثانية | عرض رسالة "انتهت مهلة الاستعلام" |
| E3 | **خطأ في تحويل البيانات** | Data binding validation | تجاهل القيمة الخاطئة + عرض باقي البطاقة |
| E4 | **لا توجد بطاقات نشطة** | التحقق من `Cards.Count == 0` | عرض رسالة "لا توجد بطاقات بعد. يرجى الدخول إلى لوحة الإدارة لإضافة بطاقات." |
| E5 | **فشل الاتصال بـ SQL Server** | `try/catch` شامل | عرض صفحة خطأ مع رسالة "تعذر الاتصال بقاعدة البيانات" — إظهار شريط الحالة مع تحذير |

---

## 5. Workflow 4: Drill Down Workflow

### 5.1 الغرض

السماح للمستخدم بالتعمق في البيانات داخل بطاقة Dashboard عبر مستويات متعددة قابلة للتكوين. كل مستوى يعرض بيانات أكثر تفصيلاً.

### 5.2 التدفق الكامل

```
┌─────────────────────────────────────────────────────────────────────┐
│                      DRILL DOWN WORKFLOW                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────────────────────────────────────┐                   │
│  │        Dashboard Main Page                    │                   │
│  │                                              │                   │
│  │  ┌──────────────────────┐                    │                   │
│  │  │  Card: Inventory by   │                    │                   │
│  │  │  Warehouse (Bar Chart)│                    │                   │
│  │  │                      │                    │                   │
│  │  │  [Warehouse A] [Ware-│                    │                   │
│  │  │   house B] [Warehouse│                    │                   │
│  │  │   C]                 │                    │                   │
│  │  │      ▲ (click)       │                    │                   │
│  │  └──────┼───────────────┘                    │                   │
│  │         │                                    │                   │
│  └─────────┼────────────────────────────────────┘                   │
│            │                                                        │
│            │ Click on "Warehouse A" bar                             │
│            ▼                                                        │
│  ┌──────────────────────────────────────────────┐                   │
│  │        Drill Down Level 1                    │                   │
│  │  /DrillDown?cardId=5&level=1&param=WarehouseA│                   │
│  │                                              │                   │
│  │  Breadcrumb: Dashboard > Inventory > WH-A    │                   │
│  │  ┌──────────────────────────────────────────┐│                   │
│  │  │  Drill-Down Card: Items by Category      ││                   │
│  │  │  (Pie Chart)                             ││                   │
│  │  │                                          ││                   │
│  │  │  [Category X] [Category Y] [Category Z]  ││                   │
│  │  │      ▲ (click)                           ││                   │
│  │  └──────┼───────────────────────────────────┘│                   │
│  └─────────┼────────────────────────────────────┘                   │
│            │                                                        │
│            ▼                                                        │
│  ┌──────────────────────────────────────────────┐                   │
│  │        Drill Down Level 2                    │                   │
│  │  /DrillDown?cardId=5&level=2&param=CatX      │                   │
│  │                                              │                   │
│  │  Breadcrumb: Dashboard > Inventory > WH-A    │                   │
│  │              > Category X                    │                   │
│  │  ┌──────────────────────────────────────────┐│                   │
│  │  │  Drill-Down Card: Items Detail (Table)   ││                   │
│  │  │                                          ││                   │
│  │  │  | SKU | Name | Qty | Price |           ││                   │
│  │  │  | ... | ...  | ... | ...  |            ││                   │
│  │  │  | ... | ...  | ... | ...  |            ││                   │
│  │  └──────────────────────────────────────────┘│                   │
│  │                                              │                   │
│  │  [← Back to Dashboard] or Breadcrumb click   │                   │
│  └──────────────────────────────────────────────┘                   │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### 5.3 خطوات التفصيلية

| الخطوة | الإجراء | التقنية | الوصف |
|:------:|---------|:-------:|-------|
| 1 | **النقر على عنصر في البطاقة** | Syncfusion Chart Click Event → JavaScript | التقاط معرف العنصر (مثل اسم المستودع) + معرف البطاقة + المستوى الحالي |
| 2 | **بناء URL التنقل** | JavaScript `window.location` | `/DrillDown?cardId={id}&level={level}&param={value}` |
| 3 | **استقبال الـ Request** | Razor Page `GET /DrillDown` | قراءة Query Parameters: `cardId`, `level`, `param` |
| 4 | **قراءة تكوين الـ Drill Down** | EF Core → `CardDrillDownLevels` | الحصول على استعلام الـ Drill Down للمستوى المطلوب |
| 5 | **تنفيذ استعلام الـ Drill Down** | ADO.NET `SqlCommand` مع parameter | تمرير `param` كـ SQL Parameter (محمي ضد SQL Injection) |
| 6 | **عرض النتائج** | Syncfusion Component | عرض النتائج في Chart أو Table (حسب تكوين البطاقة الأصلية أو المستوى الحالي) |
| 7 | **عرض Breadcrumb** | Razor View Component | عرض المسار: `Dashboard > Card Title > Level 1 > Level 2` |
| 8 | **التفاعل** | النقر على Breadcrumb | العودة إلى المستوى الأعلى أو Dashboard الرئيسي |

### 5.4 تكوين Drill Down Levels

| الحقل | الوصف | مثال |
|-------|-------|------|
| `Level` | رقم المستوى (0 = البطاقة الأصلية، 1 = أول مستوى) | 1, 2, 3 |
| `DrillDownQuery` | استعلام SQL للمستوى — يحتوي على `@param` | `SELECT Category, SUM(Qty) FROM Items WHERE Warehouse = @param GROUP BY Category` |
| `DisplayName` | اسم المستوى للعرض في Breadcrumb | "المستودع" |
| `ChartType` | نوع الرسم البياني للمستوى (اختياري — يرث من البطاقة الأصلية إذا لم يُحدد) | Pie, Bar, Table |

### 5.5 حالات الحالة

```
                    ┌──────────────────┐
                    │   Dashboard Main  │
                    │   (Level 0)       │
                    └────────┬─────────┘
                             │ Click element
                             ▼
                    ┌──────────────────┐
                    │  Navigate to      │
                    │  DrillDown Page   │
                    └────────┬─────────┘
                             │ Load config + execute query
                             ▼
                    ┌──────────────────┐
                    │  Drill Down      │
                    │  Level N         │
                    └────────┬─────────┘
                            / \
                           /   \
                    Click ▼     ▼ Breadcrumb
                  ┌─────────┐ ┌─────────┐
                  │ Level   │ │ Back to │
                  │ N+1     │ │ Level   │
                  │         │ │ N-1 or  │
                  │         │ │ Dashbrd │
                  └─────────┘ └─────────┘
```

| الحالة الحالية | الحدث | الحالة التالية | الإجراء |
|:--------------:|:-----:|:--------------:|---------|
| Dashboard Main | Click on chart element | Level 1 | التنقل مع param |
| Level N | Click on chart element | Level N+1 | التنقل مع param الجديد |
| Level N | Click Breadcrumb → "Dashboard" | Dashboard Main | العودة إلى الصفحة الرئيسية |
| Level N | Click Breadcrumb → Level M (M < N) | Level M | العودة إلى المستوى المطلوب |
| Any Level | Page Refresh | Same Level | إعادة تحميل بنفس params |

### 5.6 معالجة الأخطاء

| # | السيناريو | آلية المعالجة | السلوك |
|:-:|-----------|---------------|--------|
| E1 | **cardId غير موجود** | التحقق من وجود البطاقة | عرض رسالة "البطاقة غير موجودة" + رابط العودة إلى Dashboard |
| E2 | **level غير موجود في التكوين** | التحقق من `CardDrillDownLevels` | عرض رسالة "لا يوجد المزيد من التفاصيل" + إخفاء إمكانية النقر لأسفل |
| E3 | **param غير صالح** | `SqlParameter` + validation | تجاهل الـ param الخاطئ + عرض رسالة تحذير |
| E4 | **فشل استعلام الـ Drill Down** | `try/catch` حول التنفيذ | عرض رسالة "تعذر تحميل التفاصيل" + بقاء Breadcrumb |
| E5 | **SQL Injection** | استخدام `SqlParameter` دائماً | **إلزامي — لا يتم بناء الاستعلام بشكل نصي** |
| E6 | **Breadcrumb overflow** | Breadcrumb طويل جداً | اقتطاع المستويات القديمة مع عرض "..." |

---

## 6. ملخص حالات الحالة لكل Workflow

| الـ Workflow | الحالات الرئيسية | حالات الخطأ |
|:------------:|:-----------------:|:------------:|
| **Sync (SW-01)** | Idle → Running → Completed / Failed → Idle | Connection Error, Timeout, Bulk Insert Error, Lock Conflict |
| **Admin Panel (AP-01)** | Unauthed → Login → Authed → Action (Create/Edit/Delete) → Idle | Wrong Password, Session Expired, Validation Error, DB Save Error |
| **Dashboard (DV-01)** | Loading → Reading Cards → Executing → Rendering → Displayed | Query Failure, Timeout, No Cards, DB Connection Error |
| **Drill Down (DD-01)** | Main → Level N → Level N+1 / Back | Invalid Card ID, Missing Level Config, Query Failure |

---

## 7. اعتبارات عامة

| # | الاعتبار | الوصف |
|:-:|----------|-------|
| 1 | **SQL Injection** | جميع استعلامات SQL (في Dashboard و Drill Down و Admin Panel) تستخدم `SqlParameter` — لا بناء نصي |
| 2 | **التزامن (Concurrency)** | Sync Engine يستخدم Lock لمنع تشغيل مزامنتين في نفس الوقت |
| 3 | **الأداء** | استعلامات Dashboard تُنفذ بالتوازي (`Task.WhenAll`) حيثما أمكن — كل بطاقة مستقلة |
| 4 | **خطأ بطاقة واحدة لا يؤثر على الباقي** | في Dashboard، فشل بطاقة لا يمنع عرض بقية البطاقات |
| 5 | **التراجع (Rollback)** | في Sync Workflow، الفشل في جدول لا يتراجع عن الجداول السابقة (per-table transaction) — يُحدد السلوك النهائي أثناء التطوير |
| 6 | **الجلسات (Sessions)** | Admin Panel يستخدم Session Cookie مع Timeout (20 دقيقة افتراضياً) |
| 7 | **Breadcrumb** | جميع مستويات Drill Down مسجلة في Breadcrumb للتنقل السهل عكسياً |

---

## References

| الوثيقة | المسار |
|---------|--------|
| 01_PROJECT_BRIEF.md | `project-preparation/01_PROJECT_BRIEF.md` |
| 02_SCOPE_AND_BOUNDARIES.md | `project-preparation/02_SCOPE_AND_BOUNDARIES.md` |
| 04_USERS_ROLES_PERMISSIONS.md | `project-preparation/04_USERS_ROLES_PERMISSIONS.md` |
| APPLICATION_BLUEPRINT.md (§6) | `project-preparation/APPLICATION_BLUEPRINT.md` |
| CLIENT_DECISION_LOG.md | `client-engagement/CLIENT_DECISION_LOG.md` |

---

> **Prepared by:** TeraAgent — TASK-PREP-006
> **Date:** 2026-07-12
> **Status:** `preparation`
