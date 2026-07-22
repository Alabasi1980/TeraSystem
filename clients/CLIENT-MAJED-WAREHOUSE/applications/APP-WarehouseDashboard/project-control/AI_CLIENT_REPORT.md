# تقرير عميل الذكاء الاصطناعي — AI Client Report

**المشروع:** Warehouse Dashboard  
**التاريخ:** 2026-07-22  
**الغرض:** تقرير شامل لعميل الذكاء الاصطناعي المستخدم في المشروع،以便 دمج في صفحات أخرى (مثل QueryTester)

---

## 1. نظرة عامة

المشروع يستخدم **DeepSeek V4 Flash** عبر **OpenCode Go API** (OpenAI-compatible) لتحليل البطاقات في لوحة المعلومات.

| العنصر | القيمة |
|--------|--------|
| **مزوّد الخدمة** | OpenCode Go (OpenAI-compatible API) |
| **الموديل** | `deepseek-v4-flash` |
| **نقطة الوصول** | `https://opencode.ai/zen/go/v1/chat/completions` |
| **المصادقة** | Bearer Token (API Key) |
| **الشكل** | OpenAI Chat Completions API |
| **الوضع** | Thinking mode: `disabled` (لا يستخدم chain-of-thought) |

---

## 2. التكوين (appsettings.json)

```json
{
  "AIAssistant": {
    "ProviderName": "OpenCodeGo",
    "BaseUrl": "https://opencode.ai/zen/go/v1/chat/completions",
    "ModelId": "deepseek-v4-flash",
    "ApiKey": "sk-[REDACTED]",
    "TimeoutSeconds": 30,
    "MaxOutputTokens": 2000,
    "PromptVersion": "2.0"
  }
}
```

**ملاحظات:**
- `MaxOutputTokens`: 2000 (يجب أن يكون 2000 وليس 300 — القيمة الأصلية تسببت في ردود فارغة)
- `TimeoutSeconds`: 30 ثانية
- `PromptVersion`: "2.0" — يُستخدم لـ cache invalidation

---

## 3. بنية الكود (Architecture)

### 3.1 الواجهة (Interface)

```csharp
// Infrastructure/IAIProvider.cs
public interface IAIProvider
{
    Task<AIAssistantResponse> SendAsync(AIAssistantRequest request, CancellationToken ct = default);
}
```

### 3.2 الطلب والاستجابة (DTOs)

```csharp
// Infrastructure/AIAssistantRequest.cs
public class AIAssistantRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string? CardAssistantPrompt { get; set; }  // تعليمات خاصة بالبطاقة
    public int MaxOutputTokens { get; set; } = 300;
}

public class AIAssistantResponse
{
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TokensUsed { get; set; }
    public long ResponseTimeMs { get; set; }
}
```

### 3.3 المُكيّف (Adapter)

```csharp
// Infrastructure/OpenCodeGoAdapter.cs
public class OpenCodeGoAdapter : IAIProvider
{
    // يستخدم HttpClient (Not IHttpClientFactory)
    // يبني payload بتنسيق OpenAI Chat Completions
    // يعالج الأخطاء: HTTP errors, timeouts, empty responses, refusals
}
```

**هيكل الطلب المُرسل:**
```json
{
  "model": "deepseek-v4-flash",
  "messages": [
    { "role": "system", "content": "..." },
    { "role": "user", "content": "..." }
  ],
  "max_tokens": 2000,
  "thinking": { "type": "disabled" }
}
```

**هيكل الاستجابة المتوقعة (OpenAI format):**
```json
{
  "choices": [
    {
      "message": {
        "content": "الرد النصي...",
        "refusal": null
      },
      "finish_reason": "stop"
    }
  ],
  "usage": {
    "total_tokens": 150
  }
}
```

---

## 4. التسجيل في DI (Program.cs)

```csharp
// Program.cs — السطران 60-61
builder.Services.Configure<AIAssistantOptions>(
    builder.Configuration.GetSection(AIAssistantOptions.SectionName));

builder.Services.AddHttpClient<IAIProvider, OpenCodeGoAdapter>();
```

**ملاحظة مهمة:** يجب استخدام `AddHttpClient<IAIProvider, OpenCodeGoAdapter>()` وليس `AddHttpClient("name")`. المُكيّف يأخذ `HttpClient` مباشرة (وليس `IHttpClientFactory`).

---

## 5. كيفية الاستخدام في صفحة جديدة (QueryTester)

### الخطوة 1: حقن IAIProvider

```csharp
public class QueryTesterModel : PageModel
{
    private readonly IAIProvider _aiProvider;
    private readonly IOptions<AIAssistantOptions> _aiOptions;

    public QueryTesterModel(
        IAIProvider aiProvider,
        IOptions<AIAssistantOptions> aiOptions)
    {
        _aiProvider = aiProvider;
        _aiOptions = aiOptions;
    }
}
```

### الخطوة 2: إرسال طلب

```csharp
public async Task<IActionResult> OnPostAskAsync(string question, string? sqlContext)
{
    var request = new AIAssistantRequest
    {
        SystemPrompt = "أنت مساعد متخصص في قواعد البيانات. Help the user write SQL queries.",
        UserMessage = question,
        MaxOutputTokens = _aiOptions.Value.MaxOutputTokens
    };

    var response = await _aiProvider.SendAsync(request);

    if (response.Success)
    {
        return new JsonResult(new { answer = response.Content, tokens = response.TokensUsed });
    }
    else
    {
        return new JsonResult(new { error = response.ErrorMessage });
    }
}
```

### الخطوة 3: AJAX من الواجهة

```javascript
async function askAI(question) {
    const response = await fetch('/api/query-tester/ask', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ question: question })
    });
    const data = await response.json();
    if (data.answer) {
        document.getElementById('ai-answer').textContent = data.answer;
    } else {
        document.getElementById('ai-answer').textContent = data.error;
    }
}
```

---

## 6. ملفات مصدرية (Source Files)

| الملف | الوصف |
|-------|-------|
| `Infrastructure/IAIProvider.cs` | الواجهة |
| `Infrastructure/AIAssistantOptions.cs` | نموذج التكوين |
| `Infrastructure/AIAssistantRequest.cs` | DTOs: Request, Response, CardInsightResponse |
| `Infrastructure/OpenCodeGoAdapter.cs` | المُكيّف — يتواصل مع OpenCode Go API |
| `appsettings.json` | التكوين (قسم AIAssistant) |
| `Program.cs` (سطر 60-61) | تسجيل DI |

---

## 7. تحذيرات مهمة

1. **لا تكشف API Key في الكود** — استخدم Environment Variables في الإنتاج
2. **MaxOutputTokens يجب أن يكون 2000** — القيمة 300 تسبب ردود فارغة
3. **Thinking mode = disabled** — لا تفعّل chain-of-thought (يسبب بطء)
4. **HttpClient وليس IHttpClientFactory** — المُكيّف يأخذ `HttpClient` مباشرة
5. **PropertyNameCaseInsensitive = true** — عند استخدام `JsonSerializer.Deserialize` مع System.Text.Json

---

## 8. تكلفة وحدود

- **الموديل:** DeepSeek V4 Flash (سريع ورخيص)
- **الحد الأقصى للردود:** 2000 token
- **المهلة الزمنية:** 30 ثانية
- **الذاكرة المؤقتة:** تُخزّن النتائج في `IMemoryCache` لمدة 30 دقيقة
- **مفتاح الذاكرة المؤقتة:** `PromptVersion + card.UpdatedAt`

---

## 9. نصيحة لدمج QueryTester

لإضافة AI إلى صفحة QueryTester:

1. استخدم `IAIProvider` المُسجّل بالفعل في DI
2. أرسل system prompt مناسب لكتابة الاستعلامات
3. اجعل `MaxOutputTokens` = 2000
4. تعامل مع الأخطاء ب笑容 (timeout, empty response, HTTP errors)
5. استخدم cache إذا كان السؤال مكرراً
