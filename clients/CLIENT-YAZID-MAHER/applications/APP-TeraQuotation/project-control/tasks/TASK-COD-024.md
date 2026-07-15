# TASK-COD-024 — Add Vitality & UX Features

## Objective
Add global UX vitality features: Toast notification system, unsaved changes detection with exit confirmation, validation messages on forms, and status indicators across the application.

## Related Design Source
- `28_UI_UX_GUIDELINES.md` (§5.6 Toast, §5.7 Confirmation Dialog, §5.2 Validation, §2 UX Principles)

## Reference Files
- `App.xaml.cs`
- `App.xaml`
- `Views/QuotationFormView.xaml.cs`
- All existing View files
- `Helpers/` directory

## Dependencies
TASK-COD-020 + TASK-COD-021 (for QuotationForm-specific integration)

## Acceptance Criteria
1. **Toast system:**
   - Success toast: green background, white text, 3-4s auto-dismiss
   - Error toast: red background, white text, 5-6s auto-dismiss
   - Warning toast: amber background, dark text
   - Non-blocking (no user action needed)
   - Accessible from any ViewModel

2. **Unsaved changes detection:**
   - Warning when navigating away from QuotationForm with unsaved changes
   - Dialog: "لديك تغييرات غير محفوظة. هل تريد المغادرة؟"
   - Options: حفظ ثم خروج / خروج بدون حفظ / إلغاء

3. **Validation messages:**
   - Field-level validation below input (red text, 12px)
   - Form-level validation on save (summary message)
   - Examples: "الرجاء إدخال اسم المادة", "أدخل رقماً صحيحاً"

4. **Status indicators:**
   - Last save timestamp in QuotationForm header
   - Unsaved changes indicator (amber badge)
   - Database connection status (basic connected/disconnected indicator)

## Allowed Write Targets
- `source/TeraQuotation/App.xaml.cs`
- `source/TeraQuotation/App.xaml`
- `source/TeraQuotation/Helpers/`
- `source/TeraQuotation/Views/` (integration points only)
- `source/TeraQuotation/ViewModels/` (base class or interface for toast/validation)

## Forbidden Actions
- Change business logic
- Modify data models or database
- Add new NuGet packages
- Change Services layer logic

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Pure UX infrastructure — cross-cutting |
| One objective only | PASS | Add vitality/feedback features |
| No deferrable work included | PASS | All required for professional feel |
| No scope expansion | PASS | Within approved scope |
| No API/Auth/DB added | PASS | N/A |
| No real secrets involved | PASS | N/A |
| Allowed Write Targets narrow | PASS | Limited to helpers + integration points |
| Acceptance criteria testable | PASS | Test by doing actions + checking feedback |
| Linked to 28_UI_UX_GUIDELINES.md | PASS | Yes — §5.6, §5.7, §2 |

**Gate Status: PASS**

## Vitality & Polish Checklist
[ ] N/A — Skeleton Loading (handled in TASK-COD-021)
[ ] ✅ — Toast Notifications — success/error/warning for all actions
[ ] ✅ — Connection Status Indicator — basic DB connectivity indicator
[ ] N/A — Search in table (existing search preserved from earlier tasks)
[ ] ✅ — Micro-animations — toast entrance, unsaved indicator pulse
[ ] N/A — Empty States (handled in COD-021, 022, 023)
[ ] N/A — Realistic Data

## Model Tier
- Task Complexity: Medium
- Risk Level: Low
- Recommended: Current model sufficient
