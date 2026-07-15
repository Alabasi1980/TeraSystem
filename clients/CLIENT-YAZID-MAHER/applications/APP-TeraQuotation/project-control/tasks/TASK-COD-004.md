# TASK-COD-004: Settings Screen (S2) — 4 Tabs + CRUD كامل

> **Batch 2 — Settings Module | اليوم 2**
> **الحالة:** ✅ Approved (جاهز للتفويض)

---

## 1. الوصف

تنفيذ شاشة الإعدادات (Settings) بالكامل — شريط تبويب (TabControl) بأربعة أقسام: إدارة الموردين، إدارة القطع، إدارة التوقيعات، إعدادات الترويسة.

## 2. المخرجات

- [ ] `Views/SettingsView.xaml` + `.cs` — شاشة الإعدادات
- [ ] `ViewModels/SettingsViewModel.cs` — ViewModel مع 4 أقسام
- [ ] تنفيذ حقيقي لـ `SettingsService` (بدلاً من stub)
- [ ] تحويل `Views/SettingsView` placeholder إلى الشاشة الكاملة

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Views\SettingsView.xaml
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Views\SettingsView.xaml.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\ViewModels\SettingsViewModel.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Services\SettingsService.cs
```

## 4. Acceptance Criteria

- ✅ `dotnet build` — 0 Errors
- ✅ TabControl بـ 4 Tabs: الموردين، القطع، التوقيعات، الترويسة
- ✅ كل Tab يحتوي DataGrid مع أزرار إضافة/تعديل/حذف
- ✅ الموردين: Name, ContactInfo, Notes
- ✅ القطع: Name, Description, Notes + Search
- ✅ التوقيعات: Name + OrderIndex (قائمة مرتبة)
- ✅ الترويسة: Logo (اختياري), CompanyName, Address, Phone, Email
- ✅ RTL بالكامل
- ✅ كل التغييرات تُحفظ فوراً أو عبر زر حفظ

## 5. مصادر التصميم

- `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md` §3 (S2)
- `project-preparation/05_BUSINESS_WORKFLOWS.md` (WF-06, WF-07, WF-08)
- `project-preparation/08_TECHNICAL_ARCHITECTURE.md`

## 6. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved — جاهز للتفويض |
| 2026-07-13 | ✅ Delegated to EngineeringAgent |
| 2026-07-13 | ✅ Handback — 4 Tabs CRUD كامل, 0 Errors ✅ |
| 2026-07-13 | 🟢 Post-Execution Review: PASS |

