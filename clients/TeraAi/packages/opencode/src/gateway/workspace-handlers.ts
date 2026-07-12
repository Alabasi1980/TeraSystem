import type { GatewayProtocolResult, GatewaySession } from "./protocol"
import { workspaceStore } from "./workspace-registry"

type JsonRecord = Record<string, unknown>

export function handleWorkspace(input: {
  readonly session: GatewaySession
  readonly id: string
  readonly payload: JsonRecord
  readonly messageSize: number
}): GatewayProtocolResult {
  if (!input.session.handshake) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace", "WORKSPACE_BEFORE_HANDSHAKE", "Workspace request requires a successful handshake first", false),
      diagnostic: "workspace before handshake rejected",
    }
  }

  const action = requireString(input.payload.action)
  if (!action) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace", "INVALID_REQUEST", "Workspace request requires an action field", false),
      diagnostic: "workspace missing action field",
    }
  }

  if (action === "list") return handleWorkspaceList(input)
  if (action === "status") return handleWorkspaceStatus(input)
  if (action === "archive") return handleWorkspaceArchive(input)
  if (action === "delete") return handleWorkspaceDelete(input)
  if (action === "close") return handleWorkspaceClose(input)

  return {
    session: input.session,
    output: protocolError(input.id, "workspace", "INVALID_ACTION", `Unknown workspace action: ${action}`, false),
    diagnostic: `unknown workspace action: ${action}`,
  }
}

function handleWorkspaceList(input: {
  readonly id: string
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const workspaces = workspaceStore.getAll().map((w) => ({
    id: w.id,
    projectId: w.projectId,
    status: w.status,
  }))

  return {
    session: input.session,
    output: response(input.id, {
      method: "workspace.list",
      workspaces,
    }),
  }
}

function handleWorkspaceStatus(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const workspaceID = requireString(input.payload.workspace_id)
  if (!workspaceID) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.status", "INVALID_REQUEST", "Workspace status requires a workspace_id", false),
      diagnostic: "workspace status missing workspace_id",
    }
  }

  const record = workspaceStore.get(workspaceID)
  if (!record) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.status", "WORKSPACE_NOT_FOUND", `Workspace not found: ${workspaceID}`, false),
      diagnostic: `workspace status for unknown workspace: ${workspaceID}`,
    }
  }

  return {
    session: input.session,
    output: response(input.id, {
      method: "workspace.status",
      workspace: {
        id: record.id,
        projectId: record.projectId,
        directory: record.directory,
        createdAt: record.createdAt,
        lastActiveAt: record.lastActiveAt,
        status: record.status,
      },
    }),
  }
}

function handleWorkspaceArchive(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const workspaceID = requireString(input.payload.workspace_id)
  if (!workspaceID) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.archive", "INVALID_REQUEST", "Workspace archive requires a workspace_id", false),
      diagnostic: "workspace archive missing workspace_id",
    }
  }

  const record = workspaceStore.archive(workspaceID)
  if (!record) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.archive", "WORKSPACE_NOT_FOUND", `Workspace not found: ${workspaceID}`, false),
      diagnostic: `workspace archive for unknown workspace: ${workspaceID}`,
    }
  }

  return {
    session: input.session,
    output: response(input.id, {
      method: "workspace.archive",
      status: "archived",
    }),
  }
}

function handleWorkspaceDelete(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const workspaceID = requireString(input.payload.workspace_id)
  if (!workspaceID) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.delete", "INVALID_REQUEST", "Workspace delete requires a workspace_id", false),
      diagnostic: "workspace delete missing workspace_id",
    }
  }

  const record = workspaceStore.get(workspaceID)
  if (!record) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.delete", "WORKSPACE_NOT_FOUND", `Workspace not found: ${workspaceID}`, false),
      diagnostic: `workspace delete for unknown workspace: ${workspaceID}`,
    }
  }

  const cleaned = {
    tasks: record.tasks.size,
    approvals: record.approvals.length,
    sessions: record.sessions.length,
  }

  workspaceStore.remove(workspaceID)

  return {
    session: input.session,
    output: response(input.id, {
      method: "workspace.delete",
      status: "deleted",
      cleaned,
    }),
  }
}

function handleWorkspaceClose(input: {
  readonly id: string
  readonly payload: JsonRecord
  readonly session: GatewaySession
}): GatewayProtocolResult {
  const workspaceID = requireString(input.payload.workspace_id)
  if (!workspaceID) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.close", "INVALID_REQUEST", "Workspace close requires a workspace_id", false),
      diagnostic: "workspace close missing workspace_id",
    }
  }

  const record = workspaceStore.get(workspaceID)
  if (!record) {
    return {
      session: input.session,
      output: protocolError(input.id, "workspace.close", "WORKSPACE_NOT_FOUND", `Workspace not found: ${workspaceID}`, false),
      diagnostic: `workspace close for unknown workspace: ${workspaceID}`,
    }
  }

  const cleaned = {
    tasks: record.tasks.size,
    approvals: record.approvals.length,
    sessions: record.sessions.length,
  }

  workspaceStore.remove(workspaceID)

  return {
    session: input.session,
    output: response(input.id, {
      method: "workspace.close",
      status: "closed",
      cleaned,
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

export * as WorkspaceHandlers from "./workspace-handlers"
