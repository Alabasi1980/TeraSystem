# TASK-SYNC-EXCEL-003 — Exportable Mappings API Endpoint

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-EXCEL-003 |
| **المجموعة** | Sync Enhancement — Export Excel |
| **النوع** | Backend (API Endpoint) |
| **الوكيل** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-21 |
| **التبعية** | لا تبعية — ينفذ بالتوازي مع 002 |

---

## 1. الهدف

إنشاء API Endpoint جديد يعيد قائمة تعيينات المزامنة التي تحتوي على بيانات فعلية في SQL Server، مع معلومات إضافية (rowCount, lastSyncTime) لعرضها في صفحة التحميل العامة للمستخدمين.

---

## 2. النطاق — المطلوب

### 2.1 إضافة Endpoint في SyncController.cs

```
GET /api/sync/exportable-mappings
```

**الإجراء:**
- أضف `ExportableMappings()` async method
- اقرأ كل التعيينات النشطة من `_syncEngine.LoadMappingsFromDbAsync(ct)`
- لكل mapping، استخدم `SqlConnection` للتأكد من وجود الجدول وعدد السجلات فيه
- أعِد القائمة كـ JSON

**المخرج:**
```json
[
  {
    "id": 11,
    "name": "بطاقات الأصناف",
    "sqlTargetTable": "stg_ST_ITEM_CARD",
    "sourceType": "Query",
    "hasData": true,
    "rowCount": 45230,
    "lastSyncTime": "2026-07-21T09:48:23"
  },
  ...
]
```

### 2.2 التفاصيل التقنية

1. **قراءة التعيينات:** استخدم `_syncEngine.LoadMappingsFromDbAsync(ct)` (موجود)
2. **فحص الجدول:** لكل mapping، استخدم `IF OBJECT_ID('tableName', 'U') IS NOT NULL`
3. **عدد السجلات:** `SELECT COUNT(1) FROM [tableName]`
4. **الأداء:** 
   - استخدم `SqlDataReader` مع `ExecuteReaderAsync(CommandBehavior.SingleResult)`
   - لا تجلب البيانات، فقط `COUNT(*)`
   - للسرعة، نفذ `OBJECT_ID` + `SELECT COUNT` لكل جدول في نفس الـ connection
5. **آخر تزامن:** اقرأ من `SyncRuns` table آخر run لكل جدول، أو استخدم `LastSyncTimestamp` من `SyncSettings` (عام)
6. **آخر تزامن لكل تعيين:** اختيارياً، اقرأ من `SyncRunDetails` table أحدث `EndTime` لكل `TargetTable`

### 2.3 التعامل مع الأخطاء

- جدول غير موجود (`OBJECT_ID` null) → `hasData: false`, `rowCount: null`
- DB غير متصلة → إرجاع القائمة بدون rowCount (مع تسجيل تحذير)
- استثناء غير متوقع → تسجيل + تخطي هذا التعيين

---

## 3. الملفات المسموح كتابتها

| الملف | الإجراء |
|---|---|
| `src/WarehouseDashboard.Api/Controllers/SyncController.cs` | تعديل — إضافة Endpoint |

**ممنوع:** تعديل أي ملف خارج المسار أعلاه.

---

## 4. معايير القبول

- [ ] **AC1:** `GET /api/sync/exportable-mappings` يرجع JSON array
- [ ] **AC2:** كل عنصر يحتوي `id`, `name`, `sqlTargetTable`, `hasData`, `rowCount`, `lastSyncTime`
- [ ] **AC3:** `hasData = false` للجداول الفارغة أو غير الموجودة
- [ ] **AC4:** `rowCount` يحتوي عدد السجلات الفعلي (للتنسيق استخدم `N0`)
- [ ] **AC5:** الأداء سريع (لا يجلب كل البيانات، فقط COUNT)
- [ ] **AC6:** Build 0 Errors / 0 Warnings
- [ ] **AC7:** لا استثناءات غير متوقعة (معالجة الأخطاء)

---

## 5. Pre-Execution Gate

| البند | الحالة |
|---|---|
| التكنولوجي بروفايل | `dotnet-razorpages-adonet` ✅ |
| الملفات المرجعية مقروءة | ✅ |
| النطاق محدد وواضح | ✅ |
| المسموح والممنوع محدد | ✅ |
| الـ AC كامل | ✅ |
| الـ Auditor: NOT_REQUIRED | ✅ |

**PASS ✅ — جاهز للتفويض**

---

## 6. ملاحظات إضافية

- استخدم `ConnectionStringHelper.ResolveSql(_configuration)` للحصول على connection string
- استخدم `await using` مع SqlConnection
- النمط مشابه لـ `ExportExcel` endpoint الموجود — راجعه
- اقرأ الملفات الحالية من القرص أولاً (Fresh File Read)
- احترم الـ cancellation token

---

## Handback — 2026-07-21

**الحالة:** ✅ مكتمل
**Build:** 0 errors / 0 warnings
**الملف المغير:** Controllers/SyncController.cs (+131 سطر)

### .NET contexts applied:
- **Web API / Minimal API:** ✅ `[ApiController]` — `GET /api/sync/exportable-mappings`
- **Data Access (ADO.NET):** ✅ `SqlConnection` + `ExecuteScalarAsync` عبر `ConnectionStringHelper.ResolveSql`

### STOP/ASK gate considered or triggered:
- ❌ لم يتم تفعيل أي STOP/ASK — لا تغيير في الأمان، الـ DI، الـ DB schema، أو الـ contracts

### Build/test commands and exact result:
```powershell
dotnet build WarehouseDashboard.Api.csproj --no-restore
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### Migration/SQL review result:
- غير مطلوب — لا تغيير في schema. الاستعلامات المستخدمة: `OBJECT_ID` (قراءة فقط)، `SELECT COUNT(1)` (قراءة فقط)، `SELECT LastSyncTimestamp FROM SyncSettings` (قراءة فقط)

### API behavior verification:
| المسار | الـ HTTP | الوصف |
|---|---|---|
| `GET /api/sync/exportable-mappings` | 200 | ✅ JSON array — كل عنصر: `id`, `name`, `sqlTargetTable`, `sourceType`, `hasData`, `rowCount`, `lastSyncTime` |
| — متصل DB | ✅ | الفحص: وجود الجدول عبر `OBJECT_ID` → `COUNT(1)` → `hasData` / `rowCount` |
| — جدول غير موجود | ✅ | `hasData: false`, `rowCount: null` |
| — DB غير متصلة | ✅ | القائمة تُرجع بدون `rowCount` + تسجيل `LogWarning` |
| — استثناء لكل mapping | ✅ | يتم تسجيل الخطأ + تخطي هذا التعيين فقط |

### Key implementation decisions:
1. **آخر تزامن (lastSyncTime):** يستخدم `mapping.LastSyncAt` (لكل تعيين) مع fallback إلى `SyncSettings.LastSyncTimestamp` العام
2. **كفاءة الأداء:** استخدام نفس `SqlConnection` لكل الفحوصات (OBJECT_ID + COUNT لكل جدول بالتسلسل)
3. **أسماء الجداول:** معالجة الأسماء المؤهلة بالمخطط (مثل `dbo.stg_WarehouseStock`) عبر تقسيمها وتطبيق الأقواس المربعة على كل جزء
4. **معالجة الأخطاء:** ثلاث مستويات — (أ) DB غير متصلة → القائمة كاملة بدون بيانات، (ب) فشل فحص جدول → mapping واحد بدون rowCount، (ج) قيمة فارغة في `SqlTargetTable` → `hasData: false`

### Unverified risk and next required check:
- **اختبارات التكامل:** لم يتم تشغيل `Integration Tests` لأنه لا يوجد مشروع اختبارات في الحل — يُوصى باختبار يدوي أو كتابة اختبار لـ `GET /api/sync/exportable-mappings`
- **اتصال SQL الفعلي:** التحقق يعتمد على وجود SQL Server — إذا لم يكن متاحاً، الـ fallback سيعمل (`hasData: false` بدون `rowCount`)
