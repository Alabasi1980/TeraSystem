# TASK-KPI-FIX-003 — Restore Incremental Sync System (5 Missing Pieces)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-KPI-FIX-003 |
| **المجموعة** | Sync Engine |
| **النوع** | Full-Stack Fix |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | Accepted |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

نظام المزامنة التزايدية (Incremental Sync) أُنشئ جزئياً ثم تمت إزالته بالخطأ أثناء التعديلات المستمرة. الحالة الحالية:

| المكان | الحالة |
|---|---|
| Migration `AddSyncModeToTableMappings` | ✅ موجود في `Data/Migrations/` — يضيف `SyncMode` + `IncrementalColumn` للقاعدة |
| `TableMappingConfig.cs` (Web Model) | ❌ لا يحتوي الحقول |
| `TableMapping.cs` (API Model) | ❌ لا يحتوي الحقول |
| `WarehouseDashboardDbContext.cs` | ❌ لا يُهيّئ الحقول |
| `Index.cshtml` (Wizard UI) | ❌ 4 خطوات فقط — لا يوجد Step 5 |
| `table-mapping-wizard.js` | ❌ لا يوجد منطق المزامنة التزايدية |
| `SyncEngineService.cs` | ❌ `LoadMappingsFromDbAsync` لا يقرأ `SyncMode`/`IncrementalColumn` |
| `SyncEngineService.BuildOracleQuery` | ❌ لا يضيف WHERE clause للمزامنة التزايدية |
| `TableMappingController.cs` | ❌ لا يُرجع `SyncMode`/`IncrementalColumn` |

---

## 2. الإصلاحات المطلوبة (5 أجزاء)

### الجزء 1: نماذج البيانات (Model)

**`TableMappingConfig.cs`** (Web) — أضف بعد `ErrorMessage`:

```csharp
/// <summary>Sync mode: "Full" (full refresh) or "Incremental" (delta based on date column).</summary>
[MaxLength(10)]
public string SyncMode { get; set; } = "Full";

/// <summary>
/// Column name in the Oracle source used for incremental sync filtering.
/// Only relevant when SyncMode = "Incremental". Must be a DATE/TIMESTAMP column.
/// </summary>
[MaxLength(128)]
public string? IncrementalColumn { get; set; }
```

**`TableMapping.cs`** (API) — أضف بعد `SqlTargetTable`:

```csharp
/// <summary>Sync mode: "Full" or "Incremental".</summary>
public string SyncMode { get; set; } = "Full";

/// <summary>Date column for incremental filtering (null = full refresh).</summary>
public string? IncrementalColumn { get; set; }
```

**`WarehouseDashboardDbContext.cs`** — أضف في `OnModelCreating` داخل `entity.Property(e => e.ErrorMessage)` block:

```csharp
entity.Property(e => e.SyncMode)
    .HasMaxLength(10)
    .HasDefaultValue("Full");

entity.Property(e => e.IncrementalColumn)
    .HasMaxLength(128);
```

### الجزء 2: Wizard UI — الخطوة 5

**`Index.cshtml`** (TableMappings) — أضف بعد Step 4 (SQL Target) قبل `</fieldset>`:

```html
<!-- ═══ Step 5: Sync Settings ═══ -->
<fieldset class="wm-step-panel wm-hidden" data-step="5" aria-labelledby="wm-step5-title" role="group">
    <legend id="wm-step5-title" class="wm-step-panel__title">
        <svg class="wm-icon" aria-hidden="true"><use href="#wm-icon-settings"></use></svg>
        إعدادات المزامنة
    </legend>
    <p class="wm-step-panel__hint">اختر كيفية مزامنة البيانات من Oracle إلى SQL Server.</p>

    <div class="wm-field">
        <label class="wm-field__label">وضع المزامنة</label>
        <div class="wm-radio-group" role="radiogroup" aria-label="وضع المزامنة" id="wm-sync-mode-group">
            <label class="wm-radio-card" data-value="Full" role="radio" aria-checked="true" tabindex="0">
                <span class="wm-radio-card__icon">🔄</span>
                <span class="wm-radio-card__name"> كامل (Full Refresh)</span>
                <span class="wm-radio-card__desc">إعادة تحميل جميع البيانات في كل مزامنة</span>
                <span class="wm-radio-card__check" aria-hidden="true"></span>
            </label>
            <label class="wm-radio-card" data-value="Incremental" role="radio" aria-checked="false" tabindex="-1">
                <span class="wm-radio-card__icon">📈</span>
                <span class="wm-radio-card__name"> تزايدي (Incremental)</span>
                <span class="wm-radio-card__desc">جلب فقط السجلات الجديدة/المحدثة منذ آخر مزامنة</span>
                <span class="wm-radio-card__check" aria-hidden="true"></span>
            </label>
        </div>
    </div>

    <!-- Incremental settings (hidden unless Incremental selected) -->
    <div class="wm-field wm-hidden" id="wm-incremental-settings">
        <label class="wm-field__label" for="wm-incremental-column">حقل التاريخ</label>
        <select id="wm-incremental-column" class="wm-select" name="incrementalColumn" aria-describedby="wm-incremental-column-hint">
            <option value="">اختر عمود التاريخ...</option>
        </select>
        <span id="wm-incremental-column-hint" class="wm-field__hint">يظهر فقط أعمدة DATE/TIMESTAMP من المصدر</span>
        <div class="wm-field__error wm-hidden" id="wm-incremental-error" role="alert"></div>
    </div>

    <input type="hidden" name="syncMode" id="wm-h-syncMode" value="Full" />
    <input type="hidden" name="incrementalColumn" id="wm-h-incrementalColumn" value="" />
</fieldset>
```

**تحديث Step Indicator** — أضف Step 5 في الـ nav:

```html
<li class="wm-step" data-step="5">
    <span class="wm-step__number" aria-hidden="true">5</span>
    <span class="wm-step__label">المزامنة</span>
</li>
```

### الجزء 3: table-mapping-wizard.js — منطق الخطوة 5

أضف بعد `syncSqlTarget()`:

```javascript
/* ─── Step 5: Sync Settings ─── */
TableMappingWizard.prototype.syncSyncSettings = function () {
  var s = this.state;
  if ($('wm-h-syncMode')) $('wm-h-syncMode').value = s.syncMode || 'Full';
  if ($('wm-h-incrementalColumn')) $('wm-h-incrementalColumn').value = s.incrementalColumn || '';
};
```

أضف في `readInitialDom()`:

```javascript
s.syncMode = data.syncMode || 'Full';
s.incrementalColumn = data.incrementalColumn || '';
```

أضف في `goToStep()` — تحديث visibility:

```javascript
// Step 5: Sync Settings — show only after Step 4
if (n === 5) {
  this.syncSyncSettings();
  // Show/hide incremental settings based on syncMode
  if (s.syncMode === 'Incremental') {
    show($('wm-incremental-settings'));
  } else {
    hide($('wm-incremental-settings'));
  }
}
```

أضف event listener لـ sync mode radio cards:

```javascript
// Wire sync mode radio cards
var syncGroup = $('wm-sync-mode-group');
if (syncGroup) {
  var cards = syncGroup.querySelectorAll('.wm-radio-card');
  Array.prototype.forEach.call(cards, function(card) {
    card.addEventListener('click', function() {
      Array.prototype.forEach.call(cards, function(c) {
        c.classList.remove('wm-radio-card--selected');
        c.setAttribute('aria-checked', 'false');
      });
      card.classList.add('wm-radio-card--selected');
      card.setAttribute('aria-checked', 'true');
      s.syncMode = card.getAttribute('data-value');
      if ($('wm-h-syncMode')) $('wm-h-syncMode').value = s.syncMode;
      if (s.syncMode === 'Incremental') {
        show($('wm-incremental-settings'));
        self.loadDateColumns();
      } else {
        hide($('wm-incremental-settings'));
      }
    });
  });
}
```

أضف دالة `loadDateColumns()`:

```javascript
TableMappingWizard.prototype.loadDateColumns = function () {
  var sel = $('wm-incremental-column');
  if (!sel) return;
  var self = this;
  // Use the preview API to get column metadata
  var source = s.oracleSource;
  var sourceType = s.sourceType;
  if (!source) return;
  
  fetch(this.opts.previewApiUrl, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
    body: JSON.stringify({ source: source, sourceType: sourceType, limit: 1 })
  })
  .then(function(r) { return r.json(); })
  .then(function(data) {
    sel.innerHTML = '<option value="">اختر عمود التاريخ...</option>';
    if (data.columns && Array.isArray(data.columns)) {
      data.columns.forEach(function(col) {
        // Show only DATE/TIMESTAMP columns
        var type = (col.dataType || '').toUpperCase();
        if (type.indexOf('DATE') >= 0 || type.indexOf('TIMESTAMP') >= 0) {
          var o = document.createElement('option');
          o.value = col.columnName;
          o.textContent = col.columnName + ' (' + col.dataType + ')';
          sel.appendChild(o);
        }
      });
    }
    // Preselect for edit mode
    if (s.incrementalColumn) {
      sel.value = s.incrementalColumn;
    }
  })
  .catch(function() {
    sel.innerHTML = '<option value="">تعذر تحميل الأعمدة</option>';
  });
};
```

### الجزء 4: SyncEngineService — دعم المزامنة التزايدية

**`LoadMappingsFromDbAsync()`** — عدّل SQL query:

```sql
-- قبل:
SELECT Id, OracleSource, SourceType, SqlTargetTable
FROM TableMappings WHERE IsActive = 1

-- بعد:
SELECT Id, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn
FROM TableMappings WHERE IsActive = 1
```

وأضف القراءة:

```csharp
SyncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
IncrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
    ? null
    : reader.GetString(reader.GetOrdinal("IncrementalColumn"))
```

نفس التعديل لـ `LoadMappingsByIdsAsync()`.

**`BuildOracleQuery()`** — عدّل لدعم المزامنة التزايدية:

```csharp
private static string BuildOracleQuery(TableMapping mapping, DateTime? lastSyncAt)
{
    if (mapping.SourceType.Equals("Query", StringComparison.OrdinalIgnoreCase))
    {
        // For Query type, append WHERE clause if incremental
        var query = mapping.OracleSource;
        if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(mapping.IncrementalColumn)
            && lastSyncAt.HasValue)
        {
            var dateStr = lastSyncAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            query += $" WHERE {mapping.IncrementalColumn} > TIMESTAMP '{dateStr}'";
        }
        return query;
    }

    if (!IsSafeOracleIdentifier(mapping.OracleSource))
    {
        throw new InvalidOperationException(
            $"Oracle source '{mapping.OracleSource}' is not a safe table/view identifier.");
    }

    var baseQuery = $"SELECT * FROM {mapping.OracleSource}";
    if (mapping.SyncMode.Equals("Incremental", StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrEmpty(mapping.IncrementalColumn)
        && lastSyncAt.HasValue)
    {
        var dateStr = lastSyncAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
        baseQuery += $" WHERE {mapping.IncrementalColumn} > TIMESTAMP '{dateStr}'";
    }
    return baseQuery;
}
```

**تحديث الاستدعاء** — في `RunMappingAsync()`:

```csharp
// قبل:
var sql = BuildOracleQuery(mapping);

// بعد:
var sql = BuildOracleQuery(mapping, mapping.LastSyncAt);
```

**تحديث `LastSyncAt`** — بعد كل مزامنة ناجحة:

```csharp
// بعد نجاح LoadAsync:
await UpdateLastSyncAtAsync(mapping.Id, recordCount, ct);
```

أضف method جديد:

```csharp
private async Task UpdateLastSyncAtAsync(int mappingId, int recordCount, CancellationToken ct)
{
    try
    {
        var connStr = ConnectionStringHelper.ResolveSql(_configuration);
        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE TableMappings
            SET LastSyncAt = GETUTCDATE(), SyncRecordCount = @count, UpdatedAt = GETUTCDATE()
            WHERE Id = @id
            """;
        cmd.Parameters.AddWithValue("@id", mappingId);
        cmd.Parameters.AddWithValue("@count", recordCount);
        await cmd.ExecuteNonQueryAsync(ct);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to update LastSyncAt for mapping {Id}", mappingId);
    }
}
```

**تحديث `TableMapping` model** — أضف:

```csharp
public string SyncMode { get; set; } = "Full";
public string? IncrementalColumn { get; set; }
public DateTime? LastSyncAt { get; set; }
```

### الجزء 5: API Controller — إرجاع الحقول الجديدة

**`TableMappingController.cs`** — عدّل SELECT query لإضافة `SyncMode`, `IncrementalColumn`:

```sql
-- قبل:
SELECT Id, Name, OracleSource, SourceType, SqlTargetTable, ...

-- بعد:
SELECT Id, Name, OracleSource, SourceType, SqlTargetTable, SyncMode, IncrementalColumn, ...
```

وأضف في the JSON response:

```csharp
syncMode = reader.GetString(reader.GetOrdinal("SyncMode")),
incrementalColumn = reader.IsDBNull(reader.GetOrdinal("IncrementalColumn"))
    ? null
    : reader.GetString(reader.GetOrdinal("IncrementalColumn"))
```

---

## 3. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Models\TableMappingConfig.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Data\WarehouseDashboardDbContext.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\table-mapping-wizard.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Models\TableMapping.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\TableMappingController.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-KPI-FIX-003.md
```

---

## 4. معايير القبول

| # | المعيار | Status |
|---|---|---|
| AC-1 | `TableMappingConfig.cs` يحتوي `SyncMode` + `IncrementalColumn` | ⬜ |
| AC-2 | `TableMapping.cs` (API) يحتوي `SyncMode` + `IncrementalColumn` + `LastSyncAt` | ⬜ |
| AC-3 | `WarehouseDashboardDbContext` يُهيّئ الحقول الجديدة | ⬜ |
| AC-4 | Wizard UI يحتوي Step 5 (إعدادات المزامنة) مع radio cards | ⬜ |
| AC-5 | `table-mapping-wizard.js` يدعم sync mode selection + date column loading | ⬜ |
| AC-6 | `SyncEngineService.LoadMappingsFromDbAsync` يقرأ `SyncMode` + `IncrementalColumn` | ⬜ |
| AC-7 | `BuildOracleQuery` يضيف WHERE clause للمزامنة التزايدية | ⬜ |
| AC-8 | `LastSyncAt` يُحدّث بعد كل مزامنة ناجحة | ⬜ |
| AC-9 | `TableMappingController` يُرجع الحقول الجديدة | ⬜ |
| AC-10 | `dotnet build -c Release` ينجح بدون أخطاء (API + Web) | ⬜ |
| AC-11 | لا توجد أسرار في الملفات المعدلة | ⬜ |

---

## 5. ملاحظات تقنية

- Migration `AddSyncModeToTableMappings` موجود بالفعل — لا يحتاج جديد
- `SyncMode` القيمة الافتراضية: `"Full"` — متوافق مع البيانات القائمة
- `IncrementalColumn` يكون `null` عند Full mode
- Oracle timestamp format: `TIMESTAMP 'yyyy-MM-dd HH:mm:ss'`
- `LastSyncAt` من `TableMappings` يُستخدم كنقطة بداية للمزامنة التزايدية
