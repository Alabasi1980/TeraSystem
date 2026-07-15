import { GatewayProtocol } from "./protocol"

export type GatewayStdio = {
  readonly stdin: NodeJS.ReadableStream
  readonly stdout: NodeJS.WritableStream
  readonly stderr: NodeJS.WritableStream
}

export async function runGatewayStdio(stdio: GatewayStdio) {
  let session = GatewayProtocol.emptySession
  let buffered = ""

  for await (const chunk of stdio.stdin) {
    buffered += Buffer.isBuffer(chunk) ? chunk.toString("utf8") : String(chunk)
    const lines = buffered.split(/\r?\n/)
    buffered = lines.pop() ?? ""
    for (const line of lines) {
      if (line.length === 0) continue
      const result = GatewayProtocol.handleGatewayLine(session, line)
      session = result.session
      if (result.output) stdio.stdout.write(JSON.stringify(result.output) + "\n")
      if (result.diagnostic) stdio.stderr.write(`[${new Date().toISOString()}] [WARN] ${result.diagnostic}\n`)
      if (result.close) return
    }
  }

  if (buffered.length === 0) return
  const result = GatewayProtocol.handleGatewayLine(session, buffered)
  if (result.output) stdio.stdout.write(JSON.stringify(result.output) + "\n")
  if (result.diagnostic) stdio.stderr.write(`[${new Date().toISOString()}] [WARN] ${result.diagnostic}\n`)
}

export * as GatewayStdio from "./stdio"
