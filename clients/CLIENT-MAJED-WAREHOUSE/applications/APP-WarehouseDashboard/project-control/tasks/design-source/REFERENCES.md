# TASK-SYNC-SET-002 — Design References

## البحث الإلزامي — 2026-07-19

### Reference 1: صفحة Cards (المشروع الحالي)
- **المصدر:** `Pages/admin-secure-panel/Cards/Index.cshtml`
- **ما استلهمته:**
  - هيكل `.cards-stats` grid (4 أعمدة) مع بطاقات الإحصائيات
  - نمط `.admin-card` مع header/body/actions
  - ألوان `var(--c-*)` من blue-theme.css
  - تنسيق `.status-badge--on/off`
  - الأنيميشين `wdFadeUp` للتسلسل الهرمي
  - التجاوب (responsive) عند 767px و 480px
- **ما تجنبته:**
  - عدم تكرار `.cards-toolbar` لأن صفحة الإعدادات لا تحتاج بحث

### Reference 2: Admin Panel Sync Settings (Behance)
- **المصدر:** Convox - Call Center SaaS & UX UI Design
  - https://www.behance.net/gallery/225853541/Convox-Call-Center-SaaS-UX-UI-Design
- **ما استلهمته:**
  - تصميم بطاقات الإحصائيات (stat cards) بشكل شبكي
  - استخدام الأيقونات مع خلفيات ملونة
  - تنسيق الـ range slider مع الـ number input المتزامن
- **ما تجنبته:**
  - عدم استخدام الألوان الصارخة — التزمت بلوحة blue-theme

### Reference 3: Admin Dashboard Card Design (Behance)
- **المصدر:** Stratus CRM - SaaS & UX UI Dashboard Design
  - https://www.behance.net/gallery/215887035/Stratus-CRM-SaaS-UX-UI-Dashboard-Design
- **ما استلهمته:**
  - تبويب الإعدادات داخل بطاقة واحدة مع أقسام واضحة
  - استخدام toggle switch مع status text
  - عرض آخر مزامنة بتفاصيل historical
- **ما تجنبته:**
  - الـ tables المعقدة — استخدمت جدولاً بسيطاً يتسع للبيانات المتاحة

### Reference 4: Nova Dashboard (Behance)
- **المصدر:** Nova Dashboard — Modern Analytics Platform UI/UX Design
  - https://www.behance.net/gallery/252519657/Nova-Dashboard-Modern-Analytics-Platform-UIUX-Design
- **ما استلهمته:**
  - استخدام الـ preset buttons للفترات الزمنية
  - النصوص التوضيحية (hints) تحت الحقول
- **ما تجنبته:**
  - الـ hover effects المبالغ فيها

### Reference 5: TASK-SYNC-SET-002.md (مواصفات المشروع)
- **المصدر:** `project-control/tasks/TASK-SYNC-SET-002.md`
- **ما استلهمته:**
  - الهيكل الكامل للصفحة (HTML structure)
  - توزيع العناصر في بطاقة الإعدادات
  - خيارات الـ presets (5د/15د/30د/1س/6س/24س)
  - JS IIFE مع 5 وظائف محددة
- **ما تجنبته:**
  - الاعتماد على Syncfusion — استخدمت CSS فقط

---

## القرار البصري النهائي

| البند | القرار |
|-------|--------|
| **الاتجاه** | فاخر، بسيط، احترافي — متسق مع صفحة Cards |
| **الألوان** | `var(--c-*)` من blue-theme.css — أزرق أساسي مع أخضر/أحمر/بنفسجي/كهرماني |
| **الخط** | Cairo (من `var(--font-ar)`) |
| **المسافات** | نظام 4px scale: `var(--sp-*)` |
| **الظلال** | `var(--shadow-sm/md/lg)` |
| **الحركات** | `wdFadeUp` للبطاقات، hover effects بسيطة |
| **JS** | IIFE — لا متغيرات عامة |
| **RTL** | CSS Logical Properties (`inset-inline-start`, `padding-inline`) |
