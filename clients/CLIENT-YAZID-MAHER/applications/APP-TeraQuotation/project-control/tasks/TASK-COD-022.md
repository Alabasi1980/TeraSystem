# TASK-COD-022 — Apply Design Tokens to Login + Settings Screens

## Objective
Apply the new design system tokens and UI guidelines to Login screen and Settings screen. Add field labels, helper text, empty states, and consistent visual styling per the approved design.

## Related Design Source
- `28_UI_UX_GUIDELINES.md` (§6.1 Login, §6.2 Settings, §3 Tokens, §5 Components)

## Reference Files
- `Views/LoginView.xaml`
- `Views/SettingsView.xaml`
- `ViewModels/SettingsViewModel.cs`
- `Resources/` (TASK-COD-020 outputs)
- `Views/ChangePasswordDialog.xaml`

## Dependencies
TASK-COD-020 (Design Resources) must be complete.

## Acceptance Criteria
### Login:
1. Centered card with title, subtitle, labeled password field
2. "دخول" button full-width, clear text
3. Error message below field, not floating
4. Loading state on button
5. Design tokens applied (no hardcoded hex)

### Settings:
1. 4 tabs: الموردين, القطع, التوقيعات, الترويسة
2. Each tab has card-based layout with section title
3. Field labels: اسم المورد, رقم الهاتف, ملاحظات (etc.)
4. Add form at top with labels (not bare textboxes)
5. Empty states: "لا يوجد موردون بعد. أضف مورداً جديداً."
6. Tables styled with alternating rows, no large grey empty areas
7. Delete buttons properly styled as Danger
8. Letterhead tab: labels for all fields, save button styled as Primary
9. ChangePasswordDialog: consistent styling with main screens

## Allowed Write Targets
- `source/TeraQuotation/Views/LoginView.xaml`
- `source/TeraQuotation/Views/LoginView.xaml.cs`
- `source/TeraQuotation/Views/SettingsView.xaml`
- `source/TeraQuotation/Views/ChangePasswordDialog.xaml`
- `source/TeraQuotation/Converters/`

## Forbidden Actions
- Change business logic or data flow
- Add new features or screens
- Modify Services, Models, or Data layer

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Two related screens, same scope (polish) |
| One objective only | PASS | Apply design tokens to secondary screens |
| No deferrable work included | PASS | All required per plan |
| No scope expansion | PASS | Existing screens only |
| No API/Auth/DB added | PASS | N/A |
| No real secrets involved | PASS | N/A |
| Allowed Write Targets narrow | PASS | Views only |
| Acceptance criteria testable | PASS | Visual check + build |
| Linked to 28_UI_UX_GUIDELINES.md | PASS | Yes |

**Gate Status: PASS**

## Vitality & Polish Checklist
[ ] N/A — Skeleton Loading (small local screens, fast load)
[ ] ✅ — Toast Notifications for add/delete/save actions
[ ] ✅ — Empty States for suppliers/items/signatures tabs
[ ] N/A — Search real (items tab has search, preserve it)
[ ] ✅ — Micro-animations — hover effects on buttons and table rows
[ ] ✅ — Empty States for all sections without data
[ ] N/A — Realistic Data (data comes from user input)

## Model Tier
- Task Complexity: Low-Medium
- Risk Level: Low
- Recommended: Current model sufficient
