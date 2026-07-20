# TASK-SYNC-LOG-02 — Rich Advanced SyncLogs Page

| البند | القيمة |
|---|---|
| **المعرف** | TASK-SYNC-LOG-02 |
| **النوع** | Frontend — UI Redesign |
| **الأولوية** | High |
| **الحالة** | Approved for Delegation |
| **تاريخ الإنشاء** | 2026-07-20 |

---

## 1. الهدف

إعادة تصميم صفحة سجلات المزامنة (`/admin-secure-panel/SyncLogs`) لتصبح أداة متقدمة لتحليل مشاكل المزامنة.

التصميم الحالي ضعيف — مجرد جدول HTML بسيط يقرأ من API ويعرض 6 أعمدة فقط بدون أي تفاصيل أو فلترة.

الآن الـ API يعيد سجلات دائمة من قاعدة البيانات (جدول SyncRuns) مع تفاصيل لكل جدول داخل كل دورة (جدول SyncRunDetails).

---

## 2. API Endpoints

| الـ Endpoint | الوصف |
|---|---|
| `GET /api/sync/logs` | يرجع قائمة SyncRuns (id, startTime, endTime, status, triggerType, recordCount, duration) |
| `GET /api/sync/logs/{runId}` | يرجع SyncRun واحد + Details (per-table: targetTable, syncMode, status, rowsExtracted, rowsLoaded, attempts, durationSeconds, errorMessage) |

---

## 3. التصميم المطلوب

### 3.1 البنية العامة

```
┌──────────────────────────────────────────────────────────────┐
│ سجلات المزامنة                                                │
│ صفحة متقدمة لتتبع وتحليل المزامنة                              │
│                                                              │
│ [فلتر التاريخ من — الى] [حالة ▼] [نوع ▼] [بحث...] [تحديث]  │
│ ──────────────────────────────────────────────────────────── │
│                                                              │
│ [SyncRun Card 1]  🟢 نجح    تلقائي    1,234 سجل    12.5 ث   │
│   ┌ تفاصيل الجدول  ──────────────────────────────────────┐  │
│   │ جدول          الحالة    الصفوف   المدة    المحاولات  │  │
│   │ stg_Stock     ✅ نجح    1,000    8.2 ث    1         │  │
│   │ stg_Orders    ✅ نجح    234      4.3 ث    1         │  │
│   │ stg_Warehouse ❌ فشل    0        0.5 ث    3         │  │
│   │   ⚠ خطأ: Timeout expired...                         │  │
│   └────────────────────────────────────────────────────┘  │
│                                                              │
│ [SyncRun Card 2]  🔴 فشل    يدوي     0 سجل      0.5 ث      │
│   ...                                                       │
└──────────────────────────────────────────────────────────────┘
```

### 3.2 المكونات

**A. Filter Bar:**
- Date range (from — to date inputs)
- Status dropdown (الكل / نجح / فشل / جزئي / قيد التنفيذ)
- Trigger type dropdown (الكل / تلقائي / يدوي / يدوي (محدد))
- Search box (يبحث في اسم الجدول)
- Update button
- Auto-refresh toggle with indicator

**B. Sync Run List — كل دورة مزامنة كبطاقة قابلة للتوسيع:**

Header:
- Status badge (🟢 نجح / 🔴 فشل / 🟡 جزئي / 🔵 قيد التنفيذ)
- Trigger type badge (تلقائي / يدوي)
- Total records
- Duration
- Start time (مُنسّق)

Expanded Detail:
- جدول لكل mapping في هذه الدورة
- الأعمدة: TargetTable, SyncMode, Status (✅ نجح / ❌ فشل), Rows loaded, Duration, Attempts
- إذا فشل mapping: ظهور رسالة الخطأ بخط واضح (أحمر/برتقالي)
- إذا كانت الحالة "قيد التنفيذ": إظهار animated spinner

**C. Empty State:**
- عند عدم وجود سجلات: رسالة واضحة + إرشاد لتشغيل أول مزامنة

**D. Error State:**
- عند فشل الاتصال بالـ API: رسالة خطأ مع زر "إعادة المحاولة"

---

## 4. Allowed Sources

- `Index.cshtml` الحالي (سجلات المزامنة)
- `Index.cshtml.cs` الحالي

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncLogs\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\SyncLogs\Index.cshtml.cs`

---

## 6. Acceptance Criteria

| # | المعيار |
|---|---|
| AC-1 | كل دورة مزامنة تُعرض كبطاقة منفصلة مع status badge |
| AC-2 | كل بطاقة قابلة للتوسيع لإظهار تفاصيل الجداول |
| AC-3 | تفاصيل الجدول تظهر: TargetTable, Status, RowsLoaded, Duration, Attempts |
| AC-4 | رسائل الخطأ تظهر بشكل واضح عند فشل mapping |
| AC-5 | فلتر حسب التاريخ (from — to) |
| AC-6 | فلتر حسب الحالة (Success / Failed / Partial / Running) |
| AC-7 | زر تحديث يدوي |
| AC-8 | Auto-refresh كل 30 ثانية مع توقف عند تصغير التبويب |
| AC-9 | Last updated timestamp |
| AC-10 | الـ API name يصير `http://localhost:5001` (مثبت) |
| AC-11 | لا يظهر النص "البيانات مخزّنة مؤقتاً في الذاكرة" |
| AC-12 | `dotnet build` ينجح |

---

## 7. ملاحظات

- اقرأ الملف الحالي من القرص قبل التعديل
- استخدم CSS classes الموجودة في `blue-theme.css` (wd-card, wd-btn, wd-badge, wd-input, wd-select)
- الـ API Base URL: `http://localhost:5001` (ثابت حالياً)
- الصفحة الحالية تستخدم `<script>` داخل `@section Scripts` — حافظ على هذا النمط
- ApexCharts غير مطلوب في هذه الصفحة
- لا تستخدم مكتبات خارجية جديدة
- الكل RTL + خطوط عربية (Cairo)

---

## 8. Handback

| البند | القيمة |
|---|---|
| **الحالة** | Submitted |
| **التاريخ** | 2026-07-20 |
| **المعرّف** | TASK-SYNC-LOG-02 |
| **التنفيذ** | ui-designer |

### التغييرات

- **Index.cshtml** — أُعيدت كتابتها بالكامل (~1004 سطر بدل 226):
  - بطاقات قابلة للتوسيع لكل دورة مزامنة مع status badge
  - شريط فلترة متكامل (تاريخ، حالة، نوع، بحث)
  - Auto-refresh مع عداد تنازلي
  - Skeleton loading + empty states + error state
  - Connection status indicator
  - جميع الألوان من ColorPalette
- **Index.cshtml.cs** — لم يتغير

### Acceptance Criteria — تحقق

| # | المعيار | الحالة |
|---|---------|--------|
| AC-1 | كل دورة كبطاقة مع status badge | ✅ |
| AC-2 | بطاقات قابلة للتوسيع | ✅ |
| AC-3 | تفاصيل: TargetTable, Status, RowsLoaded, Duration, Attempts | ✅ |
| AC-4 | رسائل الخطأ بارزة | ✅ |
| AC-5 | فلتر حسب التاريخ | ✅ |
| AC-6 | فلتر حسب الحالة | ✅ |
| AC-7 | زر تحديث يدوي | ✅ |
| AC-8 | Auto-refresh 30s | ✅ |
| AC-9 | Last updated timestamp | ✅ |
| AC-10 | API base = localhost:5001 | ✅ |
| AC-11 | لا يظهر نص "ذاكرة مؤقتة" | ✅ |
| AC-12 | dotnet build ينجح | ✅ (0 errors) |
