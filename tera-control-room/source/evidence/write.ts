// Tera Control Room — Phase 0 PoC
// Evidence writer — serializes the EvidenceBundle to a pretty-printed JSON
// file alongside sibling stdout.log, stderr.log, and git.diff files.
// A light regex pass redacts obvious secret patterns from logs before write.

import path from "node:path"
import type { EvidenceBundle } from "../contracts/types.ts"

const SECRET_PATTERNS: Array<[RegExp, string]> = [
  [/sk-[A-Za-z0-9]{16,}/g, "sk-[REDACTED]"],
  [/AKIA[0-9A-Z]{16}/g, "AKIA[REDACTED]"],
  [/ghp_[A-Za-z0-9]{36}/g, "ghp_[REDACTED]"],
  [/gho_[A-Za-z0-9]{36}/g, "gho_[REDACTED]"],
  [/github_pat_[A-Za-z0-9_]{82}/g, "github_pat_[REDACTED]"],
  [/xox[baprs]-[A-Za-z0-9-]{10,}/g, "xox[REDACTED]"],
  [/AIza[0-9A-Za-z\-_]{35}/g, "AIza[REDACTED]"],
  [/-----BEGIN [A-Z ]+PRIVATE KEY-----[\s\S]*?-----END [A-Z ]+PRIVATE KEY-----/g, "[REDACTED PRIVATE KEY]"],
]

export function redactSecrets(text: string): string {
  let out = text
  for (const [re, repl] of SECRET_PATTERNS) {
    out = out.replace(re, repl)
  }
  return out
}

export interface WriteEvidenceOptions {
  gitDiff?: string
}

export async function writeEvidenceBundle(
  evidence: EvidenceBundle,
  taskId: string,
  opts: WriteEvidenceOptions = {},
): Promise<{ dir: string; stdoutPath: string; stderrPath: string; gitDiffPath: string; evidencePath: string }> {
  const evidenceRoot = path.resolve(process.cwd(), "evidence")
  const dir = path.join(evidenceRoot, taskId || "INVALID")
  await mkdirp(dir)

  const stdoutClean = redactSecrets(evidence.stdout_log || "")
  const stderrClean = redactSecrets(evidence.stderr_log || "")
  const gitDiffClean = redactSecrets(opts.gitDiff ?? "")

  const evidencePath = path.join(dir, "evidence.json")
  const stdoutPath = path.join(dir, "stdout.log")
  const stderrPath = path.join(dir, "stderr.log")
  const gitDiffPath = path.join(dir, "git.diff")

  await Bun.write(stdoutPath, stdoutClean)
  await Bun.write(stderrPath, stderrClean)
  await Bun.write(gitDiffPath, gitDiffClean)

  // Stash the resolved sibling paths back into the evidence bundle so the
  // report can reference them. Use a clone to avoid mutating the input shape
  // mid-write; const-reassignment is fine because we re-emit a new object.
  const bundleForWrite: EvidenceBundle = {
    ...evidence,
    stdout_log: `[see sibling file: ${path.basename(stdoutPath)} — content redacted and truncated in JSON]`,
    stderr_log: `[see sibling file: ${path.basename(stderrPath)} — content redacted and truncated in JSON]`,
    git_diff_path: gitDiffPath,
  }
  await Bun.write(evidencePath, JSON.stringify(bundleForWrite, null, 2))

  return { dir, stdoutPath, stderrPath, gitDiffPath, evidencePath }
}

async function mkdirp(dir: string): Promise<void> {
  // Bun has no native mkdir recursive in `Bun.fs`; fall back to Node fs.
  const fs = await import("node:fs/promises")
  await fs.mkdir(dir, { recursive: true })
}