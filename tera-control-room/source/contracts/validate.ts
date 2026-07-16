// Tera Control Room — Phase 0 PoC
// Manual validators reproducing the rules in TASK_CONTRACT_V1.md and
// HANDBACK_SCHEMA_V1.md exactly. These are intentionally dependency-free
// (JSON-Schema files live in source/schemas/ as documentation; ajv was
// intentionally dropped to keep the Bun transpileOnly path trivial and
// avoid any install failure that would invalidate the whole PoC).

import type {
  CommandExecuted,
  Handback,
  HandbackValidation,
  TaskContract,
} from "./types.ts"

// ---------------------------------------------------------------------------
// Task Contract validation (TASK_CONTRACT_V1 §3)
// ---------------------------------------------------------------------------

export type TaskContractValidation =
  | { valid: true; contract: TaskContract }
  | { valid: false; errors: string[] }

const TASK_ID_PATTERN = /^[A-Za-z0-9._-]+$/

export function validateTaskContract(raw: unknown): TaskContractValidation {
  const errors: string[] = []

  if (raw === null || typeof raw !== "object" || Array.isArray(raw)) {
    return { valid: false, errors: ["Task contract must be a JSON object."] }
  }

  const obj = raw as Record<string, unknown>

  // Field-by-field checks (all 10 mandatory fields per spec §2). Each missing
  // or wrong-typed field pushes its own error so the caller can see exactly
  // what failed in `errors[]`.
  if (!("schema_version" in obj)) errors.push("Missing required field: schema_version")
  else if (obj.schema_version !== "1.0") errors.push(`schema_version must be "1.0" (got ${JSON.stringify(obj.schema_version)})`)

  if (!("task_id" in obj)) errors.push("Missing required field: task_id")
  else if (typeof obj.task_id !== "string" || obj.task_id.length === 0) errors.push("task_id must be a non-empty string")
  else if (!TASK_ID_PATTERN.test(obj.task_id)) errors.push("task_id contains characters unsafe for folder names (allowed: A-Z a-z 0-9 . _ -)")

  if (!("agent_id" in obj)) errors.push("Missing required field: agent_id")
  else if (typeof obj.agent_id !== "string" || obj.agent_id.length === 0) errors.push("agent_id must be a non-empty string")

  if (!("objective" in obj)) errors.push("Missing required field: objective")
  else if (typeof obj.objective !== "string" || obj.objective.length < 10) errors.push("objective must be a string >= 10 chars")

  if (!("working_directory" in obj)) errors.push("Missing required field: working_directory")
  else if (typeof obj.working_directory !== "string" || obj.working_directory.length === 0) errors.push("working_directory must be a non-empty string")

  const readErr = checkPathArray(obj.allowed_read_paths, "allowed_read_paths", /* requireNonEmpty= */ true, errors)
  const writeErr = checkPathArray(obj.allowed_write_paths, "allowed_write_paths", /* requireNonEmpty= */ true, errors)
  void readErr
  void writeErr

  if (!("allowed_commands" in obj)) errors.push("Missing required field: allowed_commands")
  else if (!Array.isArray(obj.allowed_commands)) errors.push("allowed_commands must be an array")
  else {
    for (let i = 0; i < obj.allowed_commands.length; i++) {
      if (typeof obj.allowed_commands[i] !== "string") errors.push(`allowed_commands[${i}] must be a string`)
    }
  }

  if (!("timeout_seconds" in obj)) errors.push("Missing required field: timeout_seconds")
  else if (typeof obj.timeout_seconds !== "number" || !Number.isFinite(obj.timeout_seconds)) errors.push("timeout_seconds must be a number")
  else if (!(obj.timeout_seconds > 0 && obj.timeout_seconds <= 3600)) errors.push("timeout_seconds must be > 0 and <= 3600")

  if (!("expected_handback_schema" in obj)) errors.push("Missing required field: expected_handback_schema")
  else if (obj.expected_handback_schema !== "handback-v1") errors.push(`expected_handback_schema must be "handback-v1"`)

  if (errors.length > 0) return { valid: false, errors }

  // Cast is safe — all required fields are present and well-typed here.
  const contract: TaskContract = {
    schema_version: "1.0",
    task_id: obj.task_id as string,
    agent_id: obj.agent_id as string,
    objective: obj.objective as string,
    working_directory: obj.working_directory as string,
    allowed_read_paths: obj.allowed_read_paths as string[],
    allowed_write_paths: obj.allowed_write_paths as string[],
    allowed_commands: obj.allowed_commands as string[],
    timeout_seconds: obj.timeout_seconds as number,
    expected_handback_schema: "handback-v1",
  }
  return { valid: true, contract }
}

function checkPathArray(value: unknown, field: string, requireNonEmpty: boolean, errors: string[]): boolean {
  if (value === undefined) {
    errors.push(`Missing required field: ${field}`)
    return false
  }
  if (!Array.isArray(value)) {
    errors.push(`${field} must be an array of strings`)
    return false
  }
  if (requireNonEmpty && value.length === 0) {
    errors.push(`${field} must not be empty (Default Deny)`)
    return false
  }
  for (let i = 0; i < value.length; i++) {
    if (typeof value[i] !== "string" || value[i].length === 0) {
      errors.push(`${field}[${i}] must be a non-empty string`)
    }
  }
  return true
}

// ---------------------------------------------------------------------------
// Handback validation (HANDBACK_SCHEMA_V1 §3 + §4)
// ---------------------------------------------------------------------------

export type HandbackValidationResult =
  | { valid: true; handback: Handback }
  | { valid: false; errors: string[] }

// Public entry kept for compatibility with the spec contract — wrappers unify
// shape naming.
export function validateHandback(
  rawText: string,
  expectedTaskId: string,
  expectedAgentId: string,
): HandbackValidationResult {
  return validateHandbackFromJsonl(rawText, expectedTaskId, expectedAgentId)
}

// Accept a raw JSON string (already-extracted Handback) and run only the
// schema + identity checks. Used by Test 8 to inject synthetic handbacks
// without staging a fake JSONL stream.
export function validateHandbackJsonString(
  rawJson: string,
  expectedTaskId: string,
  expectedAgentId: string,
): HandbackValidationResult {
  let parsed: unknown
  try {
    parsed = JSON.parse(rawJson)
  } catch (err) {
    return { valid: false, errors: [`Handback is not valid JSON: ${(err as Error).message}`] }
  }
  return validateHandbackObject(parsed, expectedTaskId, expectedAgentId)
}

// Accept a full stdout blob of JSONL events from `opencode run --format json`
// and run the full extract+validate pipeline.
export function validateHandbackFromJsonl(
  stdout: string,
  expectedTaskId: string,
  expectedAgentId: string,
): HandbackValidationResult {
  const lastText = extractLastVisibleTextEvent(stdout)
  if (lastText === null) {
    return { valid: false, errors: ["No `text` event with part.time.end was emitted in stdout JSONL."] }
  }
  const jsonBlock = extractFirstJsonFence(lastText)
  if (jsonBlock === null) {
    return {
      valid: false,
      errors: [
        "No fenced ```json block found in the agent's last text message.",
        `Last text event content (truncated): ${lastText.slice(0, 200)}`,
      ],
    }
  }
  return validateHandbackJsonString(jsonBlock, expectedTaskId, expectedAgentId)
}

// Walk every newline-separated chunk. Treat only lines that parse as JSON
// objects with type==="text" and part.time.end set as visible agent text
// events — string output emitted by UI.println warnings (e.g. fallback
// notices) silently breaks the JSONL stream, so non-JSON lines are skipped.
function extractLastVisibleTextEvent(stdout: string): string | null {
  const lines = stdout.split(/\r?\n/)
  let last: string | null = null
  for (const line of lines) {
    if (!line || !line.startsWith("{")) continue
    let parsed: unknown
    try {
      parsed = JSON.parse(line)
    } catch {
      continue
    }
    if (parsed === null || typeof parsed !== "object") continue
    const evt = parsed as { type?: unknown; part?: unknown }
    if (evt.type !== "text") continue
    if (!evt.part || typeof evt.part !== "object") continue
    const part = evt.part as { type?: unknown; text?: unknown; time?: unknown }
    if (part.type !== "text") continue
    if (!part.time || typeof part.time !== "object") continue
    const time = part.time as { end?: unknown }
    if (time.end === undefined || time.end === null) continue
    if (typeof part.text !== "string") continue
    last = part.text
  }
  return last
}

// Extract the FIRST ```json ... ``` fenced block via the regex from the spec.
function extractFirstJsonFence(text: string): string | null {
  const re = /```json\s*\n([\s\S]*?)\n```/
  const m = re.exec(text)
  return m ? m[1] : null
}

function validateHandbackObject(
  parsed: unknown,
  expectedTaskId: string,
  expectedAgentId: string,
): HandbackValidationResult {
  if (parsed === null || typeof parsed !== "object" || Array.isArray(parsed)) {
    return { valid: false, errors: ["Handback must be a JSON object."] }
  }
  const obj = parsed as Record<string, unknown>
  const errors: string[] = []

  // 1. schema_version === "1.0"
  if (obj.schema_version !== "1.0") errors.push(`schema_version must be "1.0" (got ${JSON.stringify(obj.schema_version)})`)

  // 2-3. identity match
  if (typeof obj.task_id !== "string" || obj.task_id.length === 0) errors.push("task_id must be a non-empty string")
  else if (obj.task_id !== expectedTaskId) errors.push(`task_id identity mismatch: expected "${expectedTaskId}", got "${obj.task_id}"`)

  if (typeof obj.agent_id !== "string" || obj.agent_id.length === 0) errors.push("agent_id must be a non-empty string")
  else if (obj.agent_id !== expectedAgentId) errors.push(`agent_id identity mismatch: expected "${expectedAgentId}", got "${obj.agent_id}"`)

  // 4. status enum
  if (obj.status !== "COMPLETED" && obj.status !== "FAILED" && obj.status !== "PARTIAL_SUCCESS") {
    errors.push(`status must be one of COMPLETED, FAILED, PARTIAL_SUCCESS (got ${JSON.stringify(obj.status)})`)
  }

  // 5. summary >= 5 chars
  if (typeof obj.summary !== "string" || obj.summary.length < 5) errors.push("summary must be a string >= 5 chars")

  // 6. files_changed: array<string>
  if (!Array.isArray(obj.files_changed)) errors.push("files_changed must be an array of strings")
  else {
    for (let i = 0; i < obj.files_changed.length; i++) {
      if (typeof obj.files_changed[i] !== "string") errors.push(`files_changed[${i}] must be a string`)
    }
  }

  // 7. commands_executed: array<{command:string, exit_code:number}>
  if (!Array.isArray(obj.commands_executed)) errors.push("commands_executed must be an array")
  else {
    for (let i = 0; i < obj.commands_executed.length; i++) {
      const item = obj.commands_executed[i]
      if (item === null || typeof item !== "object" || Array.isArray(item)) {
        errors.push(`commands_executed[${i}] must be { command, exit_code }`)
        continue
      }
      const it = item as { command?: unknown; exit_code?: unknown }
      if (typeof it.command !== "string") errors.push(`commands_executed[${i}].command must be a string`)
      if (typeof it.exit_code !== "number" || !Number.isFinite(it.exit_code)) errors.push(`commands_executed[${i}].exit_code must be a number`)
    }
  }

  // 8. known_issues: array<string>
  if (!Array.isArray(obj.known_issues)) errors.push("known_issues must be an array of strings")
  else {
    for (let i = 0; i < obj.known_issues.length; i++) {
      if (typeof obj.known_issues[i] !== "string") errors.push(`known_issues[${i}] must be a string`)
    }
  }

  // 9. recommended_next_action enum
  if (obj.recommended_next_action !== "REVIEW" && obj.recommended_next_action !== "RETRY" && obj.recommended_next_action !== "ESCALATE") {
    errors.push(`recommended_next_action must be one of REVIEW, RETRY, ESCALATE (got ${JSON.stringify(obj.recommended_next_action)})`)
  }

  // 11. any mandatory field missing
  const requiredFields = [
    "schema_version",
    "task_id",
    "agent_id",
    "status",
    "summary",
    "files_changed",
    "commands_executed",
    "known_issues",
    "recommended_next_action",
  ]
  for (const f of requiredFields) {
    if (!(f in obj)) errors.push(`Missing required field: ${f}`)
  }

  if (errors.length > 0) return { valid: false, errors }

  const cmds: CommandExecuted[] = (obj.commands_executed as Array<Record<string, unknown>>).map((it) => ({
    command: it.command as string,
    exit_code: it.exit_code as number,
  }))

  const handback: Handback = {
    schema_version: "1.0",
    task_id: obj.task_id as string,
    agent_id: obj.agent_id as string,
    status: obj.status as Handback["status"],
    summary: obj.summary as string,
    files_changed: obj.files_changed as string[],
    commands_executed: cmds,
    known_issues: obj.known_issues as string[],
    recommended_next_action: obj.recommended_next_action as Handback["recommended_next_action"],
  }
  return { valid: true, handback }
}

// ---------------------------------------------------------------------------
// Pre-flight agent lookup (per TASK_CONTRACT_V1 §3.4 + Test 7 path)
// ---------------------------------------------------------------------------

export interface AgentRegistry {
  available: boolean
  primaryAgents: Set<string>
  subagents: Set<string>
  notes: string[]
}

let cachedRegistry: AgentRegistry | null = null

export async function loadAgentRegistry(): Promise<AgentRegistry> {
  if (cachedRegistry) return cachedRegistry

  const proc = Bun.spawn({
    cmd: ["opencode", "agent", "list"],
    cwd: process.cwd(),
    stdout: "pipe",
    stderr: "pipe",
    env: sanitizeEnvForChild(process.env),
  })
  const stdout = await new Response(proc.stdout).text()
  const stderr = await new Response(proc.stderr).text()
  const exitCode = await proc.exited
  if (exitCode !== 0) {
    cachedRegistry = {
      available: false,
      primaryAgents: new Set(),
      subagents: new Set(),
      notes: [`opencode agent list failed (exit ${exitCode}): ${stderr.slice(0, 200)}`],
    }
    return cachedRegistry
  }

  // Lines look like: "build (primary)" / "explore (subagent)".
  const primary = new Set<string>()
  const sub = new Set<string>()
  const re = /^(\S+)\s+\((primary|subagent)\)$/
  for (const line of stdout.split(/\r?\n/)) {
    const m = re.exec(line.trim())
    if (m && m[1] && m[2]) {
      if (m[2] === "primary") primary.add(m[1])
      else sub.add(m[1])
    }
  }
  cachedRegistry = {
    available: true,
    primaryAgents: primary,
    subagents: sub,
    notes: [],
  }
  return cachedRegistry
}

// ---------------------------------------------------------------------------
// Env sanitization shared by runner + adapter (RUNNER_SECURITY_POLICY §5)
// ---------------------------------------------------------------------------

export function sanitizeEnvForChild(env: Record<string, string | undefined>): Record<string, string | undefined> {
  // Bun-on-Windows quirk: a child-process env containing BOTH `PATH` and
  // `Path` collides case-insensitively inside libuv; the surviving key
  // happens to be the mixed-case `Path`, which libuv's path-resolution step
  // does not consult — so spawn() fails with `ENOENT uv_spawn '<binary>'`.
  // Fix: emit a single canonical `PATH` (uppercase) key, sourced from any
  // PATH-style env value already present. Same treatment for other Windows-
  // case-duplicated vars (USERPROFILE/UserProfile, SystemRoot/systemroot, ...).
  const CASE_MERGE: Record<string, string[]> = {
    PATH: ["PATH", "Path", "path"],
    USERPROFILE: ["USERPROFILE", "UserProfile", "USERPROFILE"],
    SystemRoot: ["SystemRoot", "SYSTEMROOT", "systemroot"],
    TEMP: ["TEMP", "Temp", "temp"],
    TMP: ["TMP", "Tmp", "tmp"],
    PATHEXT: ["PATHEXT", "PathExt", "pathext"],
  }

  const KEEP = new Set([
    "HOME",
    "LANG", "LC_ALL",
    "ComSpec",
    "APPDATA", "LOCALAPPDATA",    // Needed by opencode.exe Node.js binary to locate global config/auth/data
    "ProgramFiles", "ProgramFiles(x86)",
    "PROCESSOR_ARCHITECTURE",
  ])

  const PREFIX_KEEP = ["BUN_", "GIT_"]

  const SECRET_PATTERNS = [
    /API_KEY/i, /_TOKEN/i, /SECRET/i, /PRIVATE_KEY/i,
    /^OPENAI_/i, /^ANTHROPIC_/i, /^AWS_ACCESS/i, /^AWS_SECRET/i, /^GITHUB_TOKEN/i,
    /^AZURE_/i, /^GH_TOKEN/i, /^BITBUCKET/i, /^GITLAB/i, /^SSH_AUTH_SOCK/i, /^KUBE/i,
    /^CREDENTIAL/i, /_PASSWORD/i, /PASSPHRASE/i, /^HUGGING_FACE/i, /^HF_/i,
    /^OPENCODE_SERVER_PASSWORD$/i, /^OPENCODE_SERVER_USERNAME$/i,
  ]

  const out: Record<string, string | undefined> = {}

  // 1. Controlled name-normalized vars (PATH / PATHEXT / ...).
  for (const [canonical, aliases] of Object.entries(CASE_MERGE)) {
    let val: string | undefined
    for (const alias of aliases) {
      if (env[alias] !== undefined && env[alias] !== "") {
        val = env[alias]
        break
      }
    }
    if (val !== undefined) out[canonical] = val
  }

  // 2. Pass through whitelisted single-spelling vars.
  for (const [k, v] of Object.entries(env)) {
    if (v === undefined) continue
    if (Object.prototype.hasOwnProperty.call(out, k)) continue // already handled above
    if (CASE_MERGE[getCanonical(k, CASE_MERGE)]) continue // skip alt spellings of merged vars
    if (KEEP.has(k)) {
      out[k] = v
      continue
    }
    // OPENCODE_*: dropped on purpose per RUNNER_SECURITY_POLICY §5 (only
    // strictly-necessary ones, and Phase 0 needs none — the auth.json file
    // already lives at ~/.local/share/opencode and is read by the child via
    // USERPROFILE). OPENCODE_MODEL is consulted by the Adapter in the parent
    // process, not by the child.
    if (k.startsWith("OPENCODE_")) continue
    if (PREFIX_KEEP.some((p) => k.startsWith(p))) {
      out[k] = v
      continue
    }
    // Anything secret-bearing is dropped.
    if (SECRET_PATTERNS.some((p) => p.test(k))) continue
    // Drop everything else.
  }
  return out
}

function getCanonical(key: string, table: Record<string, string[]>): string | undefined {
  for (const [canonical, aliases] of Object.entries(table)) {
    if (aliases.includes(key)) return canonical
  }
  return undefined
}