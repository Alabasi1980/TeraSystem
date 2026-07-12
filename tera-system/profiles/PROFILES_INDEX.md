# Technology Profiles Index

## Purpose

Technology Profiles keep Tera Agent technically neutral at the system level.

Tera must stay a generic project orchestrator. Technology-specific execution
rules must come from the active profile, not from the core Tera system files.

## Selection Order

When Tera needs implementation rules, it determines the active profile in this order:

1. Read `project-control/PROJECT_STATE.md`.
2. If `Active Technology Profile` is defined there, use it.
3. Otherwise review `project-inputs/02_TECHNICAL_CONTEXT.md`.
4. Then derive or confirm it through `project-preparation/08_TECHNICAL_ARCHITECTURE.md`.
5. If no matching profile exists, ask the user to confirm the stack or create a
   draft profile from `tera-system/profiles/TEMPLATE.md` before execution.

## Rules

- Do not use rules from an inactive profile.
- Do not mix rules from unrelated stacks.
- Do not apply `nextjs-prisma` rules to `.NET Blazor + EF Core`.
- Do not apply `dotnet-blazor-ef` rules to a Prisma-based project.

## Relationship with Technical Context

Technology Profile must not be selected from guesswork alone.

Profile selection should be grounded in:

1. `project-inputs/02_TECHNICAL_CONTEXT.md`
2. `project-preparation/08_TECHNICAL_ARCHITECTURE.md`
3. `project-control/PROJECT_STATE.md` when a live project uses it

If the stack is still unclear, do not activate a final profile yet.

## Current Profiles

| Profile ID | Stack | Status | Notes |
|---|---|---|---|
| `nextjs-prisma` | Next.js + TypeScript + PostgreSQL + Prisma | Available | Use when the approved architecture matches this stack |
| `dotnet-blazor-ef` | .NET Blazor + EF Core | Available | Ready for future .NET-oriented projects |

## Placeholder Candidates

The following stacks may need profiles later, but are not created now:

- Django + Django ORM
- Laravel + Eloquent
- Spring Boot + Hibernate
