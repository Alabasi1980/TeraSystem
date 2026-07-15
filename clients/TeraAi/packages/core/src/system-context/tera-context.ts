export * as TeraSystemContext from "./tera-context"

import { Effect, Schema } from "effect"
import { SystemContext } from "./index"

export const tera = SystemContext.make({
  key: SystemContext.Key.make("tera/system"),
  codec: Schema.toCodecJson(Schema.String),
  load: Effect.succeed([
    "<tera-system>",
    "  TeraSystem governance is active for this workspace.",
    "  Governance files:",
    "    - tera-system/ (policies, architecture, checklists)",
    "    - project-control/ (logs, gaps, proposals)",
    "    - .opencode/agents/ (agent definitions)",
    "</tera-system>",
  ].join("\n")),
  baseline: (text) => text,
  update: (_previous, text) => text,
})
