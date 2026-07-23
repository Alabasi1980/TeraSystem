# TASK-AIQ-005 — API Endpoints — ربط الخدمات مع QueryTester

**المرحلة:** Phase 1 — البنية التحتية (Backend)
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إضافة API Endpoints لصفحة QueryTester + تسجيل الخدمات في DI

---

## 1. الوصف

تعديل ملفين:
1. **`Pages/admin-secure-panel/QueryTester/Index.cshtml.cs`** — إضافة Handler Methods للـ AI Assistant (Chat، AiExecute، SavedQueries CRUD، SchemaInfo، ClearConversation)
2. **`Program.cs`** — تسجيل الخدمات الجديدة في Dependency Injection

---

## 2. المخرجات المطلوبة

### 2.1 — تعديل `Index.cshtml.cs`

#### إضافة للـ Constructor:
```csharp
private readonly AiQueryService _aiQueryService;
private readonly SavedQueryService _savedQueryService;
private readonly AiQueryContext _aiQueryContext;

public QueryTesterModel(
    IConfiguration configuration,
    ILogger<QueryTesterModel> logger,
    AiQueryService aiQueryService,
    SavedQueryService savedQueryService,
    AiQueryContext aiQueryContext)
{
    _configuration = configuration;
    _logger = logger;
    _aiQueryService = aiQueryService;
    _savedQueryService = savedQueryService;
    _aiQueryContext = aiQueryContext;
}
```

#### Handler 1: `OnPostChatAsync` — الدردشة مع AI
```csharp
public async Task<IActionResult> OnPostChatAsync([FromBody] AiChatRequest request)
{
    var result = await _aiQueryService.ChatAsync(request);
    return Json(result);
}
```

#### Handler 2: `OnPostAiExecuteAsync` — استعلام استكشاف للـ AI
```csharp
public async Task<IActionResult> OnPostAiExecuteAsync([FromBody] AiExecuteRequest request)
{
    var result = await _aiQueryContext.ExecuteExplorerQueryAsync(request.Sql, request.Source);
    return Json(new
    {
        success = result.Success,
        reply = result.Content,
        errorMessage = result.ErrorMessage
    });
}
```
مع DTO:
```csharp
public class AiExecuteRequest
{
    public string Sql { get; set; } = string.Empty;
    public string Source { get; set; } = "SqlServer";
}
```

#### Handler 3: `OnGetListQueriesAsync` — قائمة الكويريز المحفوظة
```csharp
public async Task<IActionResult> OnGetListQueriesAsync([FromQuery] string? search)
{
    var queries = await _savedQueryService.ListAsync(search);
    return Json(new { success = true, queries });
}
```

#### Handler 4: `OnPostSaveQueryAsync` — حفظ كويري جديد
```csharp
public async Task<IActionResult> OnPostSaveQueryAsync([FromBody] SavedQueryCreate request)
{
    var result = await _savedQueryService.CreateAsync(request);
    return Json(new { success = true, query = result });
}
```

#### Handler 5: `OnGetLoadQueryAsync` — تحميل كويري + محادثته
```csharp
public async Task<IActionResult> OnGetLoadQueryAsync([FromQuery] int id)
{
    var query = await _savedQueryService.GetByIdAsync(id);
    if (query is null)
        return Json(new { success = false, errorMessage = "الكويري غير موجود." });
    return Json(new { success = true, query });
}
```

#### Handler 6: `OnPostUpdateQueryAsync` — تحديث كويري
```csharp
public async Task<IActionResult> OnPostUpdateQueryAsync([FromBody] SavedQueryUpdateRequest request)
{
    var result = await _savedQueryService.UpdateAsync(request.Id, request.Data);
    if (result is null)
        return Json(new { success = false, errorMessage = "الكويري غير موجود." });
    return Json(new { success = true, query = result });
}
```
مع DTO:
```csharp
public class SavedQueryUpdateRequest
{
    public int Id { get; set; }
    public SavedQueryUpdate Data { get; set; } = new();
}
```

#### Handler 7: `OnPostDeleteQueryAsync` — حذف كويري
```csharp
public async Task<IActionResult> OnPostDeleteQueryAsync([FromBody] DeleteRequest request)
{
    var deleted = await _savedQueryService.DeleteAsync(request.Id);
    return Json(new { success = deleted });
}
```
مع DTO:
```csharp
public class DeleteRequest
{
    public int Id { get; set; }
}
```

#### Handler 8: `OnPostClearConversationAsync` — مسح محادثة
```csharp
public async Task<IActionResult> OnPostClearConversationAsync([FromBody] DeleteRequest request)
{
    var cleared = await _savedQueryService.ClearConversationAsync(request.Id);
    return Json(new { success = cleared });
}
```

#### Handler 9: `OnGetSchemaInfoAsync` — معلومات Schema كاملة للـ AI
```csharp
public async Task<IActionResult> OnGetSchemaInfoAsync([FromQuery] string? source)
{
    var schema = await _aiQueryContext.GetSchemaSummaryAsync(source);
    return Json(new { success = true, schema });
}
```

### 2.2 — تعديل `Program.cs`

إضافة تسجيل الخدمات الجديدة (بجانب التسجيلات الموجودة):
```csharp
builder.Services.AddScoped<SavedQueryService>();
builder.Services.AddScoped<AiQueryContext>();
builder.Services.AddScoped<AiQueryService>();
```

**ملاحظة مهمة:** يجب قراءة `Program.cs` من القرص قبل التعديل — لا تعتمد على الذاكرة. حافظ على كل الأسطر الموجودة ولا تمسح أي شيء.

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml.cs  (edit)
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Program.cs  (edit)
```

---

## 4. Forbidden Actions

- ❌ لا تعدل أي ملف خارج Allowed Write Targets
- ❌ لا تضيف NuGet packages
- ❌ لا تغير أو تحذف أي Handler موجود مسبقاً في Index.cshtml.cs
- ❌ لا تغير AIAssistantOptions، API Key، أو ConnectionStrings
- ❌ لا تنشئ UI

---

## 5. معايير القبول (Acceptance Criteria)

| # | المعيار | التحقق |
|---|---------|--------|
| 1 | AI endpoints مضافين في Index.cshtml.cs | وجود handlers |
| 2 | `SavedQueryService`, `AiQueryContext`, `AiQueryService` مسجّلين في DI | Program.cs |
| 3 | `OnPostChatAsync` يستخدم AiQueryService.ChatAsync | مراجعة |
| 4 | `OnPostAiExecuteAsync` يستخدم AiQueryContext.ExecuteExplorerQueryAsync | مراجعة |
| 5 | `OnGetListQueriesAsync` + `OnPostSaveQueryAsync` + `OnGetLoadQueryAsync` موجودة | مراجعة |
| 6 | `OnPostUpdateQueryAsync` + `OnPostDeleteQueryAsync` + `OnPostClearConversationAsync` موجودة | مراجعة |
| 7 | `OnGetSchemaInfoAsync` يستخدم AiQueryContext.GetSchemaSummaryAsync | مراجعة |
| 8 | `dotnet build` PASS — 0 errors, 0 new warnings | أمر الـ build |

---

## 6. Pre-Execution Gate Result

| # | Check | Result | Notes |
|---|-------|--------|-------|
| 1 | مرتبطة بخطة معتمدة | ✅ PASS | AI_QUERY_ASSISTANT_EXECUTION_PLAN.md |
| 2 | أصغر وحدة تنفيذية | ✅ PASS | API Endpoints فقط |
| 3 | هدف واحد فقط | ✅ PASS | ربط الخدمات مع QueryTester |
| 4–22 | باقي البنود | ✅ PASS | |

**Gate Status: ✅ PASS**

---

## 7. Model Capability Assessment

| البند | القيمة |
|-------|--------|
| Task Complexity | Medium |
| Risk Level | Low |
| Decision | sufficient |
| User Approval Needed | No |

---

## 8. Handback & Review

### Sub-Agent Handback

| Action | File |
|--------|------|
| ✅ Modified | `Pages/admin-secure-panel/QueryTester/Index.cshtml.cs` — 9 handlers + 3 DTOs + constructor DI |
| ✅ Modified | `Program.cs` — 3 services registered (SavedQueryService, AiQueryContext, AiQueryService) |

**Build:** 0 errors, 0 warnings ✅
**Existing handlers preserved:** ✅
**Safety:** [CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Tera Review

- 9 handler methods مضافين (Chat, AiExecute, ListQueries, SaveQuery, LoadQuery, UpdateQuery, DeleteQuery, ClearConversation, SchemaInfo) ✅
- 3 خدمات مسجلة في DI ✅
- الـ Handlers الموجودة مسبقاً (OnPostRunAsync، OnGetTablesAsync، إلخ) لم تتأثر ✅
- Build: 0 errors, 0 warnings ✅

### Post-Execution Review Result

| Check | Result |
|-------|--------|
| Changed files within Allowed Write Targets | ✅ PASS |
| No unauthorized files created | ✅ PASS |
| Existing handlers preserved | ✅ PASS |
| Acceptance criteria satisfied | ✅ PASS 8/8 |
| Build PASS | ✅ PASS |

**Gate Status: ✅ PASS**
**Final Decision:** ✅ **Accepted**
**Auditor:** NOT_REQUIRED
