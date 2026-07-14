# TASK-COD-FIX-010: زر "تغيير كلمة المرور" في Settings

> **Phase 6 Fix | اليوم 5**
> **الحالة:** ✅ Approved

---

## 1. الوصف
إضافة زر في Settings لتغيير كلمة المرور الحالية (كلمة قديمة + كلمة جديدة + تأكيد).

## 2. المخرجات
- [ ] `Views/ChangePasswordDialog.xaml` + `.cs` — نافذة تغيير كلمة المرور
- [ ] `ViewModels/ChangePasswordViewModel.cs`
- [ ] تحديث Settings لإضافة الزر

## 3. Allowed Write Targets
```
{DIR}\Views\ChangePasswordDialog.xaml
{DIR}\Views\ChangePasswordDialog.xaml.cs
{DIR}\ViewModels\ChangePasswordViewModel.cs
{DIR}\Views\SettingsView.xaml
{DIR}\ViewModels\SettingsViewModel.cs
{DIR}\App.xaml.cs
```
`{DIR}` = المسار الكامل

## 4. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ زر "تغيير كلمة المرور" في Settings
- ✅ تحقق من كلمة القديمة صح قبل التغيير
- ✅ تأكيد كلمة جديدة (أقل شيء 4 أحرف)
- ✅ بعد التغيير → رسالة نجاح

## 5. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — نافذة تغيير كلمة المرور كاملة (Validation + BCrypt + States) |
| 2026-07-13 | 🟢 PASS |
