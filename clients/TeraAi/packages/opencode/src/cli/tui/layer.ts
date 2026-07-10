import { run as runTui, type TuiInput } from "@tera-system/tui"
import { Global } from "@tera-system/core/global"
import { AppNodeBuilder } from "@tera-system/core/effect/app-node-builder"
import { Effect } from "effect"

export function run(input: TuiInput) {
  return runTui(input).pipe(Effect.provide(AppNodeBuilder.build(Global.node)))
}
