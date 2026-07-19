# DESIGN_TOKENS.md — Warehouse Card Visual Reference

> **Purpose:** Extracted design tokens from the approved prototype, mapped to `blue-theme.css` variables.
> **Prototype Source:** `WAREHOUSE_CARD_PROTOTYPE.html` (created by external AI client)
> **Color Decision (Majed):** Card colors come from Card Builder ColorPalette first; fallback to `blue-theme.css` theme.
> **Date:** 2026-07-19

---

## 1. Color Decision Rule

```text
Priority 1: Card Builder ColorPalette (per-card configured colors)
Priority 2: blue-theme.css theme variables (fallback)
Priority 3: Prototype reference (visual guidance only, NOT color source)
```

The prototype uses colors that are very close to `blue-theme.css`, but the **authoritative color source** is always:
1. The card's own `ColorPalette` (stored in DB, exposed via `window.WD_CARDS[i].colorPalette`)
2. The project theme in `blue-theme.css`

---

## 2. Color Token Mapping

### 2.1 Prototype → blue-theme.css

| Prototype Color | Value | blue-theme.css Variable | Match Quality |
|---|---|---|---|
| Primary Navy | `#09275d` | `--c-primary: #1F4E79` | Close — use theme |
| Primary Blue | `#1f62ce` | `--c-secondary: #2E6DA4` | Close — use theme |
| Bright Blue | `#3379e7` | `--c-accent-soft: #8FBCDE` | Different — use theme |
| Muted Text | `#7e8da9` | `--c-text-muted: #5B7A99` | Close — use theme |
| Border Line | `#dbe3ef` | `--c-border: #D4E2F0` | Very close — use theme |
| Green (positive) | `#10953c` | `--c-success: #1E9E6A` | Close — use theme |
| Red (danger) | `#d51f32` | `--c-error: #D64545` | Very close — use theme |
| Orange (warning) | `#ef9208` | `--c-warning: #E0A106` | Close — use theme |
| Background | `#f4f7fb` | `--c-bg: #F3F7FB` | Very close — use theme |
| Card Surface | `#ffffff` | `--c-surface: #FFFFFF` | Exact match ✅ |

### 2.2 Status Colors (for alert tables, trend badges)

| State | Prototype | blue-theme.css | Usage |
|---|---|---|---|
| Critical/Danger | `#d51f32` | `--c-error: #D64545` | Status badges, negative trends |
| Warning | `#ef9208` | `--c-warning: #E0A106` | Status badges, caution |
| Success/Good | `#10953c` | `--c-success: #1E9E6A` | Status badges, positive trends |
| Info/Neutral | `#2E6DA4` | `--c-info: #2E6DA4` | Informational elements |

---

## 3. Card Structure Tokens

### 3.1 Card Shell

| Token | Prototype Value | blue-theme.css | Notes |
|---|---|---|---|
| Border Radius | `17px` | `--radius-lg: 12px` | Prototype uses larger radius — **use 14px** as compromise |
| Border | `1px solid #9daec74f` | `1px solid var(--c-border)` | Use theme |
| Shadow | `0 16px 35px #12325d21, 0 3px 8px #12325d17` | `--shadow-md: 0 4px 12px rgba(10,37,64,0.08)` | Use theme shadow |
| Background | `linear-gradient(145deg, #fffffffc, #fafcfff5)` | `var(--c-surface)` | Use theme (solid white) |
| Bottom Line | `6px gradient (#0d4ca9 → #3984ee → #0e4ca7)` | N/A | **New: add `.wd-card--accent-line`** |
| Padding (KPI) | `42px 39px 30px` | `--sp-6: 24px` | Use `padding: 28px 24px 20px` |

### 3.2 Icon Tile

| Token | Prototype Value | Notes |
|---|---|---|
| Size | `72px × 72px` | Fixed square |
| Border Radius | `14px` | Slightly less than card |
| Background | `linear-gradient(145deg, #1d64cd 5%, #0b2a62 80%)` | Dark blue gradient |
| Icon Color | `#ffffff` | White |
| Shadow | `0 8px 14px #08265847, inset 0 1px #ffffff5c` | Depth + inner highlight |
| CSS Class | `.wd-card__icon-tile` | **New utility class** |

### 3.3 KPI Metric Block

| Token | Prototype Value | Notes |
|---|---|---|
| Value Font Size | `56px` (compact) / `60px` (inventory) | Large, bold |
| Value Font Weight | `700` | Bold |
| Value Color | `#08265c` | Dark navy — use `var(--c-text)` |
| Value Font | `Arial, sans-serif` | Tabular nums |
| Trend Font Size | `20px` | Medium |
| Trend Positive Color | `var(--c-success)` | Green |
| Trend Negative Color | `var(--c-error)` | Red |
| Comparison Text | `14px`, `var(--c-text-muted)` | Small muted text |

### 3.4 Sparkline

| Token | Prototype Value | Notes |
|---|---|---|
| Height | `105px` | Absolute positioned at bottom |
| Line Stroke | `#135bcc`, `2.25px` | Blue line |
| Fill | `url(#sparkFill)` — gradient from `#2f72df` opacity 0.16 → 0.01 | Subtle fill |
| End Dot | `cx="440" cy="36" r="5" fill="#2264cd"` | Blue dot at last point |
| CSS Class | `.wd-card__sparkline` | **New class** |

### 3.5 Card Note (Info Footer)

| Token | Prototype Value | Notes |
|---|---|---|
| Height | `48px` | Fixed bottom bar |
| Background | `#eff3f9eb` | Semi-transparent muted |
| Border Top | `1px solid #dde4ef99` | Subtle separator |
| Icon | Info circle, `20px` | `var(--c-text-muted)` |
| Text Size | `13px` | Small |
| CSS Class | `.wd-card__note` | **New class** |

### 3.6 Alert Table

| Token | Prototype Value | Notes |
|---|---|---|
| Header BG | `linear-gradient(#f9fbfe, #f0f3f8)` | Subtle gradient |
| Header Height | `41px` | Fixed |
| Row Height | `41px` | Fixed |
| Row Hover | `#f5f9ff` | Light blue |
| Status Dot | `9px circle`, `background: currentColor` | Colored dot |
| Type Badge | Colored text only | No background |
| CSS Class | `.wd-card__table` | **New class** |

### 3.7 Chart Card

| Token | Prototype Value | Notes |
|---|---|---|
| Chart Height | `213px` | Body area |
| Bar Color | `linear-gradient(90deg, #2265d7, #3c7fe7)` | Blue gradient bars |
| Bar Width | `26px` | Fixed |
| Bar Radius | `2px 2px 0 0` | Top corners only |
| Grid Lines | `1px dashed #d7e0ec` | Subtle dashed |
| Footer BG | `#eef3f9e6` | Semi-transparent |
| Footer Height | `42px` | Fixed |
| CSS Class | `.wd-card__chart` | **New class** |

---

## 4. Typography Tokens

| Element | Prototype | Recommendation |
|---|---|---|
| Card Title | `23px`, weight `700` | `font-size: 18px; font-weight: 700` (matches `.wd-card__title`) |
| Card Subtitle | `15px`, `var(--c-text-muted)` | `font-size: 13px; color: var(--c-text-muted)` |
| KPI Value | `56-60px`, weight `700` | `font-size: 48px; font-weight: 700` (scaled down for our cards) |
| Trend Text | `20px`, weight `500` | `font-size: 16px; font-weight: 600` |
| Table Header | `13.5px`, weight `700` | `font-size: 13px; font-weight: 700` |
| Table Body | `13.5px` | `font-size: 13px` |
| Footer/Note | `13px` | `font-size: 13px` |

---

## 5. Spacing Tokens

| Context | Prototype | Recommendation |
|---|---|---|
| Card Padding (KPI) | `42px 39px 30px` | `padding: 28px 24px 20px` |
| Card Padding (Panel) | `14px top, 25px sides` | `padding: 16px 20px` |
| Gap between cards | `28px` | `gap: var(--sp-6)` (24px) |
| Icon → Title gap | `19px` | `gap: var(--sp-4)` (16px) |
| Title → Subtitle gap | `8px` | `margin-top: 4px` |
| KPI Value → Trend gap | `13px` | `margin-top: var(--sp-3)` (12px) |

---

## 6. New CSS Classes to Add

These classes should be added to `blue-theme.css` or a new card-specific stylesheet:

```css
/* === Card Accent Line (bottom gradient) === */
.wd-card--accent-line::after {
    content: "";
    position: absolute;
    inset-inline: 0;
    bottom: 0;
    height: 6px;
    background: linear-gradient(90deg, var(--c-primary), var(--c-secondary) 52%, var(--c-primary));
    border-radius: 0 0 var(--radius-lg) var(--radius-lg);
}

/* === Icon Tile === */
.wd-card__icon-tile {
    width: 64px; height: 64px;
    border-radius: 14px;
    background: linear-gradient(145deg, var(--c-secondary) 5%, var(--c-primary-strong) 80%);
    color: #fff;
    display: grid; place-items: center;
    box-shadow: 0 6px 12px rgba(10, 37, 64, 0.28), inset 0 1px rgba(255,255,255,0.36);
    flex-shrink: 0;
}

/* === KPI Change Badge === */
.wd-kpi__change {
    display: inline-flex; align-items: center; gap: 4px;
    font-size: 16px; font-weight: 600;
    font-variant-numeric: tabular-nums;
}
.wd-kpi__change--up { color: var(--c-success); }
.wd-kpi__change--down { color: var(--c-error); }
.wd-kpi__change--flat { color: var(--c-text-muted); }

/* === KPI Comparison Text === */
.wd-kpi__compare {
    font-size: 13px; color: var(--c-text-muted); margin-top: 4px;
}

/* === Card Note === */
.wd-card__note {
    display: flex; align-items: center; gap: 10px;
    font-size: 13px; color: var(--c-text-muted);
    background: var(--c-surface-muted);
    border-top: 1px solid var(--c-border);
    padding: 10px 14px;
    margin: var(--sp-6) calc(var(--sp-6) * -1) calc(var(--sp-6) * -1);
    border-radius: 0 0 var(--radius-lg) var(--radius-lg);
}

/* === Sparkline Container === */
.wd-card__sparkline {
    width: 100%; height: 80px;
    margin-top: auto;
}
```

---

## 7. Implementation Notes

1. **Do NOT copy prototype colors directly.** Always use `blue-theme.css` variables or card ColorPalette.
2. **The prototype is a visual reference only** — it shows the *layout patterns* and *visual hierarchy*, not exact implementation.
3. **Card heights are dynamic** in our system (controlled by `GridHeight`) — do not hardcode `368px`.
4. **Sparkline must use real data** from `KpiQueryBuilder` — the prototype SVG is static/placeholder.
5. **The icon tile gradient** should use `--c-secondary` and `--c-primary-strong` from the theme, not hardcoded hex values.
6. **RTL is already handled** by `blue-theme.css` and the existing card system.

---

> **Prepared by:** TeraAgent
> **Mode:** Plan Mode — No code written in this document
