# TASK-COD-KPI-001 — Rename OracleTable → SqlTable in Card Builder

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-KPI-001 |
| **المجموعة** | KPI |
| **النوع** | UI + Backend — Rename + Text Fix |
| **الوكيل** | engineering-agent |
| **الأولوية** | Critical |
| **الحالة** | ✅ Accepted |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

في **الخطوة الثانية** من معالج إنشاء البطاقات (Card Builder)، يوجد نوع مصدر بيانات اسمه:

```html
<option value="OracleTable">جدول/عرض Oracle</option>
```

**هذا خاطئ** لأن:
- البيانات المُrence她在 SQL Server (بعد المزامنة من Oracle)
- الجداول والـ Views المتاحة في `/api/tablemappings/active` هي جداول SQL Server
- المستخدم يتفاعل مع SQL Server مباشرة في هذه الخطوة

### الملفات المتأثرة (34 مطابقة في 5 ملفات)

| الملف | عدد المطابقات | النوع |
|---|---|---|
| `Builder.cshtml` | 6 | HTML/UI |
| `Builder.cshtml.cs` | 7 | C# Backend |
| `card-builder.js` | 13 | JavaScript |
| `CardEditorInput.cs` | 0 | — |
| `DashboardCard.cs` | 0 | — |

---

## 2. الهدف

إعادة تسمية `OracleTable` إلى `SqlTable` في كل مكان مع تحديث النصوص العربية.

---

## 3. النطاق

### المطلوب

- [x] **A. `Builder.cshtml`** (6 تغييرات):
  - `value="OracleTable"` → `value="SqlTable"`
  - `جدول/عرض Oracle` → `جدول/عرض SQL Server`
  - `id="wb-oracle-table"` → `id="wb-sql-table"`
  - `for="wb-oracle-table"` → `for="wb-sql-table"`
  - `aria-describedby="wb-oracle-table-hint"` → `aria-describedby="wb-sql-table-hint"`
  - `id="wb-oracle-table-hint"` → `id="wb-sql-table-hint"`
  - `data-source="OracleTable"` → `data-source="SqlTable"`
  - `id="wb-panel-oracletable"` → `id="wb-panel-sqltable"`
  - `جاري تحميل الجداول النشطة...` → `جاري تحميل جداول SQL Server...`

- [x] **B. `Builder.cshtml.cs`** (7 تغييرات):
  - `TemplateCategory.Id = "OracleTable"` → `"SqlTable"`
  - `Name = "جداول Oracle"` → `Name = "جداول SQL Server"`
  - `Description = "جداول وعروض قاعدة البيانات المتاحة"` → `"جداول وعروض SQL Server المتاحة"`
  - `OracleTables` property → `SqlTables` (rename property)
  - `LoadOracleTablesAsync()` → `LoadSqlTablesAsync()` (rename method)
  - XML comments updates

- [x] **C. `card-builder.js`** (13 تغييرة):
  - `case 'OracleTable'` → `case 'SqlTable'`
  - `this.state.sourceType === 'OracleTable'` → `this.state.sourceType === 'SqlTable'`
  - `$('wb-oracle-table')` → `$('wb-sql-table')`
  - `$('wb-oracle-table-hint')` → `$('wb-sql-table-hint')`
  - `populateOracleTableSelect` → `populateSqlTableSelect`
  - `applyOracleTable` → `applySqlTable`
  - All Arabic text updates
  - Comment updates

- [x] **D. التأكد من عمل `dotnet build -c Release` بدون أخطاء**

### غير المطلوب
- لا تغيير في `OracleSchemaService.cs` — هذا خدمة حقيقية تتصل بـ Oracle
- لا تغيير في `SchemaManagementService.cs` — هذا خدمة إدارة المخطط
- لا تغيير في `ListObjects.cshtml.cs` — هذا API استعلام Oracle
- لا تغيير في `DashboardCard.cs` Model
- لا تغيير في EF Migrations

---

## 4. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-COD-KPI-001.md
```

---

## 5. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | لا يوجد `OracleTable` في `Builder.cshtml` | ✅ |
| AC-2 | لا يوجد `OracleTable` في `Builder.cshtml.cs` | ✅ |
| AC-3 | لا يوجد `OracleTable` في `card-builder.js` | ✅ |
| AC-4 | `SqlTable` يظهر في واجهة المستخدم بالعربية | ✅ |
| AC-5 | `dotnet build -c Release` ينجح بدون أخطاء | ✅ |
| AC-6 | لا تغيير في خدمات Oracle الحقيقية | ✅ |
| AC-7 | لا توجد أسرار في الملفات المعدلة | ✅ |

---

## 6. Pre-Execution Gate Result

**Result:** PASS

- Active Technology Profile: `dotnet-razorpages-adonet`
- Smallest safe executable unit: Yes — rename فقط
- Single goal: Yes — تصحيح تسمية المصدر
- UI task: No (text changes only)
- Security sensitivity: Low
- Database impact: None
- Secrets handling: N/A

---

## 7. Delegation Notes

- **تنبيه مهم:** `OracleSchemaService.cs` و `SchemaManagementService.cs` و `ListObjects.cshtml.cs` **لا يُلمسون** — هذه خدمات حقيقية تتصل بـ Oracle
- **التغيير فقط** في ملفات Card Builder الثلاثة
- **الحفاظ على الاتساق:** كل `OracleTable` في JS يجب أن يتطابق مع `SqlTable` في HTML و C#
- **Handback:** سجّل كل ملف معدّل + نتائج Build

---

## 8. Engineering Handback

**Status:** DONE

### الملفات المعدلة (3 ملفات)

| # | الملف | التعديل |
|---|---|---|
| 1 | `Pages/admin-secure-panel/Cards/Builder.cshtml` | 5 edits — option value/label, panel attributes, label, select, hint |
| 2 | `Pages/admin-secure-panel/Cards Builder.cshtml.cs` | 8 edits — comment, property, template category, method calls, assignments |
| 3 | `wwwroot/js/card-builder.js` | 7 bulk replaceAll edits — all OracleTable/wb-oracle-table/function names |

### نتائج Build

```
dotnet build -c Release
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### ملاحظات
- لا يوجد أي `OracleTable` في ملفات Card Builder بعد التغيير
- `SqlTable` يظهر بشكل صحيح في كل مكان
- خدمات Oracle الحقيقية لم تتأثر

### Secrets Check
✅ لا توجد أسرار في الملفات المعدلة.

---

## 9. Tera Post-Execution Review

**Result:** ✅ PASS / Accepted

- **Changed files reviewed:** Yes — all 3 files verified
- **Allowed Write Targets respected:** Yes
- **Scope respected:** Yes — rename only, no logic changes
- **Secrets written:** No
- **Build verification:** `dotnet build -c Release` — succeeded, 0 warnings, 0 errors (run by Tera)
- **Grep verification:** No `OracleTable` found in Card Builder files
- **AC-1 through AC-7:** All PASS
