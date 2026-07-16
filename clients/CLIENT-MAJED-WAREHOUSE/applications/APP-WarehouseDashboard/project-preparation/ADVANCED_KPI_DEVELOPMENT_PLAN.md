# خطة تطوير البطاقة المتقدمة — Advanced KPI Card

> **تاريخ الإنشاء:** 2026-07-15
> **المشروع:** WarehouseDashboard — الماجد لادارة المستودعات
> **الحالة:** Draft — في انتظار المراجعة والموافقة

---

## 1. المشكلة الحالية

### 1.1 نوع KPI الحالي
بطاقة KPI الحالية تعرض **قيمة واحدة فقط**:

```
┌─────────────────────┐
│  إجمالي المخزون      │
│  1,250,000          │
└─────────────────────┘
```

لا توجد:
- نسبة تغير عن الفترة السابقة
- منحنى Sparkline
- إجمالي عام
- فلتر زمني
- ربط بتعيينات متعددة

### 1.2 خطأ تسمية مصدر البيانات
الخطوة الثانية من المعالج تحتوي:

```html
<option value="OracleTable">جدول/عرض Oracle</option>
```

**هذا خاطئ.** البيانات المُrence她在 SQL Server (بعد المزامنة من Oracle)، وليس في Oracle مباشرة.

### 1.3 بطاقات KPI المتقدمة المطلوبة
المستخدم يريد بطاقة واحدة تعرض:

```
┌─────────────────────────────────────────────────────┐
│  قيمة المخزون للفترة المحددة                        │
│  1,250,000 د.أ                                      │
│  01/06/2026 - 30/06/2026                            │
│                                                      │
│  +8.4% عن الشهر السابق                              │
│                                                      │
│  [───── Sparkline: 6 أشهر ─────]                    │
│                                                      │
│  إجمالي المخزون الكلي: 4,980,000 د.أ                │
└─────────────────────────────────────────────────────┘
```

---

## 2. الحل المقترح

### 2.1 تطوير نوع KPI إلى 3 أوضاع

| الوضع | الوصف | التعقيد |
|---|---|---|
| **KPI بسيط** | قيمة واحدة فقط (الحالي) | بسيط |
| **KPI مع تغير** | قيمة + نسبة تغير عن الفترة السابقة | متوسط |
| **KPI شامل** | قيمة + تغير + Sparkline + إجمالي عام | متقدم |

### 2.2 تصحيح تسمية مصدر البيانات

| الحالي | الصحيح |
|---|---|
| `OracleTable` | `SqlTable` |
| `جدول/عرض Oracle` | `جدول/عرض SQL Server` |
| `ابحث أو اختر جدولاً...` | `ابحث أو اختر جدولاً من SQL Server...` |

### 2.3 بطاقة KPI المتعددة المصادر

بطاقة KPI المتقدمة تحتاج **أكثر من مصدر بيانات واحد**:

```
البطاقة الواحدة
├─ مصدر القيمة الرئيسية (لفترة محددة)
├─ مصدر القيمة السابقة (لفترة سابقة)
├─ مصدر Sparkline (لعدة فترات)
└─ مصدر الإجمالي العام (بدون فلتر زمني)
```

**العلاقة:** كل هذهالمصادر تأتي **من نفس الجدول** في SQL Server، لكن بفلاتر مختلفة.

---

## 3. نموذج البيانات المقترح

### 3.1 تعديل DashboardCard Model

```csharp
// === القيمة الرئيسية ===
public string ValueColumn { get; set; } = string.Empty;       // اسم العمود الرقمي
public string DateColumn { get; set; } = string.Empty;        // اسم عمود التاريخ
public string CategoryColumn { get; set; } = string.Empty;    // اسم عمود التصنيف (اختياري)

// === إعدادات KPI المتقدم ===
public string KpiMode { get; set; } = "simple";               // simple / withChange / composite
public bool ShowChange { get; set; } = false;                  // إظهار نسبة التغير
public string ChangeSource { get; set; } = "previousPeriod";   // previousPeriod / previousMonth / previousYear / customQuery
public bool ShowSparkline { get; set; } = false;               // إظهار منحنى صغير
public int SparklineMonths { get; set; } = 6;                  // عدد أشهر Sparkline
public bool ShowGrandTotal { get; set; } = false;              // إظهار الإجمالي العام
public string GrandTotalSource { get; set; } = "sameTable";    // sameTable / customQuery / savedQuery

// === فلتر التاريخ ===
public string DateFilterMode { get; set; } = "dashboard";      // dashboard / fixed / relative
public string FixedStartDate { get; set; } = string.Empty;     // تاريخ ثابت بداية
public string FixedEndDate { get; set; } = string.Empty;       // تاريخ ثابت نهاية
public int RelativeDays { get; set; } = 30;                    // عدد الأيام النسبي
```

### 3.2 نموذج SQL لكل مصدر

```sql
-- القيمة الرئيسية (لفترة محددة)
SELECT SUM({ValueColumn}) AS MainValue
FROM [{TableName}]
WHERE {DateColumn} BETWEEN @StartDate AND @EndDate

-- القيمة السابقة (لفترة سابقة)
SELECT SUM({ValueColumn}) AS PreviousValue
FROM [{TableName}]
WHERE {DateColumn} BETWEEN @PreviousStartDate AND @PreviousEndDate

-- Sparkline (لعدة فترات)
SELECT FORMAT({DateColumn}, 'yyyy-MM') AS Month,
       SUM({ValueColumn}) AS MonthlyValue
FROM [{TableName}]
WHERE {DateColumn} >= DATEADD(MONTH, -{SparklineMonths}, @StartDate)
GROUP BY FORMAT({DateColumn}, 'yyyy-MM')
ORDER BY Month

-- الإجمالي العام (بدون فلتر)
SELECT SUM({ValueColumn}) AS GrandTotal
FROM [{TableName}]
```

---

## 4. هيكل المعالج المقترح

### الخطوة 1 — النوع

```
┌─────────────────────────────────────────────────┐
│  اختر نوع البطاقة                               │
│                                                  │
│  [KPI] [Bar] [Line] [Pie] [Table] [Gauge]       │
│                                                  │
│  ──── إذا KPI، تظهر خيارات إضافية ────          │
│                                                  │
│  نمط KPI:                                        │
│  ○ بسيط (قيمة واحدة)                            │
│  ○ مع تغير (قيمة + نسبة تغير)                   │
│  ● شامل (قيمة + تغير + Sparkline + إجمالي)      │
│                                                  │
└─────────────────────────────────────────────────┘
```

### الخطوة 2 — المصدر

```
┌─────────────────────────────────────────────────┐
│  اختر مصدر البيانات                              │
│                                                  │
│  نوع المصدر:                                     │
│  [قالب جاهز] [جدول SQL Server] [SQL مخصص]      │
│                                                  │
│  ──── إذا قالب جاهز ────                        │
│  شبكة القوالب المتاحة                            │
│                                                  │
│  ──── إذا جدول SQL Server ────                   │
│  اختر الجدول: [Dropdown من API]                  │
│  الحقل الرقمي: [Dropdown يُملأ تلقائياً]         │
│  حقل التاريخ: [Dropdown يُملأ تلقائياً]          │
│  حقل التصنيف: [Dropdown اختياري]                 │
│                                                  │
│  ──── إذا SQL مخصص ────                         │
│  استعلام SQL رئيسي: [TextArea]                   │
│  استعلام القيمة السابقة: [TextArea]              │
│  استعلام Sparkline: [TextArea]                   │
│  استعلام الإجمالي: [TextArea]                    │
│                                                  │
└─────────────────────────────────────────────────┘
```

### الخطوة 3 — الحقول الأساسية

```
┌─────────────────────────────────────────────────┐
│  الحقول الأساسية                                │
│                                                  │
│  العنوان: [إجمالي المخزون]                      │
│  اسم العرض: [قيمة المخزون]                      │
│  الوصف: [وصف اختياري]                           │
│                                                  │
│  ──── إعدادات KPI المتقدم (إذا كان KPI شامل) ────│
│                                                  │
│  ✓ إظهار نسبة التغير                            │
│    مقارنة مع: [الفترة السابقة ▼]                │
│                                                  │
│  ✓ إظهار منحنى Sparkline                        │
│    عدد الأشهر: [6 ▼]                            │
│                                                  │
│  ✓ إظهار الإجمالي العام                          │
│    مصدر الإجمالي: [نفس الجدول بدون فلتر ▼]      │
│                                                  │
│  ✓ فلتر التاريخ من الداشبورد                    │
│                                                  │
└─────────────────────────────────────────────────┘
```

### الخطوة 4 — الشكل

```
┌─────────────────────────────────────────────────┐
│  إعدادات الشكل                                  │
│                                                  │
│  الحجم: [4] × [3]                               │
│  لوحة الألوان: [Primary]                         │
│  تحديث تلقائي: [5 دقائق]                        │
│                                                  │
│  ──── تنقيّط Drill-down (اختياري) ────           │
│                                                  │
│  ✓ تفعيل التنقّل العميق                        │
│  مستوى التنقّل: [تفصيل الفترة ▼]                │
│                                                  │
└─────────────────────────────────────────────────┘
```

---

## 5. نموذج SQL للمعاينة

### 5.1 KPI بسيط

```sql
-- استعلام واحد فقط
SELECT SUM(Quantity) AS MainValue
FROM [stg_WarehouseStock]
WHERE ItemDate BETWEEN @StartDate AND @EndDate
```

### 5.2 KPI مع تغير

```sql
-- استعلام رئيسي
SELECT SUM(Quantity) AS MainValue
FROM [stg_WarehouseStock]
WHERE ItemDate BETWEEN @StartDate AND @EndDate

-- استعلام سابق
SELECT SUM(Quantity) AS PreviousValue
FROM [stg_WarehouseStock]
WHERE ItemDate BETWEEN @PreviousStartDate AND @PreviousEndDate
```

### 5.3 KPI شامل

```sql
-- القيمة الرئيسية
SELECT SUM(Quantity) AS MainValue
FROM [stg_WarehouseStock]
WHERE ItemDate BETWEEN @StartDate AND @EndDate

-- القيمة السابقة
SELECT SUM(Quantity) AS PreviousValue
FROM [stg_WarehouseStock]
WHERE ItemDate BETWEEN @PreviousStartDate AND @PreviousEndDate

-- Sparkline
SELECT FORMAT(ItemDate, 'yyyy-MM') AS Month,
       SUM(Quantity) AS MonthlyValue
FROM [stg_WarehouseStock]
WHERE ItemDate >= DATEADD(MONTH, -6, @StartDate)
GROUP BY FORMAT(ItemDate, 'yyyy-MM')
ORDER BY Month

-- الإجمالي العام
SELECT SUM(Quantity) AS GrandTotal
FROM [stg_WarehouseStock]
```

---

## 6. هيكل API المقترح

### 6.1 API معاينة KPI متعدد

```json
POST /api/dashboard/cardbuilder/kpi-preview

Request:
{
  "chartType": "KPI",
  "kpiMode": "composite",
  "tableName": "stg_WarehouseStock",
  "valueColumn": "Quantity",
  "dateColumn": "ItemDate",
  "startDate": "2026-06-01",
  "endDate": "2026-06-30",
  "sparklineMonths": 6,
  "showGrandTotal": true
}

Response:
{
  "status": "success",
  "mainValue": 1250000,
  "previousValue": 1153000,
  "changePercent": 8.4,
  "changeDirection": "up",
  "sparklineData": [
    { "month": "2026-01", "value": 980000 },
    { "month": "2026-02", "value": 1020000 },
    { "month": "2026-03", "value": 1100000 },
    { "month": "2026-04", "value": 1150000 },
    { "month": "2026-05", "value": 1153000 },
    { "month": "2026-06", "value": 1250000 }
  ],
  "grandTotal": 4980000,
  "unit": "د.أ"
}
```

### 6.2 API تنفيذ KPI

```json
POST /api/dashboard/card/{id}/kpi-data

Request:
{
  "startDate": "2026-06-01",
  "endDate": "2026-06-30",
  "filters": { "WarehouseId": "1" }
}

Response:
{
  "cardId": 1,
  "kpiMode": "composite",
  "mainValue": 1250000,
  "previousValue": 1153000,
  "changePercent": 8.4,
  "changeDirection": "up",
  "sparklineData": [...],
  "grandTotal": 4980000,
  "period": "01/06/2026 - 30/06/2026"
}
```

---

## 7. هيكل التخزين

### 7.1 في جدول DashboardCards

```json
{
  "Id": 1,
  "Title": "إجمالي المخزون",
  "ChartType": "KPI",
  "KpiMode": "composite",
  "SqlQuery": "SELECT SUM(Quantity) AS MainValue FROM [stg_WarehouseStock] WHERE ItemDate BETWEEN @StartDate AND @EndDate",
  "ValueColumn": "Quantity",
  "DateColumn": "ItemDate",
  "ShowChange": true,
  "ChangeSource": "previousPeriod",
  "ShowSparkline": true,
  "SparklineMonths": 6,
  "ShowGrandTotal": true,
  "GrandTotalSource": "sameTable",
  "DateFilterMode": "dashboard"
}
```

### 7.2 في DashboardService

```csharp
public async Task<CardDataResult> GetKpiDataAsync(DashboardCard card, DateRange range)
{
    var result = new CardDataResult();

    // 1. القيمة الرئيسية
    result.MainValue = await ExecuteKpiQuery(card.SqlQuery, range);

    // 2. القيمة السابقة
    if (card.ShowChange)
    {
        result.PreviousValue = await ExecuteKpiQuery(BuildPreviousQuery(card), range.Previous);
        result.ChangePercent = CalculateChange(result.MainValue, result.PreviousValue);
    }

    // 3. Sparkline
    if (card.ShowSparkline)
    {
        result.SparklineData = await ExecuteSparklineQuery(card, range);
    }

    // 4. الإجمالي العام
    if (card.ShowGrandTotal)
    {
        result.GrandTotal = await ExecuteKpiQuery(BuildGrandTotalQuery(card));
    }

    return result;
}
```

---

## 8. قائمة المهام

### المرحلة الأولى — التصحيح والتأسيس

| المعرف | الوصف | الأولوية | النطاق |
|---|---|---|---|
| TASK-KPI-001 | تصحيح تسمية `OracleTable` → `SqlTable` في المعالج والكود | Critical | UI + Backend |
| TASK-KPI-002 | إضافة حقول KPI الجديدة لـ `DashboardCard` Model | Critical | Backend |
| TASK-KPI-003 | إنشاء EF Migration جديد | Critical | Database |
| TASK-KPI-004 | تحديث `CardEditorInput` بالحقول الجديدة | High | Backend |

### المرحلة الثانية — KPI بسيط ومتغير

| المعرف | الوصف | الأولوية | النطاق |
|---|---|---|---|
| TASK-KPI-005 | تعديل الخطوة 1 لدعم أوضاع KPI الثلاثة | High | UI |
| TASK-KPI-006 | إضافة حقول الخطوة 2 لاختيار الأعمدة | High | UI |
| TASK-KPI-007 | تعديل `DashboardService` لتنفيذ استعلامات KPI المتعددة | High | Backend |
| TASK-KPI-008 | تعديل `CardBuilderService` للمعاينة المتقدمة | High | Backend |

### المرحلة الثالثة — KPI شامل

| المعرف | الوصف | الأولوية | النطاق |
|---|---|---|---|
| TASK-KPI-009 | إضافة Sparkline support في Backend | High | Backend |
| TASK-KPI-010 | إضافة Sparkline support في Frontend (Syncfusion) | High | UI |
| TASK-KPI-011 | إضافة Grand Total support | Medium | Full Stack |
| TASK-KPI-012 | ربط فلتر التاريخ من الداشبورد | High | Full Stack |

### المرحلة الرابعة — التنقّل العميق

| المعرف | الوصف | الأولوية | النطاق |
|---|---|---|---|
| TASK-KPI-013 | تصميم Drill-down المتعدد لبطاقة KPI شاملة | High | Design |
| TASK-KPI-014 | تنفيذ Drill-down في Backend | High | Backend |
| TASK-KPI-015 | تنفيذ Drill-down في Frontend | High | UI |

---

## 9. معايير القبول النهائية

| # | المعيار |
|---|---|
| AC-1 | `OracleTable` تمت تسميتها `SqlTable` في كل مكان |
| AC-2 | KPI بسيط يعرض قيمة واحدة فقط (الحالي) |
| AC-3 | KPI مع تغير يعرض قيمة + نسبة تغير + اتجاه |
| AC-4 | KPI شامل يعرض قيمة + تغير + Sparkline + إجمالي |
| AC-5 | فلتر التاريخ من الداشبورد يُطبق على البطاقة |
| AC-6 | Sparkline يعرض 6/12 شهر بنجاح |
| AC-7 | Drill-down يعمل لكل جزء من البطاقة |
| AC-8 | `dotnet build -c Release` ينجح بدون أخطاء |
| AC-9 | لا توجد أسرار في الكود |

---

## 10. ملاحظات تقنية مهمة

### 10.1 علاقة البيانات
```
البطاقة الواحدة ← جدول واحد في SQL Server
                     ├─ استعلام رئيسي (لفترة محددة)
                     ├─ استعلام سابق (لفترة سابقة)
                     ├─ استعلام Sparkline (لعدة فترات)
                     └─ استعلام إجمالي (بدون فلتر)
```

### 10.2فلتر التاريخ
```
الداشبورد يُرسل تاريخ البداية والنهاية
  ↓
البطاقة تطبق الفلتر على استعلامها
  ↓
إذا لم يوجد فلتر → تستخدم آخر 30 يوم كقيمة افتراضية
```

### 10.3 الأداء
```
- Sparkline: حد أقصى 12 استعلام شهرياً
- Grand Total: استعلام واحد فقط
- Cache: يمكن تخزين النتائج لمدة 5 دقائق
```

---

## 11. التسلسل الزمني المقترح

```
الأسبوع 1: المرحلة الأولى (التصحيح والتأسيس)
الأسبوع 2: المرحلة الثانية (KPI بسيط ومتغير)
الأسبوع 3: المرحلة الثالثة (KPI شامل)
الأسبوع 4: المرحلة الرابعة (التنقّل العميق)
```

---

## 12. المخاطر والحلول

| المخاطر | الحل |
|---|---|
| تعقيد KPI شامل | تطوير بشكل تدريجي: بسيط → متغير → شامل |
| أداء الاستعلامات المتعددة | Cache + حد أقصى للصفوف |
| فلتر التاريخ المعقد | واجهة بسيطة في المعالج + منطق واضح في Backend |
| Drill-down المتعدد | تصميم مبدئي واضح قبل التنفيذ |

---

> **الحالة:** Draft — في انتظار مراجعة المستخدم والموافقة على الخطة
> **التالي:** بعد الموافقة → إنشاء TASK-COD-KPI-001 (المرحلة الأولى)
