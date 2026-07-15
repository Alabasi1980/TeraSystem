import { describe, expect, test } from "bun:test"
import { GatewayProtocol } from "@/gateway/protocol"

describe("gateway approval api", () => {
  test("approval.request returns approved: true for non-critical risk", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest({
        task_id: "t_001",
        action_type: "destructive",
        description: "Delete old test files",
        details: {
          affected_files: ["tests/old-test.tsx"],
          affected_commands: ["rm tests/old-test.tsx"],
          risk_level: "medium",
        },
      }),
    ])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].id).toBe("appr_001")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.response",
      approved: true,
      approved_by: "engine",
    })
  })

  test("approval.request returns approved: false for critical risk", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest(
        {
          task_id: "t_002",
          action_type: "security_sensitive",
          description: "Modify auth configuration",
          details: {
            affected_files: ["src/auth/config.ts"],
            affected_commands: [],
            risk_level: "critical",
          },
        },
        "appr_002",
      ),
    ])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].id).toBe("appr_002")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.response",
      approved: false,
      approved_by: "engine",
    })
  })

  test("rejects approval.request before successful handshake", async () => {
    const result = await runGateway([
      approvalRequest({
        task_id: "t_003",
        action_type: "destructive",
        description: "Delete file",
        details: { affected_files: [], affected_commands: [], risk_level: "low" },
      }),
    ])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(result.stdout[0].id).toBe("appr_001")
    expect(result.stdout[0].payload).toMatchObject({
      method: "approval.request",
      error_code: "APPROVAL_BEFORE_HANDSHAKE",
    })
  })

  test("rejects approval.request with missing task_id", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest({ action_type: "destructive", description: "No task" }, "appr_no_task"),
    ])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.request",
      error_code: "INVALID_REQUEST",
    })
  })

  test("rejects approval.request with invalid action_type", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest({ task_id: "t_004", action_type: "invalid_type", description: "Bad type" }, "appr_bad_type"),
    ])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.request",
      error_code: "INVALID_REQUEST",
    })
  })

  test("rejects approval.request with invalid risk_level", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest(
        {
          task_id: "t_005",
          action_type: "destructive",
          description: "Bad risk",
          details: { affected_files: [], affected_commands: [], risk_level: "extreme" },
        },
        "appr_bad_risk",
      ),
    ])
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.request",
      error_code: "INVALID_REQUEST",
    })
  })

  test("gateway announces supported_methods includes approval", async () => {
    const result = await runGateway([handshake()])
    expect(result.stdout[0].payload).toMatchObject({
      method: "handshake",
      status: "ok",
      supported_methods: ["context", "task", "approval", "workspace"],
    })
  })

  test("stdout emits protocol JSON Lines only", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest({
        task_id: "t_006",
        action_type: "dependency_change",
        description: "Update dependency",
        details: { affected_files: ["package.json"], affected_commands: ["bun install"], risk_level: "low" },
      }),
    ])
    for (const line of result.stdout) {
      expect(typeof line.type).toBe("string")
      expect(typeof line.id).toBe("string")
      expect(typeof line.timestamp).toBe("string")
      expect(line.payload).toBeDefined()
    }
  })

  test("stderr used for diagnostics only", async () => {
    const result = await runGateway([
      approvalRequest({ task_id: "t_007", action_type: "destructive" }, "appr_diag"),
    ])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(typeof result.stderr).toBe("string")
  })

  test("rejects oversized approval request with MESSAGE_TOO_LARGE", async () => {
    const result = await runGateway([
      handshake(),
      approvalRequest({
        task_id: "t_big",
        action_type: "destructive",
        description: "x".repeat(20_000),
        details: { affected_files: [], affected_commands: [], risk_level: "low" },
      }),
    ])
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.request",
      error_code: "MESSAGE_TOO_LARGE",
    })
    expect(result.stderr).toContain("oversized approval request rejected")
  })

  test("approval.response is acknowledged after handshake", async () => {
    const result = await runGateway([
      handshake(),
      {
        type: "request",
        id: "appr_resp_001",
        timestamp: "2026-07-10T14:33:00.000Z",
        payload: {
          method: "approval.response",
          approved: true,
          reason: "Platform approved",
          approved_by: "majed",
        },
      },
    ])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].payload).toMatchObject({
      method: "approval.response",
      acknowledged: true,
    })
  })

  test("rejects approval.response before handshake", async () => {
    const result = await runGateway([
      {
        type: "request",
        id: "appr_resp_002",
        timestamp: "2026-07-10T14:33:00.000Z",
        payload: {
          method: "approval.response",
          approved: true,
          reason: "Too early",
          approved_by: "majed",
        },
      },
    ])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(result.stdout[0].payload).toMatchObject({
      method: "approval.response",
      error_code: "APPROVAL_BEFORE_HANDSHAKE",
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
