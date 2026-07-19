# TASK-SYNC-SET-002 — Frontend: إعادة تصميم صفحة إعدادات المزامنة

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-SET-002 |
| **المجموعة** | UI-IMPROVEMENT |
| **النوع** | Frontend Razor + CSS + JS |
| **الوكيل المقترح** | ui-designer |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

إعادة تصميم صفحة إعدادات المزامنة (`/admin-secure-panel/SyncSettings`) لتصبح احترافية ومتسقة مع تصميم صفحة Cards الجديدة، مع إضافة إحصائيات حية وإجراءات سريعة.

---

## 2. التصميم المستهدف

### 2.1 شريط الإحصائيات (4 كتل)

```
┌──────────────┬──────────────┬──────────────┬──────────────┐
│  🔄 حالة     │  ⏰ تلقائية  │  📋 جداول    │  ✅ آخر     │
│  المحرك      │              │  مفعّلة     │  نتيجة      │
│  🟢 متوقف    │  🟡 معطّلة  │     2       │  نجاح       │
└──────────────┴──────────────┴──────────────┴──────────────┘
```

- **حالة المحرك**: 🟢 متوقف / 🔵 يعمل / 🔴 خطأ
- **التلقائية**: 🟢 مفعّلة / 🟡 معطّلة
- **الجداول المفعّلة**: عدد الجداول النشطة من أصل الإجمالي (مثلاً "2 من 3")
- **آخر نتيجة**: ✅ نجاح / ❌ فشل / ➖ لا توجد

ألوان الكتل:
- المحرك: أزرق (`#1F4E79`)
- التلقائية: أصفر (`#E0A106`) / أخضر عند التفعيل
- الجداول: بنفسجي (`#7C3AED`)
- النتيجة: أخضر (`#1E9E6A`) / أحمر عند الفشل

### 2.2 بطاقة الإعدادات

```
┌─────────────────────────────────────────────────────────────┐
│  ⚙️ إعدادات المزامنة التلقائية                              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  فاصل المزامنة                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  [30] ← كل 30 دقيقة                                  │  │
│  │  ───○━━━━━●───────────  1 ────────────── 1440        │  │
│  │  Presets: [5د] [15د] [30د] [1س] [6س] [24س]          │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  المزامنة التلقائية                                         │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  [🔘───]  معطّلة                                      │  │
│  │  عند التفعيل، سيتم مزامنة البيانات كل 30 دقيقة        │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  آخر مزامنة                                                 │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  📅 2026/07/19 12:30:00                               │  │
│  │  ⏱️ منذ 45 دقيقة                                      │  │
│  │  ✅ نجاح — 1,250 سجل                                   │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  [💾 حفظ الإعدادات]  [▶ تشغيل مزامنة الآن]                   │
└─────────────────────────────────────────────────────────────┘
```

### 2.3 سجل آخر المزامنات

```
┌─────────────────────────────────────────────────────────────┐
│  📊 آخر المزامنات                                           │
├─────────────────────────────────────────────────────────────┤
│  ┌──────┬────────────┬──────────┬────────┬────────────────┐ │
│  │  #   │  التاريخ   │  الحالة  │ سجلات  │  ملاحظة        │ │
│  ├──────┼────────────┼──────────┼────────┼────────────────┤ │
│  │  1   │ 12:30:00   │ ✅ نجاح  │ 1,250  │                │ │
│  │  2   │ 12:00:00   │ ✅ نجاح  │ 1,200  │                │ │
│  │  3   │ 11:30:00   │ ❌ فشل   │ 0      │ خطأ اتصالOracle│ │
│  │  4   │ 11:00:00   │ ✅ نجاح  │ 1,180  │                │ │
│  │  5   │ 10:30:00   │ ✅ نجاح  │ 1,150  │                │ │
│  └──────┴────────────┴──────────┴────────┴────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

**ملاحظة:** سجل المزامنات هذا هو **عرض وهمي (UI فقط)** — لا يتطلب Backend جديد. يتم توليده من `SyncStatus.LastSyncTime` فقط. إذا توفرت بيانات حقيقية مستقبلاً، سيتم ربطها.

### 2.4 شريط الخطأ (عند فشل الاتصال بـ API)

إذا فشل الاتصال بخدمة API، يظهر شريط تنبيه:
```
┌─────────────────────────────────────────────────────────────┐
│  ⚠️ تعذر الاتصال بخدمة المزامنة. بعض الإحصائيات غير متاحة.  │
└─────────────────────────────────────────────────────────────┘
```

يختفي إذا كانت API متاحة.

---

## 3. تفاصيل التنفيذ

### 3.1 HTML Structure

```html
@page
@model SyncSettingsModel
@{
    ViewData["Title"] = "إعدادات المزامنة";
}

<!-- Breadcrumb -->
<div class="wd-breadcrumb-row">
    <a href="/admin-secure-panel">لوحة الإدارة</a>
    <span class="wd-sep">‹</span>
    <span>إعدادات المزامنة</span>
</div>

<!-- Page Header -->
<div class="wd-page-head">
    <h2 class="wd-page-title">إعدادات المزامنة</h2>
</div>

<!-- Error Banner (إذا LoadError موجود) -->
@if (!string.IsNullOrEmpty(Model.LoadError))
{
    <div class="alert-banner alert-banner--warn">
        <span>⚠️</span>
        <span>@Model.LoadError</span>
    </div>
}

<!-- Stats Cards -->
<div class="cards-stats">
    <div class="cards-stat cards-stat--blue">
        <div class="cards-stat__icon">🔄</div>
        <div class="cards-stat__info">
            <span class="cards-stat__label">حالة المحرك</span>
            <span class="cards-stat__value" id="stat-engine-status">
                @if (Model.SyncStatus?.IsRunning == true) { <span>🟢 يعمل</span> }
                else { <span>🔴 متوقف</span> }
            </span>
        </div>
    </div>
    <div class="cards-stat cards-stat--amber">
        ...
    </div>
    <div class="cards-stat cards-stat--purple">
        ...
    </div>
    <div class="cards-stat cards-stat--green">
        ...
    </div>
</div>

<!-- Settings Card -->
<form method="post" class="admin-card">
    <div class="admin-card__header">
        <div class="admin-card__title-group">
            <span class="admin-card__icon">⚙️</span>
            <h3 class="admin-card__title">إعدادات المزامنة التلقائية</h3>
        </div>
    </div>
    <div class="admin-card__body">
        <!-- Interval -->
        <div class="settings-field">
            <label>فاصل المزامنة</label>
            <div class="settings-interval">
                <input type="range" min="1" max="1440" step="1" id="interval-slider" />
                <input type="number" asp-for="IntervalMinutes" id="IntervalMinutes" min="1" max="1440" />
                <span class="interval-label" id="interval-label"></span>
            </div>
            <div class="settings-presets">
                <button type="button" class="preset-btn" data-minutes="5">5د</button>
                <button type="button" class="preset-btn" data-minutes="15">15د</button>
                <button type="button" class="preset-btn" data-minutes="30">30د</button>
                <button type="button" class="preset-btn" data-minutes="60">1س</button>
                <button type="button" class="preset-btn" data-minutes="360">6س</button>
                <button type="button" class="preset-btn" data-minutes="1440">24س</button>
            </div>
        </div>

        <!-- Auto-sync toggle -->
        <div class="settings-field">
            <label>المزامنة التلقائية</label>
            <div class="settings-toggle">
                <label class="wd-toggle">
                    <input type="checkbox" asp-for="IsAutoSyncEnabled" />
                    <span class="wd-toggle__slider"></span>
                </label>
                <span id="auto-sync-text">@(Model.IsAutoSyncEnabled ? "مفعّلة" : "معطّلة")</span>
            </div>
        </div>

        <!-- Last Sync -->
        <div class="settings-field">
            <label>آخر مزامنة</label>
            <div class="settings-last-sync">
                @if (Model.LastSyncTimestamp.HasValue)
                {
                    <span class="sync-date">@Model.LastSyncTimestamp.Value.ToString("yyyy/MM/dd hh:mm:ss tt")</span>
                    <span class="sync-ago" id="sync-ago"></span>
                    <span class="sync-result sync-result--@(Model.HasSyncError ? "fail" : "success")">
                        @(Model.HasSyncError ? "❌ فشل" : "✅ نجاح")
                    </span>
                    <span class="sync-records">@(Model.SyncStatus?.LastRecordCount.ToString("N0") ?? "0") سجل</span>
                }
                else
                {
                    <span class="sync-none">لم تتم مزامنة بعد</span>
                }
            </div>
        </div>
    </div>
    <div class="admin-card__actions">
        <button type="submit" class="wd-btn wd-btn--primary">💾 حفظ الإعدادات</button>
        <a href="/admin-secure-panel/Sync" class="wd-btn wd-btn--ghost">▶ تشغيل مزامنة الآن</a>
    </div>
</form>

<!-- Last Sync Runs Table -->
<div class="admin-card">
    <div class="admin-card__header">
        <div class="admin-card__title-group">
            <span class="admin-card__icon">📊</span>
            <h3 class="admin-card__title">آخر المزامنات</h3>
        </div>
    </div>
    <div class="admin-card__body">
        @if (Model.LastSyncTimestamp.HasValue)
        {
            <!-- Simple table showing last 5 runs -->
            <table class="sync-history-table">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>التاريخ</th>
                        <th>الحالة</th>
                        <th>السجلات</th>
                    </tr>
                </thead>
                <tbody>
                    <!-- This is a UI mock-up using the only available data point -->
                    <tr>
                        <td>1</td>
                        <td>@Model.LastSyncTimestamp.Value.ToString("hh:mm:ss tt")</td>
                        <td><span class="status-badge status-badge--@(Model.HasSyncError ? "off" : "on")">@(Model.HasSyncError ? "فشل" : "نجاح")</span></td>
                        <td>@(Model.SyncStatus?.LastRecordCount.ToString("N0") ?? "—")</td>
                    </tr>
                </tbody>
            </table>
            <p class="sync-history-note">* سيتم عرض آخر 5 مزامنات عند توفر البيانات.</p>
        }
        else
        {
            <div class="empty-mini">
                <p>لا توجد مزامنات سابقة.</p>
            </div>
        }
    </div>
</div>
```

### 3.2 CSS Requirements

- **لا تكرر تعريفات من blue-theme.css** — استخدم `var(--c-*)` و `var(--sp-*)` و `var(--radius-*)`
- استخدم نفس نمط `.cards-stats` و `.admin-card` من صفحة Cards
- أضف CSS جديد فقط لما هو خاص بهذه الصفحة:
  - `.settings-interval` — تنسيق حقل الفاصل (slider + number + label)
  - `.settings-presets` — أزرار الـ Presets
  - `.settings-toggle` — التوجل مع النص
  - `.settings-last-sync` — تفاصيل آخر مزامنة
  - `.sync-history-table` — جدول سجل المزامنات
  - `.alert-banner` — شريط التحذير
  - `.empty-mini` — حالة عدم وجود بيانات
  - `.sync-result` — Badge نتيجة المزامنة
  - `.sync-ago` — نص "منذ X دقيقة"

### 3.3 JavaScript Requirements

```javascript
(function() {
    // 1. Interval slider <-> number sync
    // 2. Preset buttons set interval value
    // 3. Update human-readable label ("كل X دقيقة / ساعة")
    // 4. Auto-sync toggle updates status text
    // 5. Sync "time ago" countdown (كل 30 ثانية)
    // 6. Live sync status polling (كل 30 ثانية)
})();
```

**مغلف في IIFE** — لا متغيرات عامة.

---

## 4. بيانات الـ Model المتاحة (من TASK-SYNC-SET-001)

| الخاصية | النوع | المصدر |
|---|---|---|
| `Model.IntervalMinutes` | int | DB (للقراءة/الكتابة) |
| `Model.IsAutoSyncEnabled` | bool | DB (للقراءة/الكتابة) |
| `Model.LastSyncTimestamp` | DateTime? | DB |
| `Model.SyncStatus` | SyncInfo? | API (`/api/sync/status`) |
| `Model.SyncConfig` | SyncConfigInfo? | API (`/api/sync/config`) |
| `Model.Mappings` | List\<MappingItem\> | API (`/api/sync/mappings`) |
| `Model.LoadError` | string? | خطأ API |
| `Model.ActiveMappingsCount` | int | محسوب |
| `Model.TotalMappingsCount` | int | محسوب |
| `Model.HasSyncError` | bool | محسوب |
| `Model.LastSyncStatusText` | string? | محسوب |

من `Model.SyncStatus`:
- `SyncStatus.IsRunning` — هل المحرك يعمل الآن
- `SyncStatus.LastSyncTime` — وقت آخر مزامنة (من API)
- `SyncStatus.LastStatus` — "success" / "error" / null
- `SyncStatus.LastRecordCount` — عدد السجلات

---

## 5. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncSettings\Index.cshtml
```

**فقط هذا الملف. لا تلمس أي ملف آخر.**

---

## 6. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | شريط الإحصائيات يظهر 4 كتل (حالة المحرك، التلقائية، الجداول المفعّلة، آخر نتيجة) |
| AC-2 | حقل فاصل المزامنة مع Slider + رقم + Presets |
| AC-3 | Preset buttons تعمل (5د/15د/30د/1س/6س/24س) |
| AC-4 | الـ Slider والـ Number متزامنان (تغيير أحدهما يحدث الآخر) |
| AC-5 | نص مقروء يظهر "كل X دقيقة / ساعة" |
| AC-6 | Toggle المزامنة التلقائية يعمل والنص يتغير (مفعّلة/معطّلة) |
| AC-7 | آخر مزامنة تعرض التاريخ + "منذ X دقيقة" + Badge نجاح/فشل |
| AC-8 | جدول آخر المزامنات يظهر (أو رسالة "لا توجد") |
| AC-9 | شريط الخطأ يظهر إذا `LoadError` موجود |
| AC-10 | JS مغلف في IIFE، لا متغيرات عامة |
| AC-11 | `dotnet build` ناجح بدون أخطاء |
| AC-12 | التصميم متسق مع صفحة Cards الجديدة |
| AC-13 | استخدام `var(--c-*)` و `var(--sp-*)` من blue-theme.css |
| AC-14 | زر "تشغيل مزامنة الآن" ينتقل إلى صفحة Sync |

---

## 7. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ ملف واحد — Frontend فقط |
| لا تغيير Backend | ✅ (كل البيانات محضّرة في TASK-SYNC-SET-001) |
| لا تغيير Auth | ✅ |
| CSS لا يكرر blue-theme | ✅ (CSS جديد فقط) |
| JS لا يتعارض مع الصفحات الأخرى | ✅ (IIFE مغلق) |
| Build | ✅ متوقع 0 errors |

**Gate Status:** ✅ PASS
