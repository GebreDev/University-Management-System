#!/usr/bin/env bash
# ============================================================
#  kill-server.sh  (for git-bash / WSL bash on Windows)
#  Kills any leftover UniversityManagementSystem.Web.exe
#  processes holding the app port (5134), then starts the app.
#  Run this whenever you see:
#  "Failed to bind to address ... address already in use."
# ============================================================
set -e

# Make this script work no matter where it is run from
cd "$(dirname "$0")"

echo "[1/2] Cleaning up leftover app instances..."
if taskkill //F //IM UniversityManagementSystem.Web.exe >/dev/null 2>&1; then
    echo "      Killed leftover processes."
else
    echo "      No leftover processes found."
fi

echo "[2/2] Starting the app on http://localhost:5134 ..."
dotnet run --project UniversityManagementSystem.Web --launch-profile http
