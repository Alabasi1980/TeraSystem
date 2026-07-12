import { describe, expect, test } from "bun:test"
import { GatewayProtocol } from "@/gateway/protocol"

describe("gateway task api", () => {
  test("task.create returns status created", async () => {
    const result = await runGateway([handshake(), task({ action: "create", task_id: "t_001" })])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].id).toBe("task_001")
    expect(result.stdout[1].payload).toMatchObject({
      method: "task",
      action: "create",
      status: "created",
      task_id: "t_001",
    })
  })

  test("task.cancel returns status cancelled", async () => {
    const result = await runGateway([handshake(), task({ action: "cancel", task_id: "t_002" })])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].payload).toMatchObject({
      method: "task",
      action: "cancel",
      status: "cancelled",
      task_id: "t_002",
    })
  })

  test("task.status returns correct status after create", async () => {
    const result = await runGateway([
      handshake(),
      task({ action: "create", task_id: "t_003" }, "task_create"),
      task({ action: "status", task_id: "t_003" }, "task_status"),
    ])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(3)
    expect(result.stdout[2].type).toBe("response")
    expect(result.stdout[2].payload).toMatchObject({
      method: "task",
      action: "status",
      status: "created",
      task_id: "t_003",
    })
  })

  test("task.status returns unknown for nonexistent task", async () => {
    const result = await runGateway([handshake(), task({ action: "status", task_id: "t_nope" })])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].payload).toMatchObject({
      method: "task",
      action: "status",
      status: "unknown",
      task_id: "t_nope",
    })
  })

  test("rejects task before successful handshake", async () => {
    const result = await runGateway([task({ action: "create", task_id: "t_004" })])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(result.stdout[0].id).toBe("task_001")
    expect(result.stdout[0].payload).toMatchObject({
      method: "task",
      error_code: "TASK_BEFORE_HANDSHAKE",
    })
  })

  test("rejects task with missing action", async () => {
    const result = await runGateway([handshake(), task({ task_id: "t_005" }, "task_no_action", undefined)])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "task",
      error_code: "INVALID_REQUEST",
    })
  })

  test("gateway announces supported_methods includes task", async () => {
    const result = await runGateway([handshake()])
    expect(result.stdout[0].payload).toMatchObject({
      method: "handshake",
      status: "ok",
      supported_methods: ["context", "task", "approval", "workspace"],
    })
  })

  test("rejects oversized task request with MESSAGE_TOO_LARGE", async () => {
    const result = await runGateway([handshake(), task({ action: "create", task_id: "t_big", payload: "x".repeat(70_000) })])
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "task",
      error_code: "MESSAGE_TOO_LARGE",
    })
    expect(result.stderr).toContain("oversized task request rejected")
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
