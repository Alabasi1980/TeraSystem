# PROJECT_MASTER_PLAN.md — TeraQuotation

> **Phase 5 — Execution Planning**
> **تاريخ الإصدار:** 2026-07-13
> **المعتمد:** ✅ معتمد من Majed — 2026-07-13

---

## 1. ملخص المشروع

| البند | القيمة |
|:------|:-------|
| **التطبيق** | TeraQuotation — Windows Desktop WPF/C# |
| **العميل** | يزيد ماهر (فرد، صيانة آليات) |
| **الحجم** | 🟢 Small — 5 شاشات، ~19 ميزة |
| **التقنية** | .NET 8 + WPF + CommunityToolkit.Mvvm + SQLite |
| **السعر** | ~300 JOD (60% + 40%) |
| **مدة التسليم** | 5-7 أيام عمل من بدء التنفيذ |
| **الضمان** | شهر واحد |

---

## 2. تصنيف الميزات (MVP Classification)

وفقاً لـ `MVP_DEFINITION_PROTOCOL.md`:

| التصنيف | الميزات |
|:--------|:--------|
| **🥇 Core MVP** | أساسي — بدونه التطبيق لا يعمل |
| **🥈 Extended** | مهم — يمكن أن يؤجل أسبوعاً واحداً |
| **🔵 Later** | لاحق — بعد الاستقرار |
| **⛔ Out of Scope** | خارج النطاق — غير مطلوب |

### Core MVP (P1 — لا بد منه للتسليم):

| # | الميزة | الموديول |
|:-:|:-------|:---------|
| 1 | Login Screen + أول تسجيل (كلمة مرور) | A |
| 2 | Settings — إدارة الموردين (إضافة/تعديل/حذف) | B |
| 3 | Settings — إدارة كتالوج القطع (إضافة/بحث/حذف) | B |
| 4 | Settings — إدارة التوقيعات | B |
| 5 | Settings — إعداد الترويسة (شعار + بيانات) | B |
| 6 | Quotation Form — إنشاء عرض + تسلسل تلقائي | C |
| 7 | Quotation Form — جدول القطع (7 أعمدة) | C |
| 8 | Quotation Form — إضافة قطعة من الكتالوج | C |
| 9 | Quotation Form — Quick-Add قطعة جديدة | C |
| 10 | Quotation Form — حفظ كمسودة/مكتمل | C |
| 11 | Quotation List — عرض العروض + بحث/تصفية | C |
| 12 | Quotation List — فتح عرض للتعديل | C |
| 13 | طباعة A4 (بدون أسعار + نهائية) | C |
| 14 | تصدير PDF | C |
| 15 | Auto Backup | D |

### Extended (P2 — يُنفذ بعد Core):

| # | الميزة | الموديول |
|:-:|:-------|:---------|
| 16 | Outlook Integration (اختياري) | C |
| 17 | تقرير D1: مقارنة أسعار الموردين | D |
| 18 | تقرير D2: أكثر القطع طلباً | D |
| 19 | تقرير D3: سجل العروض | D |
| 20 | تقرير D4: الإجمالي الشهري | D |

### Later / Out of Scope:

| الميزة | التصنيف | السبب |
|:-------|:--------:|:------|
| Audit Log | 🔵 Later | اختياري، ليس أساسياً |
| Multi-user | ⛔ Out of Scope | غير مطلوب |
| Cloud/Online | ⛔ Out of Scope | غير مطلوب |
| Invoicing | ⛔ Out of Scope | غير مطلوب |

---

## 3. Task Breakdown

### 🥇 Batch 1 — Foundation (اليوم 1-2)

| TASK-ID | الوصف | الموديول | الاعتماديات |
|:--------|:------|:--------:|:-----------|
| **TASK-COD-001** | Scaffold مشروع WPF + هيكل MVVM + حزم NuGet | Foundation | — |
| **TASK-COD-002** | إنشاء Entities + DbContext + Migrations + Seed | Data | 001 |
| **TASK-COD-003** | إنشاء Services Layer (QuotationService, SettingsService, ReportService, PdfService, OutlookService, BackupService, NavigationService) | Services | 002 |
| **TASK-COD-004** | Login Screen (S1) + Password Management | A | 003 |

### 🥇 Batch 2 — Settings Module (اليوم 2-3)

| TASK-ID | الوصف | الموديول | الاعتماديات |
|:--------|:------|:--------:|:-----------|
| **TASK-COD-005** | Settings Screen (S2) — هيكل عام + Navigation (Tabs) | B | 004 |
| **TASK-COD-006** | Settings — Suppliers Tab (CRUD) | B | 005 |
| **TASK-COD-007** | Settings — Items Tab (CRUD + Search) | B | 006 |
| **TASK-COD-008** | Settings — Signatures Tab + Letterhead Tab | B | 007 |

### 🥇 Batch 3 — Quotation Core (اليوم 3-4)

| TASK-ID | الوصف | الموديول | الاعتماديات |
|:--------|:------|:--------:|:-----------|
| **TASK-COD-009** | Quotation Form (S3) — هيكل + تسلسل تلقائي + حفظ | C | 008 |
| **TASK-COD-010** | Quotation Form — جدول القطع + 3 موردين + إضافة/Quick-Add | C | 009 |
| **TASK-COD-011** | Quotation List (S4) — عرض + بحث + تصفية + فتح | C | 010 |
| **TASK-COD-012** | طباعة A4 (بدون أسعار + نهائية) + PDF | C | 011 |
| **TASK-COD-013** | Outlook Integration (اختياري — Fallback PDF) | C | 012 |

### 🥈 Batch 4 — Reports & Polish (اليوم 4-5)

| TASK-ID | الوصف | الموديول | الاعتماديات |
|:--------|:------|:--------:|:-----------|
| **TASK-COD-014** | Reports Screen (S5) — هيكل عام | D | 012 |
| **TASK-COD-015** | تقرير D1 + D2 (مقارنة موردين + أكثر القطع طلباً) | D | 014 |
| **TASK-COD-016** | تقرير D3 + D4 (سجل عروض + إجمالي شهري) | D | 015 |
| **TASK-COD-017** | Auto Backup (خلفية تلقائية + استرجاع يدوي) | D | 016 |
| **TASK-COD-018** | Final Polish + RTL Check + Edge Cases + Fixes | All | 017 |
| **TASK-COD-019** | Testing + Handover Prep | All | 018 |

### ملاحظات:
- **عدد المهام الكلي:** 19 TASK-COD-* (مناسب لمشروع Small)
- **أصغر وحدة قابلة للتنفيذ:** كل TASK-ID يمثل Feature واحدة أو مجموعة صغيرة مترابطة
- **كل TASK-ID لا يجمع:** DB+UI+API في نفس المهمة (لا API أصلاً)
- **TASK-COD-019** يشمل الاختبار والتسليم — قد يُقسّم لاحقاً

---

## 4. هيكل المجلدات المتوقع بعد التنفيذ

```
clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/source/TeraQuotation/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / .cs
├── Models/
│   ├── User.cs
│   ├── Supplier.cs
│   ├── Item.cs
│   ├── Quotation.cs
│   ├── QuotationItem.cs
│   ├── Signature.cs
│   ├── Setting.cs
│   └── AuditLog.cs (اختياري)
├── Data/
│   ├── AppDbContext.cs
│   └── Migrations/
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── QuotationFormViewModel.cs
│   ├── QuotationListViewModel.cs
│   └── ReportsViewModel.cs
├── Views/
│   ├── LoginView.xaml / .cs
│   ├── SettingsView.xaml / .cs
│   ├── QuotationFormView.xaml / .cs
│   ├── QuotationListView.xaml / .cs
│   ├── ReportsView.xaml / .cs
│   └── Reports/
│       ├── SupplierComparisonView.xaml / .cs (D1)
│       ├── TopItemsView.xaml / .cs (D2)
│       ├── QuotationHistoryView.xaml / .cs (D3)
│       └── MonthlyTotalView.xaml / .cs (D4)
├── Services/
│   ├── IQuotationService.cs / QuotationService.cs
│   ├── ISettingsService.cs / SettingsService.cs
│   ├── IReportService.cs / ReportService.cs
│   ├── IPdfService.cs / PdfService.cs
│   ├── IOutlookService.cs / OutlookService.cs
│   ├── IBackupService.cs / BackupService.cs
│   └── INavigationService.cs / NavigationService.cs
├── Converters/
│   ├── BooleanToVisibilityConverter.cs
│   ├── InverseBooleanConverter.cs
│   ├── StatusToColorConverter.cs
│   ├── StatusToTextConverter.cs
│   └── ... (8 Converters)
├── Helpers/
│   └── ReportHelper.cs (طباعة موحدة)
├── TeraQuotation.csproj
└── TeraQuotation.db (SQLite — يُنشأ تلقائياً)
```

---

## 5. الجدول الزمني المقدر

| اليوم | البatches | التسليم |
|:-----|:---------|:--------|
| **اليوم 1** | Batch 1 (Foundation) | Scaffold + Data + Login |
| **اليوم 2** | Batch 2 (Settings) | Settings كاملة |
| **اليوم 3** | Batch 3 (Quotation Core) | إنشاء + عرض + طباعة |
| **اليوم 4** | Batch 4 (Reports + Polish) | تقارير + تحسينات |
| **اليوم 5** | Final Testing + Delivery | تسليم للعميل |

> المدة: **5 أيام عمل** (مع هامش يومين للطوارئ = 5-7 أيام)

---

## 6. خطة التنفيذ المقترحة

1. **Build Mode** ← طلب موافقة من Majed
2. **Pre-Execution Gate** ← لكل TASK-COD-* قبل التنفيذ
3. **EngineeringAgent** ← التنفيذ لكل TASK-COD-* بالتسلسل
4. **Post-Execution Review Gate** ← بعد كل مهمة
5. **كل Batch** ← يُسلّم كمجموعة → مراجعة → قبول → التالي

---

**إعداد:** TeraAgent
**تاريخ:** 2026-07-13
**الحالة:** ✅ **معتمد من Majed** — 2026-07-13 — التنفيذ مسموح 🚀

> ⚠️ **تصحيح المسار:** كل مصدر الكود للتطبيق موجود تحت `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/source/TeraQuotation/`. ملفات التحضير تحت `project-preparation/` وملفات التحكم تحت `project-control/` داخل نفس مسار العميل. جذر المنظومة لا يحتوي أي ملفات خاصة بالعميل.
