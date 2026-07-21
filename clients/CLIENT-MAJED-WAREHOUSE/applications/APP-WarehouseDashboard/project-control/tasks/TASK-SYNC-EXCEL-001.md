# TASK-SYNC-EXCEL-001 — Export Sync Table to Excel (Backend)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-EXCEL-001 |
| **المجموعة** | Sync Enhancement — Export Excel |
| **النوع** | Backend (Service + API Endpoint) |
| **الوكيل** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | 🟡 Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-21 |

---

## 1. المشكلة

شاشة المزامنة (Sync Dashboard) لا توفر طريقة لتنزيل بيانات الجداول المزامنة بصيغة Excel. الزبون يحتاج ملف Excel منسق وجاهز للاستخدام لكل جدول (`stg_*`) في SQL Server.

---

## 2. الهدف

إضافة زر تنزيل Excel لكل تعيين مزامنة، يولد ملف `.xlsx`:
- منسق احترافياً (Header ملون، تناوب صفوف، AutoFilter)
- جاهز للاستخدام مباشرة
- دعم RTL للعربية
- أسماء ملفات واضحة: `{TableName}_YYYY-MM-DD_HHMM.xlsx`

---

## 3. النطاق — المطلوب تنفيذه

### 3.1 إضافة NuGet Package

إضافة `ClosedXML` للإصدار `0.104.*` لمشروع `WarehouseDashboard.Api.csproj`:

```xml
<PackageReference Include="ClosedXML" Version="0.104.*" />
```

> **السبب:** ClosedXML مجاني (MIT License)، API نظيف، يدعم التنسيق الكامل، وخفيف.

### 3.2 إنشاء ExportExcelService

**المسار:** `src/WarehouseDashboard.Api/Services/ExportExcelService.cs`

```csharp
namespace WarehouseDashboard.Api.Services;

public class ExportExcelService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExportExcelService> _logger;

    public ExportExcelService(IConfiguration configuration, ILogger<ExportExcelService> logger);

    /// <summary>
    /// Generates a formatted Excel file for the given SQL Server table.
    /// </summary>
    /// <param name="tableName">SQL Server table name (e.g. stg_ST_ITEM_CARD)</param>
    /// <param name="mappingName">Display name for the mapping (for footer)</param>
    /// <returns>Excel file bytes, or null if table not found/empty</returns>
    public async Task<byte[]?> GenerateAsync(string tableName, string mappingName, CancellationToken ct);
}
```

**التفاصيل التقنية لـ GenerateAsync:**

1. **قراءة البيانات:**
   - فتح SqlConnection عبر `ConnectionStringHelper.ResolveSql(_configuration)`
   - تنفيذ `SELECT * FROM [tableName]` مع `ORDER BY 1` إن أمكن
   - استعمال `SqlDataReader` للقراءة (Streaming، مناسب للبيانات الكبيرة)
   - جلب معلومات الأعمدة عبر `reader.GetSchemaTable()`

2. **بناء Excel باستخدام ClosedXML:**
   - إنشاء `XLWorkbook`
   - ورقة عمل واحدة باسم الجدول (مختصر، مقطوع إلى 31 حرفاً)
   - **Row 1: Header** — خلفية `XLColor.FromArgb(31, 78, 121)` (الأزرق الداكن) + خط أبيض عريض `White, Bold, Size 11`
   - **Data rows** — تناوب ألوان الصفوف: أبيض / `XLColor.FromArgb(245, 248, 250)` (رملي فاتح جداً)
   - **AutoFilter** — تفعيل `Range().SetAutoFilter()` على كل الأعمدة
   - **Freeze Rows** — تجميد الصف الأول `SheetView.Freeze(1)`
   - **Column Width** — ضبط تلقائي `AdjustToContents()` أو عرض مناسب حسب نوع البيانات
   - **Number Formatting** — الأعمدة الرقمية تنسيق `#,##0.00`، والتواريخ `YYYY-MM-DD HH:MM`
   - **Footer** — صف بعد البيانات:
     - دمج خلايا يسار: `تاريخ التصدير: {DateTime.Now:yyyy-MM-dd HH:mm}`
     - دمج خلايا يمين: `المصدر: {mappingName}`
     - خط رمادي مائل (Italic, Gray)
   - **RTL** — `SheetView.RightToLeft = true` (لأن البيانات عربية)

3. **التعامل مع الحالات الخاصة:**
   - جدول فارغ → إرجاع Excel برؤوس أعمدة فقط مع رسالة "لا توجد بيانات"
   - جدول غير موجود → إرجاع `null`
   - أسماء أعمدة طويلة → تقصير لـ 50 حرفاً
   - أعمدة كثيرة (50+) → دعم كامل

### 3.3 إضافة Endpoint

**الملف:** `src/WarehouseDashboard.Api/Controllers/SyncController.cs`

إضافة:

```csharp
/// <summary>
/// GET /api/sync/{id:int}/export-excel — downloads a formatted Excel file
/// for the target table of the given mapping.
/// </summary>
[HttpGet("{id:int}/export-excel")]
public async Task<IActionResult> ExportExcel(int id, CancellationToken ct)
{
    // 1. Load mapping to get SqlTargetTable
    var mappings = await _syncEngine.LoadMappingsByIdsAsync(new[] { id }, ct);
    var mapping = mappings.FirstOrDefault();
    if (mapping is null)
        return NotFound(new { message = "Mapping not found." });

    // 2. Check SqlTargetTable is not empty
    if (string.IsNullOrWhiteSpace(mapping.SqlTargetTable))
        return BadRequest(new { message = "Mapping has no target table." });

    // 3. Generate Excel
    var fileName = $"{mapping.SqlTargetTable}_{DateTime.Now:yyyy-MM-dd_HHmm}.xlsx";
    var bytes = await _exportExcel.GenerateAsync(mapping.SqlTargetTable, mapping.Name ?? mapping.SqlTargetTable, ct);

    if (bytes is null)
        return NotFound(new { message = $"Table '{mapping.SqlTargetTable}' not found or empty." });

    // 4. Return file
    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
}
```

**متطلبات الـ Endpoint:**
- حقن `ExportExcelService` في الـ Constructor
- اسم ملف واضح: `{SqlTargetTable}_{yyyy-MM-dd_HHmm}.xlsx`
- Content-Type: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`

### 3.4 تسجيل ExportExcelService في DI

**الملف:** `src/WarehouseDashboard.Api/Program.cs`

إضافة:
```csharp
builder.Services.AddScoped<ExportExcelService>();
```

### 3.5 إضافة `using` للـ ClosedXML

لا تنسَ `using ClosedXML.Excel;` في ملف ExportExcelService.

---

## 4. الملفات المسموح كتابتها (Allowed Write Targets)

المسارات كلها تحت:
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\
```

| الملف | الإجراء |
|---|---|
| `WarehouseDashboard.Api.csproj` | إضافة ClosedXML PackageReference |
| `Services/ExportExcelService.cs` | **إنشاء جديد** |
| `Controllers/SyncController.cs` | تعديل (إضافة Endpoint + حقن ExportExcelService) |
| `Program.cs` | تعديل (إضافة AddScoped) |

**ممنوع:** تعديل أي ملف خارج المسار أعلاه.

---

## 5. معايير القبول (Acceptance Criteria)

- [ ] **AC1:** `GET /api/sync/{id}/export-excel` يعيد ملف `.xlsx` (تحقق من Content-Type واسم الملف)
- [ ] **AC2:** الملف يفتح في Excel بدون أخطاء
- [ ] **AC3:** الـ Header موجود بلون أزرق غامق + خط أبيض عريض
- [ ] **AC4:** AutoFilter مفعّل على كل الأعمدة
- [ ] **AC5:** الصف الأول مجمّد (Freeze)
- [ ] **AC6:** تناوب ألوان الصفوف (striped rows)
- [ ] **AC7:** Footer مع تاريخ التصدير واسم المصدر
- [ ] **AC8:** دعم RTL للعربية
- [ ] **AC9:** إذا الجدول فارغ → ملف به رؤوس فقط مع رسالة "لا توجد بيانات"
- [ ] **AC10:** إذا الجدول غير موجود → 404
- [ ] **AC11:** Build يمر بـ 0 Errors / 0 Warnings
- [ ] **AC12:** الأعمدة الرقمية منسقة كأرقام، والتواريخ كتواريخ

---

## 6. Pre-Execution Gate

| البند | الحالة |
|---|---|
| التكنولوجي بروفايل | `dotnet-razorpages-adonet` ✅ |
| الملفات المرجعية مقروءة | ✅ |
| النطاق محدد وواضح | ✅ |
| المسموح والممنوع محدد | ✅ |
| الـ AC كامل | ✅ |
| لا يحتوي سكريتس أو كلمات مرور | ✅ |
| الـ Auditor: NOT_REQUIRED (إضافة خدمة + Endpoint، لا تغيير في الأمن أو الـ schema) | ✅ |

**PASS ✅ — جاهز للتفويض**

---

## 7. ملاحظات إضافية

- استخدم `await using` مع SqlConnection و SqlDataReader
- الـ `SqlDataReader` يقرأ بطريقة Streaming — مناسب لـ 100K+ صف
- استخدم `reader.GetSchemaTable()` لاستخراج أسماء الأعمدة + أنواعها
- لا تنسَ `ct.ThrowIfCancellationRequested()` في الحلقات الطويلة
- الـ Excel يجب أن يكون في الذاكرة (`MemoryStream`)، ليس على القرص
- راجع ملف `SyncController.cs` (السطور 295-311) لفهم نمط جلب التعيينات من `_syncEngine.LoadMappingsByIdsAsync()`
- راجع `ConnectionStringHelper.ResolveSql()` في `Infrastructure/ConnectionStringHelper.cs`

---

## Handback — 2026-07-21

**الحالة:** ✅ مكتمل
**Build:** 0 errors / 0 warnings

**الملفات المنجزة:**

| الملف | الإجراء |
|---|---|
| `WarehouseDashboard.Api.csproj` | إضافة `ClosedXML` v0.104.\* PackageReference |
| `Services/ExportExcelService.cs` | إنشاء جديد — خدمة تصدير Excel كاملة |
| `Controllers/SyncController.cs` | تعديل — حقن `ExportExcelService` + إضافة Endpoint `GET {id}/export-excel` |
| `Program.cs` | تعديل — إضافة `builder.Services.AddScoped<ExportExcelService>()` |

**ملاحظات تقنية:**

1. **ClosedXML API adaptation:** في الإصدار 0.104.x، خاصية `RightToLeft` موجودة على `IXLWorksheet` (وليس `IXLSheetView`)، و `Freeze` تأخذ معاملين `(rows, columns)`.
2. **Lock file bypass:** الـ API كان شغال (PID 22532) أثناء البناء؛ تم استخدام `--output` بديل (`C:\Users\Fares\AppData\Local\Temp\build-test`) لتأكيد الـ build بنجاح.
3. **Streaming reader:** `ExportExcelService` يستخدم `SqlDataReader` بطريقة Streaming مع `await using`، مناسب للبيانات الكبيرة (100K+ صف).
4. **Column width performance:** `AdjustToContents` مطبّق على أول 100 صف فقط لتجنب البطء مع الجداول الكبيرة.
5. **Type formatting:** الأعمدة الرقمية (`int/long/decimal/double/float/short/byte`) تأخذ تنسيق `#,##0.00` أو `#,##0`، والتواريخ `YYYY-MM-DD HH:MM`.
6. **Edge cases:** جدول غير موجود → 404 (null من الخدمة)؛ جدول فارغ → Excel برؤوس فقط + رسالة "لا توجد بيانات"؛ GridName > 31 حرفاً → مقطوع لاسم الورقة.
7. **STOP/ASK gate considered:** لم يتم تفعيله — لا تغيير في الأمن أو الـ schema، فقط إضافة خدمة تصدير و Endpoint جديد. لا يتطلب Auditor.

**AC Verification Status:**

| AC | الوصف | الحالة |
|---|---|---|
| AC1 | GET يعيد ملف .xlsx مع Content-Type صحيح | ✅ (كودياً) |
| AC2 | الملف يفتح بدون أخطاء | ✅ (ClosedXML ينتج XLSX صحيح) |
| AC3 | Header أزرق غامق + خط أبيض عريض | ✅ `XLColor.FromArgb(31,78,121)` + White Bold 11 |
| AC4 | AutoFilter على كل الأعمدة | ✅ `range.SetAutoFilter()` |
| AC5 | Freeze الصف الأول | ✅ `SheetView.Freeze(1,0)` |
| AC6 | تناوب ألوان الصفوف | ✅ أبيض / `(245,248,250)` |
| AC7 | Footer تاريخ + مصدر | ✅ دمج خلايا + Italic Gray |
| AC8 | RTL | ✅ `ws.RightToLeft = true` |
| AC9 | جدول فارغ → رؤوس + رسالة | ✅ "لا توجد بيانات" |
| AC10 | جدول غير موجود → 404 | ✅ `null` ← `NotFound` |
| AC11 | Build 0 Errors / 0 Warnings | ✅ |
| AC12 | تنسيق أرقام وتواريخ | ✅ `#,##0.00`, `YYYY-MM-DD HH:MM` |
