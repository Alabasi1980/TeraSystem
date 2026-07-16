# TASK-ENH-LOGOUT-001 — Logout Page Redesign

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement
> **Scope:** Visual redesign + minor backend change

---

## Objective

Redesign the Logout page from a bare 7-line placeholder to a beautiful farewell screen that shows briefly before redirecting to the Login page.

## Current State

### File: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml`

```cshtml
@page
@model WarehouseDashboard.Web.Pages.AdminSecurePanel.LogoutModel
@{
    ViewData["Title"] = "تسجيل الخروج";
    Layout = "_Layout";
}
<p>جارٍ تسجيل الخروج...</p>
```

### File: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml.cs`

```csharp
public IActionResult OnGet()
{
    HttpContext.Session.Remove(SessionKey);
    HttpContext.Session.Clear();
    return RedirectToPage("/admin-secure-panel/Login");
}
```

**Problem:** The server returns a 302 redirect immediately, so the user NEVER sees any logout page. They go straight from clicking "تسجيل الخروج" to the Login page with no visual feedback.

## Requirements

### 1. Backend Change (simple)
- Change `return RedirectToPage("/admin-secure-panel/Login")` to `return Page()` in OnGet()
- This allows the page to render instead of immediately redirecting

### 2. Beautiful Logout Page Design
Create a self-contained (Layout = null) farewell page that:

**Design Concept:** Clean, centered farewell card with:
- Large checkmark or door/exit icon (SVG, using Blue Identity palette)
- "تم تسجيل الخروج بنجاح" as main heading
- "شكراً لاستخدام Warehouse Dashboard" as subtitle
- A subtle animated spinner or loading indicator
- "جاري تحويلك إلى صفحة الدخول..." text
- Auto-redirect to Login page after 2.5 seconds via `<meta http-equiv="refresh">` or JS

**Visual Style:**
- Centered card on subtle background
- Uses Blue Identity palette (same tokens as Login page)
- Smooth fade-in animation for the card
- Responsive (works on mobile)
- The card should feel like a natural "end" of a session — calm, professional, reassuring

### 3. Auto-Redirect
- After 2.5 seconds, redirect to `/admin-secure-panel/Login`
- Show a manual link "العودة إلى تسجيل الدخول" in case the redirect doesn't fire
- Use `<meta http-equiv="refresh" content="2.5;url=/admin-secure-panel/Login">` for reliability

## Acceptance Criteria

- [ ] Self-contained page (Layout = null) with full HTML structure
- [ ] Beautiful farewell design with large icon
- [ ] 2.5 second auto-redirect to Login page
- [ ] Manual link as fallback
- [ ] Arabic RTL correct
- [ ] Responsive
- [ ] Smooth entrance animation
- [ ] Build passes

## Allowed Write Targets (ABSOLUTE PATHS)

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml`
- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Logout.cshtml.cs`

## Build Command

```powershell
dotnet build --configuration Release
```
Run from: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return the final file contents, build result, and a brief description of the design.
