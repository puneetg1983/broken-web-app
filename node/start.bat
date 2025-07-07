@echo off
echo Starting Diagnostic Scenarios - Node.js Application...
echo.

REM Check if Node.js is installed
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: Node.js is not installed or not in PATH
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

REM Check if dependencies are installed
if not exist "node_modules" (
    echo Installing dependencies...
    npm install
    if %errorlevel% neq 0 (
        echo Error: Failed to install dependencies
        pause
        exit /b 1
    )
)

REM Create temp directory if it doesn't exist
if not exist "temp" (
    mkdir temp
)

echo Starting application on http://localhost:3000
echo Press Ctrl+C to stop the application
echo.

npm start

pause 