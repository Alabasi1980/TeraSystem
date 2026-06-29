# Tera Client-Facing Content Policy

## 1. Purpose

This policy defines how Tera writes documents intended for external clients.

## 2. Default Language

Client-facing documents are written in Arabic by default.

Use another language only when Majed explicitly requests it.

## 3. Content Rules

Client-facing content must be:

- clear and readable by non-technical clients
- concise but complete enough for approval
- free of internal Tera implementation details
- free of sub-agent names, internal orchestration details, token policies, or runtime mechanics
- explicit about scope, assumptions, exclusions, and pending decisions
- careful not to promise uncertain timelines, costs, integrations, or capabilities

## 4. Separation Rule

Do not expose internal Tera files directly as client documents unless Tera has rewritten them into client-facing form.

Examples:

- `project-preparation/08_TECHNICAL_ARCHITECTURE.md` is internal.
- `clients/.../client-approval/02_CLIENT_PROPOSAL.md` is client-facing.

## 5. Approval Language

Every approval document must include a clear approval section, such as:

```text
حالة الاعتماد: معتمد / يحتاج تعديل / مرفوض / بانتظار قرار
ملاحظات العميل:
الشخص المعتمد:
تاريخ الاعتماد:
```

## 6. Suggestion Language

Suggestions must be marked clearly:

```text
اقتراح من تيرا - يحتاج موافقة العميل قبل اعتباره ضمن النطاق.
```

## 7. Final Rule

```text
Client-facing content must protect clarity, scope, and trust.
```
