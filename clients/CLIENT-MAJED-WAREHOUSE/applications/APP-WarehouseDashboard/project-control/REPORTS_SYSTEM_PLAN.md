# REPORTS_SYSTEM_PLAN.md — نظام التقارير التفاعلي

> **المشروع:** WarehouseDashboard (CLIENT-MAJED-WAREHOUSE)
> **التاريخ:** 2026-07-20
> **الحالة:** Final Plan — Ready for Execution
> **المصادر:** OracleQueryLab pattern + AG Grid Community + blue-theme.css

---

## 1. نظرة عامة (Overview)

### 1.1 الهدف
إضافة صفحة تقارير تفاعلية للوحة الإدارة تعرض البيانات من Views في SQL Server بتصميم عصري واحترافي، مع شريط أدوات متقدم وفلاتر مزدوجة (Server-side + Client-side).

### 1.2 المعمارية

```
┌─ Admin Panel (Web :5000) ──────────────────────────────────────────┐
│                                                                     │
│  ┌─ Report Builder ───────┐   ┌─ Reports Page ──────────────────┐  │
│  │  (إنشاء/تعديل تقرير)   │   │  (عرض التقارير)                  │  │
│  │                        │   │                                  │  │
│  │  Step 1: اختر View     │   │  ┌────────┬───────────────────┐ │  │
│  │  Step 2: حدد الأعمدة   │   │  │        │ Report Filters     │ │  │
│  │  Step 3: حدد الفلاتر   │   │  │Sidebar │ (Server-Side)     │ │  │
│  │  Step 4: معاينة + حفظ  │   │  │ التقارر│───────────────────│ │  │
│  │                        │   │  │  ر     │ Advanced Toolbar   │ │  │
│  └────────────────────────┘   │  │        │───────────────────│ │  │
│                                │  │        │ AG Grid Table      │ │  │
│                                │  │        │ (Client-Side)      │ │  │
│                                │  └────────┴───────────────────┘ │  │
│                                └──────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
         │                                              │
         ▼                                              ▼
┌─ SQL Server ──────────────────────────────────────────────────────┐
│  Reports / ReportColumns / ReportFilters / ReportLayouts          │
│  + SQL Server Views (بيانات التقارير)                             │
└───────────────────────────────────────────────────────────────────┘
```

### 1.3 مبدأ الفلاتر المزدوجة

```
المستخدم يختار تقرير + يطبق فلاتر التقرير
        ↓
┌─────────────────────────────────┐
│  طبقة 1: Server-Side Filters   │  ← تُطبّق على SQL Server View
│  (Report-Level Filters)         │     عبر Parameterized Query
│  مثال: تاريخ من/إلى، قسم، سنة  │
└─────────────┬───────────────────┘
              ↓ بيانات مُصفّاة من SQL Server
┌─────────────────────────────────┐
│  طبقة 2: Client-Side Filters   │  ← AG Grid يطبّقها على البيانات
│  (Table-Level Filters)          │     المجلوبة في الذاكرة
│  بحث نصي، ترتيب، فلترة أعمدة   │
└─────────────────────────────────┘
```

---

## 2. قاعدة البيانات (Database Schema)

### 2.1 جدول التقارير — `Reports`

```sql
CREATE TABLE Reports (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(200) NOT NULL,          -- اسم التقرير بالعربي
    ViewName        NVARCHAR(200) NOT NULL,          -- اسم الـ View في SQL Server
    Description     NVARCHAR(500) NULL,              -- وصف مختصر
    Icon            NVARCHAR(50) NULL,               -- أيقونة (emoji أو CSS class)
    IsEnabled       BIT NOT NULL DEFAULT 1,          -- مفعّل/معطّل
    SortOrder       INT NOT NULL DEFAULT 0,          -- ترتيب العرض في الـ Sidebar
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

### 2.2 جدول أعمدة التقرير — `ReportColumns`

```sql
CREATE TABLE ReportColumns (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ReportId        INT NOT NULL,                    -- FK → Reports
    ColumnName      NVARCHAR(200) NOT NULL,          -- اسم العمود في الـ View
    DisplayName     NVARCHAR(200) NOT NULL,          -- اسم العرض بالعربي
    DataType        NVARCHAR(50) NOT NULL,           -- DataType الأصلي من الـ View
    Width           INT NULL DEFAULT 150,            -- عرض العمود بالبكسل
    IsVisible       BIT NOT NULL DEFAULT 1,          -- مرئي افتراضياً
    IsSortable      BIT NOT NULL DEFAULT 1,          -- قابل للترتيب
    IsFilterable    BIT NOT NULL DEFAULT 1,          -- قابل للفلترة
    IsImageColumn   BIT NOT NULL DEFAULT 0,          -- هل يحتوي على مسارات صور؟
    ImageBaseUrl    NVARCHAR(500) NULL,              -- Base URL لعرض الصور (filesystem)
    DateFormat      NVARCHAR(50) NULL,               -- تنسيق التاريخ (yyyy-MM-dd)
    NumberFormat    NVARCHAR(50) NULL,               -- تنسيق الأرقام (#,##0.00)
    SortOrder       INT NOT NULL DEFAULT 0,          -- ترتيب الأعمدة
    CONSTRAINT FK_ReportColumns_Reports FOREIGN KEY (ReportId)
        REFERENCES Reports(Id) ON DELETE CASCADE
);
```

### 2.3 جدول فلاتر التقرير — `ReportFilters`

```sql
CREATE TABLE ReportFilters (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ReportId        INT NOT NULL,                    -- FK → Reports
    ColumnName      NVARCHAR(200) NOT NULL,          -- اسم العمود في الـ View
    FilterType      NVARCHAR(50) NOT NULL,           -- Text | Date | DateRange | Dropdown | Number | NumberRange
    Label           NVARCHAR(200) NOT NULL,          -- اسم الفلتر بالعربي
    IsRequired      BIT NOT NULL DEFAULT 0,          -- إلزامي أم لا
    DefaultValue    NVARCHAR(500) NULL,              -- القيمة الافتراضية
    OptionsQuery    NVARCHAR(1000) NULL,             -- Query لجلب خيارات Dropdown (SELECT DISTINCT)
    Placeholder     NVARCHAR(200) NULL,              -- نص إرشادي
    SortOrder       INT NOT NULL DEFAULT 0,          -- ترتيب الفلاتر في الشريط
    CONSTRAINT FK_ReportFilters_Reports FOREIGN KEY (ReportId)
        REFERENCES Reports(Id) ON DELETE CASCADE
);
```

### 2.4 جدول تصميمات الجدول المحفوظة — `ReportLayouts`

```sql
CREATE TABLE ReportLayouts (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ReportId        INT NOT NULL,                    -- FK → Reports
    LayoutName      NVARCHAR(200) NOT NULL,          -- اسم التصميم
    IsDefault       BIT NOT NULL DEFAULT 0,          -- هل هو الافتراضي؟
    ColumnOrder     NVARCHAR(MAX) NULL,              -- JSON: ترتيب الأعمدة
    VisibleColumns  NVARCHAR(MAX) NULL,              -- JSON: الأعمدة المرئية
    ColumnWidths    NVARCHAR(MAX) NULL,              -- JSON: أعرض الأعمدة
    FilterValues    NVARCHAR(MAX) NULL,              -- JSON: قيم الفلاتر المحفوظة
    SortState       NVARCHAR(MAX) NULL,              -- JSON: حالة الترتيب
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ReportLayouts_Reports FOREIGN KEY (ReportId)
        REFERENCES Reports(Id) ON DELETE CASCADE
);
```

---

## 3. Backend Services

### 3.1 ReportService.cs — خدمات التقارير

**المسار:** `WarehouseDashboard.Web/Services/ReportService.cs`

#### 3.1.1 Report Discovery (جلب Views المتاحة)

```csharp
// جلب Views المتاحة من SQL Server
public async Task<List<ReportViewInfo>> GetAvailableViewsAsync(CancellationToken ct)

// جلب أعمدة View محدد (Schema Introspection)
public async Task<List<ColumnInfo>> GetViewColumnsAsync(string viewName, CancellationToken ct)
```

**المصادر:**
- `INFORMATION_SCHEMA.VIEWS` — لجلب Views المتاحة
- `INFORMATION_SCHEMA.COLUMNS` — لأعمدة كل View

#### 3.1.2 Report CRUD (إنشاء/تعديل/حذف)

```csharp
// إنشاء تقرير جديد
public async Task<int> CreateReportAsync(ReportDefinition report, CancellationToken ct)

// تعديل تقرير
public async Task UpdateReportAsync(int reportId, ReportDefinition report, CancellationToken ct)

// حذف تقرير
public async Task DeleteReportAsync(int reportId, CancellationToken ct)

// جلب جميع التقارير
public async Task<List<ReportListItem>> GetAllReportsAsync(CancellationToken ct)

// جلب تعريف تقرير واحد مع أعمدته وفلاتره
public async Task<ReportDefinition?> GetReportAsync(int reportId, CancellationToken ct)
```

#### 3.1.3 Report Data Execution (جلب بيانات التقرير)

```csharp
// تنفيذ تقرير مع فلاتر
public async Task<ReportDataResult> ExecuteReportAsync(
    int reportId,
    Dictionary<string, object?> filterValues,
    CancellationToken ct)
```

**المنطق:**
1. جلب تعريف التقرير (ViewName + Columns + Filters)
2. بناء SQL Query ديناميكي:
   ```sql
   SELECT [col1], [col2], [col3]
   FROM [ViewName]
   WHERE [filter1] = @filter1 AND [filter2] >= @filter2 AND [filter3] <= @filter3
   ```
3. تنفيذ الـ Query عبر ADO.NET (parameterized)
4. إرجاع Results (columns metadata + rows data)

#### 3.1.4 Filter Options (جلب خيارات الفلاتر)

```csharp
// جلب خيارات Dropdown filter من DB
public async Task<List<string>> GetFilterOptionsAsync(
    string viewName,
    string columnName,
    CancellationToken ct)
```

**المنطق:**
```sql
SELECT DISTINCT [columnName] FROM [viewName] ORDER BY [columnName]
```

### 3.2 ReportController.cs — API Endpoints

**المسار:** `WarehouseDashboard.Web/Pages/Api/Reports/`

| Endpoint | Method | الوظيفة |
|----------|--------|---------|
| `/api/reports` | GET | جلب جميع التقارير |
| `/api/reports/{id}` | GET | جلب تقرير واحد مع أعمدته وفلاتره |
| `/api/reports/views` | GET | جلب Views المتاحة من SQL Server |
| `/api/reports/views/{viewName}/columns` | GET | جلب أعمدة View محدد |
| `/api/reports` | POST | إنشاء تقرير جديد |
| `/api/reports/{id}` | PUT | تعديل تقرير |
| `/api/reports/{id}` | DELETE | حذف تقرير |
| `/api/reports/{id}/execute` | POST | تنفيذ تقرير مع فلاتر |
| `/api/reports/{id}/filters/{columnName}/options` | GET | جلب خيارات Dropdown |
| `/api/reports/{id}/layouts` | GET | جلب تصميمات محفوظة |
| `/api/reports/{id}/layouts` | POST | حفظ تصميم جديد |
| `/api/reports/{id}/layouts/{layoutId}` | DELETE | حذف تصميم |

---

## 4. Frontend — Report Builder (صفحة إنشاء التقارير)

### 4.1 المعالج (Wizard) — 4 خطوات

**المسار:** `/admin-secure-panel/ReportBuilder`
**الملف:** `Pages/admin-secure-panel/ReportBuilder/Index.cshtml`

#### الخطوة 1: اختيار الـ View
- جلب Views المتاحة من SQL Server
- عرض قائمة بحث + اختيار
- معاينة مختصرة لأعمدة الـ View

#### الخطوة 2: تكوين الأعمدة
- عرض جميع أعمدة الـ View المختار
- لكل عمود:
  - ☑ تفعيل/تعطيل (IsVisible)
  - ✏️ اسم العرض بالعربي (DisplayName)
  - 📏 العرض (Width)
  - 🔄 قابل للترتيب (IsSortable)
  - 🔍 قابل للفلترة (IsFilterable)
  - 🖼️ عمود صور (IsImageColumn)
  - 📅 تنسيق التاريخ (DateFormat)

#### الخطوة 3: تكوين الفلاتر
- إضافة فلتر جديد:
  - اختيار العمود من القائمة
  - اختيار النوع (Text/Date/DateRange/Dropdown/Number)
  - تحديد الاسم بالعربي
  - تحديد الإلزامية
  - القيمة الافتراضية
  - خيارات Dropdown (إذا كان النوع Dropdown)

#### الخطوة 4: معاينة + حفظ
- معاينة حية للتقرير (عرض أول 10 صفوف)
- زر حفظ
- الانتقال لصفحة التقرير

---

## 5. Frontend — Reports Page (صفحة عرض التقارير)

### 5.1 الهيكل العام

**المسار:** `/admin-secure-panel/Reports`
**الملف:** `Pages/admin-secure-panel/Reports/Index.cshtml`

```
┌─────────────────────────────────────────────────────────────────┐
│  Header: مختبِّر التقارير                                       │
├──────────┬──────────────────────────────────────────────────────┤
│          │  ┌──────────────────────────────────────────────┐    │
│  Sidebar │  │ Report Filters (Server-Side)                 │    │
│  (يمين)  │  │ [📅 من: ___] [📅 إلى: ___] [🔄 تطبيق]     │    │
│          │  ├──────────────────────────────────────────────┤    │
│  📊 مبيعات│ │ Advanced Toolbar                             │    │
│  📦 مخزون │ │ 🔍بحث│📥Excel│👁️أعمدة│💾حفظ│📂استرجاع│🔄reset│    │
│  🛒 مشتريات├──────────────────────────────────────────────┤    │
│  👥 موظفين│ │                                              │    │
│          │ │           AG GRID TABLE                       │    │
│          │ │                                              │    │
│          │ │  - فلاتر per-column (Client-Side)           │    │
│          │ │  - ترتيب تصاعدي/تنازلي                      │    │
│          │ │  - بحث نصي                                   │    │
│          │ │  - عرض صور (thumbnail → modal)               │    │
│          │ │  - Pagination                                 │    │
│          │ │                                              │    │
│          │  └──────────────────────────────────────────────┘    │
└──────────┴──────────────────────────────────────────────────────┘
```

### 5.2 شريط الأدوات المتقدم (Advanced Toolbar)

#### الأدوات الأساسية

| الأداة | الأيقونة | الوظيفة | التفاصيل |
|--------|---------|---------|----------|
| **بحث عام** | 🔍 | بحث نصي في كل الأعمدة | Search box يبحث فوراً |
| **تصدير Excel** | 📥 | تنزيل Excel | يحترم الفلاتر + الترتيب الحالي |
| **إظهار/إخفاء أعمدة** | 👁️ | إدارة مرئية الأعمدة | Dropdown بكل الأعمدة + toggle |
| **حفظ التصميم** | 💾 | حفظ التصميم الحالي | يحفظ: ترتيب + مرئية + عرض + فلاتر |
| **استرجاع التصميم** | 📂 | استرجاع تصميم محفوظ | Dropdown بالتصميمات المحفوظة |
| **إعادة ضبط** | 🔄 | إعادة للحالة الافتراضية | يزيل كل الفلاتر والترتيب |
| **ملء الشاشة** | ⛶ | توسيع الجدول | Full-screen mode للجدول |
| **طباعة** | 🖨️ | طباعة الجدول | Print-friendly format |

#### حفظ واسترجاع التصميم

```
┌─ تصميمات محفوظة ─────────────────────┐
│  ⭐ الافتراضي (built-in)              │
│  📋 تقرير المبيعات - نسخة 1          │
│  📋 تقرير المبيعات - نسخة 2          │
│  ➕ حفظ كتصميم جديد                   │
└──────────────────────────────────────┘
```

**البيانات المحفوظة في كل تصميم:**
- ترتيب الأعمدة (Column Order)
- الأعمدة المرئية (Visible Columns)
- أعرض الأعمدة (Column Widths)
- قيم الفلاتر المحفوظة (Filter Values)
- حالة الترتيب (Sort State)

### 5.3 فلاتر التقرير (Server-Side Filters)

 Filtration types المدعومة:

| النوع | الوظيفة | HTML Control |
|-------|---------|-------------|
| **Text** | بحث نصي | `<input type="text">` |
| **Date** | تاريخ واحد | `<input type="date">` |
| **DateRange** | نطاق تاريخ | `<input type="date">` من + إلى |
| **Dropdown** | قائمة منسدلة | `<select>` (خيارات من DB) |
| **Number** | رقم واحد | `<input type="number">` |
| **NumberRange** | نطاق أرقام | `<input type="number">` من + إلى |

**المنطق:**
1. المستخدم يختار قيمة الفلتر
2. يضغط "تطبيق" (أو Enter)
3. الـ API يبني Query مع WHERE clause
4. SQL Server يُصفّي البيانات
5. النتيجة تظهر في الجدول

### 5.4 AG Grid Table

**المكتبة:** AG Grid Community Edition (مجانية)

#### الإعدادات

```javascript
const gridOptions = {
    columnDefs: [...],           // dinamically from ReportColumns
    rowData: [...],              // from API response
    defaultColDef: {
        sortable: true,
        filter: true,            // Client-side filter per column
        resizable: true,
        floatingFilter: true,    // Quick filter row under header
    },
    pagination: true,
    paginationPageSize: 50,
    animateRows: true,
    direction: 'rtl',            // RTL support
    suppressRowClickSelection: true,
    enableCellTextSelection: true,
};
```

#### Custom Cell Renderers

| الحالة | Renderer |
|--------|----------|
| **عمود صور** | Thumbnail (40×40) → click → Modal معاينة |
| **أرقام** | تنسيق Arabic (#,##0.00) |
| **تواريخ** | تنسيق (yyyy-MM-dd) |
| **نص طويل** | Tooltip كامل + قص |

### 5.5 Image Preview Modal

```
┌─────────────────────────────────────────┐
│  ✕  معاينة الصورة                       │
├─────────────────────────────────────────┤
│                                         │
│         ┌─────────────────────┐         │
│         │                     │         │
│         │    الصورة الكاملة    │         │
│         │                     │         │
│         └─────────────────────┘         │
│                                         │
│  ◀  ▶   1 / 5                          │
│                                         │
│  📥 تنزيل   📋 نسخ الرابط              │
└─────────────────────────────────────────┘
```

### 5.6 Excel Export

**المكتبة:** AG Grid Community built-in Excel export أو SheetJS (xlsx)

**المنطق:**
1. جلب البيانات الحالية من AG Grid (بعد الفلاتر والترتيب)
2. تحويل لـ Excel format
3. تنزيل الملف

**الميزات:**
- يحترم الفلاتر الحالية (البيانات المُصفّاة فقط)
- يحترم ترتيب الأعمدة
- يحترم مرئية الأعمدة (الأعمدة المخفية لا تظهر)
- تنسيق الأرقام والتواريخ
- RTL support

---

## 6. التصميم البصري (Visual Design)

### 6.1 الألوان — من blue-theme.css

```css
/* Tokens الأساسية */
--c-primary: #2E6DA4;        /* أزرق رئيسي */
--c-primary-strong: #1F4E79;  /* أزرق غامق */
--c-surface: #FFFFFF;         /* خلفية البطاقات */
--c-surface-muted: #F0F4F8;   /* خلفية ثانوية */
--c-text: #1A2332;            /* نص أساسي */
--c-text-muted: #6B7A8D;      /* نص ثانوي */
--c-border: #D6E0EA;          /* حدود */
--c-success: #309E6A;         /* أخضر (نجاح) */
--c-error: #D64545;           /* أحمر (خطأ) */
--c-warning: #E8A838;         /* أصفر (تحذير) */
```

### 6.2 الـ Typography

```css
/* الخط */
--font-ar: 'Cairo', sans-serif;  /* خط عربي احترافي */

/* الأحجام */
.report-title { font-size: 22px; font-weight: 700; }
.report-subtitle { font-size: 14px; color: var(--c-text-muted); }
.table-header { font-size: 13px; font-weight: 600; }
.table-cell { font-size: 13px; }
.filter-label { font-size: 13px; font-weight: 600; }
```

### 6.3 الـ Spacing

```css
/* التباعد */
--sp-1: 4px;
--sp-2: 8px;
--sp-3: 12px;
--sp-4: 16px;
--sp-5: 20px;
--sp-6: 24px;
```

### 6.4 الأنيميشن

```css
/* دخول */
@keyframes wdFadeUp {
    from { opacity: 0; transform: translateY(10px); }
    to   { opacity: 1; transform: translateY(0); }
}

/* Shimmer للتحميل */
@keyframes wdShimmer {
    0% { background-position: 200% 0; }
    100% { background-position: -200% 0; }
}
```

### 6.5 RTL Support

```css
/* الصفحة بالكامل RTL */
.wd-reports-page { direction: rtl; }
.wd-sidebar { border-inline-start: none; border-inline-end: 1px solid var(--c-border); }
```

---

## 7. هيكل الملفات (File Structure)

```
WarehouseDashboard.Web/
├── Pages/
│   └── admin-secure-panel/
│       ├── Reports/
│       │   ├── Index.cshtml              # صفحة عرض التقارير الرئيسية
│       │   └── Index.cshtml.cs           # Code-behind
│       └── ReportBuilder/
│           ├── Index.cshtml              # معالج إنشاء التقارير
│           └── Index.cshtml.cs           # Code-behind
├── Pages/
│   └── Api/
│       └── Reports/
│           ├── List.cshtml               # GET /api/reports
│           ├── List.cshtml.cs
│           ├── Detail.cshtml             # GET /api/reports/{id}
│           ├── Detail.cshtml.cs
│           ├── Views.cshtml              # GET /api/reports/views
│           ├── Views.cshtml.cs
│           ├── ViewColumns.cshtml        # GET /api/reports/views/{viewName}/columns
│           ├── ViewColumns.cshtml.cs
│           ├── Create.cshtml             # POST /api/reports
│           ├── Create.cshtml.cs
│           ├── Update.cshtml             # PUT /api/reports/{id}
│           ├── Update.cshtml.cs
│           ├── Delete.cshtml             # DELETE /api/reports/{id}
│           ├── Delete.cshtml.cs
│           ├── Execute.cshtml            # POST /api/reports/{id}/execute
│           ├── Execute.cshtml.cs
│           ├── FilterOptions.cshtml      # GET /api/reports/{id}/filters/{columnName}/options
│           ├── FilterOptions.cshtml.cs
│           ├── Layouts.cshtml            # GET/POST /api/reports/{id}/layouts
│           └── Layouts.cshtml.cs
├── Services/
│   └── ReportService.cs                  # خدمة التقارير الرئيسية
├── Models/
│   ├── Report.cs                         # Entity
│   ├── ReportColumn.cs                   # Entity
│   ├── ReportFilter.cs                   # Entity
│   └── ReportLayout.cs                   # Entity
├── Data/
│   └── WarehouseDashboardDbContext.cs    # (تحديث — إضافة DbSets)
└── wwwroot/
    └── css/
        └── blue-theme.css                # (تحديث — إضافة Report styles)
```

---

## 8. خطة التنفيذ (Execution Plan)

### المرحلة 1: قاعدة البيانات (DB Schema)

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-001** | إنشاء جداول Reports + ReportColumns + ReportFilters + ReportLayouts | متوسط |
| **TASK-REPORT-002** | EF DbContext update + Migration + Seed Data | متوسط |

### المرحلة 2: Backend — Report Service

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-003** | ReportService.cs — View Discovery (جلب Views المتاحة) | متوسط |
| **TASK-REPORT-004** | ReportService.cs — Schema Introspection (جلب أعمدة View) | متوسط |
| **TASK-REPORT-005** | ReportService.cs — CRUD Operations | متوسط |
| **TASK-REPORT-006** | ReportService.cs — Dynamic Query Builder + Execution | عالي |
| **TASK-REPORT-007** | ReportService.cs — Filter Options (Dropdown choices) | متوسط |

### المرحلة 3: Backend — API Endpoints

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-008** | API: Reports CRUD endpoints (List/Detail/Create/Update/Delete) | متوسط |
| **TASK-REPORT-009** | API: Views + ViewColumns endpoints | متوسط |
| **TASK-REPORT-010** | API: Execute endpoint + Filter Options endpoint | عالي |
| **TASK-REPORT-011** | API: Layouts CRUD endpoints | متوسط |

### المرحلة 4: Frontend — Report Builder

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-012** | Report Builder — Page scaffold + Step 1 (View Selector) | عالي |
| **TASK-REPORT-013** | Report Builder — Step 2 (Column Configurator) | عالي |
| **TASK-REPORT-014** | Report Builder — Step 3 (Filter Configurator) | عالي |
| **TASK-REPORT-015** | Report Builder — Step 4 (Preview + Save) | عالي |

### المرحلة 5: Frontend — Reports Page

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-016** | Reports Page — Layout (Sidebar + Main Area) | عالي |
| **TASK-REPORT-017** | Reports Page — Sidebar (قائمة التقارير) | متوسط |
| **TASK-REPORT-018** | Reports Page — Report Filters Toolbar (Server-Side) | عالي |
| **TASK-REPORT-019** | Reports Page — Advanced Table Toolbar (7 أدوات) | عالي |
| **TASK-REPORT-020** | Reports Page — AG Grid Table + Custom Renderers | عالي |
| **TASK-REPORT-021** | Reports Page — Image Preview Modal | متوسط |
| **TASK-REPORT-022** | Reports Page — Excel Export (respect filters/sort) | متوسط |
| **TASK-REPORT-023** | Reports Page — Save/Restore Layout | متوسط |

### المرحلة 6: التكامل والاختبار

| المهمة | الوصف | التعقيد |
|--------|-------|---------|
| **TASK-REPORT-024** | ربط Report Builder + Reports Page بقاعدة البيانات | متوسط |
| **TASK-REPORT-025** | Seed Data — التقارير الأولية (3-5 تقارير) | متوسط |
| **TASK-REPORT-026** | اختبار شامل — سيناريوهات استخدام كاملة | عالي |
| **TASK-REPORT-027** | Responsive + RTL + Dark Mode support | متوسط |

---

## 9. ملخص الإجمالي

| المرحلة | عدد المهام | التعقيد |
|---------|-----------|---------|
| 1. قاعدة البيانات | 2 | متوسط |
| 2. Backend Service | 5 | متوسط-عالي |
| 3. Backend API | 4 | متوسط |
| 4. Frontend Builder | 4 | عالي |
| 5. Frontend Reports | 8 | عالي |
| 6. Integration | 4 | متوسط |
| **المجموع** | **27** | |

### التكلفة المقدرة (DEV Days)

| المرحلة | أيام تقديرية |
|---------|-------------|
| 1. DB Schema | 0.5 |
| 2. Backend Service | 2 |
| 3. Backend API | 1.5 |
| 4. Frontend Builder | 3 |
| 5. Frontend Reports | 4 |
| 6. Integration | 1.5 |
| **المجموع** | **~12.5 يوم** |

---

## 10. معايير القبول (Acceptance Criteria)

### 10.1 Report Builder
- [ ] يمكن إنشاء تقرير جديد من الصفر
- [ ] يمكن اختيار View من SQL Server
- [ ] يمكن تكوين الأعمدة (اسم عربي، عرض، مرئية)
- [ ] يمكن تكوين الفلاتر (نوع، اسم، إلزامية)
- [ ] يمكن معاينة التقرير قبل الحفظ
- [ ] يمكن تعديل تقرير موجود
- [ ] يمكن حذف تقرير

### 10.2 Reports Page
- [ ] تظهر جميع التقارير في Sidebar
- [ ] يمكن اختيار تقرير وعرض بياناته
- [ ] الفلاتر تعمل بشكل صحيح (Server-Side)
- [ ] شريط الأدوات يعمل (بحث، Excel، أعمدة، حفظ تصميم)
- [ ] AG Grid يعمل (ترتيب، فلترة، pagination)
- [ ] صور تظهر كـ thumbnails
- [ ] معاينة الصور تعمل (Modal)
- [ ] Excel Export يحترم الفلاتر والترتيب
- [ ] حفظ واسترجاع التصميم يعمل

### 10.3 التصميم
- [ ] ألوان متوافقة مع blue-theme.css
- [ ] RTL يعمل بشكل صحيح
- [ ] Responsive على الشاشات المختلفة
- [ ] أنيميشن سلسة
- [ ] Dark Mode يعمل

---

> **Prepared by:** TeraConductor — 2026-07-20
> **Approved for Execution:** Pending Majed Approval
