# TASK-UI-POLISH-002 — تحسين لوحة الإدارة الرئيسية (Admin Nav Index)

> **المرحلة:** UI Polish (حسب UI_POLISH_ROADMAP.md — رقم 4)
> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Approved for execution
> **المكلّف:** ui-designer
> **أولوية:** Medium

---

## 1. الوصف

تحسين صفحة لوحة الإدارة الرئيسية (`admin-secure-panel/Index.cshtml`) — استبدال أيقونات emoji بـ SVG icons واحترافية، وتحسين التباعد وتناسق الألوان.

**التصميم الحالي:** ✅ جيدة (بطاقات تنقل + responsive)
**التحسينات المقترحة:** SVG icons + تحسين gap وتناسق الألوان

---

## 2. Current State

الصفحة تحتوي على 9 بطاقات تنقل (nav cards) في Grid 3 أعمدة، كل بطاقة تحتوي على:
- أيقونة emoji (9 أيقونات)
- عنوان
- وصف
- Badge (بعضها)
- سهم `→` (HTML entity)

**الأيقونات الحالية (emoji/entities):**
| البطاقة | الأيقونة الحالية |
|---------|-----------------|
| إدارة الداشبوردات | 📋 `&#128203;` |
| إدارة البطاقات | 📊 `&#128202;` |
| مختبر الاستعلامات | 🔍 `&#128269;` |
| مختبِّر استعلامات Oracle | 🔎 `&#128270;` |
| تكوين التنقّل العميق | 🔗 `&#128279;` |
| شاشة المزامنة | 📋 `&#128203;` |
| إعدادات المزامنة | ⚙️ `&#9881;&#65039;` |
| سجلات المزامنة | 📄 `&#128196;` |
| تعيينات الجداول | 📋 `&#128203;` |

**السهم:** `→` (`&#8594;`) في كل بطاقة.

---

## 3. Required Changes

### A. استبدال جميع أيقونات emoji بـ SVG أيقونات مضمّنة (Inline SVG)

تصميم أيقونات SVG بسيطة وأنيقة تتناسب مع الهوية الزرقاء، مع:
- ألوان قابلة للتوريث من خلال `currentColor`
- stroke-width: 1.5 أو 2
- viewBox: 0 0 24 24
- نفس الأسلوب الموجود في Login.cshtml و Logout.cshtml

**الأيقونات المطلوبة:**

| البطاقة | الأيقونة المقترحة (وصف) |
|---------|------------------------|
| إدارة الداشبوردات | أيقونة Dashboard (شاشة/لوحة) مثل `<rect>` متعدد |
| إدارة البطاقات | أيقونة Cards/Grid (بطاقات/masonry) |
| مختبر الاستعلامات | أيقونة Search/Code (عدسة مكبرة) |
| مختبِّر استعلامات Oracle | أيقونة Database/DB (قاعدة بيانات) |
| تكوين التنقّل العميق | أيقونة Layers/Link (طبقات متداخلة) |
| شاشة المزامنة | أيقونة Sync/Refresh (سهم دائري) |
| إعدادات المزامنة | أيقونة Settings/Gear (ترس) |
| سجلات المزامنة | أيقونة Document/Log (مستند/قائمة) |
| تعيينات الجداول | أيقونة Table/Schema (جدول) |

### B. استبدال سهم `→` (HTML entity) بـ SVG Chevron

استخدام SVG سهم صغير (chevron right) بدلاً من الـ HTML entity.

### C. تنسيق الأيقونة
تغيير حجم `wd-nav-card__icon` من `font-size: 26px` + `width/height: 52px` إلى:
- حجم SVG: 24px × 24px
- حاوية الأيقونة: 48px × 48px (أصغر قليلاً وأنظف)
- لون SVG: `var(--c-primary)` عند hover

### D. تحسين Gap وتناسق الألوان
1. `--sp-6` → `--sp-8` للـ gap في grid (أوسع قليلاً)
2. البطاقات تأخذ أقل عرض للحفاظ على الانتظام
3. إضافة hover تأثير ناعم إضافي (زيادة طفيفة في الظل)
4. Badge: تغيير لون الخلفية من `--c-surface-muted` إلى `--c-primary` مع نص أبيض في hover

---

## 4. Design Guidelines

- **نفس CSS Variables** من Blue Identity Palette (موجودة في Login.cshtml)
- **SVG أيقونات مضمّنة** (لا emoji، لا صور، لا Font Awesome أو أي مكتبة خارجية)
- **لا تغيير في بنية HTML الأساسية** — حافظ على classNames الموجودة (`wd-nav-card`, `wd-nav-card__icon`, `wd-nav-card__title`, `wd-nav-card__desc`, `wd-nav-card__arrow`, `wd-nav-card__badge`)
- **نفس الـ animations** (staggered fadeInUp مع تأخير متدرج)
- **اتساق مع Login.cshtmول Logout.cshtml** في أسلوب SVG
- **Responsive:** حافظ على الـ breakpoints الموجودة (992px و 600px)

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml`

**ممنوع تعديل:** أي ملف آخر.

---

## 6. Acceptance Criteria

1. ✅ جميع أيقونات emoji (9 أيقونات) مستبدلة بـ SVG أيقونات مضمّنة
2. ✅ سهم `→` مستبدل بـ SVG chevron
3. ✅ حاوية الأيقونة 48×48 مع SVG 24×24
4. ✅ الأيقونات تستخدم `currentColor` و stroke قابلة للتوريث
5. ✅ أيقونة مختلفة لكل بطاقة لتمييزها بصرياً
6. ✅ Gap محسّن بين البطاقات
7. ✅ الألوان متناسقة (hover، badge، ظلال)
8. ✅ `dotnet build` PASS — 0 errors, 0 warnings
9. ✅ لا emoji — فقط SVG في الصفحة

---

## 7. Vitality & Polish Checklist
- [ ] 🟢 Toast Notifications — غير مطلوب
- [ ] 🟢 Connection Status — غير مطلوب
- [ ] 🟢 Search — غير مطلوب
- [x] ✅ Micro-animations — Hover effects + staggered entrance + arrow shift
- [ ] 🟢 Empty States — غير مطلوب
- [x] ✅ Realistic Data — النصوص عربية والمعلومات واقعية

---

## 8. References

- `Login.cshtml` — أسلوب SVG icon (مضمن، stroke-based, currentColor)
- `Logout.cshtml` — أسلوب SVG icon (نفس الطريقة)
- `UI_POLISH_ROADMAP.md §4` — وصف المهمة
- `28_UI_UX_GUIDELINES.md` — قواعد التصميم النهائية
