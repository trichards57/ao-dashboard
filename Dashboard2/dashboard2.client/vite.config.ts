import child_process from "child_process";
import fs from "fs";
import { URL, fileURLToPath } from "node:url";
import path from "path";
import { env } from "process";

import { TanStackRouterVite } from "@tanstack/router-plugin/vite";
import plugin from "@vitejs/plugin-react";
import { defineConfig } from "vite";

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== ""
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateName = "dashboard2.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    child_process.spawnSync(
      "dotnet",
      [
        "dev-certs",
        "https",
        "--export-path",
        certFilePath,
        "--format",
        "Pem",
        "--no-password",
      ],
      { stdio: "inherit" },
    ).status !== 0
  ) {
    throw new Error("Could not create certificate.");
  }
}

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(";")[0]
    : "https://localhost:5001";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [TanStackRouterVite(), plugin()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    proxy: {
      "^/signin-microsoft": {
        target,
        secure: false,
      },
      "^/Identity/Account": {
        target,
        secure: false,
      },
      "^/css": {
        target,
        secure: false,
      },
      "^/js": {
        target,
        secure: false,
      },
      "^/img": {
        target,
        secure: false,
      },
      "^/.well-known": {
        target,
        secure: false,
      },
      "^/connect": {
        target,
        secure: false,
      },
      "^/swagger": {
        target,
        secure: false,
      },
      "^/api": {
        target,
        secure: false,
      },
    },
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./src/tests/setup.ts"],
    testMatch: ["./src/tests/**/*.test.tsx"],
    globals: true,
  },
});
