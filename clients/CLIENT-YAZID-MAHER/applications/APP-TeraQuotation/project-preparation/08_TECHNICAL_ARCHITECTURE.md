# 08_TECHNICAL_ARCHITECTURE.md — TeraQuotation

> **Technical Architecture Document**
> **المشروع:** TeraQuotation — نظام إدارة عروض أسعار قطع السيارات
> **التقنية:** WPF (.NET 8) + SQLite + CommunityToolkit.Mvvm
> **الحجم:** Small (5 شاشات، 19 ميزة، مستخدم واحد)
> **تاريخ الإصدار:** 2026-07-13
> **المصدر المعتمد:** APPLICATION_BLUEPRINT.md ✅ + TERA_PROJECT_DECISION.md ✅ + Technology Profile `dotnet-wpf-sqlite` ✅

---

## Lifecycle Header

| الحقل | القيمة |
|:------|:-------|
| **Document State** | **Module Baseline Approved** |
| **Baseline Module** | TeraQuotation (Full Application) |
| **Current State** | Module Baseline Approved |
| **Owner** | Software Designer Agent |
| **Last Review** | 2026-07-13 |
| **Expiry** | End of Project Delivery |

---

## قائمة المحتويات

1. [هيكل المشروع (Solution Structure)](#1-هيكل-المشروع-solution-structure)
2. [طبقات التطبيق (Layers)](#2-طبقات-التطبيق-layers)
3. [تدفق البيانات (Data Flow)](#3-تدفق-البيانات-data-flow)
4. [مخطط الفئات (Class Diagram)](#4-مخطط-الفئات-class-diagram)
5. [خدمات التطبيق (Application Services)](#5-خدمات-التطبيق-application-services)
6. [استراتيجية التوجيه (Navigation)](#6-استراتيجية-التوجيه-navigation)
7. [إدارة الأخطاء والتسجيل (Error Handling & Logging)](#7-إدارة-الأخطاء-والتسجيل-error-handling--logging)
8. [الاعتبارات الأمنية (Security)](#8-الاعتبارات-الأمنية-security)
9. [اعتبارات الأداء (Performance)](#9-اعتبارات-الأداء-performance)
10. [اعتبارات النشر (Deployment)](#10-اعتبارات-النشر-deployment)
11. [قرارات معمارية مسجلة (Architecture Decision Records)](#11-قرارات-معمارية-مسجلة-architecture-decision-records)
12. [التبعيات والافتراضات](#12-التبعيات-والافتراضات)

---

## 1. هيكل المشروع (Solution Structure)

### 1.1 هيكل المجلدات

```
TeraQuotation/
├── TeraQuotation.sln
│
└── src/
    └── TeraQuotation/
        ├── TeraQuotation.csproj
        ├── App.xaml / App.xaml.cs           # نقطة بدء التطبيق + تسجيل الخدمات
        ├── MainWindow.xaml / .cs            # Shell (نافذة رئيسية) مع Frame للمحتوى
        │
        ├── Models/                          # كيانات قاعدة البيانات + View Models
        │   ├── User.cs
        │   ├── Supplier.cs
        │   ├── Item.cs
        │   ├── Quotation.cs
        │   ├── QuotationItem.cs
        │   ├── Signature.cs
        │   ├── Setting.cs                   # Key-Value للتخزين العام
        │   └── AuditLog.cs                  # (اختياري) سجل العمليات
        │
        ├── ViewModels/                      # MVVM ViewModels لكل شاشة
        │   ├── LoginViewModel.cs
        │   ├── MainViewModel.cs             # ViewModel للنافذة الرئيسية (Shell)
        │   ├── SettingsViewModel.cs
        │   ├── QuotationFormViewModel.cs
        │   ├── QuotationListViewModel.cs
        │   ├── ReportsViewModel.cs
        │   └── BaseViewModel.cs             # أساسي لـ ObservableObject
        │
        ├── Views/                           # WPF Windows / UserControls
        │   ├── LoginView.xaml / .cs
        │   ├── MainWindowView.xaml / .cs    # النافذة الرئيسية (Shell)
        │   ├── SettingsView.xaml / .cs      # حاوية للـ 4 Tabs
        │   ├── SuppliersTab.xaml / .cs
        │   ├── ItemsTab.xaml / .cs
        │   ├── SignaturesTab.xaml / .cs
        │   ├── LetterheadTab.xaml / .cs
        │   ├── QuotationFormView.xaml / .cs
        │   ├── QuotationListView.xaml / .cs
        │   └── ReportsView.xaml / .cs
        │
        ├── Services/                        # طبقة الخدمات (Business Logic)
        │   ├── IAuthenticationService.cs
        │   ├── AuthenticationService.cs
        │   ├── IQuotationService.cs
        │   ├── QuotationService.cs
        │   ├── ISettingsService.cs
        │   ├── SettingsService.cs
        │   ├── IReportService.cs
        │   ├── ReportService.cs
        │   ├── IPdfService.cs
        │   ├── PdfService.cs
        │   ├── IPrintService.cs
        │   ├── PrintService.cs
        │   ├── IOutlookService.cs
        │   ├── OutlookService.cs
        │   ├── IBackupService.cs
        │   ├── BackupService.cs
        │   ├── INavigationService.cs
        │   ├── NavigationService.cs
        │   └── ILoggingService.cs
        │       LoggingService.cs
        │
        ├── Data/                            # EF Core + SQLite
        │   ├── AppDbContext.cs
        │   └── Migrations/                  # EF Core Migrations (تُنشأ تلقائياً)
        │
        ├── Converters/                      # WPF Value Converters
        │   ├── BoolToVisibilityConverter.cs
        │   ├── StatusToColorConverter.cs
        │   ├── PriceFormatConverter.cs
        │   └── InverseBoolConverter.cs
        │
        ├── Helpers/                         # Utilities
        │   ├── PasswordHelper.cs            # Hashing (SHA256/BCrypt)
        │   ├── QuoteNumberGenerator.cs
        │   ├── Constants.cs                 # أسماء Routes, Keys, Messages
        │   └── RTLHelper.cs
        │
        ├── Styles/                          # WPF Resources / Themes
        │   └── Theme.xaml                   # الألوان، الخطوط، Styles العامة
        │
        └── Templates/                       # XAML DataTemplates
            └── DataTemplates.xaml
```

### 1.2 قواعد الهيكل

| القاعدة | الشرح |
|:--------|:------|
| **مشروع واحد** | لا `Class Library` منفصلة — مشروع WPF واحد يضم كل الطبقات |
| **فصل المسؤوليات** | Models (بيانات) ← Services (منطق) ← ViewModels (حالة) ← Views (عرض) ← Data (وصول للبيانات) |
| **واجهات للخدمات** | كل خدمة لها Interface (قابلية اختبار + استبدال) |
| **BaseViewModel** | يرث من `ObservableObject` (CommunityToolkit.Mvvm) |
| **لا Repositories** | استخدم `AppDbContext` مباشرة في Services — المشروع صغير ولا يحتاج طبقة إضافية |

---

## 2. طبقات التطبيق (Layers)

### 2.1 رسم بياني للطبقات

```
┌──────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                 │
│              Views (XAML + Code-behind)               │
│  ┌──────────┐  ┌──────────┐  ┌──────┐  ┌──────────┐ │
│  │ LoginView│  │Settings  │  │Quote │  │ Reports  │ │
│  │          │  │View (Tab)│  │Form  │  │ View     │ │
│  └────┬─────┘  └────┬─────┘  └──┬───┘  └────┬─────┘ │
│       │              │           │            │       │
│       └──────────────┴───────────┴────────────┘       │
│                         │  Binding (DataContext)       │
├─────────────────────────┼──────────────────────────────┤
│                 VIEWMODEL LAYER                        │
│  ┌──────────────────────────────────────────────────┐ │
│  │  BaseViewModel (ObservableObject)                │ │
│  │  ┌──────────┐ ┌──────────┐ ┌──────┐ ┌────────┐ │ │
│  │  │LoginVM   │ │SettingsVM│ │Quote │ │Reports │ │ │
│  │  │          │ │          │ │FormVM│ │VM      │ │ │
│  │  └────┬─────┘ └────┬─────┘ └──┬───┘ └───┬────┘ │ │
│  └───────┼─────────────┼──────────┼──────────┼──────┘ │
│          │             │          │          │         │
│          └─────────────┴──────────┴──────────┘         │
│                         │  استدعاء Service              │
├─────────────────────────┼──────────────────────────────┤
│                  SERVICE LAYER                          │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────┐ │
│  │Auth      │ │Quotation │ │Settings  │ │Report     │ │
│  │Service   │ │Service   │ │Service   │ │Service    │ │
│  ├──────────┤ ├──────────┤ ├──────────┤ ├───────────┤ │
│  │PDF       │ │Print     │ │Outlook   │ │Backup     │ │
│  │Service   │ │Service   │ │Service   │ │Service    │ │
│  └──────────┘ └──────────┘ └──────────┘ └───────────┘ │
│                         │  استخدام DbContext           │
├─────────────────────────┼──────────────────────────────┤
│                    DATA LAYER                           │
│  ┌──────────────────────────────────────────────────┐ │
│  │  AppDbContext (EF Core)                          │ │
│  │  DbSet<User>, DbSet<Supplier>, DbSet<Item>,     │ │
│  │  DbSet<Quotation>, DbSet<QuotationItem>,        │ │
│  │  DbSet<Signature>, DbSet<Setting>               │ │
│  └──────────────────────┬───────────────────────────┘ │
│                         │                             │
│                    ┌────┴────┐                        │
│                    │ SQLite  │                        │
│                    │ Tera    │                        │
│                    │Quotation│                        │
│                    │ .db     │                        │
│                    └─────────┘                        │
└──────────────────────────────────────────────────────┘
```

### 2.2 وصف الطبقات

| الطبقة | المسؤولية | الأمثلة |
|:--------|:-----------|:---------|
| **Presentation (Views)** | عرض واجهة المستخدم، التقاط الأحداث، لا منطق تجاري | XAML مع DataBinding، Converters |
| **ViewModel** | حالة واجهة المستخدم، أوامر (Commands)، تحضير البيانات للعرض | Observable Properties, RelayCommand |
| **Services** | منطق الأعمال، الوصول إلى قاعدة البيانات، توليد PDF، طباعة، Outlook | QuotationService, PdfService |
| **Data** | EF Core DbContext، Migrations، اتصال SQLite | AppDbContext.cs، Migrations/ |

### 2.3 قواعد التفاعل بين الطبقات

```
View → ViewModel:   DataBinding + Commands (ربط ثنائي الاتجاه)
ViewModel → Service: استدعاء async/await (Task-based)
Service → Data:     استخدام AppDbContext (DI عبر Constructor)
Service ← ViewModel: إرجاع Results (كائنات أو قوائم)
ViewModel ← View:   Notify (خاصية) + تحديث واجهة تلقائي
```

- **يُمنع** استدعاء `AppDbContext` مباشرة من ViewModel
- **يُمنع** وضع منطق قاعدة البيانات في View
- **يُسمح** لـ ViewModel باستخدام `IDispatcher` لتحديث واجهة المستخدم من Thread غير الرئيسي

---

## 3. تدفق البيانات (Data Flow)

### 3.1 تدفق عام: من واجهة المستخدم إلى قاعدة البيانات

```
┌──────────┐     Command      ┌────────────┐     استدعاء      ┌──────────┐     EF Core     ┌──────────┐
│  View    │ ──────────────► │ ViewModel  │ ──────────────► │ Service  │ ──────────────► │ Database │
│ (XAML)   │ ◄────────────── │ (Observable│ ◄────────────── │ (Business│ ◄────────────── │ (SQLite) │
│          │   Binding       │  Object)   │   Observable    │  Logic)  │   Results       │          │
└──────────┘                 └────────────┘   Collection    └──────────┘                 └──────────┘
```

### 3.2 تدفق محدد: إنشاء عرض سعر جديد

```
1. المستخدم يضغط "عرض سعر جديد" في MainWindow
       │
2. NavigationService.SetCurrentView("QuotationForm")
       │
3. QuotationFormViewModel.OnNavigatedTo()
       │
4. استدعاء QuotationService.CreateNewQuotation()
       │
5. QuotationService:
   a. توليد رقم تسلسلي جديد (QuoteNumberGenerator)
   b. إنشاء كائن Quotation جديد (Status = Draft, Date = Today)
   c. AppDbContext.Quotations.Add(quotation)
   d. AppDbContext.SaveChanges()
   e. إرجاع QuotationDto (Id, QuoteNumber, Date)
       │
6. ViewModel يخزّن Quotation الحالي في الخاصية CurrentQuotation
       │
7. View (XAML) يعرض البيانات عبر Binding:
   - TextBlock.Text ← {Binding CurrentQuotation.QuoteNumber}
   - DatePicker.Date ← {Binding CurrentQuotation.Date}
```

### 3.3 تدفق: إضافة بند إلى عرض السعر

```
1. المستخدم يختار قطعة من ComboBox (Items catalog)
2. يملأ بيانات الموردين (3 أعمدة)
3. يضغط "إضافة بند"
       │
4. QuotationFormViewModel.AddItemCommand.Execute()
       │
5. إنشاء QuotationItem جديد:
   - ItemId ← القطعة المختارة
   - Supplier1Type / Supplier1Price / Supplier2Type / Supplier2Price / Supplier3Type / Supplier3Price
       │
6. استدعاء QuotationService.AddQuotationItem(quotationItem)
       │
7. QuotationService:
   a. AppDbContext.QuotationItems.Add(item)
   b. AppDbContext.SaveChanges()
       │
8. ViewModel:
   a. إضافة QuotationItem إلى ObservableCollection<QuotationItem>
   b. DataGrid يعرض العنصر الجديد تلقائياً
```

### 3.4 تدفق: طباعة عرض سعر

```
1. المستخدم يضغط "طباعة" (مع أو بدون أسعار)
       │
2. ViewModel: يحدد PrintMode (WithoutPrices / WithPrices)
       │
3. استدعاء PrintService.PrintQuotation(quotationId, printMode)
       │
4. PrintService:
   a. جلب Quotation + Items من AppDbContext
   b. بناء FixedDocument (WPF)
   c. ضبط الصفحة A4 (210mm × 297mm)
   d. إدراج الترويسة (شعار + بيانات الشركة)
   e. إدراج جدول البيانات (حسب printMode)
   f. إدراج التوقيعات
   g. فتح PrintDialog → المستخدم يختار الطابعة
   h. إرسال Document إلى الطابعة
       │
5. ViewModel: تحديث Status إلى "Printed"
```

### 3.5 تدفق: PDF + Outlook

```
1. المستخدم يضغط "PDF"
       │
2. استدعاء PdfService.ExportToPdf(quotationId, filePath)
       │
3. PdfService (QuestPDF):
   a. جلب بيانات Quotation + Items
   b. بناء Document باستخدام QuestPDF Fluent API
   c. حفظ إلى ملف PDF (.pdf)
       │
4. مستخدم يضغط "Outlook":
       │
5. استدعاء OutlookService.SendViaOutlook(quotationId)
       │
6. OutlookService:
   a. التحقق من وجود Outlook (try/catch)
   b. إنشاء كائن MailItem
   c. إرفاق PDF المُنشأ
   d. تعبئة الموضوع والجسم تلقائياً
   e. عرض نافذة Outlook (المستخدم يضغط إرسال يدوياً)
       │
   Fallback: إذا Outlook غير موجود → رسالة للمستخدم + فتح PDF
```

---

## 4. مخطط الفئات (Class Diagram)

### 4.1 كيانات قاعدة البيانات (Entity Classes)

```
┌─────────────────────────────┐
│           User              │
│─────────────────────────────│
│ int Id [PK]                 │
│ string Username             │
│ string PasswordHash         │
│ DateTime CreatedAt          │
│ bool IsLocked (اختياري)     │
└─────────────────────────────┘

┌─────────────────────────────┐         ┌─────────────────────────────┐
│         Supplier            │         │           Item              │
│─────────────────────────────│         │─────────────────────────────│
│ int Id [PK]                 │         │ int Id [PK]                 │
│ string Name                 │         │ string Name                 │
│ string? ContactInfo         │         │ string? Description         │
│ string? Notes               │         │ string? Notes               │
│ DateTime CreatedAt          │         │ DateTime CreatedAt          │
│ DateTime? UpdatedAt         │         │ DateTime? UpdatedAt         │
└─────────────────────────────┘         └─────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────┐
│                            Quotation                                 │
│──────────────────────────────────────────────────────────────────────│
│ int Id [PK]                                                          │
│ string QuoteNumber [UQ] — مثال: Q-001, Q-002                       │
│ DateTime Date                                                        │
│ string? Description                                                  │
│ string Status — Draft / UpdatedWithPrices / Printed / PDFExported   │
│  / SentViaOutlook                                                    │
│ DateTime CreatedAt                                                   │
│ DateTime? UpdatedAt                                                  │
│ DateTime? PrintedAt                                                  │
└────────────────────────────────┬─────────────────────────────────────┘
                                 │ 1
                                 │
                        ┌────────┴─────────────────────────────┐
                        │          QuotationItem                │
                        │───────────────────────────────────────│
                        │ int Id [PK]                           │
                        │ int QuotationId [FK → Quotation.Id]   │
                        │ int ItemId [FK → Item.Id]             │
                        │                                       │
                        │ string? Supplier1Type  — "ورق دعاية"  │
                        │ decimal? Supplier1Price               │
                        │                                       │
                        │ string? Supplier2Type                 │
                        │ decimal? Supplier2Price               │
                        │                                       │
                        │ string? Supplier3Type                 │
                        │ decimal? Supplier3Price               │
                        │                                       │
                        │ int SortOrder                         │
                        └───────────────────────────────────────┘

┌─────────────────────────────┐         ┌─────────────────────────────┐
│         Signature           │         │         Setting             │
│─────────────────────────────│         │─────────────────────────────│
│ int Id [PK]                 │         │ string Key [PK]             │
│ string Name                 │         │ string Value                │
│ int OrderIndex              │         │ ─────────────────────────── │
│ DateTime CreatedAt          │         │ يستخدم لتخزين:              │
└─────────────────────────────┘         │ • LetterheadLogoPath       │
                                        │ • CompanyName              │
┌─────────────────────────────┐         │ • CompanyAddress           │
│        AuditLog             │         │ • CompanyPhone             │
│ (اختياري — يُقرر أثناء      │         │ • CompanyEmail             │
│  التنفيذ)                   │         │ • FirstTimeSetupDone       │
│─────────────────────────────│         │ • LastBackupDate           │
│ int Id [PK]                 │         └─────────────────────────────┘
│ string Action               │
│ string? Description         │
│ DateTime Timestamp          │
└─────────────────────────────┘
```

### 4.2 العلاقات الأساسية

| العلاقة | النوع | شرح |
|:---------|:------|:-----|
| Quotation → QuotationItem | **1 : N** | عرض سعر واحد يحتوي على بنود متعددة |
| QuotationItem → Item | **N : 1** | كل بند يشير إلى قطعة واحدة من الكتالوج |
| User | **مستقل** | مستخدم واحد، لا علاقات مباشرة مع الجداول الأخرى |
| Supplier | **مستقل** | موردين — مرجع اسمي في QuotationItem (ليس FK) |
| Signature | **مستقل** | أسماء توقيعات للطباعة |
| Setting | **Key-Value** | تخزين إعدادات عامة (شعار، ترويسة) |

### 4.3 ملاحظات مهمة

- **Supplier ليس FK في QuotationItem.** يتم تخزين أسماء الموردين كنصوص (Supplier1Type, Supplier2Type, Supplier3Type) لأن المورد قد يُحذف أو يتغير اسمه لاحقاً ونحتاج الاحتفاظ باسم المورد وقت إنشاء العرض.
- **Item هو FK إلزامي** — حذف قطعة من الكتالوج يجب أن يمنع إذا كانت مستخدمة في عروض سابقة، أو نستخدم Soft Delete.
- **Status** يُخزّن كنص بسيط (string) بقيم محددة: `Draft`, `UpdatedWithPrices`, `Printed`, `PDFExported`, `SentViaOutlook`.

---

## 5. خدمات التطبيق (Application Services)

### 5.1 AuthenticationService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `LoginAsync(username, password)` | Username + كلمة المرور | `bool` (نجاح/فشل) | يبحث عن المستخدم، يقارن الهاش |
| `ChangePasswordAsync(oldPass, newPass)` | كلمة المرور القديمة + الجديدة | `bool` | يتحقق من القديم ثم يحدّث الهاش |
| `IsFirstTimeSetup()` | — | `bool` | هل أول دخول؟ (لا يوجد مستخدم → عرض إعداد) |
| `SetupFirstUserAsync(password)` | كلمة مرور أول مستخدم | `bool` | يُنشئ مستخدم افتراضي (يزيد ماهر) |

### 5.2 QuotationService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `CreateNewQuotationAsync(description)` | وصف اختياري | `Quotation` (جديد) | ينشئ عرض سعر جديد مع رقم تسلسلي |
| `GetQuotationByIdAsync(id)` | QuotationId | `Quotation` + Items | يسترد العرض مع كل بنوده |
| `GetAllQuotationsAsync(filter)` | فلتر (بحث، حالة، تاريخ) | `List<Quotation>` | قائمة العروض للعرض في S4 |
| `UpdateQuotationAsync(quotation)` | كائن Quotation محدّث | `bool` | حفظ التعديلات على العرض |
| `AddQuotationItemAsync(item)` | QuotationItem جديد | `bool` | إضافة بند إلى العرض |
| `RemoveQuotationItemAsync(itemId)` | ItemId | `bool` | حذف بند (تحذير قبل الحذف) |
| `UpdateQuotationItemAsync(item)` | QuotationItem محدّث | `bool` | تعديل بيانات البند |
| `ChangeStatusAsync(quotationId, status)` | Id + Status جديد | `bool` | تحديث حالة العرض |
| `ArchiveQuotationAsync(quotationId)` | Id | `bool` | أرشفة العرض (Soft delete أو علامة Archived) |
| `SearchQuotationsAsync(keyword)` | نص بحث | `List<Quotation>` | بحث في رقم العرض والوصف |

### 5.3 SettingsService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `GetAllSuppliersAsync()` | — | `List<Supplier>` | جميع الموردين |
| `AddSupplierAsync(supplier)` | Supplier جديد | `bool` | إضافة مورد |
| `UpdateSupplierAsync(supplier)` | Supplier محدّث | `bool` | تعديل مورد |
| `DeleteSupplierAsync(supplierId)` | Id | `bool` | حذف مورد (مع التحقق من عدم استخدامه) |
| `GetAllItemsAsync(search)` | نص بحث (اختياري) | `List<Item>` | كل القطع مع فلتر بحث |
| `AddItemAsync(item)` | Item جديد | `bool` | إضافة قطعة للكتالوج |
| `UpdateItemAsync(item)` | Item محدّث | `bool` | تعديل قطعة |
| `DeleteItemAsync(itemId)` | Id | `bool` | حذف قطعة (مع التحقق من عدم استخدامها) |
| `GetAllSignaturesAsync()` | — | `List<Signature>` | أسماء التوقيعات |
| `AddSignatureAsync(signature)` | Signature | `bool` | إضافة توقيع |
| `DeleteSignatureAsync(signatureId)` | Id | `bool` | حذف توقيع |
| `GetSettingAsync(key)` | مفتاح | `string?` | قراءة إعداد |
| `SetSettingAsync(key, value)` | مفتاح + قيمة | `bool` | حفظ إعداد |
| `GetLetterheadDataAsync()` | — | `LetterheadDto` | الشعار + بيانات الشركة |
| `SaveLetterheadDataAsync(data)` | LetterheadDto | `bool` | حفظ بيانات الترويسة |

### 5.4 ReportService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `GetSupplierPriceComparisonAsync()` | — | `List<ComparisonRow>` | مقارنة أسعار الموردين D1 |
| `GetMostRequestedItemsAsync(topN)` | عدد العناصر (افتراضي 10) | `List<ItemRequestCount>` | أكثر القطع طلباً D2 |
| `GetQuotationHistoryAsync(from, to)` | مدى تاريخي | `List<Quotation>` | سجل العروض D3 |
| `GetMonthlyTotalAsync(year, month)` | شهر/سنة | `MonthlyTotalDto` | إجمالي الشهر D4 |

### 5.5 PdfService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `ExportQuotationToPdfAsync(quotationId, filePath, showPrices)` | Id + مسار + إظهار الأسعار | `bool` (نجاح/فشل) | توليد PDF باستخدام QuestPDF |
| `ExportReportToPdfAsync(reportData, reportType, filePath)` | بيانات التقرير + النوع | `bool` | تقارير PDF |

### 5.6 PrintService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `PrintQuotation(quotationId, printMode)` | Id + وضع الطباعة | `bool` | طباعة A4 مباشرة |
| `PrintReport(reportData, reportType)` | بيانات + نوع | `bool` | طباعة تقرير |

وضع الطباعة (PrintMode):
- `WithoutPrices` — طباعة للموردين (بدون أسعار)
- `WithPrices` — طباعة نهائية (بأسعار)

### 5.7 OutlookService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `SendQuotationViaOutlookAsync(quotationId)` | Id | `bool` (نجاح/فشل مع رسالة) | يفتح Outlook مع العرض مرفقاً |

### 5.8 BackupService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `CreateBackupAsync()` | — | `string` (مسار النسخة) | نسخ SQLite.db مع timestamp |
| `GetLastBackupDateAsync()` | — | `DateTime?` | تاريخ آخر نسخة |
| `RestoreFromBackupAsync(filePath)` | مسار النسخة | `bool` | استعادة قاعدة البيانات (بتحذير) |
| `CleanupOldBackupsAsync(maxBackups)` | عدد أقصى | `bool` | حذف النسخ القديمة (الاحتفاظ بآخر 10) |

### 5.9 NavigationService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `NavigateTo(viewName)` | اسم العرض | — | تغيير المحتوى في MainWindow Frame |
| `GoBack()` | — | — | العودة للشاشة السابقة |
| `GetCurrentView()` | — | `string` | اسم الشاشة الحالية |

### 5.10 LoggingService

| الدالة | المدخلات | المخرجات | الشرح |
|:-------|:---------|:---------|:------|
| `LogInfo(message)` | نص | — | تسجيل معلومات |
| `LogWarning(message)` | نص | — | تسجيل تحذير |
| `LogError(message, exception)` | نص + استثناء | — | تسجيل خطأ |
| `GetLogsAsync(count)` | عدد | `List<LogEntry>` | عرض آخر N سجل |

---

## 6. استراتيجية التوجيه (Navigation)

### 6.1 هيكل التنقل

```
                    ┌──────────────────┐
                    │   MainWindow     │
                    │  (Shell + Frame)  │
                    └────────┬─────────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
              ▼              ▼              ▼
     ┌────────────┐  ┌──────────────┐  ┌──────────┐
     │ S1: Login  │  │ S2: Settings  │  │ S3: Quote│
     │ (Window)   │  │ (Page/Frame) │  │ Form     │
     └────────────┘  └──────────────┘  └──────────┘
                                       ┌──────────┐
                                       │ S4: Quote│
                                       │ List     │
                                       └──────────┘
                                       ┌──────────┐
                                       │ S5:      │
                                       │ Reports  │
                                       └──────────┘
```

### 6.2 تدفق التنقل

```
[تطبيق يبدأ]
    │
    ▼
LoginView ← (إذا تم الإعداد مسبقاً) → [إدخال كلمة المرور]
    │                                           │
    │ (نجاح)                                    │ (أول مرة)
    ▼                                           ▼
MainWindow ← → NavigationBar:  ──→ SetupFirstUser (تغيير كلمة المرور)
    │                          |
    │    [الإعدادات] ────────── SettingsView
    │       │                           │
    │       ├── Tab: الموردين            │
    │       ├── Tab: القطع               │
    │       ├── Tab: التوقيعات            │
    │       └── Tab: الترويسة             │
    │                                     │
    │    [عرض سعر جديد] ──────────────── QuotationFormView
    │    [قائمة العروض] ──────────────── QuotationListView
    │       │                                   │
    │       └── (فتح عرض موجود) ──────────── QuotationFormView
    │                                         │
    │    [التقارير] ──────────────────────── ReportsView
    │
    └── شريط سفلي: [تسجيل خروج] [نسخة احتياطية]
```

### 6.3 آليات التنقل

| الآلية | الشرح |
|:--------|:-------|
| **LoginView** | `Window` منفصل (Dialog). عند النجاح → يفتح `MainWindow` ويغلق نفسه |
| **MainWindow** | نافذة رئيسية تحتوي: **شريط جانبي (Sidebar)** للتنقل + **Frame** لعرض المحتوى |
| **NavigationService** | يدير تغيير `Frame.Content` عبر Uri أو Page Name |
| **SettingsView** | `Page` مع `TabControl` (4 Tabs داخلية) |
| **QuotationFormView / QuotationListView / ReportsView** | `Pages` تُحمّل في Frame |

### 6.4 قواعد التنقل

| القاعدة | الشرح |
|:--------|:-------|
| **Login أولاً** | لا يمكن الوصول لأي شاشة بدون تسجيل دخول ناجح |
| **Settings مباشرة** | من الشريط الجانبي في أي وقت |
| **العروض** | "عرض جديد" → QuotationForm (فارغ) | "فتح عرض" من القائمة → QuotationForm (مليء بالبيانات) |
| **رجوع** | كل صفحة تدعم رجوع (Back) عند الحاجة |
| **حالة التطبيق** | عند الإغلاق، يُحفظ آخر شاشة في Setting لاستعادتها عند الفتح التالي |

---

## 7. إدارة الأخطاء والتسجيل (Error Handling & Logging)

### 7.1 استراتيجية معالجة الأخطاء

```
┌─────────────────────────────────────────────────────────────────┐
│                     طبقات معالجة الأخطاء                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  طبقة 1 — Data Layer (AppDbContext)                             │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • DbUpdateException → ترجمة رسالة خطأ مفهومة              │  │
│  │ • SQLiteException → تسجيل + عرض "خطأ في قاعدة البيانات"   │  │
│  │ • UniqueConstraintException → "رقم العرض موجود مسبقاً"    │  │
│  └───────────────────────────────────────────────────────────┘  │
│                            │                                     │
│                            ▼                                     │
│  طبقة 2 — Service Layer                                         │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • try/catch حول كل استدعاء DbContext                       │  │
│  │ • إرجاع Result<T> بدلاً من null (نجاح/فشل + رسالة)        │  │
│  │ • تسجيل الأخطاء في LoggingService                          │  │
│  │ • لا تمرير استثناءات غير معالجة إلى ViewModel              │  │
│  └───────────────────────────────────────────────────────────┘  │
│                            │                                     │
│                            ▼                                     │
│  طبقة 3 — ViewModel Layer                                       │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • عرض رسائل الخطأ للمستخدم (MessageBox / Snackbar)        │  │
│  │ • تحديث حالة UI (IsError, ErrorMessage)                   │  │
│  │ • Disable/Enable الأزرار حسب الحالة                       │  │
│  └───────────────────────────────────────────────────────────┘  │
│                            │                                     │
│                            ▼                                     │
│  طبقة 4 — Presentation Layer (Views)                            │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • Data Validation (قبل الإرسال للـ ViewModel)             │  │
│  │ • Required Field Validation                               │  │
│  │ • Input Mask / Format Validation                          │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 7.2 أنواع الأخطاء المتوقعة ومعالجتها

| نوع الخطأ | المصدر | المعالجة |
|:----------|:-------|:---------|
| فشل الاتصال بقاعدة البيانات | Data Layer | عرض "تعذر الاتصال بقاعدة البيانات" + تسجيل |
| خطأ في إدخال المستخدم (حقل مطلوب) | View | Validation border + رسالة خطأ بجانب الحقل |
| محاولة حذف مورد/قطعة مستخدمة | Service | "لا يمكن الحذف — هذا المورد/القطعة مستخدمة في عروض سابقة" |
| Outlook غير مثبت | Service | "Outlook غير موجود — سيتم فتح PDF بدلاً من ذلك" |
| فشل توليد PDF | Service | "حدث خطأ أثناء إنشاء PDF — تحقق من المسار والمجلد" |
| خطأ غير متوقع | أي طبقة | تسجيل + "حدث خطأ غير متوقع" + إمكانية إعادة المحاولة |
| فشل النسخة الاحتياطية | BackupService | "لم نتمكن من إنشاء نسخة احتياطية — تحقق من مساحة القرص" |

### 7.3 نظام التسجيل (Logging)

| الخاصية | القيمة |
|:---------|:-------|
| **مخزن السجلات** | ملف نصي (`logs/TeraQuotation.log`) في مجلد التطبيق |
| **تنسيق السجل** | `[2026-07-13 14:30:00] [INFO/WARN/ERROR] الرسالة — [اسم الخدمة/الدالة]` |
| **الحد الأقصى** | 5 MB → Automatic Rotation (تسمية الملف القديم بـ `.old`) |
| **المسار** | `%LOCALAPPDATA%\TeraQuotation\logs\TeraQuotation.log` |
| **المستويات** | INFO (افتراضي), WARNING, ERROR, FATAL |
| **ما يُسجّل** | بدء/إيقاف التطبيق، Login (نجاح/فشل)، إنشاء/تعديل/حذف عروض، طباعة، PDF، أخطاء |

### 7.4 تحقق صحة الإدخال (Input Validation)

| العنصر | قواعد التحقق |
|:-------|:-------------|
| **كلمة المرور** | ≥ 4 أحرف (للأمان الأساسي) |
| **اسم القطعة** | Required, Max 200 حرف |
| **اسم المورد** | Required, Max 200 حرف |
| **سعر** | يجب أن يكون رقم موجب (أو null/فارغ) |
| **نوع المورد** | Max 100 حرف |
| **وصف العرض** | Optional, Max 500 حرف |
| **رقم العرض** | يُنشأ تلقائياً — لا إدخال يدوي |
| **التاريخ** | يُعبأ تلقائياً — يمكن تعديله |

---

## 8. الاعتبارات الأمنية (Security)

### 8.1 تخزين كلمة المرور

| البند | القرار | الشرح |
|:------|:-------|:------|
| **خوارزمية الهاش** | **SHA256 + Salt** (أو BCrypt إن أمكن) | مناسبة لمستخدم واحد |
| **تخزين** | `User.PasswordHash` (نص مشفر) | لا تخزين نص واضح أبداً |
| **المقارنة** | `PasswordHelper.Verify(password, hash)` | مقارنة مع الـ Hash |
| **أول مستخدم** | يُنشأ أثناء First Time Setup | بعد الموافقة على شروط الإعداد |

### 8.2 حماية قاعدة البيانات

| البند | الإجراء |
|:------|:--------|
| **موقع الملف** | `%LOCALAPPDATA%\TeraQuotation\Data\TeraQuotation.db` — داخل مجلد محمي للمستخدم |
| **نسخ احتياطي** | نسخة من الملف مباشر — لا تشفير (المستخدم الوحيد يملك الجهاز) |
| **SQL Injection** | غير ممكن — EF Core يستخدم Parameterized Queries |
| **إعدادات حساسة** | لا توجد — كل الإعدادات عامة (شعار، اسم شركة) |

### 8.3 حماية التطبيق

| البند | الإجراء |
|:------|:--------|
| **نافذة Login** | Modal — لا يمكن تخطيها |
| **Session** | مستمر — طالما التطبيق مفتوح، المستخدم مسجّل الدخول |
| **Logout** | زر تسجيل خروج → العودة لشاشة Login |
| **Access Control** | غير مطلوب — مستخدم واحد |
| **Input Sanitization** | Trim + Escape أحرف خاصة في البحث |

### 8.4 ما ليس مطلوباً (لأن التطبيق محلي بمستخدم واحد)

- ❌ لا JWT / Tokens
- ❌ لا SSL/TLS
- ❌ لا Audit Trail للمستخدمين
- ❌ لا Rate Limiting
- ❌ لا 2FA
- ❌ لا Roles/Permissions

---

## 9. اعتبارات الأداء (Performance)

### 9.1 خصائص الأداء المتوقعة

| البند | التوقع |
|:------|:--------|
| **حجم قاعدة البيانات** | < 50 MB (سنوات من العروض) |
| **عدد العروض** | < 10,000 (لعميل فردي) |
| **عدد القطع** | < 5,000 |
| **عدد الموردين** | < 100 |
| **سرعة تحميل الشاشات** | < 1 ثانية |
| **الذاكرة (RAM)** | < 200 MB |

### 9.2 تحسينات الأداء

| التحسين | التطبيق | الشرح |
|:--------|:--------|:------|
| **Asynchronous Queries** | كل Services | استخدام `async/await` مع EF Core |
| **Lazy Loading للبيانات** | QuotationForm | تحميل القطع والموردين فقط عند فتح الشاشة |
| **Virtualization** | DataGrids | `VirtualizingStackPanel` للجداول الطويلة |
| **Backup على Thread منفصل** | BackupService | `Task.Run` لنسخ الملف دون تجميد الواجهة |
| **تقليل Migrations** | Data Layer | دمج التغييرات — لا تغيير Schema بعد الاستقرار |
| **Search Indexing** | قائمة العروض | بحث في SQL (LIKE) — مناسب للكميات الصغيرة |
| **تحميل مسبق للـ Settings** | عند تشغيل التطبيق | Cache الإعدادات في الذاكرة |

### 9.3 ما لا حاجة له (لأن المشروع صغير)

- ❌ لا Caching Layer (Redis / MemoryCache)
- ❌ لا Pagination معقد — يكفي `Skip/Take` أساسي
- ❌ لا Connection Pooling — اتصال SQLite واحد
- ❌ لا Indexing معقد — PK/FK indexes كافية
- ❌ لا Background Services (Windows Service)

---

## 10. اعتبارات النشر (Deployment)

### 10.1 استراتيجية النشر

```
تطبيق WPF (.NET 8)
    │
    ▼
Publish as Single File Executable
    │
    ├── TeraQuotation.exe (ملف واحد)
    ├── TeraQuotation.db (SQLite — يُنشأ تلقائياً)
    ├── logs/ (مجلد — يُنشأ تلقائياً)
    └── Backups/ (مجلد — يُنشأ تلقائياً)
```

### 10.2 خطوات النشر

| الخطوة | الأمر | الشرح |
|:-------|:------|:------|
| 1 | `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true` | نشر كـ EXE واحد |
| 2 | نسخ الملف الناتج إلى `C:\TeraQuotation\` | مجلد التثبيت |
| 3 | تشغيل `TeraQuotation.exe` | تلقائياً ينشئ المجلدات و SQLite.db |
| 4 | إنشاء اختصار على سطح المكتب | للعميل |

### 10.3 هيكل مجلد التطبيق عند التشغيل

```
%LOCALAPPDATA%\TeraQuotation\
├── Data\
│   └── TeraQuotation.db              # SQLite (يُتاح تلقائياً)
├── Logs\
│   └── TeraQuotation.log             # سجلات التطبيق
├── Backups\
│   ├── TeraQuotation_2026-07-13_1430.db
│   ├── TeraQuotation_2026-07-14_0900.db
│   └── ... (آخر 10 نسخ)
├── Exports\
│   └── (ملفات PDF مصدرة)
└── TeraQuotation.exe                  # الملف التنفيذي
```

### 10.4 متطلبات الجهاز

| المتطلب | الحد الأدنى |
|:---------|:------------|
| **نظام التشغيل** | Windows 10 (64-bit) أو Windows 11 |
| **.NET Runtime** | .NET 8 Desktop Runtime (مضمن مع self-contained) |
| **المعالج** | أي x64 |
| **الذاكرة** | 512 MB RAM |
| **القرص الصلب** | 100 MB (للتطبيق + قاعدة البيانات) |
| **Outlook** | 2016/2019/Microsoft 365 (اختياري) |
| **طابعة** | أي طابعة Windows (لطباعة A4) |

### 10.5 التحديثات

| البند | الإجراء |
|:------|:--------|
| **آلية التحديث** | يدوية — تسليم ملف EXE جديد |
| **ترحيل قاعدة البيانات** | EF Core Migrations (Add-Migration + Update-Database يدوياً) |
| **النسخ الاحتياطي قبل التحديث** | يُنشئ BackupService نسخة تلقائية قبل أي تغيير كبير |
| **التوافقية** | الإصدارات الجديدة تحتفظ بنفس Schema (إضافة أعمدة فقط، لا حذف) |

---

## 11. قرارات معمارية مسجلة (Architecture Decision Records)

### ADR-001: مشروع WPF واحد (لا Class Library منفصلة)

| الحقل | القيمة |
|:------|:-------|
| **القرار** | مشروع WPF واحد يضم كل الطبقات (Models, Views, ViewModels, Services, Data) |
| **السياق** | المشروع صغير — 5 شاشات، مستخدم واحد |
| **البدائل** | Multi-project مع Class Library منفصلة لكل طبقة |
| **الاختيار** | مشروع واحد — لتقليل التعقيد وتسريع التطوير |
| **السبب** | الفصل في مجلدات كافٍ للمشاريع الصغيرة. هندسة Multi-project تزيد وقت البناء والتعقيد بدون فائدة تذكر |

### ADR-002: EF Core بدلاً من Microsoft.Data.Sqlite الخام

| الحقل | القيمة |
|:------|:-------|
| **القرار** | استخدام Entity Framework Core مع SQLite |
| **السياق** | الحاجة لإدارة العلاقات، الـ Migrations، والتطوير السريع |
| **البدائل** | Microsoft.Data.Sqlite (ADO.NET خام)، Dapper |
| **الاختيار** | EF Core — مناسب لحجم المشروع |
| **السبب** | يوفر Migrations تلقائية، علاقات، Lazy Loading، ويقلل كود الوصول للبيانات. الأداء مقبول للكميات الصغيرة |

### ADR-003: QuestPDF لتوليد PDF

| الحقل | القيمة |
|:------|:-------|
| **القرار** | QuestPDF لتوليد مستندات PDF |
| **السياق** | الحاجة لتوليد PDF احترافي بعرض الأسعار والتقارير |
| **البدائل** | iTextSharp (تجاري)، IronPDF (تجاري)، WPF FixedDocument → XPS → PDF |
| **الاختيار** | QuestPDF — مجاني، Fluent API، مفتوح المصدر |
| **السبب** | مجاني بالكامل لـ .NET 8، واجهة برمجية نظيفة، يدعم العربية RTL، لا حاجة لترخيص |

### ADR-004: FixedDocument + PrintDialog للطباعة

| الحقل | القيمة |
|:------|:-------|
| **القرار** | استخدام WPF FixedDocument + PrintDialog لطباعة A4 |
| **السياق** | طباعة دقيقة لتنسيق A4 مع ترويسة وجداول |
| **البدائل** | QuestPDF → Print (يتطلب تحويل PDF إلى print)، RDLC Reports |
| **الاختيار** | FixedDocument + PrintDialog — تحكم كامل بالتنسيق داخل WPF |
| **السبب** | FixedDocument يوفر تحكم دقيق بـ Point/Pixel للطباعة، يدعم RTL، لا يحتاج مكتبات إضافية |

### ADR-005: Supplier أسماء كنصوص وليس FK

| الحقل | القيمة |
|:------|:-------|
| **القرار** | تخزين أسماء الموردين في QuotationItem كنصوص (Supplier1Type, Supplier2Type, Supplier3Type) بدلاً من Foreign Keys |
| **السياق** | الحفاظ على البيانات التاريخية — إذا تغير اسم المورد أو حُذف، يبقى اسمه في العروض القديمة |
| **البدائل** | FK مع Soft Delete وعدم حذف حقيقي |
| **الاختيار** | نصوص — لأنها تعكس الواقع (العرض يوثق الاسم وقت الإنشاء) |
| **السبب** | سلامة البيانات التاريخية أهم من تكامل العلاقات. لا حاجة لـ JOIN مع Suppliers في عروض قديمة |

### ADR-006: Single File Executable (Self-Contained)

| الحقل | القيمة |
|:------|:-------|
| **القرار** | نشر التطبيق كـ Single File Executable (self-contained) |
| **السياق** | العميل ليس مطوراً — يجب أن يعمل التطبيق بمجرد فتح الـ EXE |
| **البدائل** | Framework-dependent (يحتاج تثبيت .NET Runtime)، MSI Installer |
| **الاختيار** | Self-contained Single File — أسهل للعميل |
| **السبب** | يحتوي كل ما يلزم (حجم ~60-80 MB) ولا يحتاج تثبيت Runtime. يعمل مباشرة من USB أو سطح المكتب |

---

## 12. التبعيات والافتراضات

### 12.1 التبعيات (Dependencies)

| الملف | العلاقة | التأثير |
|:------|:--------|:--------|
| `06_DATA_MODEL_PREPARATION.md` | **مشتق** — هذا الملف يحدد الكيانات والعلاقات التي ستُفصّل في Data Model | Data Model يعتمد على الـ Entities المذكورة هنا |
| `07_SCREENS_AND_UI_STRUCTURE.md` | **مشتق** — Navigation و ViewModels تحدد توزيع الشاشات | UI Structure يعتمد على الـ Navigation Strategy |
| `05_BUSINESS_WORKFLOWS.md` | **مرتبط** — Services تحدد الـ Workflows | Business Workflows تستخدم Services المذكورة هنا |
| `13_REPORTS_AND_DASHBOARDS.md` | **مرتبط** — ReportService يحدد التقارير | Reports تعتمد على ReportService |
| `28_UI_UX_GUIDELINES.md` | **مرجع** — Theme و Styles العامة | EngineeringAgent يرجع لها للتنسيق |
| Technology Profile | **ملزم** — جميع القرارات التقنية من البروفايل | أي مخالفة تحتاج استثناء |

### 12.2 الافتراضات (Assumptions)

| # | الافتراض | التأثير إذا خالف |
|:-:|:---------|:----------------|
| 1 | العميل يستخدم Windows 10/11 64-bit | لن يعمل على أنظمة أقدم |
| 2 | لا توجد حاجة لشاشة إعدادات متقدمة (إدارة مستخدمين) | لا يمكن إضافة مستخدمين لاحقاً بدون تعديل |
| 3 | لا حاجة لتشفير قاعدة البيانات | الملف المادي مكشوف لأي شخص لديه صلاحية الوصول للمجلد |
| 4 | Outlook مثبت على جهاز العميل (مع Fallback) | لو لم يكن مثبتاً، تعمل بقية الميزات |
| 5 | العميل لديه طابعة A4 لطباعة العروض | لا يمكن الطباعة بدون طابعة (يبقى PDF بديلاً) |
| 6 | جميع العمليات محلية — لا اتصال بالإنترنت | لا يمكن إرسال بريد إلكتروني بدون Outlook (SMTP غير مدعوم) |
| 7 | بيانات التطبيق صغيرة (< 50,000 سجل) | الأداء مُحسّن فقط للكميات الصغيرة |

### 12.3 توصيات للملفات التالية

| الملف التالي | توصيات من هذا الملف |
|:-------------|:--------------------|
| **06_DATA_MODEL_PREPARATION.md** | استخدم الكيانات والعلاقات المذكورة في Section 4. أضف `HasColumnType` للأسعار (decimal(18,2)). قرّر إذا كان `AuditLog` ضرورياً. وثّق الـ Fluent API للعلاقات والمفاتيح |
| **07_SCREENS_AND_UI_STRUCTURE.md** | استخدم الـ 5 شاشات المذكورة مع Navigation Strategy. للـ Settings، تعامل مع 4 Tabs داخل صفحة واحدة. للـ QuotationForm، صمّم DataGrid بـ 7 أعمدة (قطعة + 3 موردين × نوع/سعر) |
| **05_BUSINESS_WORKFLOWS.md** | راجع تدفق البيانات في Section 3 وخدمات Section 5 لبناء Workflows دقيقة. ركّز على: Two-Stage Printing، حالات Status الانتقالية |
| **13_REPORTS_AND_DASHBOARDS.md** | الـ 4 تقارير محدّدة في ReportService (Section 5.4). صمّم الواجهة بناءً عليها |

---

## ختم الملف

| البند | الحالة |
|:------|:-------|
| **إعداد** | Software Designer Agent (مُصمم) |
| **تاريخ الإصدار** | 2026-07-13 |
| **الحالة** | ✅ Module Baseline Approved |
| **الاعتماد** | ⏳ بانتظار مراجعة TeraAgent و Majed |
| **المراجعة التالية المقررة** | — (مرة واحدة — قبل بدء التنفيذ) |
