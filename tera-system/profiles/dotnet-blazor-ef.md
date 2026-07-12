# Technology Profile: dotnet-blazor-ef

## 1. Profile Identity

- Profile ID: `dotnet-blazor-ef`
- Language: C#
- Framework: .NET Blazor
- Database: SQL Server or approved project database
- ORM: Entity Framework Core
- Package Manager / CLI: dotnet CLI, NuGet
- Default Project Type: Internal business application

## 2. Applicability

Use this profile when the approved architecture is based on .NET Blazor with EF Core.

## 3. Default Execution Order

1. Scaffold the Blazor project and solution structure.
2. Add only the required base packages.
3. Define entities and DbContext in separate approved tasks.
4. Create migrations in a separate approved task.
5. Apply database changes only in a dedicated database task.

## 4. First Task Rule

Safe first task:

```text
Scaffold .NET Blazor project + basic solution structure only
```

## 5. Scaffold Rules

- Use `dotnet new blazor...` only for project scaffolding.
- Keep the first task limited to project structure and minimum approved packages.

## 6. ORM / Database Rules

- Entities belong to a separate schema/domain task.
- `DbContext` belongs to a clear database setup task.
- Migrations belong to a separate migration task.
- `dotnet ef database update` is forbidden unless the task is explicitly a database apply task.
- Do not add Identity/Auth without explicit delegation.

## 7. CLI Side Effects

- `dotnet new blazor...` creates boilerplate files.
- `dotnet add package` modifies `.csproj`.
- `dotnet ef migrations add` creates migration files.
- `dotnet ef database update` changes database state.

## 8. Forbidden Defaults

Do not add by default in the first implementation task:

- Entities
- `DbContext`
- migrations
- database update
- Auth
- custom UI styling
- large services
- repository layer without clear need

## 9. Pre-Execution Gate Additions

- Confirm scaffold task does not silently add Identity or extra hosting layers.
- Confirm EF Core work is split from scaffold work unless the task explicitly combines them with approval.
- Confirm database update commands are isolated.

## 10. Acceptance Criteria Patterns

- Basic Blazor project structure exists and builds.
- No unapproved database changes were applied.
- No unapproved auth or heavy infrastructure was added.
