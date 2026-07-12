# 20_API_CONTRACTS.md — WarehouseDashboard

> **النوع:** API Contracts Document — عقود واجهات برمجة التطبيقات لمحرك المزامنة (Sync Engine)
> **الحالة:** `Draft — Pending Cross-Review` (يُعتمد عبر التقييم المتبادل قبل الاستهلاك)
> **Baseline Module:** WarehouseDashboard
> **تاريخ الإعداد:** 2026-07-12
> **الجهة المرجعية:** `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` (MBA)، `APPLICATION_BLUEPRINT.md` (approved_for_preparation)
> **الملف المرجعي للتكنولوجيا:** `tera-systems/profiles/dotnet-razorpages-adonet.md`
> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-014

---

## Lifecycle Header

| الحقل | القيمة |
|-------|--------|
| **Document State** | `Draft — Pending Cross-Review` |
| **Baseline Module** | WarehouseDashboard |
| **Last Review Date** | 2026-07-12 |
| **Next Review Due** | عند اعتماد Majed / توفر تفاصيل جداول Oracle |
| **Document Type** | API Contracts Preparation |
| **Version** | 1.0.0 |
| **Consumed By** | EngineeringAgent (بناء `SyncController` في `WarehouseDashboard.Api`) |

> **ملاحظة حوكمة:** هذه الوثيقة في حالة `Draft` حتى اعتمادها. أي وكيل (SDA/EngineeringAgent) يقرأها قبل الاعتماد يجب أن يرفع `Design Gap` وفق §4.1 من تعريف Software Designer Agent. المصادر المرجعية كلها في حالة `MBA` أو معتمدة.

---

## نبذة عن الوثيقة

توثق هذه الوثيقة **عقود API** لمحرك المزامنة (Sync Engine) في `WarehouseDashboard.Api`. المحرك يعمل كـ **Background Service** مستقل ويكشف مجموعة من REST endpoints للتحكم والمراقبة. جميع العقود أدناه مشتقة من القرارات المعتمدة في `08_TECHNICAL_ARCHITECTURE.md` (§5.1، §5.6، §8.1) و `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` (§5) و `APPLICATION_BLUEPRINT.md` (Module 1 — M1.5/M1.6/M1.7).

**قواعد أساسية معتمدة (من المراجع):**
- المنفذ (Port) منفصل للـ Api، يُربط في IIS كـ Application مستقل (`/api`).
- لا يوجد Token-based Auth في Phase 1 — الحماية عبر IIS IP and Domain Restrictions (شبكة داخلية).
- منع التشغيل المتداخل عبر `SemaphoreSlim` — لا تُقبل مزامنة جديدة أثناء تشغيل حالية.
- وضع Phase 1 = **Full Refresh** فقط؛ وضع **Incremental جاهز معمارياً لكن غير مُفعّل**.

> **اتفاقية التنسيق (Convention):** جميع الاستجابات (Responses) بصيغة JSON. جميع الحقول الزمنية بصيغة ISO 8601 (`yyyy-MM-ddTHH:mm:ss.fffZ` — UTC).

---

## 1. POST /api/sync/trigger

تفعيل مزامنة يدوية (Manual Override). تُستدعى من زر المزامنة في Dashboard (بعد دخول Admin على مستوى Web) أو من أدوات داخلية/مراقبة.

### 1.1 الصلاحية (Auth)

| المصدر | القاعدة |
|--------|---------|
| **مستوى Api (Phase 1)** | لا يوجد Token auth — الحماية عبر **IIS IP and Domain Restrictions** (الشبكة الداخلية / localhost فقط). أي متصل داخلي مصرّح له. |
| **مستوى Web (Dashboard)** | زر المزامنة يُعرض **فقط بعد دخول Admin** عبر `AdminAuthMiddleware` (Session). أي أن شرط "admin" يُفرض على طبقة Web، بينما شرط "internal" يُفرض على طبقة IIS للـ Api. |
| **Phase 2 (مستقبلي)** | إضافة JWT أو API Key للـ endpoints (حسب `08_TECHNICAL_ARCHITECTURE.md` §8.2). |

> **توفيق (Reconciliation) مع متطلبات المهمة:** المهمة تشترط `auth (admin or internal)`. التوفيق: **internal** = قيد IIS على الشبكة الداخلية؛ **admin** = إنفاذ على طبقة Web قبل عرض زر التشغيل. لا يوجد إنفاذ app-level في Phase 1 (حسب المرجع §8.1). انظر §6 و §7 (Design Gap #1).

### 1.2 Request Body

```json
{
  "mode": "full",                 // "full" | "incremental" — إلزامي
  "tables": []                    // اختياري: قائمة أسماء الجداول المحددة؛ فارغ = كل الجداول في TableMapping
}
```

| الحقل | النوع | إلزامي | الوصف |
|-------|-------|:------:|-------|
| `mode` | string (enum) | ✅ | `"full"` = حذف + إعادة إدخال كامل لكل جدول. `"incremental"` = تحديث تفاضلي (معماري جاهز — غير مُفعّل في Phase 1). |
| `tables` | string[] | ❌ | تقييد المزامنة على جداول محددة من `TableMapping`. افتراضياً `[]` = كل الجداول. |

### 1.3 Response (200 OK / 202 Accepted)

عند القبول (تم وضع المزامنة في الطابور أو بدأت فوراً):

```json
{
  "syncId": "SYNC-2026-07-12T10-15-00Z-8f3c",  // معرّف ارتباط (correlation id) فريد لكل طلب
  "status": "queued",                          // "queued" | "running" | "skipped"
  "message": "تم تفعيل المزامنة اليدوية (full).",
  "triggeredAt": "2026-07-12T10:15:00.000Z"
}
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| `syncId` | string | معرّف فريد يُستخدم لمتابعة الحالة عبر `GET /api/sync/status` والسجلات. |
| `status` | string (enum) | `queued` = وُضعت في الطابور (ستبدأ قريباً)؛ `running` = بدأت فوراً؛ `skipped` = رُفضت لأن مزامنة أخرى قيد التشغيل. |
| `message` | string | رسالة إنسانية قابلة للعرض. |
| `triggeredAt` | datetime (ISO 8601) | وقت استلام الطلب. |

> **سلوك Phase 1 لـ `mode: "incremental"`:** العقد يقبل القيمة، لكن المحرك يرد بـ **HTTP 501 Not Implemented** مع `errorCode: INCREMENTAL_NOT_ENABLED` (انظر §5). لا يُنفّذ أي عمل. هذا مقصود لمنع الاستخدام قبل تفعيل القرار الاستراتيجي.

### 1.4 أمثلة

```http
POST /api/sync/trigger
Content-Type: application/json

{ "mode": "full" }
```

```http
HTTP/1.1 202 Accepted
Content-Type: application/json

{
  "syncId": "SYNC-2026-07-12T10-15-00Z-8f3c",
  "status": "running",
  "message": "تم تفعيل المزامنة اليدوية (full).",
  "triggeredAt": "2026-07-12T10:15:00.000Z"
}
```

---

## 2. GET /api/sync/status

حالة المزامنة الحالية (مراقبة). تُستدعى لعرض شريط الحالة في Dashboard وعبر أدوات المراقبة.

### 2.1 Response (200 OK)

```json
{
  "isRunning": false,
  "lastSync": {
    "syncId": "SYNC-2026-07-12T09-45-00Z-1a2b",
    "startedAt": "2026-07-12T09:45:00.000Z",
    "finishedAt": "2026-07-12T09:46:12.000Z",
    "status": "Success",              // "Running" | "Success" | "Failed"
    "recordCount": 184320,
    "triggerType": "Auto"             // "Auto" | "Manual"
  },
  "progress": {
    "percent": 100,
    "currentTable": null,
    "tablesCompleted": 12,
    "tablesTotal": 12
  },
  "config": {
    "intervalMinutes": 30,
    "isAutoSyncEnabled": true
  }
}
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| `isRunning` | bool | `true` إذا كانت مزامنة قيد التشغيل الآن. |
| `lastSync.syncId` | string | معرّف آخر مزامنة (أو الحالية إذا `isRunning=true`). |
| `lastSync.startedAt` / `finishedAt` | datetime? | أوقات البدء/الانتهاء. `finishedAt=null` أثناء التشغيل. |
| `lastSync.status` | string | `Running` / `Success` / `Failed`. |
| `lastSync.recordCount` | int | إجمالي الصفوف المُحمّلة في آخر مزامنة. |
| `lastSync.triggerType` | string | `Auto` (جدولة) أو `Manual` (trigger). |
| `progress.percent` | int (0–100) | نسبة الإنجاز. أثناء التشغيل تعكس التقدم الفعلي؛ خارج التشغيل = 100 لآخر مزامنة أو 0 إن لم تبدأ بعد. |
| `progress.currentTable` | string? | اسم الجدول قيد المعالجة الحالية (أثناء التشغيل فقط). |
| `progress.tablesCompleted` / `tablesTotal` | int | عدّاد الجداول المكتملة من الإجمالي (من `TableMapping`). |
| `config.intervalMinutes` | int | فترة الجدولة التلقائية (من `SyncSettings`). |
| `config.isAutoSyncEnabled` | bool | هل المزامنة التلقائية مفعّلة. |

> **ملاحظة:** الحقل `progress` مضاف فوق استجابة المرجع (§5.6) لدعم عرض شريط تقدم حي في Dashboard؛ خارج أوقات التشغيل يُرجع `percent=100` مع `currentTable=null`.

---

## 3. GET /api/sync/logs (اختياري — Paginated)

استرجاع سجلات المزامنة بشكل مقسّم إلى صفحات (Pagination). يُستخدم لشاشة `SyncLogs/Index` وللمراجعة.

### 3.1 Query Parameters

| المعامل | النوع | افتراضي | الوصف |
|---------|-------|---------|-------|
| `page` | int | 1 | رقم الصفحة (1-based). |
| `pageSize` | int | 20 | عدد السجلات في الصفحة (الحد الأقصى 100). |
| `status` | string? | null | فلترة اختيارية: `Success` / `Failed` / `Running`. |
| `from` / `to` | datetime? | null | نطاق زمني لـ `StartTime`. |

### 3.2 Response (200 OK)

```json
{
  "page": 1,
  "pageSize": 20,
  "totalCount": 153,
  "totalPages": 8,
  "items": [
    {
      "syncLogId": 153,
      "syncId": "SYNC-2026-07-12T09-45-00Z-1a2b",
      "startTime": "2026-07-12T09:45:00.000Z",
      "endTime": "2026-07-12T09:46:12.000Z",
      "status": "Success",
      "recordCount": 184320,
      "durationMs": 72000,
      "triggerType": "Auto",
      "errorMessage": null
    }
  ]
}
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| `page` / `pageSize` / `totalCount` / `totalPages` | int | بيانات التقسيم (Pagination metadata). |
| `items[]` | array | قائمة السجلات مرتبة تنازلياً حسب `startTime`. |
| `items[].syncLogId` | long | PK من جدول `SyncLogs`. |
| `items[].syncId` | string | معرّف الارتباط (يطابق `syncId` في trigger/status). |
| `items[].durationMs` | long? | المدة بالميللي ثانية (null أثناء التشغيل). |
| `items[].errorMessage` | string? | تفاصيل الخطأ إن فشلت المزامنة. |

---

## 4. GET /api/sync/config (اختياري)

استرجاع إعدادات المزامنة الحالية للعرض/المراجعة.

### 4.1 Response (200 OK)

```json
{
  "intervalMinutes": 30,
  "isAutoSyncEnabled": true,
  "lastSyncTimestamp": "2026-07-12T09:46:12.000Z",
  "source": "SqlServer:SyncSettings"
}
```

| الحقل | النوع | الوصف |
|-------|-------|-------|
| `intervalMinutes` | int | فترة الجدولة (من `SyncSettings.IntervalMinutes`). |
| `isAutoSyncEnabled` | bool | من `SyncSettings.IsAutoSyncEnabled`. |
| `lastSyncTimestamp` | datetime? | آخر وقت مزامنة ناجحة (من `SyncSettings.LastSyncTimestamp`). |
| `source` | string | مصدر القراءة (للتشخيص). |

> **ملاحظة توافق مع Blueprint:** `APPLICATION_BLUEPRINT.md` (§4 Module 1) يذكر أيضاً `PUT /api/sync/config` لتحديث الإعدادات. هذا العقد يوثّق `GET` فقط حسب نطاق المهمة؛ يُوصى بتوثيق `PUT` في إصدار لاحق أو ملف منفصل. لا يُنفّذ `PUT` في Phase 1 (التغيير عبر `appsettings.json` حالياً).

---

## 5. Error Codes و HTTP Status Mapping

جميع الأخطاء تُرجع جسم JSON موحّد:

```json
{
  "errorCode": "SYNC_ALREADY_RUNNING",
  "message": "مزامنة أخرى قيد التشغيل حالياً. استخدم GET /api/sync/status للمتابعة.",
  "timestamp": "2026-07-12T10:20:00.000Z",
  "traceId": "tr-9c1e-4b7a"
}
```

| HTTP Status | errorCode | المعنى | متى يحدث |
|:-----------:|-----------|-------|----------|
| `200 OK` | — | نجاح (GET responses). | استعلام حالة/سجلات/إعدادات. |
| `202 Accepted` | — | قُبل الطلب (trigger). | بدأت/وُضعت المزامنة في الطابور. |
| `400 Bad Request` | `INVALID_REQUEST` | جسم الطلب غير صالح. | `mode` مفقود أو قيمة غير معروفة، أو `tables` بصيغة خاطئة. |
| `409 Conflict` | `SYNC_ALREADY_RUNNING` | مزامنة قيد التشغيل. | طلب trigger جديد بينما `isRunning=true` (SemaphoreSlim). `status` في الاستجابة = `skipped`. |
| `501 Not Implemented` | `INCREMENTAL_NOT_ENABLED` | الوضع غير مُفعّل. | `mode: "incremental"` في Phase 1 (معماري جاهز، غير مُفعّل). |
| `403 Forbidden` | `NETWORK_RESTRICTED` | مقيد بالشبكة. | متصل من خارج نطاق IIS IP restriction (شبكة خارجية). |
| `500 Internal Server Error` | `INTERNAL_ERROR` | خطأ غير متوقع في المحرك. | فشل غير معالَج في `SyncEngineService`. |
| `503 Service Unavailable` | `SERVICE_UNAVAILABLE` | الخدمة غير جاهزة. | المحرك لم يكمل الإقلاع بعد، أو SQL Server/Oracle غير متاح. |

**قواعد إضافية:**
- `409` للـ trigger أثناء التشغيل = سلوك طبيعي (idempotent-safe) — لا يُسجَّل كخطأ فادح.
- `501` للـ incremental = متعمد في Phase 1؛ يُزال عند تفعيل القرار الاستراتيجي.
- `traceId` اختياري للتشخيص ويرتبط بسجلات التطبيق.

---

## 6. Security — عدم تعريض الـ endpoints لـ Public Dashboard Viewers

القاعدة الأمنية الأساسية: **واجهات المزامنة ليست جزءاً من واجهة Dashboard العامة ولا يُفترض وصول المشاهدين العموميين إليها.**

| البند | التفاصيل |
|-------|----------|
| **فصل الـ Application** | الـ Api يُنشر كـ Application منفصل (`/api`) في IIS، منفصل عن تطبيق Web العام (`/`). المشاهد العادي للـ Dashboard لا يصل إلى `/api` عبر الواجهة. |
| **قيد الشبكة (Network Restriction)** | IIS **IP and Domain Restrictions** — يُسمح فقط للشبكة الداخلية / localhost. أي طلب من خارج النطاق يرد بـ `403 NETWORK_RESTRICTED`. |
| **لا Token Auth في Phase 1** | حسب `08_TECHNICAL_ARCHITECTURE.md` §8.1 — الـ Api "ليس له Auth"؛ الحماية عبر الشبكة فقط. |
| **إنفاذ Admin على طبقة Web** | زر المزامنة في Dashboard يُعرض **فقط بعد جلسة Admin صالحة** (`AdminAuthMiddleware`). المشاهد العام (Viewer) لا يرى الزر ولا يمكنه إطلاق trigger. |
| **Dashboard عام بلا Auth** | الـ Dashboard نفسه متاح لكل مستخدمي الشبكة المحلية (للعرض فقط) — لا يكشف أي endpoint تحكم. |
| **Oracle Read-Only** | المحرك لا يكتب أبداً في Oracle (صلاحية `READ ONLY`) — تقليل سطح المخاطر. |
| **Phase 2** | إضافة JWT / API Key للـ endpoints + RBAC (Admin/Viewer) حسب `08_TECHNICAL_ARCHITECTURE.md` §8.2. |

> **الخلاصة:** "admin or internal" = المشاهد العام لا يملك أي مسار لتشغيل أو مراقبة المزامنة؛ التشغيل مقصور على (أ) متصل داخلي مُصرّح به عبر IIS، و(ب) مستخدم Admin اجتاز تسجيل الدخول على طبقة Web.

---

## 7. Design Gaps / ملاحظات

| # | الملاحظة | التأثير | الحالة |
|:-:|---------|:-------:|:------:|
| 1 | **تعارض بسيط في متطلب Auth:** المهمة تشترط `auth (admin or internal)` بينما `08_TECHNICAL_ARCHITECTURE.md` §8.1 ينص على "لا Auth للـ Api في Phase 1". التوفيق مُطبّق أعلاه (internal=IIS، admin=Web-layer) لكنه يحتاج تأكيد Majed إن كان المطلوب Token app-level فعلياً في Phase 1. | أمني | يحتاج مراجعة |
| 2 | تفاصيل جداول Oracle (أسماء/هياكل) غير معروفة — تؤثر على محتوى `tables[]` الفعلي في trigger وعدد `tablesTotal` في progress، لكن **لا تؤثر على شكل العقد (contract shape)**. | منخفض | مؤجل (TBD أثناء التنفيذ) |
| 3 | `PUT /api/sync/config` مذكور في Blueprint لكن غير مشمول هنا (خارج نطاق المهمة). يُوصى بتوثيقه لاحقاً. | منخفض | مقترح لاحقاً |
| 4 | `progress.currentTable` و`tablesCompleted/tablesTotal` يتطلبان تتبّعاً داخل `SyncEngineService` لم يُوثّق تفصيله بعد — العقد يحدد الشكل فقط. | منخفض | يُنفّذ أثناء البناء |

---

## 8. Compliance Checklist

| البند | الحالة | المرجع |
|-------|:------:|--------|
| `POST /api/sync/trigger` موثّق (mode/response/auth) | ✅ | §1، `08` §5.1/§5.6 |
| `GET /api/sync/status` موثّق (lastSync/isRunning/progress) | ✅ | §2، `14` §5.1 |
| `GET /api/sync/logs` موثّق (paginated) | ✅ | §3، `08` §5.6 |
| `GET /api/sync/config` موثّق | ✅ | §4، `14` §5.1 |
| Error codes + HTTP mapping | ✅ | §5 |
| Security (لا تعريض للـ public viewers) | ✅ | §6، `08` §8.1 |
| توافق مع القرارات المعتمدة (Full Refresh Phase 1 / Incremental جاهز) | ✅ | `08` §5.3/§5.4 |
| Lifecycle Header موجود | ✅ | `Draft — Pending Cross-Review` |

---

> **إعداد:** Software Designer Agent (مُصمم) — TASK-PREP-014
> **تاريخ:** 2026-07-12
> **الحالة:** `Draft — Pending Cross-Review` ⏳ (يُعتمد عبر التقييم المتبادل قبل الاستهلاك بواسطة EngineeringAgent)
> **المرجع:** `08_TECHNICAL_ARCHITECTURE.md` (MBA)، `14_INTEGRATIONS_AND_EXTERNAL_SERVICES.md` (MBA)، `APPLICATION_BLUEPRINT.md` (approved_for_preparation)
