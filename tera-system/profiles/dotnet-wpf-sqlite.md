# Technology Profile: dotnet-wpf-sqlite

> **✅ معتمد من Majed — جاهز للاستخدام في التنفيذ**

## 1. Profile Identity

- Profile ID: `dotnet-wpf-sqlite`
- Language: **C# (.NET 8)**
- Framework: **WPF (Windows Presentation Foundation)** مع **CommunityToolkit.Mvvm**
- Database: **SQLite**
- ORM: **Entity Framework Core** (موصى به) أو **Microsoft.Data.Sqlite** (يدوي)
- Package Manager / CLI: `dotnet CLI` + `NuGet`
- Default Project Type: **WPF Application** (`dotnet new wpf`)
- Template: `dotnet new wpf -n <ProjectName>`

## 2. Applicability

يُستخدم هذا البروفايل عندما يكون المشروع:
- تطبيق **Windows Desktop** (WPF)
- يعمل **محلياً** (لا سيرفر، لا API)
- **مستخدم واحد** (أو عدد محدود بصلاحيات بسيطة)
- مع **SQLite** كقاعدة بيانات
- مع أو بدون تكامل Outlook

لا يُستخدم لـ:
- تطبيقات Web
- تطبيقات Mobile
- تطبيقات متعددة المستخدمين تحتاج سيرفر

## 3. Default Execution Order

1. Scaffold (إنشاء حل WPF + هيكل المشروع)
2. إعداد SQLite + Data Layer (Entities + Context + Migrations)
3. تطبيق طبقة MVVM (Models ← ViewModels ← Services ← Views)
4. شاشة Login (S1)
5. شاشة Settings (S2) — الموردين + القطع + التوقيعات + الترويسة
6. شاشة Quotation Form (S3) — جوهر التطبيق
7. شاشة Quotation List (S4)
8. شاشة Reports (S5) — التقارير الأربعة
9. طباعة + PDF
10. Outlook Integration (اختياري)
11. Auto Backup
12. اختبار + تسليم

## 4. First Task Rule

أول مهمة تنفيذ آمنة:

```text
TASK-COD-001: Scaffold WPF Project + SQLite Setup
```

يشمل:
- `dotnet new wpf` مع هيكل MVVM (Models, ViewModels, Views, Services, Data)
- إضافة حزمة `CommunityToolkit.Mvvm` و `Microsoft.EntityFrameworkCore.Sqlite`
- إنشاء ApplicationDbContext + Seed (إن وجد)
- تطبيق أول Migration
- التأكد من أن التطبيق يشتغل (Run دون أخطاء)

## 5. Scaffold Rules

```text
# إنشاء مشروع WPF
dotnet new wpf -n TeraQuotation -o .\src\TeraQuotation

# إضافة الحزم الأساسية
dotnet add .\src\TeraQuotation\TeraQuotation.csproj package CommunityToolkit.Mvvm
dotnet add .\src\TeraQuotation\TeraQuotation.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add .\src\TeraQuotation\TeraQuotation.csproj package QuestPDF  # للـ PDF
dotnet add .\src\TeraQuotation\TeraQuotation.csproj package Microsoft.Office.Interop.Outlook

# إنشاء مجلدات الهيكل
New-Item -ItemType Directory -Path .\src\TeraQuotation\Models, .\src\TeraQuotation\ViewModels, .\src\TeraQuotation\Views, .\src\TeraQuotation\Services, .\src\TeraQuotation\Data
```

**قيود الـ Scaffold:**
- لا تنشئ `Views/` افتراضية غير Shutdown
- لا تعدل `App.xaml` لتغيير StartupUri قبل تجهيز MainWindow
- لا تنشئ واجهات Login قبل تجهيز منطق المصادقة
- لا تنشئ أكثر من مشروع واحد في الحل إلا إذا تطلب الأمر

## 6. ORM / Database Rules

- **حدود الـ EF Core Migrations:**
  - `Add-Migration <name>` لإنشاء migration
  - `Update-Database` لتطبيقها
  - لا تحذف migration موجودة
  - لا تعدل migration يدوياً إلا لتصحيح خطأ محدد
  
- **Schema:**
  - كل Entity يكون في `Models/`
  - DbSet Properties في `Data/AppDbContext.cs`
  - ConnectionString: `Data Source=TeraQuotation.db` (ملف محلي)

- **Database-side Validation:**
  - استخدام Data Annotations (`[Required]`, `[MaxLength]`, إلخ)
  - استخدام Fluent API في `OnModelCreating` للعلاقات

- **SQLite Limits:**
  - لا تستخدم Stored Procedures (غير مدعومة)
  - لا تستخدم Types خاصة بـ SQL Server
  - استخدم `HasColumnType` لضبط الأنواع عند الحاجة

## 7. CLI Side Effects

| الأمر | التأثير الجانبي |
|:------|:----------------|
| `dotnet new wpf` | ينشئ ملف `.csproj` + `App.xaml` + `MainWindow.xaml` |
| `dotnet add package ...` | يعدّل `.csproj` (إضافة PackageReference) |
| `dotnet build` | لا تأثير جانبي — آمن |
| `dotnet run` | لا تأثير جانبي — آمن |
| `Add-Migration` | ينشئ ملف `.cs` في `Migrations/` |
| `Update-Database` | ينشئ/يعدّل ملف `SQLite.db` في مجلد المشروع |
| `dotnet publish` | ينشئ مجلد `publish/` بالمخرجات القابلة للتوزيع |

## 8. Forbidden Defaults

ما **يمنع** إنشاؤه افتراضياً بدون طلب صريح:

- ❌ **لا** Views إضافية (غير الـ 5 شاشات المطلوبة)
- ❌ **لا** Admin Panel/Panel إضافي
- ❌ **لا** صفحات Settings إضافية
- ❌ **لا** شاشات Reporting إضافية
- ❌ **لا** إشعارات النظام
- ❌ **لا** خلفية سيرفر DB (SQL Server, PostgreSQL)
- ❌ **لا** Docker
- ❌ **لا** Unit Tests (للمشاريع الصغيرة جداً — يُضاف إذا طُلب)
- ❌ **لا** API endpoints
- ❌ **لا** SignalR أو WebSockets

## 9. Pre-Execution Gate Additions

بالإضافة إلى `TeraPreExecutionGate.md`، تأكد من:

- [ ] ملف `.csproj` لا يحتوي PackageReferences غير ضرورية
- [ ] WPF لا يستخدم `App.xaml` لتوجيه غير آمن
- [ ] NuGet packages كلها متوافقة مع .NET 8
- [ ] ConnectionString لـ SQLite لا تحتوي معلومات حساسة
- [ ] مجلد `Migrations/` موجود بعد أول مهمة DB
- [ ] دعم RTL ضمن XAML (FlowDirection)

## 10. Acceptance Criteria Patterns

يقبل الـ Handback إذا:

- ✅ التطبيق يشتغل بـ `dotnet run` بدون أخطاء
- ✅ الشاشات تعرض بالعربية (RTL)
- ✅ SQLite يعمل (قراءة/كتابة/تصدير)
- ✅ الطباعة تنتج وثيقة A4 صالحة
- ✅ الـ PDF يُفتح بدون أخطاء
- ✅ Outlook Integration تعمل إذا Outlook مثبت، وإلا Fallback إلى PDF
- ✅ البحث والتصفية يعملان في الجداول
- ✅ التطبيق لا يتعطل عند إدخال بيانات ناقصة

---

**إعداد:** TeraAgent (مسودة)
**تاريخ:** 2026-07-13
**الحالة:** ✅ **معتمد من Majed** — 2026-07-13
