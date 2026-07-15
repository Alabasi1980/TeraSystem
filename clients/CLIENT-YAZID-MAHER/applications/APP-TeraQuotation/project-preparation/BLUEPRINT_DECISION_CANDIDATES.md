# BLUEPRINT_DECISION_CANDIDATES — TeraQuotation

> **مرشحات وتوصيات فقط — ليست قرارات نهائية**
> **تاريخ:** 2026-07-13
> **المصدر:** APPLICATION_BLUEPRINT.md §9

---

## المرشحات التقنية

| القرار | التوصية | الخيارات البديلة | الأساس المنطقي |
|:-------|:-------:|:----------------:|:--------------:|
| **لغة البرمجة** | **C# (.NET 8)** | — | متطلبات Windows + WPF + تكامل Office |
| **Framework UI** | **WPF** | WinForms, Blazor Hybrid, MAUI | الأنسب لتطبيقات Desktop Windows مع طباعة دقيقة |
| **قاعدة البيانات** | **SQLite** | SQL Server LocalDB | خفيفة، ملف واحد، لا تحتاج تثبيت سيرفر |
| **PDF Library** | QuestPDF | iTextSharp, IronPDF | مفتوحة المصدر، مجانية، مناسبة |
| **Outlook** | Microsoft.Office.Interop.Outlook | MAPI, SMTP | تكامل مباشر مع Outlook المثبت |
| **طباعة A4** | FixedDocument WPF | RDLC Reports | تحكم مباشر بالتنسيق |
| **المصادقة** | BCrypt password hashing | SHA256 | بسيطة وآمنة لمستخدم واحد |
| **MVVM Toolkit** | CommunityToolkit.Mvvm | — | هيكل نظيف وسريع |

## توصيات معمارية

1. **هيكل المشروع:** WPF MVVM مع فصل واضح (Models / ViewModels / Views / Services)
2. **Data Access:** Entity Framework Core مع SQLite Provider
3. **الخدمات:** Services منفصلة (QuotationService, ReportService, PrintService, BackupService)
4. **التقارير:** UserControls داخل WPF مع DataGrid — لا حاجة لمكتبة تقارير منفصلة
5. **الطباعة:** DocumentPaginator أو FixedDocument للتحكم الدقيق بـ A4

---

**ملاحظة:** هذه المرشحات ستحتاج تأكيد TeraAgent أثناء Phase 2 — Project Decision Formation.
