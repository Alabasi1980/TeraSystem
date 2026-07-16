import { spawn } from "../runner/spawn.ts"
import path from "node:path"

const wt = path.resolve(process.cwd(), "test-workspace", "wt-poc-POC-001-2139a9")

// Check if worktree still exists
const { existsSync } = await import("node:fs")
console.log("Worktree exists:", existsSync(wt))

// Test git status
const porcelain = await spawn("git", ["status", "--porcelain", "--untracked-files=all"], wt)
console.log("porcelain code:", porcelain.code)
console.log("porcelain stdout:", JSON.stringify(porcelain.stdout))
console.log("porcelain stderr:", JSON.stringify(porcelain.stderr))

// Extract files
const files = porcelain.stdout.split(/\r?\n/).filter(l => l.length >= 3).map(l => l.slice(3).trim()).filter(Boolean)
console.log("Files:", JSON.stringify(files))

// Test glob matching
const allowed = ["tests/**"]
for (const f of files) {
    let isOutside = true
    for (const g of allowed) {
        const norm = f.replace(/\\/g, "/")
        // Inline glob matcher
        const globToRegex = (pattern) => {
            let p = pattern.replace(/\\/g, "/")
            let out = "^"
            let i = 0
            while (i < p.length) {
                const ch = p[i]
                if (ch === "*") {
                    if (p[i + 1] === "*" && p[i + 2] === "/") { out += "(?:.*/)?"; i += 3; continue }
                    if (p[i + 1] === "*") { out += ".*"; i += 2; continue }
                    out += "[^/]*"; i++; continue
                }
                if (".^$+?()|[]{}".includes(ch)) { out += "\\" + ch } else { out += ch }
                i++
            }
            out += "$"
            return new RegExp(out)
        }
        const matches = globToRegex(g).test(norm)
        if (matches) { isOutside = false; break }
    }
    console.log(`  ${f}: isOutside=${isOutside}`)
}
