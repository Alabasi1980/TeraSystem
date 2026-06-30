---
version: alpha
name: Claude-design-analysis
description: A warm-canvas editorial interface for Anthropic's Claude product. The system anchors on a tinted cream canvas with serif display headlines, warm coral CTAs, and dark navy product surfaces (code editor mockups, model showcase cards). Brand voltage comes from the cream/coral pairing — deliberately warm and humanist where most AI brands use cool blue + slate. Type voice runs a slab-serif display ("Copernicus" / Tiempos Headline) for h1/h2 and a humanist sans for body. The signature Anthropic black-radial-spike mark anchors the wordmark.

colors:
  primary: "#cc785c"
  primary-active: "#a9583e"
  primary-disabled: "#e6dfd8"
  accent-teal: "#5db8a6"
  accent-amber: "#e8a55a"
  canvas: "#faf9f5"
  surface-soft: "#f5f0e8"
  surface-card: "#efe9de"
  surface-cream-strong: "#e8e0d2"
  surface-dark: "#181715"
  surface-dark-elevated: "#252320"
  surface-dark-soft: "#1f1e1b"
  hairline: "#e6dfd8"
  hairline-soft: "#ebe6df"
  ink: "#141413"
  body-strong: "#252523"
  body: "#3d3d3a"
  muted: "#6c6a64"
  muted-soft: "#8e8b82"
  on-primary: "#ffffff"
  on-dark: "#faf9f5"
  on-dark-soft: "#a09d96"
  success: "#5db872"
  warning: "#d4a017"
  error: "#c64545"
---

# Claude Design System Analysis (from getdesign.md)

## Overview

Claude.com is the warmest, most editorial interface in the AI-product category. The base atmosphere is a **tinted cream canvas** (`#faf9f5`) — distinctly warm, deliberately not the cool gray-white that every other AI brand uses. Headlines run a **slab-serif display** ("Copernicus" / Tiempos Headline) at weight 400 with negative letter-spacing, paired with **StyreneB / Inter** body sans. The combination feels like a literary publication, not a SaaS marketing page.

Brand voltage comes from the **cream + coral pairing** — coral (`#cc785c`) is the signature Anthropic accent, used on every primary CTA, on the brand wordmark, and on full-bleed callout cards. The coral is warm, slightly muted, never cyan/blue.

The system has three surface modes that alternate page-by-page:
1. **Cream canvas** (`#faf9f5`) — default body floor
2. **Light cream cards** (`#efe9de`) — feature card backgrounds
3. **Dark navy product surfaces** (`#181715`) — code editor mockups, model showcase cards, pre-footer CTAs, footer itself

**Key Characteristics:**
- Warm cream canvas (`#faf9f5`) with dark warm-ink text (`#141413`). The brand's defining color choice.
- Coral primary CTA (`#cc785c`). Used scarcely on individual buttons, generously on full-bleed coral callout cards.
- Slab-serif display headlines via Copernicus / Tiempos Headline at weight 400 with negative letter-spacing. Pairs with humanist sans body for a literary editorial voice.
- Dark navy product mockup cards (`#181715`) carrying code blocks, terminal panels, model comparison data.
- Light cream feature cards (`#efe9de`) — slightly darker than canvas.
- Border radius is hierarchical: 8px for buttons + inputs, 12px for content + product cards, 16px for hero containers.
- Section rhythm 96px — modern-SaaS standard. Internal card padding stays generous at 32px.

## Colors

### Brand & Accent
- **Coral / Primary** (`#cc785c`): The signature warm coral. Used on every primary CTA background, brand wordmark accent.
- **Coral Active** (`#a9583e`): The press / hover-darker variant.
- **Coral Disabled** (`#e6dfd8`): A desaturated cream-tinted disabled state.
- **Accent Teal** (`#5db8a6`): Used sparingly on secondary product surfaces.
- **Accent Amber** (`#e8a55a`): Used on category badges and inline highlights.

### Surface
- **Canvas** (`#faf9f5`): The default page floor. Tinted cream — warm, deliberately not pure white.
- **Surface Soft** (`#f5f0e8`): Section dividers, very-soft band backgrounds.
- **Surface Card** (`#efe9de`): Feature cards, content cards. One step darker than canvas.
- **Surface Cream Strong** (`#e8e0d2`): Strongest-cream variant.
- **Surface Dark** (`#181715`): Code editor mockups, model showcase cards, footer.
- **Surface Dark Elevated** (`#252320`): Elevated cards inside dark bands.
- **Surface Dark Soft** (`#1f1e1b`): Slightly lighter dark for code block backgrounds.
- **Hairline** (`#e6dfd8`): 1px border tone on cream surfaces.
- **Hairline Soft** (`#ebe6df`): Barely-visible divider.

### Text
- **Ink** (`#141413`): All headlines and primary text. Warm dark, slightly off-pure-black.
- **Body Strong** (`#252523`): Emphasized paragraphs, lead text.
- **Body** (`#3d3d3a`): Default running-text color.
- **Muted** (`#6c6a64`): Sub-headings, breadcrumbs.
- **Muted Soft** (`#8e8b82`): Captions, fine-print.
- **On Primary** (`#ffffff`): Text on coral buttons.
- **On Dark** (`#faf9f5`): Cream-tinted white used on dark surfaces.
- **On Dark Soft** (`#a09d96`): Footer body text.
- **Success** (`#5db872`): Green status.
- **Warning** (`#d4a017`): Warning callouts.
- **Error** (`#c64545`): Validation errors.

## Typography

### Font Family
The system runs **Copernicus** (or **Tiempos Headline** as substitute) as the slab-serif display face for headlines, and **StyreneB** (or **Inter** as substitute) as the humanist sans for body, navigation, and UI labels. **JetBrains Mono** handles code blocks.

### Hierarchy

| Token | Size | Weight | Line Height | Letter Spacing | Use |
|---|---|---|---|---|---|
| display-xl | 64px | 400 | 1.05 | -1.5px | Hero h1 — Copernicus serif |
| display-lg | 48px | 400 | 1.1 | -1px | Section heads — Copernicus |
| display-md | 36px | 400 | 1.15 | -0.5px | Sub-section heads — Copernicus |
| display-sm | 28px | 400 | 1.2 | -0.3px | Callout headlines — Copernicus |
| title-lg | 22px | 500 | 1.3 | 0 | Pricing plan size — StyreneB |
| title-md | 18px | 500 | 1.4 | 0 | Feature card titles |
| title-sm | 16px | 500 | 1.4 | 0 | Connector tile titles |
| body-md | 16px | 400 | 1.55 | 0 | Default running-text — StyreneB |
| body-sm | 14px | 400 | 1.55 | 0 | Footer body, fine-print |
| caption | 13px | 500 | 1.4 | 0 | Badge labels, captions |
| caption-uppercase | 12px | 500 | 1.4 | 1.5px | Category tags, "NEW" badges |
| code | 14px | 400 | 1.6 | 0 | Code blocks — JetBrains Mono |
| button | 14px | 500 | 1.0 | 0 | Standard button labels |
| nav-link | 14px | 500 | 1.4 | 0 | Top-nav menu items |

### Principles
Display sizes use weight 400 (regular), never bold. Negative letter-spacing (-0.3 to -1.5px) is essential. Body type stays at weight 400 for paragraphs, weight 500 for labels.

### Font Substitutes
- **Copernicus →** Tiempos Headline, Cormorant Garamond (w500, -0.02em), EB Garamond
- **StyreneB →** Inter, Söhne

## Layout

### Spacing System
- **Base unit:** 4px
- **Tokens:** 4px · 8px · 12px · 16px · 24px · 32px · 48px · 96px (section)
- **Section padding:** 96px
- **Card internal padding:** 32px (feature cards), 24px (code-window cards)
- **Callout CTA bands:** 48px inside coral cards

### Grid & Container
- **Max content width:** ~1200px centered
- **Grid:** 12-column grid; hero often 6/6 split
- **Feature cards:** 3-up desktop, 2-up tablet, 1-up mobile
- **Pricing grid:** 3-up desktop, 1-up mobile

## Elevation & Depth

| Level | Treatment | Use |
|---|---|---|
| Flat | No shadow, no border | Body sections, hero bands |
| Soft hairline | 1px `#e6dfd8` border | Inputs, sub-nav, cards |
| Cream card | `#efe9de` background — no shadow | Feature cards |
| Dark surface | `#181715` background — no shadow | Code mockups, showcase cards |

## Shapes

### Border Radius Scale
| Token | Value | Use |
|---|---|---|
| xs | 4px | Badge accents, small dropdowns |
| sm | 6px | Small inline buttons |
| md | 8px | Standard CTAs, text inputs, tabs |
| lg | 12px | Content cards (feature, pricing, code-window) |
| xl | 16px | Hero illustration container |
| pill | 9999px | Badge pills, "NEW" tags |
| full | 9999px / 50% | Avatar substitutes, icon buttons |

## Components

### Top Navigation
**top-nav** — Cream nav bar, 64px tall, `#faf9f5` background. Logo left, menu center, CTA right.

### Buttons
- **button-primary** — Coral (`#cc785c`), white text, 14px/500, 12x20px padding, 40px height, 8px rounded.
- **button-secondary** — Cream with hairline outline, same size.
- **button-secondary-on-dark** — Over dark surfaces, `#252320` background.
- **button-text-link** — Inline text button, no background.
- **button-icon-circular** — 36px circular icon button.
- **text-link** — Inline body links in coral.

### Cards & Containers
- **hero-band** — Cream canvas hero with 6-6 grid. Padding 96px.
- **feature-card** — Cream card (`#efe9de`), 12px rounded, 32px padding. Icon + title + description.
- **product-mockup-card-dark** — Dark navy (`#181715`), 12px rounded, 32px padding.
- **code-window-card** — Dark card with code editor mockup, syntax-highlighted text.
- **pricing-tier-card** — Cream with hairline, 12px rounded, 32px padding.
- **pricing-tier-card-featured** — Dark surface featured tier.
- **callout-card-coral** — Full-bleed coral CTA card, 12px rounded, 48px padding.
- **connector-tile** — Cream tile with logo + name + description.
- **footer** — Dark navy, 4-column link list.

### Inputs & Forms
- **text-input** — Cream background, 8px rounded, 40px height, hairline border. Coral focus ring.

### Tags / Badges
- **badge-pill** — Small pill, `#efe9de` background, 4px × 12px padding.
- **badge-coral** — Coral-fill badge for "NEW", "BETA".

## Do's and Don'ts

### Do
- Anchor every page on the cream canvas
- Use Copernicus serif for every display headline
- Reserve coral for primary CTAs and callout cards
- Alternate cream-to-dark rhythm between bands
- Apply 96px section spacing

### Don't
- Don't use cool grays or pure white
- Don't bold serif display (stay at 400)
- Don't use cool blue or cyan accents
- Don't use Inter for display headlines
- Don't repeat same surface mode in two consecutive bands

## Responsive Behavior

### Breakpoints
- **Mobile:** < 768px — Hamburger nav, stacked hero, 1-up grids
- **Tablet:** 768–1024px — 2-up grids
- **Desktop:** 1024–1440px — Full layout, 3-up grids
- **Wide:** > 1440px — Max content 1200px

## Known Gaps
- Copernicus and StyreneB are licensed fonts; substitutes documented above.
- Animation timings not in scope.
- Form validation states beyond focused state not extracted.
