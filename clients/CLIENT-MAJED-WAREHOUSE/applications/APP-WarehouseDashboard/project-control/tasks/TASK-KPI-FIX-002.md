# TASK-KPI-FIX-002 — Rename OracleTable → SqlTable in Builder

| البند | القيمة |
|---|---|
| **المعرف** | TASK-KPI-FIX-002 |
| **المجموعة** | Card Builder Fix |
| **النوع** | UI + Backend Rename |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | Accepted |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

القائمة المنسدلة في الخطوة 2 (اختيار مصدر البيانات) تعرض `OracleTable` كقيمة و"جدول/عرض Oracle" كنص عربي. هذا يُضلّل المستخدم لأن:
- البيانات تأتي فعلياً من **SQL Server** (الجداول التي تم مزامنتها من Oracle)
- المستخدم يريد اختيار "SQL Server" كمصدر بيانات
- لا يوجد خيار `SqlTable` في القائمة

**النتيجة:** المستخدم لا يستطيع اختيار مصدر بيانات SQL Server لاختيار الجداول المزامنة.

---

## 2. الإصلاحات المطلوبة

### 2.1 Builder.cshtml — تغيير option value + نص عربي

```html
<!-- قبل -->
<option value="OracleTable">جدول/عرض Oracle</option>

<!-- بعد -->
<option value="SqlTable">جدول SQL Server (المزامن)</option>
```

وتحديث `data-source="OracleTable"` في Panel:
```html
<!-- قبل -->
<div class="wb-source-panel wd-hidden" data-source="OracleTable" id="wb-panel-oracletable">

<!-- بعد -->
<div class="wb-source-panel wd-hidden" data-source="SqlTable" id="wb-panel-sqltable">
```

وتحديث label:
```html
<!-- قبل -->
<label class="wd-field__label" for="wb-oracle-table">جدول أو عرض Oracle</label>

<!-- بعد -->
<label class="wd-field__label" for="wb-sql-table">جدول SQL Server</label>
```

وتحديث select id:
```html
<!-- قبل -->
<select id="wb-oracle-table" ...>

<!-- بعد -->
<select id="wb-sql-table" ...>
```

وتحديث hint:
```html
<!-- قبل -->
<span id="wb-oracle-table-hint" ...>اختر جدولاً لعرض أعمدة المصدر</span>

<!-- بعد -->
<span id="wb-sql-table-hint" ...>اختر جدولاً مزامناً من SQL Server</span>
```

### 2.2 Builder.cshtml.cs — تحديث TemplateCategories

```csharp
// قبل
new() { Id = "OracleTable", Name = "جداول Oracle", Icon = "server", Description = "جداول وعروض قاعدة البيانات المتاحة" },

// بعد
new() { Id = "SqlTable", Name = "جداول SQL Server", Icon = "server", Description = "جداول مزامنة من Oracle إلى SQL Server" },
```

### 2.3 card-builder.js — تحديث كل مرجع لـ OracleTable

البحث والاستبدال في card-builder.js:

| قبل | بعد |
|---|---|
| `'OracleTable'` | `'SqlTable'` |
| `wb-oracle-table` | `wb-sql-table` |
| `wb-oracle-table-hint` | `wb-sql-table-hint` |
| `wb-panel-oracletable` | `wb-panel-sqltable` |
| `'جدول/عرض Oracle'` | `'جدول SQL Server'` |
| `this.state.selectedTable.sqlTargetTable` | (لا يتغير — هذا اسم الجدول في SQL Server) |

---

## 3. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-KPI-FIX-002.md
```

---

## 4. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | القائمة المنسدلة تعرض `SqlTable` بدل `OracleTable` | ⬜ |
| AC-2 | النص العربي: "جدول SQL Server (المزامن)" | ⬜ |
| AC-3 | Panel يحتوي `data-source="SqlTable"` | ⬜ |
| AC-4 | جميع مراجع `OracleTable` في card-builder.js تغيرت إلى `SqlTable` | ⬜ |
| AC-5 | TemplateCategories تستخدم `SqlTable` | ⬜ |
| AC-6 | `dotnet build -c Release` ينجح بدون أخطاء | ⬜ |
| AC-7 | لا توجد أسرار في الملفات المعدلة | ⬜ |

---

## 5. ملاحظات تقنية

- الجداول المُحمّلة من `/api/tablemappings/active` تأتي من جدول `TableMappings` في SQL Server — هذه صحيحة.
- `Value = t.SqlTargetTable` في `LoadOracleTablesAsync()` — صحيح لأن `SqlTargetTable` هو اسم الجدول في SQL Server.
- الاسم الداخلي `selectedTable.sqlTargetTable` في JS — لا يتغير (هو فعلياً اسم SQL Server).
- التغيير هو **تجميلي + تسمية فقط** — لا يوجد تغيير في منطق البيانات.
