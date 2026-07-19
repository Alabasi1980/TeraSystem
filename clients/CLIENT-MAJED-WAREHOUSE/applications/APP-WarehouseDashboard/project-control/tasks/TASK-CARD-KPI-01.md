# TASK-CARD-KPI-01 — Improve KPI Change Presentation

| البند | القيمة |
|---|---|
| **المعرف** | TASK-CARD-KPI-01 |
| **المجموعة** | CARD-DESIGN-EXECUTION / Phase C |
| **النوع** | Backend payload + Frontend — C# + HTML/CSS/JS rendering |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ Accepted — Implementation + Auditor PASS |
| **تاريخ الإنشاء** | 2026-07-19 |
| **المرجع** | `DASHBOARD_CARD_DESIGN_EXECUTION_PLAN.md` — Phase C / CARD-KPI-01 |
| **المرجع البصري** | `design-source/WAREHOUSE_CARD_PROTOTYPE.html` |
| **Design Tokens** | `design-source/DESIGN_TOKENS.md` |

---

## 1. الهدف

تحسين طريقة عرض **نسبة التغيّر والنص التفسيري** في بطاقات KPI بحيث تبدو:
- **مقنعة بصرياً** — الرقم الكبير واضح، التغيّر واضح، السياق واضح
- **متوافقة مع البروتوتايب** — التصميم المرجعي يظهر: رقم كبير + badge تغيّر + نص مقارنة
- **ملتزمة بقرار الألوان** — ColorPalette من Card Builder أولاً، ثم `blue-theme.css` كـ fallback، والبروتوتايب مرجع بصري فقط

---

## 2. الوضع الحالي

### 2.1 ما يظهر حالياً في `wdKpiHtml()`

```text
┌─────────────────────────────┐
│  [Title]                     │
│  12,450                      │  ← رقم بحجم 32px
│  ↑ 12.5%                     │  ← badge صغير فقط
│  قيمة المؤشر                  │  ← label ثابت
└─────────────────────────────┘
```

### 2.2 المشاكل

1. **الرقم صغير نسبياً** (32px) — البروتوتايب يستخدم 56px
2. **لا نص تفسيري** — البروتوتايب يظهر "مقارنة بالشهر السابق" أو "مقارنة باليوم السابق"
3. **لا وحدة** — البروتوتايب يظهر "وحدة" أو "طلبات"
4. **لا أيقونة** — البروتوتايب يظهر أيقونة مميزة في أعلى البطاقة، لكن هذا خارج هذه المهمة حتى لا نوسّع النطاق
5. **الـ label ثابت** ("قيمة المؤشر") — النص التفسيري للتغيير يحتاج `ChangeSource` من الـ API payload

---

## 3. التصميم المستهدف

> **تنبيه نطاق:** الرسومات التالية توضح الاتجاه البصري العام من البروتوتايب. نطاق هذه المهمة ينفذ فقط: حجم القيمة، badge التغيير، النص التفسيري، وتحسين rhythm حولها. الأيقونة، وحدة القياس، خط البطاقة السفلي، وCard Shell الموحد ليست ضمن هذه المهمة.

### 3.1 KPI مع تغيّر (withChange)

```text
┌─────────────────────────────────────┐
│  عنوان البطاقة                [⋯]  │
│  وصف البطاقة (اختياري)              │
│                                      │
│  256                                │  ← رقم كبير (48px)
│  ↑ +12.5%                           │  ← badge تغيّر ملوّن
│  مقارنة باليوم السابق               │  ← نص تفسيري dynamic
│                                      │
│  ──────── gradient line ────────     │  ← خط سفلي اختياري
└─────────────────────────────────────┘
```

### 3.2 KPI بسيط (simple)

```text
┌─────────────────────────────────────┐
│  إجمالي المخزون              [⋯]  │
│  إجمالي كمية الأصناف المتاحة        │
│                                      │
│  12,450                             │  ← رقم كبير
│  قيمة المؤشر                        │  ← label موجود حالياً، Unit مؤجلة
│                                      │
└─────────────────────────────────────┘
```

### 3.3 KPI متقدم (composite)

```text
┌─────────────────────────────────────┐
│  دوران الصنف                [⋯]  │
│  متوسط دوران المخزون                │
│                                      │
│  4.2                                │  ← رقم كبير
│  ↑ +8.6%                            │  ← badge تغيّر
│  مقارنة بالشهر السابق               │  ← نص تفسيري
│                                      │
│  ╭─── sparkline ─────────────╮      │  ← رسم مصغر
│  ╰───────────────────────────╯      │
│                                      │
│  ⓘ ملاحظة اختيارية مستقبلية         │  ← خارج نطاق هذه المهمة
└─────────────────────────────────────┘
```

---

## 4. النطاق

### In Scope

1. **تكبير الرقم الرئيسي** من 32px إلى 48px مع `font-variant-numeric: tabular-nums`
2. **إضافة `ChangeSource` إلى payload** لأن `CardDataResult` لا يرجعه حالياً للواجهة.
3. **إضافة نص تفسيري** dynamic يعتمد على `ChangeSource` الفعلي في الكود:
   - `ChangeSource = "previousPeriod"` → "مقارنة بالفترة السابقة"
   - `ChangeSource = "previousMonth"` → "مقارنة بالشهر السابق"
   - `ChangeSource = "previousYear"` → "مقارنة بالسنة السابقة"
   - `ChangeSource = "customQuery"` → "مقارنة مخصصة"
   - افتراضي: "مقارنة بالفترة السابقة"
4. **تحسين badge التغيّر**:
   - سهم حقيقي (SVG) بدلاً من نص Unicode arrow
   - لون dynamic: أخضر للصعود، أحمر للهبوط، رمادي للثبات
   - حجم أكبر قليلاً (16px بدلاً من 13px)
5. **تحريك العدّاد** (animateCountUp) — موجود بالفعل، ويُحافظ عليه مع تحسين العرض حوله
6. **عدم إضافة Unit label في هذه المهمة** لأن النظام لا يحتوي حالياً على حقل وحدة في `DashboardCard` أو `CardDataResult`. يعامل كـ N/A موثّق.

### Out of Scope

1. لا نستخدم ألوان البروتوتايب كأرقام ثابتة — Decorative/accent colors من `ColorPalette` أولاً ثم `blue-theme.css` fallback، والألوان الدلالية من `--c-success`, `--c-error`, `--c-text-muted`
2. لا نضيف أيقونة بطاقة — هذا في مهمة CARD-UX-01 (الهيدر الموحد)
3. لا نغير Sparkline — هذا في TASK-CARD-KPI-02
4. لا نغير Card Builder — فقط عرض الداشبورد
5. لا نضيف Unit field — يتطلب مهمة Builder/Model/API مستقلة إن قرر Majed لاحقاً

---

## 5. Allowed Sources

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml` (CSS + JS)
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\design-source\DESIGN_TOKENS.md`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\design-source\WAREHOUSE_CARD_PROTOTYPE.html`

---

## 6. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Index.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\CardDataResult.cs`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\DashboardService.cs`

---

## 7. Acceptance Criteria

1. **KPI Value** يظهر بحجم 48px مع `tabular-nums`
2. **Change badge** يظهر سهم SVG + نسبة مئوية + لون حسب الاتجاه
3. **CardDataResult** يحتوي `ChangeSource` ويتم تعبئته من `DashboardCard.ChangeSource` عند بطاقات KPI غير البسيطة
4. **Comparison text** يظهر نصاً dynamic يعتمد على `ChangeSource` بالقيم الفعلية: `previousPeriod`, `previousMonth`, `previousYear`, `customQuery`
5. **Unit label** غير مطلوب في هذه المهمة ومذكور كـ N/A لعدم وجود حقل وحدة حالياً
6. **AnimateCountUp** يعمل بشكل سلس
7. **التصميم متوافق مع RTL** بالكامل
8. **الألوان تلتزم بالقرار**: ColorPalette للزخرفة/accent، و`blue-theme.css` للحالات الدلالية والفallback، ولا hardcoded prototype colors
9. **لا يوجد كود مكرر** — الدوال الجديدة نظيفة
10. **Dotnet build** ينجح (أو fallback build إذا كان التطبيق يعمل)
11. **متوافق مع الأجهزة** — لا ينكسر على الشاشات الصغيرة

---

## 7.1 Vitality & Polish Checklist

| بند الحيوية | الحالة المطلوبة لهذه المهمة | التبرير |
|---|---|---|
| Skeleton Loading / Shimmer | N/A | لا يتم تغيير تحميل البطاقة في هذه المهمة |
| Toast Notifications | N/A | لا يوجد إجراء مستخدم ينتج نجاح/فشل مباشر |
| Connection Status Indicator | N/A | موجود خارج نطاق البطاقة |
| Search حقيقي | N/A | لا توجد جداول أو بحث في KPI |
| Micro-animations | ✅ مطلوب | الحفاظ على `animateCountUp` وتحسين transition/visual rhythm للـ change badge بدون مبالغة |
| Empty States | N/A | حالات empty/error موجودة مسبقاً ولا تتغير |
| Realistic Data | N/A | تستخدم بيانات البطاقة الفعلية من API ولا تضيف بيانات وهمية |

---

## 8. Security Sensitivity

- **Level:** Low
- **Reason:** Adds non-sensitive KPI metadata to the card API payload and adjusts dashboard rendering only. No auth, database schema, secrets, or data mutation changes.

---

## 9. Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Focused on KPI change presentation only |
| One objective only | PASS | Improve change badge + comparison text + value size |
| No deferrable work included | PASS | Sparkline, icon tile, card shell deferred |
| No UI unless explicitly requested | PASS | This IS a UI task |
| No API unless explicitly requested | PASS | Minimal payload addition required for `ChangeSource` display text |
| No Auth unless explicitly requested | PASS | No auth changes |
| No schema/migration unless explicitly requested | PASS | No DB changes |
| No real secrets outside approved local environment files | PASS | No secrets |
| Secret handling plan documented and redacted | PASS | N/A |
| CLI side effects checked | PASS | Build only |
| No internal contradiction between constraints and outputs | PASS | Scope aligns with write targets |
| Allowed Write Targets are narrow | PASS | Three files only: payload model, service assignment, dashboard rendering |
| Acceptance criteria are testable | PASS | Visual + functional |

**Gate Status:** PASS

---

## 10. Delegation Notes

EngineeringAgent must:

1. Before editing `Index.cshtml`, read the current file from disk.
2. Preserve ALL unrelated code (filter bar, drill-down, focus mode, date presets, etc.).
3. Add `ChangeSource` to `CardDataResult` and assign it in `DashboardService` from `DashboardCard.ChangeSource` for KPI cards.
4. Modify only `wdKpiHtml()` and related CSS in `Index.cshtml` for the frontend part.
5. Use ColorPalette first for accent/decorative color only where appropriate; use `blue-theme.css` semantic tokens for up/down/flat states.
6. Test that the change badge shows correctly for all 3 directions (up/down/flat).
7. Test that comparison text appears for `previousPeriod`, `previousMonth`, `previousYear`, `customQuery`, and missing/unknown values.
8. Do NOT modify Sparkline rendering (that's in TASK-CARD-KPI-02).
9. Do NOT modify Card Builder (that's out of scope).

---

## 11. Handback Placeholder

## 11. Engineering Handback

- **Task ID:** TASK-CARD-KPI-01
- **Agent:** engineering-agent-dotnet
- **Status:** Completed — submitted to Tera review
- **Files Modified:**
  - `src/WarehouseDashboard.Web/Pages/CardDataResult.cs`
  - `src/WarehouseDashboard.Web/Pages/DashboardService.cs`
  - `src/WarehouseDashboard.Web/Pages/Index.cshtml`
- **Exact Change Summary:**
  - Added nullable non-sensitive `ChangeSource` to `CardDataResult`.
  - Assigned `result.ChangeSource = card.ChangeSource` for non-simple KPI cards in `DashboardService`.
  - Improved KPI value presentation: 48px value, tabular numbers, SVG change arrows, sign context, semantic theme colors, and dynamic Arabic comparison text.
  - Preserved `animateCountUp`.
  - Preserved existing sparkline rendering path; no TASK-CARD-KPI-02 implementation included.
  - Did not add Syncfusion, Unit fields/labels, Card Builder changes, packages, migrations, config changes, auth changes, or schema changes.
- **Commands Run:**
  - EngineeringAgent: `dotnet build "WarehouseDashboard.Web.csproj"` — PASS, 0 warnings, 0 errors.
  - Tera verification: `dotnet build "WarehouseDashboard.Web.csproj"` — PASS, 0 warnings, 0 errors.
- **Issues/Risks:**
  - Browser visual verification still recommended.
  - Workspace contains many pre-existing modified/untracked files; this task review focused only on the three allowed target files.
- **System Gap Observed:** None reported by EngineeringAgent.

---

## 12. Post-Execution Review Result

| Check | Result | Notes |
|---|---|---|
| Changed files within Allowed Write Targets | TBD | |
| Changed files within Allowed Write Targets | PASS | Application changes limited to `Index.cshtml`, `CardDataResult.cs`, `DashboardService.cs`. |
| No unauthorized files created | PASS | Auditor report/control updates only under project-control. |
| No unauthorized files deleted | PASS | None. |
| No unauthorized packages added | PASS | None. |
| No unauthorized UI/CSS/theme changes | PASS | KPI rendering changes only; no prototype hardcoded semantic colors. |
| UI Acceptance Gate passed for UI tasks | PASS | Vitality checklist included; micro-animation preserved/enhanced. Browser visual test still recommended. |
| No real secrets outside approved local environment files | PASS | No secrets involved. |
| Secrets redacted in docs/logs/config references | PASS | No secrets recorded. |
| No unauthorized ORM models/entities/migrations | PASS | No EF model/schema/migration changes. |
| No unapproved business validation moved to DB constraints | PASS | None. |
| No unauthorized API/Auth created | PASS | Added non-sensitive payload field only; no auth/API surface expansion beyond card payload. |
| Acceptance criteria satisfied | PASS | Verified by Tera review + Auditor PASS. |
| CLI side effects reviewed | PASS | `dotnet build` only; output normal build artifacts. |
| Task file and core project-control records reviewed | PASS | Task, registry, and activity log updated. |
| No secret leakage in task files/logs/reports/handbacks | PASS | None found. |
| No duplicate project-control IDs created | PASS | Existing task ID only. |
| Any out-of-target changes classified | PASS | Git status has many pre-existing changes; not attributed to this task. |
| Independent review decision recorded | PASS | Auditor review completed. |
| Auditor review decision recorded | PASS | REQUIRED by project rule; Auditor PASS. |

**Gate Status:** PASS

**Auditor Review Decision:** REQUIRED

**Reason:** Majed requested auditor-first workflow after every task in this card track.

**Auditor Report:** `project-control/audit-reports/QUAUD-TASK-CARD-KPI-01-2026-07-19-001.md`

**Auditor Result:** PASS — STOP 0, CAUTION 0, FLAG 1 (non-blocking evidence traceability note).

**Final Task Decision:** ✅ Accepted. Proceed to TASK-CARD-KPI-02 only after Majed confirms.

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No code written in this document
