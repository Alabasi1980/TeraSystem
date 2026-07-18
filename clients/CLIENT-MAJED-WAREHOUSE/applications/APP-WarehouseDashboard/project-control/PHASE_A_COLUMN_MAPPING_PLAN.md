# Phase A — Column-Level Type Mapping Editor

> **تاريخ:** 2026-07-18  
> **الحالة:** بدأ التنفيذ — TASK-COD-COL-001 قيد التجهيز  
> **المراجع:** QUAUD-TABLEMAPPINGS-001 F-002 (STOP), F-003, F-004, F-005

---

## هيكل المهام

### TASK-COD-COL-001 — Data Model + Migration ✅ نبدأ بها الآن

**الهدف:** إنشاء جدول `ColumnMappings` مع العلاقة مع `TableMappingConfig`.

**الملفات الجديدة:**
- `Models/ColumnMapping.cs` — Entity
- Migration new file

**الملفات المعدلة:**
- `Data/WarehouseDashboardDbContext.cs` — إضافة DbSet + config
- `Models/TableMappingConfig.cs` — إضافة `ICollection<ColumnMapping>` navigation property

**المحتوى:**
| العمود | النوع | ملاحظة |
|---|---|---|
| Id | int (PK, auto) | |
| TableMappingConfigId | int (FK) | → TableMappings.Id, CASCADE delete |
| OracleColumnName | nvarchar(128) | اسم العمود في Oracle |
| SqlColumnName | nvarchar(128) | اسم العمود في SQL Server (قابل للتعديل، الافتراضي = Oracle) |
| SqlDataType | nvarchar(50) | نوع SQL Server (NVARCHAR, INT, BIGINT, DECIMAL...) |
| SqlMaxLength | int? | |
| SqlPrecision | int? | |
| SqlScale | int? | |
| IsNullable | bit | |
| IsExcluded | bit | هل يستبعد من المزامنة؟ |
| DefaultValue | nvarchar(500)? | |
| TransformationExpression | nvarchar(max)? | تحويل مستقبلاً |
| SortOrder | int | ترتيب الأعمدة |

---

### TASK-COD-COL-002 — UI: Column Mapping Editor داخل الـ Wizard

**الهدف:** إضافة محرر تفاعلي للأعمدة — إما ضمن Step 3 الحالي أو كـ Step 3.5.

**الميزات:**
- جدول تفاعلي لكل الأعمدة (قابل للتمرير)
- لكل عمود: SqlColumnName (input), SqlDataType (dropdown), precision/scale, nullable, excluded (checkbox), default value
- زر "إعادة تعيين إلى الاقتراح التلقائي"
- عرض Oracle Type + SQL Type الحالي
- زر "تطبيق الاقتراح التلقائي لجميع الأعمدة"

**الملفات المعدلة:**
- `Index.cshtml` — واجهة المحرر
- `table-mapping-wizard.js` — منطق المحرر

---

### TASK-COD-COL-003 — Backend + Schema Generation

**الهدف:**
1. إنشاء API لحفظ وتحميل `ColumnMappings` مع `TableMappingConfig`
2. تحديث `GenerateCreateTableSql()` لاستخدام التجاوزات
3. تحديث `GenerateAlterStatements()` لاستخدام التجاوزات
4. تحديث `CompareSchemasAsync()` لاستخدام الأنواع المعدلة

**الملفات المعدلة:**
- `OracleSchemaService.cs` — دالة `ApplyColumnOverrides()` 
- `SchemaManagementService.cs` — استخدام `ColumnMappings`
- `Index.cshtml.cs` — حفظ/تحميل الـ ColumnMappings
- `TableMappingController.cs` — API (اختياري)
