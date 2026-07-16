// Smoke test — run the adapter with a simple file creation task
import path from "node:path"
import { runAdapter, buildPromptFromContract } from "../adapter/adapter.ts"
import { spawnSync } from "node:child_process"

const ROOT = process.cwd()
const REPO = path.join(ROOT, "test-workspace")
const WT = path.join(REPO, "wt-smoke-test")

// Ensure test workspace has a git commit
spawnSync("git", ["init", "-b", "main"], { cwd: REPO })
spawnSync("git", ["add", "-A"], { cwd: REPO })
spawnSync("git", ["commit", "-m", "init"], {
  cwd: REPO,
  env: { ...process.env, GIT_AUTHOR_NAME: "smoke", GIT_AUTHOR_EMAIL: "smoke@test", GIT_COMMITTER_NAME: "smoke", GIT_COMMITTER_EMAIL: "smoke@test" },
})

// Create worktree
spawnSync("git", ["worktree", "add", "--force", "-b", "task/smoke-test", WT, "HEAD"], { cwd: REPO })

const contract = {
  schema_version: "1.0",
  task_id: "SMOKE-001",
  agent_id: "build",
  objective: 'Create the file "tests/adapter-proof.txt" containing exactly the line: Tera Control Room PoC works.',
  working_directory: WT,
  allowed_read_paths: ["tests/**", "README.md"],
  allowed_write_paths: ["tests/**"],
  allowed_commands: ["git status", "git diff"],
  timeout_seconds: 60,
  expected_handback_schema: "handback-v1",
}

console.log("=== Smoke Test ===")
const start = Date.now()
const result = await runAdapter({ contract, worktreePath: WT, timeoutSeconds: 60 })
const elapsed = Date.now() - start
console.log("Duration:", elapsed, "ms")
console.log("Status:", result.status)
console.log("Exit code:", result.exitCode)
console.log("Timed out:", result.timedOut)

if (result.handback) {
  console.log("Handback:", JSON.stringify(result.handback, null, 2))
} else {
  console.log("Handback: null")
}

if (result.handbackValidation) {
  console.log("Handback validation:", JSON.stringify(result.handbackValidation, null, 2))
}

// Show stdout context
const lines = result.stdout.split("\n").filter(Boolean)
console.log("JSONL lines:", lines.length)
for (let i = Math.max(0, lines.length - 5); i < lines.length; i++) {
  try {
    const evt = JSON.parse(lines[i])
    if (evt.type === "text" && evt.part?.text) {
      console.log("TEXT:", evt.part.text.slice(0, 300))
    }
  } catch {}
}

// Check if the target file was created
import { existsSync } from "node:fs"
const proofPath = path.join(WT, "tests", "adapter-proof.txt")
console.log("Target file exists:", existsSync(proofPath))
if (existsSync(proofPath)) {
  console.log("Target file content:", (await import("node:fs/promises")).readFile(proofPath, "utf8"))
}

// Cleanup
try { spawnSync("git", ["worktree", "remove", "--force", WT], { cwd: REPO }) } catch {}
try { spawnSync("git", ["branch", "-D", "task/smoke-test"], { cwd: REPO }) } catch {}
console.log("=== Smoke Test Complete ===")
