# TASK-CARD-LIST-001 — تطوير صفحة إدارة البطاقات

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-LIST-001 |
| **المجموعة** | UI-IMPROVEMENT |
| **النوع** | Frontend Razor + CSS + JS |
| **الوكيل المقترح** | ui-designer |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

تحويل صفحة إدارة البطاقات (`/admin-secure-panel/Cards`) من جدول Radzen خام إلى تصميم احترافي يعرض البطاقات بشكل بصري منظم مع معلومات كافية للمستخدم الإداري.

---

## 2. المشكلة الحالية

الصفحة الحالية عبارة عن:
- جدول Radzen DataGrid بأعمدة تقنية خام (X, Y, العرض, الارتفاع, التحديث بالثواني)
- لا إحصائيات ملخصة
- لا بحث ولا تصفية
- لا تمثيل بصري للبطاقة
- معلومات ناقصة (لا وصف، لا ColorPalette، لا KpiMode)
- أسماء أعمدة تقنية لا تخدم المستخدم

---

## 3. التصميم المستهدف

### 3.1 شريط الإحصائيات (Summary Bar)

في أعلى الصفحة، قبل قائمة البطاقات:

```
┌─────────────┬─────────────┬─────────────┬─────────────┐
│  إجمالي     │  نشطة       │  KPI        │  Charts     │
│  البطاقات   │             │             │             │
│     12      │     10      │     5       │     7       │
└─────────────┴─────────────┴─────────────┴─────────────┘
```

- كل كتلة: أيقونة + عنوان + رقم
- ألوان: إجمالي (أزرق)، نشطة (أخضر)، KPI (بنفسجي)، Charts (برتقالي)
- خلفية `var(--c-surface)` مع ظل خفيف

### 3.2 شريط البحث والتصفية

```
┌──────────────────────────┬──────────────┬──────────────┐
│  🔍 ابحث بعنوان البطاقة… │  KPI ▼       │  الكل ▼      │
└──────────────────────────┴──────────────┴──────────────┘
```

- حقل بحث بالعنوان (debounce 200ms)
- قائمة منسدلة لتصفية حسب النوع (KPI, Bar, Line, Pie, Table, Gauge, الكل)
- قائمة منسدلة لتصفية حسب الحالة (الكل, نشطة, غير نشطة)

### 3.3 قائمة البطاقات (Card List)

بدلاً من جدول، عرض كل بطاقة كـ "بطاقة إدارية" (Admin Card):

```
┌─────────────────────────────────────────────────────────────────────┐
│  📊  الوحدات                                     KPI  ● نشطة     │
│      عدد الوحدات المسجلة في المستودع                            │
│      ─────────────────────────────────────────────────────────     │
│      🎨 primary   ⏱️ تحديث كل ساعة   📐 3×2   📅 dashboard      │
│                                                                  │
│      [تعديل]  [نسخ]  [حذف]                                      │
└─────────────────────────────────────────────────────────────────────┘
```

**كل بطاقة إدارية تشمل:**

| العنصر | الوصف |
|---|---|
| أيقونة النوع | أيقونة حسب ChartType (📊 KPI, 📈 Bar, 📉 Line, 🥧 Pie, 📋 Table, ⏱️ Gauge) |
| العنوان | `c.Title` — بخط عريض |
| نوع الرسم | Badge ملون حسب النوع |
| حالة النشاط | Badge أخضر (نشطة) أو أحمر (غير نشطة) |
| الوصف | `c.Description` — بخط أصغر ورمادي (إذا وُجد) |
| معلومات الإعدادات | سطر صغير يعرض: ColorPalette + RefreshInterval (بصيغة مقروءة) + حجم الشبكة + DateFilterMode |
| أزرار الإجراءات | تعديل (链接 للـ Builder) + نسخ (Clone) + حذف (weetalert) |

**تصميم Admin Card:**
- خلفية `var(--c-surface)`
- حد: `1px solid var(--c-border)`
- Border-radius: `var(--radius-lg)`
- ظل: `var(--shadow-sm)` → `var(--shadow-md)` عند Hover
- Padding: `var(--sp-4)`到 `var(--sp-5)`
- عرض: 100% (قائمة عمودية)
- Animation: `wdFadeUp` مع `animation-delay` متدرج

### 3.4 تحويل RefreshInterval إلى صيغة مقروءة

| القيمة | النص المعرض |
|---|---|
| 0 | بدون تحديث تلقائي |
| 60 | تحديث كل دقيقة |
| 300 | تحديث كل 5 دقائق |
| 3600 | تحديث كل ساعة |
| 86400 | تحديث كل يوم |
| أخرى | تحديث كل X ثانية |

### 3.5 أزرار الإجراءات

| الزر | التصميم | السلوك |
|---|---|---|
| تعديل | `wd-btn wd-btn--ghost wd-btn--sm` | ينتقل إلى `/admin-secure-panel/Cards/Builder?edit={id}` |
| نسخ | `wd-btn wd-btn--ghost wd-btn--sm` | ينتقل إلى `/admin-secure-panel/Cards/Builder?clone={id}` |
| حذف | `wd-btn wd-btn--ghost wd-btn--sm wd-btn--danger` | SweetAlert تأكيد → POST Delete |

### 3.6 Empty State

يبقى كما هو (موجود بالفعل) — لكن مع تحسين بسيط: أيقونة أكبر ونص أكثر وضوحاً.

---

## 4. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Index.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\Index.cshtml.cs
```

**ملاحظة:** الـ CSS يجب أن يكون داخل `<style>` في الـ cshtml (لا ملف منفصل لأن هذه الصفحة لها design خاص بها).

---

## 5. التغييرات المطلوبة في Backend (Index.cshtml.cs)

### 5.1 توسيع CardRow

```csharp
public record CardRow(
    int Id,
    string Title,
    string? Description,        // NEW
    string ChartType,
    string DataSourceType,
    bool IsActive,
    string ColorPalette,        // NEW
    int GridWidth,
    int GridHeight,
    int RefreshInterval,
    string DateFilterMode,      // NEW
    string KpiMode);            // NEW
```

### 5.2 تحديث الاستعلام

```csharp
Cards = await _db.DashboardCards
    .OrderBy(c => c.GridPositionY)
    .ThenBy(c => c.GridPositionX)
    .Select(c => new CardRow(
        c.Id, c.Title, c.Description, c.ChartType, c.DataSourceType,
        c.IsActive, c.ColorPalette, c.GridWidth, c.GridHeight,
        c.RefreshInterval, c.DateFilterMode ?? "dashboard", c.KpiMode ?? "simple"))
    .ToListAsync();
```

---

## 6. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | شريط الإحصائيات يظهر (إجمالي + نشطة + KPI + Charts) |
| AC-2 | حقل بحث يعمل (تصفية بالعنوان مع debounce) |
| AC-3 | قائمة تصفية حسب النوع تعمل |
| AC-4 | كل بطاقة تُعرض كـ Admin Card (وليس صف في جدول) |
| AC-5 | كل بطاقة تُظهر: العنوان + النوع + الحالة + الوصف + ColorPalette + RefreshInterval + DateFilterMode |
| AC-6 | أزرار تعديل/نسخ/حذف تعمل |
| AC-7 | حذف يحتاج تأكيد |
| AC-8 | RefreshInterval يُعرض بصيغة مقروءة (وليس رقم خام) |
| AC-9 | Empty State يظهر عندما لا توجد بطاقات |
| AC-10 | `dotnet build` ناجح بدون أخطاء |
| AC-11 | لا تراجع على الميزات الأخرى (Theme Switcher, Header, Toast) |
| AC-12 | التصميم يتوافق مع الهوية البصرية (blue-theme.css) |

---

## 7. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ صفحة واحدة (Index.cshtml + Index.cshtml.cs) |
| لا تغيير Backend جذري | ✅ فقط توسيع CardRow + الاستعلام |
| لا تغيير Auth | ✅ |
| Allowed Write Targets ضيقة | ✅ ملفان فقط |
| معايير القبول قابلة للاختبار | ✅ |

**Gate Status:** ✅ PASS

---

## 8. ملاحظات للوكيل

1. **لا تستخدم Syncfusion** — هذا القيد ساري. استخدم CSS فقط.
2. **التصميم RTL** — الصفحة عربية، تأكد من `direction: rtl` في العناصر.
3. **استخدم متغيرات blue-theme.css** — لا تكرر تعريفات CSS.
4. **الـ Badge للألوان:**
   - KPI: `background: rgba(139, 92, 246, 0.12); color: #7C3AED;`
   - Bar: `background: rgba(59, 130, 246, 0.12); color: #2563EB;`
   - Line: `background: rgba(16, 185, 129, 0.12); color: #059669;`
   - Pie: `background: rgba(245, 158, 11, 0.12); color: #D97706;`
   - Table: `background: rgba(107, 114, 128, 0.12); color: #4B5563;`
   - Gauge: `background: rgba(239, 68, 68, 0.12); color: #DC2626;`
5. **Animation:** استخدم `wdFadeUp` مع `animation-delay: @(i * 60)ms` للبطاقات.
6. **البحث والتصفية:** كلاهما يعمل فقط في الـ Client-side (لا حاجة لتعديل Backend).
7. **الحذف:** استخدم `window.confirm` بدلاً من SweetAlert (موجود بالفعل في الكود الحالي).
8. **الـ Clone handler** موجود في Backend (`OnPostCloneAsync`) — لا حاجة لتعديله.
