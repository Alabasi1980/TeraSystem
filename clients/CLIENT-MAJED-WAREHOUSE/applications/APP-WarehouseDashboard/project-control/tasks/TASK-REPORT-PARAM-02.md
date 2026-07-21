# TASK-REPORT-PARAM-02

## العنوان
إضافة `GetParameterOptionsAsync` إلى `ReportService` + API endpoint للباراميترات

## الهدف
إنشاء دالة في `ReportService` تنفذ `OptionsQuery` المخزّنة في `ReportFilter` وتجلب خيارات الباراميتر (قيمة/نص).
ثم إنشاء API endpoint لاستدعائها.

## الفكرة
عندما ينشئ المستخدم تقريراً، يكتب لكل فلتر من نوع Dropdown Query مثل:
```sql
SELECT WarehouseID AS Value, WarehouseName AS Text FROM Warehouses
```
حيث `ValueColumn = "Value"` و `TextColumn = "Text"`.

نحتاج دالة تنفذ هذه الـ Query وتعيد النتائج كـ `List<ParameterOption>` (مع Value و Text).

## الملفات المطلوب تغييرها

### 1. Service — `ReportService.cs`

**المسار الكامل:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Services\ReportService.cs
```

**التغيير المطلوب:**

#### أ. أضف DTO جديد بعد `ReportLayoutSaveRequest` (آخر الملف):

```csharp
/// <summary>Option for a parameter dropdown with value and display text.</summary>
public class ParameterOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
```

#### ب. أضف دالة جديدة في SECTION 4 (بعد `GetFilterOptionsAsync`):

```csharp
/// <summary>
/// Executes a filter's OptionsQuery and returns Value/Text pairs.
/// Each row of the query result is mapped using ValueColumn and TextColumn.
/// </summary>
public async Task<List<ParameterOption>> GetParameterOptionsAsync(
    int reportId,
    int filterId,
    CancellationToken ct = default)
{
    var results = new List<ParameterOption>();

    try
    {
        // 1. Load the filter from DB
        var filter = await _db.ReportFilters
            .FirstOrDefaultAsync(f => f.Id == filterId && f.ReportId == reportId, ct);

        if (filter is null || string.IsNullOrWhiteSpace(filter.OptionsQuery))
            return results;

        if (string.IsNullOrWhiteSpace(filter.ValueColumn) || string.IsNullOrWhiteSpace(filter.TextColumn))
            return results;

        // 2. Execute the OptionsQuery
        var connectionString = GetConnectionString();
        if (connectionString is null) return results;

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync(ct);

        await using var cmd = new SqlCommand(filter.OptionsQuery, conn);
        cmd.CommandTimeout = 30;

        await using var reader = await cmd.ExecuteReaderAsync(ct);

        // 3. Map results using ValueColumn and TextColumn
        while (await reader.ReadAsync(ct))
        {
            var value = reader[filter.ValueColumn]?.ToString();
            var text = reader[filter.TextColumn]?.ToString();

            if (value is not null)
            {
                results.Add(new ParameterOption
                {
                    Value = value,
                    Text = text ?? value
                });
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to get parameter options for filter {FilterId} in report {ReportId}.", filterId, reportId);
    }

    return results;
}
```

### 2. API — `ReportData.cshtml.cs`

**المسار الكامل:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Reports\ReportData.cshtml.cs
```

**التغيير المطلوب:**

أضف endpoint جديد بعد `OnGetOptionsAsync` (أو في نهاية الـ API endpoints، قبل request DTOs):

```csharp
/// <summary>
/// GET /api/reports-data/parameterOptions?reportId=1&filterId=2
/// Returns parameter options (Value/Text pairs) for a filter's OptionsQuery.
/// </summary>
public async Task<IActionResult> OnGetParameterOptionsAsync(
    [FromQuery] int reportId,
    [FromQuery] int filterId,
    CancellationToken ct)
{
    // Admin session check — skip if AdminAuth:Bypass is enabled
    var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    if (!config.GetValue<bool>("AdminAuth:Bypass") &&
        HttpContext.Session.GetString("AdminAuthenticated") != "true")
    {
        return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
    }

    if (reportId <= 0 || filterId <= 0)
        return new JsonResult(new { error = "ReportId and FilterId are required." }) { StatusCode = 400 };

    var options = await _reportService.GetParameterOptionsAsync(reportId, filterId, ct);
    return new JsonResult(options);
}
```

### 3. DTO في بداية ملف الـ API (اختياري)

إذا أردت، يمكنك إضافة `using WarehouseDashboard.Web.Services;` — لكنها موجودة بالفعل في الملف.

## Allowed Write Targets
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Services/ReportService.cs`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/Api/Reports/ReportData.cshtml.cs`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/project-control/tasks/TASK-REPORT-PARAM-02.md`

## معايير القبول
1. `ReportService.cs` يحتوي على دالة `GetParameterOptionsAsync(int reportId, int filterId, CancellationToken ct)`
2. الدالة تقرأ `OptionsQuery` و `ValueColumn` و `TextColumn` من الـ Filter
3. الدالة تنفذ الـ Query باستخدام ADO.NET وتعيد `List<ParameterOption>`
4. DTO `ParameterOption` موجود (public class with Value + Text)
5. API endpoint `GET /api/reports-data/parameterOptions?reportId=&filterId=` موجود
6. الـ endpoint يرجع `JsonResult(options)` بنجاح
7. `dotnet build` يمر بدون أخطاء
8. لا يوجد تعديل لأي ملف خارج `Allowed Write Targets`

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | دالة واحدة + API واحد فقط |
| One objective only | PASS | جلب خيارات الباراميترات عبر Query |
| No deferrable work included | PASS | لا يوجد |
| No UI unless explicitly requested | PASS | لا يوجد UI |
| No API unless explicitly requested | PASS | API مطلوب صراحة |
| No Auth unless explicitly requested | PASS | لا يوجد Auth جديد (نفس نمط الـ Admin bypass) |
| No schema/migration unless explicitly requested | PASS | لا يوجد Schema |
| No real secrets outside approved local environment files | PASS | لا يوجد أسرار |
| CLI side effects checked | PASS | لا توجد أوامر Shell |
| No internal contradiction | PASS | لا يوجد تعارض |
| Allowed Write Targets are narrow | PASS | 3 مسارات محددة |
| Acceptance criteria are testable | PASS | dotnet build + فحص API |

**Gate Status:** PASS
**Required Action:** تنفيذ التغييرات

---

## إرشادات للمهندس

أهلاً. المهمة الثانية — وهي مكملة للأولى.

**ما المطلوب؟**

البارحة أضاف زميلك `ValueColumn` و `TextColumn` إلى الـ Model. اليوم دورك:

1. **في Service:** أضف دالة `GetParameterOptionsAsync` التي:
   - تستلم `reportId` و `filterId`
   - تجلب الـ Filter من قاعدة البيانات
   - تقرأ `OptionsQuery` و `ValueColumn` و `TextColumn`
   - تنفذ الـ Query عبر ADO.NET (SqlConnection, SqlCommand)
   - تقرأ النتائج (Value و Text من الأعمدة المحددة)
   - ترجع `List<ParameterOption>`
   - أضف `ParameterOption` كـ DTO جديد (فقط `Value` و `Text`)

2. **في API (ReportData.cshtml.cs):** أضف endpoint:
   - `GET /api/reports-data/parameterOptions?reportId=X&filterId=Y`
   - يستدعي الدالة الجديدة
   - يرجع JSON

**مهم:** اقرأ ملفاتك جيداً. لاحظ أن ملف `ReportData.cshtml.cs` يحتوي على نمط موحد لكل endpoint — كلها تبدأ بفحص Admin bypass. اتبع نفس النمط. 

لا تنسَ فحص `dotnet build` في النهاية.

بعد الانتهاء، أضف `## Post-Execution Review Result` في نهاية هذا الملف.

---

## Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| `ParameterOption` DTO added in `ReportService.cs` | PASS | بعد `ReportLayoutSaveRequest` بأسفل الملف |
| `GetParameterOptionsAsync` added in SECTION 4 | PASS | بعد `GetFilterOptionsAsync` مباشرة |
| `OnGetParameterOptionsAsync` API endpoint added | PASS | بعد `OnGetOptionsAsync` في `ReportData.cshtml.cs` |
| Admin bypass check followed (same pattern) | PASS | نفس نمط باقي endpoints |
| `dotnet build` — 0 errors, 0 warnings | PASS | Build succeeded |
| No files modified outside Allowed Write Targets | PASS | فقط 3 ملفات ضمن المسارات المسموحة |
| All acceptance criteria met (1–8) | PASS | جميع المعايير مستوفاة |

### Summary
تم تنفيذ المهمة TASK-REPORT-PARAM-02 بنجاح:

1. **ReportService.cs**:
   - أُضيف DTO `ParameterOption` (خصائص `Value` و `Text`) بعد `ReportLayoutSaveRequest`.
   - أُضيفت دالة `GetParameterOptionsAsync(int reportId, int filterId, CancellationToken ct)` في SECTION 4 بعد `GetFilterOptionsAsync`. الدالة تقرأ `OptionsQuery` و `ValueColumn` و `TextColumn` من الفلتر، وتنفذ الاستعلام عبر ADO.NET، وتعيّن النتائج إلى `List<ParameterOption>`.

2. **ReportData.cshtml.cs**:
   - أُضيف endpoint `GET /api/reports-data/parameterOptions?reportId=&filterId=` بعد `OnGetOptionsAsync` بنفس نمط التحقق من الصلاحية (Admin bypass).

3. **Build**: `dotnet build` نجح بدون أخطاء أو تحذيرات.

**Files modified:**
- `Services/ReportService.cs` — DTO + method
- `Pages/Api/Reports/ReportData.cshtml.cs` — API endpoint
- `project-control/tasks/TASK-REPORT-PARAM-02.md` — هذا التقرير
