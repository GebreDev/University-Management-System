@echo off
REM ============================================================
REM  kill-server.bat
REM  Kills any leftover UniversityManagementSystem.Web.exe
REM  processes that are still holding the app port (5134),
REM  then starts the app.
REM  Run this whenever you see:
REM  "Failed to bind to address ... address already in use."
REM ============================================================
title University Management System - Dev Launcher

REM Make this script work no matter where it is run from
cd /d "%~dp0"

echo [1/2] Cleaning up leftover app instances...
taskkill /F /IM UniversityManagementSystem.Web.exe >nul 2>&1
if %errorlevel%==0 (
    echo       Killed leftover processes.
) else (
    echo       No leftover processes found.
)

echo [2/2] Starting the app on http://localhost:5134 ...
dotnet run --project UniversityManagementSystem.Web --launch-profile http
