# TASK-ENH-005B — Wizard Sync Mode Step + Dashboard Update

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ENH-005B |
| **المجموعة** | ENH (Sync Enhancement P1) |
| **النوع** | Frontend (Razor + JS) |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | ✅ Complete |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

حالياً ويزرد التعيينات لا يتيح اختيار وضع المزامنة (Full/Incremental) ولا حقل التاريخ للمزامنة التزايدية. كذلك Sync Dashboard لا يعرض وضع المزامنة لكل تعيين.

---

## 2. الهدف

تحديث ويزرد TableMappings ليسمح باختيار وضع المزامنة + حقل التاريخ، وتحديث Sync Dashboard لعرض هذه المعلومات.

---

## 3. النطاق

### المطلوب

- [x] **إضافة قسم إعدادات المزامنة** في Step 4 من الويزرد (بعد اسم الجدول)
- [x] **تحديث الـ JS** لإدارة وضع المزامنة
- [x] **تحديث hidden form fields** لإرسال SyncMode + IncrementalColumn
- [x] **تحديث `OnPostAddAsync` و `OnPostEditAsync` و `OnGetMappingAsync`** في Page Model
- [x] **تحديث Sync Dashboard** لعرض وضع المزامنة

### غير المطلوب

- لا تغيير في API project
- لا تغيير في قاعدة البيانات (تم في ENH-005A)
- لا تغيير في Sync Engine (تم في ENH-005A)

---

## 4. تفاصيل التغييرات

### 4.1 تحديث HTML (Step 4 في الويزرد)

بعد قسم "إنشاء الجدول تلقائياً" و "تطبيق المخطط"، أضف:

```html
<!-- Sync Mode Section -->
<div class="wm-syncmode-section" style="margin-top: 16px; padding-top: 16px; border-top: 1px solid var(--c-border);">
    <h4 style="font-size: 14px; font-weight: 600; margin: 0 0 12px; color: var(--c-text);">إعدادات المزامنة</h4>

    <div class="wm-field">
        <label>وضع المزامنة</label>
        <div class="wm-radio-group" style="display:flex; gap:16px; margin-top:4px;">
            <label class="wm-radio-row" style="display:flex; align-items:center; gap:6px; cursor:pointer;">
                <input type="radio" name="syncMode" value="Full" checked />
                <span>🔄 كامل (Full Refresh)</span>
            </label>
            <label class="wm-radio-row" style="display:flex; align-items:center; gap:6px; cursor:pointer;">
                <input type="radio" name="syncMode" value="Incremental" />
                <span>⏩ تزايدي (Incremental)</span>
            </label>
        </div>
        <span class="wm-hint">التزايدي يزامن فقط السجلات الجديدة/المعدّلة من آخر مزامنة.</span>
    </div>

    <div id="wm-inc-field" class="wm-field" style="display:none;">
        <label for="wm-inc-column">حقل التاريخ</label>
        <div style="display:flex; gap:8px; align-items:center;">
            <select class="wm-select" id="wm-inc-column" style="flex:1;">
                <option value="">-- اختر الحقل --</option>
            </select>
            <button type="button" class="wd-btn wd-btn--ghost wd-btn--sm" id="wm-btn-load-columns" title="تحميل الأعمدة">
                ⟳
            </button>
        </div>
        <span class="wm-hint">اختر حقل التاريخ (DATE/TIMESTAMP) الذي سيُستخدم للتصفية.</span>
        <div class="wm-inc-preview" id="wm-inc-preview" style="margin-top:8px; font-size:12px; color:var(--c-text-muted); background:var(--c-surface-muted); padding:8px 12px; border-radius:var(--radius-md); direction:ltr; text-align:left; display:none;">
            <!-- Preview SQL shown here -->
        </div>
    </div>
</div>
```

### 4.2 تحديث hidden form fields

أضف hidden inputs داخل `#wm-form`:

```html
<input type="hidden" asp-for="SyncMode" id="wm-h-syncMode" value="Full" />
<input type="hidden" asp-for="IncrementalColumn" id="wm-h-incrementalColumn" value="" />
```

أضف BindProperty في Page Model:
```csharp
[BindProperty]
public string SyncMode { get; set; } = "Full";

[BindProperty]
public string? IncrementalColumn { get; set; }
```

### 4.3 تحديث JS (table-mapping-wizard.js)

**في `init()`**:
- أضف `wireSyncMode()` → يربط أحداث الـ radio buttons
- أضف `wireColumnLoader()` → يربط زر تحميل الأعمدة

**في `wireSyncMode()`**:
- عند اختيار "Incremental": أظهر حقل التاريخ، حمّل الأعمدة من Oracle
- عند اختيار "Full": أخفِ حقل التاريخ
- حدّث `state.syncMode` عند التغيير

**في `wireColumnLoader()`**:
- يستدعي API أو service لجلب أعمدة Oracle للمصدر المحدد
- يفلتر الأعمدة من نوع DATE/TIMESTAMP فقط (أو يعرضها كلها)
- يملأ `select#wm-inc-column`

**API لجلب الأعمدة**:
استخدم `OracleSchemaService.GetOracleTableColumnsAsync()` الموجود في Web project.

في Page Model، أضف handler:
```csharp
public async Task<IActionResult> OnGetOracleColumnsAsync(string source, string sourceType)
{
    var columns = await _oracleSchema.GetOracleTableColumnsAsync(source, sourceType);
    return new JsonResult(columns.Select(c => new { name = c.ColumnName, type = c.DataType, maxLength = c.MaxLength }));
}
```

هذا handler يُستخدم من JS:
```javascript
fetch('/admin-secure-panel/TableMappings?handler=OracleColumns&source=' + encodeURIComponent(source) + '&sourceType=' + sourceType)
```

**في `bootstrapEditMode(data)`**:
- أضف تعبئة `syncMode` و `incrementalColumn` من البيانات
- اختيار radio button المناسب
- إذا Incremental: أظهر حقل التاريخ + اختر القيمة المحفوظة

**في `submitForm()`** (أو `saveMapping()`):
- اقرأ قيمة `syncMode` من radio المختار
- اقرأ قيمة `incrementalColumn` من الـ select
- ضع القيم في hidden inputs

### 4.4 تحديث OnPostAddAsync و OnPostEditAsync

في `Index.cshtml.cs`:

```csharp
[BindProperty]
public string SyncMode { get; set; } = "Full";

[BindProperty]
public string? IncrementalColumn { get; set; }
```

في `OnPostAddAsync`:
```csharp
_db.TableMappings.Add(new TableMappingConfig
{
    OracleSource = OracleSource,
    SourceType = SourceType,
    SqlTargetTable = SqlTargetTable,
    SyncMode = SyncMode,
    IncrementalColumn = IncrementalColumn,
    IsActive = true,
    CreatedAt = DateTime.UtcNow,
    UpdatedAt = DateTime.UtcNow
});
```

في `OnPostEditAsync`:
```csharp
mapping.OracleSource = OracleSource;
mapping.SourceType = SourceType;
mapping.SqlTargetTable = SqlTargetTable;
mapping.SyncMode = SyncMode;
mapping.IncrementalColumn = IncrementalColumn;
mapping.UpdatedAt = DateTime.UtcNow;
```

في `OnGetMappingAsync` (للتعديل):
```csharp
Select(m => new
{
    editId = m.Id,
    oracleSource = m.OracleSource,
    sourceType = m.SourceType,
    sqlTargetTable = m.SqlTargetTable,
    syncMode = m.SyncMode,
    incrementalColumn = m.IncrementalColumn
})
```

### 4.5 تحديث Sync Dashboard

في `Pages/admin-secure-panel/Sync/Index.cshtml`:

في جدول التعيينات، أضف عمود "الوضع":
```
| □ | المصدر | الهدف | الوضع | آخر مزامنة | سجلات | الحالة |
```

الوضع يعرض:
- `🔄 كامل` لـ `Full`
- `⏩ تزايدي` لـ `Incremental` (مع اسم الحقل كـ tooltip: `CREATED_DATE`)

---

## 5. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\wwwroot\js\table-mapping-wizard.js
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\TableMappings\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Sync\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Sync\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-ENH-005B.md
```

---

## 6. معايير القبول

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | Step 4 في الويزرد فيه قسم "إعدادات المزامنة" مع اختيار Full/Incremental | ✅ |
| AC-2 | عند اختيار Incremental، يظهر dropdown بأعمدة Oracle (خاصة التاريخ) | ✅ |
| AC-3 | عند حفظ التعيين، SyncMode و IncrementalColumn يُحفظان في DB | ✅ |
| AC-4 | عند تعديل تعيين، القيم المحفوظة تظهر في الويزرد | ✅ |
| AC-5 | Sync Dashboard يعرض "🔄 كامل" أو "⏩ تزايدي" لكل تعيين | ✅ |
| AC-6 | `dotnet build -c Release` = 0 errors / 0 warnings | ✅ |
| AC-7 | لا أسرار أو connection strings حقيقية | ✅ |
