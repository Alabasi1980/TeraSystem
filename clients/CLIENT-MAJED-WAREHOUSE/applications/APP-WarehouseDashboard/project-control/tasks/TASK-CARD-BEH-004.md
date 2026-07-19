# TASK-CARD-BEH-004 — DateFilterMode: Fixed and Relative Modes

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-BEH-004 |
| **المجموعة** | CARD-DESIGN-EXECUTION |
| **النوع** | Backend / C# (DashboardService) |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase B / CARD-BEH-04 |
| **القرارات المعتمدة** | DateFilterMode = كل البطاقات (Majed confirmed) |

---

## 1. الهدف

تفعيل `DateFilterMode` لـ **fixed** و **relative**، بحيث:

- `fixed`: البطاقة تستخدم `FixedStartDate` و `FixedEndDate` من إعداداتها **بغض النظر عن فلتر الداشبورد**
- `relative`: البطاقة تستخدم `RelativeDays` من إعداداتها **بغض النظر عن فلتر الداشبورد**
- `dashboard`: تتبع فلتر الداشبورد (شغال حالياً ✅)

---

## 2. المنطق

في `GetCardDataAsync` من `DashboardService.cs`، قبل استدعاء `BuildSql`:

```csharp
// DateFilterMode: resolve effective date range based on card's mode
DateRange? effectiveDateRange = dateRange; // From filter bar (dashboard mode)

if (string.Equals(card.DateFilterMode, "fixed", StringComparison.OrdinalIgnoreCase)
    && !string.IsNullOrWhiteSpace(card.FixedStartDate)
    && !string.IsNullOrWhiteSpace(card.FixedEndDate))
{
    // Fixed dates from card settings — ignore filter bar
    if (DateTime.TryParse(card.FixedStartDate, out var from) 
        && DateTime.TryParse(card.FixedEndDate, out var to))
    {
        effectiveDateRange = new DateRange(from, to.AddDays(1).AddTicks(-1));
    }
}
else if (string.Equals(card.DateFilterMode, "relative", StringComparison.OrdinalIgnoreCase))
{
    // Relative days from card settings — ignore filter bar
    var days = card.RelativeDays > 0 ? card.RelativeDays : 30;
    effectiveDateRange = new DateRange(DateTime.UtcNow.AddDays(-days), DateTime.UtcNow);
}
// else "dashboard": use filter bar dateRange as-is
```

ثم استخدم `effectiveDateRange` بدلاً من `dateRange` في باقي الدالة:

```csharp
var sql = BuildSql(card, effectiveDateRange);
...
var kpiQueries = KpiQueryBuilder.Build(card, effectiveDateRange);
```

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs
```

## 4. Acceptance Criteria

| # | المعيار | Status |
|---|---|---|
| AC-1 | بطاقة بـ `DateFilterMode = "fixed"` + تواريخ → تعرض بيانات النطاق المحدد بغض النظر عن فلتر الداشبورد | ☐ |
| AC-2 | بطاقة بـ `DateFilterMode = "relative"` + `RelativeDays = 7` → تعرض آخر 7 أيام بغض النظر عن فلتر الداشبورد | ☐ |
| AC-3 | بطاقة بـ `DateFilterMode = "dashboard"` → لا تتأثر (تستخدم فلتر الداشبورد كالسابق) | ☐ |
| AC-4 | `DateFilterMode = null` / empty → تعامل كـ "dashboard" | ☐ |
| AC-5 | FixedStartDate/FixedEndDate غير صالحة → لا فلتر (fallback) | ☐ |
| AC-6 | `dotnet build --no-restore` ناجح | ☐ |

## 5. Pre-Execution Gate

**Result:** PASS
- ملف واحد
- منطق واضح
- لا تغيير في API ولا Frontend ولا DB
- لا تأثير على البطاقات الحالية
