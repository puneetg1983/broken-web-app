#!/usr/bin/env pwsh

# Script to create 10 Azure Web Apps similar to existing one
# This script automates:
# 1. Creating Azure Web Apps with the same configuration as existing app
# 2. Setting up GitHub Actions integration
# 3. Creating workflow files for each app

# Configuration
$existingWebAppName = "broken-webapp-aspnet"
$existingWebAppUrl = "broken-webapp-aspnet-fmfzf8fdakanh8gm.canadacentral-01.azurewebsites.net"
$location = "canadacentral" # Extract from existing URL
$resourceGroup = "broken-web-apps" # Hardcoded based on lookup
$appServicePlanName = "broken-web-apps" # From the az webapp list output
$baseName = "broken-webapp-aspnet"
$githubRepo = (git config --get remote.origin.url) -replace '\.git$', ''

# Define descriptive scenario names instead of using random names
$scenarioNames = @(
    "slowresponse",
    "highmemory",
    "highcpu",
    "deadlock",
    "databasetimeout",
    "outofmemory",
    "threadleak",
    "connectionleak",
    "invalidoperation",
    "uncaughtexception"
)
$scenarioCount = $scenarioNames.Count

# Login to Azure
Write-Host "Logging into Azure..." -ForegroundColor Cyan
az login

# Get subscription ID from current context
$subscriptionId = (az account show --query id --output tsv)
Write-Host "Using subscription: $subscriptionId" -ForegroundColor Yellow
Write-Host "Using resource group: $resourceGroup" -ForegroundColor Yellow

# Verify the existing web app exists
$webappExists = az webapp show --name $existingWebAppName --resource-group $resourceGroup
if (-not $webappExists) {
    Write-Host "Error: Could not find existing Web App. Please check the name and try again." -ForegroundColor Red
    exit 1
}

# Get existing App Service Plan details
Write-Host "Getting existing App Service Plan details..." -ForegroundColor Cyan
$appServicePlan = (az appservice plan show --name $appServicePlanName --resource-group $resourceGroup)

if (-not $appServicePlan) {
    Write-Host "Error: Could not find existing App Service Plan. Please check the name and try again." -ForegroundColor Red
    exit 1
}

# Check if the existing workflow file exists
$existingWorkflowPath = ".github/workflows/main_$existingWebAppName.yml"
if (-not (Test-Path $existingWorkflowPath)) {
    Write-Host "Error: Could not find existing workflow file at $existingWorkflowPath" -ForegroundColor Red
    exit 1
}

Write-Host "Found existing workflow file at $existingWorkflowPath" -ForegroundColor Green

# Create web apps with scenario-based names
$newApps = @()
for ($i = 0; $i -lt $scenarioCount; $i++) {
    $scenario = $scenarioNames[$i]
    $newAppName = "$baseName-$scenario"
    $currentAppNumber = $i + 1
    
    Write-Host "Creating Web App ${currentAppNumber} of ${scenarioCount}: ${newAppName}..." -ForegroundColor Green
    
    # Check if app already exists
    $existingApp = az webapp show --name $newAppName --resource-group $resourceGroup --query name --output tsv 2>$null
    if ($existingApp) {
        Write-Host "Web App $newAppName already exists, skipping creation..." -ForegroundColor Yellow
    } else {
        # Create the web app - fixed the runtime parameter
        az webapp create `
            --name $newAppName `
            --resource-group $resourceGroup `
            --plan $appServicePlanName
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Error creating web app $newAppName. Continuing with next app..." -ForegroundColor Red
            continue
        }
        
        # Configure app settings similar to existing app
        az webapp config appsettings set `
            --name $newAppName `
            --resource-group $resourceGroup `
            --settings WEBSITE_RUN_FROM_PACKAGE=1 ASPNETCORE_ENVIRONMENT=Production
        
        # Set up Application Insights
        $appInsightsName = "$newAppName-insights"
        az monitor app-insights component create `
            --app $appInsightsName `
            --location $location `
            --resource-group $resourceGroup `
            --application-type web
        
        $instrumentationKey = (az monitor app-insights component show --app $appInsightsName --resource-group $resourceGroup --query instrumentationKey --output tsv)
        
        az webapp config appsettings set `
            --name $newAppName `
            --resource-group $resourceGroup `
            --settings APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey ApplicationInsightsAgent_EXTENSION_VERSION="~3"
    }
    
    # Get the actual hostname of the newly created app
    $actualHostname = az webapp show --name $newAppName --resource-group $resourceGroup --query defaultHostName --output tsv
    $newWebAppUrl = "https://$actualHostname"
    
    # Create GitHub Actions workflow file for this app
    $workflowFileName = ".github/workflows/main_$newAppName.yml"
    
    # Create the directory if it doesn't exist
    if (-not (Test-Path ".github/workflows")) {
        New-Item -ItemType Directory -Force -Path ".github/workflows"
    }
    
    # Create workflow file by copying the existing one and updating only the app name and URL
    $workflowContent = Get-Content $existingWorkflowPath -Raw
    
    # Update only the app name and URL - don't touch the authentication part
    $workflowContent = $workflowContent -replace "name: Build and deploy ASP app to Azure Web App - $existingWebAppName", "name: Build and deploy ASP app to Azure Web App - $newAppName"
    $workflowContent = $workflowContent -replace "env:`n  WEBAPP_URL: https://$existingWebAppUrl", "env:`n  WEBAPP_URL: $newWebAppUrl"
    $workflowContent = $workflowContent -replace "app-name: '$existingWebAppName'", "app-name: '$newAppName'"
    
    # Write the new workflow file
    Set-Content -Path $workflowFileName -Value $workflowContent
    
    # Store app details for summary
    $newApps += @{
        Name = $newAppName
        Scenario = $scenario
        URL = $newWebAppUrl
        Hostname = $actualHostname
        WorkflowFile = $workflowFileName
    }
    
    Write-Host "Completed setup for $newAppName" -ForegroundColor Green
    Write-Host "Scenario: $scenario" -ForegroundColor Yellow
    Write-Host "URL: $newWebAppUrl" -ForegroundColor Yellow
    Write-Host "Workflow file created at: $workflowFileName" -ForegroundColor Yellow
    Write-Host "----------------------------------------"
}

# Output summary of created apps
Write-Host "`nSUMMARY OF CREATED WEB APPS:" -ForegroundColor Green
Write-Host "----------------------------------------"
foreach ($app in $newApps) {
    Write-Host "App Name: $($app.Name)" -ForegroundColor Cyan
    Write-Host "Scenario: $($app.Scenario)" -ForegroundColor Yellow
    Write-Host "URL: $($app.URL)"
    Write-Host "Workflow File: $($app.WorkflowFile)"
    Write-Host "----------------------------------------"
}

Write-Host "`nAuthentication Information:" -ForegroundColor Magenta
Write-Host "Using the same authentication mechanism as the existing workflow file." -ForegroundColor Yellow
Write-Host "No modifications made to the authentication section of the workflow." -ForegroundColor Yellow

# Function to update WEBAPP_URLs in workflow files with actual hostnames
function Update-WorkflowUrls {
    Write-Host "`nUpdating workflow files with actual hostnames..." -ForegroundColor Cyan
    
    # Get the list of web apps and their hostnames
    $webapps = az webapp list --resource-group $resourceGroup --query "[?contains(name, 'broken-webapp-aspnet')].[name, defaultHostName]" | ConvertFrom-Json
    
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
}

# Run the function to update workflow URLs with actual hostnames
Update-WorkflowUrls

Write-Host "`nNext Steps:" -ForegroundColor Magenta
Write-Host "1. Commit and push the workflow files to your GitHub repository"
Write-Host "2. GitHub Actions will automatically deploy your code to the new web apps"
Write-Host "3. To manually trigger deployments, go to Actions tab in your GitHub repository"

# Save app details to a JSON file for reference
$outputFile = "azure-webapps-deployment-details.json"
$newApps | ConvertTo-Json -Depth 4 | Set-Content -Path $outputFile
Write-Host "`nApp details saved to $outputFile for future reference." -ForegroundColor Green 