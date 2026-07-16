// Tera Control Room — Phase 0 PoC
// Adapter — spawns `opencode run` as a subprocess with a sanitized env, streams
// stdout/stderr to buffers, applies a hard external timeout (taskkill fallback
// per RUNNER_SECURITY_POLICY §6), and extracts the Handback from the emitted
// JSONL event stream.
//
// This file deliberately contains no business decisions beyond "execute this
// contract against this worktree and report what happened" — the runner layer
// above it adjudicates permission policy and security classification.

import type {
  AdapterResult,
  HandbackValidation,
  TaskContract,
} from "../contracts/types.ts"
import {
  type HandbackValidationResult,
  sanitizeEnvForChild,
  validateHandbackFromJsonl,
} from "../contracts/validate.ts"

export interface AdapterArgs {
  contract: TaskContract
  /** Absolute path to the git worktree that will be the agent's cwd. */
  worktreePath: string
  timeoutSeconds: number
  /**
   * Optional override of the prompt body. When undefined, the adapter
   * generates the standard prompt from the contract (TASK_CONTRACT_V1 §4).
   * When provided, the adapter still wraps the body with the Handback
   * instruction footer so that valid-handback tests can succeed.
   */
  promptOverride?: string
  /**
   * Provider/model id to pass via `--model`. Defaults to "opencode-go/glm-5.2"
   * which is the verified active model on this machine. Override via
   * `OPENCODE_MODEL` env var when running in other environments.
   */
  modelId?: string
}

// Pre-discovered constraint substitution — documented in the PoC report.
// `opencode run --agent` requires a primary-mode agent; `engineering-agent`
// is `mode: subagent` and gets silently replaced. We default the test
// fixtures to `build` (OpenCode's built-in primary dev agent).
export const DEFAULT_AGENT_ID = "build"

export function buildPromptFromContract(contract: TaskContract): string {
  const cmds = contract.allowed_commands.length
    ? contract.allowed_commands.join("; ")
    : "(none)"
  return [
    `You are executing Task ${contract.task_id} as agent ${contract.agent_id}.`,
    "",
    "OBJECTIVE:",
    contract.objective,
    "",
    "CONSTRAINTS:",
    `- You may only write to paths matching: ${contract.allowed_write_paths.join(", ")}.`,
    `- You may only read from paths matching: ${contract.allowed_read_paths.join(", ")}.`,
    `- Authorized commands: ${cmds}.`,
    `- Timeout: ${contract.timeout_seconds}s.`,
    "",
    "REQUIRED HANDBACK:",
    "When you finish, output a single fenced JSON block (```json ... ```)",
    "conforming to the handback-v1 schema:",
    `- schema_version: "1.0"`,
    `- task_id: must equal "${contract.task_id}"`,
    `- agent_id: must equal "${contract.agent_id}"`,
    `- status: one of COMPLETED, FAILED, PARTIAL_SUCCESS`,
    `- summary: short text`,
    `- files_changed: list of relative paths actually modified`,
    `- commands_executed: array of { command, exit_code }`,
    `- known_issues: array of short notes`,
    `- recommended_next_action: REVIEW or RETRY or ESCALATE`,
    "",
    "Do not output anything after the Handback JSON block. Do not output prose handback.",
  ].join("\n")
}

export async function runAdapter(args: AdapterArgs): Promise<AdapterResult> {
  const startedAt = Date.now()
  const contract = args.contract
  const prompt = args.promptOverride
    ? args.promptOverride + "\n\n" + buildHandbackFooter(contract)
    : buildPromptFromContract(contract)

  // SPIKE NOTE: On Windows, `opencode.cmd` is a batch shim. Passing the prompt
  // as a positional argv element strips newlines (cmd.exe truncates at the
  // first \n). We instead pipe the prompt through stdin — `opencode run`
  // supports `process.stdin` piping per `clients\TeraAi\...\run.ts` §416
  // (`piped = process.stdin.isTTY ? undefined : await Bun.stdin.text()`).
  // This keeps multi-line prompts verbatim across the OS boundary.
  const cmd = [
    "opencode", "run",
    "--agent", contract.agent_id,
    "--model", args.modelId ?? process.env.OPENCODE_MODEL ?? "opencode-go/glm-5.2",
    "--dir", args.worktreePath,
    "--format", "json",
    "--auto",
  ]

  let proc: ReturnType<typeof Bun.spawn>
  try {
    // Bun.spawn accepts `stdin: <Uint8Array>` — it writes the buffer to the
    // child's stdin and closes it automatically. Passing the multi-line
    // prompt as a Uint8Array preserves the newlines (Windows argv-via-cmd /c
    // would strip them at the first \n; stdin does not).
    const promptBytes = new TextEncoder().encode(prompt)
    proc = Bun.spawn({
      cmd,
      cwd: args.worktreePath,
      stdin: promptBytes,
      stdout: "pipe",
      stderr: "pipe",
      env: sanitizeEnvForChild(process.env),
    })
  } catch (err) {
    return finalize(startedAt, null, "", `Failed to spawn opencode: ${(err as Error).message}`, {
      valid: false,
      errors: ["Failed to spawn opencode"],
    }, false)
  }

  // stdin is fed by Bun directly from the TypedArray above — nothing to await
  // here, but keep a no-op promise so the cleanup Promise.allSettled shape
  // remains unchanged.
  const stdinWriter = Promise.resolve()

  const pid = proc.pid!
  const stdoutChunks: Buffer[] = []
  const stderrChunks: Buffer[] = []
  const stdoutPromise = drainToBuffer(proc.stdout, stdoutChunks)
  const stderrPromise = drainToBuffer(proc.stderr, stderrChunks)

  const timeoutMs = Math.max(1, Math.floor(args.timeoutSeconds * 1000))
  let killTimer: ReturnType<typeof setTimeout> | null = null
  let timedOut = false
  let exitedCode: number | null = null

  const exitPromise = (async () => proc.exited)()

  const raceResult = await Promise.race([
    exitPromise.then((code) => ({ kind: "exited" as const, code })),
    new Promise<{ kind: "timeout" }>((resolve) => {
      killTimer = setTimeout(() => resolve({ kind: "timeout" }), timeoutMs)
    }),
  ])

  if (raceResult.kind === "timeout") {
    timedOut = true
    await killProcessTree(pid)
    // Give the OS a moment to finalize the killed process.
    try {
      exitedCode = await Promise.race([
        exitPromise,
        new Promise<number | null>((r) => setTimeout(() => r(null), 1500)),
      ])
    } catch {
      exitedCode = null
    }
  } else {
    exitedCode = raceResult.code
  }

  if (killTimer) clearTimeout(killTimer)
  await Promise.allSettled([stdoutPromise, stderrPromise, stdinWriter])

  const stdoutText = Buffer.concat(stdoutChunks).toString("utf8")
  const stderrText = Buffer.concat(stderrChunks).toString("utf8")

  if (timedOut) {
    return finalize(startedAt, exitedCode, stdoutText, stderrText, { valid: false, errors: ["Task timed out — handback not parsed."] }, true)
  }

  const handbackResult: HandbackValidationResult = validateHandbackFromJsonl(
    stdoutText,
    contract.task_id,
    contract.agent_id,
  )

  return finalize(startedAt, exitedCode, stdoutText, stderrText, handbackResult, false)
}

function finalize(
  startedAt: number,
  exitCode: number | null,
  stdoutText: string,
  stderrText: string,
  handbackResult: HandbackValidationResult,
  timedOut: boolean,
): AdapterResult {
  const durationMs = Date.now() - startedAt
  const rawJsonl = stdoutText.split(/\r?\n/).filter((l) => l.trim().length > 0)

  const handbackValidation: HandbackValidation | null = timedOut
    ? null
    : { valid: handbackResult.valid, errors: handbackResult.valid ? [] : handbackResult.errors }

  if (timedOut) {
    return {
      status: "TASK_TIMEOUT",
      exitCode,
      stdout: stdoutText,
      stderr: stderrText,
      handback: null,
      handbackValidation,
      timedOut: true,
      durationMs,
      rawJsonl,
    }
  }

  if (handbackResult.valid) {
    return {
      status: "COMPLETED",
      exitCode,
      stdout: stdoutText,
      stderr: stderrText,
      handback: handbackResult.handback,
      handbackValidation: { valid: true, errors: [] },
      timedOut: false,
      durationMs,
      rawJsonl,
    }
  }

  const hasVisibleText = /"type"\s*:\s*"text"/.test(stdoutText)
  if (exitCode !== null && exitCode !== 0 && !hasVisibleText) {
    return {
      status: "EXECUTION_FAILED",
      exitCode,
      stdout: stdoutText,
      stderr: stderrText,
      handback: null,
      handbackValidation,
      timedOut: false,
      durationMs,
      rawJsonl,
    }
  }

  return {
    status: "INVALID_HANDBACK",
    exitCode,
    stdout: stdoutText,
    stderr: stderrText,
    handback: null,
    handbackValidation,
    timedOut: false,
    durationMs,
    rawJsonl,
  }
}

function buildHandbackFooter(contract: TaskContract): string {
  return [
    "",
    "REQUIRED HANDBACK:",
    "When you finish, output a single fenced JSON block (```json ... ```)",
    "conforming to the handback-v1 schema:",
    `- schema_version: "1.0"`,
    `- task_id: must equal "${contract.task_id}"`,
    `- agent_id: must equal "${contract.agent_id}"`,
    `- status: one of COMPLETED, FAILED, PARTIAL_SUCCESS`,
    `- summary: short text`,
    `- files_changed: list of relative paths actually modified`,
    `- commands_executed: array of { command, exit_code }`,
    `- known_issues: array of short notes`,
    `- recommended_next_action: REVIEW or RETRY or ESCALATE`,
    "Do not output anything after the Handback JSON block.",
  ].join("\n")
}

// --- helpers ---

async function drainToBuffer(stream: ReadableStream<Uint8Array> | null, sink: Buffer[]): Promise<void> {
  if (!stream) return
  const reader = stream.getReader()
  for (;;) {
    const { value, done } = await reader.read()
    if (done) break
    if (value && value.length > 0) sink.push(Buffer.from(value))
  }
}

// Windows-safe process-tree kill per RUNNER_SECURITY_POLICY §6:
// SIGTERM-equivalent via `taskkill /T /PID`, escalate to `taskkill /F /T /PID`
// if the first attempt fails or the process is still alive.
async function killProcessTree(pid: number): Promise<void> {
  try {
    await runCmd(["taskkill", "/T", "/PID", String(pid)])
    return
  } catch {
    /* escalate */
  }
  try {
    await runCmd(["taskkill", "/F", "/T", "/PID", String(pid)])
  } catch {
    try { process.kill(pid) } catch { /* best effort */ }
  }
}

function runCmd(cmd: string[]): Promise<void> {
  return new Promise((resolve, reject) => {
    const p = Bun.spawn({ cmd, stdout: "ignore", stderr: "ignore" })
    p.exited.then((code) => (code === 0 ? resolve() : reject(new Error("non-zero")))).catch(reject)
  })
}