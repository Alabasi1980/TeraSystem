# TASK-COD-027 — Fix Builder Page HTTP 500 (IHttpClientFactory not registered)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-027 |
| **المجموعة** | FIX — Card Builder follow-up |
| **المرحلة** | Phase 6 — Implementation (Bug Fix) |
| **الوكيل** | engineering-agent |
| **التبعية** | TASK-COD-026 |
| **الأولوية** | Critical (الصفحة غير قابلة للوصول للمستخدم المسجّل) |
| **الحالة** | 🟢 Accepted (build 0 errors, IHttpClientFactory removed) |

---

## 1. الهدف
إصلاح خطأ HTTP 500 عند فتح `/admin-secure-panel/Cards/Builder` من قِبَل مستخدم مسجّل الدخول.

## 2. السبب الجذري (مؤكد بتحليل TeraAgent)
- `Pages/admin-secure-panel/Cards/Builder.cshtml.cs` يحقن `IHttpClientFactory` عبر منشئ `BuilderModel`.
- `Program.cs` **لا يستدعي `builder.Services.AddHttpClient(...)` إطلاقاً** (تحقق عبر grep — لا يوجد أي تسجيل `AddHttpClient` في المشروع).
- بالتالي عند طلب مُصادَق عليه (جلسة صالحة) يتم تفعيل `BuilderModel` → فشل حقن `IHttpClientFactory` → `InvalidOperationException` → **HTTP 500**.
- عند طلب غير مُصادَق عليه، الـ `AdminAuthMiddleware` يُعيد التوجيه لصفحة الدخول قبل تفعيل الصفحة → لا يحدث فشل حقن → يظهر 200 (لذلك فحص PowerShell أرجع 200 بينما المتصفح المسجّل يرجع 500).

## 3. الحل المفضّل (Refactor — إزالة الاعتماد على HttpClient)
بدل تسجيل `AddHttpClient` (حل مؤقت يعتمد على استدعاء HTTP عبر التطبيقات)، الأفضل حقن `CardBuilderService` الموجود أصلاً والذي يتصل بقاعدة البيانات مباشرة:

1. **إزالة** `private readonly IHttpClientFactory _httpClientFactory;` واستبداله بـ `private readonly CardBuilderService _cardBuilderService;`
2. **تعديل المنشئ**: `public BuilderModel(CardBuilderService cardBuilderService, ILogger<BuilderModel> logger)` وتعيين الحقل.
3. **إعادة كتابة `LoadOracleTablesAsync()`** لاستخدام الخدمة:
   ```csharp
   var tables = await _cardBuilderService.GetAvailableTablesAsync(ct);
   OracleTables = tables.Select(t => new SelectListItem
   {
       Value = t.SqlTargetTable,
       Text = $"{t.OracleSource} ({t.SqlTargetTable})"
   }).ToList();
   ```
4. **إعادة كتابة `LoadCloneDataAsync()`** لاستخدام `CloneFromCardAsync`:
   ```csharp
   var req = await _cardBuilderService.CloneFromCardAsync(cloneId, ct);
   if (req is null) return;
   CardType   = req.ChartType;
   SourceType = req.DataSourceType;
   SourceId   = req.SqlQuery;          // أفضل جهد: مصدر SQL مباشر
   CustomSql  = req.SqlQuery;
   Title      = req.Title;
   DisplayName= req.Title;             // لا يوجد DisplayName منفصل في الطلب
   GridWidth  = req.GridWidth;
   GridHeight = req.GridHeight;
   GridX      = req.GridPositionX;
   GridY      = req.GridPositionY;
   RefreshInterval = req.RefreshInterval;
   ChartOptionsJson  = "{}";
   FiltersJson = System.Text.Json.JsonSerializer.Serialize(req.DrillDownLevels ?? new());
   DrillDownConfigJson = "{}";
   CustomLabelsJson   = "{}";
   CloneId = string.Empty;             // نسخة جديدة وليست تحديثاً
   ```
5. **حذف الدوال الميتة** التي تستدعي نقاط نهاية غير موجودة: `OnGetOracleTablesAsync` (ميت) و `OnGetMeasurementsAsync` (يستدعي `/api/tablemappings/{id}/columns` غير موجود → 500 كامن). وكذلك حذف الكلاس `OracleTableDto` غير المستخدم بعد التعديل.
6. لا حاجة لتعديل `Program.cs` (الخدمة مسجّلة أصلاً في السطر 54: `builder.Services.AddScoped<CardBuilderService>();`).

## 4. التواقيع المتاحة (من الكود الحالي — لا تعدّلها)
- `CardBuilderService.GetAvailableTablesAsync(CancellationToken ct = default)` → `Task<List<TableMappingConfig>>`
  - `TableMappingConfig`: `int Id`, `string OracleSource`, `string SourceType`, `string SqlTargetTable`, `bool IsActive`, ...
- `CardBuilderService.CloneFromCardAsync(int cardId, CancellToken ct = default)` → `Task<CardBuilderRequest?>`
  - `CardBuilderRequest`: `Title`, `ChartType`, `DataSourceType`, `SqlQuery`, `GridPositionX`, `GridPositionY`, `GridWidth`, `GridHeight`, `RefreshInterval`, `IsActive`, `List<CardDrillDownInput> DrillDownLevels`, ...
- `CardBuilderService` مسجّل كـ `Scoped` في `Program.cs`.

## 5. القيود
- **المسار المسموح للكتابة الوحيد:**
  ```
  D:\HAE_Stores\TeraSystem\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
  ```
- **ممنوع** تعديل أي ملف HTML/CSS/JS أو أي ملف آخر (خاصة `Program.cs`, `CardBuilderService.cs`, `card-builder.js`, `Builder.cshtml`).
- الحفاظ على أسماء الحقول المخفية وحقول `BindProperty` الموجودة كما هي (الـ JS يعتمد عليها: `cardType`, `sourceType`, `sourceId`, `customSql`, `title`, `displayName`, `measurement`, `gridWidth`, `gridHeight`, `gridX`, `gridY`, `colorPalette`, `refreshInterval`, إلخ).
- لا أسرار مشفّرة.

## 6. معايير القبول
| # | المعيار | Status |
|---|---|---|
| AC-1 | `dotnet build -c Release` = 0 errors / 0 warnings | ⬜ |
| AC-2 | طلب مُصادَق عليه لـ `/admin-secure-panel/Cards/Builder` يرجع 200 (لا استثناء حقن) | ⬜ |
| AC-3 | القائمة المنسدلة للجداول (Step 2) تمتلئ من `TableMappings` عند وجودها | ⬜ |
| AC-4 | زر "نسخ" يملأ Wizard بإعدادات البطاقة (Clone) | ⬜ |
| AC-5 | لا استدعاءات HTTP داخلية من Web إلى API داخل `BuilderModel` | ⬜ |

## 7. ملاحظات
- هذا الإصلاح يحلّ مشكلة الـ 500 ويزيل اعتماداً هشاً على استدعاء HTTP عبر التطبيقات (Web → API) كان غير ضروري لأن `CardBuilderService` يتصل بقاعدة البيانات مباشرة.
- بعد الإصلاح: صفحة المنشئ ستعمل حتى لو الـ API متوقف (لأنها不再 تعتمد عليه).
