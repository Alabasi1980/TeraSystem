// Tera Control Room — Phase 0 PoC
// Tiny cross-platform spawn helper around Bun.spawn — returns stdout/stderr as
// strings plus the exit code. Used by the worktree manager and security
// sandbox probe so neither has to re-implement stream draining.

export interface SpawnResult {
  code: number
  stdout: string
  stderr: string
}

export async function spawn(cmd: string, args: string[], cwd: string, opts?: { env?: Record<string, string | undefined>; timeoutMs?: number }): Promise<SpawnResult> {
  const proc = Bun.spawn({
    cmd: [cmd, ...args],
    cwd,
    stdout: "pipe",
    stderr: "pipe",
    env: opts?.env ?? process.env,
  })

  const stdoutPromise = drain(proc.stdout)
  const stderrPromise = drain(proc.stderr)

  let timedOut = false
  let timer: ReturnType<typeof setTimeout> | null = null
  if (opts?.timeoutMs) {
    timer = setTimeout(() => {
      timedOut = true
      try { proc.kill() } catch { /* ignore */ }
    }, opts.timeoutMs)
  }

  const code = await proc.exited
  if (timer) clearTimeout(timer)

  const [stdout, stderr] = await Promise.all([stdoutPromise, stderrPromise])
  return { code: timedOut ? -1 : code, stdout, stderr }
}

async function drain(stream: ReadableStream<Uint8Array> | null): Promise<string> {
  if (!stream) return ""
  const reader = stream.getReader()
  const chunks: Buffer[] = []
  for (;;) {
    const { value, done } = await reader.read()
    if (done) break
    if (value && value.length) chunks.push(Buffer.from(value))
  }
  return Buffer.concat(chunks).toString("utf8")
}