# DESIGN.md Integration Protocol

## Purpose

Use `getdesign.md` / DESIGN.md as an approved external design-system source, without making it mandatory or allowing direct brand copying.

## Status

```text
getdesign.md = Approved External Design System Source
getdesign.md != mandatory design source
getdesign.md != final executable project design
```

## Workflow

1. Identify the application type and visual need.
2. Search/select up to 3 suitable DESIGN.md options.
3. Present style qualities to the user, not brand imitation promises.
4. After selection, save raw source in:

```text
project-preparation/design-source/DESIGN.md
project-preparation/design-source/DESIGN_SOURCE_NOTES.md
```

5. Convert raw DESIGN.md into:

```text
project-preparation/28_UI_UX_GUIDELINES.md
```

6. Apply client branding overrides.
7. Record forbidden styling and brand-imitation risks.
8. EngineeringAgent implements only from `28_UI_UX_GUIDELINES.md`.

## Required DESIGN_SOURCE_NOTES.md

```text
Selected source:
Reason:
Useful patterns:
Excluded patterns:
Client color/font overrides:
Brand imitation risk:
License/legal notes if known:
Final decision by Tera:
```

## Prohibited Use

- Do not copy a famous brand identity literally.
- Do not promise the client a design “like Stripe/Linear/Apple” as a deliverable identity.
- Do not let EngineeringAgent execute directly from raw DESIGN.md without `28_UI_UX_GUIDELINES.md`.
- Do not use a DESIGN.md that conflicts with client scope, accessibility, RTL/LTR needs, or approved Technology Profile.

## getdesign.app

`getdesign.app` / URL extraction may be used only as optional External URL Analysis when available and approved. It is not a mandatory dependency.
