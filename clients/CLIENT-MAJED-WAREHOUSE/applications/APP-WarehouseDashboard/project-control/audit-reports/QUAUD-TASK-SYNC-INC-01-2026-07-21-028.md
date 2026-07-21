# QUAUD-TASK-SYNC-INC-01-2026-07-21-028.md

## Audit ID: QUAUD-028
## Task Reviewed: TASK-SYNC-INC-01 — تحسين المزامنة التزايدية مع تاريخ بداية
## Invoked By: TeraAgent
## Audit Mode: Standard
## Scope: Changed Code (Model + Migration + Sync Engine + API + UI)
## Report Path: `project-control/audit-reports/QUAUD-TASK-SYNC-INC-01-2026-07-21-028.md`
## Evidence Sources Used:
- `project-control/tasks/TASK-SYNC-INC-01.md` (task file + acceptance criteria)
- `src/WarehouseDashboard.Web/Models/TableMappingConfig.cs`
- `src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `src/WarehouseDashboard.Api/Models/TableMapping.cs`
- `src/WarehouseDashboard.Web/Data/Migrations/20260721083315_AddInitialSyncStartDateToTableMappings.cs`
- `src/WarehouseDashboard.Api/Services/SyncEngineService.cs`
- `src/WarehouseDashboard.Api/Services/SqlServerLoadService.cs`
- `src/WarehouseDashboard.Api/Controllers/TableMappingController.cs`
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml`
- `src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/Index.cshtml.cs`
- `src/WarehouseDashboard.Web/wwwroot/js/table-mapping-wizard.js`

---

## Overall Quality Gate: NEEDS_FIX

| Severity | Count |
|---|---|
| STOP | 0 |
| CAUTION | 2 |
| FLAG | 2 |

---

## 1. الملخص التنفيذي

المهمة **TASK-SYNC-INC-01** تهدف إلى تحسين المزامنة التزايدية بإضافة "تاريخ بداية المزامنة" (`InitialSyncStartDate`). المراجعة شملت جميع الطبقات (Model → Migration → Sync Engine → API → UI).

**النتيجة العامة: NEEDS_FIX**

تم تنفيذ الغالبية العظمى من معايير القبول بشكل صحيح. النموذج، الهجرة، الاستعلام الأوراكي، واجهة المستخدم، والتحكم بالـ API جميعها تعمل بشكل متسق. إلا أن هناك **ملاحظتين من مستوى CAUTION** تتعلقان بسلامة البيانات في عملية الـ deduplication، وملاحظتين من مستوى FLAG تتعلقان باتساق بسيط.

**القرار**: المهمة غير جاهزة للإغلاق الكامل حتى تُعالج مسألة المعاملة (transaction) في `LoadTableIncrementalAsync`.

---

## 2. تدقيق معايير القبول

### 2.1 طبقة النموذج (Model Layer)

| المعيار | النتيجة | التفاصيل |
|---|---|---|
| `TableMappingConfig.cs` — خاصية `InitialSyncStartDate` nullable | **PASS** | سطر 79: `public DateTime? InitialSyncStartDate { get; set; }` ✅ |
| `TableMapping.cs` (API) — خاصية `InitialSyncStartDate` | **PASS** | سطر 63: `public DateTime? InitialSyncStartDate { get; set; }` ✅ |
| EF Core config في `DbContext` | **PASS** | سطور 403-405: `HasColumnType("datetime2").IsRequired(false)` ✅ |
| `[MaxLength(50)]` على الخاصية | **FLAG-01** | لم يُضاف. ملاحظة: `MaxLength` على `DateTime?` غير معتاد ولا يُنفذه EF Core على datetime2. المعيار في ملف المهمة نفسه غير دقيق تقنياً. |

### 2.2 الهجرة (Migration)

| المعيار | النتيجة | التفاصيل |
|---|---|---|
| الهجرة تضيف `InitialSyncStartDate` كـ datetime2 nullable | **PASS** | `AddColumn<DateTime>` مع `type: "datetime2", nullable: true` ✅ |
| `Down()` عكس صحيح لـ `Up()` | **PASS** | `DropColumn` في Down عكس `AddColumn` في Up تمامًا ✅ |
| لا توجد مخاطر فقدان بيانات | **PASS** | العمود nullable بدون قيمة افتراضية، لا يُ affect أي بيانات موجودة ✅ |

### 2.3 محرك المزامنة (Sync Engine)

| المعيار | النتيجة | التفاصيل |
|---|---|---|
| `BuildOracleQuery` — الحالة الأولى: مزامنة أولى مع `InitialSyncStartDate` | **PASS** | `lastSyncAt` = null → `effectiveStartDate` = `InitialSyncStartDate` → WHERE clause يُضاف ✅ |
| `BuildOracleQuery` — الحالة الثانية: مزامنة أولى بدون `InitialSyncStartDate` | **PASS** | كلاهما null → لا WHERE clause → يجلب كل شيء ✅ |
| `BuildOracleQuery` — الحالة الثالثة: مزامنة تالية | **PASS** | `lastSyncAt` != null → `effectiveStartDate` = `lastSyncAt` → WHERE clause بالـ watermark ✅ |
| `LoadTableIncrementalAsync` — deduplication | **CAUTION-01** | انظر التفصيل أدناه |
| `LoadMappingsFromDbAsync` — يحمل `InitialSyncStartDate` | **PASS** | سطر 757: موجود في SELECT، سطور 780-782: معالجة null صحيحة ✅ |
| `LoadMappingsByIdsAsync` — يحمل `InitialSyncStartDate` | **PASS** | سطر 840: موجود في SELECT، سطور 866-868: معالجة null صحيحة ✅ |

### 2.4 طبقة API

| المعيار | النتيجة | التفاصيل |
|---|---|---|
| `CreateMappingRequest` يتضمن `InitialSyncStartDate` | **PASS** | سطر 405 في Controller ✅ |
| `GetAll` يُعيده | **PASS** | سطر 53 في SELECT، سطور 82-84 معالجة null ✅ |
| `GetActive` يُعيده | **PASS** | سطر 117 في SELECT، سطور 136-138 معالجة null ✅ |
| `CreateMapping` يحفظه | **PASS** | سطر 196 في INSERT، سطر 205 parameter مع null handling ✅ |
| `UpdateMapping` يحفظه | **PASS** | سطر 264 في UPDATE، سطر 274 parameter مع null handling ✅ |
| `OnGetMapping` يُعيده | **FLAG-02** | مفقود من projection في سطر 87-100. لا يُ affect Wizard (يستخدم Razor inline)، لكنه غير متسق |

### 2.5 طبقة واجهة المستخدم (UI)

| المعيار | النتيجة | التفاصيل |
|---|---|---|
| Step 5 — حقل التاريخ موجود | **PASS** | سطور 960-964 في cshtml: `<input type="date" id="wm-start-date">` ✅ |
| يُخفي الحقل في وضع Full | **PASS** | `style="display:none"` افتراضياً، JS يُخفيه عند Full ✅ |
| يُظهر الحقل في وضع Incremental | **PASS** | `setSyncMode()` يُظهره عند Incremental ✅ |
| `BindProperty` في CodeBehind | **PASS** | سطر 62: `public DateTime? InitialSyncStartDate { get; set; }` ✅ |
| `OnPostAddAsync` يحفظه | **PASS** | سطر 180: `InitialSyncStartDate = InitialSyncStartDate` ✅ |
| `OnPostEditAsync` يحفظه | **PASS** | سطر 251: `mapping.InitialSyncStartDate = InitialSyncStartDate` ✅ |
| JS `save()` يُزامن الحقل | **PASS** | سطور 1399-1402: ينسخ القيمة إلى hidden field ✅ |
| JS `bootstrapEditMode` يحمّله | **PASS** | سطور 137-138: يملأ الحقل من بيانات التعديل ✅ |
| JS `openWizardForEdit` يستقبله | **PASS** | سطر 1138: parameter الثامن ✅ |
| Razor `openWizardForEdit` يُمرره | **PASS** | سطر 644 في cshtml: `@JsonSerializer.Serialize(m.InitialSyncStartDate?.ToString("yyyy-MM-dd") ?? "")` ✅ |
| JS `resetState` يُعيد الضبط | **PASS** | سطور 1495-1498: يُخفي المجموعة ويُفرغ الحقل ✅ |
| Hidden form field موجود | **PASS** | سطر 1001: `<input type="hidden" name="InitialSyncStartDate" id="wm-h-initialSyncStartDate">` ✅ |

---

## 3. الإيجاد التفصيلي

### CAUTION-01: Deduplication بدون معاملة (Transaction)

| الحقل | القيمة |
|---|---|
| **Finding ID** | CAUTION-01 |
| **Rule ID** | QG-DB-002 (عدم تناسق المعاملات في عمليات حذف+إدراج) |
| **Domain** | Data Integrity |
| **Severity** | CAUTION |
| **Location** | `SqlServerLoadService.cs` — `LoadTableIncrementalAsync()`، سطور 134-185 |
| **Evidence** | عملية الـ DELETE (سطر 157-161) تتم خارج المعاملة. SqlBulkCopy (سطر 174-185) يستخدم `new SqlBulkCopy(connection)` بدون معاملة. إذا فشل الـ INSERT بعد الـ DELETE، تُفقد البيانات في نطاق التاريخ المحدد. |
| **Expected Standard** | `LoadTableAsync()` (المزامنة الكاملة) يستخدم معاملة صريحة: `connection.BeginTransaction()` + `SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)` + `transaction.Commit()` مع `Rollback` عند الفشل. المتوقع أن تكون `LoadTableIncrementalAsync` متسقة مع هذا النمط. |
| **Observed Condition** | لا توجد معاملة. الـ DELETE ينفّذ مباشرة على الاتصال، ثم SqlBulkCopy بدون transaction. في حالة فشل الـ BulkCopy (network error, timeout, constraint violation)، الصفوف المحذوفة لا تُستعاد. |
| **Impact** | فقدان بيانات في سيناريو فشل المزامنة التزايدية. على الرغم من أن فشل BulkCopy نادر، إلا أن المخاطرة حقيقية خاصة مع جداول كبيرة أو اتصال غير مستقر. |
| **Recommended Action** | لفّ كود DELETE + SqlBulkCopy في معاملة واحدة مماثلة لـ `LoadTableAsync()`. استخدم `connection.BeginTransaction()` ومرّر المعاملة إلى `SqlBulkCopy` و `SqlCommand`. |
| **Changed Code / Baseline** | جديد في هذا التغيير (الكود القديم كان INSERT فقط بدون DELETE) |
| **Confidence** | High |
| **Blocking** | Yes — البيانات المفقودة لا يمكن استعادتها تلقائياً |
| **Blocking Reason** | الخطر على سلامة البيانات في سيناريو الفشل |
| **Waiver Allowed** | Yes — يمكن التنازل إذا وافق Majed مع توثيق المخاطرة |
| **Required Owner** | EngineeringAgent |
| **Referral** | لا — ضمن نطاق المدقق |
| **Status** | Open |

---

### CAUTION-02: أعمدة String لا تُعالَج في Deduplication

| الحقل | القيمة |
|---|---|
| **Finding ID** | CAUTION-02 |
| **Rule ID** | Heuristic — كود مضلل |
| **Domain** | Code Correctness |
| **Severity** | CAUTION |
| **Location** | `SqlServerLoadService.cs` — `LoadTableIncrementalAsync()`، سطر 139 |
| **Evidence** | الشرط: `if (col.DataType == typeof(DateTime) \|\| col.DataType == typeof(string))` — يشمل أعمدة String. لكن الكود الداخلي (سطر 146) يتحقق فقط من `if (col.DataType == typeof(DateTime))`. أعمدة String (مثل تاريخ بصيغة نصية "2024-01-01") لن تحصل على dedup أبداً لأن `Compute("MIN")` لا يُستدعى لها. |
| **Expected Standard** | إما: (أ) إزالة `typeof(string)` من الشرط الخارجي إذا لم تكن مدعومة، أو (ب) إضافة معالجة للـ string date columns. |
| **Observed Condition** | الشرط يشمل String لكن التنفيذ لا يعالجها. الكود مضلل وقد يُسبب ثقة زائدة بأن أعمدة التاريخ النصية محمية بالـ dedup. |
| **Impact** | منطقي: لا تأثير وظيفي مباشر لأن أعمدة Oracle التاريخية عادة ما تكون `DateTime` وليس `string`. لكن الكود المضلل يُ صعّب الصيانة وقد يُسبب أخطاء مستقبلية. |
| **Recommended Action** | نظّف الشرط ليكون فقط `typeof(DateTime)` أو أضف معالجة صريحة للأعمدة النصية مع تحذير. |
| **Changed Code / Baseline** | جديد |
| **Confidence** | High |
| **Blocking** | No — لا يُسبب فقدان بيانات في الاستخدام الحالي |
| **Blocking Reason** | N/A |
| **Waiver Allowed** | Yes |
| **Required Owner** | EngineeringAgent |
| **Referral** | لا |
| **Status** | Open |

---

### FLAG-01: `[MaxLength(50)]` مفقود على `InitialSyncStartDate`

| الحقل | القيمة |
|---|---|
| **Finding ID** | FLAG-01 |
| **Rule ID** | Spec deviation |
| **Domain** | Spec compliance |
| **Severity** | FLAG |
| **Location** | `TableMappingConfig.cs`، سطر 79 |
| **Evidence** | ملف المهمة يحدد `[MaxLength(50)]` على الخاصية. لم يُضاف. |
| **Expected Standard** | التزام بالمواصفة كما هي مكتوبة. |
| **Observed Condition** | `[MaxLength(50)]` غير موجود. |
| **Impact** | لا تأثير وظيفي — `MaxLength` على `DateTime?` غير معتاد و لا يُنفذه EF Core على datetime2. المواصفة نفسها غير دقيقة تقنياً. |
| **Recommended Action** | تحديث مواصفة المهمة لحذف `[MaxLength(50)]` لأنه غير مناسب لـ DateTime?. أو إضافته إذا كان الهدف هو التقييد النصي (غير معتاد). |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | N/A (spec issue) |
| **Status** | Open |

---

### FLAG-02: `OnGetMapping` لا يُعي `InitialSyncStartDate`

| الحقل | القيمة |
|---|---|
| **Finding ID** | FLAG-02 |
| **Rule ID** | API consistency |
| **Domain** | API Completeness |
| **Severity** | FLAG |
| **Location** | `Index.cshtml.cs`، `OnGetMapping()` سطور 87-100 |
| **Evidence** | الـ projection لا يتضمن `InitialSyncStartDate`. |

```
.Select(m => new
{
    editId = m.Id,
    name = m.Name,
    oracleSource = m.OracleSource,
    sourceType = m.SourceType,
    sqlTargetTable = m.SqlTargetTable
})
```

| **Expected Standard** | الـ API يُعيد جميع الحقول المُعرّفة في النموذج، خاصة أن `GetAll` و `GetActive` في الـ API يُعيده. |
| **Observed Condition** | مفقود. |
| **Impact** | منخفض — الـ Wizard edit flow يستخدم Razor inline serialization (سطر 644 في cshtml) وليس هذا الـ endpoint. لكن أي مستخدم للـ endpoint سيلاحظ الغياب. |
| **Recommended Action** | أضف `initialSyncStartDate = m.InitialSyncStartDate` إلى projection. |
| **Confidence** | High |
| **Blocking** | No |
| **Waiver Allowed** | Yes |
| **Required Owner** | EngineeringAgent |
| **Status** | Open |

---

## 4. ملخص معايير القبول

| المعيار | الحالة |
|---|---|
| **Model Layer** | ✅ مُنجز (عدا FLAG-01 البسيط) |
| **Migration** | ✅ مُنجز |
| **BuildOracleQuery — 3 حالات** | ✅ مُنجز وصحيح |
| **LoadTableIncrementalAsync — Dedup** | ⚠️ CAUTION — يفتقر لمعاملة |
| **LoadMappingsFromDbAsync** | ✅ مُنجز |
| **API Create/Update/GetAll/GetActive** | ✅ مُنجز |
| **UI Step 5 — حقل التاريخ** | ✅ مُنجز |
| **UI — إخفاء/إظهار حسب الوضع** | ✅ مُنجز |
| **UI — حفظ وتحميل في وضع التعديل** | ✅ مُنجز |
| **dotnet build** | لم يُفحص (خارج نطاق المدقق — يُطلب من QA) |

---

## 5. المخاطر

| المخاطرة | الخطورة | التوضيح |
|---|---|---|
| فقدان بيانات عند فشل BulkCopy في Incremental | **متوسطة** | DELETE بدون transaction يعني أن فشل الإدراج لا يُستعاد. |
| Dedup على عمود التاريخ الخطأ | **منخفضة** | يعتمد على أول DateTime column في الـ DataTable. قد لا يكون العمود المستخدم في watermarking. |
| Oracle TIMESTAMP literal timezone | **منخفضة** | مقبول لأداة الإدارة (مذكور في مواصفة المهمة). |

---

## 6. التوصيات

1. **[CAUTION-01] ضروري**: لفّ DELETE + SqlBulkCopy في معاملة واحدة في `LoadTableIncrementalAsync`. انسخ نمط `LoadTableAsync`:
   ```csharp
   using var transaction = connection.BeginTransaction();
   // DELETE مع transaction
   // SqlBulkCopy مع transaction
   // Commit
   // Rollback عند الخطأ
   ```

2. **[CAUTION-02] مُوصى**: نظّف شرط `typeof(string)` في حلقة الـ dedup ليكون واضحًا.

3. **[FLAG-02] مُوصى**: أضف `InitialSyncStartDate` إلى `OnGetMapping` projection للاتساق.

---

## 7. Handback to Orchestrator

```
Status: NEEDS_FIX
Report Path: project-control/audit-reports/QUAUD-TASK-SYNC-INC-01-2026-07-21-028.md
Blocking Findings:
  - CAUTION-01: LoadTableIncrementalAsync缺乏transaction — DELETE + INSERT غير محميين بمعاملة
Recommended Next Action:
  1. إضافة معاملة (Transaction) إلى LoadTableIncrementalAsync لحماية سلامة البيانات
  2. إعادة مراجعة الملف بعد التعديل
  3. (اختياري) تنظيف شرط typeof/string في dedup
  4. (اختياري) إضافة InitialSyncStartDate إلى OnGetMapping projection
```
