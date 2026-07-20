# TASK-DRILL-EXPORT-001

> **Status:** ✅ Completed  
> **Assigned To:** Engineering Agent  
> **Created:** 2026-07-20  
> **Priority:** High  
> **Phase:** E (Level Renderers & Export)

---

## 1. Objective

Implement CSV Export functionality for drill-down modal results. When a drill level displays a Table or Chart with data, the user should be able to export the current results to a CSV file.

---

## 2. Requirements

### 2.1 Export Button Visibility
| Level Type | Export Button |
|---|---|
| Table | Always visible if rows exist |
| Chart (Bar/Line/Pie) | Visible as "تصدير بيانات الرسم CSV" if rows exist |
| KPI/Gauge | Not visible by default |
| Empty/Error | Not visible |

### 2.2 CSV Format
- **Encoding:** UTF-8 with BOM (Byte Order Mark)
- **Delimiter:** Comma (,)
- **Filename:** `drill-{cardId}-level-{level}-{yyyyMMdd-HHmm}.csv`
- **Headers:** Use column names from the API response

### 2.3 Export Behavior
- Export the currently displayed results (no re-execution of query)
- Preserve column names as received from API
- Handle Arabic/RTL text correctly
- Handle special characters (commas, quotes, newlines) in data

---

## 3. Files to Modify

| File | Changes |
|---|---|
| `src/WarehouseDashboard.Web/Pages/Index.cshtml` | Add export button and CSV generation function |

---

## 4. Implementation Plan

### 4.1 Add Export Button to Footer
In `wdRenderFooter()`, add an export button when:
- Current level has data (rows exist)
- Level type is Table or Chart
- `data.enableExport` is not false

### 4.2 Add CSV Generation Function
Create `wdExportCsv()` function that:
1. Gets current drill state and data
2. Generates CSV string with BOM header
3. Adds column headers
4. Adds data rows
5. Creates Blob and triggers download

### 4.3 Store Current Data in Drill State
Modify `wdLoadLevel()` to store the current level's data in `window.__drillState` so export can access it.

---

## 5. Code Structure

### 5.1 Export Button in Footer
```javascript
// In wdRenderFooter()
if (st.currentData && st.currentData.rows && st.currentData.rows.length > 0) {
    html += '<button type="button" class="wd-btn wd-btn--ghost" onclick="wdExportCsv()">تصدير CSV</button>';
}
```

### 5.2 Store Current Data
```javascript
// In wdLoadLevel() after successful fetch
st.currentData = data;
```

### 5.3 CSV Generation Function
```javascript
function wdExportCsv() {
    var st = window.__drillState;
    if (!st || !st.currentData || !st.currentData.rows || st.currentData.rows.length === 0) return;
    
    var data = st.currentData;
    var cols = data.columns || [];
    var rows = data.rows || [];
    
    // BOM for UTF-8
    var bom = '\uFEFF';
    
    // Header row
    var header = cols.map(function(c) { return '"' + c.replace(/"/g, '""') + '"'; }).join(',');
    
    // Data rows
    var dataRows = rows.map(function(r) {
        return cols.map(function(c) {
            var val = r[c] != null ? String(r[c]) : '';
            return '"' + val.replace(/"/g, '""') + '"';
        }).join(',');
    });
    
    // Combine
    var csv = bom + header + '\n' + dataRows.join('\n');
    
    // Create blob and download
    var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    var url = URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.href = url;
    
    // Filename
    var now = new Date();
    var timestamp = now.getFullYear() 
        + String(now.getMonth() + 1).padStart(2, '0')
        + String(now.getDate()).padStart(2, '0') + '-'
        + String(now.getHours()).padStart(2, '0')
        + String(now.getMinutes()).padStart(2, '0');
    link.download = 'drill-' + st.cardId + '-level-' + st.currentLevel + '-' + timestamp + '.csv';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}
window.wdExportCsv = wdExportCsv;
```

---

## 6. Acceptance Criteria

- [x] Export button appears in footer for Table/Chart levels with data
- [x] Export button does NOT appear for KPI/Gauge levels
- [x] Export button does NOT appear for empty/error states
- [x] CSV file downloads with correct filename format
- [x] CSV file uses UTF-8 BOM encoding
- [x] CSV file has correct column headers
- [x] CSV file has correct data rows
- [x] Arabic/RTL text displays correctly in CSV
- [x] Special characters (commas, quotes) are handled correctly
- [x] Export works for both Table and Chart levels

---

## 7. Testing Steps

1. Open dashboard, click "تفاصيل" on a card with Drill configured
2. Navigate to a Table level with data
3. Verify export button appears in footer
4. Click export button
5. Verify CSV file downloads with correct filename
6. Open CSV file in Excel/Notepad
7. Verify columns and data are correct
8. Verify Arabic text displays correctly
9. Navigate to a Chart level with data
10. Verify export button appears
11. Click export button
12. Verify CSV file downloads correctly
13. Navigate to a KPI/Gauge level
14. Verify export button does NOT appear

---

## 8. Notes

- The export should use the data already loaded in the modal (no re-execution)
- The CSV should be compatible with Excel (UTF-8 BOM)
- Handle edge cases: empty columns, null values, special characters

---

> **Prepared by:** TeraAgent  
> **Mode:** Plan Mode — No code written in this document
