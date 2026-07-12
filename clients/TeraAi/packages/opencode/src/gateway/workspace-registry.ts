import { isAbsolute, resolve, sep } from "node:path"

export type WorkspaceStatus = "active" | "idle" | "closed"

export interface ApprovalRecord {
  readonly type: "request" | "response"
  readonly task_id?: string
  readonly action_type?: string
  readonly description?: string
  readonly details?: {
    readonly affected_files?: readonly string[]
    readonly affected_commands?: readonly string[]
    readonly risk_level?: string
  }
  readonly approved?: boolean
  readonly reason?: string
  readonly approved_by?: string
  readonly timestamp: string
}

export interface WorkspaceRecord {
  readonly id: string
  readonly projectId: string
  readonly directory: string
  readonly createdAt: string
  lastActiveAt: string
  status: WorkspaceStatus
  tasks: Map<string, string>
  approvals: ApprovalRecord[]
  sessions: string[]
}

export class WorkspaceStore {
  private readonly records = new Map<string, WorkspaceRecord>()

  get(id: string) {
    return this.records.get(id)
  }

  getAll() {
    return Array.from(this.records.values())
  }

  create(id: string, projectId: string, directory?: string) {
    const now = new Date().toISOString()
    const resolvedDir = directory && directory.length > 0 ? directory : resolve(".tera-workspace", id)
    const record: WorkspaceRecord = {
      id,
      projectId,
      directory: resolvedDir,
      createdAt: now,
      lastActiveAt: now,
      status: "active",
      tasks: new Map(),
      approvals: [],
      sessions: [],
    }
    this.records.set(id, record)
    return record
  }

  updateStatus(id: string, status: WorkspaceStatus) {
    const record = this.records.get(id)
    if (!record) return
    record.status = status
    record.lastActiveAt = new Date().toISOString()
    return record
  }

  remove(id: string) {
    return this.records.delete(id)
  }
}

/**
 * Resolves `inputPath` against a workspace's directory and rejects anything
 * that escapes it. Used by future file-scoped gateway tools to guarantee that
 * a workspace can never read or write outside its own root.
 */
export function resolveWorkspacePath(
  workspaceId: string,
  inputPath: string,
):
  | { ok: true; path: string }
  | { ok: false; error: "PATH_TRAVERSAL" | "OUTSIDE_WORKSPACE" | "WORKSPACE_NOT_FOUND" } {
  const record = workspaceStore.get(workspaceId)
  if (!record) return { ok: false, error: "WORKSPACE_NOT_FOUND" }

  const root = resolve(record.directory)
  const resolved = resolve(root, inputPath)

  // Anything that is not the root itself nor nested under it escapes the
  // workspace and must be refused.
  if (resolved !== root && !resolved.startsWith(root + sep)) {
    return { ok: false, error: isAbsolute(inputPath) ? "OUTSIDE_WORKSPACE" : "PATH_TRAVERSAL" }
  }

  return { ok: true, path: resolved }
}

/** Singleton workspace store used by the gateway. */
export const workspaceStore = new WorkspaceStore()

export * as WorkspaceRegistry from "./workspace-registry"
