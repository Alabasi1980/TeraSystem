import { AgentV2 } from "@tera-system/core/agent"
import { AISDK } from "@tera-system/core/aisdk"
import { Catalog } from "@tera-system/core/catalog"
import { CommandV2 } from "@tera-system/core/command"
import { Credential } from "@tera-system/core/credential"
import { AppNodeBuilder } from "@tera-system/core/effect/app-node-builder"
import { LayerNodePlatform } from "@tera-system/core/effect/app-node-platform"
import { LayerNode } from "@tera-system/core/effect/layer-node"
import { EventV2 } from "@tera-system/core/event"
import { FileSystem } from "@tera-system/core/filesystem"
import { FSUtil } from "@tera-system/core/fs-util"
import { Integration } from "@tera-system/core/integration"
import { Location } from "@tera-system/core/location"
import { Npm } from "@tera-system/core/npm"
import { PluginV2 } from "@tera-system/core/plugin"
import { Reference } from "@tera-system/core/reference"
import { SkillV2 } from "@tera-system/core/skill"
import { Effect, Layer } from "effect"
import { tempLocationLayer } from "../fixture/location"

const npmLayer = Layer.succeed(
  Npm.Service,
  Npm.Service.of({
    add: () => Effect.succeed({ directory: "", entrypoint: undefined }),
    install: () => Effect.void,
    which: () => Effect.succeed(undefined),
  }),
)

export const PluginTestLayer = AppNodeBuilder.build(
  LayerNode.group([
    FileSystem.node,
    FSUtil.node,
    Location.node,
    Npm.node,
    Credential.node,
    EventV2.node,
    LayerNodePlatform.httpClient,
    PluginV2.node,
    AgentV2.node,
    AISDK.node,
    Catalog.node,
    CommandV2.node,
    Integration.node,
    Reference.node,
    SkillV2.node,
  ]),
  [
    [Location.node, tempLocationLayer],
    [Npm.node, npmLayer],
  ],
)
