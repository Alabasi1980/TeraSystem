import { describe, expect, test } from "bun:test"
import { GatewayProtocol } from "@/gateway/protocol"
import { GatewayStdio } from "@/gateway/stdio"

describe("gateway context api", () => {
  test("accepts handshake version 1.2 and advertises context, task, and approval", async () => {
    const result = await runGateway([handshake()])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("response")
    expect(result.stdout[0].id).toBe("handshake_001")
    expect(result.stdout[0].payload).toMatchObject({
      method: "handshake",
      status: "ok",
      contract_version: "1.2",
      supported_methods: ["context", "task", "approval", "workspace"],
    })
  })

  test("rejects incompatible handshake versions with structured protocol output", async () => {
    const result = await runGateway([handshake({ contract_version: "9.9" }), context()])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("response")
    expect(result.stdout[0].payload).toMatchObject({
      method: "handshake",
      status: "error",
      error: { code: "VERSION_MISMATCH" },
    })
    expect(result.stderr).toContain("incompatible handshake rejected")
  })

  test("rejects context before successful handshake", async () => {
    const result = await runGateway([context()])
    expect(result.stdout).toHaveLength(1)
    expect(result.stdout[0].type).toBe("error")
    expect(result.stdout[0].id).toBe("ctx_001")
    expect(result.stdout[0].payload).toMatchObject({
      method: "context",
      error_code: "CONTEXT_BEFORE_HANDSHAKE",
    })
  })

  test("accepts context after handshake and preserves correlation id", async () => {
    const result = await runGateway([handshake(), context({ id: "ctx_same_id" })])
    expect(result.stderr).toBe("")
    expect(result.stdout).toHaveLength(2)
    expect(result.stdout[1].type).toBe("response")
    expect(result.stdout[1].id).toBe("ctx_same_id")
    expect(result.stdout[1].payload).toEqual({ method: "context", status: "ok", acknowledged: true })
  })

  test("stdio emits protocol json lines on stdout and diagnostics on stderr", async () => {
    const result = await runGatewayStdio([handshake(), context({ id: "ctx_stdio" })])
    expect(result.stderr).toBe("")
    expect(result.stdoutLines).toHaveLength(2)
    expect(result.stdoutLines.every((line) => JSON.parse(line).type === "response")).toBe(true)
    expect(JSON.parse(result.stdoutLines[1]).id).toBe("ctx_stdio")
  })

  test("rejects context when workspace identity differs from handshake", async () => {
    const result = await runGateway([handshake(), context({ workspace_id: "ws_other" })])
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].payload).toMatchObject({
      method: "context",
      error_code: "INVALID_WORKSPACE",
    })
  })

  test("rejects oversized context request with MESSAGE_TOO_LARGE", async () => {
    const result = await runGateway([handshake(), context({ context_data: "x".repeat(1_049_000) })])
    expect(result.stdout[1].type).toBe("error")
    expect(result.stdout[1].id).toBe("ctx_001")
    expect(result.stdout[1].payload).toMatchObject({
      method: "context",
      error_code: "MESSAGE_TOO_LARGE",
    })
    expect(result.stderr).toContain("oversized context request rejected")
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

async function runGatewayStdio(messages: object[]) {
  const stdoutChunks: string[] = []
  const stderrChunks: string[] = []
  await GatewayStdio.runGatewayStdio({
    stdin: asyncIterable(messages.map((message) => JSON.stringify(message) + "\n")) as unknown as NodeJS.ReadableStream,
    stdout: writable(stdoutChunks) as unknown as NodeJS.WritableStream,
    stderr: writable(stderrChunks) as unknown as NodeJS.WritableStream,
  })
  return {
    stdoutLines: stdoutChunks
      .join("")
      .split("\n")
      .filter((line) => line.length > 0),
    stderr: stderrChunks.join(""),
  }
}

function asyncIterable(chunks: string[]) {
  return {
    async *[Symbol.asyncIterator]() {
      yield* chunks
    },
  }
}

function writable(chunks: string[]) {
  return {
    write(chunk: string | Buffer) {
      chunks.push(Buffer.isBuffer(chunk) ? chunk.toString("utf8") : chunk)
      return true
    },
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

function context(payload?: Record<string, unknown>) {
  return {
    type: "request",
    id: String(payload?.id ?? "ctx_001"),
    timestamp: "2026-07-10T14:30:01.000Z",
    payload: {
      method: "context",
      context_type: "project",
      context_data: "context",
      workspace_id: "ws_abc123",
      project_id: "proj_xyz789",
      capabilities: capabilities(),
      ...payload,
    },
  }
}

function capabilities() {
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
