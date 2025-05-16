#!/usr/bin/env pwsh

# Get the list of web apps and their hostnames
Write-Host "Getting web app hostnames..." -ForegroundColor Cyan
$webapps = az webapp list --resource-group broken-web-apps --query "[?contains(name, 'broken-webapp-aspnet')].[name, defaultHostName]" | ConvertFrom-Json

# Create a hashtable to store the app names and their hostnames
$hostnameMap = @{}
foreach ($webapp in $webapps) {
    $appName = $webapp[0]
    $hostname = $webapp[1]
    $hostnameMap[$appName] = $hostname
    Write-Host "App: $appName, Hostname: $hostname" -ForegroundColor Yellow
}

# Update the YAML files
$workflowDir = ".github/workflows"
$workflowFiles = Get-ChildItem -Path $workflowDir -Filter "main_broken-webapp-aspnet-*.yml"

foreach ($file in $workflowFiles) {
    $appName = $file.Name -replace "main_", "" -replace "\.yml", ""
    
    if ($hostnameMap.ContainsKey($appName)) {
        $hostname = $hostnameMap[$appName]
        Write-Host "Updating $($file.FullName) with hostname: $hostname" -ForegroundColor Green
        
        # Read the file content line by line
        $content = Get-Content -Path $file.FullName
        $newContent = @()
        
        for ($i = 0; $i -lt $content.Length; $i++) {
            if ($content[$i] -match "WEBAPP_URL: https://.+") {
                $newContent += "  WEBAPP_URL: https://$hostname"
            } else {
                $newContent += $content[$i]
            }
        }
        
        # Write the updated content back to the file
        Set-Content -Path $file.FullName -Value $newContent
        
        Write-Host "Updated $($file.Name) successfully" -ForegroundColor Green
    }
    else {
        Write-Host "Warning: Could not find hostname for $appName" -ForegroundColor Red
    }
}

Write-Host "All workflow files updated with correct hostnames." -ForegroundColor Green 