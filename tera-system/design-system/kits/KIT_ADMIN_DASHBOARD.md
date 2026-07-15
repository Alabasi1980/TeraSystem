# KIT_ADMIN_DASHBOARD.md

## Purpose

Default internal Tera design kit for ERP, CRM, admin panels, internal tools, and dashboards without an external design source.

It is a starting point, not a mandatory final identity.

## Visual Direction

```text
Clean administrative dashboard.
Calm, structured, readable, business-oriented.
Optimized for RTL Arabic and data-heavy screens.
```

## Design Tokens

```yaml
colors:
  background: "#F6F7F9"
  surface: "#FFFFFF"
  surface-muted: "#F1F3F5"
  border: "#E2E8F0"
  border-strong: "#CBD5E1"
  text: "#111827"
  text-muted: "#64748B"
  primary: "#1F4E79"
  primary-hover: "#173B5C"
  secondary: "#F59E0B"
  accent: "#0F766E"
  success: "#16A34A"
  warning: "#F59E0B"
  danger: "#DC2626"
  info: "#2563EB"

typography:
  font-family-ar: "Cairo, Tajawal, sans-serif"
  font-family-en: "Inter, system-ui, sans-serif"
  h1: "28px / 1.25 / 700"
  h2: "22px / 1.3 / 700"
  h3: "18px / 1.35 / 600"
  body: "14px / 1.6 / 400"
  small: "12px / 1.5 / 400"
  button: "14px / 1.2 / 600"

spacing:
  base-unit: 4px
  scale: [4, 8, 12, 16, 24, 32, 48, 64]

radius:
  sm: "4px"
  md: "8px"
  lg: "12px"
  xl: "16px"
  full: "999px"

shadow:
  none: "none"
  sm: "0 1px 2px rgba(15, 23, 42, 0.06)"
  md: "0 4px 12px rgba(15, 23, 42, 0.08)"
```

## Layout Rules

- Default pattern: Admin Shell.
- Sidebar: right side for RTL, 260px expanded, 72px collapsed.
- Topbar: height 64px, contains search/user/actions when needed.
- Content padding: 24px desktop, 16px tablet, 12px mobile.
- Cards use white surface, 1px border, 12px radius.
- Tables prioritize readability and compact row actions.

## Component Rules

- Primary button: primary background, white text, md radius.
- Secondary button: surface background, border, text color.
- Danger button: danger background only for destructive actions.
- Inputs: label above, 40px height, visible focus ring.
- Tables: sticky header when long, clear empty state, pagination by default.
- Alerts: use status colors with text labels, not color alone.

## RTL/LTR

- Arabic RTL default.
- Use logical CSS properties where possible.
- Numeric values may remain LTR inside RTL UI.

## Forbidden Styling

- No random gradients.
- No unapproved dark mode.
- No extra UI libraries without approval.
- No arbitrary Tailwind classes outside token system when a token exists.
- No mixing with unrelated brand styles.

## Best For

- ERP modules
- CRM dashboards
- admin panels
- internal CRUD apps
- procurement/inventory/accounting screens
