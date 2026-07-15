# TASK-COD-FIX-011: Fix Login Password Bug (SecureString → string)

> **Phase 6 Fix | اليوم 5**
> **الحالة:** ✅ Approved

---

## 1. المشكلة
- كلمة المرور لا تُحفظ في قاعدة البيانات (Users: 0 في الـ DB)
- `PasswordBox.SecurePassword` ليست Dependency Property → binding يفشل
- `AuthService` Singleton + `DbContext` Scoped = DI anti-pattern
- مسار DB نسبي (يختلف حسب طريقة التشغيل)

## 2. الحلول

### أ. LoginView.xaml — استبدال SecurePassword بـ Password
```xml
CommandParameter="{Binding ElementName=PasswordInput, Path=Password}"
```

### ب. LoginViewModel.cs — إزالة ConvertSecureStringToString، استقبال string مباشر
```csharp
[RelayCommand]
public async Task LoginAsync(string? password)
{
    if (string.IsNullOrEmpty(password)) { ... }
    ...
}

[RelayCommand]
public async Task SetPasswordAsync(string? password)
{
    if (string.IsNullOrEmpty(password)) { ... }
    ...
}
```

### ج. AuthService.cs — تغيير DI من Singleton إلى Scoped
```csharp
services.AddScoped<IAuthService, AuthService>();
```

### د. مسار DB مطلق في App.xaml.cs
```csharp
var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TeraQuotation.db");
options.UseSqlite($"Data Source={dbPath}");
```

## 3. Allowed Write Targets
```
{DIR}\Views\LoginView.xaml
{DIR}\ViewModels\LoginViewModel.cs
{DIR}\Services\AuthService.cs
{DIR}\App.xaml.cs
```
`{DIR}` = المسار الكامل

## 4. Acceptance
- ✅ dotnet build — 0 Errors
- ✅ أول تشغيل → تعيين كلمة مرور → حفظ = 1 User في DB
- ✅ نفس الجلسة → تسجيل دخول بنفس الكلمة → يعمل
- ✅ إغلاق التطبيق → فتحه ثاني → تسجيل دخول → يعمل

## 5. Task Log
| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved |
| 2026-07-13 | ✅ EngineeringAgent — 3 ملفات: SecurePassword→Password, Singleton→Scoped, مسار مطلق, 0 Errors |
| 2026-07-13 | 🟢 PASS |
