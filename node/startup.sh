#!/bin/bash

# Startup script for Azure App Service Windows
# This script ensures the Node.js application starts correctly

echo "Starting Node.js Diagnostic Scenarios Application..."

# Set environment variables
export NODE_ENV=production
export PORT=8080

# Create temp directory if it doesn't exist
mkdir -p temp

# Install dependencies if node_modules doesn't exist
if [ ! -d "node_modules" ]; then
    echo "Installing dependencies..."
    npm ci --only=production
fi

# Start the application
echo "Starting application on port $PORT..."
node app.js 