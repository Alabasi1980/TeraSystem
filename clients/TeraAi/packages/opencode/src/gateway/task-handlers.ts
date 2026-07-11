import type { GatewayProtocolResult, GatewaySession } from "./protocol"

type JsonRecord = Record<string, unknown>

const TASK_REQUEST_MAX_BYTES = 65_536

// Ephemeral in-memory task state — no persistence, cleared on process exit.
const taskStore = new Map<string, string>()

export function handleTask(input: {
  readonly session: GatewaySession
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
}): GatewayProtocolResult {
  if (input.messageSize > TASK_REQUEST_MAX_BYTES) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "MESSAGE_TOO_LARGE", "Task request exceeds 65536 bytes", false),
      diagnostic: "oversized task request rejected",
    }
  }

  if (!input.session.handshake) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "TASK_BEFORE_HANDSHAKE", "Task request requires a successful handshake first", false),
      diagnostic: "task before handshake rejected",
    }
  }

  const action = requireString(input.payload.action)
  if (!action) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "INVALID_REQUEST", "Task request requires an action field", false),
      diagnostic: "task missing action field",
    }
  }

  if (action === "create") return handleTaskCreate(input)
  if (action === "cancel") return handleTaskCancel(input)
  if (action === "status") return handleTaskStatus(input)

  return {
    session: input.session,
    output: protocolError(input.id, "task", "INVALID_ACTION", `Unknown task action: ${action}`, false),
    diagnostic: `unknown task action: ${action}`,
  }
}

function handleTaskCreate(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const taskID = requireString(input.payload.task_id)
  if (!taskID) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "INVALID_REQUEST", "Task create requires a task_id", false),
      diagnostic: "task create missing task_id",
    }
  }

  taskStore.set(taskID, "created")

  return {
    session: input.session,
    output: response(input.id, {
      method: "task",
      action: "create",
      status: "created",
      task_id: taskID,
    }),
  }
}

function handleTaskCancel(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const taskID = requireString(input.payload.task_id)
  if (!taskID) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "INVALID_REQUEST", "Task cancel requires a task_id", false),
      diagnostic: "task cancel missing task_id",
    }
  }

  taskStore.set(taskID, "cancelled")

  return {
    session: input.session,
    output: response(input.id, {
      method: "task",
      action: "cancel",
      status: "cancelled",
      task_id: taskID,
    }),
  }
}

function handleTaskStatus(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const taskID = requireString(input.payload.task_id)
  if (!taskID) {
    return {
      session: input.session,
      output: protocolError(input.id, "task", "INVALID_REQUEST", "Task status requires a task_id", false),
      diagnostic: "task status missing task_id",
    }
  }

  const status = taskStore.get(taskID) ?? "unknown"

  return {
    session: input.session,
    output: response(input.id, {
      method: "task",
      action: "status",
      status,
      task_id: taskID,
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
