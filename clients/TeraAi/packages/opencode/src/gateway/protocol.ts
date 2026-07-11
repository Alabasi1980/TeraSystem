import { handleTask } from "./task-handlers"

const CONTRACT_VERSION = "1.2"
const ENGINE_VERSION = "0.1.0"
const HANDSHAKE_REQUEST_MAX_BYTES = 4_096
const CONTEXT_REQUEST_MAX_BYTES = 1_048_576

type JsonRecord = Record<string, unknown>

export type GatewaySession = {
  readonly handshake?: {
    readonly workspaceID: string
    readonly projectID: string
  }
}

export type GatewayProtocolResult = {
  readonly session: GatewaySession
  readonly output?: JsonRecord
  readonly diagnostic?: string
  readonly close?: boolean
}

export const emptySession: GatewaySession = {}

export function handleGatewayLine(session: GatewaySession, line: string): GatewayProtocolResult {
  const messageSize = Buffer.byteLength(line, "utf8")
  const parsed = parseMessage(line)
  if (!parsed.ok) {
    return {
      session,
      output: protocolError("unknown", "unknown", "MALFORMED_JSON", "Gateway message is not valid JSON", true),
      diagnostic: "malformed JSON received",
      close: true,
    }
  }

  const message = parsed.value
  const id = requireString(message.id) ?? "unknown"
  const payload = requireRecord(message.payload)
  const method = payload ? requireString(payload.method) : undefined

  if (message.type !== "request" || !payload || !method) {
    return {
      session,
      output: protocolError(id, method ?? "unknown", "INVALID_REQUEST", "Gateway request envelope is invalid", false),
      diagnostic: "invalid request envelope received",
    }
  }

  if (method === "handshake") return handleHandshake({ id, payload, messageSize, session })
  if (method === "context") return handleContext({ id, payload, messageSize, session })
  if (method === "task") return handleTask({ id, payload, messageSize, session })

  return {
    session,
    output: protocolError(id, method, "UNSUPPORTED_METHOD", `Gateway method is not supported: ${method}`, false),
    diagnostic: `unsupported method received: ${method}`,
  }
}

function handleHandshake(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
  readonly session: GatewaySession
}): GatewayProtocolResult {
  if (input.messageSize > HANDSHAKE_REQUEST_MAX_BYTES) {
    return {
      session: input.session,
      output: protocolError(input.id, "handshake", "MESSAGE_TOO_LARGE", "HandshakeRequest exceeds 4096 bytes", false),
      diagnostic: "oversized handshake request rejected",
    }
  }

  if (String(input.payload.contract_version) !== CONTRACT_VERSION) {
    return {
      session: emptySession,
      output: response(input.id, {
        method: "handshake",
        status: "error",
        error: {
          code: "VERSION_MISMATCH",
          message: `Engine supports contract_version ${CONTRACT_VERSION}, got ${String(input.payload.contract_version)}`,
        },
      }),
      diagnostic: "incompatible handshake rejected",
      close: true,
    }
  }

  const workspaceID = requireString(input.payload.workspace_id)
  const projectID = requireString(input.payload.project_id)
  if (!workspaceID || !projectID) {
    return {
      session: input.session,
      output: protocolError(input.id, "handshake", "INVALID_REQUEST", "Handshake requires workspace_id and project_id", false),
      diagnostic: "handshake missing workspace_id or project_id",
    }
  }

  return {
    session: { handshake: { workspaceID, projectID } },
    output: response(input.id, {
      method: "handshake",
      status: "ok",
      engine_version: ENGINE_VERSION,
      contract_version: CONTRACT_VERSION,
      supported_methods: ["context", "task"],
    }),
  }
}

function handleContext(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
  readonly session: GatewaySession
}): GatewayProtocolResult {
  if (input.messageSize > CONTEXT_REQUEST_MAX_BYTES) {
    return {
      session: input.session,
      output: protocolError(input.id, "context", "MESSAGE_TOO_LARGE", "ContextRequest exceeds 1048576 bytes", false),
      diagnostic: "oversized context request rejected",
    }
  }

  if (!input.session.handshake) {
    return {
      session: input.session,
      output: protocolError(input.id, "context", "CONTEXT_BEFORE_HANDSHAKE", "ContextRequest requires a successful handshake first", false),
      diagnostic: "context before handshake rejected",
    }
  }

  if (
    requireString(input.payload.workspace_id) !== input.session.handshake.workspaceID ||
    requireString(input.payload.project_id) !== input.session.handshake.projectID
  ) {
    return {
      session: input.session,
      output: protocolError(input.id, "context", "INVALID_WORKSPACE", "ContextRequest workspace_id/project_id does not match handshake", false),
      diagnostic: "context workspace mismatch rejected",
    }
  }

  if (!isValidCapabilityEnvelope(input.payload.capabilities)) {
    return {
      session: input.session,
      output: protocolError(input.id, "context", "INVALID_CAPABILITY_ENVELOPE", "ContextRequest capability envelope is invalid", false),
      diagnostic: "invalid capability envelope rejected",
    }
  }

  return {
    session: input.session,
    output: response(input.id, {
      method: "context",
      status: "ok",
      acknowledged: true,
    }),
  }
}

function response(id: string, payload: JsonRecord) {
  return { type: "response", id, timestamp: new Date().toISOString(), payload }
}

function protocolError(id: string, method: string, errorCode: string, message: string, fatal: boolean) {
  return {
    type: "error",
    id,
    timestamp: new Date().toISOString(),
    payload: {
      method,
      error_type: "protocol_error",
      error_code: errorCode,
      message,
      fatal,
    },
  }
}

function parseMessage(line: string): { readonly ok: true; readonly value: JsonRecord } | { readonly ok: false } {
  try {
    const value: unknown = JSON.parse(line)
    if (!isRecord(value)) return { ok: false }
    return { ok: true, value }
  } catch {
    return { ok: false }
  }
}

function requireRecord(value: unknown) {
  if (!isRecord(value)) return
  return value
}

function requireString(value: unknown) {
  if (typeof value !== "string") return
  if (value.length === 0) return
  return value
}

function isRecord(value: unknown): value is JsonRecord {
  return typeof value === "object" && value !== null && !Array.isArray(value)
}

function isStringArray(value: unknown) {
  return Array.isArray(value) && value.every((item) => typeof item === "string")
}

function isValidCapabilityEnvelope(value: unknown) {
  const capabilities = requireRecord(value)
  const networkPolicy = requireRecord(capabilities?.network_policy)
  const approvalRequired = requireRecord(capabilities?.approval_required)
  return Boolean(
    capabilities &&
      isStringArray(capabilities.allowed_read_paths) &&
      isStringArray(capabilities.allowed_write_paths) &&
      isStringArray(capabilities.forbidden_paths) &&
      isStringArray(capabilities.allowed_commands) &&
      isStringArray(capabilities.forbidden_commands) &&
      networkPolicy &&
      typeof networkPolicy.internet_access === "boolean" &&
      isStringArray(networkPolicy.allowed_domains) &&
      approvalRequired &&
      typeof approvalRequired.destructive_actions === "boolean" &&
      typeof approvalRequired.security_sensitive === "boolean" &&
      typeof approvalRequired.major_dependency_changes === "boolean",
  )
}

export * as GatewayProtocol from "./protocol"
