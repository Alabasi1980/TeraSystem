# Design Tokens Schema

## Purpose

Define the required structure for executable design tokens in `28_UI_UX_GUIDELINES.md`.

## Token Groups

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
  line-height:

spacing:
  base-unit: 4px
  scale: [4, 8, 12, 16, 24, 32, 48, 64]

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

## Rules

- Tokens must be explicit before UI implementation.
- If a token is unknown, record it as an Open Design Gap.
- EngineeringAgent must not invent missing tokens.
- Client branding overrides must be recorded separately from base kit tokens.
