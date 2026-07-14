# TASK-COD-003: Login Screen (S1) + Password Management

> **Batch 1 — Foundation | اليوم 1**
> **الحالة:** ✅ Approved (جاهز للتفويض)

---

## 1. الوصف

تنفيذ شاشة الدخول (S1) بالكامل — أول شاشة تظهر عند تشغيل التطبيق. تشمل التحقق من كلمة المرور، أول تسجيل (First Time Setup)، رسائل الخطأ، والتنقل إلى الشاشة الرئيسية بعد النجاح.

## 2. المخرجات المطلوبة

- [ ] `Views/LoginView.xaml` + `Views/LoginView.xaml.cs` — واجهة تسجيل الدخول
- [ ] `ViewModels/LoginViewModel.cs` — منطق تسجيل الدخول (MVVM)
- [ ] تعديل `App.xaml.cs` — فتح شاشة Login أولاً بدلاً من MainWindow
- [ ] تحديث `MainWindow.xaml` — إضافة Frame للتنقل بين الشاشات
- [ ] `Services/IAuthService.cs` + `Services/AuthService.cs` — خدمة المصادقة (تجزئة + تحقق)
- [ ] تحويل `Services/NavigationService.cs` إلى تنفيذ حقيقي كامل

## 3. Allowed Write Targets

```
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Views\LoginView.xaml
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Views\LoginView.xaml.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\ViewModels\LoginViewModel.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Services\IAuthService.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\Services\AuthService.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\App.xaml.cs
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\MainWindow.xaml
D:\Teranoo Foundation\Customer 01 - Noor\TeraSystem\clients\CLIENT-YAZID-MAHER\applications\APP-TeraQuotation\source\TeraQuotation\MainWindow.xaml.cs
```

## 4. Acceptance Criteria

- ✅ `dotnet build` — 0 Errors
- ✅ التطبيق يعرض شاشة Login أولاً عند التشغيل
- ✅ عند أول تشغيل (لا يوجد مستخدم): يطلب إنشاء كلمة مرور جديدة
- ✅ عند التشغيل الطبيعي: يطلب إدخال كلمة المرور
- ✅ كلمة مرور خاطئة → رسالة خطأ
- ✅ كلمة مرور صحيحة → تنقل إلى التطبيق الرئيسي
- ✅ كلمة المرور مخزنة بشكل آمن (BCrypt أو SHA256 مع Salt)
- ✅ الـ PasswordBox يستخدم خاصية bind آمنة (لا تخزين النص في ViewModel)

## 5. مصادر التصميم

- `project-preparation/07_SCREENS_AND_UI_STRUCTURE.md` — تفاصيل شاشة S1
- `project-preparation/08_TECHNICAL_ARCHITECTURE.md` — القرارات المعمارية
- `project-preparation/05_BUSINESS_WORKFLOWS.md` — WF-01: تسجيل الدخول والإعداد الأولي

## 6. مواصفات الشاشة

### S1 — Login Screen Layout

```
┌─────────────────────────────┐
│       [Logo/Title]          │
│       TeraQuotation         │
│                             │
│    ┌───────────────────┐    │
│    │   كلمة المرور       │    │
│    │  [______________]  │    │
│    └───────────────────┘    │
│                             │
│    ┌───────────────────┐    │
│    │      دخول          │    │
│    └───────────────────┘    │
│                             │
│    [رسالة الخطأ هنا]        │
└─────────────────────────────┘
```

### الحالات:
1. **FirstTimeSetup:** لا يوجد مستخدم في قاعدة البيانات → يظهر حقل إنشاء كلمة مرور جديدة + زر "تعيين كلمة المرور"
2. **Normal:** يوجد مستخدم → يظهر حقل إدخال كلمة مرور + زر "دخول"
3. **Error:** كلمة مرور خاطئة → رسالة خطأ حمراء أسفل الحقل
4. **Success:** كلمة مرور صحيحة → تنقل إلى MainWindow مع Frame للتطبيق

## 7. تفاصيل التنفيذ

### IAuthService / AuthService
```csharp
public interface IAuthService
{
    Task<bool> IsFirstTimeAsync();           // هل يوجد مستخدم؟
    Task<(bool Success, string Error)> SetPasswordAsync(string password);  // أول تسجيل
    Task<(bool Success, string Error)> ValidatePasswordAsync(string password);  // تحقق
}
```

استخدم BCrypt (NuGet: `BCrypt.Net-Next`) لتجزئة كلمة المرور.
إذا تعذرت إضافة الحزمة، استخدم SHA256 مع Salt يدوي.

### LoginViewModel
```csharp
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string? _password;  // لا تُستخدم مباشرة — عبر PasswordBox
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isFirstTime;
    [ObservableProperty] private bool _isLoading;
    
    // Commands
    [RelayCommand] async Task LoginAsync();       // تحقق من كلمة المرور
    [RelayCommand] async Task SetPasswordAsync(); // أول تسجيل
}
```

ملاحظة: WPF PasswordBox لا يدربط (bind) النص مباشرة لأسباب أمنية. استخدم أحد:
- `PasswordBoxAssistant` ( attached property behaviour)
- أو مناداة command مع تمرير `PasswordBox.SecurePassword` عبر parameter

### App.xaml.cs — Startup Logic
- عند بدء التشغيل:
  1. Apply Migrations (موجود)
  2. فتح شاشة Login في نافذة صغيرة (أو داخل MainWindow Frame)
  3. بعد نجاح Login → التنقل إلى الشاشة الرئيسية

### MainWindow.xaml
- يحتوي على `Frame x:Name="MainFrame"` للتنقل الداخلي
- شاشة Login تُعرض أولاً داخل الـ Frame
- بعد النجاح، التنقل إلى Settings أو شاشة ترحيبية

## 8. Vitality & Polish Checklist

| البند | الحالة |
|:------|:------:|
| Skeleton Loading / Shimmer | N/A — شاشة بسيطة بحقل واحد |
| Toast Notifications | ✅ رسالة خطأ في الواجهة (Error TextBlock) |
| Connection Status Indicator | N/A — تطبيق محلي |
| Search | N/A — شاشة دخول |
| Micro-animations | N/A — واجهة بسيطة |
| Empty States | ✅ حالة FirstTimeSetup (لا يوجد مستخدم) |
| Realistic Data | N/A — شاشة دخول |

## 9. Pre-Execution Gate Result

| # | السؤال | النتيجة |
|:-:|:-------|:-------:|
| 1 | مرتبطة بخطة معتمدة؟ | ✅ PROJECT_MASTER_PLAN.md |
| 2 | أصغر وحدة تنفيذية؟ | ✅ شاشة واحدة فقط |
| 3-22 | جميع البنود | ✅ PASS |
| **النتيجة** | | **🟢 PASS** |

## 10. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved — جاهز للتفويض |
| 2026-07-13 | ✅ Delegated to EngineeringAgent |
| 2026-07-13 | ✅ Handback — LoginScreen يعمل، 0 Build Errors |
| 2026-07-13 | 🟢 **Post-Execution Review: PASS** |
