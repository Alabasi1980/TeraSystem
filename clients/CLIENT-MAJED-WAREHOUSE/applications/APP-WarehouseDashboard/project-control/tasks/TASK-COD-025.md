# TASK-COD-025 — Dynamic Table Mappings (إدارة تعيينات الجداول ديناميكياً)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-COD-025 |
| **المجموعة** | B9 — Dynamic Mappings |
| **المرحلة** | Phase F — Deployment (Enhancement) |
| **الوكيل** | engineering-agent |
| **التقدير** | 20–30h |
| **التبعية** | ALL (B1-B8 + B7 ✅) |
| **الأولوية** | Critical |
| **الحالة** | 🟡 Assigned |

---

## 1. الهدف

تحويل تعيينات التزامن من ملف ثابت (`appsettings.json`) إلى **نظام ديناميكي** يُدار من لوحة الإدارة. يُنشئ الجداول تلقائياً في SQL Server، ويُعطل التزامن دون حذف البيانات، ويُظهر تنبيهاً واضحاً قبل أي تعديل هيكلية.

## 2. المبدأ الأساسي

```
قاعدة بيانات واحدة: WarehouseDashboard (SQL Server)
├── جداول الإعدادات (موجودة)
│   ├── DashboardCards
│   ├── CardDrillDownLevels
│   ├── SyncSettings
│   ├── AdminPassword
│   └── TableMappings ← NEW
│
└── جداول البيانات (تُنشأ تلقائياً عند إضافة تزامن)
    ├── stg_WarehouseStock
    ├── stg_Items
    └── ... أي جدول جديد
```

## 3. المتطلبات الوظيفية

### 3.1 إضافة تزامن جديد
```
Admin يختار جدول من Oracle (أو يُدخل اسم يدوياً)
    ↓
النظام يقرأ هيكل الجدول من Oracle تلقائياً
    ↓
يُنشئ الجدول في SQL Server تلقائياً
    ↓
يحفظ التعيين في جدول TableMappings
    ↓
يبدأ التزامن
```

### 3.2 تعطيل التزامن (بدون حذف)
```
Admin يُعطّل تزامن جدول معين
    ↓
IsActive = false
    ↓
SyncEngine يتجاوز هذا الجدول
    ↓
البيانات تبقى في الجدول
    ↓
يمكن إعادة التفعيل لاحقاً
```

### 3.3 تعديل هيكل التزامن
```
Admin يعدّل مصدر Oracle أو أعمدة
    ↓
النظام يقرأ الهيكل الجديد من Oracle
    ↓
يقارنه بالهيكل الحالي في SQL Server
    ↓
إذا وُجدت تغييرات:
    ↓
┌─────────────────────────────────────────────┐
│  ⚠️ تنبيه: سيتم تعديل هيكل الجدول           │
│                                               │
│  📋 ملخص التغييرات:                           │
│                                               │
│  🗑️ حذف أعمدة (مع بياناتها):                 │
│     • OLD_COL_1                               │
│     • OLD_COL_2                               │
│                                               │
│  ➕ إضافة أعمدة جديدة:                        │
│     • NEW_COL_1 (VARCHAR2 → NVARCHAR)        │
│                                               │
│  ✏️ تعديل نوع أعمدة:                          │
│     • CHANGED_COL (NUMBER → DECIMAL)         │
│                                               │
│  ⚠️ تحذير: حذف الأعمدة نهائي                 │
│                                               │
│         [تأكيد التعديل]    [إلغاء]            │
└─────────────────────────────────────────────┘
```

## 4. المكونات المطلوبة

### 4.1 نموذج البيانات — `TableMappingConfig`
```
文件: WarehouseDashboard.Web/Models/TableMappingConfig.cs

Properties:
- Id (int, PK, Auto)
- OracleSource (string) — e.g., "NATEJSOFT.WAREHOUSE_STOCK"
- SourceType (string) — "Table" | "View" | "Query"
- SqlTargetTable (string) — e.g., "stg_WarehouseStock"
- IsActive (bool, default true)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- LastSyncAt (DateTime?)
- SyncRecordCount (int)
- ErrorMessage (string?) — آخر خطأ
```

### 4.2 EF Core Migration
```
文件: WarehouseDashboardDbContext.cs — أضف DbSet<TableMappingConfig>
 Migration: dotnet ef migrations add AddTableMappings
```

### 4.3 صفحة إدارية — `TableMappings/Index.cshtml`
```
文件: Pages/admin-secure-panel/TableMappings/Index.cshtml + .cshtml.cs

الوظائف:
- عرض كل التعيينات في جدول (Syncfusion Grid)
- إضافة جديد → نموذج يفتح Modal
- تعديل → نموذج يفتح Modal + تنبيه هيكل
- تبديل تفعيل/تعطيل (Toggle)
- لا يوجد حذف — فقط تعطيل
```

### 4.4 خدمة كشف هيكل Oracle — `OracleSchemaService`
```
文件: WarehouseDashboard.Web/Services/OracleSchemaService.cs

الوظائف:
- GetTableColumnsAsync(oracleSource) → List<ColumnInfo>
  تقرأ من ALL_TAB_COLUMNS في Oracle
  تُرجع: ColumnName, DataType, DataLength, Nullable

- GetSqlServerTableColumnsAsync(targetTable) → List<ColumnInfo>
  تقرأ من INFORMATION_SCHEMA.COLUMNS في SQL Server

- CompareSchemasAsync(oracleSource, targetTable) → SchemaDiff
  تقارن الهيكلاين وترجع:
  - ColumnsToAdd (new columns from Oracle)
  - ColumnsToRemove (columns in SQL but not in Oracle)
  - ColumnsToModify (type changes)
```

### 4.5 خدمة مقارنة الهيكلاين — `SchemaDiffService`
```
文件: WarehouseDashboard.Web/Services/SchemaDiffService.cs

الوظائف:
- Compare(oracleColumns, sqlColumns) → SchemaDiffResult
- GenerateAlterStatements(diff) → List<string> (SQL ALTER statements)
- ApplySchemaChanges(connection, diff) → executes ALTERs
```

### 4.6 تعديل SyncEngineService
```
文件: WarehouseDashboard.Api/Services/SyncEngineService.cs

التغييرات:
- يقرأ التعيينات من DB بدلاً من config
- يتجاوز التعيينات غير الفعّالة (IsActive = false)
- لا يُنشئ جداول تلقائياً (الإنشاء من Admin UI)
```

### 4.7 تعديل SqlServerLoadService
```
文件: WarehouseDashboard.Api/Services/SqlServerLoadService.cs

التغييرات:
- EnsureTableExistsAsync: يبقى كما هو (للأمان)
- لا يحتاج تعديل كبير
```

## 5. Allowed Write Targets

```
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Models/TableMappingConfig.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Data/WarehouseDashboardDbContext.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Migrations/
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Services/OracleSchemaService.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Services/SchemaDiffService.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Pages/admin-secure-panel/TableMappings/
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Web/Program.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/Services/SyncEngineService.cs
clients/CLIENT-MAJED-WAREHOUSE/applications/APP-WarehouseDashboard/src/WarehouseDashboard.Api/Program.cs
```

## 6. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | جدول `TableMappings` موجود في DB (EF Migration) | ⬜ |
| AC-2 | صفحة إدارية CRUD كاملة مع Syncfusion Grid | ⬜ |
| AC-3 | إضافة تزامن جديد يُنشئ الجدول في SQL Server تلقائياً | ⬜ |
| AC-4 | تعطيل التزامن (IsActive=false) يتجاوز الجدول | ⬜ |
| AC-5 | تعديل التزامن يُظهر تنبيه هيكل قبل التطبيق | ⬜ |
| AC-6 | حذف أعمدة محذوفة من Oracle يُنفَّذ فعلياً | ⬜ |
| AC-7 | SyncEngineService يقرأ من DB | ⬜ |
| AC-8 | لا إعادة تشغيل مطلوبة بعد التعديل | ⬜ |
| AC-9 | `dotnet build -c Release` = 0 errors | ⬜ |
| AC-10 | لا secrets hardcoded | ⬜ |

## 7. التدفق المفصّل

### 7.1 إضافة تزامن جديد
```
1. Admin يفتح /admin-secure-panel/TableMappings
2. يضغط "إضافة تعيين جديد"
3. يُدخل:
   - Oracle Source: NATEJSOFT.WAREHOUSE_STOCK
   - Source Type: Table
   - SQL Target: stg_WarehouseStock
4. النظام يقرأ هيكل Oracle تلقائياً (OracleSchemaService)
5. النظام يعرض معاينة: "سيتم إنشاء جدول بـ 15 عمود"
6. Admin يضغط "تأكيد"
7. النظام يُنشئ الجدول في SQL Server (CREATE TABLE)
8. النظام يحفظ التعيين في TableMappings
9. يظهر "تم الإنشاء بنجاح"
```

### 7.2 تعطيل تزامن
```
1. Admin يرى قائمة التعيينات
2. يُغيّر Toggle من "مفعّل" إلى "معطّل"
3. IsActive = false
4. SyncEngine يتجاوز هذا الجدول في الدورة القادمة
5. البيانات تبقى في الجدول
```

### 7.3 تعديل هيكل
```
1. Admin يضغط "تعديل" على تعيين موجود
2. يعدّل Oracle Source (مثلاً من WMS.STOCK إلى WMS.NEW_STOCK)
3. النظام يقرأ الهيكل الجديد من Oracle
4. يقارنه بالهيكل الحالي في SQL Server
5. يعرض تنبيه:
   - أعمدة محذوفة: 3 أعمدة (ستُحذف مع بياناتها)
   - أعمدة جديدة: 2 عمود (ستُضاف)
   - أعمدة متغيرة: 1 عمود (نوعه سيتغير)
6. Admin يضغط "تأكيد" أو "إلغاء"
7. إذا أكمل: يُنفَّذ ALTER TABLE statements
8. يُحدّث التعيين في DB
```

## 8. ملاحظات تقنية مهمة

### Oracle Type Mapping
| Oracle | SQL Server |
|---|---|
| VARCHAR2(n) | NVARCHAR(n) |
| NUMBER(p,0) | BIGINT |
| NUMBER(p,s) | DECIMAL(p,s) |
| DATE | DATETIME2 |
| TIMESTAMP | DATETIME2 |
| CLOB | NVARCHAR(MAX) |
| BLOB | VARBINARY(MAX) |

### أمان
- لا حذف تعيينات — فقط تعطيل
- تنبيه إلزامي قبل تعديل هيكل
- حذف أعمدة = حذف نهائي مع تحذير
- كل العمليات في transaction

### أداء
- SyncEngineService يقرأ من DB في كل دورة (لا cache)
- هذا يضمن التعديلات الفورية بدون إعادة تشغيل
