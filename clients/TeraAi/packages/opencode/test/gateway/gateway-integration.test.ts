import { describe, expect, test, afterEach } from "bun:test"
import path from "node:path"

const GATEWAY_CWD = path.join(import.meta.dir, "../..")
const GATEWAY_ARGS = ["run", "--conditions=browser", "./src/index.ts", "gateway"] as const

interface GatewayResponse {
  type: string
  id: string
  timestamp: string
  payload: Record<string, unknown>
}

describe("gateway integration (child process)", () => {
  let proc: ReturnType<typeof Bun.spawn>

  afterEach(() => {
    if (proc && !proc.killed) {
      try { proc.kill("SIGTERM") } catch { /* already dead */ }
    }
  })

  /**
   * Spawn a fresh gateway process and return helpers.
   * Uses raw stream reader instead of async generator to avoid edge cases.
   */
  function startGateway() {
    proc = Bun.spawn([process.execPath, ...GATEWAY_ARGS], {
      cwd: GATEWAY_CWD,
      stdin: "pipe",
      stdout: "pipe",
      stderr: "pipe",
    })

    const stdin = proc.stdin as unknown as {
      write(data: string): number
      end(): void
    }
    const stdoutStream = proc.stdout as unknown as ReadableStream<Uint8Array>

    // Buffered line reader
    const reader = stdoutStream.getReader()
    const decoder = new TextDecoder()
    let buffer = ""

    async function readNextLine(): Promise<string> {
      while (true) {
        const newlineIndex = buffer.indexOf("\n")
        if (newlineIndex >= 0) {
          const line = buffer.slice(0, newlineIndex)
          buffer = buffer.slice(newlineIndex + 1)
          return line
        }
        const { done, value } = await reader.read()
        if (done) {
          reader.releaseLock()
          if (buffer.length > 0) {
            const line = buffer
            buffer = ""
            return line
          }
          throw new Error("stdout stream ended")
        }
        buffer += decoder.decode(value, { stream: true })
      }
    }

    const stderrStream = proc.stderr as unknown as ReadableStream<Uint8Array>

    return {
      sendRequest: async (request: object): Promise<GatewayResponse> => {
        stdin.write(JSON.stringify(request) + "\n")
        const line = await readNextLine()
        return JSON.parse(line) as GatewayResponse
      },

      finish: async (): Promise<{ stderr: string }> => {
        stdin.end()
        await proc.exited
        reader.releaseLock()
        return { stderr: await Bun.readableStreamToText(stderrStream) }
      },
    }
  }

  // ---------------------------------------------------------------------------
  // Full protocol flow
  // ---------------------------------------------------------------------------

  test("full protocol flow: handshake then context then task then approval", async () => {
    const gw = startGateway()

    // 1. Handshake
    const hs = await gw.sendRequest(handshake())
    expect(hs.type).toBe("response")
    expect(hs.id).toBe("handshake_001")
    expect(typeof hs.timestamp).toBe("string")
    expect(hs.payload).toMatchObject({
      method: "handshake",
      status: "ok",
      contract_version: "1.2",
      engine_version: "0.1.0",
      supported_methods: ["context", "task", "approval"],
    })

    // 2. Context
    const ctx = await gw.sendRequest(context())
    expect(ctx.type).toBe("response")
    expect(ctx.id).toBe("ctx_001")
    expect(ctx.payload).toMatchObject({
      method: "context",
      status: "ok",
      acknowledged: true,
    })

    // 3. Task.create
    const task1 = await gw.sendRequest(task({ action: "create", task_id: "t_001" }))
    expect(task1.type).toBe("response")
    expect(task1.id).toBe("task_001")
    expect(task1.payload).toMatchObject({
      method: "task",
      action: "create",
      status: "created",
      task_id: "t_001",
    })

    // 4. Approval.request (non-critical risk → auto-approved)
    const appr = await gw.sendRequest(
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
    )
    expect(appr.type).toBe("response")
    expect(appr.id).toBe("appr_001")
    expect(appr.payload).toMatchObject({
      method: "approval.response",
      approved: true,
      approved_by: "engine",
    })

    const { stderr } = await gw.finish()
    expect(typeof stderr).toBe("string")
  })

  // ---------------------------------------------------------------------------
  // Response format validation
  // ---------------------------------------------------------------------------

  test("all responses are valid JSON Lines with envelope structure", async () => {
    const gw = startGateway()

    const responses = [
      await gw.sendRequest(handshake()),
      await gw.sendRequest(context()),
      await gw.sendRequest(task({ action: "create", task_id: "t_002" })),
      await gw.sendRequest(
        approvalRequest({
          task_id: "t_002",
          action_type: "destructive",
          details: { affected_files: [], affected_commands: [], risk_level: "low" },
        }),
      ),
    ]

    for (const response of responses) {
      expect(typeof response.type).toBe("string")
      expect(["response", "error"]).toContain(response.type)
      expect(typeof response.id).toBe("string")
      expect(response.id.length).toBeGreaterThan(0)
      expect(typeof response.timestamp).toBe("string")
      expect(new Date(response.timestamp).toISOString()).toBe(response.timestamp)
      expect(response.payload).toBeDefined()
      expect(typeof response.payload).toBe("object")
      expect(Array.isArray(response.payload)).toBe(false)
    }

    const { stderr } = await gw.finish()
    expect(typeof stderr).toBe("string")
  })

  // ---------------------------------------------------------------------------
  // Handshake
  // ---------------------------------------------------------------------------

  test("handshake announces supported methods", async () => {
    const gw = startGateway()
    const response = await gw.sendRequest(handshake())
    expect(response.payload).toMatchObject({
      method: "handshake",
      status: "ok",
      supported_methods: ["context", "task", "approval"],
    })
    await gw.finish()
  })

  test("handshake response carries same id as request", async () => {
    const gw = startGateway()
    const response = await gw.sendRequest(handshake({}, "my_hs_001"))
    expect(response.id).toBe("my_hs_001")
    await gw.finish()
  })

  // ---------------------------------------------------------------------------
  // Context
  // ---------------------------------------------------------------------------

  test("context is accepted after handshake with id correlation", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const ctxResponse = await gw.sendRequest(context({}, "ctx_custom_id"))
    expect(ctxResponse.id).toBe("ctx_custom_id")
    expect(ctxResponse.payload).toMatchObject({
      method: "context",
      status: "ok",
      acknowledged: true,
    })
    await gw.finish()
  })

  // ---------------------------------------------------------------------------
  // Task lifecycle
  // ---------------------------------------------------------------------------

  test("task.status returns created after task.create", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    await gw.sendRequest(task({ action: "create", task_id: "t_lifecycle" }, "task_create"))
    const statusResponse = await gw.sendRequest(
      task({ action: "status", task_id: "t_lifecycle" }, "task_status"),
    )
    expect(statusResponse.payload).toMatchObject({
      method: "task",
      action: "status",
      status: "created",
      task_id: "t_lifecycle",
    })
    await gw.finish()
  })

  test("task.status returns unknown for nonexistent task", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const statusResponse = await gw.sendRequest(
      task({ action: "status", task_id: "t_ghost" }, "task_ghost"),
    )
    expect(statusResponse.payload).toMatchObject({
      method: "task",
      action: "status",
      status: "unknown",
      task_id: "t_ghost",
    })
    await gw.finish()
  })

  test("task.cancel returns status cancelled", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const cancelResponse = await gw.sendRequest(
      task({ action: "cancel", task_id: "t_cancel" }, "task_cancel"),
    )
    expect(cancelResponse.payload).toMatchObject({
      method: "task",
      action: "cancel",
      status: "cancelled",
      task_id: "t_cancel",
    })
    await gw.finish()
  })

  // ---------------------------------------------------------------------------
  // Approval
  // ---------------------------------------------------------------------------

  test("approval.request auto-approves non-critical risk", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const apprResponse = await gw.sendRequest(
      approvalRequest(
        {
          task_id: "t_auto",
          action_type: "destructive",
          description: "Auto-approve test",
          details: { affected_files: [], affected_commands: [], risk_level: "low" },
        },
        "appr_auto",
      ),
    )
    expect(apprResponse.payload).toMatchObject({
      method: "approval.response",
      approved: true,
      approved_by: "engine",
    })
    await gw.finish()
  })

  test("approval.request denies critical risk", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const apprResponse = await gw.sendRequest(
      approvalRequest(
        {
          task_id: "t_crit",
          action_type: "security_sensitive",
          description: "Modify auth config",
          details: { affected_files: ["src/auth/config.ts"], affected_commands: [], risk_level: "critical" },
        },
        "appr_crit",
      ),
    )
    expect(apprResponse.payload).toMatchObject({
      method: "approval.response",
      approved: false,
      approved_by: "engine",
    })
    await gw.finish()
  })

  test("approval.response is acknowledged after handshake", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    const apprResponse = await gw.sendRequest({
      type: "request",
      id: "appr_resp_001",
      timestamp: "2026-07-10T14:33:00.000Z",
      payload: {
        method: "approval.response",
        approved: true,
        reason: "Platform approved",
        approved_by: "majed",
      },
    })
    expect(apprResponse.payload).toMatchObject({
      method: "approval.response",
      acknowledged: true,
    })
    await gw.finish()
  })

  // ---------------------------------------------------------------------------
  // stderr discipline
  // ---------------------------------------------------------------------------

  test("stderr does not contain protocol JSON lines", async () => {
    const gw = startGateway()
    await gw.sendRequest(handshake())
    await gw.sendRequest(context())
    await gw.sendRequest(task({ action: "create", task_id: "t_stderr" }))
    await gw.sendRequest(
      approvalRequest({
        task_id: "t_stderr",
        action_type: "destructive",
        details: { affected_files: [], affected_commands: [], risk_level: "low" },
      }),
    )
    const { stderr } = await gw.finish()

    expect(stderr).not.toContain('"type":"response"')
    expect(stderr).not.toContain('"type":"error"')
    expect(stderr).not.toContain('"type":"request"')
    expect(stderr).not.toContain('"payload"')
  })
})

// ---------------------------------------------------------------------------
// Message factories
// ---------------------------------------------------------------------------

function handshake(extra?: Record<string, unknown>, id = "handshake_001"): object {
  return {
    type: "request",
    id,
    timestamp: "2026-07-10T14:30:00.000Z",
    payload: {
      method: "handshake",
      contract_version: "1.2",
      platform_version: "0.1.0",
      engine_version: "0.1.0",
      workspace_id: "ws_abc123",
      project_id: "proj_xyz789",
      ...extra,
    },
  }
}

function context(extra?: Record<string, unknown>, id = "ctx_001"): object {
  return {
    type: "request",
    id,
    timestamp: "2026-07-10T14:30:01.000Z",
    payload: {
      method: "context",
      context_type: "project",
      context_data: "Integration test context",
      workspace_id: "ws_abc123",
      project_id: "proj_xyz789",
      capabilities: defaultCapabilities(),
      ...extra,
    },
  }
}

function task(payload: Record<string, unknown>, id = "task_001"): object {
  return {
    type: "request",
    id,
    timestamp: "2026-07-10T14:30:02.000Z",
    payload: { method: "task", ...payload },
  }
}

function approvalRequest(payload: Record<string, unknown>, id = "appr_001"): object {
  return {
    type: "request",
    id,
    timestamp: "2026-07-10T14:32:00.000Z",
    payload: { method: "approval.request", ...payload },
  }
}

function defaultCapabilities() {
  return {
    allowed_read_paths: ["src/", "tests/"],
    allowed_write_paths: ["src/"],
    forbidden_paths: ["*.env", ".secrets/"],
    allowed_commands: ["bun test"],
    forbidden_commands: ["rm -rf"],
    network_policy: { internet_access: false, allowed_domains: [] },
    approval_required: {
      destructive_actions: true,
      security_sensitive: true,
      major_dependency_changes: true,
    },
  }
}
