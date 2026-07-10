# Technology Profile — Effect TS in TeraOpenCode

## File
`.tera-workspace/RESEARCH/TECHNOLOGY_PROFILE_EFFECT.md`

## Date
2026-07-10

## Status
Draft v1 — required before Phase 3 core modifications

---

## 1. Purpose

This profile gives Tera agents a safe operating model for modifying the Effect TS parts of TeraOpenCode.

Phase 3 must not begin with direct edits to `packages/core/` until agents understand the core patterns below.

---

## 2. Why Effect TS Matters Here

TeraOpenCode uses Effect TS as a runtime architecture system, not just as a helper library.

The core engine depends on:

- `Effect` for typed async/effectful workflows.
- `Layer` for dependency wiring.
- `Context.Service` for service declarations.
- `Schema` for runtime validation and typed domain models.
- `Scope` for lifecycle-managed registration and cleanup.
- `Ref` for state managed inside Effect services.

This means normal TypeScript edits that ignore Effect layers can break runtime wiring even if the code typechecks locally.

---

## 3. Observed Core Patterns

### 3.1 Service Pattern

Common structure:

```ts
export interface Interface {
  readonly method: (...) => Effect.Effect<...>
}

export class Service extends Context.Service<Service, Interface>()("@opencode/v2/ServiceName") {}

const layer = Layer.effect(
  Service,
  Effect.gen(function* () {
    const dependency = yield* Dependency.Service
    return Service.of({ ... })
  }),
)

export const node = makeLocationNode({ service: Service, layer, deps: [Dependency.node] })
```

Rules:
- Add a service only when a real runtime capability is needed.
- Declare all dependencies in the `deps` array.
- Do not access services that are not declared as dependencies.
- Prefer existing service extension points before adding new services.

### 3.2 Layer Node Pattern

`packages/core/src/effect/app-node.ts` defines two layer categories:

- `makeGlobalNode`
- `makeLocationNode`

Most project/runtime capabilities are `LocationNode`s.

Rules:
- TeraSystem project-aware context should usually be `Location` scoped.
- Avoid process-global state unless it is truly shared across all workspaces.
- Keep dependency trees explicit.

### 3.3 System Context Pattern

Relevant files:

- `packages/core/src/system-context/index.ts`
- `packages/core/src/system-context/registry.ts`
- `packages/core/src/system-context/builtins.ts`

Concepts:
- A `SystemContext.Source<A>` loads typed context.
- `SystemContext.make(...)` turns a source into composable context.
- `SystemContextRegistry.Service.register(...)` registers location-scoped context.
- Built-ins currently register environment and date context.

Safe extension path for Phase 3:

```text
Add TeraSystem context as new SystemContext sources,
register them through the existing SystemContextRegistry,
and avoid modifying session internals first.
```

### 3.4 Tool Pattern

Relevant files:

- `packages/core/src/tool/tool.ts`
- `packages/core/src/tool/registry.ts`
- `packages/core/src/tool/builtins.ts`
- `packages/core/src/tool/AGENTS.md`

Core rule from local AGENTS.md:

> Do not add a second executable entry type, registry-owned executor, authorization callback, output-path callback, or legacy normalization path.

Safe extension path:

- Define tools with `Tool.make({ description, input, output, execute, toModelOutput })`.
- Register built-ins through `Tools.Service.register({ name: tool })`.
- Keep tool permission/side-effect logic inside the leaf tool implementation.
- Convert expected typed failures into `ToolFailure`.
- Do not use `catchCause` for normal tool errors.

---

## 4. Phase 3 Risk Zones

| Area | Risk | Rule |
|---|---|---|
| `packages/core/src/session/` | Very high | Do not modify in first Phase 3 pass |
| `packages/core/src/system-context/` | Medium | Safe first extension point |
| `packages/core/src/tool/` | Medium | Use existing canonical tool architecture |
| `packages/opencode/src/config/` | Medium | Add config only after context/tool shape is stable |
| `packages/core/src/effect/` | High | Do not modify layer machinery unless absolutely necessary |

---

## 5. Initial Phase 3 Strategy

Phase 3 should be split into small, reversible batches:

### Batch 3.1 — Read-only TeraSystem Context Source

Goal:
- Add a minimal read-only context source that exposes TeraSystem workspace metadata.

Preferred target:
- `packages/core/src/system-context/`

Avoid:
- Session V2 changes.
- Tool changes.
- Permission changes.

Success criteria:
- Context source registers through existing `SystemContextRegistry`.
- App still starts.
- No new global state.

### Batch 3.2 — Read-only TeraSystem Tool

Goal:
- Add one safe read-only tool, e.g. reading Tera project metadata.

Preferred target:
- `packages/core/src/tool/`

Rules:
- Use existing `Tool.make` pattern.
- No write operations.
- No task registry mutation.

### Batch 3.3 — Config Bridge

Goal:
- Add minimal config support for enabling/disabling Tera integration.

Preferred target:
- Config layer after the first context/tool proof works.

---

## 6. Do / Do Not

### Do

- Use existing `SystemContextRegistry` instead of creating a parallel context system.
- Use existing `Tool.make` instead of creating a second tool abstraction.
- Keep first integrations read-only.
- Add one capability per commit.
- Run `bun install` and a startup check after each batch.

### Do Not

- Do not touch Session V2 first.
- Do not add a new runtime container or dependency injection system.
- Do not add broad filesystem scanning before defining exact allowed paths.
- Do not mix branding cleanup with core integration.
- Do not rename internal `@opencode/...` Context.Service keys in Phase 3 unless a separate compatibility plan exists.

---

## 7. Recommended First Phase 3 Design Question

Before coding, answer:

```text
What exact TeraSystem facts should the model receive as system context?
```

Initial proposed facts:

- Current Tera workspace root.
- Current client ID, if detectable.
- Current application ID, if detectable.
- Relevant governance file paths only, not full file contents.
- A short statement that TeraSystem governance exists and must be respected.

Avoid injecting large policy files initially. Large context should be summarized or fetched by explicit tools later.

---

## 8. Validation Checklist for Phase 3 Changes

For every Phase 3 batch:

```text
[ ] Only one capability added
[ ] No Session V2 edits unless explicitly approved
[ ] Existing Effect service/layer pattern followed
[ ] Existing tool/context registry used
[ ] No broad write access introduced
[ ] bun install passes
[ ] Startup check passes
[ ] Commit created
[ ] .tera-workspace/TASKS updated
```

---

## 9. Current Recommendation

Start Phase 3 with a minimal read-only `TeraSystemContext` source.

Do not start with tools, config, or Session V2.

Reason:
- `system-context` already exists for exactly this purpose.
- It is lower risk than tools or session orchestration.
- It creates visible model value without changing core execution flow.
