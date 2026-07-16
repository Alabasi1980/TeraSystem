# TASK-ENH-ADMIN-001 — Admin Nav Index Polish

> **Status:** Assigned
> **Agent:** ui-designer
> **Created:** 2026-07-15
> **Type:** UI Enhancement (Icon replacement + polish)

---

## Objective

Polish the Admin Navigation page by replacing emoji icons with SVG icons and adding minor visual touch-ups.

## Current State

### File: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml`

The page has 7 nav cards showing emoji icons:
- 📊 → إدارة البطاقات
- 🔍 → مختبر الاستعلامات
- 🔗 → تكوين التنقل العميق
- 📋 → شاشة المزامنة
- ⚙️ → إعدادات المزامنة
- 📄 → سجلات المزامنة
- 📋 → تعيينات الجداول

## Requirements

### 1. SVG Icon Replacement
Replace each emoji with a matching inline SVG icon. Use the same approach as the Card Builder (`Cards/Builder.cshtml`) — inline SVG symbols or direct SVG elements.

Suggested icon mapping:
| Emoji | Section | Suggested SVG |
|-------|---------|---------------|
| 📊 | إدارة البطاقات | Grid/layout icon (4 squares) |
| 🔍 | مختبر الاستعلامات | Search/database icon |
| 🔗 | تكوين التنقل العميق | Hierarchy/tree icon |
| 📋 | شاشة المزامنة | Sync/refresh icon |
| ⚙️ | إعدادات المزامنة | Settings/gear icon |
| 📄 | سجلات المزامنة | Document/list icon |
| 📋 | تعيينات الجداول | Table/columns icon |

### 2. Minor Visual Polish
- Increase icon container size slightly to accommodate SVGs nicely (56px → 52px is fine)
- Add a subtle gradient or color to each icon background (like Sync page's summary cards)
- Improve the card description text styling
- Keep the arrow indicator on the right

### 3. Consistency
- SVGs must use `currentColor` so they inherit the icon container color
- Use Blue Identity palette colors
- Keep existing animations (wdFadeUp with staggered delays)
- Keep responsive grid (3 → 2 → 1 columns)

## Acceptance Criteria

- [ ] All 7 emoji replaced with matching SVG icons
- [ ] SVGs use currentColor and proper viewBox
- [ ] Icons are crisp and professional
- [ ] Card hover effects preserved
- [ ] Responsive layout unchanged
- [ ] Build passes

## Allowed Write Targets

- `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web\Pages\admin-secure-panel\Index.cshtml`

## Build Command

```powershell
dotnet build --configuration Release
```
From: `D:\Teranoo Foundation\TeraSystem\TeraSystem-master\clients\CLIENT-MAJED-WAREHOUSE\applications\APP-WarehouseDashboard\src\WarehouseDashboard.Web`

Return final file and build result.
