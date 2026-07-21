# TASK-UI-POLISH-001 — إعادة تصميم صفحة تسجيل الخروج (Logout)

> **المرحلة:** UI Polish (حسب UI_POLISH_ROADMAP.md — رقم 3)
> **التاريخ:** 2026-07-21
> **الحالة:** ✅ Accepted (2026-07-21)
> **المكلّف:** ui-designer
> **أولوية:** Medium

---

## 1. Description

إعادة تصميم صفحة تسجيل الخروج (`/admin-secure-panel/Logout`) من صفحة بسيطة من 7 أسطر إلى صفحة خروج أنيقة احترافية تتناسق مع صفحة تسجيل الدخول (`Login.cshtml`) من حيث التصميم والهوية البصرية.

---

## 2. Current State

### Logout.cshtml (الوضع الحالي — 7 أسطر فقط)
```cshtml
@page
@model WarehouseDashboard.Web.Pages.AdminSecurePanel.LogoutModel
@{
    ViewData["Title"] = "تسجيل الخروج";
    Layout = "_Layout";
}
<p>جارٍ تسجيل الخروج...</p>
```

### Logout.cshtml.cs (الوضع الحالي)
```csharp
public IActionResult OnGet()
{
    HttpContext.Session.Remove(SessionKey);
    HttpContext.Session.Clear();
    return RedirectToPage("/admin-secure-panel/Login");
}
```

**المشكلة:** الـ `OnGet()` يعيد التوجيه فوراً إلى Login قبل أن تظهر أي واجهة للمستخدم. يجب تغييره ليعرض صفحة الخروج أولاً.

---

## 3. Required Changes

### A. Logout.cshtml.cs — تعديل الـ Backend
- **إزالة** `RedirectToPage("/admin-secure-panel/Login")` من `OnGet()`
- جعل `OnGet()` يمرّر إلى الصفحة (`Page()`) بدلاً من إعادة التوجيه
- إضافة property `RedirectDelaySeconds` بقيمة 4 (ثوانٍ)
- إضافة property `RedirectUrl` بقيمة `"/admin-secure-panel/Login"`
- الحفاظ على `HttpContext.Session.Remove(SessionKey); HttpContext.Session.Clear();`

### B. Logout.cshtml — إعادة تصميم كاملة
- **Layout = null;** (صفحة مستقلة مثل Login.cshtml)
- **Standalone HTML page** مع `<!DOCTYPE html>` واتجاه RTL
- **نفس التصميم Splitted أو Full-page Centered** المتسق مع Login.cshtml
- **عناصر الصفحة:**
  - أيقونة SVG وداع/خروج (مثل Warehouse SVG أو باب مفتوح)
  - رسالة ترحيبية وداع: "تم تسجيل الخروج بنجاح"
  - نص فرعي: "نتمنى لك يوماً موفقاً. سيتم تحويلك إلى صفحة الدخول..."
  - Spinner متحرك أثناء العد التنازلي
  - عداد تنازلي مرئي (4, 3, 2, 1...)
  - زر "العودة إلى تسجيل الدخول" (للمستخدم الذي لا يريد الانتظار)

### C. Auto-redirect Logic (JavaScript)
- Auto-redirect to Login page after 4 seconds
- Countdown timer display (visible seconds ticking down)
- Click handler on "العودة إلى تسجيل الدخول" button (immediate redirect)
- استخدام `setTimeout` + تحديث عداد كل ثانية

---

## 4. Design Guidelines

### Design Source: Login.cshtml (التصميم الحالي لصفحة الدخول)
- استخدم نفس **CSS Variables** من Blue Identity Palette (`--c-primary: #1F4E79`، إلخ)
- استخدم نفس **الخط** Cairo من Google Fonts
- استخدم **SVG أيقونات** (لا emoji)
- اتساق تام مع Login.cshtml من حيث:
  - Border-radius (`--radius-sm: 6px; --radius-md: 10px; --radius-lg: 14px; --radius-xl: 20px;`)
  - Shadows (`--shadow-sm/md/lg/xl`)
  - Typography (Cairo بأوزان 400/500/600/700)
  - Animations (`fadeInUp`، دوران spinner)
  - Responsive breakpoints (1024px, 768px, 375px)

### الهيكل المرئي المقترح
```
┌────────────────────────────────────────────────┐
│  LEFT PANEL (48%)          │  RIGHT PANEL      │
│  ┌────────────────────┐    │  ┌──────────────┐ │
│  │  [Warehouse Icon]  │    │  │ 🔒 Badge     │ │
│  │  Warehouse         │    │  │              │ │
│  │  Dashboard         │    │  │  👋 تم تسجيل │ │
│  │                    │    │  │  الخروج بنجاح │ │
│  │  • ميزة 1         │    │  │              │ │
│  │  • ميزة 2         │    │  │  [⟳ Spinner] │ │
│  │  • ميزة 3         │    │  │  إعادة توجيه  │ │
│  └────────────────────┘    │  │  خلال 4 ثوانٍ │ │
│                            │  │              │ │
│                            │  │  [زر العودة] │ │
│                            │  └──────────────┘ │
└────────────────────────────────────────────────┘
```

أو يمكن أن تكون صفحة **Full-page Centered** بتصميم أنظف إذا رأيت ذلك مناسباً، لكن يجب أن تتناسق مع Login.cshtml.

### Responsive
- Desktop (1200px+): Split أو Centered
- Tablet (768px): تكييف
- Mobile (375px): تكديس عمودي

---

## 5. Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml.cs`

**ممنوع تعديل:** أي ملف آخر (Login.cshtml, blue-theme.css, _Layout.cshtml، إلخ)

---

## 6. Acceptance Criteria

1. ✅ Logout.cshtml.cs: `OnGet()` يُرجع `Page()` بدلاً من `RedirectToPage()`، مع إضافة `RedirectDelaySeconds = 4` و `RedirectUrl = "/admin-secure-panel/Login"`
2. ✅ صفحة خروج كاملة (وليس 7 أسطر) — تصميم احترافي أنيق
3. ✅ نفس الألوان والخطوط والـ CSS tokens من Login.cshtml
4. ✅ SVG أيقونة وداع/خروج (لا emoji)
5. ✅ رسالة "تم تسجيل الخروج بنجاح" + نص فرعي
6. ✅ Spinner متحرك + عداد تنازلي مرئي
7. ✅ Auto-redirect بعد 4 ثوانٍ إلى Login
8. ✅ زر "العودة إلى تسجيل الدخول" للتحويل الفوري
9. ✅ Responsive (Desktop, Tablet, Mobile)
10. ✅ `dotnet build` PASS — 0 errors, 0 warnings
11. ✅ Auditor: NOT_REQUIRED (تغيير تجميلي بحت)

---

## 7. Vitality & Polish Checklist
- [ ] 🟢 Toast Notifications — غير مطلوب (صفحة انتقالية)
- [ ] 🟢 Connection Status Indicator — غير مطلوب
- [ ] 🟢 Search — غير مطلوب
- [ ] ✅ Micro-animations — Spinner + fadeInUp + عد تنازلي
- [ ] 🟢 Empty States — غير مطلوب
- [ ] ✅ Realistic Data — النصوص عربية واقعية

---

## 8. References

- `Login.cshtml` — التصميم المرجعي الأساسي (نفس الهوية)
- `UI_POLISH_ROADMAP.md §3` — وصف المهمة
- `28_UI_UX_GUIDELINES.md` — قواعد التصميم النهائية
- `KIT_ADMIN_DASHBOARD` — قاعدة التخطيط
- `blue-theme.css` — تعريفات CSS Variables
