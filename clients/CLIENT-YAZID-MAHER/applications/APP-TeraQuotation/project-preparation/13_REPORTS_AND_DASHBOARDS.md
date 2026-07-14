# 13_REPORTS_AND_DASHBOARDS.md — TeraQuotation

> **Reports and Dashboards — Technical Specification**
> **المشروع:** TeraQuotation — نظام إدارة عروض أسعار قطع السيارات
> **التقنية:** WPF (.NET 8) + CommunityToolkit.Mvvm + SQLite/EF Core
> **تاريخ الإصدار:** 2026-07-13
> **المصادر:** APPLICATION_BLUEPRINT.md ✅ + 08_TECHNICAL_ARCHITECTURE.md ✅ + 06_DATA_MODEL_PREPARATION.md ✅ + 07_SCREENS_AND_UI_STRUCTURE.md ✅ + TERA_PROJECT_DECISION.md ✅

---

## Lifecycle Header

| الحقل | القيمة |
|:------|:-------|
| **Document State** | **Module Baseline Approved** |
| **Baseline Module** | TeraQuotation (Full Application) |
| **Current State** | Module Baseline Approved |
| **Owner** | Software Designer Agent (مُصمم) |
| **Last Review** | 2026-07-13 |
| **Expiry** | End of Project Delivery |

---

## قائمة المحتويات

1. [نظرة عامة — هيكل التقارير](#1-نظرة-عامة--هيكل-التقارير)
2. [الهيكل البرمجي (Software Structure)](#2-الهيكل-البرمجي-software-structure)
3. [DTOs المشتركة و ReportService](#3-dtos-المشتركة-و-reportservice)
4. [التقرير D1 — Supplier Price Comparison](#4-التقرير-d1--supplier-price-comparison-مقارنة-أسعار-الموردين)
5. [التقرير D2 — Most Requested Items](#5-التقرير-d2--most-requested-items-أكثر-القطع-طلباً)
6. [التقرير D3 — Quotation History](#6-التقرير-d3--quotation-history-سجل-العروض)
7. [التقرير D4 — Monthly Total](#7-التقرير-d4--monthly-total-الإجمالي-الشهري)
8. [طباعة وتصدير PDF للتقارير](#8-طباعة-وتصدير-pdf-للتقارير)
9. [تكامل شاشة S5 (Reports Screen)](#9-تكامل-شاشة-s5-reports-screen)
10. [سيناريوهات الاختبار والتأكيد](#10-سيناريوهات-الاختبار-والتأكيد)
11. [Design Gaps](#11-design-gaps)

---

## 1. نظرة عامة — هيكل التقارير

### 1.1 قائمة التقارير

| الكود | اسم التقرير | الهدف | المدخلات الرئيسية |
|:-----:|:------------|:------|:------------------|
| **D1** | Supplier Price Comparison (مقارنة أسعار الموردين) | مقارنة أسعار 3 موردين لكل قطعة في عرض معين | اختيار عرض (Quotation) |
| **D2** | Most Requested Items (أكثر القطع طلباً) | عرض القطع الأكثر ظهوراً في العروض | نطاق تاريخ (اختياري) + عدد النتائج |
| **D3** | Quotation History (سجل العروض) | سجل كامل للعروض مع التصفية | نطاق تاريخ + تصفية حالة (اختياري) |
| **D4** | Monthly Total (الإجمالي الشهري) | إجمالي قيمة العروض الصادرة كل شهر | سنة محددة |

### 1.2 مبادئ عامة

| المبدأ | القاعدة |
|:-------|:--------|
| **عرض البيانات** | DataGrid داخل UserControl — لا رسوم بيانية (المشروع صغير) |
| **الترتيب الافتراضي** | تنازلي (الأحدث/الأعلى أولاً) لكل تقرير |
| **الترقيم** | ترقيم تلقائي للصفوف (1, 2, 3...) في DataGrid |
| **تاريخ التقرير** | يُطبع في أسفل التقرير: "تاريخ التقرير: 13/07/2026" |
| **شعار التطبيق** | يُطبع في رأس كل صفحة عند الطباعة/PDF |
| **البيانات الفارغة** | رسالة واضحة + لا طباعة إذا كان الجدول فارغاً |
| **اتجاه الطباعة** | عمودي (Portrait) لـ D3, D4 — أفقي (Landscape) لـ D1, D2 |
| **العملة** | كل الأسعار تُعرض بـ `ToString("N2")` مع "ر.س" أو "د.ك" حسب إعدادات الترويسة |

---

## 2. الهيكل البرمجي (Software Structure)

### 2.1 هيكل المجلدات

```
src/TeraQuotation/
└── Views/
    └── Reports/
        ├── SupplierComparisonView.xaml / .cs        # D1
        ├── TopItemsView.xaml / .cs                   # D2
        ├── QuotationHistoryView.xaml / .cs            # D3
        └── MonthlyTotalView.xaml / .cs                # D4
```

### 2.2 الفئات (Classes) لكل تقرير

| التقرير | View | ViewModel | DTO |
|:--------|:-----|:----------|:----|
| D1 | `SupplierComparisonView` | `SupplierComparisonViewModel` | `ComparisonRowDto` + `ItemDetailDto` |
| D2 | `TopItemsView` | `TopItemsViewModel` | `ItemRequestCountDto` |
| D3 | `QuotationHistoryView` | `QuotationHistoryViewModel` | `QuotationHistoryDto` |
| D4 | `MonthlyTotalView` | `MonthlyTotalViewModel` | `MonthlyTotalDto` |

### 2.3 مبدأ عمل التقرير في S5

```
[المستخدم يضغط زر تقرير في S5]
        │
        ▼
ReportsViewModel.ShowReportCommand("D1")
        │
        ├── 1. يعيّن CurrentReportViewModel = new SupplierComparisonViewModel(...)
        │
        ├── 2. ContentControl.AutoBinding → DataTemplate → SupplierComparisonView
        │
        ├── 3. ViewModel.OnLoaded() → استدعاء ReportService
        │
        ├── 4. ReportService → EF Core → LINQ → DTO List
        │
        ├── 5. ObservableCollection<DTO> ← DataGrid ItemsSource
        │
        └── 6. المستخدم يضغط [طباعة] أو [PDF]
```

### 2.4 حقن التبعيات (DI)

كل ViewModel يستقبل `IReportService` عبر Constructor Injection:

```csharp
public class SupplierComparisonViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    public SupplierComparisonViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task LoadAsync()
    {
        // استدعاء خدمة التقرير
        var data = await _reportService.GetSupplierPriceComparisonAsync(selectedQuotationId);
        ComparisonData = new ObservableCollection<ComparisonRowDto>(data);
    }
}
```

---

## 3. DTOs المشتركة و ReportService

### 3.1 واجهة IReportService

```csharp
public interface IReportService
{
    // D1: مقارنة أسعار الموردين لعرض معين
    Task<List<ComparisonRowDto>> GetSupplierPriceComparisonAsync(int quotationId);

    // D2: أكثر القطع طلباً (مع نطاق تاريخ اختياري)
    Task<List<ItemRequestCountDto>> GetMostRequestedItemsAsync(
        int topN = 10,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    // D3: سجل العروض
    Task<List<QuotationHistoryDto>> GetQuotationHistoryAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? statusFilter = null);

    // D4: الإجمالي الشهري
    Task<List<MonthlyTotalDto>> GetMonthlyTotalsAsync(int year);
}
```

### 3.2 DTOs

```csharp
// ===== D1: مقارنة أسعار الموردين =====
public class ComparisonRowDto
{
    public string ItemName { get; set; } = string.Empty;       // اسم القطعة
    public string? Supplier1Name { get; set; }                  // اسم المورد 1
    public decimal? Supplier1Price { get; set; }                // سعر المورد 1
    public string? Supplier2Name { get; set; }                  // اسم المورد 2
    public decimal? Supplier2Price { get; set; }                // سعر المورد 2
    public string? Supplier3Name { get; set; }                  // اسم المورد 3
    public decimal? Supplier3Price { get; set; }                // سعر المورد 3
    public decimal? BestPrice { get; set; }                     // أفضل سعر (Min)
    public string? BestSupplierName { get; set; }               // اسم أفضل مورد
    public decimal? PriceDifference { get; set; }               // الفرق بين الأعلى والأفضل
}

// ===== D2: أكثر القطع طلباً =====
public class ItemRequestCountDto
{
    public int Rank { get; set; }                               // الترتيب
    public int ItemId { get; set; }                             // معرف القطعة
    public string ItemName { get; set; } = string.Empty;        // اسم القطعة
    public int RequestCount { get; set; }                       // عدد مرات الطلب
    public decimal? AveragePrice { get; set; }                  // متوسط السعر (اختياري)
}

// ===== D3: سجل العروض =====
public class QuotationHistoryDto
{
    public int Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;     // رقم العرض
    public DateTime Date { get; set; }                          // التاريخ
    public string? Description { get; set; }                    // الوصف
    public string Status { get; set; } = string.Empty;          // الحالة
    public string StatusText { get; set; } = string.Empty;      // الحالة بالعربية
    public int ItemCount { get; set; }                          // عدد القطع في العرض
    public decimal? TotalAmount { get; set; }                   // إجمالي العرض (مجموع أسعار المورد الأول)
}

// ===== D4: الإجمالي الشهري =====
public class MonthlyTotalDto
{
    public int Year { get; set; }                               // السنة
    public int Month { get; set; }                              // رقم الشهر (1-12)
    public string MonthName { get; set; } = string.Empty;       // اسم الشهر بالعربية
    public int QuotationCount { get; set; }                     // عدد العروض
    public decimal? TotalSupplier1 { get; set; }                // إجمالي المورد 1
    public decimal? TotalSupplier2 { get; set; }                // إجمالي المورد 2
    public decimal? TotalSupplier3 { get; set; }                // إجمالي المورد 3
    public decimal? GrandTotal { get; set; }                    // الإجمالي الكلي للشهر
}
```

### 3.3 تطبيق ReportService (هيكل فقط)

```csharp
public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    // ... تفاصيل كل دالة في الأقسام التالية
}
```

> **ملاحظة:** لا يتم إنشاء Repository منفصل. `ReportService` يستخدم `AppDbContext` مباشرة لأن المشروع صغير (قاعدة 08_TECHNICAL_ARCHITECTURE.md).

---

## 4. التقرير D1 — Supplier Price Comparison (مقارنة أسعار الموردين)

### 4.1 الغرض

مقارنة أسعار 3 موردين لكل قطعة داخل عرض سعر **محدد**، مع تحديد أفضل سعر وتمييزه للمستخدم.

### 4.2 المدخلات (Parameters)

| المعامل | النوع | المصدر | إجباري؟ |
|:--------|:------|:-------|:--------:|
| `quotationId` | `int` | اختيار المستخدم من ComboBox في شاشة S5 | ✅ نعم |

**آلية اختيار العرض في S5:**
- ComboBox يعرض قائمة `{QuoteNumber} — {Description} — {Date}`
- مصدرها `QuotationService.GetAllQuotationsAsync()` (جميع العروض، مرتبة تنازلياً)
- المستخدم يختار عرضاً ← التقرير يتجدد تلقائياً
- عرض فقط العروض التي Status ≥ `UpdatedWithPrices` (لأنها تحتوي أسعاراً)

### 4.3 مصدر البيانات (Data Source)

| الكيان | الحقول المستخدمة |
|:-------|:-----------------|
| `Quotation` | `Id`, `QuoteNumber` (لعنوان التقرير) |
| `QuotationItem` | `ItemId`, `Supplier1Type`, `Supplier1Price`, `Supplier2Type`, `Supplier2Price`, `Supplier3Type`, `Supplier3Price` |
| `Item` | `Id`, `Name` (اسم القطعة) |

### 4.4 الاستعلام (Query Logic) — LINQ

```csharp
public async Task<List<ComparisonRowDto>> GetSupplierPriceComparisonAsync(int quotationId)
{
    // 1. التحقق من وجود العرض
    var quotation = await _context.Quotations
        .FirstOrDefaultAsync(q => q.Id == quotationId);

    if (quotation == null)
        return new List<ComparisonRowDto>();

    // 2. جلب بنود العرض مع أسماء القطع
    var items = await _context.QuotationItems
        .Include(qi => qi.Item)
        .Where(qi => qi.QuotationId == quotationId)
        .OrderBy(qi => qi.SortOrder)
        .ToListAsync();

    // 3. تحويل إلى DTO مع حساب أفضل سعر لكل قطعة
    return items.Select(qi =>
    {
        // جمع الأسعار الموجودة (غير null)
        var prices = new[]
        {
            (Name: qi.Supplier1Type, Price: qi.Supplier1Price),
            (Name: qi.Supplier2Type, Price: qi.Supplier2Price),
            (Name: qi.Supplier3Type, Price: qi.Supplier3Price)
        }
        .Where(p => p.Price.HasValue)
        .ToList();

        // تحديد أفضل سعر (الأقل)
        var best = prices.OrderBy(p => p.Price).FirstOrDefault();

        return new ComparisonRowDto
        {
            ItemName = qi.Item.Name,
            Supplier1Name = qi.Supplier1Type,
            Supplier1Price = qi.Supplier1Price,
            Supplier2Name = qi.Supplier2Type,
            Supplier2Price = qi.Supplier2Price,
            Supplier3Name = qi.Supplier3Type,
            Supplier3Price = qi.Supplier3Price,
            BestPrice = best.Price,
            BestSupplierName = best.Name,
            PriceDifference = prices.Any()
                ? prices.Max(p => p.Price) - prices.Min(p => p.Price)
                : null
        };
    }).ToList();
}
```

### 4.5 المخرجات (Output Structure)

**جدول المقارنة:**

| # | اسم القطعة | المورد 1 | سعر 1 | المورد 2 | سعر 2 | المورد 3 | سعر 3 | أفضل سعر 🏆 | أفضل مورد |
|:-:|:-----------|:---------|:-----:|:---------|:-----:|:---------|:-----:|:----------:|:---------:|
| 1 | فلتر زيت | وكلاء أحمد | 45.00 | العالمية | 42.00 | مؤسسة النور | 48.00 | **42.00** ✅ | العالمية |
| 2 | سير توقيت | وكلاء أحمد | 120.00 | العالمية | 115.00 | مؤسسة النور | — | **115.00** ✅ | العالمية |
| 3 | بواجي | وكلاء أحمد | 25.00 | العالمية | — | مؤسسة النور | 30.00 | **25.00** ✅ | وكلاء أحمد |

**عنوان التقرير:** `مقارنة أسعار الموردين — عرض Q-003 (13/07/2026)`

### 4.6 التنسيق

| الخاصية | القيمة |
|:--------|:-------|
| **اتجاه الطباعة** | أفقي (Landscape) — لأن عدد الأعمدة 9 |
| **حجم الورق** | A4 |
| **تمييز أفضل سعر** | لون الخلفية: `#E8F5E9` (أخضر فاتح) + نص Bold |
| **الخلايا الفارغة** | `—` (شرطة) بدلاً من تركها فارغة |
| **رأس الصفحة** | شعار التطبيق + اسم التقرير + رقم العرض والتاريخ |
| **تذييل الصفحة** | تاريخ الطباعة + "TeraQuotation" |

### 4.7 الحالات الخاصة

| الحالة | السلوك |
|:-------|:-------|
| **عرض غير موجود** | `MessageBox.Show("العرض المحدد غير موجود.")` |
| **لا توجد بنود في العرض** | DataGrid فارغ + رسالة "لا توجد قطع في هذا العرض" |
| **لا توجد أسعار مدخلة** | رسالة تنبيه: "لم يتم إدخال أسعار بعد لهذا العرض. يرجى إدخال الأسعار أولاً." |
| **عرض بحالة Draft (مسودة)** | يتم السماح بعرض التقرير لكن الأسعار قد تكون فارغة — المستخدم مسؤول |
| **خطأ في قاعدة البيانات** | رسالة خطأ + تسجيل في LoggingService |

### 4.8 ViewModel + View

```csharp
public class SupplierComparisonViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IQuotationService _quotationService;

    // قائمة العروض المتاحة للاختيار
    public ObservableCollection<Quotation> AvailableQuotations { get; }

    // العرض المختار
    private Quotation? _selectedQuotation;
    public Quotation? SelectedQuotation
    {
        get => _selectedQuotation;
        set
        {
            if (SetProperty(ref _selectedQuotation, value) && value != null)
                _ = LoadComparisonDataAsync();
        }
    }

    // بيانات المقارنة
    public ObservableCollection<ComparisonRowDto> ComparisonData { get; }

    // حالة التحميل
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // رسالة فارغة
    private string _emptyMessage = string.Empty;
    public string EmptyMessage
    {
        get => _emptyMessage;
        set => SetProperty(ref _emptyMessage, value);
    }

    // الأوامر
    public IAsyncRelayCommand PrintCommand { get; }
    public IAsyncRelayCommand ExportPdfCommand { get; }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var quotations = await _quotationService.GetAllQuotationsAsync();
            AvailableQuotations.Clear();
            foreach (var q in quotations.Where(q => q.Status != "Draft"))
                AvailableQuotations.Add(q);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadComparisonDataAsync()
    {
        if (SelectedQuotation == null) return;

        IsLoading = true;
        try
        {
            var data = await _reportService
                .GetSupplierPriceComparisonAsync(SelectedQuotation.Id);

            ComparisonData.Clear();
            if (data.Count == 0)
            {
                EmptyMessage = "لا توجد بيانات مقارنة لهذا العرض.";
            }
            else
            {
                EmptyMessage = string.Empty;
                foreach (var row in data)
                    ComparisonData.Add(row);
            }
        }
        catch (Exception ex)
        {
            EmptyMessage = "حدث خطأ أثناء تحميل التقرير.";
            // تسجيل الخطأ
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

**ملف العرض (SupplierComparisonView.xaml):**
- `ComboBox` لاختيار العرض (Top area)
- `DataGrid` مع تنسيق الصف الأخضر لأفضل سعر
- `Button` طباعة + PDF (Bottom)

---

## 5. التقرير D2 — Most Requested Items (أكثر القطع طلباً)

### 5.1 الغرض

عرض القطع الأكثر ظهوراً في عروض الأسعار، مرتبة تنازلياً، لمساعدة المستخدم في تحليل الطلب واتخاذ قرارات المخزون.

### 5.2 المدخلات (Parameters)

| المعامل | النوع | المصدر | إجباري؟ | القيمة الافتراضية |
|:--------|:------|:-------|:--------:|:-----------------:|
| `topN` | `int` | NumericUpDown (5–50) | ❌ | 10 |
| `fromDate` | `DateTime?` | DatePicker | ❌ | null (بداية السجلات) |
| `toDate` | `DateTime?` | DatePicker | ❌ | null (اليوم) |

### 5.3 مصدر البيانات (Data Source)

| الكيان | الحقول المستخدمة |
|:-------|:-----------------|
| `QuotationItem` | `ItemId`, `QuotationId` + `Supplier1Price/2/3` (لحساب متوسط السعر) |
| `Item` | `Id`, `Name` |
| `Quotation` | `Id`, `Date` (لتصفية النطاق الزمني) |

### 5.4 الاستعلام (Query Logic) — LINQ

```csharp
public async Task<List<ItemRequestCountDto>> GetMostRequestedItemsAsync(
    int topN = 10,
    DateTime? fromDate = null,
    DateTime? toDate = null)
{
    var query = _context.QuotationItems
        .Include(qi => qi.Item)
        .Include(qi => qi.Quotation)
        .AsQueryable();

    // تصفية حسب النطاق الزمني
    if (fromDate.HasValue)
        query = query.Where(qi => qi.Quotation.Date >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(qi => qi.Quotation.Date <= toDate.Value);

    // تجميع حسب القطعة
    var grouped = await query
        .GroupBy(qi => new { qi.ItemId, qi.Item.Name })
        .Select(g => new
        {
            g.Key.ItemId,
            g.Key.Name,
            Count = g.Count(),
            // متوسط سعر المورد الأول (كمؤشر)
            AvgPrice = g.Average(qi => (double?)(qi.Supplier1Price ?? 0))
        })
        .OrderByDescending(g => g.Count)
        .Take(topN)
        .ToListAsync();

    // ترقيم الصفوف في C#
    return grouped.Select((item, index) => new ItemRequestCountDto
    {
        Rank = index + 1,
        ItemId = item.ItemId,
        ItemName = item.Name,
        RequestCount = item.Count,
        AveragePrice = item.AvgPrice.HasValue
            ? (decimal)item.AvgPrice.Value
            : null
    }).ToList();
}
```

### 5.5 المخرجات (Output Structure)

| الرتبة | اسم القطعة | عدد مرات الطلب | متوسط السعر (م1) |
|:-----:|:-----------|:--------------:|:----------------:|
| 🥇 1 | فلتر زيت | 15 | 43.50 |
| 🥈 2 | سير توقيت | 12 | 118.00 |
| 🥉 3 | بواجي | 10 | 27.00 |
| 4 | زيت محرك | 8 | 95.00 |
| 5 | طقم كلتش | 7 | 450.00 |

**عنوان التقرير:** `أكثر {N} قطع طلباً — من {fromDate} إلى {toDate}`

### 5.6 التنسيق

| الخاصية | القيمة |
|:--------|:-------|
| **اتجاه الطباعة** | عمودي (Portrait) — 4 أعمدة فقط |
| **حجم الورق** | A4 |
| **المراكز الثلاثة الأولى** | أيقونات 🥇🥈🥉 أو ألوان (ذهبي/فضي/برونزي) |
| **الترتيب** | تنازلي حسب عدد مرات الطلب |
| **رأس الصفحة** | شعار + اسم التقرير + نطاق التاريخ + تاريخ التقرير |

### 5.7 الحالات الخاصة

| الحالة | السلوك |
|:-------|:-------|
| **لا توجد قطع على الإطلاق** | رسالة "لم يتم إنشاء أي عروض أسعار بعد" |
| **لا توجد قطع في النطاق الزمني** | رسالة "لا توجد قطع ضمن النطاق الزمني المحدد" |
| **عدد نتائج أقل من `topN`** | عرض العدد الموجود فقط (لا رسالة خطأ) |
| **`topN` = 0 أو سالب** | تجاوز القيمة إلى 10 (الافتراضي) مع Validation |
| **تاريخ `fromDate` > `toDate`** | رسالة "تاريخ البداية يجب أن يكون قبل تاريخ النهاية" |

### 5.8 ViewModel + View

```csharp
public class TopItemsViewModel : ObservableObject
{
    // Top N (5–50)
    private int _topN = 10;
    public int TopN
    {
        get => _topN;
        set
        {
            if (SetProperty(ref _topN, Math.Clamp(value, 5, 50)))
                _ = LoadAsync();
        }
    }

    // نطاق التاريخ
    private DateTime? _fromDate;
    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            if (SetProperty(ref _fromDate, value))
                _ = LoadAsync();
        }
    }

    private DateTime? _toDate;
    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            if (SetProperty(ref _toDate, value))
                _ = LoadAsync();
        }
    }

    public ObservableCollection<ItemRequestCountDto> TopItems { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand PrintCommand { get; }
    public IAsyncRelayCommand ExportPdfCommand { get; }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _reportService
                .GetMostRequestedItemsAsync(TopN, FromDate, ToDate);
            TopItems.Clear();
            foreach (var item in data) TopItems.Add(item);
        }
        finally { IsLoading = false; }
    }
}
```

---

## 6. التقرير D3 — Quotation History (سجل العروض)

### 6.1 الغرض

سجل كامل لجميع عروض الأسعار مع إمكانية التصفية حسب التاريخ والحالة. يمكن استخدامه للرجوع إلى العروض القديمة أو تحليل أداء العروض.

### 6.2 المدخلات (Parameters)

| المعامل | النوع | المصدر | إجباري؟ |
|:--------|:------|:-------|:--------:|
| `fromDate` | `DateTime?` | DatePicker | ❌ |
| `toDate` | `DateTime?` | DatePicker | ❌ |
| `statusFilter` | `string?` | ComboBox (الكل, Draft, UpdatedWithPrices, Printed, PDFExported, SentViaOutlook) | ❌ |

### 6.3 مصدر البيانات (Data Source)

| الكيان | الحقول المستخدمة |
|:-------|:-----------------|
| `Quotation` | `Id`, `QuoteNumber`, `Date`, `Description`, `Status`, `CreatedAt` |
| `QuotationItem` | `QuotationId`, `Supplier1Price`, `Supplier2Price`, `Supplier3Price` (لحساب الإجمالي) |

### 6.4 الاستعلام (Query Logic) — LINQ

```csharp
public async Task<List<QuotationHistoryDto>> GetQuotationHistoryAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? statusFilter = null)
{
    var query = _context.Quotations
        .Include(q => q.QuotationItems)
        .AsQueryable();

    // تصفية حسب التاريخ
    if (fromDate.HasValue)
        query = query.Where(q => q.Date >= fromDate.Value);

    if (toDate.HasValue)
        query = query.Where(q => q.Date <= toDate.Value);

    // تصفية حسب الحالة
    if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
        query = query.Where(q => q.Status == statusFilter);

    // جلب البيانات مع حساب الإجمالي لكل عرض
    var result = await query
        .OrderByDescending(q => q.Date)
        .ThenByDescending(q => q.Id)
        .Select(q => new QuotationHistoryDto
        {
            Id = q.Id,
            QuoteNumber = q.QuoteNumber,
            Date = q.Date,
            Description = q.Description,
            Status = q.Status,
            StatusText = TranslateStatus(q.Status),
            ItemCount = q.QuotationItems.Count,
            TotalAmount = q.QuotationItems.Sum(qi =>
                (qi.Supplier1Price ?? 0) +
                (qi.Supplier2Price ?? 0) +
                (qi.Supplier3Price ?? 0))
        })
        .ToListAsync();

    return result;
}

// دالة مساعدة لترجمة الحالة
private static string TranslateStatus(string status) => status switch
{
    "Draft"             => "مسودة",
    "UpdatedWithPrices" => "محدّث بالأسعار",
    "Printed"           => "مطبوع",
    "PDFExported"       => "تم تصدير PDF",
    "SentViaOutlook"    => "مرسل عبر البريد",
    _                   => status
};
```

### 6.5 المخرجات (Output Structure)

| رقم العرض | التاريخ | الوصف | عدد القطع | الإجمالي | الحالة |
|:---------|:-------:|:------|:---------:|:--------:|:------|
| Q-015 | 13/07/2026 | توريد قطع مكينة كاملة | 8 | 2,450.00 | 🟢 مطبوع |
| Q-014 | 12/07/2026 | صيانة دورية | 3 | 520.00 | 🟣 مرسل عبر البريد |
| Q-013 | 10/07/2026 | قطع فرامل | 5 | 890.00 | 🟠 تم تصدير PDF |
| Q-012 | 08/07/2026 | طلب موردين | 4 | — | ⚪ مسودة |

**عنوان التقرير:** `سجل العروض — من 01/07/2026 إلى 13/07/2026`

### 6.6 التنسيق

| الخاصية | القيمة |
|:--------|:-------|
| **اتجاه الطباعة** | عمودي (Portrait) — 6 أعمدة |
| **حجم الورق** | A4 |
| **الفرز** | تنازلي حسب التاريخ (الأحدث أولاً) |
| **حالة المسودة** | إذا `Draft`, `TotalAmount` يُظهر `—` (شرطة) لأنها غير مكتملة |
| **ألوان الحالة** | نفس `StatusToColorConverter` من `07_SCREENS_AND_UI_STRUCTURE.md` |

### 6.7 الحالات الخاصة

| الحالة | السلوك |
|:-------|:-------|
| **لا توجد عروض في النطاق** | رسالة "لا توجد عروض في النطاق المحدد" |
| **نطاق تاريخ خاطئ (`from > to`)** | رسالة تحذير + لا تنفيذ استعلام |
| **عروض بحالة Draft بدون أسعار** | يُظهر `—` في عمود الإجمالي |
| **جميع العروض في النطاق** | إذا `fromDate = null` و `toDate = null` → جميع العروض |

### 6.8 ViewModel

```csharp
public class QuotationHistoryViewModel : ObservableObject
{
    // فلاتر
    private DateTime? _fromDate;
    public DateTime? FromDate
    {
        get => _fromDate;
        set { if (SetProperty(ref _fromDate, value)) _ = LoadAsync(); }
    }

    private DateTime? _toDate;
    public DateTime? ToDate
    {
        get => _toDate;
        set { if (SetProperty(ref _toDate, value)) _ = LoadAsync(); }
    }

    private string _selectedStatus = "All";
    public string SelectedStatus
    {
        get => _selectedStatus;
        set { if (SetProperty(ref _selectedStatus, value)) _ = LoadAsync(); }
    }

    // قائمة خيارات الحالة للـ ComboBox
    public List<string> StatusOptions { get; } = new()
    {
        "All", "Draft", "UpdatedWithPrices", "Printed", "PDFExported", "SentViaOutlook"
    };

    public ObservableCollection<QuotationHistoryDto> HistoryData { get; }
}
```

---

## 7. التقرير D4 — Monthly Total (الإجمالي الشهري)

### 7.1 الغرض

عرض إجمالي قيمة العروض الصادرة شهرياً لسنة محددة، مقسمة حسب كل مورد، مع إجمالي كلي. يساعد في تحليل الأداء المالي الشهري.

### 7.2 المدخلات (Parameters)

| المعامل | النوع | المصدر | إجباري؟ | القيمة الافتراضية |
|:--------|:------|:-------|:--------:|:-----------------:|
| `year` | `int` | ComboBox (2026–2030) أو العام الحالي | ❌ | السنة الحالية (DateTime.Today.Year) |

### 7.3 مصدر البيانات (Data Source)

| الكيان | الحقول المستخدمة |
|:-------|:-----------------|
| `Quotation` | `Id`, `Date`, `Status` |
| `QuotationItem` | `QuotationId`, `Supplier1Price`, `Supplier2Price`, `Supplier3Price` |

### 7.4 الاستعلام (Query Logic) — LINQ

```csharp
public async Task<List<MonthlyTotalDto>> GetMonthlyTotalsAsync(int year)
{
    // 1. جلب كل العروض للسنة المحددة (ما عدا المسودات)
    var quotations = await _context.Quotations
        .Include(q => q.QuotationItems)
        .Where(q => q.Date.Year == year && q.Status != "Draft")
        .ToListAsync();

    // 2. تجميع حسب الشهر
    var monthlyGroups = quotations
        .GroupBy(q => q.Date.Month)
        .OrderBy(g => g.Key)
        .Select(g =>
        {
            var monthItems = g.SelectMany(q => q.QuotationItems).ToList();

            return new MonthlyTotalDto
            {
                Year = year,
                Month = g.Key,
                MonthName = GetArabicMonthName(g.Key),
                QuotationCount = g.Count(),
                TotalSupplier1 = monthItems.Sum(qi => qi.Supplier1Price ?? 0),
                TotalSupplier2 = monthItems.Sum(qi => qi.Supplier2Price ?? 0),
                TotalSupplier3 = monthItems.Sum(qi => qi.Supplier3Price ?? 0),
                GrandTotal = monthItems.Sum(qi =>
                    (qi.Supplier1Price ?? 0) +
                    (qi.Supplier2Price ?? 0) +
                    (qi.Supplier3Price ?? 0))
            };
        })
        .ToList();

    return monthlyGroups;
}

private static string GetArabicMonthName(int month) => month switch
{
    1  => "يناير",
    2  => "فبراير",
    3  => "مارس",
    4  => "أبريل",
    5  => "مايو",
    6  => "يونيو",
    7  => "يوليو",
    8  => "أغسطس",
    9  => "سبتمبر",
    10 => "أكتوبر",
    11 => "نوفمبر",
    12 => "ديسمبر",
    _  => month.ToString()
};
```

### 7.5 المخرجات (Output Structure)

**جدول شهري:**

| الشهر | عدد العروض | إجمالي المورد 1 | إجمالي المورد 2 | إجمالي المورد 3 | الإجمالي الكلي |
|:------|:----------:|:---------------:|:---------------:|:---------------:|:--------------:|
| يناير | 3 | 1,200.00 | 950.00 | 1,500.00 | **3,650.00** |
| فبراير | 5 | 2,300.00 | 1,800.00 | 2,100.00 | **6,200.00** |
| مارس | 2 | 800.00 | — | 650.00 | **1,450.00** |
| ... | ... | ... | ... | ... | ... |
| **الإجمالي السنوي** | **10** | **4,300.00** | **2,750.00** | **4,250.00** | **11,300.00** |

**بطاقات ملخص (في أعلى التقرير):**
```
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  الإجمالي    │  │  عدد العروض  │  │  متوسط الشهر │
│  11,300.00   │  │     10       │  │   3,766.67   │
└──────────────┘  └──────────────┘  └──────────────┘
```

### 7.6 التنسيق

| الخاصية | القيمة |
|:--------|:-------|
| **اتجاه الطباعة** | عمودي (Portrait) — لكن يمكن أفقي إذا زادت الأعمدة |
| **حجم الورق** | A4 |
| **صف الإجمالي السنوي** | `FontWeight=Bold`, خلفية `#F5F5F5` (رمادي فاتح) |
| **الأشهر بدون عروض** | عرض الشهر مع `—` أو إخفاء الصف (اختياري) |
| **البطاقات** | 3 بطاقات ملونة في أعلى التقرير |

### 7.7 الحالات الخاصة

| الحالة | السلوك |
|:-------|:-------|
| **لا توجد عروض في السنة** | رسالة "لا توجد عروض في السنة المحددة" |
| **سنة مستقبلية (> 2030)** | رسالة تحذير "السنة المحددة غير متوقعة" + منع الاستعلام |
| **شهر معين بدون عروض** | عرض صف الشهر بقيمة صفرية (0.00) أو إخفاء الصف |
| **المسودات (Draft) لا تُحتسب** | `Status != "Draft"` — العروض غير المكتملة لا تدخل في الإجمالي |

### 7.8 ViewModel

```csharp
public class MonthlyTotalViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    // السنة المختارة
    private int _selectedYear = DateTime.Today.Year;
    public int SelectedYear
    {
        get => _selectedYear;
        set { if (SetProperty(ref _selectedYear, value)) _ = LoadAsync(); }
    }

    // قائمة السنوات (السنة الحالية - 5 إلى السنة الحالية + 2)
    public List<int> AvailableYears { get; } = Enumerable
        .Range(DateTime.Today.Year - 5, 8)
        .ToList();

    // البيانات الشهرية
    public ObservableCollection<MonthlyTotalDto> MonthlyData { get; }

    // بطاقات الملخص
    private decimal _grandTotal;
    public decimal GrandTotal
    {
        get => _grandTotal;
        set => SetProperty(ref _grandTotal, value);
    }

    private int _totalQuotationCount;
    public int TotalQuotationCount
    {
        get => _totalQuotationCount;
        set => SetProperty(ref _totalQuotationCount, value);
    }

    private decimal _monthlyAverage;
    public decimal MonthlyAverage
    {
        get => _monthlyAverage;
        set => SetProperty(ref _monthlyAverage, value);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand PrintCommand { get; }
    public IAsyncRelayCommand ExportPdfCommand { get; }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _reportService.GetMonthlyTotalsAsync(SelectedYear);
            MonthlyData.Clear();
            foreach (var item in data) MonthlyData.Add(item);

            // حساب الملخص
            GrandTotal = data.Sum(d => d.GrandTotal ?? 0);
            TotalQuotationCount = data.Sum(d => d.QuotationCount);
            MonthlyAverage = data.Count > 0 ? GrandTotal / data.Count : 0;
        }
        finally { IsLoading = false; }
    }
}
```

---

## 8. طباعة وتصدير PDF للتقارير

### 8.1 الآلية العامة

```
زر طباعة → PrintService.PrintReport(reportData, reportType) → FixedDocument → PrintDialog
زر PDF    → PdfService.ExportReportToPdfAsync(reportData, reportType, filePath) → QuestPDF → ملف
```

### 8.2 PrintService — إضافة دالة طباعة التقارير

```csharp
public interface IPrintService
{
    bool PrintQuotation(int quotationId, PrintMode printMode);
    bool PrintReport<T>(List<T> reportData, ReportType reportType);
}

public enum ReportType
{
    SupplierComparison,  // D1
    TopItems,            // D2
    QuotationHistory,    // D3
    MonthlyTotal         // D4
}
```

**طباعة D1 (مقارنة أسعار الموردين):**
- FixedDocument مع `FixedPage` بحجم A4 Landscape
- جدول HTML-like (Grid) بـ 9 أعمدة
- صف الرأس: خلفية داكنة + نص أبيض
- تلوين صف أفضل سعر باللون الأخضر الفاتح
- تذييل مع تاريخ الطباعة

**طباعة D2 (أكثر القطع طلباً):**
- A4 Portrait
- جدول بـ 4 أعمدة
- أيقونات المراكز الثلاثة الأولى (نص: 🥇🥈🥉) أو ألوان

**طباعة D3 (سجل العروض):**
- A4 Portrait
- جدول بـ 6 أعمدة
- ألوان الحالة (دوائر ملونة أو أشرطة جانبية)

**طباعة D4 (الإجمالي الشهري):**
- A4 Portrait
- جدول بـ 6 أعمدة + صف المجموع
- بطاقات ملخص في أعلى الصفحة (اختياري — يمكن حذفها في الطباعة والاكتفاء بالجدول)

### 8.3 PdfService — إضافة دالة تصدير التقارير

```csharp
public interface IPdfService
{
    bool ExportQuotationToPdfAsync(int quotationId, string filePath, bool showPrices);
    bool ExportReportToPdfAsync<T>(List<T> reportData, ReportType reportType, string filePath);
}
```

- يُستخدم QuestPDF (كما هو مقرر في Architecture)
- مسار الحفظ الافتراضي: `%USERPROFILE%\Desktop\` أو آخر مسار استخدمه المستخدم
- اسم الملف الافتراضي: `{ReportType}_{Date}.pdf` مثال: `SupplierComparison_2026-07-13.pdf`
- يُفتح `SaveFileDialog` قبل الحفظ

### 8.4 قواعد الطباعة والـ PDF

| القاعدة | الشرح |
|:--------|:------|
| **لا طباعة لجداول فارغة** | إذا `reportData.Count == 0` → `MessageBox.Show("لا توجد بيانات للطباعة.")` |
| **تاريخ الطباعة** | يُضاف في تذييل كل صفحة |
| **اتجاه الورق** | D1: Landscape. D2/D3/D4: Portrait |
| **الهوامش** | 20mm من كل جانب |
| **الترويسة** | شعار التطبيق (من `Setting.LetterheadLogoPath`) + اسم التقرير |
| **ترقيم الصفحات** | "صفحة 1 من N" في التذييل |
| **التعريب** | جميع النصوص بالعربية (RTL) في الطباعة |

---

## 9. تكامل شاشة S5 (Reports Screen)

### 9.1 هيكل S5

كما هو موثق في `07_SCREENS_AND_UI_STRUCTURE.md` — Section 6:

```xml
<!-- ReportsView.xaml — الهيكل العام -->
<UserControl ...>
    <Grid>
        <!-- شريط أزرار التقارير -->
        <StackPanel Orientation="Horizontal" Grid.Row="0">
            <Button Content="📊 مقارنة أسعار الموردين"
                    Command="{Binding ShowReportCommand}"
                    CommandParameter="D1" />
            <Button Content="🔥 أكثر القطع طلباً"
                    Command="{Binding ShowReportCommand}"
                    CommandParameter="D2" />
            <Button Content="📋 سجل العروض"
                    Command="{Binding ShowReportCommand}"
                    CommandParameter="D3" />
            <Button Content="💰 الإجمالي الشهري"
                    Command="{Binding ShowReportCommand}"
                    CommandParameter="D4" />
        </StackPanel>

        <!-- منطقة عرض التقرير الديناميكي -->
        <ContentControl Content="{Binding CurrentReportViewModel}"
                        Grid.Row="1">
            <ContentControl.Resources>
                <DataTemplate DataType="{x:Type vm:SupplierComparisonViewModel}">
                    <views:SupplierComparisonView />
                </DataTemplate>
                <DataTemplate DataType="{x:Type vm:TopItemsViewModel}">
                    <views:TopItemsView />
                </DataTemplate>
                <DataTemplate DataType="{x:Type vm:QuotationHistoryViewModel}">
                    <views:QuotationHistoryView />
                </DataTemplate>
                <DataTemplate DataType="{x:Type vm:MonthlyTotalViewModel}">
                    <views:MonthlyTotalView />
                </DataTemplate>
            </ContentControl.Resources>
        </ContentControl>
    </Grid>
</UserControl>
```

### 9.2 ReportsViewModel

```csharp
public partial class ReportsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private object? _currentReportViewModel;

    [ObservableProperty]
    private bool _isLoading;

    public ReportsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private void ShowReport(string reportCode)
    {
        IsLoading = true;

        CurrentReportViewModel = reportCode switch
        {
            "D1" => _serviceProvider.GetRequiredService<SupplierComparisonViewModel>(),
            "D2" => _serviceProvider.GetRequiredService<TopItemsViewModel>(),
            "D3" => _serviceProvider.GetRequiredService<QuotationHistoryViewModel>(),
            "D4" => _serviceProvider.GetRequiredService<MonthlyTotalViewModel>(),
            _ => null
        };

        IsLoading = false;
    }
}
```

### 9.3 تسجيل الخدمات في App.xaml.cs

```csharp
// تسجيل الخدمات
services.AddTransient<IReportService, ReportService>();
services.AddTransient<IPrintService, PrintService>();
services.AddTransient<IPdfService, PdfService>();

// تسجيل ViewModels التقارير (Transient — يُنشأ جديد كل مرة)
services.AddTransient<SupplierComparisonViewModel>();
services.AddTransient<TopItemsViewModel>();
services.AddTransient<QuotationHistoryViewModel>();
services.AddTransient<MonthlyTotalViewModel>();
services.AddTransient<ReportsViewModel>();
```

### 9.4 تفاعل المستخدم مع النتائج

| الإجراء | الوصف |
|:--------|:------|
| **اختيار تقرير** | يضغط زر من الأربعة → يظهر ContentControl مع ViewModel و View الخاصين بالتقرير |
| **تغيير المدخلات** | أي تغيير في ComboBox/DatePicker/NumericUpDown → `PropertyChanged` → `LoadAsync()` تلقائياً |
| **طباعة** | زر في كل View → يستدعي `IPrintService.PrintReport()` |
| **PDF** | زر في كل View → `SaveFileDialog` ← `IPdfService.ExportReportToPdfAsync()` |
| **حالة فارغة** | `EmptyMessage` TextBlock يظهر عند عدم وجود بيانات |
| **خطأ** | `ErrorMessage` TextBlock مع زر "إعادة المحاولة" |

---

## 10. سيناريوهات الاختبار والتأكيد

### 10.1 سيناريوهات D1 — Supplier Price Comparison

| # | السيناريو | الخطوات | النتيجة المتوقعة |
|:-:|:----------|:--------|:-----------------|
| 1 | عرض كامل بأسعار | اختر عرضاً مع 5 قطع وأسعار لكل الموردين | جدول ب 5 صفوف، أفضل سعر محدد باللون الأخضر |
| 2 | عرض بأسعار جزئية | اختر عرضاً: م1 له سعر، م2/m3 بدون | أعمدة فارغة تظهر `—`، أفضل سعر هو الموجود |
| 3 | عرض بلا أسعار (Draft) | اختر عرضاً بحالة Draft | رسالة تنبيه "لم يتم إدخال أسعار بعد" |
| 4 | عرض بلا بنود | اختر عرضاً محفوظاً بدون قطع | رسالة "لا توجد قطع في هذا العرض" |
| 5 | طباعة/PDF | اضغط طباعة بعد تحميل التقرير | مستند A4 Landscape مع الجدول كاملاً |
| 6 | أفضل سعر مكرر | موردان بنفس السعر | يتم تمييز الأول فقط (أو كلاهما — يُقرر أثناء التنفيذ) |

### 10.2 سيناريوهات D2 — Most Requested Items

| # | السيناريو | الخطوات | النتيجة المتوقعة |
|:-:|:----------|:--------|:-----------------|
| 1 | تقرير افتراضي | فتح التقرير بدون تغيير إعدادات | Top 10 قطع مرتبة |
| 2 | تغيير `TopN` | تغيير القيمة من 10 إلى 5 | يتغير الجدول إلى 5 قطع |
| 3 | تصفية تاريخية | اختيار نطاق شهر واحد فقط | تظهر فقط القطع المطلوبة في ذلك الشهر |
| 4 | نطاق بدون نتائج | اختيار تاريخ مستقبلي | رسالة "لا توجد قطع ضمن النطاق الزمني" |
| 5 | `fromDate` > `toDate` | إدخال تاريخ بداية بعد تاريخ النهاية | رسالة تحذير، لا استعلام |

### 10.3 سيناريوهات D3 — Quotation History

| # | السيناريو | الخطوات | النتيجة المتوقعة |
|:-:|:----------|:--------|:-----------------|
| 1 | سجل كامل | فتح التقرير بدون فلاتر | جميع العروض مرتبة، الأحدث أولاً |
| 2 | تصفية حسب الحالة | اختيار "مطبوع" فقط | تظهر العروض المطبوعة فقط |
| 3 | نطاق تاريخ | اختيار أسبوع محدد | عروض ذلك الأسبوع فقط |
| 4 | عرض بحالة Draft | الموجود في القائمة | إجمالي `—` (شرطة) |
| 5 | طباعة | مع 50+ عرض | صفحات متعددة مع ترقيم |

### 10.4 سيناريوهات D4 — Monthly Total

| # | السيناريو | الخطوات | النتيجة المتوقعة |
|:-:|:----------|:--------|:-----------------|
| 1 | سنة كاملة | فتح التقرير للسنة الحالية | جدول 12 شهر، بطاقات ملخص |
| 2 | سنة بدون عروض | اختيار سنة 2020 (لا عروض) | رسالة "لا توجد عروض في السنة المحددة" |
| 3 | المسودات غير محتسبة | إنشاء عرض Draft في مارس | مارس لا يحتسب في الإجمالي |
| 4 | الإجمالي السنوي | التحقق من صحة المجموع | `GrandTotal` = مجموع كل الأشهر |
| 5 | طباعة | طباعة التقرير | A4 Portrait مع جدول وصف الإجمالي |

---

## 11. Design Gaps

| # | الفجوة | النوع | التأثير | التوصية |
|:-:|:-------|:-----:|:--------|:--------|
| 1 | **D1 — اختيار العرض من S5** | 🎨 تجربة مستخدم | المستخدم يحتاج لاختيار عرض من قائمة. القائمة قد تكون طويلة (> 100 عرض). | إضافة ComboBox مع `IsEditable=True` + `TextSearch` للبحث السريع. أو تكبير القائمة المنسدلة (MaxDropDownHeight). |
| 2 | **D4 — عرض بدون عروض في شهر** | 🤔 قرار تصميم | هل نظهر صف الشهر بقيمة 0.00 أم نخفيه؟ | **مقترح:** إظهار جميع الأشهر 12 مع صفر — لأن المستخدم يريد رؤية جدول كامل للسنة. يمكن نقاشها مع العميل. |
| 3 | **حساب الإجمالي في D3** | 🤔 قرار تقني | الإجمالي: مجموع Supplier1Price فقط؟ أم Supplier1 + 2 + 3؟ | **مقترح:** مجموع الثلاثة أسعار (لأن كل سعر يمثل عرضاً من مورد مختلف). لكن هذا ليس دقيقاً. **الحل الأفضل:** إظهار 3 أعمدة منفصلة (إجمالي م1، م2، م3) بدلاً من عمود إجمالي واحد — أو إظهار إجمالي المورد الأول فقط كمرجع. |
| 4 | **D2 — متوسط السعر** | 🤔 قرار | هل متوسط السعر مفيد للمستخدم؟ العميل لم يطلبه صراحة. | **مقترح:** إضافته كعمود إضافي (اختياري) لأنه يضيف قيمة تحليلية دون تعقيد. |
| 5 | **طباعة التقارير — ترويسة** | 📄 مفقود | `07_SCREENS_AND_UI_STRUCTURE.md` لم يحدد تنسيق رأس التقرير في الطباعة | استخدام نفس تنسيق ترويسة عرض السعر: شعار + اسم الشركة + عنوان التقرير. يُنفذ كـ `ReportHeaderTemplate` مشترك. |
| 6 | **عدم وجود خطوط إرشادية لتصدير CSV** | 📄 غير مطلوب | العميل لم يطلب Excel/CSV | لا حاجة حالياً. يمكن إضافته في الإصدار الثاني. |

### 11.1 توصيات للتنفيذ

1. **ترتيب أولوية التقارير:** D3 (سجل العروض) → D1 (مقارنة الموردين) → D4 (الإجمالي الشهري) → D2 (أكثر القطع طلباً). لأن D3 يعتمد على `QuotationService` فقط ولا يحتاج علاقات معقدة.

2. **إعادة استخدام مكونات الطباعة:** إنشاء `ReportHelper` ثابت (static) يحتوي على دوال مساعدة لبناء `FixedDocument` مع رأس وتذييل موحدين لجميع التقارير (تقليل تكرار الكود).

3. **توحيد أسماء ملفات PDF:**
   - D1: `Comparison_Q-003.pdf`
   - D2: `TopItems_2026-07-13.pdf`
   - D3: `QuotationHistory_Jul2026.pdf`
   - D4: `MonthlyTotal_2026.pdf`

4. **التحميل البطيء (Lazy Load):** لا تحمّل بيانات أي تقرير عند فتح S5. التحميل يبدأ فقط عند ضغط المستخدم على زر التقرير. هذا يمنع 4 استعلامات متزامنة غير ضرورية.

5. **تجربة فارغة متسقة:** استخدام نفس `EmptyStateMessage` Template عبر جميع التقارير (نص + أيقونة) الموجود في `Styles/DataTemplates.xaml`.

---

## ختم الملف

| البند | الحالة |
|:------|:-------|
| **إعداد** | Software Designer Agent (مُصمم) |
| **تاريخ الإصدار** | 2026-07-13 |
| **الحالة** | ✅ Module Baseline Approved |
| **عدد التقارير المفصّلة** | **4 تقارير** (D1–D4) |
| **عدد الأسطر التقريبية** | ~1050 سطراً |
| **عدد DTOs المحددة** | 4 DTOs |
| **عدد دوال LINQ المفصّلة** | 4 دوال استعلام (معالجة في C#/LINQ) |
| **عدد سيناريوهات الاختبار** | 25 سيناريو |

---

**روابط ذات صلة:**
- [07_SCREENS_AND_UI_STRUCTURE.md](./07_SCREENS_AND_UI_STRUCTURE.md) — شاشة S5 (التقارير)
- [06_DATA_MODEL_PREPARATION.md](./06_DATA_MODEL_PREPARATION.md) — نموذج البيانات
- [08_TECHNICAL_ARCHITECTURE.md](./08_TECHNICAL_ARCHITECTURE.md) — القرارات المعمارية
- [APPLICATION_BLUEPRINT.md](./APPLICATION_BLUEPRINT.md) — المخطط العام
- [TERA_PROJECT_DECISION.md](../project-control/TERA_PROJECT_DECISION.md) — قرارات المشروع
