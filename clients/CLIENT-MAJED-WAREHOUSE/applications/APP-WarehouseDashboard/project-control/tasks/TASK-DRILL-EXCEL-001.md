# TASK-DRILL-EXCEL-001 — Excel منسق مع فلاتر لمودال التنقل العميق

> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** engineering-agent-dotnet
> **أولوية:** High

---

## 1. الوصف

إضافة تصدير Excel حقيقي (`.xlsx`) من مودال التنقل العميق مع:
- تنسيق احترافي (ألوان header، عرض أعمدة، borders)
- AutoFilter على كل الأعمدة
- صفوف مرقمة
- خلايا منسقة حسب نوع البيانات

---

## 2. التغييرات المطلوبة

### A. إضافة ClosedXML NuGet للـ Web project

```
dotnet add package ClosedXML --version 0.104.*
```

المسار:
`D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\WarehouseDashboard.Web.csproj`

### B. إضافة Export Handler في Drill.cshtml.cs

أضف دالة جديدة في `Pages/Api/Dashboard/Drill.cshtml.cs`:

```csharp
/// <summary>
/// GET /api/dashboard/drill/{cardId}/{level}/export?parentValue=...
/// Generates a formatted .xlsx file with the drill-down data.
/// </summary>
public async Task<IActionResult> OnGetExcelAsync(int cardId, int level, string? parentValue, CancellationToken cancellationToken)
{
    // 1. Get the drill data (reuse existing query logic)
    var data = await GetDrillDataAsync(cardId, level, parentValue, cancellationToken);
    
    if (data.Status != "success")
    {
        return Json(new { success = false, errorMessage = data.ErrorMessage ?? "لا توجد بيانات للتصدير." });
    }
    
    // 2. Build Excel using ClosedXML
    using var workbook = new ClosedXML.Excel.XLWorkbook();
    var ws = workbook.Worksheets.Add("Drill Data");
    
    // Set RTL direction for Arabic support
    ws.RightToLeft = true;
    
    // Column count
    var colCount = data.Columns.Count;
    var rowCount = data.Rows.Count;
    
    // Header row with styling
    for (int i = 0; i < colCount; i++)
    {
        var cell = ws.Cell(1, i + 1);
        cell.Value = data.Columns[i];
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
        cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#1F4E79");
        cell.Style.Border.SetOutsideBorder(ClosedXML.Excel.XLBorderStyleValues.Thin);
        cell.Style.Border.SetOutsideBorderColor(ClosedXML.Excel.XLColor.FromHtml("#D4E2F0"));
        cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
    }
    
    // Data rows
    for (int r = 0; r < rowCount; r++)
    {
        var row = data.Rows[r];
        for (int c = 0; c < colCount; c++)
        {
            var colName = data.Columns[c];
            var val = row.GetValueOrDefault(colName);
            var cell = ws.Cell(r + 2, c + 1);
            
            if (val == null)
            {
                cell.Value = "";
            }
            else if (val is int || val is long || val is short || val is byte)
            {
                cell.Value = Convert.ToInt64(val);
                cell.Style.NumberFormat.Format = "#,##0";
                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
            }
            else if (val is decimal || val is double || val is float)
            {
                cell.Value = Convert.ToDouble(val);
                cell.Style.NumberFormat.Format = "#,##0.00";
                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
            }
            else if (val is DateTime)
            {
                cell.Value = (DateTime)val;
                cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            }
            else
            {
                cell.Value = val.ToString() ?? "";
            }
            
            cell.Style.Border.SetOutsideBorder(ClosedXML.Excel.XLBorderStyleValues.Thin);
            cell.Style.Border.SetOutsideBorderColor(ClosedXML.Excel.XLColor.FromHtml("#E0E0E0"));
        }
    }
    
    // AutoFilter
    if (rowCount > 0 && colCount > 0)
    {
        ws.Range(1, 1, 1 + rowCount, colCount).SetAutoFilter();
    }
    
    // Column widths — auto-fit
    ws.Columns().AdjustToContents(1, 100);
    // Minimum width per column
    for (int c = 1; c <= colCount; c++)
    {
        var col = ws.Column(c);
        if (col.Width < 10) col.Width = 10;
        if (col.Width > 50) col.Width = 50;
    }
    
    // Freeze header row
    ws.SheetView.FreezeRows(1);
    
    // 3. Return as file download
    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Seek(0, SeekOrigin.Begin);
    
    var fileName = $"Drill_{cardId}_Level_{level}_{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
    return File(stream.ToArray(), 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
        fileName);
}

// Helper to reuse drill data logic
private async Task<DrillDataResult> GetDrillDataAsync(int cardId, int level, string? parentValue, CancellationToken ct)
{
    // Copy the existing drill query logic from OnGetAsync into this helper
    // OR refactor OnGetAsync to use this shared method
    // This should execute the same query and return the same DrillDataResult
}
```

**ملاحظة:** تحتاج `using ClosedXML.Excel;` في قائمة الـ usings.

### C. تحديث واجهة المستخدم

في `Index.cshtml`، أضف زر "Excel" في `wdRenderFooter()` بجانب زر CSV الموجود:

```javascript
// Excel export button
var excelBtn = document.createElement('button');
excelBtn.type = 'button';
excelBtn.className = 'wd-btn wd-btn--ghost';
excelBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/><line x1="8" y1="16" x2="16" y2="16"/><line x1="8" y1="12" x2="16" y2="12"/></svg> Excel';
excelBtn.addEventListener('click', function() { wdExportExcel(); });
footerEl.appendChild(excelBtn);
```

وأضف دالة `wdExportExcel()`:

```javascript
function wdExportExcel() {
  var st = window.__drillState;
  if (!st || !st.currentData) return;
  
  var cardId = st.cardId;
  var level = st.currentLevel;
  var parentValue = st.parentValueForCurrentLevel || '';
  
  var url = '/api/dashboard/drill/' + cardId + '/' + level + '/export?parentValue=' + encodeURIComponent(parentValue);
  window.location.href = url;
}
```

أو الأفضل — استخدم رابط مباشر:

```javascript
function wdExportExcel() {
  var st = window.__drillState;
  if (!st) return;
  var url = '/api/dashboard/drill/' + st.cardId + '/' + st.currentLevel + '/export?parentValue=' + encodeURIComponent(st.parentValueForCurrentLevel || '');
  window.open(url, '_blank');
}
```

---

## 3. ملفات التعديل

1. `D:\...\WarehouseDashboard.Web\WarehouseDashboard.Web.csproj` — إضافة ClosedXML
2. `D:\...\WarehouseDashboard.Web\Pages\Api\Dashboard\Drill.cshtml.cs` — إضافة `OnGetExcelAsync`
3. `D:\...\WarehouseDashboard.Web\Pages\Index.cshtml` — إضافة زر Excel + `wdExportExcel()`

---

## 4. Acceptance Criteria

1. ✅ ClosedXML مضاف للـ Web project (build PASS)
2. ✅ `GET .../drill/{cardId}/{level}/export` يُعيد ملف `.xlsx`
3. ✅ Excel بخلفية زرقاء للـ header + bold + AutoFilter
4. ✅ أرقام منسّقة (#,##0.00)، تواريخ (yyyy-MM-dd HH:mm)
5. ✅ Freeze header row + Right-to-Left
6. ✅ زر Excel في المودال (بجانب CSV)
7. ✅ `dotnet build` PASS

---

## 5. After Completion

أعد:
1. ملخص التغييرات (NuGet + API + Frontend)
2. `dotnet build` PASS
3. Auditor: NOT_REQUIRED
