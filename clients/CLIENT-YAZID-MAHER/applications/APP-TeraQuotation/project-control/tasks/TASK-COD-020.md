# TASK-COD-020 — Create WPF Design System Resources

## Objective
Create centralized WPF design resources: ResourceDictionary with design tokens, common converters, reusable styles, and shared UI components (EmptyState, StatusBadge, Toast helpers).

## Related Design Source
- `28_UI_UX_GUIDELINES.md` (Design Tokens §3, Component Rules §5)

## Reference Files
- `Views/*.xaml` (existing screens to identify style extraction needs)
- `Converters/*.cs`
- `App.xaml`

## Acceptance Criteria
1. ResourceDictionary file with all color, spacing, radius, typography tokens exists in `Resources/`
2. Common converters in `Converters/`
3. Shared styles for buttons, cards, fields, status badges in `Resources/`
4. EmptyState control/template usable from any view
5. Toast helper ready for integration
6. App.xaml references the new ResourceDictionary
7. Project builds without errors

## Allowed Write Targets
- `source/TeraQuotation/Resources/`
- `source/TeraQuotation/Converters/`
- `source/TeraQuotation/App.xaml`

## Forbidden Actions
- Modify existing Views, ViewModels, Services, Models
- Add new NuGet packages
- Change business logic
- Modify Data layer or DbContext

## Pre-Execution Gate Result

| Check | Result | Notes |
|---|---|---|
| Smallest safe executable unit | PASS | Design resources only — no screen changes |
| One objective only | PASS | Create centralized design system |
| No deferrable work included | PASS | Everything here is prerequisite for UI tasks |
| No UI/module scope expansion | PASS | Only shared resources, not screens |
| No API/Auth/DB added | PASS | N/A |
| No real secrets involved | PASS | N/A |
| Allowed Write Targets narrow | PASS | Limited to Resources/ + Converters/ + App.xaml |
| Acceptance criteria testable | PASS | Build check + grep for tokens |
| Linked to 28_UI_UX_GUIDELINES.md | PASS | Yes |

**Gate Status: PASS**

## Vitality & Polish Checklist
[ ] N/A — This is design infrastructure, not a user-visible screen

## Model Tier
- Task Complexity: Medium
- Risk Level: Low
- Recommended: Current model sufficient
