# RTL / LTR Rules

## Purpose

Ensure Arabic and multilingual UI behaves correctly.

## Direction Decision

Every UI project must record:

```text
Primary direction: RTL / LTR / Both
Primary language:
Secondary language:
Mirroring required: Yes / No
```

## RTL Rules

- Sidebar defaults to the right for Arabic unless user prefers otherwise.
- Icons with direction meaning must be mirrored.
- Numeric and code values may remain LTR inside RTL UI.
- Table alignment follows data type: text aligns start, numbers align end.
- Breadcrumbs and step indicators must respect direction.

## LTR Rules

- Sidebar defaults to left.
- Do not hardcode left/right in CSS when logical properties are possible.

## Engineering Rule

Prefer logical CSS properties where possible:

```text
margin-inline-start / margin-inline-end
padding-inline-start / padding-inline-end
inset-inline-start / inset-inline-end
```
