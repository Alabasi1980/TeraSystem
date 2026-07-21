# TASK-REPORT-PARAM-01

## العنوان
إضافة عمودي `ValueColumn` و `TextColumn` إلى `ReportFilter` Model + Migration

## الهدف
إضافة حقلين جديدين إلى كيان `ReportFilter` لتخزين اسمي العمودين (قيمة ونص) اللذين سيُستخدما في نتائج كويري الباراميتر (`OptionsQuery`).

هذا يسمح لمنشئ التقارير بأن يكتب Query مثل:
```sql
SELECT ItemID AS Value, ItemName AS Text FROM Items
```
ثم يحدد `ValueColumn = "Value"` و `TextColumn = "Text"`.

## الملفات المطلوب تغييرها

### 1. Model — `WarehouseDashboard.Web/Models/ReportFilter.cs`

**المسار الكامل:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\ReportFilter.cs
```

**التغيير المطلوب:**
أضف خاصيتين جديدتين بعد السطر `public string? OptionsQuery { get; set; }`:

```csharp
[MaxLength(200)]
public string? ValueColumn { get; set; }

[MaxLength(200)]
public string? TextColumn { get; set; }
```

**تفسير:**
- `ValueColumn`: اسم العامود في نتيجة الـ Query الذي سيُستخدم كقيمة تُرسل للسيرفر عند الفلترة
- `TextColumn`: اسم العامود في نتيجة الـ Query الذي سيُعرض للمستخدم في القائمة المنسدلة

### 2. DbContext — `WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`

**المسار الكامل:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs
```

**التغيير المطلوب:**
أضف تهيئة للحقلين الجديدين داخل `modelBuilder.Entity<ReportFilter>` (بعد إعداد `OptionsQuery`):

```csharp
entity.Property(e => e.ValueColumn)
    .HasColumnType("nvarchar(200)")
    .IsRequired(false);

entity.Property(e => e.TextColumn)
    .HasColumnType("nvarchar(200)")
    .IsRequired(false);
```

**تفسير:**
نضيف تهيئة Fluent API للحقلين الجديدين لتكون متوافقة مع بقية الحقول في نفس الكيان.

### 3. Migration — إضافة Migration جديدة

**الأمر:**
```powershell
dotnet ef migrations add AddValueTextColumnsToReportFilter
```

**ملاحظة:** شغّل هذا الأمر فقط من المسار:
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web
```

**بعد إنشاء الـ Migration، تحقق أن الملف المُنشأ يحتوي فقط على:**
- `migrationBuilder.AddColumn<string>(name: "ValueColumn", ...)` على جدول `ReportFilters`
- `migrationBuilder.AddColumn<string>(name: "TextColumn", ...)` على جدول `ReportFilters`

**لا تقم بتشغيل `dotnet ef database update`.** هذه المهمة تنشئ الـ Migration فقط.

### 4. تحديث ملف المهمة

**المسار:**
```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-REPORT-PARAM-01.md
```

بعد الانتهاء، أضف قسم `## Post-Execution Review Result` في نهاية هذا الملف (باستخدام القالب الموجود في ملف المهمة).

## Allowed Write Targets
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Models/ReportFilter.cs`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs`
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Data/Migrations/` (الملف المُنشأ فقط)
- `clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/project-control/tasks/TASK-REPORT-PARAM-01.md`

## معايير القبول
1. ملف `ReportFilter.cs` يحتوي على الخاصيتين `ValueColumn` و `TextColumn` من نوع `string?`
2. ملف `WarehouseDashboardDbContext.cs` يحتوي على تهيئة للحقلين الجديدين
3. ملف Migration جديد يُسمى `AddValueTextColumnsToReportFilter.cs` موجود في مجلد `Migrations/`
4. الـ Migration يحتوي فقط على `AddColumn` للحقلين الجديدين ولا شيء آخر
5. `dotnet build` يمر بدون أخطاء
6. لا يوجد `dotnet ef database update` تم تشغيله
7. لا يوجد تعديل لأي ملف خارج `Allowed Write Targets`

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | إضافة حقلين + Migration فقط |
| One objective only | PASS | هدف واحد: إضافة ValueColumn و TextColumn |
| No deferrable work included | PASS | لا يوجد |
| No UI unless explicitly requested | PASS | لا يوجد UI |
| No API unless explicitly requested | PASS | لا يوجد API |
| No Auth unless explicitly requested | PASS | لا يوجد Auth |
| No schema/migration unless explicitly requested | PASS | Migration مطلوب صراحة |
| No real secrets outside approved local environment files | PASS | لا يوجد أسرار |
| Secret handling plan documented and redacted | PASS | لا ينطبق |
| CLI side effects checked | PASS | `dotnet ef migrations add` = ينشئ ملف Migration واحد فقط |
| No internal contradiction between constraints and outputs | PASS | لا يوجد تعارض |
| Allowed Write Targets are narrow | PASS | مقيدة بـ 4 مسارات محددة |
| Acceptance criteria are testable | PASS | `dotnet build` بدون أخطاء |

**Gate Status:** PASS
**Required Action:** تنفيذ التغييرات ورفع النتيجة

---

## إرشادات للمهندس

أهلاً بك في أول مهمة من سلسلة تحسين نظام الباراميترات في التقارير.

**ما المطلوب منك بالضبط؟**

تخيل أن المستخدم سينشئ تقريراً، وعند إضافة فلتر من نوع "قائمة منسدلة" (Dropdown)، يريد أن يكتب Query مثل:

```sql
SELECT WarehouseID AS Value, WarehouseName AS Text FROM Warehouses
```

حيث:
- `WarehouseID` = القيمة التي سترسل للسيرفر عند الفلترة
- `WarehouseName` = النص الذي سيراه المستخدم في القائمة

لكن حالياً الـ `ReportFilter` ليس عنده مكان يخزن فيه هذين الاسمين (`ValueColumn` و `TextColumn`). لذا مهمتك الأولى:
1. أضف الحقلين إلى الـ Model
2. أضف تهيئتهما في DbContext
3. أنشئ Migration جديدة (بدون تطبيقها)
4. تأكد أن `dotnet build` يشتغل

**ملاحظة مهمة:** لا تلمس أي ملف آخر غير المذكورين في `Allowed Write Targets`.

بعد الانتهاء، أضف قسم `Post-Execution Review Result` في نهاية هذا الملف (باستخدام القالب الموجود في نهاية ملف المهمة).

شكراً، وإذا احتجت توضيحاً إضافياً فأنا هنا.

---

## Post-Execution Review Result

| # | Acceptance Criterion | Result | Notes |
|---|---|---|---|
| 1 | `ReportFilter.cs` يحتوي على الخاصيتين `ValueColumn` و `TextColumn` من نوع `string?` | PASS | موجودتان في السطور 29-33 مع `[MaxLength(200)]` |
| 2 | `WarehouseDashboardDbContext.cs` يحتوي على تهيئة للحقلين الجديدين | PASS | تمت الإضافة في السطور 741-747 داخل `entity.Property` |
| 3 | ملف Migration جديد باسم `AddValueTextColumnsToReportFilter.cs` موجود في `Migrations/` | PASS | الملف: `20260721045735_AddValueTextColumnsToReportFilter.cs` |
| 4 | الـ Migration يحتوي فقط على `AddColumn` للحقلين الجديدين ولا شيء آخر | PASS | `Up()` يحتوي فقط على `AddColumn` لـ `ValueColumn` و `TextColumn` |
| 5 | `dotnet build` يمر بدون أخطاء | PASS | Build succeeded — 0 Warnings, 0 Errors |
| 6 | لم يتم تشغيل `dotnet ef database update` | PASS | لم يتم تشغيله — فقط `migrations add` |
| 7 | لا يوجد تعديل لأي ملف خارج `Allowed Write Targets` | PASS | تم التعديل فقط على المسارات الأربعة المسموح بها |

**Overall Result:** PASS ✓
**Date:** 2026-07-21
**Engineer:** Engineering Agent (Fallback)
