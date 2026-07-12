# 22_DEPLOYMENT_AND_ENVIRONMENTS.md — WarehouseDashboard

> **النوع:** Deployment & Environments Document — توثيق النشر وبيئات التشغيل
> **الحالة:** `Prepared (Pending Baseline Approval)`
> **Baseline Module:** WarehouseDashboard
> **تاريخ الإعداد:** 2026-07-12
> **مُعدّ بالاعتماد على:** `08_TECHNICAL_ARCHITECTURE.md` (§9 Deployment / Hosting، الحالة MBA) + `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` (§1.3 Connection String Storage، الحالة MBA) + `tera-system/profiles/dotnet-razorpages-adonet.md` (§5 Scaffold، §9 IIS hosting)
> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-015

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Prepared (Pending Baseline Approval)` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | عند توفر Connection String الفعلي و TNS (انظر §6 ملاحظات) |
| **Document Type** | Deployment & Environments Preparation |
| **Version** | 1.0.0 |
| **Derived From** | 08 (MBA)، 14 (MBA)، dotnet-razorpages-adonet |

> **ملاحظة حوكمة:** هذه الوثيقة مُعدّة من مراجع معتمدة (MBA) لكنها لم تُعتمد بعد كجزء من Module Baseline. تُرفع للموافقة عبر المسار المعتاد قبل بدء مهام النشر الفعلية.

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **منهجية النشر (Deployment)** و**إدارة البيئات (Environments)** لتطبيق **WarehouseDashboard** على **IIS محلي** (Windows Server). النظام مكوّن من مشروعين منشورين (Published) تحت نفس موقع IIS:

- `WarehouseDashboard.Api` ← محرك المزامنة (Sync Engine) — Background Service
- `WarehouseDashboard.Web` ← Dashboard + Admin Panel (Razor Pages)

كل المنطق محلي: Oracle (مصدر خارجي للقراءة فقط)، SQL Server (وجهة محلية)، IIS (استضافة محلية). لا يوجد Cloud أو خدمات خارجية (انظر `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §6).

---

## 1. IIS Setup — إعداد IIS

### 1.1 متطلبات السيرفر (Server Prerequisites)

| المتطلب | التفاصيل | المرجع |
|---------|----------|--------|
| **نظام التشغيل** | Windows Server 2019+ أو Windows 10/11 Pro | `08` §9.3 |
| **IIS** | IIS 10+ مع تفعيل `ASP.NET Core Hosting` (via Hosting Bundle) | `08` §9.3 |
| **.NET 8 Runtime** | تثبيت `ASP.NET Core 8.x Hosting Bundle` (يوفر Runtime + IIS Hosting Module) | `08` §9.2، `dotnet-razorpages-adonet` §5/§9 |
| **SQL Server** | SQL Server 2019+ (أو Express) — محلي | `08` §2.3 |
| **Oracle** | لا حاجة لـ Oracle Client — `Oracle.ManagedDataAccess.Core` (Managed Driver) | `14` §1.1 |
| **الذاكرة RAM** | 4GB كحد أدنى (8GB موصى به) | `08` §9.3 |
| **التخزين** | 50GB + مساحة بيانات Oracle/SQL Server | `08` §9.3 |

> **خطوة حرجة:** تثبيت `dotnet-hosting-8.x.x-win.exe` (ASP.NET Core Hosting Bundle) على السيرفر قبل النشر. بدونه، لن يتعرف IIS على تطبيقات .NET 8. يُفضّل إعادة تشغيل السيرفر أو `iisreset` بعد التثبيت.

### 1.2 Application Pools — مجموعات التطبيقات

يُنشأ **Pool منفصل لكل مشروع** بإعدادات موحّدة:

| Pool | المشروع | .NET CLR Version | Managed Pipeline | Identity |
|------|---------|------------------|------------------|----------|
| `WDApiPool` | `WarehouseDashboard.Api` | **No Managed Code** | Integrated | `ApplicationPoolIdentity` |
| `WDWebPool` | `WarehouseDashboard.Web` | **No Managed Code** | Integrated | `ApplicationPoolIdentity` |

> **لماذا No Managed Code؟** تطبيقات .NET Core / .NET 8 لا تستخدم CLR الكلاسيكي. يتم توجيه الطلبات عبر `AspNetCoreModuleV2` (من Hosting Bundle). اختيار `No Managed Code` هو الإعداد الصحيح والوحيد المعتمد لـ .NET 8 على IIS (`08` §9.1).

**إعدادات موصى بها لـ Pool (موثوقية BackgroundService):**

| الإعداد | القيمة | السبب |
|---------|--------|-------|
| `Idle Time-out (minutes)` | **0** (معطّل) | منع توقف BackgroundService (محرك المزامنة) عند الخمول |
| `Regular Time Interval (minutes)` | **0** (أو قيمة عالية) | منع إعادة التدوير الدورية التي توقف المزامنة |
| `Start Mode` | `AlwaysRunning` | ضمان إقلاع Pool فوراً مع IIS |

### 1.3 هيكل الموقع والـ Bindings — الموقع والربط

```
IIS Server
│
└── Site: WarehouseDashboard
    │
    ├── /            → Application: WarehouseDashboard.Web  (Pool: WDWebPool)
    │
    └── /api         → Application: WarehouseDashboard.Api  (Pool: WDApiPool)
```

| العنصر | القيمة | ملاحظات |
|--------|--------|---------|
| **Site Binding** | `http://localhost` أو `http://server-ip` (المنفذ 80) | شبكة داخلية محلية |
| **Web App** | المسار الفيزيائي `C:\WarehouseDashboard\web` | Pool: `WDWebPool` |
| **Api App** | المسار الفيزيائي `C:\WarehouseDashboard\api` | Pool: `WDApiPool` — مربوط كـ Application منفصل تحت `/api` (`08` §9.1) |
| **HTTPS** | اختياري (شبكة داخلية) | يُنصح به لاحقاً عبر شهادة داخلية إن توفرت |

> **ملاحظة:** الـ Api يعمل على نفس الموقع تحت `/api` (يستمع عبر نفس binding). لا حاجة لمنفذ منفصل لأنه Application داخلي ضمن IIS (`08` §5.6 ملاحظة). الوصول للـ Api داخلي فقط — يمكن تقييده عبر IIS `IP and Domain Restrictions` (`08` §8.1).

### 1.4 خطوات النشر (Publish + IIS)

| # | الخطوة | الأمر / الإجراء (كمستند توثيقي) |
|---|--------|-----------------------------------|
| 1 | نشر Api | `dotnet publish src/WarehouseDashboard.Api -c Release -o C:\WarehouseDashboard\api` |
| 2 | نشر Web | `dotnet publish src/WarehouseDashboard.Web -c Release -o C:\WarehouseDashboard\web` |
| 3 | إنشاء Pools | IIS Manager ← Application Pools ← `No Managed Code` |
| 4 | إنشاء Site + Applications | ربط `/` بـ WDWebPool، `/api` بـ WDApiPool |
| 5 | تعيين Connection Strings | عبر Environment Variables أو `appsettings.json` المشفّر (انظر §2، §4) |
| 6 | تثبيت Hosting Bundle | `dotnet-hosting-8.x.x-win.exe` (مثبّت مسبقاً على السيرفر) |
| 7 | اختبار | `http://localhost/` + `http://localhost/api/sync/status` |

> هذه الخطوات مطابقة لـ `08_TECHNICAL_ARCHITECTURE.md` §9.2 وتُفصّل هنا للنشر.

---

## 2. Environment Config — إعدادات البيئة والبيانات السرية

### 2.1 المبدأ العام

تُفصَل **الأسرار (Secrets)** عن الكود المنشور. القاعدة (من `08` §8.4 و `14` §1.3):

- `appsettings.json` **لا يحتوي كلمات مرور حقيقية** — فقط أسماء متغيّرات البيئة أو عناصر نائبة.
- كلمات المرور تُمرَّر عبر **Environment Variables** على مستوى النظام أو Pool.
- عند النشر، يُشفَّر قسم `ConnectionStrings` في `appsettings.json` عبر `aspnet_regiis -pe`.

### 2.2 بنية appsettings.json (كمستند — لا أسرار)

مثال هيكلي لأقسام `appsettings.json` (القيم الحسّاسة كمراجع فقط):

```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=[ORACLE_USER];Password=[ORACLE_PASS_ENV];Data Source=[ORACLE_TNS_OR_EASY_CONNECT]",
    "SqlServerDb": "Server=[SQL_SERVER];Database=WarehouseDashboard;User Id=[SQL_USER];Password=[SQL_PASS_ENV];TrustServerCertificate=True"
  },
  "SyncSettings": {
    "IntervalMinutes": 30,
    "IsAutoSyncEnabled": true
  },
  "Syncfusion": {
    "LicenseKey": "[SYNCFUSION_KEY_ENV]"
  },
  "AdminPanel": {
    "HiddenRoute": "/admin-secure-panel"
  }
}
```

> **قاعدة:** القيم بين `[...]` تُستبدل وقت التشغيل من Environment Variables ولا تُحفظ في الملف المنشور.

### 2.3 Environment Variables — متغيّرات البيئة (النهج المُوصى به)

تُعيين على مستوى **System Environment Variables** في Windows (أو عبر IIS Configuration Editor لكل Pool):

| المتغيّر | الغرض | يُستخدم في |
|----------|-------|------------|
| `OraclePass` | كلمة مرور حساب Oracle (READ ONLY) | `WarehouseDashboard.Api` |
| `SqlServerPass` | كلمة مرور حساب SQL Server | `Api` + `Web` |
| `SyncfusionLicenseKey` | مفتاح رخصة Syncfusion | `Api` + `Web` (Program.cs) |
| `OracleConnectionString` (اختياري) | نص الاتصال الكامل إن تفضّل تمريره ككتلة | `Api` |

> يقرأ التطبيق القيم من `Environment.GetEnvironmentVariable(...)` أو عبر `IConfiguration` الذي يدمج Environment Variables تلقائياً. هذا يتوافق مع `dotnet-razorpages-adonet` §5 و `14` §1.3.

### 2.4 تشفير IIS (Encryption) — aspnet_regiis

لتشفير قسم `ConnectionStrings` داخل `appsettings.json` المنشور على السيرفر:

```text
# من موجه الأوامر (كمسؤول) — مسار يختلف حسب إصدار .NET:
cd C:\Windows\Microsoft.NET\Framework64\v8.0.xxxx\
aspnet_regiis -pe "ConnectionStrings" -app "/WarehouseDashboard" -prov "DataProtectionConfigurationProvider"
```

- يُشفَّر القسم باستخدام مفتاح machine-level — لا يمكن قراءته خارج السيرفر.
- `Web` يقرأ `SqlServerDb` فقط؛ `Api` يقرأ `OracleDb` + `SqlServerDb`.
- المرجع: `08` §8.4، `14` §1.3.

> **تحذير:** التشفير مرتبط بجهاز معيّن. عند النقل لسيرفر آخر، يلزم إعادة التشفير أو نقل مفتاح RSA. لذلك Environment Variables تبقى النهج الأساسي الأبسط.

---

## 3. Syncfusion License — تسجيل رخصة Syncfusion

### 3.1 المبدأ

مكتبة Syncfusion تتطلب تسجيل `UnlockKey` (رخصة Community) قبل استخدام أي مكوّن. الرخصة **متوفرة** (مفتاح جاهز لدى العميل/المشروع) لكن **لا يُكتب قيمتها في أي وثيقة أو ملف منشور**.

### 3.2 التسجيل في Program.cs (كمستند — لا قيمة المفتاح)

يُسجَّل المفتاح في `Program.cs` لكلا المشروعين (`Api` و`Web`) في بداية `Main` وقبل `builder.Build()`:

```csharp
// Program.cs (Web و Api) — تسجيل الرخصة من Environment Variable
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    Environment.GetEnvironmentVariable("SyncfusionLicenseKey") ?? "[SYNCFUSION_KEY_REDACTED]"
);
```

- القيمة تُؤخذ من `SyncfusionLicenseKey` (Environment Variable) — لا تُحفظ في الكود.
- إن تعذّرت القراءة، يُستخدم عنصر نائب `[SYNCFUSION_KEY_REDACTED]` يُستبدل وقت النشر.
- الحزمة المطلوبة: `Syncfusion.Licensing` + `Syncfusion.Blazor` (للتسجيل فقط — الواجهة الفعلية Razor Pages، حسب `08` §2.2).
- المرجع: `dotnet-razorpages-adonet` §5 (تسجيل عبر `RegisterLicense`)، `08` §2.1/§10.1 (القرار #3).

> **قاعدة صارمة (من المهمة):** لا يُكتب مفتاح Syncfusion بأي شكل في هذه الوثيقة أو في ملفات `appsettings.json` المنشورة. يُمرَّر حصراً عبر Environment Variable أو ملف محمي خارج التحكم بالمصدر.

---

## 4. Oracle + SQL Server Connection Config — إعدادات الاتصال

### 4.1 Oracle (المصدر — قراءة فقط)

| البند | القيمة / النهج | المرجع |
|------|----------------|--------|
| **المكتبة** | `Oracle.ManagedDataAccess.Core` (3.x) — Managed Driver | `14` §1.1، `08` §2.1 |
| **الحاجة لـ Oracle Client** | لا — Managed Driver لا يتطلب Instant Client | `14` §1.1 |
| **الصلاحية** | `READ ONLY` على الجداول/Views المطلوبة (يُؤكد مع DBA) | `14` §1.2، Open Issue #3 |
| **مصادقة الاتصال** | `User ID / Password` أو `OS Authentication` (TNS / Easy Connect) — يحددها العميل | `14` §1.3 |
| **تخزين كلمة المرور** | Environment Variable `OraclePass` أو قسم مشفّر | `14` §1.3، §2 هنا |
| **المشروع المستخدم** | `WarehouseDashboard.Api` فقط | `14` §1.1 |

> **مؤجَّل (Deferred):** تفاصيل TNS / Easy Connect الفعلية وقائمة الجداول تُحدّد أثناء التنفيذ (Oracle Testing Early — أول 3 أيام). لا يُكتب أي connection string فعلي هنا (`14` §7).

### 4.2 SQL Server (الوجهة — Config + Logs + Data)

| البند | القيمة / النهج | المرجع |
|------|----------------|--------|
| **المكتبة** | `Microsoft.Data.SqlClient` (5.x) — للـ Api/Web | `08` §2.1 |
| **ORM للـ Config + Logs** | EF Core `Microsoft.EntityFrameworkCore.SqlServer` (8.x) | `08` §2.2 |
| **قاعدة البيانات** | `WarehouseDashboard` (نفس القاعدة لـ Config + Logs + Data Tables) | `08` §3.2، §4.3 |
| **تخزين كلمة المرور** | Environment Variable `SqlServerPass` أو قسم مشفّر | `14` §1.3، §2 هنا |
| **المصادقة** | `SQL Server Auth` (موصى به للفصل) أو `Windows Auth` (Integrated) | حسب بيئة العميل |

### 4.3 عزل الاتصالات (Separation of Concerns)

- `Api` يتصل بـ **Oracle + SQL Server**.
- `Web` يتصل بـ **SQL Server فقط** (لا Oracle، لا منطق مزامنة) — حسب `08` §1.3.
- لا يوجد اتصال مباشر بين `Api` و `Web` — نقطة التقاءهما الوحيدة هي SQL Server (`08` §1.3).

---

## 5. Scheduled Sync — جدولة المزامنة

### 5.1 النهج المعتمد (Committed Approach)

حسب `08_TECHNICAL_ARCHITECTURE.md` §5.5 و القرار #9 (§10.1)، المزامنة تُجدوَل داخلياً عبر:

- **الآلية:** `PeriodicTimer` داخل `BackgroundService` (`SyncEngineService`) في `WarehouseDashboard.Api`.
- **الفترة الافتراضية:** **30 دقيقة** (قابلة للتكوين عبر `SyncSettings.IntervalMinutes` في `appsettings.json`).
- **منع التداخل:** `SemaphoreSlim` — إن كانت المزامنة قيد التشغيل تُتخطّى الدورة.
- **الإيقاف الآمن:** `CancellationToken` — يتوقف خلال ~5 ثوانٍ من إيقاف الخدمة.

> هذا النهج **معتمد نهائياً** ولا يتطلب Hangfire أو Quartz.NET (`08` §10.1 قرار #9).

### 5.2 اعتبار موثوقية IIS (مهم — Design Note)

بما أن `Api` يُستضاف داخل IIS (Application تحت `/api`)، فإن `BackgroundService` خاضع لدورة حياة الـ App Pool. لضمان استمرار المزامنة كل 30 دقيقة:

1. **تعطيل Idle Timeout** (`0`) و **Regular Time Interval** (`0`) لـ `WDApiPool` (انظر §1.2).
2. **Start Mode = AlwaysRunning**.
3. بديل موثوق (إن رُفض الاعتماد على IIS للخدمة الطويلة): تشغيل `Api` كـ **Windows Service** مستقل عبر `sc create` بدلاً من IIS — لكن هذا يخالف القرار المعتمد (IIS Hosting، `08` §10.1 قرار #12). يُوثَّق هنا كخيار طوارئ لا غير.

### 5.3 التشغيل اليدوي (Manual Trigger)

يمكن إجبار مزامنة فورية عبر:

- `POST /api/sync/trigger` (من `Api`) — يُستدعى من زر "تحديث" في Dashboard أو عبر Task Scheduler.

### 5.4 Windows Task Scheduler (خيار تكميلي — لا بديل عن المعتمد)

إن رُغب بضمان تنفيذ دوري مستقل عن IIS (احتياطي)، يُنشأ Task في Windows Task Scheduler يستدعي `POST /api/sync/trigger` كل 30 دقيقة عبر `curl` أو PowerShell `Invoke-RestMethod`. هذا **تكميلي** ولا يستبدل `PeriodicTimer` المعتمد.

> **القرار الموصى به:** الاعتماد على `PeriodicTimer` (المعتمد) + ضبط Pool لمنع التدوير. Task Scheduler = احتياطي اختياري فقط.

---

## 6. Update / Rollback Procedure — إجراء التحديث والتراجع

### 6.1 إجراء التحديث (Update)

| # | الخطوة | الإجراء |
|---|--------|---------|
| 1 | وضع صيانة (اختياري) | إيقاف App Pool مؤقتاً أو تحويله لصفحة Maintenance |
| 2 | نسخ احتياطي | نسخ مجلد `C:\WarehouseDashboard\` الحالي + نسخة من `appsettings.json` المشفّر + Environment Variables (تصدير) |
| 3 | إيقاف Pools | `WDApiPool` + `WDWebPool` (أو Stop Site) |
| 4 | إعادة النشر | `dotnet publish` إلى `C:\WarehouseDashboard\api` و `web` (استبدال الملفات) |
| 5 | استعادة الإعدادات | التأكد من `appsettings.json` المشفّر + Environment Variables موجودة |
| 6 | تشغيل Pools | Start `WDApiPool` ثم `WDWebPool` |
| 7 | اختبار | `http://localhost/` + `http://localhost/api/sync/status` + تشغيل مزامنة تجريبية يدوية |
| 8 | مراقبة السجلات | مراجعة `SyncLogs` للتأكد من نجاح المزامنة الأولى |

> **قاعدة:** التحديث على مستوى **التطبيق المنشور فقط** — لا يُطبَّق `dotnet ef database update` إلا في مهام قاعدة بيانات مخصّصة ومعتمدة (`dotnet-razorpages-adonet` §6).

### 6.2 إجراء التراجع (Rollback)

| # | الخطوة | الإجراء |
|---|--------|---------|
| 1 | إيقاف Pools | Stop `WDApiPool` + `WDWebPool` |
| 2 | استعادة المجلد | استرجاع `C:\WarehouseDashboard\` من النسخة الاحتياطية (الخطوة 2 أعلاه) |
| 3 | استعادة الإعدادات | استرجاع `appsettings.json` المشفّر + Environment Variables إن تغيّرت |
| 4 | تشغيل Pools | Start Pools بالترتيب |
| 5 | اختبار | نفس خطوات الاختبار §6.1 |

> **ملاحظة قاعدة البيانات:** التراجع عن **كود التطبيق** لا يتراجع تلقائياً عن تغييرات schema (Migrations). أي تغيير في Config/Logs schema يجب أن يُدار في مهمة DB منفصلة قابلة للتراجع (Down Migration). لا يُطبَّق تراجع DB تلقائياً مع تراجع الكود.

### 6.3 حدود التراجع (Rollback Limits)

- ✅ قابل للتراجع: ملفات التطبيق المنشورة، إعدادات IIS، Environment Variables.
- ⚠️ يتطلب إدارة منفصلة: تغييرات SQL Server schema (Migrations) — عبر Down Migration مخصّص.
- ⚠️ لا يُتراجع: بيانات Data Tables المُحمّلة (تُعاد تعبئتها عبر مزامنة جديدة).

---

## 7. Compliance Checklist — قائمة الامتثال

| البند | الحالة | المرجع |
|-------|:------:|--------|
| IIS Pools بإعداد `No Managed Code` | ✅ موثّق | §1.2، `08` §9.1 |
| تثبيت ASP.NET Core 8 Hosting Bundle | ✅ مطلوب | §1.1 |
| فصل الأسرار عبر Environment Variables | ✅ موثّق | §2، `14` §1.3 |
| تشفير `ConnectionStrings` عبر `aspnet_regiis -pe` | ✅ موثّق | §2.4، `08` §8.4 |
| تسجيل Syncfusion License من Environment Variable (لا قيمة مكتوبة) | ✅ موثّق | §3، `dotnet-razorpages-adonet` §5 |
| Oracle اتصال قراءة فقط + Managed Driver | ✅ موثّق | §4.1، `14` §1 |
| جدولة 30 دقيقة عبر `PeriodicTimer` (المعتمد) | ✅ موثّق | §5.1، `08` §5.5 |
| إجراء Update/Rollback واضح | ✅ موثّق | §6 |
| Lifecycle Header موجود | ✅ | أعلى الوثيقة |

---

## 8. Open Issues / Gaps — فجوات ومسائل مفتوحة

| # | المسألة | التأثير | الحل / المصدر |
|:-:|---------|:-------:|---------------|
| 1 | تفاصيل Oracle TNS / Easy Connect الفعلية غير معروفة | يؤخر اختبار الاتصال | يُحدّد أثناء التنفيذ (Oracle Testing Early) — `14` §7 |
| 2 | هل صلاحية Oracle `READ ONLY` مضمونة من DBA؟ | أمني | يُؤكد مع العميل أثناء الإعداد — `14` Open Issue #3 |
| 3 | قيمة مفتاح Syncfusion متوفرة لكن **لا تُكتب** | حوكمة أسرار | تُمرَّر عبر `SyncfusionLicenseKey` Env Var فقط — §3 |
| 4 | اختيار `Windows Auth` مقابل `SQL Auth` لـ SQL Server | إعداد بيئة | يُحدّد مع العميل أثناء النشر |
| 5 | موثوقية `BackgroundService` تحت IIS مقابل Windows Service | تشغيلي | الحل الموصى به في §5.2 (ضبط Pool) — لا يغيّر القرار المعتمد |

> لا توجد فجوة تصميمية **تحُول** دون نشر هذه الوثيقة؛ كل البنود أعلاه مؤجَّلة للتنفيذ/الإعداد وهي متوافقة مع `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` §7.

---

## 9. Task Engineering Review Decision (للتدقيق)

| الحقل | القيمة |
|-------|--------|
| **Decision** | `APPROVED_FOR_GATE` (كنص تصميمي — بانتظار اعتماد Module Baseline) |
| **النطاق** | وثيقة نشر/بيئات — لا كود تنفيذي |
| **المراجع المستهلكة** | `08` (MBA) ✅، `14` (MBA) ✅، `dotnet-razorpages-adonet` ✅ |
| **فجوات مصمّمة (Design Gaps)** | لا توجد فجوة معطّلة — مؤجّلات متوافقة مع `14` §7 |
| **الوكلاء المقترحون للمراجعة لاحقاً** | EngineeringAgent (نشر فعلي)، Auditor (حوكمة أسرار) |

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-015
> **تاريخ:** 2026-07-12
> **الحالة:** `Prepared (Pending Baseline Approval)`
> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` (MBA)، `tera-system/profiles/dotnet-razorpages-adonet.md`
