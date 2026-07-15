export * as PublicEventManifest from "./public-event-manifest"

import { Event } from "@tera-system/schema/event"
import { EventManifest } from "@tera-system/schema/event-manifest"

export const Definitions = EventManifest.ServerDefinitions
export const Latest = Event.latest(Definitions)
