# 28_UI_UX_GUIDELINES.md

> Final executable UI/UX design rules for this project.  
> EngineeringAgent must implement UI from this file, not from guesses or raw references alone.

## 1. Design Source Decision

| Field | Value |
|---|---|
| Design Source Mode | INTERNAL_TERA_KIT / GETDESIGN_MD / FIGMA_DESIGN_FILE / USER_PROVIDED_REFERENCE / EXTERNAL_URL_ANALYSIS / HYBRID / NO_UI / N/A |
| Selected Source | TBD |
| Source Status | Draft / Approved / Needs Decision / N/A |
| Design Governance Level | None / Compact / Full |
| Tera Decision | TBD |

## 2. Approved Design Direction

- Visual style:
- Product mood:
- Target users:
- Density: Spacious / Balanced / Dense
- Primary direction: RTL / LTR / Both
- Primary language:
- Notes:

## 3. Raw Design Sources

Raw sources are stored or summarized in:

```text
project-preparation/design-source/
```

| Source | Type | Path / Link | Used? | Notes |
|---|---|---|---|---|---|
|  | getdesign.md / Figma file / screenshot / CSS / URL / client notes |  | Yes / No |  |

## 4. Client Branding Overrides

| Item | Approved Value | Notes |
|---|---|---|
| Primary color |  |  |
| Secondary color |  |  |
| Accent color |  |  |
| Logo usage |  |  |
| Font preference |  |  |
| Forbidden colors/styles |  |  |

## 5. Design Tokens

```yaml
colors:
  background:
  surface:
  surface-muted:
  border:
  border-strong:
  text:
  text-muted:
  primary:
  primary-hover:
  secondary:
  accent:
  success:
  warning:
  danger:
  info:

typography:
  font-family-ar:
  font-family-en:
  h1:
  h2:
  h3:
  body:
  small:
  button:

spacing:
  base-unit:
  scale:

radius:
  sm:
  md:
  lg:
  xl:
  full:

shadow:
  none:
  sm:
  md:
  lg:

motion:
  duration-fast:
  duration-normal:
  easing-standard:
```

## 6. Layout System

- Approved layout pattern:
- Navigation pattern:
- Sidebar: None / Fixed / Collapsible / Drawer
- Topbar: None / Simple / Action-heavy
- Content width:
- Card/grid rules:
- Table workspace rules:
- Form layout rules:

## 7. Component Rules

| Component | Variants | States | Rules | Forbidden |
|---|---|---|---|---|
| Button |  | hover/focus/disabled/loading |  |  |
| Input |  | focus/error/disabled |  |  |
| Table |  | empty/loading/error |  |  |
| Card |  | hover/active |  |  |
| Sidebar |  | expanded/collapsed/active |  |  |
| Topbar |  | sticky/scroll |  |  |
| Modal/Drawer |  | open/close/error |  |  |
| Alert/Toast |  | success/warning/danger/info |  |  |

## 8. RTL/LTR Rules

- Primary direction:
- Mirroring required:
- Sidebar side:
- Icon mirroring rules:
- Number/code direction:
- CSS logical properties required: Yes / No

## 9. Responsive Rules

| Breakpoint | Behavior |
|---|---|
| Mobile |  |
| Tablet |  |
| Desktop |  |
| Wide |  |

## 10. Accessibility Rules

- Contrast requirement:
- Keyboard focus requirement:
- Form label/error requirement:
- Color-only indicators allowed? No / Exception:
- Touch target notes:
- Loading/empty/error state requirement:

## 11. Motion Rules

- Motion style:
- Durations:
- Easing:
- Reduced motion handling:
- Forbidden motion:

## 12. Forbidden Styling

- Do not invent colors outside approved tokens.
- Do not invent spacing scale outside approved tokens.
- Do not introduce unapproved UI libraries or design systems.
- Do not mix unrelated visual styles.
- Do not add dark mode unless approved.
- Do not copy a famous brand identity literally.

## 13. Engineering Implementation Instructions

EngineeringAgent must follow this priority order:

1. `project-preparation/28_UI_UX_GUIDELINES.md`
2. `project-preparation/design-source/DESIGN.md` only when referenced here
3. Existing implemented component patterns
4. `tera-system/design-system/` as fallback/reference only

If a required UI rule is missing, EngineeringAgent must raise a `Design Gap` instead of guessing.

## 14. UI Acceptance Checklist

| Check | Result | Notes |
|---|---|---|
| Design Source Decision exists | PASS / FAIL |  |
| Tokens followed | PASS / FAIL |  |
| Layout pattern followed | PASS / FAIL |  |
| Component rules followed | PASS / FAIL |  |
| RTL/LTR respected | PASS / FAIL / N/A |  |
| Responsive behavior respected | PASS / FAIL / N/A |  |
| Accessibility baseline met | PASS / FAIL |  |
| Forbidden styling avoided | PASS / FAIL |  |
| Design gaps recorded instead of guessed | PASS / FAIL / N/A |  |

## 15. Figma Source Mapping

This section is required only when `Design Source Mode = FIGMA_DESIGN_FILE`.

| Item | Value / Notes |
|---|---|
| Figma file / link |  |
| Pages / Frames used |  |
| Components mapped |  |
| Variants / States documented |  |
| Variables / Tokens extracted |  |
| Responsive breakpoints in Figma |  |
| Missing states (loading, empty, error) |  |
| Responsive assumptions |  |
| Figma fonts available in stack? | Yes / No / Gap |
| Figma components replicable in chosen framework? | Yes / Partially / Gap |
| Design Gaps from Figma analysis |  |

## 16. Open Design Gaps

| Gap ID | Gap | Impact | Decision Needed | Status |
|---|---|---|---|---|
| DG-001 |  | Low / Medium / High |  | Open / Resolved / Deferred |
