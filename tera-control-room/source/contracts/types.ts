// Tera Control Room — Phase 0 PoC
// Contract type definitions — pure types, no logic.
//
// These interfaces mirror exactly the schemas in:
//   - specs/TASK_CONTRACT_V1.md
//   - specs/HANDBACK_SCHEMA_V1.md
//   - specs/POC_ACCEPTANCE_CRITERIA.md (EvidenceBundle, §4)

export type SchemaVersion = "1.0"
export type HandbackSchemaRef = "handback-v1"
export type HandbackStatus = "COMPLETED" | "FAILED" | "PARTIAL_SUCCESS"
export type RecommendedNextAction = "REVIEW" | "RETRY" | "ESCALATE"
export type AgentMode = "primary" | "subagent"

export interface TaskContract {
  schema_version: SchemaVersion
  task_id: string
  agent_id: string
  objective: string
  working_directory: string
  allowed_read_paths: string[]
  allowed_write_paths: string[]
  allowed_commands: string[]
  timeout_seconds: number
  expected_handback_schema: HandbackSchemaRef
}

export interface CommandExecuted {
  command: string
  exit_code: number
}

export interface Handback {
  schema_version: SchemaVersion
  task_id: string
  agent_id: string
  status: HandbackStatus
  summary: string
  files_changed: string[]
  commands_executed: CommandExecuted[]
  known_issues: string[]
  recommended_next_action: RecommendedNextAction
}

export type FinalStatus =
  | "COMPLETED"
  | "INVALID_TASK_CONTRACT"
  | "ADAPTER_START_FAILED"
  | "AGENT_PROFILE_NOT_FOUND"
  | "WORKTREE_CREATION_FAILED"
  | "EXECUTION_FAILED"
  | "TASK_TIMEOUT"
  | "INVALID_HANDBACK"
  | "PERMISSION_DENIED"
  | "COMMAND_DENIED"
  | "EVIDENCE_COLLECTION_FAILED"
  | "PARTIAL_SUCCESS"

export interface HandbackValidation {
  valid: boolean
  errors: string[]
}

export interface AdapterResult {
  status: FinalStatus
  exitCode: number | null
  stdout: string
  stderr: string
  handback: Handback | null
  handbackValidation: HandbackValidation | null
  timedOut: boolean
  durationMs: number
  rawJsonl: string[]
}

export type SecurityClassification =
  | "SECURE_ISOLATION_CONFIRMED"
  | "PARTIAL_ISOLATION"
  | "ISOLATION_NOT_AVAILABLE"

export interface SecurityResult {
  classification: SecurityClassification
  forbiddenFileCreated: boolean
  dockerUsed: boolean
  networkDisabled: boolean
  notes: string[]
}

export interface EvidenceBundle {
  task_contract: TaskContract | Record<string, unknown>
  started_at: string
  finished_at: string
  duration_ms: number
  base_commit: string
  head_commit: string
  worktree_path: string
  agent_profile: string
  adapter_command: string
  process_exit_code: number | null
  stdout_log: string
  stderr_log: string
  files_changed: string[]
  git_diff_path: string
  handback: Handback | null
  handback_validation: HandbackValidation | null
  security_result: SecurityResult
  final_status: FinalStatus
}