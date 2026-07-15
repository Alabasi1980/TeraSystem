# TASK-COD-023 — Apply Design Tokens to QuotationList + Reports Screens

## Objective
Apply the new design system tokens and UI guidelines to Quotation List and Reports screens. Improve filtering display, status badges, empty states, and clean up excessive colors in reports.

## Related Design Source
- `28_UI_UX_GUIDELINES.md` (§6.4 QuotationList, §6.5 Reports, §3 Tokens, §5 Components)

## Reference Files
- `Views/QuotationListView.xaml`
- `Views/QuotationDetailView.xaml`
- `Views/ReportsView.xaml`
- `Resources/` (TASK-COD-020 outputs)

## Dependencies
TASK-COD-020 (Design Resources) must be complete.

## Acceptance Criteria
### QuotationList:
1. Header with title + "عرض جديد" button
2. Search/filter bar clearly separated (search text, status filter, date range)
3. Results area: status badges use new design tokens (not raw colors)
4. Empty state: "لا توجد عروض أسعار بعد. اضغط 'عرض جديد' لإنشاء أول عرض."
5. Consistent card/row styling
6. Status colors from design system (not hardcoded)

### Reports:
1. Report selection as clean cards, not multi-colored buttons
2. Each report card shows name + description
3. Active report visually distinguished
4. Results area: clean table, no excessive hex colors
5. Summary cards in stats report: use design token backgrounds (SuccessBg, WarningBg, InfoBg)
6. Empty state: "اختر تقريراً لعرض النتائج"
7. Print/PDF buttons styled consistently (Secondary variant)

### QuotationDetailView:
1. Apply design tokens
2. Consistent styling with QuotationForm (but simpler — read-only)

## Allowed Write Targets
- `source/TeraQuotation/Views/QuotationListView.xaml`
- `source/TeraQuotation/Views/QuotationDetailView.xaml`
- `source/TeraQuotation/Views/ReportsView.xaml`
- `source/TeraQuotation/Converters/`

## Forbidden Actions
- Change business logic or data flow
- Add new features or screens
- Modify Services, Models, or Data layer
- Change report calculation logic

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Remaining screens grouped by similar scope |
| One objective only | PASS | Apply design tokens to remaining non-QuotationForm screens |
| No deferrable work included | PASS | All required per plan |
| No scope expansion | PASS | Existing screens only |
| No API/Auth/DB added | PASS | N/A |
| No real secrets involved | PASS | N/A |
| Allowed Write Targets narrow | PASS | Views only |
| Acceptance criteria testable | PASS | Visual check + build |
| Linked to 28_UI_UX_GUIDELINES.md | PASS | Yes |

**Gate Status: PASS**

## Vitality & Polish Checklist
[ ] N/A — Skeleton Loading (existing loading covers needs)
[ ] ✅ — Toast Notifications for delete/PDF/print actions
[ ] ✅ — Empty States for QuotationList (no quotations)
[ ] N/A — Search real (list has search, preserve it)
[ ] ✅ — Micro-animations — hover effects on rows and buttons
[ ] ✅ — Empty States for reports (no data)
[ ] N/A — Realistic Data

## Model Tier
- Task Complexity: Low-Medium
- Risk Level: Low
- Recommended: Current model sufficient
