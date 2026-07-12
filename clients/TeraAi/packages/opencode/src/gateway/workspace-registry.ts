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

  create(id: string, projectId: string, directory: string) {
    const now = new Date().toISOString()
    const record: WorkspaceRecord = {
      id,
      projectId,
      directory,
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

/** Singleton workspace store used by the gateway. */
export const workspaceStore = new WorkspaceStore()

export * as WorkspaceRegistry from "./workspace-registry"
