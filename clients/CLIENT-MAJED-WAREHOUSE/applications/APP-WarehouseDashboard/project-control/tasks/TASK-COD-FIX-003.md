# TASK-COD-FIX-003 — Fix Apply Schema SQL Server Identifier Quoting

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-FIX-003 |
| **المجموعة** | FIX |
| **النوع** | Backend / SQL DDL Bug Fix |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | ✅ Accepted — Build PASS; awaiting user Apply Schema retest |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

زر **تطبيق المخطط / Apply Schema** في TableMappings يفشل عند إنشاء جدول SQL Server من Oracle Query بخطأ:

```text
Incorrect syntax near 'NVARCHAR'.
```

تم عزل السبب إلى توليد SQL خاطئ في `SchemaManagementService.GenerateCreateTableSql()`:

```text
CREATE TABLE [[Items2]] (...)
```

الدالة `QuoteSqlServerIdentifier(tableName)` تُرجع اسمًا مقتبسًا مسبقًا مثل `[Items2]`، ثم يتم لفه مرة ثانية داخل أقواس مربعة في جملة `CREATE TABLE`.

## 2. الهدف

إصلاح توليد جملة `CREATE TABLE` بحيث يستخدم اسم الجدول المقتبس مرة واحدة فقط، مع الحفاظ على دعم `schema.table` مثل `dbo.Items2`.

## 3. النطاق

### المطلوب
- [x] تعديل توليد SQL في `SchemaManagementService.GenerateCreateTableSql()` لإزالة الاقتباس المزدوج لاسم الجدول.
- [x] التأكد أن `QuoteSqlServerIdentifier("Items2")` ينتج استخدامًا صحيحًا في SQL النهائي: `CREATE TABLE [Items2] (...)`.
- [x] التأكد أن `QuoteSqlServerIdentifier("dbo.Items2")` ينتج استخدامًا صحيحًا: `CREATE TABLE [dbo].[Items2] (...)`.
- [x] إن أمكن بدون تضخيم، إضافة/تحديث اختبار أو تحقق صغير يغطي SQL النهائي أو تشغيل Build على الأقل.
- [x] عدم تغيير منطق Oracle extraction أو wizard UI أو EF migrations.

### غير المطلوب
- لا تغيير في تصميم الواجهة.
- لا إضافة migrations.
- لا تغيير في جدول `TableMappings`.
- لا تغيير في بيانات الاتصال أو الأسرار.
- لا إنشاء جداول فعلية في قاعدة البيانات إلا إذا كان ذلك جزءًا من تحقق محلي واضح ومؤقت ومذكور في handback.

## 4. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Services\SchemaManagementService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-FIX-003.md
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\test-reports\
```

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | SQL النهائي لا يحتوي على `[[TableName]]` | ✅ |
| AC-2 | SQL النهائي يستخدم `[TableName]` أو `[schema].[table]` بشكل صحيح | ✅ |
| AC-3 | `dotnet build -c Release` ينجح بدون أخطاء | ✅ |
| AC-4 | Apply Schema لإنشاء جدول جديد لا يفشل بسبب `Incorrect syntax near 'NVARCHAR'` | ⚪ Not DB-tested; fixed generated SQL path |
| AC-5 | لا توجد أسرار أو connection strings حقيقية في handback أو ملفات التحكم | ✅ |

## 6. Pre-Execution Gate Result

**Result:** PASS

- Active Technology Profile: `dotnet-razorpages-adonet`
- Smallest safe executable unit: Yes
- Single goal: Yes — إصلاح اقتباس اسم الجدول في DDL فقط
- UI task: No
- Security sensitivity: Low — لا يمس auth/secrets/permissions
- Database impact: DDL generation fix only; no migration required
- Secrets handling: Must use `[REDACTED]` if referencing local connection strings

## 7. Delegation Notes

- Tera identified the root cause by reproducing `CREATE TABLE [[Items2]] (...)` directly against SQL Server; SQL Server returns the same `Incorrect syntax near 'NVARCHAR'` error.
- EngineeringAgent must keep the fix surgical.
- EngineeringAgent must report exact changed file(s), verification command(s), and whether Apply Schema was retested.

## 8. Engineering Handback

**Status:** DONE

**Files changed:**
- `src/WarehouseDashboard.Web/Services/SchemaManagementService.cs`
- `project-control/tasks/TASK-COD-FIX-003.md`

**Fix summary:**
- Changed `GenerateCreateTableSql()` to use the already-quoted result of `QuoteSqlServerIdentifier(tableName)` directly.
- Final shape is now `CREATE TABLE [Items2] (...)` or `CREATE TABLE [dbo].[Items2] (...)`, not `CREATE TABLE [[Items2]] (...)`.
- No Oracle extraction, wizard UI, EF migrations, config, or unrelated services changed.

**Verification:**
- `dotnet build -c Release` from `src` — succeeded, 0 warnings, 0 errors.
- Source shape check confirmed `CREATE TABLE {QuoteSqlServerIdentifier(tableName)}` and no extra outer square brackets in the create-table line.

**Apply Schema DB retest:**
- Not executed against a live database to avoid creating persistent DB objects.

**Secrets:**
- No secrets or live connection strings written.

**System/workflow gaps observed:**
- None.

## 9. Tera Post-Execution Review

**Result:** PASS / Accepted with live UI retest pending

- Changed files reviewed: Yes
- Allowed Write Targets respected: Yes
- Scope respected: Yes — one-line DDL identifier quoting fix only
- Secrets written: No
- Verification run by Tera: `dotnet build -c Release` from `src` — succeeded, 0 warnings, 0 errors
- Remaining validation: user should click **تطبيق / Apply Schema** again in the running Admin Panel to confirm the live workflow now creates the target table.
