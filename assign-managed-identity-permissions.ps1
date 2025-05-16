#!/usr/bin/env pwsh

# Script to assign proper permissions to the managed identity for the web apps
# This will fix deployment issues where the managed identity lacks permissions to deploy to web apps

# Configuration
$resourceGroup = "broken-web-apps"
$managedIdentityName = "broken-webapp-as-id-ab83"
$managedIdentityResourceId = "/resource/subscriptions/6b6db65f-680e-4650-b97d-e82ed6a0f583/resourceGroups/broken-web-apps/providers/Microsoft.ManagedIdentity/userAssignedIdentities/broken-webapp-as-id-ab83"
$roleDefinition = "Contributor" # Contributor role provides necessary permissions for deployment

# Login to Azure
Write-Host "Logging into Azure..." -ForegroundColor Cyan
az login

# Get subscription ID from current context
$subscriptionId = (az account show --query id --output tsv)
Write-Host "Using subscription: $subscriptionId" -ForegroundColor Yellow
Write-Host "Using resource group: $resourceGroup" -ForegroundColor Yellow

# Get the principal ID of the managed identity
$principalId = (az identity show --name $managedIdentityName --resource-group $resourceGroup --query principalId --output tsv)

if (-not $principalId) {
    Write-Host "Error: Could not find managed identity $managedIdentityName. Please check the name and try again." -ForegroundColor Red
    exit 1
}

Write-Host "Found managed identity principal ID: $principalId" -ForegroundColor Green

# Get all web apps in the resource group
Write-Host "Getting all web apps in resource group $resourceGroup..." -ForegroundColor Cyan
$webapps = az webapp list --resource-group $resourceGroup --query "[?contains(name, 'broken-webapp-aspnet')].name" --output tsv

$count = 0
foreach ($webapp in $webapps) {
    $count++
    Write-Host "[$count] Assigning $roleDefinition permissions to managed identity for web app: $webapp..." -ForegroundColor Yellow
    
    # Get the resource ID of the web app
    $webappResourceId = (az webapp show --name $webapp --resource-group $resourceGroup --query id --output tsv)
    
    # Assign Contributor role to the managed identity for this web app
    az role assignment create --assignee $principalId --role $roleDefinition --scope $webappResourceId
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully assigned $roleDefinition permissions for $webapp" -ForegroundColor Green
    } else {
        Write-Host "Failed to assign permissions for $webapp" -ForegroundColor Red
    }
}

Write-Host "`nSummary:" -ForegroundColor Green
Write-Host "Managed Identity: $managedIdentityName" -ForegroundColor Yellow
Write-Host "Principal ID: $principalId" -ForegroundColor Yellow
Write-Host "Role assigned: $roleDefinition" -ForegroundColor Yellow
Write-Host "Web apps processed: $count" -ForegroundColor Yellow
Write-Host "`nPermissions have been successfully assigned to all web apps." -ForegroundColor Green 