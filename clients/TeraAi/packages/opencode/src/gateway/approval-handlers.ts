import type { GatewayProtocolResult, GatewaySession } from "./protocol"
import { workspaceStore } from "./workspace-registry"

type JsonRecord = Record<string, unknown>

const APPROVAL_REQUEST_MAX_BYTES = 16_384
const APPROVAL_RESPONSE_MAX_BYTES = 4_096

const VALID_ACTION_TYPES = ["destructive", "security_sensitive", "dependency_change"]
const VALID_RISK_LEVELS = ["low", "medium", "high", "critical"]

export function handleApproval(input: {
  readonly session: GatewaySession
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
}): GatewayProtocolResult {
  const method = requireString(input.payload.method)

  if (method === "approval.request") return handleApprovalRequest(input)
  if (method === "approval.response") return handleApprovalResponse(input)

  return {
    session: input.session,
    output: protocolError(input.id, "approval", "INVALID_REQUEST", `Unknown approval method: ${method ?? "undefined"}`, false),
    diagnostic: `unknown approval method: ${method}`,
  }
}

function handleApprovalRequest(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
  readonly session: GatewaySession
}): GatewayProtocolResult {
  if (input.messageSize > APPROVAL_REQUEST_MAX_BYTES) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.request", "MESSAGE_TOO_LARGE", "ApprovalRequest exceeds 16384 bytes", false),
      diagnostic: "oversized approval request rejected",
    }
  }

  if (!input.session.handshake) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.request", "APPROVAL_BEFORE_HANDSHAKE", "ApprovalRequest requires a successful handshake first", false),
      diagnostic: "approval before handshake rejected",
    }
  }

  const taskID = requireString(input.payload.task_id)
  if (!taskID) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.request", "INVALID_REQUEST", "ApprovalRequest requires a task_id", false),
      diagnostic: "approval request missing task_id",
    }
  }

  const actionType = requireString(input.payload.action_type)
  if (!actionType || !VALID_ACTION_TYPES.includes(actionType)) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.request", "INVALID_REQUEST", `Invalid action_type: ${actionType ?? "undefined"}. Must be one of: ${VALID_ACTION_TYPES.join(", ")}`, false),
      diagnostic: `invalid action_type: ${actionType}`,
    }
  }

  // Stub approval: deny critical risk levels, auto-approve everything else.
  const details = requireRecord(input.payload.details)
  const riskLevel = details ? requireString(details.risk_level) : undefined
  if (riskLevel && !VALID_RISK_LEVELS.includes(riskLevel)) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.request", "INVALID_REQUEST", `Invalid risk_level: ${riskLevel}. Must be one of: ${VALID_RISK_LEVELS.join(", ")}`, false),
      diagnostic: `invalid risk_level: ${riskLevel}`,
    }
  }

  const approved = riskLevel !== "critical"

  const workspaceRecord = workspaceStore.get(input.session.handshake!.workspaceID)
  if (workspaceRecord) {
    workspaceRecord.approvals.push({
      type: "request",
      task_id: taskID,
      action_type: actionType,
      description: requireString(input.payload.description),
      details: details ? {
        affected_files: isStringArray(details.affected_files) ? (details.affected_files as readonly string[]) : undefined,
        affected_commands: isStringArray(details.affected_commands) ? (details.affected_commands as readonly string[]) : undefined,
        risk_level: riskLevel,
      } : undefined,
      timestamp: new Date().toISOString(),
    })
  }

  return {
    session: input.session,
    output: response(input.id, {
      method: "approval.response",
      approved,
      reason: approved ? "Stub approval — auto-approved by engine gateway" : "Stub denial — critical risk level requires platform review",
      approved_by: "engine",
    }),
  }
}

function handleApprovalResponse(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
  readonly session: GatewaySession
}): GatewayProtocolResult {
  if (input.messageSize > APPROVAL_RESPONSE_MAX_BYTES) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.response", "MESSAGE_TOO_LARGE", "ApprovalResponse exceeds 4096 bytes", false),
      diagnostic: "oversized approval response rejected",
    }
  }

  if (!input.session.handshake) {
    return {
      session: input.session,
      output: protocolError(input.id, "approval.response", "APPROVAL_BEFORE_HANDSHAKE", "ApprovalResponse requires a successful handshake first", false),
      diagnostic: "approval response before handshake rejected",
    }
  }

  const workspaceRecord = workspaceStore.get(input.session.handshake!.workspaceID)
  if (workspaceRecord) {
    workspaceRecord.approvals.push({
      type: "response",
      approved: Boolean(input.payload.approved),
      reason: requireString(input.payload.reason),
      approved_by: requireString(input.payload.approved_by),
      timestamp: new Date().toISOString(),
    })
  }

  return {
    session: input.session,
    output: response(input.id, {
      method: "approval.response",
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

function requireString(value: unknown) {
  if (typeof value !== "string") return
  if (value.length === 0) return
  return value
}

function requireRecord(value: unknown): JsonRecord | undefined {
  if (typeof value !== "object" || value === null || Array.isArray(value)) return
  return value as JsonRecord
}

function isStringArray(value: unknown): boolean {
  return Array.isArray(value) && value.every((item) => typeof item === "string")
}
