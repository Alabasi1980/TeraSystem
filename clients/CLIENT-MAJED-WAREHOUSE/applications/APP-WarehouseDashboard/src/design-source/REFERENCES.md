# Visual References — Card Builder Wizard

> **Research Date:** 2026-07-14  
> **Agent:** ui-designer  
> **Task:** TASK-COD-026 (UI portion)

---

## 📋 Reference Summary

| # | Source | Key Patterns Applied |
|---|--------|---------------------|
| 1 | **Alpomi Custom Card Builder** (alpomi.com) | 4-step guided wizard, live preview, 100+ data fields, 3 viz types |
| 2 | **clariBI Custom Dashboard Card Builder** | 4-step wizard: Source → Visualize → Design → Preview & Save |
| 3 | **Core Dashboard Builder** (Dribbble: Tran Mau Tri Tam) | Drag-drop dashboard builder, modular parts |
| 4 | **Soft UI Dashboard Builder** (Dribbble: Creative Tim) | Bootstrap-based drag-drop builder |
| 5 | **SaaS Builder UI** (Dribbble: Exalt Studio) | Internal tool dashboard & component editor |
| 6 | **Visual Card Builder** (Home Assistant CardBuilder) | Full-screen drag-drop, live entity data, layered sidebar |

---

## 🎯 Key Patterns Applied to This Design

### 1. **4-Step Wizard Flow** (Alpomi, clariBI)
- Step 1: **Type Picker** — Card-style selector with icons (KPI, Bar, Line, Pie, Table, Gauge)
- Step 2: **Source Selector** — Searchable dropdown with categories (Templates, Saved Queries, Tables, Custom SQL)
- Step 3: **Basic Fields** — Minimal required fields only (Title, Display Name, Measurement)
- Step 4: **Visual Settings** — Grid controls, color palette, refresh interval, chart-specific options

### 2. **Live Preview Pane** (Alpomi, CardBuilder)
- Right side (desktop) / Bottom (mobile < 768px)
- Real-time rendering with actual chart components (Syncfusion EJ2)
- Debounced updates (150ms)
- Skeleton loading while preview loads

### 3. **Type Picker — Clickable Cards** (Alpomi, CardBuilder)
- 6 card types displayed as clickable tiles
- Icon + name + hover/selected states
- Single-click selects → enables "Next"
- RTL-friendly card layout

### 4. **Source Selector with Search** (Alpomi, clariBI)
- Categorized dropdown: Templates / Saved Queries / Oracle Tables / Custom SQL
- Template cards show thumbnail + description
- Oracle Tables loaded from API (`/api/tablemappings/active`)
- Search/filter within dropdown

### 5. **Visual Settings — Grid Controls** (CardBuilder, Alpomi)
- Grid Width (1-12), Height (1-6), X/Y position
- Color palette picker using design tokens (--c-primary, --c-secondary, --c-accent, --c-success, --c-warning, --c-info)
- Refresh interval dropdown (Off / 1m / 5m / 15m / 1h)
- Collapsible chart-specific options (Pie labels, Bar stacked, etc.)

### 6. **Advanced Accordion** (Alpomi, CardBuilder)
- Hidden by default, expandable
- Custom SQL textarea (syntax highlight optional)
- Key-value filters
- Drill-down configuration
- Custom labels/tooltips

### 6. **Fixed Bottom Navigation** (Standard wizard pattern)
- Previous / Next / Cancel / Save / Save & Add Another
- Proper disabled states per step validation
- RTL button order (Previous on right, Next on left in RTL)

### 7. **Templates** (Alpomi, clariBI)
- 4-6 pre-built templates covering common warehouse scenarios
- Pre-fill Steps 3-4 when selected
- Stored in `card-templates.js` as static JSON

### 8. **Clone Mode** (clariBI, CardBuilder)
- URL/query parameter detection (`?clone=cardId`)
- Pre-fills all steps from existing card data
- "Save & Add Another" creates new instead of updating

### 9. **Design Tokens from `_CardsLayout`** (Project Standard)
- Colors: `--c-primary`, `--c-secondary`, `--c-accent`, `--c-bg`, `--c-surface`, `--c-border`, `--c-text`, `--c-text-muted`
- Status: `--c-success`, `--c-warning`, `--c-error`, `--c-info`
- Spacing: `--sp-1` through `--sp-12` (4px scale)
- Radius: `--radius-sm` through `--radius-xl`, `--radius-full`
- Shadows: `--shadow-sm`, `--shadow-md`, `--shadow-lg`
- Motion: `--dur-fast` (120ms), `--dur-norm` (240ms), `--ease`
- Font: `--font-ar: "Cairo", "Tajawal", Tahoma, sans-serif`
- RTL: `dir="rtl"`, `lang="ar"`, logical properties

### 10. **Vitality Requirements** (Self-Check Gate)
- ✅ Skeleton loading for preview pane
- ✅ Toast notifications (success/error/warning)
- ✅ Connection status indicator
- ✅ Search in source selector
- ✅ Micro-animations (stagger entries, hover, counters)
- ✅ Empty states for each step
- ✅ Realistic preview data (not lorem ipsum)
- ✅ Prototype feels "alive" not just "functional"

---

## 🎨 Visual Direction Decisions

| Decision | Rationale | Reference |
|----------|-----------|-----------|
| **Blue theme tokens** | Consistent with `_CardsLayout.cshtml` and project guidelines | Project standard |
| **Cairo font, RTL** | Arabic-first design, per project requirements | `_CardsLayout.cshtml` |
| **Card-based type picker** | Visual, clickable, single-click selection | Alpomi, CardBuilder |
| **Right-side preview (desktop)** | Standard wizard pattern, keeps flow linear | Alpomi, clariBI |
| **Bottom preview (mobile < 768px)** | Responsive stack, preview stays visible | Responsive best practice |
| **Advanced accordion collapsed** | Progressive disclosure — 90% users don't need it | Alpomi "Advanced" section |
| **Palette from design tokens** | Consistency with dashboard cards | `_CardsLayout` tokens |
| **Syncfusion EJ2 for preview** | Same library as production dashboard | Project requirement v27.2.3 |
| **Skeleton shimmer on preview** | Perceived performance, vitality | `_CardsLayout` skeleton pattern |
| **Toast notifications** | Feedback on save/clone/error | `_CardsLayout` toast pattern |

---

## 🚫 Patterns Explicitly Avoided

| Pattern | Reason | Alternative Used |
|---------|--------|------------------|
| Drag-drop canvas | Over-engineered for 90% use case; complex on mobile | Click-to-select cards + grid inputs |
| Full YAML/JSON editor | Too technical for target users | Guided fields + Advanced accordion for SQL |
| Side-by-side code/preview | Screen real estate on Arabic RTL | Right/bottom preview pane |
| Multi-step form in modal | Wizard needs full width for preview | Full-page wizard with `_CardsLayout` |
| Color picker (freeform) | Breaks design system consistency | Token-based palette picker |

---

## ✅ Acceptance Criteria Mapping

| AC | Implementation |
|----|----------------|
| AC-1: Create card in ≤ 5 clicks | Type (1) → Template/Source (1) → Title/Field (2) → Save (1) = 5 clicks |
| AC-2: Live preview before save | Right/bottom pane, debounced 150ms, real Syncfusion render |
| AC-3: Templates available | 6 templates in `card-templates.js`, click to pre-fill |
| AC-4: Advanced hidden by default | Accordion collapsed, "خيارات متقدمة" toggle |
| AC-5: No forced SQL for normal users | Custom SQL only in Advanced accordion |
| AC-6: Blue theme, RTL, Cairo | Tokens from `_CardsLayout`, `dir="rtl"`, `font-family: var(--font-ar)` |

---

> **Self-Certification:** All 8 Vitality Self-Check items addressed in implementation plan.
> **References documented:** 6 sources → `design-source/REFERENCES.md` ✅