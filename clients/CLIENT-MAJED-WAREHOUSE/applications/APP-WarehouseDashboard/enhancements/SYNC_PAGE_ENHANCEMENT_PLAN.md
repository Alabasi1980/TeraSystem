# خطة تطوير شاشة المزامنة — Sync Page Enhancement Plan

> **تاريخ الإنشاء:** 2026-07-15  
> **الحالة:** ✅ معتمدة للتنفيذ  
> **المشروع:** WarehouseDashboard — الماجد لادارة المستودعات  
> **النسخة:** v1.0

---

## 1. الخلفية والمشكلة الحالية

شاشات المزامنة الحالية (SyncSettings + SyncLogs) تعمل بشكل أساسي ولكنها بدائية:

| الشاشة | الوضع الحالي | المشكلة |
|---|---|---|
| **SyncSettings** | حقل فاصل زمني + toggle تشغيل/إيقاف | لا تحكم في أي جدول يُزامن، ولا جدولة متقدمة |
| **SyncLogs** | عرض بسيط لسجلات الذاكرة المؤقتة | سجلات تختفي عند إعادة التشغيل، لا تفاصيل لكل تعيين |
| **المزامنة نفسها** | كل التعيينات النشطة تُزامن معًا | لا اختيار، لا وضع (Full/Inc)، لا فلترة |

**الهدف:** تحويل شاشات المزامنة إلى واجهة متكاملة تنافسية تليق بمستخدم متقدم في إدارة المستودعات.

---

## 2. الرؤية النهائية (Target Vision)

شاشة مزامنة واحدة متكاملة (Sync Dashboard) تجمع:

```
┌─────────────────────────────────────────────────────────────┐
│  🗄️ لوحة المزامنة — Sync Dashboard                          │
├─────────────────────────────────────────────────────────────┤
│  ┌───┐  ┌──────────┐  ┌──────────┐  ┌──────────┐           │
│  │✅ │  │ 45,230   │  │ 2:15     │  │ 334/s    │           │
│  │IDL│  │ إجمالي   │  │ الوقت    │  │ السرعة   │           │
│  └───┘  └──────────┘  └──────────┘  └──────────┘           │
│                                                             │
│  ═══════════════════════════════════════════════════════     │
│                                                             │
│  [☰ تحديد الكل]  [▶ مزامنة المحدد]  [⟳ مزامنة الكل]       │
│  [📥 تصدير]  [🕒 جدولة متقدمة]                               │
│                                                             │
│  ┌───┬──────────┬────────┬────────┬──────┬─────────┬─────┐ │
│  │ # │ المصدر   │ الهدف  │ الوضع  │ سجلات│ الحالة  │     │ │
│  ├───┼──────────┼────────┼────────┼──────┼─────────┼─────┤ │
│  │☑  │ ST_ITEMS │StItems │ Full   │20,000│ ✅ تم   │[▶] │ │
│  │☐  │ ST_SALES │StSales │ Δ Inc  │   —  │ ◻️ جاهز  │[▶] │ │
│  │☑  │ Query03  │Items2  │ Full   │ 5,230│ ✅ تم   │[▶] │ │
│  └───┴──────────┴────────┴────────┴──────┴─────────┴─────┘ │
│                                                             │
│  ═══════════════════════════════════════════════════════     │
│                                                             │
│  📊 إحصائيات الأداء                                         │
│  ┌─────────────────────────────────────────────┐           │
│  │ ⏱️ آخر مزامنة: 2026-07-15 14:30             │           │
│  │ 📈 متوسط السرعة: 334 سجل/ثانية              │           │
│  │ 💾 حجم البيانات: 12.5 MB                    │           │
│  └─────────────────────────────────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. تفاصيل الميزات — مرحلية

### المرحلة الأولى — الأساسي (P0 — MVP للتطوير)
*هذه المرحلة تحوّل المزامنة من أداة بدائية إلى واجهة تحكم متقدمة.*

| # | الميزة | الوصف | أولوية |
|---|---|---|---|
| P0.1 | **Sync Dashboard (شاشة مزامنة موحدة)** | دمج SyncSettings + SyncLogs في شاشة واحدة تفاعلية | 🔴 |
| P0.2 | **اختيار الجداول للمزامنة** | جدول يعرض كل التعيينات مع Checkbox + تحديد الكل/إلغاء الكل | 🔴 |
| P0.3 | **زر مزامنة المحدد** | POST يرسل قائمة mapping Ids المحددة فقط | 🔴 |
| P0.4 | **شريط تقدم حي (Live Progress)** | Progress bar + عداد سجلات لكل تعيين أثناء المزامنة | 🔴 |
| P0.5 | **بطاقات ملخص** | 4 بطاقات: حالة المزامنة، إجمالي السجلات، الوقت، السرعة | 🟡 |

### المرحلة الثانية — متقدمة (P1)
| # | الميزة | الوصف | أولوية |
|---|---|---|---|
| P1.1 | **وضع المزامنة (Full / Incremental)** | اختيار Full Refresh أو Delta Sync لكل تعيين | 🟡 |
| P1.2 | **جدولة متقدمة** | دعم Cron (يومي، أسبوعي، مخصص) بدل مجرد interval | 🟡 |
| P1.3 | **مقارنة البيانات (Data Comparison)** | عرض Oracle vs SQL Server قبل/بعد المزامنة | 🟡 |
| P1.4 | **تصدير السجلات (CSV/PDF)** | زر تصدير لسجل المزامنة | 🟢 |

### المرحلة الثالثة — احترافية (P2)
| # | الميزة | الوصف | أولوية |
|---|---|---|---|
| P2.1 | **تصفية البيانات أثناء المزامنة (Filters)** | WHERE clause addition لكل تعيين (مثال: group_code = '03') | 🟡 |
| P2.2 | **إشعارات (Notifications)** | Webhook/Email عند اكتمال المزامنة أو فشلها | 🟢 |
| P2.3 | **سجل دائم في قاعدة البيانات** | بدل الذاكرة المؤقتة — تخزين في جدول SyncLogs | 🟡 |
| P2.4 | **خيارات النسخ الاحتياطي (Backup)** | نسخ احتياطي لجدول SQL Server قبل المزامنة | 🟢 |

---

## 4. هيكل الملفات المقترح (Files to Create/Modify)

### ملفات جديدة (New)

```
enhancements/SYNC_PAGE_ENHANCEMENT_PLAN.md          ← هذا الملف (الخطة)
enhancements/TRACKING.md                            ← تتبع المهام (TASK-ENH-*)

src/WarehouseDashboard.Web/Pages/admin-secure-panel/Sync/Index.cshtml    ← Sync Dashboard
src/WarehouseDashboard.Web/Pages/admin-secure-panel/Sync/Index.cshtml.cs ← Page Model

src/WarehouseDashboard.Api/Controllers/SyncController.cs
  ← إضافة endpoints:
     POST /api/sync/trigger-selected  ← لاستقبال قائمة mapping IDs
     GET  /api/sync/progress          ← للتقدم الحي

src/WarehouseDashboard.Api/Services/SyncEngineService.cs
  ← إضافة:
     RunSelectedMappingsAsync(List<int> mappingIds)
     <events or callbacks> ← للتقدم الحي
```

### ملفات موجودة ستحتاج تعديل

| الملف | التعديل |
|---|---|
| `WarehouseDashboard.Web/Pages/admin-secure-panel/Index.cshtml` | تحديث رابط "سجلات المزامنة" إلى `/admin-secure-panel/Sync` |
| `WarehouseDashboard.Web/Pages/admin-secure-panel/SyncSettings/Index.cshtml` | إلغاء أو تحويل إلى صفحة إعدادات متقدمة |
| `WarehouseDashboard.Web/Pages/admin-secure-panel/SyncLogs/Index.cshtml` | إلغاء أو دمج في Sync Dashboard |
| `WarehouseDashboard.Api/Controllers/SyncController.cs` | إضافة endpoints جديدة |
| `WarehouseDashboard.Api/Services/SyncEngineService.cs` | دعم المزامنة الانتقائية والتقدم الحي |

---

## 5. السير التقني (Technical Flow)

### P0.2 — مزامنة جداول محددة

```
مستخدم → يختار تعيينات في الـ Checkbox → يضغط "مزامنة المحدد"
  ↓
طلب POST /api/sync/trigger-selected
  Body: { mappingIds: [1, 3, 5] }
  ↓
SyncEngineService.RunSelectedMappingsAsync([1, 3, 5])
  ↓
لكل mappingId: استخراج من Oracle → SqlBulkCopy إلى SQL Server
  ↓
تحديث سجل المزامنة لكل تعيين على حدة
  ↓
إرجاع النتيجة: { status, recordsPerMapping: [...] }
```

### P0.4 — التقدم الحي (Live Progress)

```
المتصفح → POST /api/sync/trigger-selected
  ↓
API تُنشئ SyncRunId GUID وتُعيده فوراً
  ↓
المتصفح → polling GET /api/sync/progress?runId=GUID كل 2 ثانية
  ↓
API تُعيد:
{
  "runId": "...",
  "overallPercent": 45,
  "mappings": [
    { "id": 1, "status": "running", "rowsSoFar": 8500, "percent": 85 },
    { "id": 3, "status": "completed", "rowsSoFar": 5230, "percent": 100 },
    { "id": 5, "status": "pending", "rowsSoFar": 0, "percent": 0 }
  ],
  "elapsedSeconds": 12
}
```

---

## 6. هيكل واجهة المستخدم المقترحة (UI Structure)

```
Sync Dashboard (/admin-secure-panel/Sync)
│
├── 🗄️ شريط ملخص (Summary Cards Row)
│   ├── حالة المزامنة 🟢 نشطة / 🔴 متوقفة
│   ├── إجمالي السجلات (45,230)
│   ├── وقت آخر مزامنة (14:30)
│   └── سرعة الأداء (334/ثانية)
│
├── ⚡ أشرطة التقدم (Progress Section)
│   └── لكل تعيين جاري: شريط + عداد سجلات + حالة
│
├── 📋 جدول التعيينات (Mappings Table)
│   ├── Checkbox (تحديد الكل / إلغاء الكل)
│   ├── المصدر (Oracle)
│   ├── الهدف (SQL Server)
│   ├── وضع المزامنة (Full / Incremental)
│   ├── آخر مزامنة (تاريخ + عدد سجلات)
│   ├── الحالة ⬤ (ناجح / فشل / قيد التنفيذ / معلّق)
│   └── زر ▶ (تشغيل تعيين واحد)
│
├── 🔘 أزرار الإجراءات (Action Buttons)
│   ├── ▶ مزامنة المحدد (للتعيينات المختارة فقط)
│   ├── ⟳ مزامنة الكل (جميع التعيينات النشطة)
│   └── 📥 تصدير التقرير
│
└── 📊 إحصائيات الأداء (Performance Stats)
    ├── الوقت الإجمالي
    ├── السجلات/ثانية
    └── حجم البيانات
```

---

## 7. أولويات التنفيذ

```text
Phase 1 (P0) — الأساسي
├── P0.1: Sync Dashboard UI — الإطار الأساسي
├── P0.2: اختيار الجداول + مزامنة المحدد
├── P0.3: شريط تقدم حي
└── P0.4: بطاقات ملخص

Phase 2 (P1) — متقدمة
├── P1.1: وضع المزامنة (Full/Incremental)
├── P1.2: جدولة متقدمة (Cron)
├── P1.3: مقارنة البيانات
└── P1.4: تصدير السجلات

Phase 3 (P2) — احترافية
├── P2.1: تصفية البيانات
├── P2.2: إشعارات
├── P2.3: سجل دائم
└── P2.4: نسخ احتياطي
```

---

## 8. TASK-ID Registry

| المهمة | المرحلة | الوصف | الحالة |
|---|---|---|---|
| TASK-ENH-001 | P0.1 | إنشاء شاشة Sync Dashboard (UI) | ⬜ Pending |
| TASK-ENH-002 | P0.2 | إضافة اختيار الجداول + مزامنة المحدد (API + UI) | ⬜ Pending |
| TASK-ENH-003 | P0.3 | شريط تقدم حي (API progress endpoint + UI) | ⬜ Pending |
| TASK-ENH-004 | P0.4 | بطاقات ملخص (Summary Cards) | ⬜ Pending |
| TASK-ENH-005 | P1.1 | وضع المزامنة (Full / Incremental) | ⬜ Pending |
| TASK-ENH-006 | P1.2 | جدولة متقدمة (Cron) | ⬜ Pending |
| TASK-ENH-007 | P1.3 | مقارنة البيانات | ⬜ Pending |
| TASK-ENH-008 | P1.4 | تصدير السجلات | ⬜ Pending |
| TASK-ENH-009 | P2.1 | تصفية البيانات أثناء المزامنة | ⬜ Pending |
| TASK-ENH-010 | P2.2 | إشعارات | ⬜ Pending |
| TASK-ENH-011 | P2.3 | سجل دائم في قاعدة البيانات | ⬜ Pending |
| TASK-ENH-012 | P2.4 | نسخ احتياطي | ⬜ Pending |

---

## 9. القرارات التقنية (Technical Decisions)

| القرار | الخيار | المبرر |
|---|---|---|
| **Progress tracking** | Polling (كل 2 ثانية) | أبسط من SignalR، متوافق مع البنية الحالية (API منفصلة) |
| **مزامنة محددة** | POST بقائمة mappingIds | مرن، لا يحتاج تغيير هيكل الـ SyncEngine جذرياً |
| **Progress store** | Dictionary<Guid, SyncRunProgress> في الذاكرة | مؤقت للمرحلة الأولى، يمكن تحويله لاحقاً إلى DB |
| **UI Framework** | Razor Pages + JS (بدون Syncfusion للتقدم) | لنفس أسلوب باقي الشاشات، مرن للتحديث الحي |
| **Incremental Sync** | تأجيل للمرحلة الثانية | يحتاج تغييرات أكبر في SqlServerLoadService |

---

## 10. ملفات التحكم والتتبع

سيتم تتبع المهام في:

```text
enhancements/TRACKING.md    ← سجل التتبع
project-control/            ← TASK-ENH-* سيتم إنشاؤها هنا
```

---

## 11. الموافقة

| البند | الحالة |
|---|---|
| تمت مراجعة الخطة | ✅ |
| موافقة المستخدم | ⬜ ينتظر التأكيد |
| جاهز للتنفيذ | ⬜ بعد الموافقة |
