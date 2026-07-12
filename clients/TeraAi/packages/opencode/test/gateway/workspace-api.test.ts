import { describe, expect, test } from "bun:test"
import { GatewayProtocol } from "@/gateway/protocol"

describe("gateway workspace api", () => {
  test("workspace.list returns workspace created by handshake", async () => {
    const result = await runGateway([handshake(), workspace({ action: "list" })])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace.list",
      workspaces: [
        { id: "ws_abc123", projectId: "proj_xyz789", status: "active" },
      ],
    })
  })

  test("workspace.status returns metadata for known workspace", async () => {
    const result = await runGateway([handshake(), workspace({ action: "status", workspace_id: "ws_abc123" })])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace.status",
      workspace: {
        id: "ws_abc123",
        projectId: "proj_xyz789",
        status: "active",
      },
    })
    const ws = (result.stdout[1].payload as Record<string, unknown>).workspace as Record<string, unknown>
    expect(ws).toHaveProperty("createdAt")
    expect(ws).toHaveProperty("lastActiveAt")
    expect(ws).toHaveProperty("directory")
  })

  test("workspace.status returns error for unknown workspace", async () => {
    const result = await runGateway([handshake(), workspace({ action: "status", workspace_id: "ws_unknown" })])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace.status",
      error_code: "WORKSPACE_NOT_FOUND",
    })
    expect(result.stderr).toContain("workspace status for unknown workspace: ws_unknown")
  })

  test("rejects workspace before successful handshake", async () => {
    const result = await runGateway([workspace({ action: "list" })])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(result.stdout[0].id).toBe("ws_001")
    expect(result.stdout[0].payload).toMatchObject({
      method: "workspace",
      error_code: "WORKSPACE_BEFORE_HANDSHAKE",
    })
  })

  test("rejects workspace with missing action", async () => {
    const result = await runGateway([handshake(), workspace({}, "ws_no_action", undefined)])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace",
      error_code: "INVALID_REQUEST",
    })
  })

  test("rejects workspace with unknown action", async () => {
    const result = await runGateway([handshake(), workspace({ action: "unknown_action" })])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace",
      error_code: "INVALID_ACTION",
    })
    expect(result.stderr).toContain("unknown workspace action: unknown_action")
  })

  test("gateway announces supported_methods includes workspace", async () => {
    const result = await runGateway([handshake()])
    expect(result.stdout[0].payload).toMatchObject({
      method: "handshake",
      status: "ok",
      supported_methods: ["context", "task", "approval", "workspace"],
    })
  })

  test("workspace.close returns cleanup summary with tasks and sessions", async () => {
    const result = await runGateway([
      handshake(),
      task({ action: "create", task_id: "t_close_001" }, "task_close_001"),
      task({ action: "create", task_id: "t_close_002" }, "task_close_002"),
      workspace({ action: "close", workspace_id: "ws_abc123" }, "ws_close"),
    ])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(4)
    expect(result.stdout[3].type).toBe("response")
    expect(result.stdout[3].payload).toMatchObject({
      method: "workspace.close",
      status: "closed",
      cleaned: { tasks: 2, approvals: 0, sessions: 1 },
    })
  })

  test("workspace.close removes workspace from registry", async () => {
    const result = await runGateway([
      handshake(),
      workspace({ action: "close", workspace_id: "ws_abc123" }, "ws_close"),
      workspace({ action: "status", workspace_id: "ws_abc123" }, "ws_status"),
    ])
    expect(result.stdout).toHaveLength(3)
    expect(result.stdout[2].type).toBe("error")
    expect(result.stdout[2].payload).toMatchObject({
      method: "workspace.status",
      error_code: "WORKSPACE_NOT_FOUND",
    })
  })

  test("workspace.close returns error for unknown workspace", async () => {
    const result = await runGateway([
      handshake(),
      workspace({ action: "close", workspace_id: "ws_nonexistent" }, "ws_close"),
    ])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "workspace.close",
      error_code: "WORKSPACE_NOT_FOUND",
    })
  })

  test("workspace A tasks isolated from workspace B", async () => {
    // Create workspace A with a task
    const resultA = await runGateway([
      handshake({ workspace_id: "ws_iso_a", project_id: "proj_iso_a" }),
      task({ action: "create", task_id: "t_iso_001" }, "task_create_iso"),
      task({ action: "status", task_id: "t_iso_001" }, "task_status_iso"),
    ])
    expect(resultA.stdout[2].payload).toMatchObject({
      method: "task",
      action: "status",
      status: "created",
      task_id: "t_iso_001",
    })

    // Create workspace B — should NOT see workspace A's task
    const resultB = await runGateway([
      handshake({ workspace_id: "ws_iso_b", project_id: "proj_iso_b" }),
      task({ action: "status", task_id: "t_iso_001" }, "task_status_iso_b"),
    ])
    expect(resultB.stdout[1].payload).toMatchObject({
      method: "task",
      action: "status",
      status: "unknown",
      task_id: "t_iso_001",
    })
  })

  test("workspace A approvals isolated from workspace B", async () => {
    // Create workspace A with an approval — close shows 1 approval
    const resultA = await runGateway([
      handshake({ workspace_id: "ws_iso_app_a", project_id: "proj_iso_app_a" }),
      approvalRequest({
        task_id: "t_iso_app_001",
        action_type: "destructive",
        description: "Delete file in A",
        details: { affected_files: [], affected_commands: [], risk_level: "low" },
      }, "appr_iso_a"),
      workspace({ action: "close", workspace_id: "ws_iso_app_a" }, "ws_close_app_a"),
    ])
    expect(resultA.stdout[2].payload).toMatchObject({
      method: "workspace.close",
      cleaned: { tasks: 0, approvals: 1, sessions: 1 },
    })

    // Create workspace B with no approvals — close shows 0
    const resultB = await runGateway([
      handshake({ workspace_id: "ws_iso_app_b", project_id: "proj_iso_app_b" }),
      workspace({ action: "close", workspace_id: "ws_iso_app_b" }, "ws_close_app_b"),
    ])
    expect(resultB.stdout[1].payload).toMatchObject({
      method: "workspace.close",
      cleaned: { tasks: 0, approvals: 0, sessions: 1 },
    })
  })
})

function runGateway(messages: object[]) {
  const stdout: Record<string, unknown>[] = []
  const stderr: string[] = []
  let session = GatewayProtocol.emptySession
  for (const message of messages) {
    const result = GatewayProtocol.handleGatewayLine(session, JSON.stringify(message))
    if (result.output) stdout.push(JSON.parse(JSON.stringify(result.output)) as Record<string, unknown>)
    if (result.diagnostic) stderr.push(result.diagnostic)
    if (result.close) break
    session = result.session
  }
  return {
    stdout,
    stderr: stderr.join("\n"),
  }
}

function handshake(payload?: Record<string, unknown>) {
  return {
    type: "request",
    id: "handshake_001",
    timestamp: "2026-07-10T14:30:00.000Z",
    payload: {
      method: "handshake",
      contract_version: "1.2",
      platform_version: "0.1.0",
      engine_version: "0.1.0",
      workspace_id: "ws_abc123",
      project_id: "proj_xyz789",
      ...payload,
    },
  }
}

function workspace(payload?: Record<string, unknown>, id?: string, action?: string) {
  return {
    type: "request",
    id: id ?? "ws_001",
    timestamp: "2026-07-10T14:30:02.000Z",
    payload: {
      method: "workspace",
      action: action ?? payload?.action,
      ...payload,
    },
  }
}

function task(payload?: Record<string, unknown>, id?: string, action?: string) {
  return {
    type: "request",
    id: id ?? "task_001",
    timestamp: "2026-07-10T14:30:02.000Z",
    payload: {
      method: "task",
      action: action ?? payload?.action,
      ...payload,
    },
  }
}

function approvalRequest(payload?: Record<string, unknown>, id?: string) {
  return {
    type: "request",
    id: id ?? "appr_001",
    timestamp: "2026-07-10T14:32:00.000Z",
    payload: {
      method: "approval.request",
      ...payload,
    },
  }
}
