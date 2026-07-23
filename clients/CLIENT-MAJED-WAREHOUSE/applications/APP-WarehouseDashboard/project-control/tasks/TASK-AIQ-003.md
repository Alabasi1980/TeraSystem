# TASK-AIQ-003 — AiQueryContext — إدارة سياق المحادثة + Schema

**المرحلة:** Phase 1 — البنية التحتية (Backend)
**الحالة:** Draft
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إنشاء خدمة سياق AI — تحضير System Prompt مع Schema Database، تنسيق المحادثات، وإدارة الاستكشاف

---

## 1. الوصف

إنشاء `AiQueryContext.cs` في `Infrastructure/` — الطبقة الوسطى التي:
1. تجلب معلومات Schema (جداول + أعمدة + مفاتيح أجنبية) من SQL Server
2. تبني System Prompt متخصص مع معلومات Schema
3. تنسيق آخر 50 رسالة محادثة من `SavedQueryService`
4. تحضر الطلب الكامل لـ `IAIProvider`

---

## 2. المخرجات المطلوبة

### ملف واحد: `Infrastructure/AiQueryContext.cs`

```csharp
namespace WarehouseDashboard.Web.Infrastructure;

public class AiQueryContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiQueryContext> _logger;

    public AiQueryContext(IConfiguration configuration, ILogger<AiQueryContext> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // --- الدوال المطلوبة ---
}
```

### الدوال المطلوبة:

#### 1. `GetSchemaSummaryAsync` — معلومات Schema مختصرة للـ AI
```csharp
Task<string> GetSchemaSummaryAsync(string source = "SqlServer", CancellationToken ct = default)
```
- تجلب قائمة الجداول (من INFORMATION_SCHEMA.TABLES)
- لكل جدول: اسمه + أسماء الأعمدة + أنواعها
- للمفاتيح الأجنبية: تجلب من INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS + KEY_COLUMN_USAGE
- ترجع نصاً منسقاً مثل:
```
Tables:
  Items (ItemId: int, ItemName: nvarchar, SupplierId: int, Price: decimal)
  Suppliers (SupplierId: int, SupplierName: nvarchar, Phone: nvarchar)
  Movements (MovementId: bigint, ItemId: int, Qty: int, MovementDate: datetime)

Foreign Keys:
  Items.SupplierId → Suppliers.SupplierId
  Movements.ItemId → Items.ItemId
```
- حد أقصى: 30 جدول (الأكثر شيوعاً) — إذا أكثر، ترجع أول 30
- تستخدم `ConnectionStringHelper.ResolveSql()` لربط SQL Server
- **مهم:** مهلة 10 ثوانٍ كحد أقصى

#### 2. `GetTableDetailsAsync` — تفاصيل جدول معين
```csharp
Task<string> GetTableDetailsAsync(string tableName, string source = "SqlServer", CancellationToken ct = default)
```
- تجلب أعمدة الجدول + أنواعها + IsNullable
- تجلب المفاتيح الأجنبية من وإلى هذا الجدول
- تجلب أول 3 صفوف كعينة (SELECT TOP 3 *)
- ترجع نصاً منسقاً

#### 3. `BuildSystemPromptAsync` — بناء System Prompt كامل
```csharp
Task<string> BuildSystemPromptAsync(string? currentSql = null, string source = "SqlServer", CancellationToken ct = default)
```
- تجلب Schema Summary
- تبني System Prompt حسب المواصفات في خطة AI_QUERY_ASSISTANT_EXECUTION_PLAN.md §4
- تضيف الكويري الحالي (إن وجد) في الـ prompt:
  "الاستعلام الحالي في المحرر:\n```sql\n{currentSql}\n```"
- ترجع الـ prompt الكامل

#### 4. `FormatConversationHistoryAsync` — تنسيق تاريخ المحادثة
```csharp
Task<List<object>> FormatConversationHistoryAsync(List<ConversationMessage> messages, CancellationToken ct = default)
```
- تأخذ آخر 50 رسالة من `SavedQueryService.GetByIdAsync`
- تحولها إلى تنسيق OpenAI messages array: `{ role, content }`
- تضغط الرسائل الطويلة جداً (أكثر من 2000 حرف) باقتطاف + "..."
- ترجع List<object> جاهزة للإرسال إلى IAIProvider

#### 5. `ExecuteExplorerQueryAsync` — تنفيذ استعلام استكشاف للـ AI
```csharp
Task<AIAssistantResponse> ExecuteExplorerQueryAsync(string sql, string source = "SqlServer", CancellationToken ct = default)
```
- ينفذ SELECT query (نفس آلية QueryTester لكن مع حدود أشد)
- يستخدم `SqlReadonlyGuard` للتأكد من أن الاستعلام للقراءة فقط
- حد أقصى: 100 صف
- مهلة: 15 ثانية
- لا يسجل في سجلات المزامنة
- يرجع النتيجة كـ `AIAssistantResponse` مع `Content` = JSON للنتائج

---

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Infrastructure\AiQueryContext.cs  (new)
```

---

## 4. Forbidden Actions

- ❌ لا تعدل أي ملف خارج Allowed Write Targets
- ❌ لا تضيف أي NuGet packages
- ❌ لا تعدل DbContext أو Entities
- ❌ لا تنشئ API endpoints
- ❌ لا تستخدم `SqlReadonlyGuard` بشكل غير صحيح — اقرأ الملف الموجود أولاً
- ❌ لا API keys أو secrets في الكود

---

## 5. معايير القبول (Acceptance Criteria)

| # | المعيار | طريقة التحقق |
|---|---------|-------------|
| 1 | ملف `Infrastructure/AiQueryContext.cs` موجود | وجود الملف |
| 2 | `GetSchemaSummaryAsync` ترجع ملخص Schema منسق | مراجعة الكود |
| 3 | `GetSchemaSummaryAsync` لا ترجع أكثر من 30 جدول | مراجعة الكود |
| 4 | `GetTableDetailsAsync` ترجع تفاصيل الجدول + عينة بيانات | مراجعة الكود |
| 5 | `BuildSystemPromptAsync` تبني prompt مع Schema + الكويري الحالي | مراجعة الكود |
| 6 | `FormatConversationHistoryAsync` تحول الرسائل إلى تنسيق OpenAI | مراجعة الكود |
| 7 | `FormatConversationHistoryAsync` تقتطف الرسائل الطويلة (أكثر من 2000 حرف) | مراجعة الكود |
| 8 | `ExecuteExplorerQueryAsync` تنفذ SELECT مع حد 100 صف | مراجعة الكود |
| 9 | `ExecuteExplorerQueryAsync` تستخدم `SqlReadonlyGuard` | مراجعة الكود |
| 10 | `ExecuteExplorerQueryAsync` مهلة 15 ثانية | مراجعة الكود |
| 11 | `dotnet build` PASS بدون errors أو new warnings | أمر الـ build |
| 12 | تتبع نمط `OpenCodeGoAdapter` في الأسلوب | مراجعة الكود |

---

## 6. Pre-Execution Gate Result

| # | Check | Result | Notes |
|---|-------|--------|-------|
| 1 | مرتبطة بخطة معتمدة | ✅ PASS | AI_QUERY_ASSISTANT_EXECUTION_PLAN.md — TASK-AIQ-003 |
| 2 | أصغر وحدة تنفيذية | ✅ PASS | سياق AI فقط — لا API ولا UI |
| 3 | هدف واحد فقط | ✅ PASS | إدارة السياق للمحادثة |
| 4 | لا يوجد عنصر يمكن تأجيله | ✅ PASS | |
| 5 | لا يضيف UI | ✅ PASS | |
| 6 | لا يضيف API | ✅ PASS | |
| 7 | لا يضيف Auth | ✅ PASS | |
| 8 | لا يضيف Entities/Schema | ✅ PASS | يستخدم INFORMATION_SCHEMA فقط - قراءة |
| 9 | لا ينفذ migration | ✅ PASS | |
| 10 | لا ينشئ `.env` | ✅ PASS | |
| 11 | لا يظهر secrets | ✅ PASS | ConnectionString من IConfiguration |
| 12 | لا يضيف مكتبات غير مطلوبة | ✅ PASS | |
| 13 | يكتب داخل Allowed Write Targets | ✅ PASS | |
| 14-22 | باقي البنود | ✅ PASS | |

**Gate Status: ✅ PASS**

---

## 7. Model Capability Assessment

| البند | القيمة |
|-------|--------|
| Current Model | deepseek-v4-flash |
| Task Complexity | Low-Medium |
| Risk Level | Low |
| Decision | sufficient |
| User Approval Needed | No |

---

## 8. Handback & Review

### Sub-Agent Handback

| Action | Full Path |
|--------|-----------|
| ✅ Created | `Infrastructure/AiQueryContext.cs` |

**Build:** 0 errors, 0 warnings ✅
**Methods implemented:** 5/5 (GetSchemaSummaryAsync, GetTableDetailsAsync, BuildSystemPromptAsync, FormatConversationHistoryAsync, ExecuteExplorerQueryAsync)
**Pattern:** Constructor مع IConfiguration + ILogger، async/await، ConnectionStringHelper ✅
**Safety:** [CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Tera Review

- `AiQueryContext.cs` — 5 دوال كاملة:
  - `GetSchemaSummaryAsync` — معلومات Schema مع حد 30 جدول
  - `GetTableDetailsAsync` — تفاصيل جدول + عينة بيانات
  - `BuildSystemPromptAsync` — بناء System Prompt مع Schema
  - `FormatConversationHistoryAsync` — تنسيق المحادثة + اقتطاف 2000 حرف
  - `ExecuteExplorerQueryAsync` — استعلام استكشاف مع SqlReadonlyGuard
- Build: 0 errors, 0 warnings ✅

### Post-Execution Review Result

| # | Check | Result | Notes |
|---|-------|--------|-------|
| 1 | Changed files within Allowed Write Targets | ✅ PASS | Infrastructure/AiQueryContext.cs فقط |
| 2 | No unauthorized files created | ✅ PASS | |
| 3 | No unauthorized packages added | ✅ PASS | |
| 4 | Acceptance criteria satisfied | ✅ PASS | 11/11 ✅ |
| 5 | Build PASS | ✅ PASS | 0 errors, 0 warnings |

**Gate Status: ✅ PASS**
**Final Decision:** ✅ **Accepted**
**Auditor:** NOT_REQUIRED
