# TERA_PROJECT_DECISION.md — TeraQuotation

> **Phase 2 — Project Decision Formation**
> **تاريخ الإصدار:** 2026-07-13
> **المعتمد:** Majed (Blueprint Confirmation Gate ✅)
> **المصدر:** APPLICATION_BLUEPRINT.md (approved_for_preparation) + TERA_HANDOFF_PACKAGE.md

---

## 1. Project Classification

| البند | القرار |
|:------|:-------|
| **الحجم** | 🟢 **صغير (Small)** — 5 شاشات، 19 ميزة، مستخدم واحد، WPF Desktop |
| **التعقيد التقني** | 🟢 منخفض — SQLite محلي، مستخدم واحد، لا API |
| **مستوى الأمان** | 🟢 أساسي — كلمة مرور فقط |
| **مصدر المعلومات** | كامل — TERA_HANDOFF_PACKAGE.md معتمد + Blueprint معتمد |
| **حاجة للبحث (Domain Intelligence)** | 🟢 لا — كل شيء واضح ومؤكد |

---

## 2. Technology Profile Decision

| القرار | الاختيار | السبب |
|:-------|:---------|:------|
| **لغة البرمجة** | **C# (.NET 8)** | Windows Desktop + WPF + تكامل Outlook |
| **Framework** | **WPF + CommunityToolkit.Mvvm** | الأنسب لتطبيقات Desktop مع طباعة A4 دقيقة |
| **قاعدة البيانات** | **SQLite + Microsoft.Data.Sqlite (أو EF Core)** | خفيفة، محلية، لا تحتاج سيرفر |
| **توليد PDF** | **QuestPDF** (مفتوحة المصدر) | بديل مجاني |
| **تكامل Outlook** | **Microsoft.Office.Interop.Outlook** | مباشر مع Outlook المثبت (مع Fallback PDF) |
| **طباعة A4** | **FixedDocument + PrintDialog** | تحكم دقيق بالتنسيق |
| **المصادقة** | **SHA256/BCrypt لهاش كلمة المرور** | مستخدم واحد — كافٍ وآمن |

**ملاحظة:** لا يوجد Technology Profile جاهز في `tera-system/profiles/` لـ WPF/C#.
سيتم إنشاء مسودة بروفايل جديدة للموافقة عليها قبل أي تنفيذ.

---

## 3. Required Preparation Files (لـ Small — الحد الأدنى الأساسي)

بناءً على حجم المشروع **الصغير**، الملفات المطلوبة قبل التنفيذ:

| الأولوية | الملف | الغرض | الحجم |
|:--------:|:------|:------|:-----:|
| 🥇 عاجل | `08_TECHNICAL_ARCHITECTURE.md` | تثبيت القرارات التقنية النهائية | أساسي |
| 🥇 عاجل | `06_DATA_MODEL_PREPARATION.md` | تصميم قاعدة البيانات + Entities | أساسي |
| 🥇 عاجل | `07_SCREENS_AND_UI_STRUCTURE.md` | تفصيل كل شاشة بالعناصر | أساسي |
| 🥈 مهم | `05_BUSINESS_WORKFLOWS.md` | تفصيل سير العمل (اختصاراً: موجود في Blueprint) | يُنشأ بعد الأساسيات |
| 🥈 مهم | `13_REPORTS_AND_DASHBOARDS.md` | تفصيل التقارير الأربعة | يُنشأ بعد الأساسيات |
| 🥉 قبل التنفيذ | `09_IMPLEMENTATION_PLAN.md` | خطة التنفيذ التفصيلية → يصبح PROJECT_MASTER_PLAN.md | متأخر |
| 🥉 قبل التنفيذ | `10_TESTING_AND_ACCEPTANCE.md` | سيناريوهات الاختبار + معايير القبول | متأخر |
| 🎯 تسليم | `11_DELIVERY_AND_HANDOVER.md` | خطة التسليم للعميل | تسليم |
| 📘 تسليم | `12_USER_MANUAL_DRAFT.md` | دليل المستخدم | تسليم |

### ما لن يُنشأ (مستغنى عنه لمشروع صغير):

| الملف | سبب الاستغناء |
|:------|:-------------|
| `01_PROJECT_BRIEF.md` | ✅ المعلومات في CLIENT_BRIEF.md + APPLICATION_BLUEPRINT.md |
| `02_SCOPE_AND_BOUNDARIES.md` | ✅ كامل في Blueprint (موديولات + حدود) |
| `03_COMPETITOR_ANALYSIS.md` | ❌ غير مطلوب — تطوير داخلي لعميل فردي |
| `04_USER_STORIES_AND_PERSONAS.md` | ❌ مستخدم واحد — لا حاجة لـ Personas |
| `14_API_SPECIFICATION.md` | ❌ لا API — تطبيق محلي |

---

## 4. Required Sub-Agents — الحد الأدنى

| الوكيل | دوره | متى؟ |
|:-------|:-----|:-----|
| **tera-software-designer** | تصميم Technical Architecture + Data Model + UI Structure | 🥇 مرحلة التحضير (فوراً) |
| **EngineeringAgent** | كتابة الكود (C#, WPF, SQLite) | 🥇 مرحلة التنفيذ (Phase 6) |
| **UI Designer** | توجيه تصميم واجهات WPF | 🥈 اختياري — إذا احتاج التصميم تحسينات بصرية |

### ما لن يُنشأ:

| الوكيل | سبب الاستغناء |
|:-------|:-------------|
| **Domain Intelligence / Research Agents** | ❌ المشروع واضح بالكامل |
| **SecurityAgent** | ❌ مستوى الأمان أساسي (مستخدم واحد + كلمة مرور) |
| **QA Agent** | ❌ مشروع صغير — EngineeringAgent يدير QA |
| **Documentation Agent** | ❌ التوثيق بسيط — يُنجز مباشرة |

---

## 5. Key Decisions Summary

| # | القرار | التصنيف | الأساس |
|:-:|:-------|:-------:|:-------|
| 1 | المشروع **Small** | التصنيف | 5 شاشات، ميزات محدودة، مستخدم واحد |
| 2 | WPF + .NET 8 + CommunityToolkit.Mvvm | التقنية | الأنسب لـ Windows Desktop |
| 3 | SQLite (محلي، لا سيرفر) | قاعدة البيانات | خفيف، سهل، مناسب |
| 4 | QuestPDF + FixedDocument | طباعة/PDF | مجاني + تحكم دقيق |
| 5 | الحد الأدنى من ملفات التحضير (8 ملفات) | حجم التحضير | Small ← لا تضخم |
| 6 | 2-3 وكلاء فرعيين | الفريق | فقط الأساسيين |

---

## 6. Next Steps

| الترتيب | الخطوة | المخرجات | الموافقة |
|:-------:|:-------|:---------|:--------:|
| 1 | **إنشاء Technology Profile لـ WPF/C#** | `tera-system/profiles/dotnet-wpf-sqlite.md` ✅ (جاهزة) | ✅ **تمت الموافقة** |
| 2 | **إنشاء PREPARATION_PLAN.md** | خطة التحضير (Phase 3) | ⏳ بعد الموافقة |
| 3 | **تفعيل tera-software-designer** | تنفيذ التحضير (Phase 4) | ⏳ |

---

**إعداد:** TeraAgent
**تاريخ:** 2026-07-13
**الحالة:** ✅ **معتمد من Majed** — 2026-07-13
