# TASK-COD-028 — Store & Restore Full Wizard Source State for Builder Edit

> **Status:** ✅ Accepted  
> **Agent:** engineering-agent-dotnet  
> **Created:** 2026-07-18  
> **Phase:** 6 (Implementation)  
> **Priority:** High  
> **Problem:** Editing a card via Builder loses Source Type because only `DataSourceType` (View/SQL Query) was stored, not the actual dropdown value (Template/SavedQuery/SqlTable/CustomSQL)

---

## 1. The Core Problem

The Card Builder has **4 source types** in Step 2:
| UI Label | Dropdown Value |
|---|---|
| قالب جاهز | `Template` |
| استعلام محفوظ | `SavedQuery` |
| جدول SQL Server (المزامن) | `SqlTable` |
| استعلام SQL مخصص (متقدم) | `CustomSQL` |

**But the database only stores:** `DataSourceType = "View"` (for SqlTable) or `"SQL Query"` (for everything else).

This means when editing, **3 out of 4 source types** cannot be restored correctly:
- Template → shows as CustomSQL ❌
- SavedQuery → shows as CustomSQL ❌
- SqlTable → shows as SqlTable ✅ (because we parse the View→SqlTable mapping)
- CustomSQL → shows as CustomSQL ✅

---

## 2. What We Need to Store

Add **2 new fields** to `DashboardCard`:

| Field | Type | Default | Purpose |
|---|---|---|---|
| `OriginalSourceType` | `nvarchar(50)` | `"SqlTable"` | Stores the actual Step 2 dropdown value: Template/SavedQuery/SqlTable/CustomSQL |
| `OriginalSourceId` | `nvarchar(200)` | `""` | Source-specific identifier: table name for SqlTable, template ID for Template, query ID for SavedQuery |

### Why these are sufficient:

| SourceType | OriginalSourceType | OriginalSourceId | How to reconstruct |
|---|---|---|---|
| Template | `"Template"` | (template ID) | Set Step 2 to Template; templates are rendered from JS data |
| SavedQuery | `"SavedQuery"` | (query ID) | Set Step 2 to SavedQuery; show the note |
| SqlTable | `"SqlTable"` | `"stg_invoice"` | Set source dropdown to SqlTable; select table from dropdown; JS builds query |
| CustomSQL | `"CustomSQL"` | (empty) | Set source dropdown to CustomSQL; fill textarea with SqlQuery |

---

## 3. Files to Modify

### 3.1 Model — Add Properties

**File:** `src/WarehouseDashboard.Web/Models/DashboardCard.cs`

Add after `AggregationType`:
```csharp
/// <summary>
/// Original SourceType from Step 2 of the Card Builder: "Template", "SavedQuery", "SqlTable", "CustomSQL".
/// Default: "SqlTable". This is the value of the source dropdown at card creation.
/// </summary>
public string OriginalSourceType { get; set; } = "SqlTable";

/// <summary>
/// Source-specific identifier: table name for SqlTable, template ID for Template, query ID for SavedQuery.
/// Empty for CustomSQL. Used to reconstruct the Step 2 selection when editing.
/// </summary>
public string OriginalSourceId { get; set; } = "";
```

### 3.2 Input Model — Add Properties

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/CardEditorInput.cs`

Add after `AggregationType`:
```csharp
public string OriginalSourceType { get; set; } = "SqlTable";
public string OriginalSourceId { get; set; } = "";
```

Add to validation (optional):
```csharp
public static readonly string[] AllowedOriginalSourceTypes = { "Template", "SavedQuery", "SqlTable", "CustomSQL" };
```

### 3.3 Builder PageModel — Add Properties + Load/Save

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml.cs`

**Add BindProperties:**
```csharp
[BindProperty]
[JsonPropertyName("originalSourceType")]
public string OriginalSourceType { get; set; } = "SqlTable";

[BindProperty]
[JsonPropertyName("originalSourceId")]
public string OriginalSourceId { get; set; } = "";
```

**In `LoadEditDataAsync()`** (when loading card for editing):
```csharp
// After all other properties are loaded:
OriginalSourceType = card.OriginalSourceType;
OriginalSourceId = card.OriginalSourceId;

// Then use OriginalSourceType to set SourceType instead of the current logic:
if (!string.IsNullOrEmpty(card.OriginalSourceType))
{
    SourceType = card.OriginalSourceType;
    
    // For SqlTable: set SourceId to OriginalSourceId (table name)
    if (card.OriginalSourceType == "SqlTable")
    {
        SourceId = card.OriginalSourceId;
        CustomSql = string.Empty;
    }
    // For CustomSQL: fill CustomSql with the SQL query
    else if (card.OriginalSourceType == "CustomSQL")
    {
        SourceId = string.Empty;
        CustomSql = card.SqlQuery;
    }
    // For Template/SavedQuery: they still need the SQL query for preview
    else
    {
        SourceId = card.OriginalSourceId;
        CustomSql = card.SqlQuery;
    }
}
```

**In `BuildDashboardCard()`** (when building DTO from form data):
```csharp
OriginalSourceType = OriginalSourceType ?? SourceType,  // fallback to current sourceType
OriginalSourceId = OriginalSourceId ?? SourceId ?? "",
```

**In entity creation (OnPostAsync, create new):**
```csharp
OriginalSourceType = dto.OriginalSourceType,
OriginalSourceId = dto.OriginalSourceId ?? "",
```

**In `MapDtoToEntity()`** (for edit update):
```csharp
entity.OriginalSourceType = dto.OriginalSourceType ?? "SqlTable";
entity.OriginalSourceId = dto.OriginalSourceId ?? "";
```

**In `DashboardCardDto`:**
```csharp
public string OriginalSourceType { get; set; } = "SqlTable";
public string OriginalSourceId { get; set; } = "";
```

**In `PreviewRequest`:**
```csharp
public string OriginalSourceType { get; set; } = "SqlTable";
public string OriginalSourceId { get; set; } = "";
```

### 3.4 Builder UI — Add Hidden Fields

**File:** `src/WarehouseDashboard.Web/Pages/admin-secure-panel/Cards/Builder.cshtml`

Add hidden fields near the other hidden fields:
```html
<input type="hidden" name="originalSourceType" id="wb-h-originalSourceType" value="@Model.OriginalSourceType">
<input type="hidden" name="originalSourceId" id="wb-h-originalSourceId" value="@Model.OriginalSourceId">
```

Add to initialData in inline script:
```javascript
originalSourceType: @Html.Raw(Json.Serialize(Model.OriginalSourceType)),
originalSourceId: @Html.Raw(Json.Serialize(Model.OriginalSourceId)),
```

### 3.5 Card Builder JS — Pass Original Data Through Init

**File:** `src/WarehouseDashboard.Web/wwwroot/js/card-builder.js`

**In `readInitialDom()`**:
Add after line 162 (aggregationType):
```javascript
s.originalSourceType = id.originalSourceType || ($('wb-h-originalSourceType').value) || 'SqlTable';
s.originalSourceId = id.originalSourceId || ($('wb-h-originalSourceId').value) || '';
```

**In `syncHiddenInputs()`**:
Add near the other sync lines:
```javascript
if ($('wb-h-originalSourceType')) $('wb-h-originalSourceType').value = s.sourceType;
if ($('wb-h-originalSourceId')) $('wb-h-originalSourceId').value = s.sourceId;
```

Wait - the `originalSourceType` should be saved ONCE at card creation and never changed. The hidden field value should only be set from the server. But actually, looking at this more carefully:

For **CREATE**: `originalSourceType` = the value the user selected in Step 2 at creation time (first save)
For **EDIT**: `originalSourceType` = the value that was saved when the card was created (doesn't change)

So the original source type should be set by the server on load, and on save it should be sent back. The user CAN change the source type when editing, and the new value should be saved as the new `originalSourceType`.

Actually, this simplifies things. The `originalSourceType` is just the current `sourceType` from the wizard. We're just persisting it explicitly. So the sync logic should be:

```javascript
// In syncHiddenInputs:
if ($('wb-h-originalSourceType')) $('wb-h-originalSourceType').value = s.sourceType;
if ($('wb-h-originalSourceId')) $('wb-h-originalSourceId').value = s.sourceId;
```

This means every time the user changes the source type, `originalSourceType` is updated. On save, the latest value is stored. On edit, the stored value is used to reconstruct the wizard.

### 3.6 KPI Column Value Pre-Population (existing fix verification)

The fix from TASK-COD-027 should already handle KPI column values via `addSavedKpiOption()`. Verify this is still working.

---

## 4. Migration Script

```sql
ALTER TABLE DashboardCards 
ADD OriginalSourceType NVARCHAR(50) NOT NULL DEFAULT 'SqlTable';

ALTER TABLE DashboardCards
ADD OriginalSourceId NVARCHAR(200) NOT NULL DEFAULT '';

-- Backfill: set OriginalSourceType based on DataSourceType
UPDATE DashboardCards 
SET OriginalSourceType = CASE 
    WHEN DataSourceType = 'View' THEN 'SqlTable'
    ELSE 'CustomSQL' 
END;
```

---

## 5. Complete Flow

### Create:
```
User selects source type → stored as originalSourceType in hidden field
Submit → server saves: OriginalSourceType = dto.OriginalSourceType (= sourceType)
```

### Edit:
```
Open Builder?edit=ID:
  LoadEditDataAsync():
    SourceType = card.OriginalSourceType  ← restored exactly
    SourceId = card.OriginalSourceId      ← restored exactly
    CustomSql = card.SqlQuery (for CustomSQL only)
  
  readInitialDom():
    state.originalSourceType = from initialData
    state.originalSourceId = from initialData
  
  applyInitialUi():
    showSourcePanel(state.sourceType)  ← shows correct panel
    // For SqlTable: populateSqlTableSelect() selects the right table
    // For CustomSQL: textarea shows the SQL
    // For Template: templates render, markTemplateSelected()
  
  Save: 
    syncHiddenInputs → originalSourceType = sourceType (current wizard value)
```

### Round-trip:
```
Template → saved as Template → loaded as Template → shows Template ✅
SqlTable → saved as SqlTable → loaded as SqlTable → shows SqlTable with table selected ✅
CustomSQL → saved as CustomSQL → loaded as CustomSQL → shows CustomSQL with SQL ✅
SavedQuery → saved as SavedQuery → loaded as SavedQuery → shows SavedQuery ✅
```

---

## 6. Acceptance Criteria

| # | Criterion |
|---|---|
| AC-1 | New card created with Template source → opens in Builder showing Template |
| AC-2 | New card created with SavedQuery source → opens in Builder showing SavedQuery |
| AC-3 | New card created with SqlTable source → opens in Builder showing SqlTable with table selected |
| AC-4 | New card created with CustomSQL source → opens in Builder showing CustomSQL with SQL filled |
| AC-5 | Existing cards (before migration) fall back correctly to View→SqlTable / SQL Query→CustomSQL |
| AC-6 | Changing source type in edit mode is saved correctly |
| AC-7 | Build succeeds with 0 errors |

---

## 7. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\DashboardCard.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\CardEditorInput.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Builder.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\card-builder.js
```

---

## 8. IMPORTANT — Before Editing Any File

1. Read the current file from disk first — do NOT rely on memory or this task document
2. Preserve unrelated changes from other sessions
3. After all edits, run `dotnet build` and verify PASS
4. The database migration script is in §4 — document whether you ran it or not

---

> **Assigned by:** TeraAgent — 2026-07-18
