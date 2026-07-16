# TASK-KPI-FIX-001 — Builder.cshtml.cs Missing KPI BindProperty Fields

| البند | القيمة |
|---|---|
| **المعرف** | TASK-KPI-FIX-001 |
| **المجموعة** | KPI |
| **النوع** | Backend — Fix |
| **الوكيل** | engineering-agent |
| **الأولوية** | Critical |
| **الحالة** | Accepted |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

`Builder.cshtml.cs` **لا يحتوي** على `[BindProperty]` للحقول الجديدة (KPI fields). هذا يعني:
- الحقول المخفية في النموذج (valueColumn, dateColumn, kpiMode، إلخ) **لن تُرقَّم** عند الحفظ
- `DashboardCardDto` لا يحتوي هذه الحقول
- `BuildDashboardCard()` لا يُعيّن هذه الحقول
- `PreviewRequest` لا يحتوي هذه الحقول

**النتيجة:** بطاقات KPI المتقدمة لا يمكن حفظها بالبيانات الصحيحة.

---

## 2. الإصلاحات المطلوبة

### 2.1 إضافة 14 BindProperty إلى BuilderModel

إضافة هذه الحقول بعد `CustomLabelsJson`:

```csharp
// === Advanced KPI Fields ===

[BindProperty]
[JsonPropertyName("kpiMode")]
public string KpiMode { get; set; } = "simple";

[BindProperty]
[JsonPropertyName("valueColumn")]
public string ValueColumn { get; set; } = string.Empty;

[BindProperty]
[JsonPropertyName("dateColumn")]
public string DateColumn { get; set; } = string.Empty;

[BindProperty]
[JsonPropertyName("categoryColumn")]
public string CategoryColumn { get; set; } = string.Empty;

[BindProperty]
[JsonPropertyName("showChange")]
public bool ShowChange { get; set; } = false;

[BindProperty]
[JsonPropertyName("changeSource")]
public string ChangeSource { get; set; } = "previousPeriod";

[BindProperty]
[JsonPropertyName("showSparkline")]
public bool ShowSparkline { get; set; } = false;

[BindProperty]
[JsonPropertyName("sparklineMonths")]
public int SparklineMonths { get; set; } = 6;

[BindProperty]
[JsonPropertyName("showGrandTotal")]
public bool ShowGrandTotal { get; set; } = false;

[BindProperty]
[JsonPropertyName("grandTotalSource")]
public string GrandTotalSource { get; set; } = "sameTable";

[BindProperty]
[JsonPropertyName("dateFilterMode")]
public string DateFilterMode { get; set; } = "dashboard";

[BindProperty]
[JsonPropertyName("fixedStartDate")]
public string FixedStartDate { get; set; } = string.Empty;

[BindProperty]
[JsonPropertyName("fixedEndDate")]
public string FixedEndDate { get; set; } = string.Empty;

[BindProperty]
[JsonPropertyName("relativeDays")]
public int RelativeDays { get; set; } = 30;
```

### 2.2 إضافة الحقول إلى DashboardCardDto

إضافة بعد `CustomLabelsJson` في `DashboardCardDto`:

```csharp
// === Advanced KPI ===
public string KpiMode { get; set; } = "simple";
public string ValueColumn { get; set; } = string.Empty;
public string DateColumn { get; set; } = string.Empty;
public string CategoryColumn { get; set; } = string.Empty;
public bool ShowChange { get; set; } = false;
public string ChangeSource { get; set; } = "previousPeriod";
public bool ShowSparkline { get; set; } = false;
public int SparklineMonths { get; set; } = 6;
public bool ShowGrandTotal { get; set; } = false;
public string GrandTotalSource { get; set; } = "sameTable";
public string DateFilterMode { get; set; } = "dashboard";
public string FixedStartDate { get; set; } = string.Empty;
public string FixedEndDate { get; set; } = string.Empty;
public int RelativeDays { get; set; } = 30;
```

### 2.3 تحديث BuildDashboardCard()

في method `BuildDashboardCard()`، أضف بعد `CustomLabelsJson = CustomLabelsJson,`:

```csharp
// Advanced KPI
KpiMode = KpiMode,
ValueColumn = ValueColumn,
DateColumn = DateColumn,
CategoryColumn = CategoryColumn,
ShowChange = ShowChange,
ChangeSource = ChangeSource,
ShowSparkline = ShowSparkline,
SparklineMonths = SparklineMonths,
ShowGrandTotal = ShowGrandTotal,
GrandTotalSource = GrandTotalSource,
DateFilterMode = DateFilterMode,
FixedStartDate = FixedStartDate,
FixedEndDate = FixedEndDate,
RelativeDays = RelativeDays,
```

### 2.4 تحديث BuildCardFromRequest()

في method `BuildCardFromRequest()`، أضف بعد `CustomLabelsJson = request.CustomLabelsJson`:

```csharp
// Advanced KPI
KpiMode = request.KpiMode,
ValueColumn = request.ValueColumn,
DateColumn = request.DateColumn,
CategoryColumn = request.CategoryColumn,
ShowChange = request.ShowChange,
ChangeSource = request.ChangeSource,
ShowSparkline = request.ShowSparkline,
SparklineMonths = request.SparklineMonths,
ShowGrandTotal = request.ShowGrandTotal,
GrandTotalSource = request.GrandTotalSource,
DateFilterMode = request.DateFilterMode,
FixedStartDate = request.FixedStartDate,
FixedEndDate = request.FixedEndDate,
RelativeDays = request.RelativeDays,
```

### 2.5 تحديث PreviewRequest

إضافة الحقول إلى `PreviewRequest`:

```csharp
// Advanced KPI
public string KpiMode { get; set; } = "simple";
public string ValueColumn { get; set; } = string.Empty;
public string DateColumn { get; set; } = string.Empty;
public string CategoryColumn { get; set; } = string.Empty;
public bool ShowChange { get; set; } = false;
public string ChangeSource { get; set; } = "previousPeriod";
public bool ShowSparkline { get; set; } = false;
public int SparklineMonths { get; set; } = 6;
public bool ShowGrandTotal { get; set; } = false;
public string GrandTotalSource { get; set; } = "sameTable";
public string DateFilterMode { get; set; } = "dashboard";
public string FixedStartDate { get; set; } = string.Empty;
public string FixedEndDate { get; set; } = string.Empty;
public int RelativeDays { get; set; } = 30;
```

---

## 3. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-KPI-FIX-001.md
```

---

## 4. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `BuilderModel` يحتوي 14 `[BindProperty]` جديد | ⬜ |
| AC-2 | `DashboardCardDto` يحتوي 14 حقل KPI جديد | ⬜ |
| AC-3 | `BuildDashboardCard()` يُعيّن جميع الحقول الجديدة | ⬜ |
| AC-4 | `BuildCardFromRequest()` يُعيّن جميع الحقول الجديدة | ⬜ |
| AC-5 | `PreviewRequest` يحتوي 14 حقل KPI جديد | ⬜ |
| AC-6 | `dotnet build -c Release` ينجح بدون أخطاء | ⬜ |
| AC-7 | لا توجد أسرار في الملفات المعدلة | ⬜ |

---

## 5. Pre-Execution Gate

**Result:** PASS
- Fix task — إصلاح مشكلة محددة
- Single file — `Builder.cshtml.cs` فقط
- No security impact
- No database changes
