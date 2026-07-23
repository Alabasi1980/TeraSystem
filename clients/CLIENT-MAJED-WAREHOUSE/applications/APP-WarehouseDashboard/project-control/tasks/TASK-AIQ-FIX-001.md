# TASK-AIQ-FIX-001 — إصلاح ثغرات التدقيق (F-001, F-002, F-004, F-007)

**المرحلة:** Phase 1 — Fixes
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إصلاح 4 ثغرات اكتشفها Auditor في Phase 1

---

## 1. الإصلاحات المطلوبة

### 🔶 F-001 (CAUTION) — تمرير تاريخ المحادثة إلى AI Provider

**المشكلة:** `formattedHistory` يُحسب في `ChatAsync` لكن لا يُرسل إلى `IAIProvider`.

**الملفات المتأثرة:**
1. **`Infrastructure/AIAssistantRequest.cs`** — إضافة خاصية `Messages`
2. **`Infrastructure/OpenCodeGoAdapter.cs`** — استخدام `Messages` إذا وُجدت
3. **`Services/AiQueryService.cs`** — تمرير `formattedHistory` إلى `AIAssistantRequest`

#### 1.1 — تعديل `AIAssistantRequest.cs`
```csharp
public class AIAssistantRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string? CardAssistantPrompt { get; set; }
    public int MaxOutputTokens { get; set; } = 300;
    
    /// <summary>Optional conversation history (system prompt already set via SystemPrompt).</summary>
    public List<object>? Messages { get; set; }
}
```

#### 1.2 — تعديل `OpenCodeGoAdapter.cs` (داخل `SendAsync`)
تعديل بناء `messages` array:
```csharp
// Build messages array
var messagesList = new List<object>();

// System prompt
messagesList.Add(new { role = "system", content = systemPrompt });

// Conversation history (if provided)
if (request.Messages is { Count: > 0 })
{
    foreach (var msg in request.Messages)
    {
        messagesList.Add(msg);
    }
}

// Current user message
messagesList.Add(new { role = "user", content = request.UserMessage });

var payload = new
{
    model = _options.ModelId,
    messages = messagesList,
    max_tokens = request.MaxOutputTokens > 0 ? request.MaxOutputTokens : _options.MaxOutputTokens,
    thinking = new { type = "disabled" }
};
```

#### 1.3 — تعديل `AiQueryService.cs` (داخل `ChatAsync`)
بعد `var formattedHistory = await _aiContext.FormatConversationHistoryAsync(...)`، تمريره إلى `AIAssistantRequest`:
```csharp
var aiRequest = new AIAssistantRequest
{
    SystemPrompt = systemPrompt,
    UserMessage = request.Message,
    Messages = formattedHistory,
    MaxOutputTokens = 2000  // سيتم تحسينه في F-007
};
```

---

### 🟡 F-002 — إضافة CancellationToken إلى 7 Handlers

**الملف:** `Pages/admin-secure-panel/QueryTester/Index.cshtml.cs`

إضافة `HttpContext.RequestAborted` كـ `CancellationToken` إلى كل استدعاء خدمة في الـ 7 Handlers التالية:
- `OnPostChatAsync` → `_aiQueryService.ChatAsync(request, HttpContext.RequestAborted)`
- `OnGetListQueriesAsync` → `_savedQueryService.ListAsync(search, HttpContext.RequestAborted)`
- `OnPostSaveQueryAsync` → `_savedQueryService.CreateAsync(request, HttpContext.RequestAborted)`
- `OnGetLoadQueryAsync` → `_savedQueryService.GetByIdAsync(id, HttpContext.RequestAborted)`
- `OnPostUpdateQueryAsync` → `_savedQueryService.UpdateAsync(request.Id, request.Data, HttpContext.RequestAborted)`
- `OnPostDeleteQueryAsync` → `_savedQueryService.DeleteAsync(request.Id, HttpContext.RequestAborted)`
- `OnPostClearConversationAsync` → `_savedQueryService.ClearConversationAsync(request.Id, HttpContext.RequestAborted)`

---

### 🟡 F-004 — إضافة `[Required]` على DTOs

**الملف:** `Models/Dto/SavedQueryDtos.cs`

إضافة `using System.ComponentModel.DataAnnotations;` ووضع `[Required]` على:
- `SavedQueryCreate.Name`
- `SavedQueryCreate.SqlQuery`
- `SavedQueryUpdate.Name`
- `SavedQueryUpdate.SqlQuery`

---

### 🟡 F-007 — قراءة MaxOutputTokens من AIAssistantOptions

**الملف:** `Services/AiQueryService.cs`

1. إضافة `IOptions<AIAssistantOptions>` إلى الـ Constructor:
```csharp
private readonly IOptions<AIAssistantOptions> _aiOptions;

public AiQueryService(
    IAIProvider aiProvider,
    AiQueryContext aiContext,
    SavedQueryService savedQueryService,
    IOptions<AIAssistantOptions> aiOptions,
    ILogger<AiQueryService> logger)
{
    _aiProvider = aiProvider;
    _aiContext = aiContext;
    _savedQueryService = savedQueryService;
    _aiOptions = aiOptions;
    _logger = logger;
}
```

2. استخدام `_aiOptions.Value.MaxOutputTokens` بدلاً من `2000`:
```csharp
MaxOutputTokens = _aiOptions.Value.MaxOutputTokens
```

3. إضافة using: `using Microsoft.Extensions.Options;`
4. إضافة using: `using WarehouseDashboard.Web.Infrastructure;` (إذا لم تكن موجودة)

---

## 2. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Infrastructure\AIAssistantRequest.cs  (edit)
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Infrastructure\OpenCodeGoAdapter.cs  (edit)
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Services\AiQueryService.cs  (edit)
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\QueryTester\Index.cshtml.cs  (edit)
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\Dto\SavedQueryDtos.cs  (edit)
```

---

## 3. معايير القبول

| # | المعيار | التحقق |
|---|---------|--------|
| 1 | `AIAssistantRequest.Messages` property موجود | مراجعة |
| 2 | `OpenCodeGoAdapter` يبني messages متضمنة history إذا وُجدت | مراجعة |
| 3 | `AiQueryService.ChatAsync` يمرر `formattedHistory` إلى `AIAssistantRequest` | مراجعة |
| 4 | 7 Handlers يمررون `HttpContext.RequestAborted` | مراجعة |
| 5 | `[Required]` على Name و SqlQuery في SavedQueryCreate و SavedQueryUpdate | مراجعة |
| 6 | `AiQueryService` يستخدم `IOptions<AIAssistantOptions>` لـ MaxOutputTokens | مراجعة |
| 7 | `dotnet build` PASS — 0 errors, 0 new warnings | أمر الـ build |

---

## 4. Pre-Execution Gate

| # | Check | Result |
|---|-------|--------|
| 1 | مرتبطة بخطة معتمدة | ✅ PASS (موصى بها من Auditor) |
| 2 | أصغر وحدة تنفيذية | ✅ PASS |
| 3 | يكتب داخل Allowed Write Targets | ✅ PASS |
| 4–22 | باقي البنود | ✅ PASS |

**Gate Status: ✅ PASS**

---

## 5. Handback & Review

### Fixes Applied

| Fix | File | Change |
|-----|------|--------|
| F-001 | `AIAssistantRequest.cs` | إضافة `Messages` property |
| F-001 | `OpenCodeGoAdapter.cs` | بناء messages مع history |
| F-001 | `AiQueryService.cs` | تمرير `formattedHistory` |
| F-002 | `Index.cshtml.cs` | 7 Handlers ← HttpContext.RequestAborted |
| F-004 | `SavedQueryDtos.cs` | `[Required]` على Name + SqlQuery |
| F-007 | `AiQueryService.cs` | MaxOutputTokens من `IOptions<AIAssistantOptions>` |

**Build:** 0 errors, 0 warnings ✅
**AC:** 7/7 ✅
**Safety:** [CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Post-Execution Review: ✅ PASS → **Accepted**
