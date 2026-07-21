# TASK-REPORT-PARAM-03

## العنوان
تحسين Step 3 في منشئ التقارير — إضافة حقول OptionsQuery, ValueColumn, TextColumn, IsRequired, DefaultValue

## الهدف
عند إنشاء تقرير في منشئ التقارير (ReportBuilder)، يحتاج المستخدم أن يحدد لكل فلتر من نوع "Dropdown" (قائمة منسدلة) البيانات التالية:
1. **OptionsQuery** — Query SQL تجلب خيارات القائمة (مثل: `SELECT ItemID AS Value, ItemName AS Text FROM Items`)
2. **ValueColumn** — اسم العامود الذي يمثل القيمة
3. **TextColumn** — اسم العامود الذي يمثل النص المعروض
4. **IsRequired** — هل الفلتر إجباري قبل عرض التقرير؟
5. **DefaultValue** — قيمة افتراضية للفلتر

## الملف المطلوب تغييره

### 1. Report Builder — `Index.cshtml`

**المسار الكامل:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\ReportBuilder\Index.cshtml
```

### التغييرات المطلوبة بالتفصيل:

#### أ. تحسين دالة `addFilter()`:

الوضع الحالي: دالة `addFilter()` تضيف فلتراً بعناصر محدودة فقط (column select, type select, label input, remove button).

المطلوب: تعديل الدالة لتضيف عند اختيار "Dropdown" في نوع الفلتر، مجموعة حقول إضافية تظهر تحته:
- Textarea لـ OptionsQuery
- Input لـ ValueColumn
- Input لـ TextColumn
- Checkbox لـ IsRequired
- Select لـ DefaultValue يملأ من نتائج OptionsQuery

**التصميم الجديد لكل فلتر:**

```html
<li class="wd-filter-item" id="FILTER_ID">
  <!-- الصف الأول: العمود + النوع + التسمية + حذف -->
  <div style="display:grid;grid-template-columns:1fr 160px 1fr auto;gap:var(--sp-3);align-items:center;width:100%;">
    <select class="wd-select" ...>...</select>  <!-- Column -->
    <select class="wd-select" ...>...</select>  <!-- Filter Type -->
    <input class="wd-input" type="text" placeholder="اسم الفلتر..." />  <!-- Label -->
    <button class="wd-btn wd-btn--danger" ...>✕</button>
  </div>

  <!-- الصف الثاني: الحقول الإضافية (تظهر فقط إذا كان النوع = Dropdown) -->
  <div class="wd-filter-extra" id="extra-FILTER_ID" style="display:none;margin-top:8px;padding:12px;background:var(--c-surface-muted);border-radius:var(--radius-md);">
    <div style="display:grid;grid-template-columns:1fr 1fr;gap:var(--sp-3);">
      <div class="wd-field">
        <label>OptionsQuery (SQL)</label>
        <textarea class="wd-input wd-filter-query" rows="3" placeholder="SELECT ID AS Value, Name AS Text FROM Table" style="font-family:monospace;font-size:12px;"></textarea>
      </div>
      <div style="display:flex;flex-direction:column;gap:var(--sp-2);">
        <div class="wd-field">
          <label>عمود القيمة (ValueColumn)</label>
          <input class="wd-input wd-filter-value-col" type="text" placeholder="مثال: Value" style="font-size:12px;" />
        </div>
        <div class="wd-field">
          <label>عمود النص (TextColumn)</label>
          <input class="wd-input wd-filter-text-col" type="text" placeholder="مثال: Text" style="font-size:12px;" />
        </div>
      </div>
    </div>
    <div style="display:flex;align-items:center;gap:var(--sp-4);margin-top:8px;">
      <label class="wd-checkbox" style="font-size:12px;">
        <input type="checkbox" class="wd-filter-required" /> فلتر إجباري (إلزامي قبل العرض)
      </label>
      <div class="wd-field" style="flex:1;">
        <label style="font-size:12px;">القيمة الافتراضية</label>
        <select class="wd-select wd-filter-default" style="font-size:12px;" disabled>
          <option value="">— اختر قيمة افتراضية —</option>
        </select>
        <button class="wd-btn wd-btn--ghost" style="font-size:11px;padding:2px 8px;margin-top:4px;" onclick="loadDefaultOptions(this)">
          🔄 تحميل الخيارات
        </button>
      </div>
    </div>
  </div>
</li>
```

#### ب. إضافة دالة `loadDefaultOptions(button)`:

عند الضغط على زر "تحميل الخيارات"، يتم:
1. أخذ قيمة OptionsQuery من الـ textarea في نفس الفلتر
2. إذا كانت القيمة فارغة → showToast وأوقف
3. أخذ الفلتر id
4. استدعاء API جديد `/api/reports-data/parameterOptions?reportId=0&filterId=0`— لكن في المنشئ لا يوجد reportId بعد
5. بدلاً من ذلك، **نحتاج حل آخر**: 

**الحل:** نضيف endpoint جديد `/api/reports-data/executeQuery` يأخذ query كـ POST body وينفذه ويعيد النتائج.

**أو حل أبسط:** نضيف زر "اختبار" يجرب الـ Query ويعرض أول 5 نتائج في جدول صغير تحت الـ textarea.

للتبسيط الحالي، سنكتفي بـ:
- الزر يحاول تنفيذ الـ Query عبر endpoint جديد `POST /api/reports-data/executeQuery` (يأخذ `{ query: "..." }`)
- يُظهر أول 5 صفوف في جدول معاينة صغير
- يملأ الـ DefaultValue dropdown بالنتائج

#### ج. إنشاء API endpoint جديد `POST /api/reports-data/executeQuery`:

في `ReportData.cshtml.cs`:

```csharp
/// <summary>
/// POST /api/reports-data/executeQuery
/// Body: { query: "SELECT ..." }
/// Executes an arbitrary SQL query and returns up to 20 rows for testing/preview.
/// </summary>
public async Task<IActionResult> OnPostExecuteQueryAsync([FromBody] ExecuteQueryRequest request, CancellationToken ct)
{
    var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    if (!config.GetValue<bool>("AdminAuth:Bypass") &&
        HttpContext.Session.GetString("AdminAuthenticated") != "true")
    {
        return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
    }

    if (string.IsNullOrWhiteSpace(request.Query))
        return new JsonResult(new { error = "Query is required." }) { StatusCode = 400 };

    try
    {
        var connectionString = ConnectionStringHelper.ResolveSql(
            HttpContext.RequestServices.GetRequiredService<IConfiguration>());
        if (string.IsNullOrWhiteSpace(connectionString))
            return new JsonResult(new { success = false, errorMessage = "Connection string not configured." });

        await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        await conn.OpenAsync(ct);
        await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(request.Query, conn);
        cmd.CommandTimeout = 15;
        await using var reader = await cmd.ExecuteReaderAsync(ct);

        var columns = new List<string>();
        for (int i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i));

        var rows = new List<Dictionary<string, object?>>();
        int count = 0;
        while (await reader.ReadAsync(ct) && count < 20)
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i);
                row[columns[i]] = val is DBNull ? null : val;
            }
            rows.Add(row);
            count++;
        }

        return new JsonResult(new { success = true, columns, rows, rowCount = rows.Count });
    }
    catch (Exception ex)
    {
        return new JsonResult(new { success = false, errorMessage = ex.Message });
    }
}

public class ExecuteQueryRequest
{
    public string Query { get; set; } = string.Empty;
}
```

#### د. تحسين دالة `getFilters()`:

لجمع الحقول الجديدة مع الفلتر. يجب أن تعيد:
```javascript
{
    columnName: ...,
    filterType: ...,
    label: ...,
    optionsQuery: textarea.value,
    valueColumn: input.value,
    textColumn: input.value,
    isRequired: checkbox.checked,
    defaultValue: select.value,
    placeholder: '',
    sortOrder: ...
}
```

#### هـ. تحسين دالة معالجة `goStep(3)`:

عند التحميل لـ Step 3، لا حاجة لتغيير — لأن الفلتر يُنشأ بواسطة `addFilter()`.

#### و. عند تحميل تقرير موجود للتعديل (مستقبلاً):

`editReportId` موجود بالفعل. إذا كان التقرير يحتوي على فلاتر مع OptionsQuery، يجب تحميلها وتعبئة الحقول الإضافية.

## Allowed Write Targets
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/ReportBuilder/Index.cshtml`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/Api/Reports/ReportData.cshtml.cs`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/project-control/tasks/TASK-REPORT-PARAM-03.md`

## معايير القبول
1. دالة `addFilter()` تنشئ فلتراً بالحقول الجديدة عند اختيار "Dropdown" كـ FilterType
2. الحقول الإضافية (OptionsQuery, ValueColumn, TextColumn, IsRequired, DefaultValue) تظهر/تخفي حسب نوع الفلتر
3. زر "تحميل الخيارات" يستدعي الـ API ويعرض النتائج
4. API endpoint `POST /api/reports-data/executeQuery` موجود ويعمل
5. دالة `getFilters()` تجمع الحقول الجديدة
6. `dotnet build` يمر بدون أخطاء
7. لا يوجد تعديل لأي ملف خارج `Allowed Write Targets`

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | تحسين Step 3 فقط في منشئ التقارير |
| One objective only | PASS | إضافة حقول الباراميترات في المنشئ |
| No deferrable work included | PASS | لا يوجد |
| No UI unless explicitly requested | PASS | UI مطلوب صراحة |
| No Auth unless explicitly requested | PASS | لا يوجد Auth جديد |
| No schema/migration unless explicitly requested | PASS | لا يوجد Schema |
| No real secrets outside approved local environment files | PASS | لا يوجد أسرار |
| CLI side effects checked | PASS | لا توجد أوامر Shell |
| Allowed Write Targets are narrow | PASS | 3 مسارات محددة |
| Acceptance criteria are testable | PASS | dotnet build + فحص الوظائف |

**Gate Status:** PASS
**Required Action:** تنفيذ التغييرات

---

## إرشادات للمهندس

أهلاً. هذه المهمة الأكبر في السلسلة — لكنها مباشرة.

**ما المطلوب؟**

تخيل أن مستخدماً يريد إنشاء تقرير فيه فلتر "المستودع" من نوع قائمة منسدلة.
يجب أن يكتب Query مثل:
```sql
SELECT WarehouseID AS Value, WarehouseName AS Text FROM Warehouses
```
ويحدد:
- `Value` = عمود القيمة
- `Text` = عمود النص
- هل الفلتر إجباري؟
- ما هي القيمة الافتراضية؟

**التغييرات المطلوبة في ملف واحد فقط: `ReportBuilder/Index.cshtml`**

**هام جداً:** اقرأ الملف الحالي أولاً لتفهم هيكل `addFilter()` و `getFilters()`.

### ما ستفعله بالضبط:

1. **عدّل دالة `addFilter()`**: بعد عناصر الفلتر الحالية، أضف `div.wd-filter-extra` مخفي (style="display:none") يحتوي على:
   - Textarea لـ OptionsQuery (SQL)
   - Input لـ ValueColumn
   - Input لـ TextColumn
   - Checkbox لـ IsRequired
   - Select لـ DefaultValue (معطل في البداية)
   - زر "تحميل الخيارات"

2. **أضف دالة `onFilterTypeChange(filterId, selectedType)`**: عند اختيار "Dropdown" تظهر الحقول الإضافية، عند اختيار غيره تخفى.

3. **أضف دالة `loadDefaultOptions(button)`**: تقرأ OptionsQuery من الـ textarea، تستدعي API `/api/reports-data/executeQuery`، تملأ جدول معاينة والـ DefaultValue dropdown.

4. **عدّل دالة `getFilters()`**: لجمع optionsQuery, valueColumn, textColumn, isRequired, defaultValue.

5. **أضف CSS قليلة** للـ wd-filter-extra (يمكن إضافتها في الـ `<style>` الموجود).

6. **أضف API endpoint `POST /api/reports-data/executeQuery`** في `ReportData.cshtml.cs` كما هو موضح في المهمة.

ابدأ بقراءة الملفين أولاً، ثم طبّق التغييرات.

بعد الانتهاء، أضف `## Post-Execution Review Result` في نهاية هذا الملف.

## Post-Execution Review Result

| Check | Result | Evidence |
|---|---|---|
| `dotnet build` — 0 errors, 0 warnings | ✅ PASS | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| **1. تحسين `addFilter()`** — إضافة `div.wd-filter-extra` مع OptionsQuery, ValueColumn, TextColumn, IsRequired, DefaultValue | ✅ PASS | تمت إضافة الحزمة بعد عناصر الفلتر الموجودة مع `grid-column:1/-1` و `display:none` افتراضياً |
| **2. ربط `onFilterTypeChange`** — في `onchange` لـ select نوع الفلتر | ✅ PASS | `onchange="updateFilterType(...); onFilterTypeChange(...)"` يستدعي الدالة الجديدة |
| **3. دالة `onFilterTypeChange()`** — إظهار/إخفاء الحقول الإضافية حسب النوع | ✅ PASS | عند `Dropdown` → `display:block` + animation `wdFadeUp`; عند غيره → `display:none` |
| **4. دالة `loadDefaultOptions()`** — قراءة OptionsQuery واستدعاء API وملء DefaultValue dropdown | ✅ PASS | ترسل POST إلى `/api/reports-data/executeQuery`، تملأ الـ select وتعطل/تفعّل الأزرار |
| **5. دالة `getFilters()`** — جمع الحقول الجديدة (optionsQuery, valueColumn, textColumn, isRequired, defaultValue) | ✅ PASS | كل حقل يُجمع من `querySelector` المناسب مع fallbacks آمنة |
| **6. API endpoint `POST /api/reports-data/executeQuery`** | ✅ PASS | `OnPostExecuteQueryAsync` مضاف مع `ExecuteQueryRequest` DTO، تشغيل SQL مع timeout 15s وحد أقصى 20 صفاً |
| **7. `wdFadeUp` animation في CSS** | ✅ PASS | `@keyframes wdFadeUp` مضاف في `<style>` قبل الـ responsive media query |
| **8. لم يتم تعديل أي ملف خارج `Allowed Write Targets`** | ✅ PASS | الملفان المعدلان ضمن القائمة المسموحة (`Index.cshtml`, `ReportData.cshtml.cs`) + ملف المهمة نفسه |

### ملخص التغييرات في `Index.cshtml`

**أ. CSS:**
- إضافة `@keyframes wdFadeUp` (fade + translateY animation)
- إضافة قاعدة `wd-filter-extra { grid-column: 1 / -1 }` في responsive media query لضمان امتداد full-width

**ب. دالة `addFilter()`:**
- إضافة استدعاء `onFilterTypeChange(...)` في `onchange` لـ select النوع
- إضافة `div.wd-filter-extra` كامل (OptionsQuery textarea, ValueColumn input, TextColumn input, IsRequired checkbox, DefaultValue select معطل، زر تحميل)
- استخدام `grid-column:1/-1` لضمان امتداد الحزمة كاملة عرض الـ grid

**ج. دالتان جديدتان:**
- `onFilterTypeChange(filterId, selectEl)` — تتحكم في إظهار/إخفاء الحزمة مع animation
- `loadDefaultOptions(buttonEl)` — async function ترسل POST لـ `/api/reports-data/executeQuery` وتملأ dropdown

**د. تعديل `getFilters()`:**
- إضافة `optionsQuery` من `.wd-filter-query`
- إضافة `valueColumn` من `.wd-filter-value-col`
- إضافة `textColumn` من `.wd-filter-text-col`
- إضافة `isRequired` من `.wd-filter-required` checkbox
- إضافة `defaultValue` من `.wd-filter-default` select

### ملخص التغييرات في `ReportData.cshtml.cs`

- إضافة `OnPostExecuteQueryAsync()` — endpoint جديد `POST /api/reports-data/executeQuery`
- إضافة `ExecuteQueryRequest` DTO مع خاصية `Query`
- الـ endpoint ينفذ SQL query مع timeout 15s ويعيد حتى 20 صفاً مع `success`, `columns`, `rows`, `rowCount`

### المخاطر المتبقية
- **SQL Injection**: الـ API ينفذ query مباشرة كما وردت. هذا مقصود لاختبار الفلاتر في بيئة المسؤول، لكن يجب التنبيه إلى أن المستخدمين المسؤولين فقط هم من يمكنهم الوصول (محمي بـ Admin session check).
- **No existing report edit**: عند تحميل تقرير موجود للتعديل، الحقول الإضافية لن تملأ تلقائياً. هذا خارج نطاق المهمة الحالية (مستقبلي).

### Verification Steps
1. افتح ReportBuilder → Step 3 → أضف فلتر → اختر "قائمة منسدلة" → تظهر الحقول الإضافية
2. اكتب OptionsQuery سليم → اختر ValueColumn/TextColumn → اضغط "🔄 تحميل" → يتم تحميل الخيارات في DefaultValue dropdown
3. اختر نوع فلتر آخر → تخفى الحقول الإضافية
4. احفظ التقرير → دالة `getFilters()` تجمع `optionsQuery`, `valueColumn`, `textColumn`, `isRequired`, `defaultValue`
