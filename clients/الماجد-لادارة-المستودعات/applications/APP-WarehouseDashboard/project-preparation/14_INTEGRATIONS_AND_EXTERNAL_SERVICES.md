# 14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md — WarehouseDashboard

> **النوع:** Integrations & External Services Document — توثيق التكاملات والخدمات الخارجية
> **الحالة:** `Module Baseline Approved`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الإعداد:** 2026-07-12
> **الجهة المعتمدة للمرجع:** `08_TECHNICAL_ARCHITECTURE.md` (الحالة: MBA) + `06_DATA_MODEL_PREPARATION.md` (الحالة: MBA)
> **الملف المرجعي للتكنولوجيا:** `tera-system/profiles/dotnet-razorpages-adonet.md`
> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-010

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Module Baseline Approved` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | N/A — ثابت طوال Phase 1 (يُراجع عند توفر تفاصيل جداول Oracle) |
| **Document Type** | Integrations & External Services Preparation |
| **Version** | 1.0.0 |
| **Deferred Items** | تفاصيل جداول Oracle الفعلية — مؤجلة (انظر §7) |

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **التكاملات والخدمات الخارجية** لتطبيق **WarehouseDashboard**. المصدر الوحيد للبيانات هو **Oracle Database** (نظام خارجي يُقرأ فقط)، والوجهة هي **SQL Server** المحلي. لا توجد أي خدمات خارجية أخرى (لا Cloud، لا SMTP، لا أنظمة طرف ثالث).

الوثيقة تعتمد كلياً على القرارات المعتمدة في `08_TECHNICAL_ARCHITECTURE.md` (§4 Data Flow، §5 Sync Engine) و `06_DATA_MODEL_PREPARATION.md` (§3.3 خريطة تحويل الأنواع) وملف التكنولوجيا `dotnet-razorpages-adonet.md`.

> **ملاحظة هامة:** تفاصيل جداول Oracle الفعلية (الأسماء، الهياكل، عدد الصفوف) **غير معروفة بعد** ويتم توفيرها من العميل أثناء التنفيذ. هذه الوثيقة توثق **المنهجية والطريقة** (method) فقط، بينما تبقى **الهياكل الفعلية مؤجلة** (TBD) — انظر §7.

---

## 1. Oracle Connection — الاتصال بـ Oracle

### 1.1 مكتبة الاتصال (Connector)

| البند | القيمة |
|-------|--------|
| **التقنية** | ODP.NET (Managed Driver) |
| **الحزمة (NuGet)** | `Oracle.ManagedDataAccess.Core` (الإصدار 3.x — أحدث مستقر) |
| **المشروع المستخدم** | `WarehouseDashboard.Api` (Sync Engine) فقط |
| **الحاجة لـ Oracle Client** | لا — Managed Driver لا يتطلب Oracle Instant Client مثبّتاً على السيرفر |
| **المرجع** | `08_TECHNICAL_ARCHITECTURE.md` §2.1، §2.2، §10.1 (القرار #4) |

### 1.2 مبدأ القراءة فقط (Read-Only)

النظام يتعامل مع Oracle كـ **مصدر بيانات خارجي للقراءة فقط**:

| القاعدة | الشرح |
|---------|-------|
| **لا كتابة أبداً** | Api لا ي执行力 أي `INSERT/UPDATE/DELETE` في Oracle — فقط `SELECT` |
| **صلاحيات قاعدة البيانات** | يُنصح بمنح حساب Oracle المستخدم صلاحية `READ ONLY` على الجداول/Views المطلوبة |
| **الاتفاق (Convention)** | حسب `dotnet-razorpages-adonet.md` §6 — اتصالات ODP.NET للقراءة فقط بالاتفاق |
| **المرجع الأمني** | `08_TECHNICAL_ARCHITECTURE.md` §8.1 (Oracle اتصال قراءة فقط) |

### 1.3 طريقة تخزين Connection String

| البند | النهج |
|-------|-------|
| **المكان الأساسي** | `appsettings.json` في `WarehouseDashboard.Api` — قسم `ConnectionStrings` |
| **كلمة المرور** | **لا تُحفظPlain Text** — يُفضل استخدام Environment Variables (مثل `OraclePass`) |
| **تشفير IIS** | عند النشر، يُشفّر قسم الاتصال عبر `aspnet_regiis -pe` |
| **المصادقة (Authentication)** | حسب بيئة العميل — `User ID / Password` أو `OS Authentication` (TNS / Easy Connect) |
| **المرجع** | `08_TECHNICAL_ARCHITECTURE.md` §8.4 (Connection Strings Protection) |

> **ملاحظة:** تفاصيل TNS Names أو Easy Connect string تُحدد أثناء التنفيذ بالتنسيق مع العميل وبيئة Oracle الخاصة به. لا يُكتب أي connection string فعلي في هذه الوثيقة.

---

## 2. Data Extraction — استخراج البيانات

### 2.1 القاعدتان الأساسيتان للاستخراج

يتم الاستخراج داخل `OracleExtractionService` عبر سلسلة:
`OracleConnection` → `OracleCommand` → `OracleDataReader` → `DataTable`

يتم تعبئة `DataTable` من `OracleDataReader`، ثم تُمرَّر إلى `SqlBulkCopy`.

### 2.2 SQL Queries مقابل Views — المنهج لكل جدول

لكل جدول Oracle مُراد نقله، يتم اختيار أحد المنهجين التاليين:

| المنهج | متى يُستخدم | الميزة | العيب | أين يُعرَّف |
|--------|-------------|--------|-------|-------------|
| **SQL Query مباشر** | عند الحاجة لتحويل/فلترة/ربط (JOIN) أثناء الاستخراج | تحكم كامل في الأعمدة والشروط | منطق الاستعلام داخل التطبيق | `TableMapping` في Sync Engine |
| **Oracle View** | عند توفر View جاهز في Oracle، أو عند رغبة DBA العميل بإدارة المنطق في Oracle | فصل المنطق في Oracle، صيانة أسهل لدى العميل | يعتمد على توفر/إنشاء الـ View في Oracle | يتم الإشارة لاسم الـ View فقط |

**قاعدة القرار الموصى بها:**

```
لكل جدول Oracle مطلوب:
  ├─ هل يوجد View جاهز في Oracle يغطي الحاجة؟
  │     ├─ نعم  → استخدم View (SELECT * FROM [ViewName])
  │     └─ لا   → اكتب SQL Query في TableMapping (SELECT أعمدة محددة ...)
  └─ هل الاستعلام يحتاج JOIN أو تحويل معقد؟
        ├─ نعم  → يُفضّل View في Oracle (لإدارة DBA) أو Query معقّد في التطبيق
        └─ لا   → Query بسيط على الجدول المباشر
```

### 2.3 ربط الجداول بـ TableMapping

يحتفظ Sync Engine بقائمة `TableMapping` تربط كل مصدر Oracle (جدول أو View أو Query) بجدول الهدف في SQL Server:

| الحقل في TableMapping | الوصف |
|----------------------|-------|
| `OracleSource` | اسم الجدول / الـ View / نص الاستعلام |
| `SourceType` | `Table` أو `View` أو `Query` |
| `SqlTargetTable` | اسم الجدول الهدف في SQL Server (مثلاً `stg_WarehouseStock`) |
| `LastSyncTimestamp` | لتتبع Incremental مستقبلاً |

> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` §3.1 (نموذج `TableMapping.cs`)، §5.3 (Full Refresh Workflow).

---

## 3. Type Mapping — خريطة تحويل الأنواع

يتم تحويل أنواع Oracle إلى .NET ثم إلى SQL Server أثناء النقل. هذه الخريطة مُستنسخة من `06_DATA_MODEL_PREPARATION.md` §3.3 (المعتمدة) لسهولة المرجع.

### 3.1 خريطة التحويل المعتمدة

| نوع Oracle | نوع .NET (في DataTable) | نوع SQL Server المقترح | ملاحظات |
|------------|------------------------|------------------------|---------|
| `NUMBER(p,0)` | `long` أو `int` | `int` أو `bigint` | يعتمد على precision |
| `NUMBER(p,s)` حيث s > 0 | `decimal` | `decimal(p,s)` | يحافظ على الدقة |
| `VARCHAR2(n)` | `string` | `nvarchar(n)` أو `nvarchar(max)` | `nvarchar` لدعم Unicode (عربي) |
| `NVARCHAR2(n)` | `string` | `nvarchar(n)` | |
| `CHAR(n)` | `string` | `nchar(n)` | |
| `DATE` | `DateTime` | `datetime2` | `datetime2` له دقة أعلى |
| `TIMESTAMP` | `DateTime` | `datetime2` | |
| `CLOB` | `string` | `nvarchar(max)` | |
| `BLOB` | `byte[]` | `varbinary(max)` | |
| `RAW(n)` | `byte[]` | `binary(n)` أو `varbinary(n)` | |
| `FLOAT` | `double` | `float` | |

### 3.2 مبدأ التحويل (الخطوات)

1. قراءة البيانات من Oracle عبر `OracleDataReader` إلى `DataTable`.
2. استخدام التحويل الضمني في ODP.NET (`GetValue()`).
3. `SqlBulkCopy` يقرأ `DataTable` مباشرة ويكتب في SQL Server.
4. **تطابق بالاسم:** أسماء أعمدة `DataTable` يجب أن تتطابق مع أسماء أعمدة الجدول الهدف في SQL Server.

### 3.3 قواعد تطبيق الخريطة

| القاعدة | الشرح |
|---------|-------|
| **تطابق الأسماء** | عمود `WAREHOUSE_NAME` في Oracle → `WarehouseName` في SQL Server (توحيد التسمية) |
| **تطابق الأنواع** | لا يُسمح بفقدان دقة (مثلاً `NUMBER(12,2)` لا يتحول لـ `int`) |
| **Unicode** | جميع النصوص `nvarchar` لدعم العربية |
| **المرجع** | `06_DATA_MODEL_PREPARATION.md` §3.3، §6.2 (لماذا nvarchar)، §6.3 (لماذا datetime2) |

---

## 4. Error Handling — معالجة الأخطاء

### 4.1 مبدأ التراجع لكل جدول (Per-Table Rollback)

كل جدول يُعالج في **معاملة مستقلة (Transaction)** خاصة به:

| المرحلة | السلوك |
|---------|--------|
| **بداية الجدول** | فتح اتصال SQL Server + `BEGIN TRANSACTION` |
| **الحذف** | `DELETE FROM [TargetTable]` |
| **التحميل** | `SqlBulkCopy.WriteToServer(DataTable)` |
| **النجاح** | `COMMIT` — الجدول محدّث بالكامل |
| **الفشل** | `ROLLBACK` — يبقى الجدول في حالته السابقة (Atomicity لكل جدول) |

> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` §4.3 (معاملة واحدة)، §5.3 (h: ROLLBACK per table)، `06_DATA_MODEL_PREPARATION.md` §3.2 (Transactional).

### 4.2 إعادة المحاولة (Retry)

| البند | النهج |
|-------|-------|
| **نطاق إعادة المحاولة** | على مستوى الجدول الفاشل فقط — لا إعادة تشغيل المزامنة كاملة |
| **عدد المحاولات** | يُقترح حد أقصى (مثلاً 3 محاولات) مع فاصل زمني قصير — يُحدد أثناء التنفيذ |
| **شرط التوقف** | بعد استنفاد المحاولات، يُسجَّل الخطأ في `ErrorLogs` وينتقل للجدول التالي |
| **تجنب التداخل** | `SemaphoreSlim` يمنع تشغيل مزامنة جديدة أثناء تشغيل الحالية |

### 4.3 التسجيل (Logging)

| الوجهة | التفاصيل |
|--------|----------|
| **SyncLogs** | سجل لكل عملية مزامنة: `StartTime`, `EndTime`, `Status`, `RecordCount`, `Duration`, `TriggerType`, `ErrorMessage` |
| **ErrorLogs** | سجل لكل خطأ تفصيلي: `SyncLogId`, `TableName`, `ErrorMessage`, `Timestamp` |
| **من يكتب** | `SyncLogService` عبر EF Core `WarehouseDashboardLogContext` (في Api) |
| **المرجع** | `08_TECHNICAL_ARCHITECTURE.md` §5.2، §5.3 (الخطوة 4)؛ `06_DATA_MODEL_PREPARATION.md` §2.1، §2.2 |

> **قاعدة:** فشل جدول واحد **لا يوقف** بقية الجداول — يُسجَّل ويفشل كجزء من حالة المزامنة الكلية (`Failed` مع تفاصيل)، بينما تبقى الجداول الناجحة محدّثة.

---

## 5. Sync API — واجهات المزامنة

محرك المزامنة يكشف مجموعة من **REST endpoints** للتحكم والمراقبة. هذه endpoints تخص `WarehouseDashboard.Api` (على Port منفصل، يُربط في IIS كـ Application منفصل).

### 5.1 endpoints المعتمدة

| Method | Endpoint | الوظيفة | الاستجابة (Response) |
|--------|----------|---------|----------------------|
| `POST` | `/api/sync/trigger` | تفعيل مزامنة يدوية | `{ "status": "triggered", "message": "..." }` |
| `GET` | `/api/sync/status` | حالة المزامنة الحالية | `{ "isRunning": bool, "lastSyncTime": datetime, "lastStatus": string, "lastRecordCount": int }` |
| `GET` | `/api/sync/logs` | سجلات المزامنة (آخر 100) | قائمة بـ `{ startTime, endTime, status, recordCount, duration }` |
| `GET` | `/api/sync/config` | إعدادات المزامنة الحالية | `{ "intervalMinutes": int, "isAutoSyncEnabled": bool, "lastSyncTimestamp": datetime }` |

> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` §5.1 (Manual Override)، §5.6 (API Endpoints).

### 5.2 تفاصيل التشغيل

| البند | القيمة |
|-------|--------|
| **الجدولة التلقائية** | `PeriodicTimer` داخل `BackgroundService` — الفترة الافتراضية 30 دقيقة (قابلة للتكوين) |
| **المنع المتداخل** | `SemaphoreSlim` — إذا كانت المزامنة قيد التشغيل، تُتخطى الدورة |
| **الإيقاف الآمن** | `CancellationToken` — يتوقف خلال 5 ثوانٍ من إيقاف الخدمة |
| **الأمان** | Api داخلي — لا Auth في Phase 1؛ يمكن تقييد الوصول عبر IIS (IP and Domain Restrictions) |
| **المرجع** | `08_TECHNICAL_ARCHITECTURE.md` §5.5 (الجدولة)، §8.1 (Api ليس له Auth) |

---

## 6. No Other External Services — لا توجد خدمات خارجية أخرى

تؤكد هذه الوثيقة أن **النظام محلي بالكامل** ولا يعتمد على أي خدمة خارجية غير Oracle:

| الخدمة الخارجية | الحالة |
|-----------------|--------|
| Oracle Database (Source) | ✅ الخدمة الخارجية الوحيدة — قراءة فقط |
| SQL Server (Destination) | محلي — جزء من نفس الحل |
| IIS (Hosting) | محلي — Windows Server |
| Cloud / SaaS | ❌ غير مستخدم |
| SMTP / Email | ❌ غير مستخدم |
| Queue / Message Bus | ❌ غير مستخدم |
| External APIs / 3rd-party | ❌ غير مستخدم |
| Authentication Provider خارجي | ❌ لا — BCrypt محلي |

> **الخلاصة:** Oracle هو **المصدر الخارجي الوحيد**. كل شيء آخر (SQL Server، IIS، Sync Engine، Dashboard، Admin Panel) محلي ضمن بيئة العميل. هذا متوافق مع `APPLICATION_BLUEPRINT.md` §8 (IIS Hosting محلي فقط) و `08_TECHNICAL_ARCHITECTURE.md` §1.1.

---

## 7. Deferred — مؤجل (تفاصيل جداول Oracle غير معروفة)

### 7.1 ما هو مؤجل

| البند المؤجل | السبب | متى يُحدد |
|--------------|-------|-----------|
| **أسماء جداول Oracle الفعلية** | يحددها العميل أثناء التنفيذ | أول 3 أيام (Oracle Testing Early) |
| **هياكل الأعمدة لكل جدول** | تعتمد على جداول العميل | أثناء التنفيذ |
| **عدد الصفوف / حجم البيانات** | يحدد الحاجة لـ Incremental | أثناء التنفيذ |
| **أسماء Views المتاحة في Oracle** | يعتمد على بيئة DBA العميل | أثناء التنفيذ |
| **Connection String الفعلي** | بيئة العميل | أثناء الإعداد |
| **هياكل Data Tables في SQL Server** | تُشتق من جداول Oracle | أثناء التنفيذ |

### 7.2 ما هو موثّق الآن (المنهجية فقط)

تم توثيق **الطريقة** (method) بشكل نهائي ومعتمد:

- ✅ كيفية الاتصال (ODP.NET Managed، read-only)
- ✅ كيفية الاستخراج (Query vs View per table)
- ✅ خريطة تحويل الأنواع (Oracle → .NET → SQL Server)
- ✅ معالجة الأخطاء (per-table rollback + retry + logging)
- ✅ واجهات المزامنة (trigger/status endpoints)
- ✅ عدم وجود خدمات خارجية أخرى

### 7.3 النهج الموصى به عند توفر التفاصيل (خلال التنفيذ)

```
1. العميل يُحدد جداول Oracle المطلوبة
2. لكل جدول:
   a. تحديد اسم الجدول/View الهدف في SQL Server
   b. تحويل الأنواع حسب خريطة §3.1
   c. إنشاء الجدول يدوياً (CREATE TABLE) — لا EF Core (القاعدة G5)
   d. إضافة PK + Indexes حسب الحاجة
   e. إضافة الجدول إلى TableMapping في Sync Engine
   f. اختبار المزامنة
3. تحديث هذه الوثيقة بالهياكل الفعلية عند توفرها
```

> **المرجع:** `06_DATA_MODEL_PREPARATION.md` §3.4 (هيكل الجداول مؤجل)، §3.5 (مثال توضيحي)، §8 Open Issues (#1).

---

## 8. Compliance Checklist

| البند | الحالة | المرجع |
|-------|:------:|--------|
| Oracle Connector محدد (ODP.NET Managed) | ✅ | §1، `08_TECHNICAL_ARCHITECTURE.md` §2.1 |
| مبدأ القراءة فقط موثّق | ✅ | §1.2، §8.1 |
| منهج Connection String موثّق | ✅ | §1.3 |
| SQL Query vs View موثّق لكل جدول | ✅ | §2 |
| خريطة تحويل الأنواع كاملة | ✅ | §3، `06_DATA_MODEL_PREPARATION.md` §3.3 |
| Per-table rollback + retry + logging | ✅ | §4 |
| Sync API endpoints موثّقة | ✅ | §5 |
| لا خدمات خارجية أخرى | ✅ | §6 |
| تأجيل تفاصيل جداول Oracle موثّق | ✅ | §7 |
| Lifecycle Header | ✅ | الحالة: Module Baseline Approved |

---

## 9. Open Issues / Gaps

| # | المشكلة | التأثير | الحل |
|:-:|---------|:-------:|------|
| 1 | تفاصيل جداول Oracle غير معروفة | يمنع توثيق الهياكل الفعلية | §7 — يُحدد أثناء التنفيذ (العميل متاح) |
| 2 | Connection String الفعلي (TNS/Easy Connect) غير معروف | يؤخر اختبار الاتصال | يُحدد في أول 3 أيام (Oracle Testing Early) |
| 3 | هل صلاحية Oracle `READ ONLY` مضمونة من DBA؟ | أمني | يُؤكد مع العميل أثناء الإعداد |

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-010
> **تاريخ:** 2026-07-12
> **الحالة:** `Module Baseline Approved` ✅ (المنهجية معتمدة — الهياكل TBD)
> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `06_DATA_MODEL_PREPARATION.md` (MBA)، `dotnet-razorpages-adonet.md`
