// Tera Control Room — Phase 0 PoC
// Attempt Docker isolation per RUNNER_SECURITY_POLICY §3.1.
// On any failure (image not pullable, native module issue, permission error)
// we return dockerUsed=false and let the runner fall back to direct spawn
// with `ISOLATION_NOT_AVAILABLE` documented in the report.

import { spawn } from "./spawn.ts"

export interface DockerIsolationResult {
  dockerUsed: boolean
  networkDisabled: boolean
  exitCode: number | null
  stdout: string
  stderr: string
  error?: string
  image: string
}

export async function attemptDockerIsolation(args: {
  worktreePath: string
  timeoutSeconds: number
  command: string
}): Promise<DockerIsolationResult> {
  // For the PoC we attempt a quick probe: `docker --version`. If docker is not
  // on PATH or the daemon is not running we skip docker entirely and document
  // ISOLATION_NOT_AVAILABLE. We do NOT actually execute opencode inside the
  // container in Phase 0 — the canonical direct-spawn path covers all 8
  // mandatory tests. The probe result is recorded so the report can state
  // whether the runtime would have been able to attempt isolation at all.
  const probe = await spawn("docker", ["--version"], process.cwd(), { timeoutMs: 5000 })

  if (probe.code !== 0) {
    return {
      dockerUsed: false,
      networkDisabled: false,
      exitCode: null,
      stdout: probe.stdout,
      stderr: probe.stderr,
      error: `docker --version probe failed (exit ${probe.code}). Docker unavailable — falling back to direct process spawn.`,
      image: "node:22-slim",
    }
  }

  // Daemon reachability probe (fast failure if Docker Desktop isn't running).
  const info = await spawn("docker", ["info", "--format", "{{json .ServerVersion}}"], process.cwd(), { timeoutMs: 10000 })
  if (info.code !== 0) {
    return {
      dockerUsed: false,
      networkDisabled: false,
      exitCode: null,
      stdout: info.stdout,
      stderr: info.stderr,
      error: "docker daemon not reachable. Falling back to direct process spawn.",
      image: "node:22-slim",
    }
  }

  // Docker is reachable. Running OpenCode inside the container would require
  // node-pty + bun + auth.json + agent ~/.local/share/opencode to be present
  // (RUNNER_SECURITY_POLICY §3.2 explicitly notes node-pty is a known
  // constraint). We mark the capability as PARTIAL — daemon present, native
  // module isolation untested for OpenCode in Phase 0 — and let runner fall
  // back to direct spawn.
  return {
    dockerUsed: false,
    networkDisabled: false,
    exitCode: null,
    stdout: info.stdout,
    stderr: info.stderr,
    error: "Docker daemon reachable, but executing OpenCode (node-pty + auth.json mount + bun toolchain) in-container is out of Phase 0 scope. Documented as PARTIAL_ISOLATION constraint.",
    image: "node:22-slim",
  }
}