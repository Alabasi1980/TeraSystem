# TASK-DRILL-RENDER-002

> **Status:** In Progress  
> **Assigned To:** Engineering Agent  
> **Created:** 2026-07-20  
> **Priority:** High  
> **Phase:** E (Level Renderers & Export)

---

## 1. Objective

Implement the Chart renderer with selection list inside the drill-down modal. When a drill level displays a Chart (Bar/Line/Pie), the user should be able to select an item from a companion list/table below the chart to navigate to the next level.

---

## 2. Current State

### What Exists
- `wdRenderChart()` function renders Bar/Line/Pie charts using ApexCharts
- `wdRenderLevel()` handles Table, KPI, Gauge renderers
- For Chart types, the current code renders the chart but does NOT add a selection list
- The selection list is only added for Table types (via row click handlers)

### What's Missing
When `data.chartType` is `Bar`, `Line`, or `Pie`:
1. No selection list appears below the chart
2. User cannot select an item to navigate to the next level
3. The `ParameterColumn` and `LabelColumn` are not used for chart selection

---

## 3. Requirements

### 3.1 Selection List for Charts
When a drill level displays a Chart (Bar/Line/Pie) AND:
- `st.hasNextLevel` is true
- `st.parameterColumn` exists
- `data.rows` has data

Then render a selection list/table below the chart with:
- Header: "اختر عنصراً للانتقال للمستوى التالي:"
- List items showing the label column value
- Click handler that calls `wdSelectRow(paramVal, displayVal)`

### 3.2 Data Mapping
The chart data comes from `data.rows` and `data.columns`:
- First column = label (x-axis categories)
- Second column = value (y-axis values)
- `st.parameterColumn` = which column to use as parameter for next level
- `st.labelColumn` = which column to display in the selection list

### 3.3 Selection List UI
- Simple table or list below the chart
- Each row is clickable
- Hover effect on rows
- Click triggers `wdSelectRow(paramVal, displayVal)`

---

## 4. Files to Modify

| File | Changes |
|---|---|
| `src/WarehouseDashboard.Web/Pages/Index.cshtml` | Update `wdRenderLevel()` function to add selection list for Chart types |

---

## 5. Code Location

The code to modify is in `Index.cshtml`, inside the `wdRenderLevel()` function (around lines 1946-1994).

Current code for Chart types:
```javascript
} else {
    div.style.height = '400px';
    bodyEl.innerHTML = '';
    bodyEl.appendChild(div);
    if (data.chartType === 'Gauge') {
        wdRenderGauge(div, data);
    } else {
        wdRenderChart(div, data);
    }
    // Add selection table below chart if hasNextLevel and parameterColumn exists
    if (st.hasNextLevel && st.parameterColumn && data.rows && data.rows.length > 0) {
        // ... selection table code ...
    }
}
```

The selection table code already exists but needs to be verified and potentially enhanced.

---

## 6. Acceptance Criteria

- [ ] Chart renders correctly (Bar/Line/Pie)
- [ ] Selection list appears below chart when conditions are met
- [ ] Selection list shows correct label values
- [ ] Click on selection list item calls `wdSelectRow()`
- [ ] Parameter value is correctly extracted from `ParameterColumn`
- [ ] Label value is correctly extracted from `LabelColumn`
- [ ] Selection list has proper styling (hover, cursor, spacing)
- [ ] Works for all chart types (Bar, Line, Pie)

---

## 7. Testing Steps

1. Open dashboard, click "تفاصيل" on a card with Drill configured
2. Navigate to a level that displays a Chart (Bar/Line/Pie)
3. Verify chart renders correctly
4. Verify selection list appears below chart
5. Verify selection list shows correct items
6. Click on an item in the selection list
7. Verify modal navigates to next level with correct parameter

---

## 8. Notes

- The selection list code already exists in the current `wdRenderLevel()` function (lines 1956-1993)
- Verify it works correctly for Chart types
- If it doesn't work, debug and fix the issue
- The code should be similar to the Table selection list but adapted for Chart data

---

> **Prepared by:** TeraAgent  
> **Mode:** Plan Mode — No code written in this document
