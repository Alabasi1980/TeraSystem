# TASK-AIQ-001 — إنشاء جداول SavedQueries و AiConversations + EF Entities + Migration

**المرحلة:** Phase 1 — البنية التحتية (Backend)
**الحالة:** Approved → In Progress
**تاريخ الاعتماد:** 2026-07-22
**المفوّض إلى:** engineering-agent-dotnet
**تاريخ الإنشاء:** 2026-07-22
**الهدف:** إنشاء جدولي قاعدة البيانات وكيانات EF Core للمكوّن الجديد AI Query Assistant

---

## 1. الوصف

إنشاء جدولين جديدين في SQL Server (نفس قاعدة WarehouseDashboard) مع كيانات EF Core وهجرة قاعدة البيانات:

1. **SavedQueries** — تخزين الكويريز المحفوظة (الاسم، الوصف، SQL، نوع المصدر)
2. **AiConversations** — سجل المحادثات مع AI لكل كويري محفوظ (الرسالة، الدور، SQL snapshot)

---

## 2. المخرجات المطلوبة

### 2.1 Entities — ملفان جديدان

**`Models/SavedQuery.cs`:**
```csharp
public class SavedQuery
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SqlQuery { get; set; } = string.Empty;
    public string DataSourceType { get; set; } = "SqlServer";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<AiConversation> Conversations { get; set; } = new List<AiConversation>();
}
```

**`Models/AiConversation.cs`:**
```csharp
public class AiConversation
{
    public long Id { get; set; }
    public int? SavedQueryId { get; set; }
    public string Role { get; set; } = string.Empty;  // 'user', 'assistant', 'system'
    public string Message { get; set; } = string.Empty;
    public string? SqlSnapshot { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public SavedQuery? SavedQuery { get; set; }
}
```

### 2.2 DbSets — تعديل `Data/WarehouseDashboardDbContext.cs`

إضافة:
```csharp
public DbSet<SavedQuery> SavedQueries => Set<SavedQuery>();
public DbSet<AiConversation> AiConversations => Set<AiConversation>();
```

مع **Fluent API configuration** في `OnModelCreating`:
- `SavedQueries`: Id (PK, auto-increment), Name (nvarchar(200), required), Description (nvarchar(500), nullable), SqlQuery (nvarchar(max), required), DataSourceType (nvarchar(50), required, default SqlServer, CHECK constraint), CreatedAt (datetime2, default GETUTCDATE()), UpdatedAt (datetime2, default GETUTCDATE())
- Index على `Name` و `UpdatedAt DESC`
- `AiConversations`: Id (PK, bigint, auto-increment), SavedQueryId (FK → SavedQueries.Id, nullable, CASCADE delete), Role (nvarchar(20), required, CHECK), Message (nvarchar(max), required), SqlSnapshot (nvarchar(max), nullable), CreatedAt (datetime2, default GETUTCDATE())
- Index على `(SavedQueryId, CreatedAt)`

### 2.3 EF Migration

تشغيل `dotnet ef migrations add AddSavedQueriesAndAiConversations` داخل `src/WarehouseDashboard.Web/`

### 2.4 Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\
```

---

## 3. معايير القبول (Acceptance Criteria)

| # | المعيار | طريقة التحقق |
|---|---------|-------------|
| 1 | ملف `SavedQuery.cs` موجود في `Models/` | وجود الملف |
| 2 | ملف `AiConversation.cs` موجود في `Models/` | وجود الملف |
| 3 | `SavedQueries` و `AiConversations` مضافتان في `WarehouseDashboardDbContext` | build PASS |
| 4 | Fluent API configuration كاملة لجميع الأعمدة والقيود | build PASS + مراجعة |
| 5 | CHECK constraint لـ `Role IN ('user','assistant','system')` | وجوده في التهيئة |
| 6 | CHECK constraint لـ `DataSourceType IN ('SqlServer', 'Oracle')` | وجوده في التهيئة |
| 7 | FK: `AiConversations.SavedQueryId → SavedQueries.Id ON DELETE CASCADE` | وجوده في التهيئة |
| 8 | Migration جديدة بعنوان `AddSavedQueriesAndAiConversations` | وجود ملف Migration |
| 9 | build PASS بدون errors أو warnings | `dotnet build` |
| 10 | لا تغيير في أي ملف خارج Allowed Write Targets | مراجعة الملفات المتغيرة |

---

## 4. Pre-Execution Gate Result

| # | Check | Result | Notes |
|---|-------|--------|-------|
| 1 | مرتبطة بخطة معتمدة | ✅ PASS | AI_QUERY_ASSISTANT_EXECUTION_PLAN.md — TASK-AIQ-001 |
| 2 | أصغر وحدة تنفيذية | ✅ PASS | فقط جداول + Entities + Migration — لا يشمل Services ولا API |
| 3 | هدف واحد فقط | ✅ PASS | إنشاء جداول البيانات فقط |
| 4 | لا يوجد عنصر يمكن تأجيله | ✅ PASS | كل العناصر مطلوبة لهذه المهمة |
| 5 | لا يضيف UI | ✅ PASS | back-end فقط |
| 6 | لا يضيف API | ✅ PASS | لا Routes جديدة |
| 7 | لا يضيف Auth | ✅ PASS | |
| 8 | يضيف Entities/Schema — مسموح | ✅ PASS | هذه مهمة Data Schema معتمدة |
| 9 | ينفذ migration — مسموح | ✅ PASS | مهمة قاعدة بيانات معتمدة |
| 10 | لا ينشئ `.env` | ✅ PASS | |
| 11 | لا يظهر secrets | ✅ PASS | |
| 12 | لا يضيف مكتبات غير مطلوبة | ✅ PASS | لا NuGet packages جديدة |
| 13 | يكتب داخل Allowed Write Targets | ✅ PASS | مسارات محددة بدقة |
| 14 | لا يعدل `tera-system/` أو `project-preparation/` | ✅ PASS | |
| 15 | أوامر مقترحة تحتاج موافقة | ✅ PASS | `dotnet ef migrations add` — يؤثر على قاعدة البيانات |
| 16 | تم فحص آثار `dotnet ef migrations add` | ✅ PASS | يُنشئ ملفات Migration داخل `Data/Migrations/` |
| 17 | لا أوامر تنشئ ملفات خارج النطاق | ✅ PASS | |
| 18 | لا تناقض بين القيود والمخرجات | ✅ PASS | |
| 19 | معايير القبول قابلة للاختبار | ✅ PASS | build + file existence |
| 20 | مسار تراجع آمن | ✅ PASS | حذف الملفات + إزالة migration |
| 21 | UI/Frontend? | N/A | |
| 22 | UI/Frontend? | N/A | |

**Gate Status: ✅ PASS**

**ملاحظات:**
- أمر `dotnet ef migrations add` سيُنفَّذ داخل `src/WarehouseDashboard.Web/`
- لا حاجة لـ `dotnet ef database update` الآن — Program.cs ينفذ `db.Database.Migrate()` تلقائياً عند بدء التشغيل

---

## 5. Model Capability Assessment

| البند | القيمة |
|-------|--------|
| Current Model | deepseek-v4-flash |
| Task Complexity | Low-Medium |
| Risk Level | Low |
| Required Reasoning | Low |
| Context Size | Low |
| Verification Difficulty | Low |
| Recommended Model Tier | Light |
| Minimum Acceptable Model Tier | Light |
| Decision | sufficient |
| User Approval Needed | No |

---

## 6. Handback & Review

### Sub-Agent Handback

| Action | Full Path |
|--------|-----------|
| ✅ Created | `Models/SavedQuery.cs` |
| ✅ Created | `Models/AiConversation.cs` |
| ✅ Modified | `Data/WarehouseDashboardDbContext.cs` |
| ✅ Generated | `Data/Migrations/20260722054351_AddSavedQueriesAndAiConversations.cs` |
| ✅ Generated | `Data/Migrations/20260722054351_AddSavedQueriesAndAiConversations.Designer.cs` |
| ✅ Updated (auto) | `Data/Migrations/WarehouseDashboardDbContextModelSnapshot.cs` |

**Build:** 0 errors, 0 warnings ✅
**Migration:** `AddSavedQueriesAndAiConversations`
**Safety:** [CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Tera Review

المراجعة اليدوية للملفات:
- `SavedQuery.cs` — صحيح، جميع الخصائص موجودة ✅
- `AiConversation.cs` — صحيح، Id bigint، FK nullable، Role, Message, SqlSnapshot ✅
- `DbContext.cs` — DbSets مضافتان، Fluent API كاملة مع CHECK constraints و FK ✅
- Migration — `SavedQueries` + `AiConversations` تم إنشاؤها ✅

**مشكلة تم حلها:** الـ Migration الأوّلي كان يحاول إضافة عمودي `ValueFormatType` و `ValueUnit` إلى `DashboardCards` — لكنهما موجودان فعلياً في قاعدة البيانات (من جلسة سابقة). تم التحقق من وجودهما عبر SQL Server والتأكد من أنها إضافة مشروعة من عميل آخر. تم تعديل الـ Migration ليكون No-op لهذين العمودين مع توثيق السبب، مع بقاء الجداول الجديدة فقط.

### Post-Execution Review Result

| # | Check | Result | Notes |
|---|-------|--------|-------|
| 1 | Changed files within Allowed Write Targets | ✅ PASS | كل الملفات داخل المسارات المسموحة |
| 2 | No unauthorized files created | ✅ PASS | |
| 3 | No unauthorized files deleted | ✅ PASS | |
| 4 | No unauthorized packages added | ✅ PASS | لا NuGet packages جديدة |
| 5 | No unauthorized UI/CSS/theme changes | ✅ PASS | |
| 6 | UI Acceptance Gate passed | N/A | |
| 7 | No real secrets outside approved local environment files | ✅ PASS | |
| 8 | Secrets redacted in docs/logs | ✅ PASS | |
| 9 | No unauthorized ORM models/entities/migrations | ✅ PASS | |
| 10 | No unapproved business validation moved to DB constraints | ✅ PASS | |
| 11 | No unauthorized API/Auth created | ✅ PASS | |
| 12 | Acceptance criteria satisfied | ✅ PASS | 10/10 ✅ |
| 13 | CLI side effects reviewed | ✅ PASS | `dotnet ef migrations add` — أنشأ ملفات Migration فقط |
| 14 | Task file and core project-control records reviewed | ✅ PASS | |
| 15 | No secret leakage | ✅ PASS | |
| 16 | No duplicate project-control IDs created | ✅ PASS | |
| 17 | Any out-of-target changes classified | ✅ PASS | أعمدة ValueFormatType/ValueUnit موجودة في DB من جلسة سابقة — تم تعديل الـ Migration ليكون No-op مع توثيق |
| 18 | Independent review decision recorded | ✅ PASS | Not Required |
| 19 | Auditor review decision recorded | ✅ PASS | NOT_REQUIRED |

**Gate Status: ✅ PASS**
**Root Cause of fix:** Model drift — أعمدة `ValueFormatType`/`ValueUnit` في `DashboardCard.cs` كانت مضافة للـ Model و DB لكن بدون Migration رسمي.
**Final Decision:** ✅ **Accepted**

**Independent Review:**
- ProjectControlAgent: Not Required
- SecurityAgent: Not Required
- QAAndAcceptanceAgent: Not Required
- Auditor: NOT_REQUIRED

**Deviation Classification:** N/A
