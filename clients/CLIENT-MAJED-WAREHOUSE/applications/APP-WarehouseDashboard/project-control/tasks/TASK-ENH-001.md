# TASK-ENH-001 — Sync Dashboard UI (شاشة المزامنة الموحدة + بطاقات ملخص)

| البند | القيمة |
|---|---|
| **المعرف** | TASK-ENH-001 |
| **المجموعة** | ENH (Sync Enhancement P0) |
| **النوع** | Frontend (Razor Page + JS) |
| **الوكيل** | engineering-agent |
| **الأولوية** | High |
| **الحالة** | 🟢 Completed |
| **تاريخ الإنشاء** | 2026-07-15 |

---

## 1. المشكلة

شاشات المزامنة الحالية (SyncSettings + SyncLogs) بدائية ومنفصلة. المستخدم يحتاج شاشة واحدة متكاملة تسمح باختيار التعيينات، رؤية التقدم الحي، وعرض ملخص الحالة.

---

## 2. الهدف

إنشاء صفحة **Sync Dashboard** واحدة في `/admin-secure-panel/Sync` تجمع:
- بطاقات ملخص (حالة المزامنة، إجمالي السجلات، الوقت، السرعة)
- جدول التعيينات مع Checkboxes للاختيار
- زر "مزامنة المحدد" و"مزامنة الكل"
- شريط تقدم حي لكل تعيين أثناء المزامنة
- سجل آخر المزامنات

---

## 3. النطاق

### المطلوب

- [x] **إنشاء صفحة Razor جديدة** في `Pages/admin-secure-panel/Sync/Index.cshtml`
- [x] **إنشاء Page Model** `Index.cshtml.cs`
- [x] **بطاقات ملخص (Summary Cards)** في أعلى الصفحة (4 بطاقات)
- [x] **جدول التعيينات النشطة** يحتوي: Checkbox، المصدر، الهدف، الحالة، إجراءات
- [x] **أزرار إجراءات**: مزامنة المحدد، مزامنة الكل
- [x] **شريط تقدم حي** يظهر أثناء المزامنة (progress bar لكل تعيين)
- [x] **تحديث التنقل**: تعديل `/admin-secure-panel/Index.cshtml` للإشارة إلى `/admin-secure-panel/Sync`
- [x] **استخدام API endpoints** من ENH-002 و ENH-003
- [x] **نمط CSS موحد** مع باقي لوحة الإدارة (نفس ألوان `_CardsLayout`)

### غير المطلوب

- لا SignalR (polling بسيط)
- لا تغيير في API project (الـ API جاهز من ENH-002 + ENH-003)
- لا تغيير في قاعدة البيانات
- لا إضافة Auth جديد

---

## 4. هيكل الصفحة

```
┌──────────────────────────────────────────────────────────┐
│  لوحة الإدارة « المزامنة                                 │
│                                                          │
│  ┌──────┐  ┌───────────┐  ┌──────────┐  ┌──────────┐   │
│  │ 🟢   │  │  45,230   │  │  14:30   │  │ 334/s    │   │
│  │نشطة  │  │ إجمالي    │  │آخر مرة   │  │السرعة    │   │
│  └──────┘  └───────────┘  └──────────┘  └──────────┘   │
│                                                          │
│  ═══════════════════════════════════════════════════      │
│                                                          │
│  [☰ الكل]  [▶ مزامنة المحدد]  [⟳ مزامنة الكل]          │
│                                                          │
│  ┌───┬────────────┬──────────┬──────────┬──────┬──────┐ │
│  │ □ │ المصدر     │ الهدف    │آخر مزامنة│سجلات │ حالة │ │
│  ├───┼────────────┼──────────┼──────────┼──────┼──────┤ │
│  │ □ │ ST_ITEMS   │ StItems  │14:30     │20,000│ ✅   │ │
│  │ □ │ Query03    │ Items2   │14:25     │5,230 │ ✅   │ │
│  └───┴────────────┴──────────┴──────────┴──────┴──────┘ │
│                                                          │
│  ═══════════════════════════════════════════════════      │
│                                                          │
│  📊 شريط التقدم (أثناء المزامنة)                         │
│  ┌─────────────────────────────────────────────────┐    │
│  │ StItems: ████████████████████░░░░ 85%           │    │
│  │ Items2:  ████████████████████████ 100% ✅        │    │
│  │ StSales: ████░░░░░░░░░░░░░░░░░░ 20% ▶ قيد..     │    │
│  │ الإجمالي: ████████████████░░░░░░ 68%             │    │
│  └─────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────┘
```

---

## 5. تفاصيل تقنية

### 5.1 Page Model (`Index.cshtml.cs`)

```csharp
public class SyncDashboardModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SyncDashboardModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public string ApiBaseUrl { get; set; } = "http://localhost:5001";
    public List<TableMappingItem> Mappings { get; set; } = new();
    public SyncStatusInfo? SyncStatus { get; set; }

    public async Task OnGetAsync()
    {
        // Load mappings from API or DB
        // Load sync status from GET /api/sync/status
        // Load sync config from GET /api/sync/config
    }
}
```

### 5.2 CSS

استخدام نفس متغيرات CSS المستخدمة في باقي لوحة الإدارة:
- `var(--c-surface)`, `var(--c-border)`, `var(--c-primary)`, `var(--c-text)`, `var(--c-text-muted)`
- نفس `wd-*` naming convention
- دعم RTL (الاتجاه من اليمين لليسار)

### 5.3 JS Logic

```javascript
// 1. تحميل البيانات عند بدء الصفحة
async function loadDashboard() { ... }

// 2. اختيار الكل / إلغاء الكل
function toggleSelectAll(checked) { ... }

// 3. مزامنة المحدد
async function syncSelected() {
    const selectedIds = getSelectedMappingIds();
    if (selectedIds.length === 0) return;
    
    const resp = await fetch(apiBase + '/api/sync/trigger-selected', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ mappingIds: selectedIds })
    });
    const data = await resp.json();
    startPolling(data.runId);
}

// 4. استعلام التقدم
async function pollProgress(runId) {
    const resp = await fetch(apiBase + '/api/sync/progress?runId=' + runId);
    const progress = await resp.json();
    updateProgressUI(progress);
    if (progress.overallPercent < 100) {
        setTimeout(() => pollProgress(runId), 2000);
    }
}

// 5. مزامنة الكل
async function syncAll() { ... }
```

---

## 6. API Dependencies

| الـ API | الغرض | من أي مهمة |
|---|---|---|
| `GET /api/sync/status` | حالة المحرك (isRunning, lastSyncTime, ...) | موجود مسبقاً |
| `GET /api/sync/config` | إعدادات المزامنة (interval, autoSync) | موجود مسبقاً |
| `GET /api/sync/logs` | سجل آخر المزامنات | موجود مسبقاً |
| `POST /api/sync/trigger-selected` | تشغيل مزامنة محددة (يرجع runId) | ENH-002 + ENH-003 |
| `GET /api/sync/progress?runId=xxx` | استعلام التقدم الحي | ENH-003 |
| `POST /api/sync/trigger` | مزامنة الكل (للزر "مزامنة الكل") | موجود مسبقاً |
| `GET /TableMappings` أو endpoint تعيينات | قائمة التعيينات النشطة | يحتاج إنشاء أو استخدام الموجود |

**ملاحظة حول التعيينات:** حالياً لا يوجد API يعيد التعيينات للـ Web project. الخيارات:
1. إضافة endpoint بسيط في API: `GET /api/sync/mappings` يرجع قائمة التعيينات النشطة
2. استخدام DbContext مباشرة في Page Model

**الحل الموصى به:** إضافة `GET /api/sync/mappings` إلى `SyncController`:

```csharp
[HttpGet("mappings")]
public async Task<IActionResult> Mappings(CancellationToken ct)
{
    var mappings = await _syncEngine.LoadMappingsFromDbAsync(ct);
    return Ok(mappings);
}
```

**ملاحظة:** `LoadMappingsFromDbAsync` حالياً `private`. يجب جعلها `public` أو إنشاء دالة عامة.

---

## 7. Allowed Write Targets

```text
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Sync\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Sync\Index.cshtml.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Controllers\SyncController.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Api\Services\SyncEngineService.cs
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\project-control\tasks\TASK-ENH-001.md
```

---

## 8. معايير القبول

| # | المعيار | الحالة |
|---|---|---|
| AC-1 | صفحة `/admin-secure-panel/Sync` تفتح بدون خطأ (200) | ✅ |
| AC-2 | بطاقات الملخص الأربعة تظهر وتعرض بيانات حقيقية من API | ✅ |
| AC-3 | جدول التعيينات يعرض كل التعيينات النشطة مع Checkboxes | ✅ |
| AC-4 | زر "مزامنة المحدد" يرسل فقط التعيينات المختارة | ✅ |
| AC-5 | شريط التقدم يظهر أثناء المزامنة ويتحدّث كل 2 ثانية | ✅ |
| AC-6 | زر "مزامنة الكل" يزامن جميع التعيينات | ✅ |
| AC-7 | زر "تحديد الكل / إلغاء الكل" يعمل | ✅ |
| AC-8 | `dotnet build -c Release` نجاح (API + Web) — 0 errors, 0 warnings | ✅ |
| AC-9 | لا أسرار أو connection strings حقيقية | ✅ |

---

## 9. ملاحظات التنفيذ

- الـ **apiBase** يجب أن يكون `https://localhost:5001` (أو متغير بيئة)
- استخدام `IHttpClientFactory` في Page Model للاتصال بـ API
- الـ progress bar: استخدام `<progress>` أو `div` مع `width %` مع JS update
- زر "مزامنة المحدد": تعطيله أثناء التشغيل ومنع الضغط المتكرر
- زر "مزامنة الكل": استخدام `POST /api/sync/trigger` الموجود
- يجب فتح `LoadMappingsFromDbAsync` في SyncEngineService لتصبح Public
- إضافة `GET /api/sync/mappings` endpoint لجلب التعيينات
- التعامل مع حالة "لا يوجد تعيينات" (empty state)
- التعامل مع خطأ API (toast error)
