# Technology Profile: nextjs-prisma

## 1. Profile Identity

- Profile ID: `nextjs-prisma`
- Language: TypeScript
- Framework: Next.js
- Database: PostgreSQL
- ORM: Prisma
- Package Manager / CLI: npm, npx
- Default Project Type: Web admin / internal business application

## 2. Applicability

Use this profile when the approved architecture is based on Next.js with TypeScript,
PostgreSQL, and Prisma.

## 3. Default Execution Order

1. Scaffold project.
2. Add ORM basics.
3. Define schema.
4. Apply database changes in a separate approved task.
5. Implement auth/permissions if approved.
6. Build backend workflows and UI in small batches.

## 4. First Task Rule

Safe first task:

```text
Scaffold Next.js + TypeScript + install Prisma + create a basic prisma/schema.prisma + create .env.example only
```

## 5. Scaffold Rules

- Prefer an explicit scaffold command with explicit flags.
- Prefer disabling unwanted defaults at scaffold time.
- Directory name must come from the project/task context, not be hardcoded in the system.

Example safe pattern:

```bash
npx create-next-app@latest <project-folder> --typescript --eslint --app --src-dir --no-tailwind --import-alias "@/*" --use-npm
```

## 6. ORM / Database Rules

- Do not use `npx prisma init` if the task forbids creating `.env`.
- Create `prisma/schema.prisma` manually when needed.
- In the first task, allow only:
  - `generator client`
  - `datasource db`
- Do not add Prisma models in the first task.
- Treat `prisma generate`, `db push`, and migrations as separate tasks unless the user explicitly approves combining them.
- Prisma schema may define field types and relations.
- Business validation rules such as `amount > 0` must not become database constraints unless explicitly approved.

## 7. CLI Side Effects

- `create-next-app` creates scaffold files and may add UI/CSS defaults.
- Prisma commands may create `.env`, schema files, generated client code, or database-side changes.
- Package installation changes `package.json` and lockfiles.

## 8. Forbidden Defaults

Do not add by default in the first implementation task:

- Prisma models
- `ConnectionTest` model
- `db push`
- migrations
- `prisma generate`
- real database connection test
- UI
- API
- Auth
- `.env` with real values

## 9. Pre-Execution Gate Additions

- Confirm scaffold flags avoid unapproved CSS/UI defaults.
- Confirm Prisma command does not create `.env` when `.env.example` only is allowed.
- Confirm schema file is basic only if the task is still the first technical setup task.
- Confirm database apply commands are split into their own approved task.

## 10. Acceptance Criteria Patterns

- Scaffold succeeds with the approved project structure only.
- No unapproved UI/CSS/theme files remain.
- No real secrets are written.
- No schema models or database apply commands run unless the task explicitly allows them.
