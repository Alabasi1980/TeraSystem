# TASK-HEADER-001 — Shared Header Partial + Link Fix + Connection Status

| البند | القيمة |
|---|---|
| **المعرف** | TASK-HEADER-001 |
| **المجموعة** | UI-IMPROVEMENT |
| **النوع** | Frontend Razor + CSS |
| **الوكيل المقترح** | engineering-agent-dotnet |
| **الأولوية** | High |
| **الحالة** | ✅ ACCEPTED (2026-07-19) |
| **تاريخ الإنشاء** | 2026-07-19 |

---

## 1. الهدف

1. **إنشاء Header مشترك** — ملف `_Header.cshtml` partial يُستخدم من كلا الـ Layouts
2. **إصلاح الروابط** — Dashboard brand → `/`, Admin brand → `/admin-secure-panel`
3. **إضافة Connection Status في Admin** — يظهر مؤشر اتصال قاعدة البيانات
4. **إصلاح aria-label** — ليكون دقيقاً حسب الموقع

---

## 2. Allowed Write Targets

```
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\Shared\_Header.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\_DashboardLayout.cshtml
D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Cards\_CardsLayout.cshtml
```

---

## 3. تفاصيل التنفيذ

### 3.1 إنشاء `_Header.cshtml`

المسار: `Pages/Shared/_Header.cshtml`

يقبل المعاملات عبر `@model` أو `ViewData`:
- `BrandHref` — رابط اسم المشروع (القيمة الافتراضية: `/`)
- `BrandText` — نص اسم المشروع
- `IsAdmin` — هل نحن في صفحة Admin (يُظهر زر Logout)
- `ShowConnectionStatus` — هل يُظهر مؤشر اتصال قاعدة البيانات (الافتراضي: true)

```html
@{
    var brandHref = ViewData["BrandHref"] as string ?? "/";
    var brandText = ViewData["BrandText"] as string ?? "Warehouse Dashboard";
    var isAdmin = ViewData["IsAdmin"] as bool? ?? false;
    var showConn = ViewData["ShowConnectionStatus"] as bool? ?? true;
}
<header class="wd-topbar">
    <a href="@brandHref" class="wd-topbar__brand" aria-label="@(isAdmin ? "العودة للوحة المعلومات" : "الرئيسية")">
        <img class="wd-topbar__logo-img" src="~/images/logo-placeholder.svg" alt=""
             onerror="this.style.display='none';this.nextElementSibling.style.display='inline-block';" />
        <span class="wd-logo-dot" style="display:none;"></span>
        <span class="wd-topbar__brand-text">@brandText</span>
    </a>
    <div class="wd-topbar__actions">
        @if (showConn) {
            <span id="wd-conn" class="wd-conn wd-conn--checking" title="حالة الاتصال بقاعدة البيانات">
                <span class="wd-conn__dot"></span>
                <span id="wd-conn-text">جارٍ الفحص…</span>
            </span>
        }
        <span id="wd-sync-status" class="wd-sync-status" title="حالة محرك المزامنة">
            <span class="wd-sync-status__dot"></span>
            <span class="wd-sync-status__text" id="wd-sync-status-text">...</span>
        </span>
        <div class="wd-theme-switcher">
            <button type="button" class="wd-theme-switcher__btn" id="wd-theme-toggle" title="غيّر الثيم" aria-label="غيّر الثيم">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                    <circle cx="12" cy="12" r="3"/><path d="M12 1v2M12 21v2M4.22 4.22l1.42 1.42M18.36 18.36l1.42 1.42M1 12h2M21 12h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42"/>
                </svg>
            </button>
            <div class="wd-theme-switcher__menu" id="wd-theme-menu" hidden>
                <button type="button" class="wd-theme-option" data-theme="blue">🔵 أزرق</button>
                <button type="button" class="wd-theme-option" data-theme="emerald">🟢 زمردي</button>
                <button type="button" class="wd-theme-option" data-theme="midnight">⚫ داكن</button>
            </div>
        </div>
        @if (isAdmin) {
            <a class="wd-btn wd-btn--on-dark wd-btn--sm" href="/admin-secure-panel/Logout">تسجيل الخروج</a>
        } else {
            <button type="button" class="wd-btn wd-btn--on-dark wd-btn--sm" id="wd-refresh-btn" onclick="wdRefreshAll()">
                <span aria-hidden="true">⟳</span> تحديث
            </button>
        }
    </div>
</header>
```

### 3.2 تعديل `_DashboardLayout.cshtml`

**قبل السطر 27** (أو مكان الهيدر الحالي)، أضف:
```html
@{
    ViewData["BrandHref"] = "/";
    ViewData["BrandText"] = "Warehouse Dashboard · لوحة المعلومات";
    ViewData["IsAdmin"] = false;
    ViewData["ShowConnectionStatus"] = true;
}
@await Html.PartialAsync("_Header")
```

واحذف الهيدر القديم بالكامل (السطور 27-59).

### 3.3 تعديل `_CardsLayout.cshtml`

**قبل السطر 263** (أو مكان الهيدر الحالي)، أضف:
```html
@{
    ViewData["BrandHref"] = "/admin-secure-panel";
    ViewData["BrandText"] = "Warehouse Dashboard · لوحة الإدارة";
    ViewData["IsAdmin"] = true;
    ViewData["ShowConnectionStatus"] = true;
}
@await Html.PartialAsync("_Header")
```

واحذف الهيدر القديم بالكامل (السطور 263-289).

---

## 4. معايير القبول

| # | المعيار |
|---|---|
| AC-1 | `_Header.cshtml` موجود في `Pages/Shared/` |
| AC-2 | Dashboard brand link يشير إلى `/` |
| AC-3 | Admin brand link يشير إلى `/admin-secure-panel` |
| AC-4 | Admin layout يظهر Connection Status Indicator |
| AC-5 | `aria-label` دقيق: "الرئيسية" للـ Dashboard، "العودة للوحة المعلومات" للـ Admin |
| AC-6 | زر التحديث يظهر في Dashboard فقط |
| AC-7 | زر تسجيل الخروج يظهر في Admin فقط |
| AC-8 | `dotnet build` ناجح بدون أخطاء |
| AC-9 | لا تراجع على الميزات الأخرى (Theme Switcher, Sync Status, Toast, Shortcuts HUD) |

---

## 5. Pre-Execution Gate

| Check | Result |
|---|---|
| أصغر وحدة آمنة | ✅ Header partial + تعديل 2 Layouts |
| لا تغيير Backend | ✅ |
| لا تغيير Auth | ✅ |
| Allowed Write Targets ضيقة | ✅ 3 ملفات |
| معايير القبول قابلة للاختبار | ✅ |

**Gate Status:** ✅ PASS
