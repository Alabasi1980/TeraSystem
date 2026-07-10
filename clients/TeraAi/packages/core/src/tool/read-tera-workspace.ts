export * as ReadTeraWorkspaceTool from "./read-tera-workspace"

import { Effect, Layer, Schema } from "effect"
import { ToolFailure } from "@tera-system/llm"
import { makeLocationNode } from "../effect/app-node"
import { FileSystem } from "../filesystem"
import { PermissionV2 } from "../permission"
import { Location } from "../location"
import { RelativePath } from "../schema"
import { ToolRegistry } from "./registry"
import { Tool } from "./tool"
import { Tools } from "./tools"

export const name = "read_tera_workspace"

const MAX_BYTES = 512_000

export const Input = Schema.Struct({
  path: Schema.String,
})

export const Output = Schema.Struct({
  path: Schema.String,
  content: Schema.String,
  truncated: Schema.Boolean,
})

function validatePath(path: string): Effect.Effect<void, ToolFailure> {
  if (path.includes(".."))
    return Effect.fail(new ToolFailure({ message: "Path must not contain '..'" }))
  if (!path.startsWith(".tera-workspace/"))
    return Effect.fail(new ToolFailure({ message: "Path must start with .tera-workspace/" }))
  return Effect.void
}

const layer = Layer.effectDiscard(
  Effect.gen(function* () {
    const tools = yield* Tools.Service
    const fs = yield* FileSystem.Service
    const permission = yield* PermissionV2.Service
    const location = yield* Location.Service

    yield* tools
      .register({
        [name]: Tool.make({
          description:
            "Read files from the .tera-workspace/ governance directory. " +
            "Use this to read TeraSystem task registry, decisions log, plans, " +
            "and any governance documents. Only .tera-workspace/ files are accessible.",
          input: Input,
          output: Output,
          execute: (input, context) =>
            Effect.gen(function* () {
              yield* validatePath(input.path)

              yield* permission.assert({
                action: name,
                resources: [input.path],
                save: ["*"],
                metadata: {
                  root: location.directory,
                  path: input.path,
                },
                sessionID: context.sessionID,
                agent: context.agent,
                source: {
                  type: "tool" as const,
                  messageID: context.assistantMessageID,
                  callID: context.toolCallID,
                },
              })

              const result = yield* fs.read({ path: RelativePath.make(input.path) }).pipe(
                Effect.mapError(
                  () => new ToolFailure({ message: `Unable to read ${input.path}` }),
                ),
              )

              const decoder = new TextDecoder("utf-8", { fatal: false })
              const text = decoder.decode(result.content)
              const truncated = result.content.byteLength > MAX_BYTES
              const display = truncated
                ? text.slice(0, MAX_BYTES) + "\n\n... [truncated at 500KB]"
                : text

              return {
                path: input.path,
                content: display,
                truncated,
              }
            }).pipe(
              Effect.catchTag("PermissionV2.BlockedError", () =>
                Effect.fail(new ToolFailure({ message: `Permission denied: cannot read ${input.path}` })),
              ),
              Effect.catchTag("PermissionV2.CorrectedError", (error) =>
                Effect.fail(new ToolFailure({ message: error.feedback })),
              ),
              Effect.catchTag("Session.NotFoundError", () =>
                Effect.fail(new ToolFailure({ message: "Session not found" })),
              ),
            ),
        }),
      })
      .pipe(Effect.orDie)
  }),
)

export const node = makeLocationNode({
  name: "tool/read-tera-workspace",
  layer,
  deps: [ToolRegistry.node, FileSystem.node, PermissionV2.node, Location.node],
})
