# TASK-COD-021 — Refactor QuotationForm → Master-Detail Workspace

## Objective
Restructure QuotationForm from the current complex table layout into the approved Calm Quotation Workspace: header with status, guidance strip, right panel (details + suppliers), center panel (materials list + add material), left/collapsible pricing panel, and sticky action bar. Implement all feedback states (empty, loading, incomplete, complete).

## Related Design Source
- `28_UI_UX_GUIDELINES.md` (§4.3 Layout, §6.3 Screen Rules, §5 Components)
- Insights from UI Designer blueprint + Design Reviewer (Master-Detail)
- Majed confirmed: "نوع القطعة" = brand (أصلي/تجاري/بديل), client name from Settings, all screens in one plan

## Reference Files
- `Views/QuotationFormView.xaml` + `.cs`
- `ViewModels/QuotationFormViewModel.cs`
- `ViewModels/QuotationDetailViewModel.cs`
- `Views/QuotationDetailView.xaml` + `.cs`
- `Resources/` (TASK-COD-020 outputs)
- `Models/QuotationItem.cs`
- `Services/QuotationService.cs`

## Dependencies
TASK-COD-020 (Design Resources) must be complete first.

## Acceptance Criteria
1. RTL three-zone layout: right (details + suppliers) / center (materials) / left (pricing)
2. Header: quote number, date, status badge, last save indicator, back button
3. Guidance strip: single-line contextual hint
4. Right panel: quote details card + supplier cards with pricing status
5. Center panel: add material form with labels + materials list with status per item
6. Left panel: pricing per selected material with brand dropdown, price field, best price highlight
7. Sticky action bar: save, print without prices, print final, PDF, Outlook
8. Disabled states with explanation tooltips
9. States: empty (no materials), loading, incomplete, complete
10. Visual: colors from design tokens, no hardcoded hex, proper RTL
11. Existing logic (save, print, export) preserved — only UI changes

## Allowed Write Targets
- `source/TeraQuotation/Views/QuotationFormView.xaml`
- `source/TeraQuotation/Views/QuotationFormView.xaml.cs`
- `source/TeraQuotation/ViewModels/QuotationFormViewModel.cs`
- `source/TeraQuotation/Converters/`
- `source/TeraQuotation/Resources/`
- `source/TeraQuotation/Services/INavigationService.cs` (if navigation needs alignment)

## Forbidden Actions
- Change database schema or migrations
- Add new NuGet packages
- Change data models (Models/*.cs)
- Break existing print/export/save functionality

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | One screen, entire restructure as one task |
| One objective only | PASS | Refactor QuotationForm |
| No deferrable work included | PASS | All elements needed per approved blueprint |
| No scope expansion | PASS | Within approved app scope |
| No API/Auth/DB added | PASS | N/A |
| No real secrets involved | PASS | N/A |
| Allowed Write Targets narrow | PASS | Limited to quotation-related files |
| Acceptance criteria testable | PASS | Build + visual check + states test |
| Linked to 28_UI_UX_GUIDELINES.md | PASS | Yes — screens rules + design tokens |

**Gate Status: PASS**

## Vitality & Polish Checklist
[ ] ✅ — Skeleton Loading / Shimmer for data loading states
[ ] ✅ — Toast Notifications for save/add/error actions
[ ] ✅ — Connection Status Indicator — database connectivity indicator
[ ] N/A — Search in table (materials have search in add flow)
[ ] ✅ — Micro-animations — hover effects, entrance transitions for panels
[ ] ✅ — Empty States for materials, suppliers, pricing panel
[ ] ✅ — Realistic Data — placeholder data reflects real part names and prices

## Model Tier
- Task Complexity: High (restructuring major screen)
- Risk Level: Medium (existing logic must be preserved)
- Recommended: Current model sufficient with step-by-step execution
- Safeguards: Build check after each feature block, preserve existing services
