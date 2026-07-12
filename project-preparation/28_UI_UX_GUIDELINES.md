# 28_UI_UX_GUIDELINES.md — [Project Name]

> **قالب هيكلي لإرشادات التصميم** — يملأ لكل مشروع حسب مصدر التصميم المعتمد.
> **لا يحتوي هذا الملف محتوى تطبيقياً** — إنه إطار يجب تعبئته من قبل TeraAgent باستخدام `DESIGN_SOURCE_PROTOCOL.md`.

---

## Design Source Decision

| الحقل | القيمة |
|-------|--------|
| **Design Source Mode** | `USER_PROVIDED_REFERENCE` / `INTERNAL_KIT` / `FIGMA_DESIGN_FILE` / `GENERATIVE` |
| **Selected Source** | [رابط أو وصف المصدر] |
| **Why selected** | [سبب اختيار هذا المصدر] |
| **Client overrides** | [أي تعديلات أو إضافات طلبها العميل] |
| **Final executable file** | This file (`28_UI_UX_GUIDELINES.md`) |

---

## 1. Brand Identity

| Element | Value |
|---------|-------|
| **Style** | [وصف الأسلوب العام — مثلاً: Minimal, Premium, Playful, Corporate] |
| **Vibe** | [الشعور المستهدف] |
| **Target audience perception** | [كيف يجب أن يصف الجمهور التطبيق] |
| **Design inspiration** | [مصادر الإلهام — Apple HIG, Refactoring UI, إلخ] |

---

## 2. Color System

### 2.1 60% — Background & Surface

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-` | | | Main background |
| `--color-` | | | Card surface |
| `--color-` | | | Elevated surface |

### 2.2 30% — Sidebar, Navigation, Cards

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-` | | | Sidebar / nav bg |
| `--color-` | | | Hover states |
| `--color-` | | | Active states |

### 2.3 Text Colors

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-text-primary` | | | Primary text |
| `--color-text-secondary` | | | Secondary text |
| `--color-text-muted` | | | Muted / placeholders |

### 2.4 10% — Accent (Buttons, Alerts, Key Elements)

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-primary` | | | Primary buttons, CTAs |
| `--color-primary-hover` | | | Hover state |
| `--color-danger` | | | Danger buttons, alerts |
| `--color-success` | | | Success indicators |

### 2.5 Data Visualization Colors

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-chart-1` | | | Chart series 1 |
| `--color-chart-2` | | | Chart series 2 |
| `--color-chart-3` | | | Chart series 3 |
| `--color-chart-4` | | | Chart series 4 |

### 2.6 Neutrals & Borders

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-border` | | | Subtle borders |
| `--color-border-strong` | | | Strong borders |
| `--color-white` | `#FFFFFF` | | Modal surfaces |
| `--color-shadow` | rgba(...) | | Shadow opacity |

---

## 3. Typography

### 3.1 Font Family

| Role | Font | Fallback |
|------|------|----------|
| **Display / Headings** | | |
| **Body / UI** | | |
| **Arabic fallback** | `Noto Sans Arabic` | — |

### 3.2 Type Scale

| Token | Size | Line Ht | Weight | Usage |
|-------|------|---------|--------|-------|
| `--text-display` | | | | Hero titles |
| `--text-h1` | | | | Page titles |
| `--text-h2` | | | | Section headers |
| `--text-h3` | | | | Card titles |
| `--text-body` | | | | Default body |
| `--text-small` | | | | Captions, labels |

### 3.3 RTL / Arabic Typography Notes
- Arabic text needs ~10-15% larger font size for readability
- Line-height for Arabic: 1.6–1.8
- Use `Noto Sans Arabic` for clean modern Arabic
- No `letter-spacing` for Arabic text

---

## 4. Spacing System

Base unit: 4px / 8px / [other]

| Token | Value | Usage |
|-------|-------|-------|
| `--space-1` | | Micro spacing |
| `--space-2` | | Tight spacing |
| `--space-4` | | Standard padding |
| `--space-6` | | Card padding |
| `--space-8` | | Section padding |
| `--space-12` | | Large section spacing |

### Layout Grid
- [Number]-column grid
- Sidebar width: [N]px
- Content max-width: [N]px

---

## 5. Component Design Rules

### 5.1 [Component Name]
- Rule 1
- Rule 2
- States: default / hover / active / disabled

### 5.2 [Component Name]
- ...

---

## 6. RTL / LTR Rules

> **المرجع الكامل:** `tera-system/design-system/RTL_LTR_RULES.md`

- [قاعدة تحويل方向的 RTL]
- [محاذاة النصوص]
- [أيقونات — flip للاتجاه]

---

## 7. Micro-interactions & Motion

| Element | Animation | Duration | Easing |
|---------|-----------|----------|--------|
| [Element] | [وصف] | [ms] | [easing] |

---

## 8. Responsive Breakpoints

| Breakpoint | Width | Layout Changes |
|------------|-------|----------------|
| Desktop | ≥ 1024px | Full layout |
| Tablet | 768–1023px | Adjusted grid |
| Mobile | < 768px | Single column |

---

## 9. Accessibility Requirements

- [ ] Contrast ratios meet WCAG AA
- [ ] All interactive elements focusable
- [ ] Arabic screen reader support
- [ ] Touch targets ≥ 44px

---

## 10. Vitality & Polish Checklist

> **المرجع:** `DESIGN_REVIEW_STANDARDS.md §10`

- [ ] Skeleton loading states
- [ ] Toast / notification system
- [ ] Connection status indicator
- [ ] Search / filter with feedback
- [ ] Empty states (illustrated)
- [ ] Micro-animations (hover, transition, page)
- [ ] Realistic / meaningful demo data
- [ ] General "aliveness" — التطبيق حي وليس ثابت

---

*هذا القالب يُملأ لكل مشروع بناءً على Design Source Decision. يبقى الملف في `project-preparation/` كمصدر تنفيذي لـ EngineeringAgent.*

*لمشاهدة مثال تطبيقي حي، راجع `dashboard-premium-prototype/project-preparation/28_UI_UX_GUIDELINES.md`.*
