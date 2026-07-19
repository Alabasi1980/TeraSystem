# TASK-DRILL-ADMIN-001 — Phase B: Backend Test Query + Save Contract Fields

| البند | القيمة |
|---|---|
| **المعرف** | TASK-DRILL-ADMIN-001 |
| **المجموعة** | DRILL-DOWN (Phase B) |
| **النوع** | C# Backend |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `project-preparation/DRILL_DOWN_DEVELOPMENT_PLAN.md` v3.0 — §10, §11.2 |

---

## 1. الهدف

إضافة feature اختبار الاستعلام الآمن (Test Query) في صفحة Admin DrillDown، وتحديث handlers الحفظ/التحميل لتشمل الحقول الجديدة (`ParameterColumn`, `LabelColumn`, `RequiresParentValue`) التي أضيفت في Phase A.

---

## 2. التغييرات المطلوبة

### 2.1 تحديث LevelDto ليشمل الحقول الجديدة

في `Index.cshtml.cs`، `LevelDto` يحتاج إضافة:

```csharp
public string? ParameterColumn { get; }
public string? LabelColumn { get; }
public bool RequiresParentValue { get; }

public LevelDto(int id, int level, string displayName, string targetChartType,
    string drillDownQuery, string? parameterColumn, string? labelColumn,
    bool requiresParentValue)
{
    // ... existing properties ...
    ParameterColumn = parameterColumn;
    LabelColumn = labelColumn;
    RequiresParentValue = requiresParentValue;
}
```

### 2.2 تحديث OnGetLevelsAsync ليشمل الحقول الجديدة

```csharp
var levels = await _db.CardDrillDownLevels
    .Where(l => l.ParentCardId == cardId)
    .OrderBy(l => l.Level)
    .Select(l => new LevelDto(
        l.Id, l.Level, l.DisplayName, l.TargetChartType, l.DrillDownQuery,
        l.ParameterColumn, l.LabelColumn, l.RequiresParentValue))
    .ToListAsync();
```

### 2.3 تحديث OnPostSaveAsync ليشمل الحقول الجديدة

أضف parameters جديدة للـ handler:

```csharp
public async Task<IActionResult> OnPostSaveAsync(
    int parentCardId,
    int level,
    string displayName,
    string targetChartType,
    string drillDownQuery,
    int? id,
    string? parameterColumn,
    string? labelColumn,
    bool requiresParentValue)
```

ثم بعد تعيين `entity.DrillDownQuery = drillDownQuery;` أضف:

```csharp
entity.ParameterColumn = parameterColumn?.Trim();
entity.LabelColumn = labelColumn?.Trim();
entity.RequiresParentValue = requiresParentValue;
```

### 2.4 إضافة OnPostTestQueryAsync — اختبار الاستعلام الآمن

أضف handler جديد:

```csharp
/// <summary>
/// POST ?handler=TestQuery
/// Safely executes a Drill Down SQL query for testing purposes.
/// Validates: SELECT/WITH only, @p0 via SqlParameter, max 100 rows,
/// 30s timeout, error sanitization, ParameterColumn/LabelColumn validation.
/// </summary>
public async Task<IActionResult> OnPostTestQueryAsync(
    string drillDownQuery,
    string? parameterColumn,
    string? labelColumn,
    string? testParameterValue)
{
    // 1. Validate query is SELECT or WITH
    var trimmedSql = (drillDownQuery ?? string.Empty).TrimStart();
    if (!trimmedSql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) &&
        !trimmedSql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
    {
        return Json(new
        {
            success = false,
            errorMessage = "يُسمح فقط باستعلامات SELECT أو WITH لأسباب أمنية."
        });
    }

    if (string.IsNullOrWhiteSpace(trimmedSql))
    {
        return Json(new { success = false, errorMessage = "الرجاء إدخال استعلام SQL." });
    }

    // 2. Resolve connection string
    var connTemplate = _config.GetConnectionString("SqlServer") ?? string.Empty;
    var connString = ConnectionStringHelper.Resolve(connTemplate);

    if (string.IsNullOrWhiteSpace(connString))
    {
        return Json(new { success = false, errorMessage = "لم يتم ضبط سلسلة الاتصال بقاعدة البيانات." });
    }

    try
    {
        await using var conn = new SqlConnection(connString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(trimmedSql, conn)
        {
            CommandTimeout = 30 // max 30 seconds for test queries
        };

        // Only @p0 is ever bound, via SqlParameter
        if (trimmedSql.Contains("@p0", StringComparison.OrdinalIgnoreCase))
        {
            cmd.Parameters.Add(new SqlParameter("@p0", SqlParamValue(testParameterValue)));
        }

        await using var reader = await cmd.ExecuteReaderAsync();

        // Build column schema
        var colCount = reader.FieldCount;
        var columns = new List<string>(colCount);
        for (var i = 0; i < colCount; i++)
        {
            columns.Add(reader.GetName(i) is { Length: > 0 } name ? name : $"Column{i + 1}");
        }

        // Validate ParameterColumn exists in result
        var warnings = new List<string>();
        if (!string.IsNullOrWhiteSpace(parameterColumn))
        {
            var found = columns.Exists(c => string.Equals(c, parameterColumn, StringComparison.OrdinalIgnoreCase));
            if (!found)
            {
                warnings.Add($"⚠ عمود الباراميتر '{parameterColumn}' غير موجود في النتيجة. الأعمدة المتاحة: {string.Join(", ", columns)}");
            }
        }

        // Validate LabelColumn exists in result
        if (!string.IsNullOrWhiteSpace(labelColumn))
        {
            var found = columns.Exists(c => string.Equals(c, labelColumn, StringComparison.OrdinalIgnoreCase));
            if (!found)
            {
                warnings.Add($"⚠ عمود التسمية '{labelColumn}' غير موجود في النتيجة.");
            }
        }

        // Read max 100 rows
        var maxRows = 100;
        var rows = new List<Dictionary<string, object?>>(maxRows);
        var count = 0;
        while (count < maxRows && await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>(colCount, StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < colCount; i++)
            {
                row[columns[i]] = ConvertCell(reader.GetValue(i));
            }
            rows.Add(row);
            count++;
        }

        return Json(new
        {
            success = true,
            columns,
            rows,
            rowCount = count,
            warnings = warnings.Count > 0 ? warnings : null
        });
    }
    catch (Exception ex)
    {
        var safeMessage = Sanitize(ex.Message);
        return Json(new { success = false, errorMessage = safeMessage });
    }
}
```

### 2.5 إضافة المراجع المطلوبة

في أعلى الملف، تأكد من وجود:
```csharp
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseDashboard.Web.Infrastructure;
```

### 2.6 إضافة الدوال المساعدة

أضف هذه الدوال في الـ class (بنفس نمط `Drill.cshtml.cs`):

```csharp
private static object SqlParamValue(string? raw)
{
    if (raw is null) return DBNull.Value;
    if (int.TryParse(raw, out var i)) return i;
    if (long.TryParse(raw, out var l)) return l;
    if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
        System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
    if (bool.TryParse(raw, out var b)) return b;
    return raw;
}

private static object? ConvertCell(object value)
{
    if (value is DBNull or null) return null;
    if (value is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm",
        System.Globalization.CultureInfo.InvariantCulture);
    if (value is byte[] bytes) return Convert.ToBase64String(bytes);
    return value;
}

private static string Sanitize(string message)
{
    var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;
    var cleaned = message.Replace("{SQL_PASSWORD}", "***", StringComparison.Ordinal);
    if (password.Length > 0)
        cleaned = cleaned.Replace(password, "***", StringComparison.Ordinal);
    return cleaned;
}
```

### 2.7 حقن IConfiguration

أضف `IConfiguration` إلى الـ Constructor:

```csharp
private readonly IConfiguration _config;

public DrillDownModel(WarehouseDashboardDbContext db, ILogger<DrillDownModel> logger, IConfiguration config)
{
    _db = db;
    _logger = logger;
    _config = config;
}
```

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\DrillDown\Index.cshtml.cs
```

**فقط هذا الملف.** لا تلمس `.cshtml`.

---

## 4. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `LevelDto` محدث بـ `ParameterColumn`, `LabelColumn`, `RequiresParentValue` |
| AC-2 | `OnGetLevelsAsync` يعيد الحقول الجديدة في الـ JSON |
| AC-3 | `OnPostSaveAsync` يقبل ويحفظ `parameterColumn`, `labelColumn`, `requiresParentValue` |
| AC-4 | `OnPostTestQueryAsync` يرفض الاستعلامات غير SELECT/WITH |
| AC-5 | `OnPostTestQueryAsync` يمرر `@p0` عبر `SqlParameter` |
| AC-6 | `OnPostTestQueryAsync` يحدد النتيجة بـ 100 صف كحد أقصى |
| AC-7 | `OnPostTestQueryAsync` يستخدم timeout 30 ثانية |
| AC-8 | `OnPostTestQueryAsync` يعيد الأعمدة والصفوف مع عدد الصفوف |
| AC-9 | `OnPostTestQueryAsync` يتحقق من `ParameterColumn` في النتيجة ويحذر إن لم يكن موجوداً |
| AC-10 | `OnPostTestQueryAsync` يتحقق من `LabelColumn` في النتيجة ويحذر إن لم يكن موجوداً |
| AC-11 | `Sanitize()` يُستخدم لتنظيف رسائل الأخطاء (لا تسريبات) |
| AC-12 | `IConfiguration` محقون في الـ Constructor |
| AC-13 | `dotnet build` — 0 errors, 0 warnings |
| AC-14 | لا secrets في أي ملف |
| AC-15 | Encoding عربي سليم (لا mojibake) |

---

## 5. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ ملف واحد — Backend فقط |
| لا تغيير في قاعدة البيانات | ✅ |
| API آمن | ✅ SELECT/WITH only, SqlParameter, sanitize, timeout |
| Build متوقع | ✅ 0 errors, 0 warnings |

**Gate Status:** ✅ PASS

---

## 6. ملاحظات للوكيل المنفذ

1. **اقرأ الملف قبل تعديله** — `Index.cshtml.cs` يحتوي على 228 سطراً حالياً.
2. استخدم الـ `SqlParamValue` و `ConvertCell` و `Sanitize` من `Drill.cshtml.cs` كنموذج — لكن اكتبها في هذا الملف (لا تعيد استخدام دوال من ملف آخر).
3. احتفظ بكل الكود الموجود — أنت تضيف فقط.
4. لا تلمس `.cshtml`.
5. بعد التعديل، شغّل `dotnet build` للتأكد من 0 errors.

---

**End of TASK-DRILL-ADMIN-001**
