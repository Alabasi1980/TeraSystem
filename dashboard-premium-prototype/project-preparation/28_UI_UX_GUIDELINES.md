# 28_UI_UX_GUIDELINES.md — Dynamic Dashboard Builder Prototype

## Design Source Decision
- **Design Source Mode:** `USER_PROVIDED_REFERENCE`
- **Selected Source:** Client-provided color palette (Copper & Burgundy — Classic Premium)
- **Why selected:** Client specified exact color preferences
- **Client overrides:** None
- **Final executable file:** This file (`28_UI_UX_GUIDELINES.md`)

---

## 1. Brand Identity

| Element | Value |
|---------|-------|
| **Style** | Classic Premium / Luxurious / Professional |
| **Vibe** | Trustworthy, established, handcrafted quality |
| **Target audience perception** | "هذا تطبيق جاد لمؤسسة محترمة" |
| **Design inspiration** | Apple HIG, Refactoring UI, Premium dashboard templates |

---

## 2. Color System (HSL)

### 2.1 60% — Background & Surface

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-cream` | `#F5F0E8` | hsl(39, 38%, 94%) | Main page background |
| `--color-cream-dark` | `#E8DCD0` | hsl(30, 34%, 86%) | Section backgrounds, card surfaces |
| `--color-cream-light` | `#FCF9F5` | hsl(39, 44%, 97%) | Elevated surfaces, modal backgrounds |

### 2.2 30% — Cards, Sidebar, Navigation

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-mocha` | `#8B7355` | hsl(33, 24%, 44%) | Sidebar bg, card headers |
| `--color-mocha-light` | `#A0886A` | hsl(33, 22%, 52%) | Hover states, secondary cards |
| `--color-mocha-dark` | `#6B5344` | hsl(23, 22%, 34%) | Active nav items, footer |

### 2.3 Text Colors

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-espresso` | `#2C1810` | hsl(15, 47%, 12%) | Primary text (headings, body) |
| `--color-espresso-light` | `#3D2A1F` | hsl(23, 33%, 18%) | Secondary text, muted headings |
| `--color-espresso-muted` | `#6B5A50` | hsl(22, 14%, 37%) | Muted text, labels, placeholders |

### 2.4 10% — Accent (Buttons, Alerts, Key Elements)

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-copper` | `#CD7F32` | hsl(30, 62%, 50%) | Primary buttons, key CTAs |
| `--color-copper-light` | `#E09140` | hsl(30, 72%, 57%) | Hover state, active accents |
| `--color-copper-dark` | `#B8722A` | hsl(30, 63%, 44%) | Pressed state, borders |
| `--color-burgundy` | `#800020` | hsl(345, 100%, 25%) | Danger buttons, alerts, premium accents |
| `--color-burgundy-light` | `#A00028` | hsl(345, 100%, 31%) | Hover state for burgundy |

### 2.5 Data Visualization Colors

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-olive` | `#6B8E4E` | hsl(88, 29%, 43%) | Positive trends, success indicators |
| `--color-olive-light` | `#7DA55E` | hsl(88, 28%, 51%) | Chart series, light success |
| `--color-orange-soft` | `#D4876A` | hsl(17, 55%, 62%) | Warning, medium trends |
| `--color-orange-soft-light` | `#E09A80` | hsl(17, 62%, 69%) | Chart series |
| `--color-copper-glow` | `#E8A84C` | hsl(38, 75%, 60%) | Sparklines, highlights |
| `--color-golden-olive` | `#B8860B` | hsl(43, 89%, 38%) | Star ratings, premium badges |

### 2.6 Neutrals & Borders

| Token | Hex | HSL | Usage |
|-------|-----|-----|-------|
| `--color-border` | `#E8DCD0` | hsl(30, 34%, 86%) | Subtle borders, dividers |
| `--color-border-dark` | `#D4C4B4` | hsl(30, 27%, 77%) | Stronger borders, table lines |
| `--color-white` | `#FFFFFF` | hsl(0, 0%, 100%) | Modal surface, dropdowns |
| `--color-shadow` | rgba(44, 24, 16, X) | — | Shadows: 0.08 light, 0.12 hover |

### 2.7 Chart Color Scale (Sequential)

```
Chart-1: #CD7F32 (Copper)
Chart-2: #D4876A (Soft Orange)
Chart-3: #6B8E4E (Olive)
Chart-4: #800020 (Burgundy)
Chart-5: #A0886A (Mocha Light)
Chart-6: #B8860B (Golden Olive)
```

---

## 3. Typography

### 3.1 Font Family

| Role | Font | Fallback |
|------|------|----------|
| **Display / Headings** | `Playfair Display` | `Noto Sans Arabic`, serif |
| **Body / UI** | `Inter` | `Noto Sans Arabic`, system-ui, sans-serif |
| **Arabic fallback** | `Noto Sans Arabic` | — |

### 3.2 Type Scale

| Token | Size | Line Height | Weight | Usage |
|-------|------|-------------|--------|-------|
| `--text-display` | 48px / 3rem | 1.1 | 700 | Hero / welcome title |
| `--text-h1` | 36px / 2.25rem | 1.2 | 700 | Page titles |
| `--text-h2` | 28px / 1.75rem | 1.25 | 600 | Section headers |
| `--text-h3` | 22px / 1.375rem | 1.3 | 600 | Card titles |
| `--text-h4` | 18px / 1.125rem | 1.4 | 600 | Sub-section titles |
| `--text-body-lg` | 16px / 1rem | 1.6 | 400 | Body text large |
| `--text-body` | 14px / 0.875rem | 1.5 | 400 | Body text default |
| `--text-small` | 12px / 0.75rem | 1.4 | 400 | Captions, labels |
| `--text-tiny` | 11px / 0.6875rem | 1.3 | 500 | Badges, KPIs |

### 3.3 Arabic Typography Notes
- Arabic text needs ~10-15% larger font size for readability
- Line-height for Arabic: 1.6–1.8 (longer than Latin due to character height)
- Use `Noto Sans Arabic` for clean, modern Arabic rendering
- No `letter-spacing` for Arabic text

---

## 4. Spacing System

Base unit: 4px

| Token | Value | Usage |
|-------|-------|-------|
| `--space-1` | 4px | Micro spacing |
| `--space-2` | 8px | Tight spacing, icon margins |
| `--space-3` | 12px | Element padding (small) |
| `--space-4` | 16px | Standard padding |
| `--space-5` | 20px | Button padding H |
| `--space-6` | 24px | Card padding |
| `--space-8` | 32px | Section padding |
| `--space-10` | 40px | Large spacing |
| `--space-12` | 48px | Page section margin |
| `--space-16` | 64px | Major section padding |
| `--space-20` | 80px | Page top padding |

### Layout Grid
- Dashboard content: 12-column grid
- Card grid: 3-4 columns (depending on card size)
- Sidebar: 280px fixed width (desktop)
- Content max-width: 1440px

---

## 5. Cards (The Most Important Element)

| Property | Value |
|----------|-------|
| Background | `var(--color-cream-dark)` or `var(--color-white)` |
| Border-radius | 12px |
| Padding | `var(--space-6)` (24px) |
| Shadow (rest) | `0 2px 8px rgba(44, 24, 16, 0.08)` |
| Shadow (hover) | `0 4px 16px rgba(44, 24, 16, 0.12)` + translateY(-2px) |
| Border | None (use shadow instead) |
| Optional accent | 3px top border in `var(--color-copper)` or `var(--color-burgundy)` |

### KPI Card Specific
- Large value number: `--text-h1` or `--text-display` weight 700
- Label: `--text-small` weight 500, `--color-espresso-muted`
- Change indicator: colored badge (green for up, burgundy for down)
- Icon: 24px, subtle opacity, top-right or left of value
- Sparkline or mini-chart optional at bottom

---

## 6. Buttons

| Variant | BG | Text | Hover | Pressed |
|---------|----|------|-------|---------|
| **Primary** | `--color-copper` | White | `--color-copper-light` | `--color-copper-dark` |
| **Secondary** | transparent | `--color-copper` | bg: `--color-copper` at 10% | bg: 15% |
| **Burgundy** | `--color-burgundy` | White | `--color-burgundy-light` | darker |
| **Ghost** | transparent | `--color-espresso` | bg: `--color-cream-dark` | bg: darker |
| **Outline** | transparent | `--color-mocha` | border + bg tint | fill |

- Border-radius: 8px
- Padding: 12px 24px (default), 8px 16px (small)
- Transition: 0.2s ease
- Font weight: 500
- With icon: 8px gap between icon & text

---

## 7. Sidebar & Navigation

| Property | Value |
|----------|-------|
| Background | `--color-espresso` (very dark) |
| Width | 280px |
| Item padding | 12px 20px |
| Item border-radius | 8px |
| Active item | `--color-copper` accent bar on right (RTL) |
| Active bg | `rgba(205, 127, 50, 0.15)` |
| Hover bg | `rgba(255, 255, 255, 0.05)` |
| Text color (inactive) | `#B8A89A` (muted cream) |
| Text color (active) | `--color-copper` |
| Logo area | 64px height, centered logo |
| Divider | `rgba(255, 255, 255, 0.08)` |

---

## 8. Header / Top Bar

| Property | Value |
|----------|-------|
| Background | `var(--color-cream)` (blur with backdrop-filter) |
| Height | 64px |
| Border-bottom | `1px solid var(--color-border)` |
| Breadcrumbs | `--text-small`, `--color-espresso-muted` |
| User avatar | 36px circle |
| Notification badge | `--color-burgundy` dot |

---

## 9. Charts (Recharts)

Use `--color-copper` as the primary chart color.
All chart colors from the Data Visualization palette (§2.5).

### Chart Types
- **Line chart:** Smooth curved lines, gradient fill below, dot on hover
- **Bar chart:** Rounded bars (radius 4px), subtle grid lines
- **Pie chart:** Donut style with center label
- **Area chart:** Gradient fill, transparency 0.1-0.3

### Chart Styling
- Grid lines: `#E8DCD0` at 0.5 opacity
- Tooltip: white bg, 8px radius, shadow, espresso text
- Legend: `--text-small`, `--color-espresso-muted`
- No 3D, no unnecessary effects

---

## 10. Modal / Dialog

| Property | Value |
|----------|-------|
| Overlay | rgba(44, 24, 16, 0.5) |
| Content bg | white |
| Border-radius | 16px |
| Max-width | 800px (analytic), 1100px (wide detail) |
| Padding | 32px |
| Animation | Scale + Fade (0.3s ease) |
| Close button | top-right (LTR) / top-left (RTL) |
| Tabs inside modal | Underline style, 3 tabs max |

---

## 11. Tables

| Property | Value |
|----------|-------|
| Header bg | `--color-cream-dark` |
| Header text | `--color-espresso` weight 600 |
| Row hover | `rgba(205, 127, 50, 0.04)` |
| Border | `1px solid var(--color-border)` |
| Border-radius | 8px (on container) |
| Padding | 12px 16px |
| Font size | `--text-body` (14px) |
| Pagination | Centered, copper accent on active |

---

## 12. Filters Bar

| Property | Value |
|----------|-------|
| Background | `var(--color-cream-dark)` |
| Border-radius | 12px |
| Padding | 16px 20px |
| Gap between filters | 12px |
| Input style | Border `1px solid var(--color-border-dark)`, radius 8px, bg white |
| Date picker | Copper accent, dropdown style |

---

## 13. RTL / LTR Rules

- **Default direction:** RTL (Arabic-first)
- Use CSS logical properties: `margin-inline-start` instead of `margin-left`
- Padding/margin flipped automatically via Tailwind RTL support
- Sidebar: `right: 0` in RTL
- Icons: mirror for directional icons in RTL (e.g., arrows)
- Text alignment: body text right-aligned in RTL
- Numbers: always left-aligned in table cells (even in RTL)

---

## 14. Animations & Transitions

| Element | Animation | Duration | Easing |
|---------|-----------|----------|--------|
| Card hover | translateY(-2px) + shadow | 0.3s | ease-out |
| Sidebar item | bg color + border | 0.2s | ease |
| Modal open | scale(0.95→1) + opacity(0→1) | 0.3s | ease-out |
| Page transition | fade in | 0.4s | ease |
| Counter (KPI) | count up (optional) | 1s | ease-out |
| Chart animation | grow in | 0.6s | ease-out |

---

## 15. MVP Constraints (for this prototype only)

1. **This is a visual-only prototype** — no real data fetching, no API, no auth
2. Use mock data from static JS objects/arrays
3. No routing library needed — use simple state-based screen switching
4. No TypeScript — plain JSX (fastest path to visual result)
5. Focus visual effort on: Main Dashboard > Modal > Login > Sidebar
6. Do NOT build: full CRUD, real tables, actual forms with validation, backend
7. Colors, spacing, fonts must match this guideline exactly — no inventions
8. Fonts: Google Fonts (Playfair Display + Inter + Noto Sans Arabic)
