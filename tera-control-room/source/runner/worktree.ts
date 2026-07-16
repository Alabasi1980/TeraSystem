// Tera Control Room — Phase 0 PoC
// Worktree manager — creates an isolated git worktree per task, returns its
// path / branch / base commit, and cleans up after the runner has persisted
// the EvidenceBundle.
//
// Worktrees live OUTSIDE the host TeraSystem repo. They are direct children of
// the test-workspace sandbox so an agent attempting `../<escape>` lands inside
// test-workspace root (Test 4's `../forbidden.txt` → `<test-workspace>\forbidden.txt`).
// We never touch the host TeraSystem repository or any client code.

import path from "node:path"
import { spawn } from "./spawn.ts"

export interface WorktreeInfo {
  worktreePath: string
  branchName: string
  baseCommit: string
}

export class WorktreeError extends Error {
  constructor(message: string) {
    super(message)
    this.name = "WorktreeError"
  }
}

export async function createWorktree(
  repoPath: string,
  taskId: string,
): Promise<WorktreeInfo> {
  // 1. Verify repo is a git worktree.
  const insideCheck = await runGit(["rev-parse", "--is-inside-work-tree"], repoPath)
  if (insideCheck.code !== 0 || insideCheck.stdout.trim() !== "true") {
    throw new WorktreeError(`WORKTREE_CREATION_FAILED: ${repoPath} is not inside a git work tree.`)
  }

  // 2. Capture base commit (before the agent runs).
  const baseResult = await runGit(["rev-parse", "HEAD"], repoPath)
  if (baseResult.code !== 0) {
    throw new WorktreeError(`WORKTREE_CREATION_FAILED: git rev-parse HEAD failed: ${baseResult.stderr}`)
  }
  const baseCommit = baseResult.stdout.trim()

  // 3. Log working tree status (informational — must not fail the run).
  const statusResult = await runGit(["status", "--porcelain"], repoPath)
  if (statusResult.stdout.trim().length > 0) {
    console.error(`[worktree] WARNING: working tree at ${repoPath} is not clean:\n${statusResult.stdout}`)
  }

  // 4. Branch name with random hex suffix to avoid collisions across re-runs.
  const rand = Math.random().toString(16).slice(2, 8)
  const branchName = `task/${taskId}-${rand}`
  // Placement: worktree is a DIRECT CHILD of repoPath (`<repoPath>\wt-poc-<taskId>-<rand>\`).
  // This is critical for the spec's Test 4 — `../forbidden.txt` emitted by the
  // agent from inside the worktree resolves to `<repoPath>\forbidden.txt` =
  // `<test-workspace>\forbidden.txt`, exactly the path the spec verifies.
  // Git refuses nested worktree placement by default; `--force` overrides it.
  const worktreeName = `wt-poc-${taskId}-${rand}`
  const worktreePath = path.join(repoPath, worktreeName)

  // 5. Create the worktree on a fresh branch from base commit.
  const addResult = await runGit(["worktree", "add", "--force", "-b", branchName, worktreePath, "HEAD"], repoPath)
  if (addResult.code !== 0) {
    throw new WorktreeError(`WORKTREE_CREATION_FAILED: git worktree add failed: ${addResult.stderr}`)
  }

  return { worktreePath, branchName, baseCommit }
}

export async function cleanupWorktree(
  repoPath: string,
  worktreePath: string,
  branchName: string,
): Promise<string[]> {
  const notes: string[] = []
  if (worktreePath) {
    const r = await runGit(["worktree", "remove", "--force", worktreePath], repoPath)
    if (r.code !== 0) notes.push(`git worktree remove exit ${r.code}: ${r.stderr.trim()}`)
    else notes.push(`worktree removed: ${worktreePath}`)
  }
  if (branchName) {
    const r = await runGit(["branch", "-D", branchName], repoPath)
    if (r.code !== 0) notes.push(`git branch -D exit ${r.code}: ${r.stderr.trim()}`)
    else notes.push(`branch deleted: ${branchName}`)
  }
  return notes
}

export async function getGitStatus(worktreePath: string): Promise<{ porcelain: string; diff: string; headCommit: string }> {
  const st = await runGit(["status", "--porcelain", "--untracked-files=all"], worktreePath)
  const df = await runGit(["diff"], worktreePath)
  const hd = await runGit(["rev-parse", "HEAD"], worktreePath)
  return {
    porcelain: st.stdout,
    diff: df.stdout,
    headCommit: hd.stdout.trim(),
  }
}

async function runGit(args: string[], cwd: string): Promise<{ code: number; stdout: string; stderr: string }> {
  return await spawn("git", args, cwd)
}