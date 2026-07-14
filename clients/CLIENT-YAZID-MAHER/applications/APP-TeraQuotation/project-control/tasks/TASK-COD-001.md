# TASK-COD-001: Scaffold WPF Project + MVVM Structure + NuGet Packages

> **Batch 1 — Foundation | اليوم 1**
> **الحالة:** ✅ Approved (جاهز للتنفيذ)

---

## 1. الوصف

إنشاء هيكل مشروع WPF كامل بنمط MVVM مع إعداد قاعدة البيانات SQLite وأول Migration، جاهز لبدء تطوير الشاشات.

## 2. المخرجات المطلوبة

- [x] مشروع WPF (.NET 8) باسم `TeraQuotation`
- [x] مجلدات الهيكل: Models, ViewModels, Views, Services, Data, Converters, Helpers
- [x] حزم NuGet: CommunityToolkit.Mvvm, Microsoft.EntityFrameworkCore.Sqlite, QuestPDF
- [x] Entities (Models): User, Supplier, Item, Quotation, QuotationItem, Signature, Setting, AuditLog
- [x] AppDbContext مع DbSets لكل الكيانات
- [x] أول Migration يتم تطبيقها على SQLite
- [x] التطبيق يشتغل (`dotnet run`) بدون أخطاء

## 3. Allowed Write Targets

```
clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/source/TeraQuotation/
├── Models/*.cs
├── Data/AppDbContext.cs
├── Data/Migrations/
├── TeraQuotation.csproj
```

> ⚠️ **تصحيح:** المسار الأصلي `src/TeraQuotation/` كان خاطئاً (جذر المنظومة). تم نقل الملفات إلى مسار العميل الصحيح. راجع GAP-0002. هذا هو المسار الصحيح لكل المهام اللاحقة.

## 4. Acceptance Criteria

- ✅ `dotnet build` يمر بدون أخطاء
- ✅ `dotnet run` يفتح نافذة WPF (فارغة حالياً)
- ✅ مجلدات Models, ViewModels, Views, Services, Data, Converters, Helpers موجودة
- ✅ جميع الكيانات الثمانية معرفة في Models/
- ✅ AppDbContext مع DbSets
- ✅ Migration منشأة ومطبّقة على SQLite
- ✅ ملف `TeraQuotation.db` يُنشأ في مجلد الـ Output (أو المشروع)
- ✅ ملف `.csproj` يحتوي الحزم المطلوبة فقط

## 5. مصادر للمساعدة

- `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/project-preparation/08_TECHNICAL_ARCHITECTURE.md` (القرارات المعمارية)
- `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/project-preparation/06_DATA_MODEL_PREPARATION.md` (نموذج البيانات)
- `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/project-preparation/07_SCREENS_AND_UI_STRUCTURE.md` (الشاشات)
- `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/project-preparation/05_BUSINESS_WORKFLOWS.md` (سير العمل)
- `clients/CLIENT-YAZID-MAHER/applications/APP-TeraQuotation/project-preparation/13_REPORTS_AND_DASHBOARDS.md` (التقارير)
- `tera-system/profiles/dotnet-wpf-sqlite.md` (Technology Profile المعتمد)

## 6. Pre-Execution Gate Result

| # | السؤال | النتيجة |
|:-:|:-------|:-------:|
| 1 | مرتبطة بخطة معتمدة؟ | ✅ PROJECT_MASTER_PLAN.md — Batch 1, TASK-COD-001 |
| 2 | أصغر وحدة تنفيذية؟ | ✅ Scaffold فقط — لا شاشات ولا منطق |
| 3 | هدف واحد؟ | ✅ Foundation / Infrastructure |
| 4 | عناصر قابلة للتأجيل؟ | لا — كلها أساسية |
| 5 | تضيف UI غير مطلوب؟ | لا |
| 6 | تضيف API/Route غير مطلوب؟ | لا — Desktop |
| 7 | تضيف Auth غير مطلوب؟ | لا — Entities فقط |
| 8 | تضيف DB بدون أن تكون مهمة Data؟ | ✅ هذه مهمة Data معتمدة |
| 9 | تنفذ Migration بدون موافقة؟ | ✅ Migration مطلوبة صراحة |
| 10 | Secrets خطيرة؟ | لا — SQLite محلي |
| 11 | مكتبات غير ضرورية؟ | لا — فقط CommunityToolkit.Mvvm + EF Core.Sqlite + QuestPDF |
| 12 | تكتب خارج Allowed Targets؟ | لا |
| 13 | تعدل system/preparation؟ | لا |
| 14 | أوامر Shell مؤثرة؟ | نعم (dotnet new/packages/migrations) — آثارها معروفة |
| 15 | فحص آثار CLI؟ | ✅ تم — كل أمر ضمن النطاق |
| 16 | أمر ينشئ كود خارج النطاق؟ | لا |
| 17 | تناقض في القيود؟ | لا |
| 18 | معايير قبول قابلة للاختبار؟ | ✅ |
| 19 | مسار تراجع آمن؟ | ✅ حذف المجلد وإعادة Scaffold |
| 20 | UI/Frontend؟ | N/A — مهمة Foundation |
| **النتيجة النهائية** | | **🟢 PASS** |

## Vitality & Polish Checklist

N/A — هذه مهمة Foundation/Scaffold، لا تحتوي واجهات مستخدم.

---

## 7. Task Log

| التاريخ | الحدث |
|:--------|:------|
| 2026-07-13 | ✅ Approved — جاهز للتفويض إلى EngineeringAgent |
| 2026-07-13 | ✅ Delegated to EngineeringAgent |
| 2026-07-13 | ✅ Handback received — Build: 0 Errors, 8 Entities, 1 Migration, SQLite DB ready |
| 2026-07-13 | 🟢 **Post-Execution Review: PASS** |
