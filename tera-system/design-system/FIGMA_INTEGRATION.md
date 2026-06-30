# Figma Integration Protocol

## Purpose

Define how Tera handles a Figma design file as a formal design source when `Design Source Mode = FIGMA_DESIGN_FILE`.

## Core Rule

```text
No frontend implementation directly from Figma.
Always extract executable rules into 28_UI_UX_GUIDELINES.md first.
```

## When to Use FIGMA_DESIGN_FILE

Use `FIGMA_DESIGN_FILE` instead of `USER_PROVIDED_REFERENCE` when the client provides:

- A Figma file (link, `.fig` file, or exported frames).
- Structured screenshots showing component variants, states, or auto-layout.
- Figma variables or design tokens.
- A design system or component library defined in Figma.

Do NOT use `FIGMA_DESIGN_FILE` for:

- A single screenshot without structure (use `USER_PROVIDED_REFERENCE`).
- A reference website or inspiration link (use `EXTERNAL_URL_ANALYSIS`).

## Flow

```text
Client provides Figma file / link / structured exports
        ↓
Tera records Design Source Mode = FIGMA_DESIGN_FILE
        ↓
UIVisualDesignerAgent analyzes the Figma source:
  - Extracts colors, typography, spacing
  - Maps frames/pages to project screens
  - Documents component variants and states
  - Identifies layout patterns and constraints
  - Notes responsive assumptions
  - Records missing or unclear design areas as Design Gaps
        ↓
Updates project-preparation/28_UI_UX_GUIDELINES.md with:
  - Section 3: Figma file reference
  - Section 15: Figma Source Mapping
  - Sections 5-8: Tokens, components, layout, RTL rules
        ↓
EngineeringAgent implements from 28_UI_UX_GUIDELINES.md only
        ↓
UI Acceptance Gate verifies against Figma-derived rules
```

## Required User Input for Official Figma Adoption

When the user says that Figma is the official design source for a project, Tera must ask for the following information before analysis or UI execution planning:

```text
I will treat Figma as the official design source only after you provide or confirm:

1. Figma link / file / exported frames:
2. Approved pages / frames / screens:
3. Frames that must NOT be used:
4. Commitment level: exact / close match / guided adaptation:
5. Interface direction: RTL Arabic / LTR English / both:
6. What to extract: colors, typography, spacing, layout rules, component rules, states, responsive assumptions, RTL/LTR behavior:
7. Missing states to check: empty / loading / error / disabled / mobile / tablet:
8. Restrictions: no API, no MCP, no plugin, no direct EngineeringAgent implementation from Figma:
9. Any project identity or brand overrides that should override Figma:
```

Tera must not expect the user to remember this checklist. If `FIGMA_DESIGN_FILE` is selected and any required item is missing, Tera asks only for the missing items.

## Official Figma Adoption Prompt

Tera may offer the user this ready-to-fill prompt:

```text
أريد اعتماد Figma كمصدر تصميم رسمي لهذا المشروع.

Design Source Mode:
FIGMA_DESIGN_FILE

رابط Figma:
[ضع الرابط هنا]

نطاق التصميم المعتمد:
- الصفحات/Frames المطلوبة: [اكتب أسماء الصفحات أو الشاشات]
- لا تعتمد أي Frames أخرى خارج هذه القائمة إلا بعد سؤالي.

درجة الالتزام:
- استخدم Figma كمصدر تصميم أساسي للواجهة.
- لا تنفذ منه مباشرة.
- استخرج منه القواعد إلى project-preparation/28_UI_UX_GUIDELINES.md.
- يجب أن يبقى 28_UI_UX_GUIDELINES.md هو المرجع التنفيذي النهائي.

المطلوب استخراجه من Figma:
- الألوان
- الخطوط
- spacing
- layout rules
- component rules
- buttons
- forms
- tables
- sidebar/topbar إن وجدت
- component states مثل hover / active / disabled إن وجدت
- responsive assumptions
- RTL/LTR behavior

اللغة والاتجاه:
- الواجهة: [RTL عربي / LTR إنجليزي / الاثنين]
- إذا كان التصميم في Figma باتجاه مختلف، سجّل ذلك كـ Design Gap ولا تخمّن.

قيود مهمة:
- لا تضف Figma API أو MCP أو Plugin الآن.
- لا تسمح لـ EngineeringAgent بتنفيذ UI مباشرة من Figma.
- إذا كانت هناك حالات ناقصة في التصميم مثل empty/loading/error/disabled/mobile، سجّلها في Open Design Gaps.
- بعد التحليل أعطني ملخصًا بما تم اعتماده، وما هي Design Gaps قبل بدء تنفيذ الواجهة.
```

## UIVisualDesignerAgent Responsibilities

When `FIGMA_DESIGN_FILE` is active, UIVisualDesignerAgent must:

1. Reference the Figma file in `project-preparation/design-source/`.
2. Extract all available design tokens (colors, typography, spacing).
3. Map Figma components to project components.
4. Document component variants, states (hover, focus, disabled, loading, error, empty).
5. Note responsive behavior if the Figma includes multiple frame sizes.
6. Record any missing design details as Design Gaps.
7. Never copy the Figma file as executable spec — the final output is always `28_UI_UX_GUIDELINES.md`.

## Restrictions

- EngineeringAgent must NOT open or read Figma files directly.
- EngineeringAgent must NOT implement UI from Figma screenshots without extracted rules.
- Figma fonts that are not available in the project stack must be noted as Design Gaps.
- Figma components that cannot be replicated exactly in the chosen UI framework must be noted with alternatives.
