import { cmd } from "./cmd"
import { GatewayStdio } from "@/gateway/stdio"

export const GatewayCommand = cmd({
  command: "gateway",
  describe: "start Tera Engine Gateway over stdio JSON Lines",
  async handler() {
    await GatewayStdio.runGatewayStdio({
      stdin: process.stdin,
      stdout: process.stdout,
      stderr: process.stderr,
    })
  },
})
