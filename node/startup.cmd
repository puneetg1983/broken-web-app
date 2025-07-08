@echo off
REM Startup script for Azure App Service Windows
REM This script ensures the Node.js application starts correctly

echo Starting Node.js Diagnostic Scenarios Application...

REM Set environment variables
set NODE_ENV=production
set PORT=%HTTP_PLATFORM_PORT%

REM Create temp directory if it doesn't exist
if not exist "temp" mkdir temp

REM Install dependencies if node_modules doesn't exist
if not exist "node_modules" (
    echo Installing dependencies...
    call npm ci --only=production
)

REM Start the application
echo Starting application on port %PORT%...
node app.js 