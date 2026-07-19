# TASK-DRILL-SCHEMA-001 — Phase A: Schema & API Contract Foundation

| البند | القيمة |
|---|---|
| **المعرف** | TASK-DRILL-SCHEMA-001 |
| **المجموعة** | DRILL-DOWN (Phase A) |
| **النوع** | C# Backend + EF Core Migration |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High (P0) |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `project-preparation/DRILL_DOWN_DEVELOPMENT_PLAN.md` v3.0 — §6, §9 |

---

## 1. الهدف

إضافة ثلاثة حقول جديدة إلى جدول `CardDrillDownLevels` لتمكين آلية Drill Down الاحترافية المبنية على عقد باراميترات صريح بدلاً من الاعتماد العشوائي على أول عمود. ثم تحديث Drill API لإرجاع هذه الحقول في الـ payload.

الحقول الثلاثة (P0):
- `ParameterColumn` — العمود الذي يؤخذ منه `@p0` للمستوى التالي.
- `LabelColumn` — النص المعروض في Breadcrumb.
- `RequiresParentValue` — هل المستوى يحتاج قيمة من المستوى السابق؟

الحقول المؤجلة (P1) — **لا تضفها في هذه المهمة**:
- `EnableExport`, `MaxRows` (سيُضافان في مهمة لاحقة).

---

## 2. السياق الحالي

### 2.1 ملفات المعنية (يجب قراءتها قبل التعديل)

| الملف | الحالة الحالية |
|---|---|
| `src/WarehouseDashboard.Web/Models/CardDrillDownLevel.cs` | يحتوي على: Id, ParentCardId, Level, DisplayName, DrillDownQuery, TargetChartType, Card navigation |
| `src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs` | Entity config لـ `CardDrillDownLevels` في الأسطر 238-276 (approx) — يحتوي على CHECK constraints + Unique Index |
| `src/WarehouseDashboard.Web/Pages/Api/Dashboard/Drill.cshtml.cs` | Drill API — يحتوي على local SqlParamValue + Sanitize helpers |
| `src/WarehouseDashboard.Web/Pages/DrillDataResult.cs` | API payload — لا يحتوي على ParameterColumn/LabelColumn/RequiresParentValue fields |
| `src/WarehouseDashboard.Web/Migrations/` | آخر migration: `20260718173658_AddDashboardCardDescription` — استخدم النمط `migrationBuilder.AddColumn<string>(...)` |
| `src/WarehouseDashboard.Web/Migrations/WarehouseDashboardDbContextModelSnapshot.cs` | يجب تحديثه عند إنشاء migration جديد |

### 2.2 نمط Migration المعتمد

راجع `Migrations/20260718173658_AddDashboardCardDescription.cs` للنمط المعتمد: `AddColumn` مع `defaultValue` و `Down` يعكس التغييرات.

### 2.3 تحذير مهم حول المجلدين

يوجد migrations في مسارين:
1. `src/WarehouseDashboard.Web/Migrations/` — النشط (استخدمته آخر migrations).
2. `src/WarehouseDashboard.Web/Data/Migrations/` — قديم.

**استخدم مسار .NET CLI الافتراضي** (`dotnet ef migrations add ...`) — سيُنشئ الملف في المجلد الصحيح تلقائياً حسب DbContext location. لا تنشئ migration يدوياً في المجلد القديم.

---

## 3. التغييرات المطلوبة

### 3.1 تحديث Model: `CardDrillDownLevel.cs`

أضف الخصائص الثلاث الجديدة بعد `TargetChartType`:

```csharp
/// <summary>
/// Column name in this level's result set whose value is passed to the next level's
/// <c>@p0</c> parameter. When null/empty, the first column is used as fallback.
/// Setting this explicitly is strongly recommended for non-trivial drill chains.
/// </summary>
public string? ParameterColumn { get; set; }

/// <summary>
/// Column name used to render a human-readable label for this level's selected value
/// in the breadcrumb (e.g., CategoryName instead of CategoryCode). When null/empty,
/// falls back to <see cref="ParameterColumn"/>.
/// </summary>
public string? LabelColumn { get; set; }

/// <summary>
/// When true, this level requires a parent value from the previous level (passed as
/// <c>@p0</c> via SqlParameter). Level 1 typically sets this to false (root). Levels > 1
/// typically set this to true. When true but no parentValue is provided, the API should
/// return an error instead of running the query.
/// </summary>
public bool RequiresParentValue { get; set; } = false;
```

### 3.2 تحديث DbContext: `WarehouseDashboardDbContext.cs`

داخل قسم `modelBuilder.Entity<CardDrillDownLevel>(entity => { ... })` (بعد `TargetChartType` property):

```csharp
entity.Property(e => e.ParameterColumn)
    .HasMaxLength(100)
    .IsRequired(false);

entity.Property(e => e.LabelColumn)
    .HasMaxLength(100)
    .IsRequired(false);

entity.Property(e => e.RequiresParentValue)
    .IsRequired()
    .HasDefaultValue(false);
```

**ملاحظة مهمة**: لا تضف CHECK constraints على ParameterColumn/LabelColumn — القيم تُتحقق منها عند runtime في API (case-insensitive lookup).

### 3.3 إنشاء Migration جديد

استخدم الأمر (من داخل `src/WarehouseDashboard.Web`):

```powershell
dotnet ef migrations add AddDrillDownParameterContract
```

<span style="color: red;">تأكد أن الـ migration يحتوي على AddColumn للحقول الثلاث + DefaultValue لـ RequiresParentValue = 0 (bit).</span>

### 3.4 تحديث Drill API payload: `DrillDataResult.cs`

أضف الخصائص الجديدة بعد `HasNextLevel`:

```csharp
/// <summary>
/// Column name in the current level's result set whose value should be passed as
/// <c>@p0</c> to the next level. Null/empty = first column fallback.
/// </summary>
public string? ParameterColumn { get; set; }

/// <summary>
/// Column name used for human-readable labels in the breadcrumb. Null/empty =
/// falls back to ParameterColumn.
/// </summary>
public string? LabelColumn { get; set; }

/// <summary>
/// When true, the next level requires a parent value (selected row's
/// ParameterColumn value) to execute. Used by client to enforce row selection
/// before navigation.
/// </summary>
public bool NextRequiresParentValue { get; set; }
```

**ملاحظة تصميمية**: استخدمنا `NextRequiresParentValue` (وليس `RequiresParentValue`) في الـ payload لأن العميل (frontend) يحتاج معرفة ما إذا كان **المستوى التالي** يتطلب قيمة ليعرف هل يجب إجبار المستخدم على اختيار صف قبل الانتقال.

### 3.5 تحديث Drill API: `Drill.cshtml.cs`

في `OnGetAsync`، بعد استرجاع `config` وقبل `var hasNext = ...`:

#### أ. تحديث `result` initialization:

أضف إلى `DrillDataResult` constructor call:

```csharp
ParameterColumn = config.ParameterColumn,
LabelColumn = config.LabelColumn,
```

#### ب. إضافة فحص RequiresParentValue للمستوى الحالي:

قبل تنفيذ SQL (قبل `try`)، أضف:

```csharp
// If THIS level requires a parent value but none was provided → error out
// (Level 1 typically has RequiresParentValue = false)
if (config.RequiresParentValue && string.IsNullOrWhiteSpace(parentValue))
{
    result.Status = "error";
    result.ErrorMessage = "هذا المستوى يتطلب قيمة من المستوى السابق. يرجى اختيار عنصر من المستوى السابق.";
    return Json(result);
}
```

#### ج. تحديث `hasNext` query لجلب `RequiresParentValue` للمستوى التالي:

استبدل:

```csharp
var hasNext = await _db.CardDrillDownLevels
    .AnyAsync(l => l.ParentCardId == cardId && l.Level == level + 1, cancellationToken);
```

بـ:

```csharp
var nextLevel = await _db.CardDrillDownLevels
    .Where(l => l.ParentCardId == cardId && l.Level == level + 1)
    .Select(l => new { l.RequiresParentValue })
    .FirstOrDefaultAsync(cancellationToken);

var hasNext = nextLevel != null;
result.HasNextLevel = hasNext;
result.NextRequiresParentValue = nextLevel?.RequiresParentValue ?? false;
```

ثم احذف السطر القديم `HasNextLevel = hasNext` من الـ initializer الأصلي لأنه أصبح يُضبط أعلاه.

### 3.6 التحقق من ParameterColumn موجود في نتيجة الاستعلام (Defensive)

بعد بناء `result.Columns` وأثناء تنفيذ query، تحقق أن `ParameterColumn` (إن تم تحديده) موجود في الأعمدة:

```csharp
// Validate ParameterColumn references a real column (when specified)
if (!string.IsNullOrWhiteSpace(config.ParameterColumn))
{
    var parameterColumnFound = columns.Exists(
        c => string.Equals(c, config.ParameterColumn, StringComparison.OrdinalIgnoreCase));
    if (!parameterColumnFound)
    {
        result.Status = "error";
        result.ErrorMessage = $"عمود الباراميتر المحدد '{config.ParameterColumn}' غير موجود في نتيجة الاستعلام. الأعمدة المتاحة: {string.Join(", ", columns)}";
        return Json(result);
    }
}

// Validate LabelColumn references a real column (when specified)
if (!string.IsNullOrWhiteSpace(config.LabelColumn))
{
    var labelColumnFound = columns.Exists(
        c => string.Equals(c, config.LabelColumn, StringComparison.OrdinalIgnoreCase));
    if (!labelColumnFound)
    {
        result.Status = "error";
        result.ErrorMessage = $"عمود التسمية المحدد '{config.LabelColumn}' غير موجود في نتيجة الاستعلام. الأعمدة المتاحة: {string.Join(", ", columns)}";
        return Json(result);
    }
}
```

ضع هذا التحقيق داخل `try { ... }` بعد `result.Columns = columns;` وقبل loop قراءة الصفوف.

---

## 4. Allowed Write Targets

يجب أن تكون جميع المسارات كاملة:

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\CardDrillDownLevel.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DrillDataResult.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Dashboard\Drill.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Migrations\  (new migration files only — AddDrillDownParameterContract + .Designer + ModelSnapshot update)
```

**لا تلمس**:
- `Index.cshtml` (Dashboard page) — مهمة لاحقة.
- `admin-secure-panel/DrillDown/*` — مهمة TASK-DRILL-ADMIN-001.
- أي migration قديم.
- `appsettings.json`.

---

## 5. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `ParameterColumn`, `LabelColumn`, `RequiresParentValue` مضافون إلى `CardDrillDownLevel.cs` كما في §3.1 |
| AC-2 | DbContext config يضبط `HasMaxLength(100)`, `IsRequired(false)` لـ `ParameterColumn` و `LabelColumn`، و `IsRequired().HasDefaultValue(false)` لـ `RequiresParentValue` كما في §3.2 |
| AC-3 | Migration جديد اسمه `AddDrillDownParameterContract` يُنشأ في `Migrations/` (النشط)، يحتوي على `AddColumn` للحقول الثلاث + `defaultValue` صحيح (null لـ strings, `0` لـ bit) |
| AC-4 | `Down()` في الـ migration يعكس التغييرات (DropColumn لكل حقول الثلاثة) |
| AC-5 | `DrillDataResult.cs` يحتوي على `ParameterColumn`, `LabelColumn`, `NextRequiresParentValue` كما في §3.4 |
| AC-6 | `Drill.cshtml.cs` يضبط `ParameterColumn` و `LabelColumn` في `result` من `config` |
| AC-7 | إذا `config.RequiresParentValue == true` و `parentValue` فارغ → API يرجع `status=error` مع رسالة واضحة |
| AC-8 | API يجلب `RequiresParentValue` للمستوى التالي ويضبط `NextRequiresParentValue` في الـ payload |
| AC-9 | إذا `ParameterColumn` محدد لكنه غير موجود في نتيجة الاستعلام → API يرجع `status=error` مع رسالة تحدد الأعمدة المتاحة |
| AC-10 | نفس التحقيق ينطبق على `LabelColumn` |
| AC-11 | كل قيم SQL تمر عبر `SqlParameter` (موجود حالياً — تأكد من عدم كسره) |
| AC-12 | `Sanitize()` يستخدم للأخطاء (موجود حالياً — تأكد من عدم كسره) |
| AC-13 | `dotnet build` من `src/WarehouseDashboard.Web` ناجح: 0 errors, 0 warnings |
| AC-14 | `dotnet ef migrations add AddDrillDownParameterContract` ينجح بدون أخطاء |
| AC-15 | `dotnet ef database update` (أو تغيير المسار من قبل Majed) ينطبق الـ migration على قاعدة البيانات بنجاح — لا يُطلب تنفيذه بدون إذن Majed، فقط تأكد من صحة الـ migration structurally |
| AC-16 | لا secrets في أي ملف |
| AC-17 | Encoding عربي سليم في كل الرسائل (لا mojibake) |

---

## 6. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ Schema + API payload فقط — لا UI، لا Admin page |
| لا تغيير واجهة | ✅ |
| لا تغيير Auth | ✅ |
| API آمن | ✅ SqlParameter + Sanitize موجودان + ParameterColumn validation مضاف |
| Migration آمن | ✅ AddColumn فقط — لا DropColumn لحقول قائمة، لا data loss |
| Build متوقع | ✅ 0 errors, 0 warnings |
| تراجع آمن | ✅ Migration Down() يعكس كل شيء |
| لا scope creep | ✅ لا تضف EnableExport/MaxRows (مهمة لاحقة) |

**Gate Status:** ✅ PASS

---

## 7. ملاحظات للوكيل المنفذ

1. **اقرأ كل ملف قبل تعديله** — استخدم `read` tool أولاً للتحقق من المحتوى الحالي.
2. **احافظ على التعليقات والـ XML docs الموجودة** — لا تحذفها.
3. **استخدم نفس نمط الكتابة** الموجود في الملفات (4-space indent, Arabic comments where appropriate).
4. **بعد إنشاء الـ migration**، افتحه وتأكد أن `Up()` و `Down()` صحيحان completely قبل اعتبار المهمة منجزة.
5. **شغّل `dotnet build`** قبل اعتبار المهمة منجزة.
6. **لا تطبّق الـ migration على DB** — هذا دور Majed. فقط أنشئ الـ migration file.
7. **أبلغ عن أي surprises** — مثل وجود schema drift، أو model snapshot قديم، أو ملفات encoding مشكلة.
8. إذا تعذّر `dotnet ef migrations add` بسبب app lock (الـ Web app قد يكون قيد التشغيل) — استخدم الـ build أولاً ثم إعادة محاولة. إذا استمر، أبلغ عن المشكلة.
9. **التزم بـ Allowed Write Targets** — لا تلمس أي ملف خارج القائمة.

---

## 8. Mid-Task Compliance Checkpoint

بعد كل مجموعة من write operations، سجّل:

```
[CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓
```

---

## 9. Handback Format

عند التسليم، أبلغ بالشكل التالي:

```
## TASK-DRILL-SCHEMA-001 — Handback

### Files Modified
- [list with line counts]

### Migration Created
- Name: AddDrillDownParameterContract
- Path: [full path]
- Up() operations: [list]
- Down() operations: [list]

### Build Result
- dotnet build: [PASS/FAIL] — [N errors, N warnings]

### Acceptance Criteria Status
- AC-1 through AC-17: [✅/❌ per item]

### Compliance Checkpoints
[CP] Allowed Write Targets ✓ | No secrets ✓ | In scope ✓

### Issues / Surprises (if any)
- [description]

### Recommended Next Action
- [suggestion]
```

---

## 10. Vitality & Polish Checklist

N/A — هذه مهمة Backend (Schema + API) بدون UI. ينطبق البند "N/A" بسبب: لا يوجد أي rendering أو interaction surface في هذه المهمة. كل العمل في Model/DbContext/Migration/API payload.

---

**End of TASK-DRILL-SCHEMA-001**
