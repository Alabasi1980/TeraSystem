# ISSUES_AND_GAPS.md — WarehouseDashboard

> **Purpose:** سجل الفجوات والمشاكل المكتشفة أثناء التنفيذ.

## [2026-07-14 18:15] - GAP_Encoding_AdminSecurePanel

- **Related Task:** TASK-COD-FIX-002
- **Severity:** Medium
- **Summary:** ثلاث صفحات Razor داخل `admin-secure-panel` محفوظة بترميز UTF-16LE/BOM وتظهر في المتصفح كمحتوى عربي مشوّه (mojibake).
- **Affected Files:**
  - `Pages/admin-secure-panel/_ViewStart.cshtml`
  - `Pages/admin-secure-panel/SyncLogs/Index.cshtml`
  - `Pages/admin-secure-panel/SyncSettings/Index.cshtml`
- **Recommended Action:** Normalization to UTF-8 and re-verify rendering in browser.
- **Status:** Open

## [2026-07-14 19:35] - GAP_UI_Builder_JS_Missing

- **Related Task:** TASK-COD-026
- **Severity:** High (blocks Card Builder functionality)
- **Summary:** الـ ui-designer الأول تعطّل أثناء العمل. أنجز قبل التعطل: `Builder.cshtml` (483 سطر)، `Builder.cshtml.cs` (589 سطر)، `card-builder.css` (1028 سطر)، `_CardsLayout.cshtml` موجود. **المفقود:** ملفّا JS اللذان يربطان كل شيء: `wwwroot/js/card-builder.js` و `wwwroot/js/card-templates.js` (كلاهما مُشار إليهما في `Builder.cshtml` الأسطر 462–463 لكن غير موجودين → الصفحة تظهر كـ HTML ثابت بدون تفاعل).
- **Affected Files:**
  - `wwwroot/js/card-builder.js` — ❌ مفقود
  - `wwwroot/js/card-templates.js` — ❌ مفقود
- **Contract Mismatch discovered:** نموذج `Builder.cshtml.cs` (ui-designer) يستخدم أسماء `cardType`/`sourceType`/`measurement`، بينما الـ backend (engineering-agent) يتوقع `CardPreviewRequest { ChartType, DataSourceType, SqlQuery }`. الـ JS يجب أن يبني عقد الـ backend عند استدعاء `/api/dashboard/cardbuilder/preview`. أيضاً `OnGetMeasurementsAsync` في الـ PageModel يستدعي `/api/tablemappings/{id}/columns` وهو **غير موجود** في الـ backend → يجب ألا يعتمد عليه الـ JS.
- **Recommended Action:** إعادة تفويض عميل ui-designer جديد لإنشاء الملفّين فقط مع الالتزام بعقد الـ backend الموثّق في `CardBuilderService.cs` + `CardBuilderModels.cs` + `Pages/Api/Dashboard/CardBuilder.cshtml.cs` + `Pages/Api/TableMappings/Active.cshtml.cs`.
- **Status:** ✅ Resolved — عميل ui-designer جديد أنشأ `card-builder.js` (961 سطر) + `card-templates.js` (102 سطر) والتزم بعقد الـ backend. TASK-COD-026 → Accepted.
