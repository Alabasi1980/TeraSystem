# TASK-DRILL-CARDS-001

> **Status:** In Progress  
> **Assigned To:** Engineering Agent  
> **Created:** 2026-07-20  
> **Priority:** Medium  
> **Phase:** F (Admin Cards Integration)

---

## 1. Objective

Display Drill Down metadata in the Cards admin page. Show the number of Drill levels configured for each card and provide a quick link to configure Drill Down.

---

## 2. Requirements

### 2.1 Display Drill Metadata
In the Cards admin page (`admin-secure-panel/Cards/Index.cshtml`), for each card:
- Show the number of Drill Down levels configured
- Example: "2 مستويات Drill" or "لا توجد مستويات Drill"

### 2.2 Quick Link to Drill Configuration
- Add a button/link "تكوين Drill" for each card
- Clicking it should navigate to the DrillDown admin page with the card ID
- URL format: `/admin-secure-panel/DrillDown?cardId={cardId}`

---

## 3. Files to Modify

| File | Changes |
|---|---|
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Index.cshtml.cs` | Add Drill level count to card data |
| `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Index.cshtml` | Display Drill metadata and link |

---

## 4. Implementation Plan

### 4.1 Backend Changes (Index.cshtml.cs)
1. In the page model, when loading cards, also load the Drill Down levels count for each card
2. Add a dictionary or property to store `CardId -> DrillLevelCount`
3. Pass this data to the view

### 4.2 Frontend Changes (Index.cshtml)
1. For each card row, display the Drill level count
2. Add a "تكوين Drill" button/link that navigates to the DrillDown page

---

## 5. Acceptance Criteria

- [ ] Cards admin page shows Drill level count for each card
- [ ] Cards without Drill show "لا توجد مستويات Drill"
- [ ] Cards with Drill show "{count} مستويات Drill"
- [ ] "تكوين Drill" button/link appears for each card
- [ ] Clicking "تكوين Drill" navigates to DrillDown page with correct card ID
- [ ] Build succeeds with 0 errors

---

## 6. Testing Steps

1. Open admin panel
2. Navigate to Cards page
3. Verify Drill level count is displayed for each card
4. Verify "تكوين Drill" button appears
5. Click "تكوين Drill" button
6. Verify navigation to DrillDown page with correct card ID

---

> **Prepared by:** TeraAgent  
> **Mode:** Plan Mode — No code written in this document
