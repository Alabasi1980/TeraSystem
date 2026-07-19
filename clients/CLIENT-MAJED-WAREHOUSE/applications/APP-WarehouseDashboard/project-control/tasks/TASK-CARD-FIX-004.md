# TASK-CARD-FIX-004 — Fix Custom Date Filter (dateFrom / dateTo)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-FIX-004 |
| **النوع** | Bug Fix / Frontend + API |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## المشكلة

عند اختيار "مخصص" في شريط التاريخ:

```
Frontend: ?preset=custom  (لا يُرسل dateFrom/dateTo)
         ↓
API: ResolvePresetDates("custom") → null
         ↓
لا فلتر ← كل البيانات
```

---

## المطلوب — 2 ملفات

### 1. `Card.cshtml.cs` — استقبال dateFrom/dateTo

أضف معاملات اختيارية إلى `OnGetAsync`:

```csharp
public async Task<IActionResult> OnGetAsync(int id, string? preset, string? dateFrom, string? dateTo, CancellationToken cancellationToken)
```

حدّث `ResolvePresetDates`:

```csharp
private static DashboardService.DateRange? ResolvePresetDates(string? preset, string? dateFrom = null, string? dateTo = null)
{
    if (string.IsNullOrWhiteSpace(preset))
        return null;

    if (string.Equals(preset, "custom", StringComparison.OrdinalIgnoreCase))
    {
        if (!string.IsNullOrWhiteSpace(dateFrom) && !string.IsNullOrWhiteSpace(dateTo)
            && DateTime.TryParse(dateFrom, out var from) && DateTime.TryParse(dateTo, out var to))
        {
            // Include full end-of-day for the To date
            return new DashboardService.DateRange(from, to.AddDays(1).AddTicks(-1));
        }
        return null;
    }

    var today = DateTime.Today;
    return preset.ToLowerInvariant() switch
    {
        "today" => new DashboardService.DateRange(today, today.AddDays(1).AddTicks(-1)),
        "yesterday" => new DashboardService.DateRange(today.AddDays(-1), today.AddTicks(-1)),
        "7days" => new DashboardService.DateRange(today.AddDays(-6), today.AddDays(1).AddTicks(-1)),
        "30days" => new DashboardService.DateRange(today.AddDays(-29), today.AddDays(1).AddTicks(-1)),
        "month" => new DashboardService.DateRange(new DateTime(today.Year, today.Month, 1), today.AddDays(1).AddTicks(-1)),
        _ => null
    };
}
```

### 2. `Index.cshtml` — إرسال dateFrom/dateTo في طلب wdLoadCard

في دالة `wdLoadCard`، أضف `dateFrom` و `dateTo` كمعاملات إضافية عند `preset === 'custom'`:

```csharp
function wdLoadCard(id, showSkeleton) {
    var el = document.getElementById('card-' + id);
    if (!el) return;
    if (showSkeleton) {
        var body = el.querySelector('.wd-card__body');
        body.innerHTML = '<div class="wd-skeleton-wrap"><div class="wd-skel wd-skel--tall"></div></div>';
    }
    var preset = window.WD_DATE_PRESET || 'today';
    var url = '/api/dashboard/card/' + id + '?preset=' + encodeURIComponent(preset);
    
    // Custom dates: append dateFrom and dateTo from the date inputs
    if (preset === 'custom') {
        var dateFrom = document.getElementById('wd-date-from');
        var dateTo = document.getElementById('wd-date-to');
        if (dateFrom && dateTo && dateFrom.value && dateTo.value) {
            url += '&dateFrom=' + encodeURIComponent(dateFrom.value) + '&dateTo=' + encodeURIComponent(dateTo.value);
        }
    }
    
    fetch(url, { headers: { 'Accept': 'application/json' } })
        .then(function (r) { return r.json(); })
        .then(function (data) { wdRenderCard(data); })
        .catch(function (err) {
            var b = el.querySelector('.wd-card__body');
            if (b) b.innerHTML = wdErrorHtml('تعذر الاتصال بالخادم: ' + (err && err.message ? err.message : ''), id);
            showToast('error', 'تعذر تحميل بطاقة #' + id);
        });
}
```

DO NOT change any other part of wdLoadCard or any other function.
DO NOT change the auto-refresh code.

## Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Api\Dashboard\Card.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml
```

## Verification

- `dotnet build --no-restore`
- No C# errors (file locking is acceptable)

## Notes for Agent

- Read both files from disk first before editing
- Preserve all existing code, only modify the specific sections described
- The `dw-date-from` and `wd-date-to` input elements already exist in the HTML
