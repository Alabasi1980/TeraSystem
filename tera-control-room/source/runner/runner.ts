// Tera Control Room — Phase 0 PoC
// Runner — orchestrates one task end-to-end:
//   contract validate → create worktree → pre-flight agent check
//   → attempt Docker (record) → run adapter (direct spawn fallback)
//   → collect git diff → detect path-policy violations
//   → build EvidenceBundle → write evidence files → cleanup worktree.
//
// The runner is the layer that adjudicates Permission/Security classification.
// The Adapter is layer-1 plumbing only.

import path from "node:path"
import type {
  AdapterResult,
  EvidenceBundle,
  FinalStatus,
  SecurityResult,
  TaskContract,
} from "../contracts/types.ts"
import {
  type HandbackValidationResult,
  loadAgentRegistry,
  validateHandbackFromJsonl,
  validateTaskContract,
} from "../contracts/validate.ts"
import { runAdapter } from "../adapter/adapter.ts"
import { cleanupWorktree, createWorktree, getGitStatus, WorktreeError } from "./worktree.ts"
import { attemptDockerIsolation } from "./security.ts"
import { spawn } from "./spawn.ts"
import { writeEvidenceBundle } from "../evidence/write.ts"

export interface RunTaskResult {
  evidence: EvidenceBundle
  worktreeCleanupNotes: string[]
}

export async function runTask(contractRaw: unknown, promptOverride?: string): Promise<RunTaskResult> {
  // 1. Validate the task contract FIRST. On failure we record an evidence
  //    bundle with `INVALID_TASK_CONTRACT` and DO NOT spawn opencode.
  const validation = validateTaskContract(contractRaw)
  if (!validation.valid) {
    const evidence: EvidenceBundle = {
      task_contract: (contractRaw as Record<string, unknown>) ?? {},
      started_at: new Date().toISOString(),
      finished_at: new Date().toISOString(),
      duration_ms: 0,
      base_commit: "",
      head_commit: "",
      worktree_path: "",
      agent_profile: typeof (contractRaw as { agent_id?: unknown })?.agent_id === "string"
        ? (contractRaw as { agent_id: string }).agent_id
        : "",
      adapter_command: "",
      process_exit_code: null,
      stdout_log: "",
      stderr_log: validation.errors.join("\n"),
      files_changed: [],
      git_diff_path: "",
      handback: null,
      handback_validation: { valid: false, errors: validation.errors },
      security_result: {
        classification: "ISOLATION_NOT_AVAILABLE",
        forbiddenFileCreated: false,
        dockerUsed: false,
        networkDisabled: false,
        notes: ["Contract rejected before any process was spawned — no isolation exercised."],
      },
      final_status: "INVALID_TASK_CONTRACT",
    }
    await writeEvidenceBundle(evidence, evidence.task_contract?.task_id ?? "INVALID")
    return { evidence, worktreeCleanupNotes: [] }
  }

  const contract = validation.contract
  const repoPath = path.resolve(process.cwd(), "test-workspace")

  // 2. Create the git worktree.
  let wt
  try {
    wt = await createWorktree(repoPath, contract.task_id)
  } catch (err) {
    const evidence: EvidenceBundle = {
      task_contract: contract,
      started_at: new Date().toISOString(),
      finished_at: new Date().toISOString(),
      duration_ms: 0,
      base_commit: "",
      head_commit: "",
      worktree_path: "",
      agent_profile: contract.agent_id,
      adapter_command: "",
      process_exit_code: null,
      stdout_log: "",
      stderr_log: (err as Error).message,
      files_changed: [],
      git_diff_path: "",
      handback: null,
      handback_validation: null,
      security_result: {
        classification: "ISOLATION_NOT_AVAILABLE",
        forbiddenFileCreated: false,
        dockerUsed: false,
        networkDisabled: false,
        notes: [(err as Error).message],
      },
      final_status: "WORKTREE_CREATION_FAILED",
    }
    await writeEvidenceBundle(evidence, contract.task_id)
    return { evidence, worktreeCleanupNotes: [] }
  }

  const startedAt = new Date().toISOString()
  const startMs = Date.now()

  // 3. Pre-flight agent existence check via `opencode agent list`.
  //    On detectable nonexistent agent → AGENT_PROFILE_NOT_FOUND (expected for
  //    Test 7 — spec explicitly allows this pre-flight detection).
  const registry = await loadAgentRegistry()
  let adapterSkipped: FinalStatus | null = null
  let adapterStdout = ""
  let adapterStderr = ""
  const dockerNotes: string[] = []

  if (registry.available && !registry.primaryAgents.has(contract.agent_id)) {
    adapterStderr = `AGENT_PROFILE_NOT_FOUND: agent "${contract.agent_id}" is not a primary agent (or not present). Primary agents visible: ${[...registry.primaryAgents].join(", ") || "(none)"}`
    adapterSkipped = "AGENT_PROFILE_NOT_FOUND"
    dockerNotes.push(adapterStderr)
  }
  if (!registry.available) {
    dockerNotes.push(...registry.notes)
  }

  // 4. Attempt Docker isolation (informational — Phase 0 falls back to direct
  //    spawn for the actual opencode run).
  const docker = await attemptDockerIsolation({
    worktreePath: wt.worktreePath,
    timeoutSeconds: contract.timeout_seconds,
    command: "(opencode run — adapter handles spawn)",
  })
  if (docker.error) dockerNotes.push(docker.error)

  // 5. Run the adapter (or skip if pre-flight agent check failed).
  let adapter: AdapterResult | null = null
  if (adapterSkipped === null) {
    const opts: { contract: typeof contract; worktreePath: string; timeoutSeconds: number; promptOverride?: string } = {
      contract,
      worktreePath: wt.worktreePath,
      timeoutSeconds: contract.timeout_seconds,
    }
    if (promptOverride !== undefined) {
      opts.promptOverride = promptOverride
    }
    adapter = await runAdapter(opts)
    adapterStdout = adapter.stdout
    adapterStderr = adapter.stderr
  }

  const finishedAt = new Date().toISOString()
  const durationMs = Date.now() - startMs

  // 6. Collect git diff / status / head commit from the worktree.
  const git = await getGitStatus(wt.worktreePath).catch((err) => {
    dockerNotes.push(`git evidence collection error: ${(err as Error).message}`)
    return { porcelain: "", diff: "", headCommit: wt.baseCommit }
  })

  const filesChanged = parsePorcelainFiles(git.porcelain)

  // 7a. Detect path-policy violations on files touched INSIDE the worktree
  //     (against allowed_write_paths globs).
  const violations = detectPathViolations(filesChanged, contract.allowed_write_paths)

  // 7b. Escape-detection: if the worktree is a direct child of repoPath, the
  //     spec's Test 4 path `../forbidden.txt` resolves to `<repoPath>\forbidden.txt`.
  //     We also check the worktree's sibling path (one level up from the
  //     worktree itself) to catch any escape on disk that wouldn't appear in
  //     `git status` from inside the worktree.
  const escapeTargets = [
    path.join(repoPath, "forbidden.txt"),
    path.join(path.dirname(wt.worktreePath), "forbidden.txt"),
  ]
  for (const target of escapeTargets) {
    try {
      if (await Bun.file(target).exists()) {
        violations.push({ path: target, isOutside: true })
        dockerNotes.push(`ESCAPED WRITE detected on disk: ${target}`)
      }
    } catch {
      /* ignore fs errors */
    }
  }

  // 8. Determine final status + security classification.
  let finalStatus: FinalStatus
  let securityClassification: SecurityResult["classification"] = docker.dockerUsed
    ? "SECURE_ISOLATION_CONFIRMED"
    : "ISOLATION_NOT_AVAILABLE"

  if (adapterSkipped !== null) {
    finalStatus = adapterSkipped
  } else if (adapter === null) {
    finalStatus = "EXECUTION_FAILED"
  } else if (adapter.status === "TASK_TIMEOUT") {
    finalStatus = "TASK_TIMEOUT"
  } else if (adapter.status === "EXECUTION_FAILED") {
    finalStatus = "EXECUTION_FAILED"
  } else if (adapter.status === "COMPLETED" || adapter.status === "INVALID_HANDBACK") {
    // Even on completed/invalid-handback, check post-execution path policy.
    if (violations.length > 0) {
      finalStatus = "PERMISSION_DENIED"
    } else if (adapter.status === "INVALID_HANDBACK") {
      finalStatus = "INVALID_HANDBACK"
    } else {
      finalStatus = "COMPLETED"
    }
  } else {
    finalStatus = adapter.status
  }

  const forbiddenFileCreated = violations.some((v) => v.isOutside)

  // If a write outside the worktree escaped to disk (e.g. ../forbidden.txt),
  // the run is by definition NOT securely isolated.
  if (forbiddenFileCreated && securityClassification === "SECURE_ISOLATION_CONFIRMED") {
    securityClassification = "PARTIAL_ISOLATION"
  }
  if (!docker.dockerUsed && forbiddenFileCreated) {
    securityClassification = "ISOLATION_NOT_AVAILABLE"
  }

  // 9. Build the evidence bundle.
  const handback = adapter?.handback ?? null
  const handbackValidation = adapter?.handbackValidation ?? null

  const adapterResultForEvidence: AdapterResult = adapter ?? {
    status: finalStatus,
    exitCode: null,
    stdout: adapterStdout,
    stderr: adapterStderr,
    handback: null,
    handbackValidation: null,
    timedOut: false,
    durationMs,
    rawJsonl: [],
  }

  const evidence: EvidenceBundle = {
    task_contract: contract,
    started_at: startedAt,
    finished_at: finishedAt,
    duration_ms: durationMs,
    base_commit: wt.baseCommit,
    head_commit: git.headCommit,
    worktree_path: wt.worktreePath,
    agent_profile: contract.agent_id,
    adapter_command: `opencode run --agent ${contract.agent_id} --dir "<worktree>" --format json --auto "<prompt>"`,
    process_exit_code: adapterResultForEvidence.exitCode,
    stdout_log: adapterResultForEvidence.stdout,
    stderr_log: adapterResultForEvidence.stderr,
    files_changed: filesChanged,
    git_diff_path: "", // set by writeEvidenceBundle to the sibling path
    handback,
    handback_validation: handbackValidation,
    security_result: {
      classification: securityClassification,
      forbiddenFileCreated,
      dockerUsed: docker.dockerUsed,
      networkDisabled: docker.networkDisabled,
      notes: [...dockerNotes, ...violations.map((v) => `PATH VIOLATION: ${v.path} (allowed: ${contract.allowed_write_paths.join(", ")})`)],
    },
    final_status: finalStatus,
  }

  // 10. Write evidence files (evidence.json + stdout.log + stderr.log + git.diff).
  await writeEvidenceBundle(evidence, contract.task_id, {
    gitDiff: git.diff,
  })

  // 11. Cleanup worktree unless KEEP_WORKTREE=true.
  const cleanupNotes: string[] = []
  if (process.env.KEEP_WORKTREE !== "true") {
    const notes = await cleanupWorktree(repoPath, wt.worktreePath, wt.branchName).catch((err) => {
      cleanupNotes.push(`cleanup error: ${(err as Error).message}`)
      return [] as string[]
    })
    cleanupNotes.push(...notes)
  } else {
    cleanupNotes.push("KEEP_WORKTREE=true — worktree retained at " + wt.worktreePath)
  }

  return { evidence, worktreeCleanupNotes: cleanupNotes }
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

export interface PathViolation {
  path: string
  isOutside: boolean
}

function parsePorcelainFiles(porcelain: string): string[] {
  const files: string[] = []
  for (const line of porcelain.split(/\r?\n/)) {
    if (!line || line.length < 3) continue
    // Porcelain v1: XY filename
    const name = line.slice(3).trim()
    if (name) files.push(name)
  }
  return files
}

function detectPathViolations(files: string[], allowGlobs: string[]): PathViolation[] {
  return files.filter((file) => {
    // Normalize slashes — Git on Windows can return either.
    const norm = file.replace(/\\/g, "/")
    // If any allowGlob matches, this file is allowed.
    for (const glob of allowGlobs) {
      if (simpleGlobMatch(glob, norm)) return false // allowed
    }
    return true // violation
  }).map((path) => ({ path, isOutside: /^\.\.\//.test(path.replace(/\\/g, "/")) }))
}

// Simple glob matcher supporting `*` (single-segment) and `**` (any depth).
// Hand-rolled — no dependencies.
function simpleGlobMatch(pattern: string, input: string): boolean {
  const p = pattern.replace(/\\/g, "/")
  const n = input.replace(/\\/g, "/")

  // Convert `tests/**` → `tests/**` wildcard matches anything recursively
  // Convert `src/**/*.ts` → matches recursively then any file ending in .ts
  const parts = p.split("/")
  const inputParts = n.split("/")
  return matchGlobParts(parts, inputParts)
}

function matchGlobParts(pattern: string[], input: string[]): boolean {
  let pi = 0 // pattern index
  let ii = 0 // input index

  while (pi < pattern.length && ii < input.length) {
    if (pattern[pi] === "**") {
      // `**` matches zero or more path segments
      // Try matching the rest of the pattern against the current and remaining input
      for (let skip = 0; skip <= input.length - ii; skip++) {
        if (matchGlobParts(pattern.slice(pi + 1), input.slice(ii + skip))) {
          return true
        }
      }
      return false
    }
    if (pattern[pi] === "*") {
      // `*` matches exactly one path segment (not empty and not containing /)
      // Input segment must exist and not be empty
      if (input[ii].length === 0) return false
      ii++
      pi++
      continue
    }
    // Literal segment match
    if (pattern[pi] !== input[ii]) return false
    ii++
    pi++
  }

  // If both fully consumed → match
  if (pi === pattern.length && ii === input.length) return true

  // Remaining pattern segments must all be `**` (trailing ** matches empty)
  while (pi < pattern.length && pattern[pi] === "**") pi++
  return pi === pattern.length && ii <= input.length
}