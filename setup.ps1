# PowerShell script to automate Azure setup for diagnostic scenarios

function Exit-OnError {
    param([string]$Message)
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

function Invoke-WithRetry {
    param(
        [scriptblock]$ScriptBlock,
        [int]$MaxAttempts = 3,
        [int]$DelaySeconds = 10
    )
    
    $attempt = 1
    while ($attempt -le $MaxAttempts) {
        try {
            $result = & $ScriptBlock
            return $result
        }
        catch {
            if ($attempt -eq $MaxAttempts) {
                throw $_
            }
            Write-Host "Attempt $attempt failed. Retrying in $DelaySeconds seconds..." -ForegroundColor Yellow
            Start-Sleep -Seconds $DelaySeconds
            $attempt++
        }
    }
}

# Display script purpose and get user consent
Write-Host "This script will perform the following actions:" -ForegroundColor Cyan
Write-Host "1. Check and install WinGet if not present" -ForegroundColor Cyan
Write-Host "2. Check and install Azure CLI (az) if not present" -ForegroundColor Cyan
Write-Host "3. Check and install GitHub CLI (gh) if not present" -ForegroundColor Cyan
Write-Host "4. Set up Azure resources:" -ForegroundColor Cyan
Write-Host "   - Create/verify Azure Resource Group" -ForegroundColor Cyan
Write-Host "   - Create/verify Managed Identity" -ForegroundColor Cyan
Write-Host "   - Create federated credentials" -ForegroundColor Cyan
Write-Host "   - Assign necessary permissions" -ForegroundColor Cyan
Write-Host "5. Configure GitHub repository:" -ForegroundColor Cyan
Write-Host "   - Set up required secrets in your GitHub repository" -ForegroundColor Cyan
Write-Host ""
$consent = Read-Host "Do you want to proceed? (Y/N)"
if ($consent -ne "Y" -and $consent -ne "y") {
    Write-Host "Script execution cancelled by user." -ForegroundColor Yellow
    exit 0
}

# Check and install WinGet if not present
Write-Host "Checking for WinGet installation..." -ForegroundColor Cyan
if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {
    Write-Host "WinGet not found. Installing..." -ForegroundColor Yellow
    
    # Download the WinGet installer
    $wingetUrl = "https://aka.ms/getwinget"
    $installerPath = "$env:TEMP\Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.msixbundle"
    
    Write-Host "Downloading WinGet installer..."
    Invoke-WebRequest -Uri $wingetUrl -OutFile $installerPath
    
    # Install WinGet
    Write-Host "Installing WinGet..."
    Add-AppxPackage -Path $installerPath
    
    # Clean up
    Remove-Item -Path $installerPath -Force
    
    # Verify installation
    if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {
        Exit-OnError 'Failed to install WinGet. Please install it manually from https://aka.ms/getwinget'
    }
    Write-Host "WinGet installed successfully." -ForegroundColor Green
} else {
    Write-Host "WinGet is already installed." -ForegroundColor Green
}

# Check and install Azure CLI if not present
Write-Host "Checking for Azure CLI installation..." -ForegroundColor Cyan
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "Azure CLI not found. Installing..." -ForegroundColor Yellow
    winget install Microsoft.AzureCLI
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to install Azure CLI.' }
    Write-Host "Azure CLI installed successfully." -ForegroundColor Green
} else {
    Write-Host "Azure CLI is already installed." -ForegroundColor Green
}

# Check and install GitHub CLI if not present
Write-Host "Checking for GitHub CLI installation..." -ForegroundColor Cyan
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "GitHub CLI not found. Installing..." -ForegroundColor Yellow
    winget install GitHub.cli
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to install GitHub CLI.' }
    
    # Refresh environment variables to include the newly installed GitHub CLI
    Write-Host "Refreshing environment variables..." -ForegroundColor Yellow
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
    
    # Verify installation after PATH refresh
    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
        Write-Host "GitHub CLI installation completed but not found in PATH. Please restart your PowerShell session and run the script again." -ForegroundColor Red
        exit 1
    }
    Write-Host "GitHub CLI installed successfully." -ForegroundColor Green
} else {
    Write-Host "GitHub CLI is already installed." -ForegroundColor Green
}

# Prompt for required parameters
$SUBSCRIPTION_ID = Read-Host "Enter your Azure Subscription ID"
$RESOURCE_GROUP = Read-Host "Enter your Resource Group name (will be created if not existing)"
$MANAGED_IDENTITY_NAME = Read-Host "Enter your Managed Identity name (will be created if not existing)"

# Get and validate GitHub repository URL
do {
    $REPO_URL = Read-Host "Enter your GitHub repository URL (e.g. https://github.com/username/repo-name or https://github.com/username/repo-name.git)"
    if (-not ($REPO_URL -match '^https://github\.com/[^/]+/[^/]+(\.git)?$')) {
        Write-Host "Invalid repository URL format. Please enter a URL in the format: https://github.com/username/repo-name or https://github.com/username/repo-name.git" -ForegroundColor Red
        continue
    }
    # Extract repo name from URL (remove .git if present)
    $REPO_NAME = $REPO_URL -replace '^https://github\.com/(.+)\.git$', '$1' -replace '^https://github\.com/(.+)$', '$1'
    Write-Host "DEBUG: Extracted REPO_NAME: '$REPO_NAME'" -ForegroundColor Gray
} while (-not ($REPO_URL -match '^https://github\.com/[^/]+/[^/]+(\.git)?$'))

# Set the subscription
az account set --subscription $SUBSCRIPTION_ID
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to set Azure subscription.' }

# Check if resource group exists
Write-Host "Checking if resource group '$RESOURCE_GROUP' exists..."
$rgExists = az group exists --name $RESOURCE_GROUP | ConvertFrom-Json
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to check resource group existence.' }
if (-not $rgExists) {
    $REGION = Read-Host "Enter Azure region for the resource group [centralus]"
    if ([string]::IsNullOrWhiteSpace($REGION)) { $REGION = "centralus" }
    Write-Host "Resource group not found. Creating resource group in region '$REGION'..."
    az group create --name $RESOURCE_GROUP --location $REGION | Out-Null
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to create resource group.' }
} else {
    Write-Host "Resource group already exists."
}

# Check if managed identity exists
Write-Host "Checking if managed identity '$MANAGED_IDENTITY_NAME' exists in resource group '$RESOURCE_GROUP'..."
$identity = az identity show --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query 'name' -o tsv 2>$null
if ($LASTEXITCODE -ne 0) { $identity = $null }
if (-not $identity) {
    Write-Host "Managed identity not found. Creating managed identity..."
    az identity create --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP | Out-Null
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to create managed identity.' }
    Write-Host "Waiting for managed identity to propagate in Azure AD..."
    Start-Sleep -Seconds 30
} else {
    Write-Host "Managed identity already exists."
}

# Federated credential details
function New-FederatedCredentialName {
    $base = "cred$((Get-Date).ToUniversalTime().Ticks.ToString('x8'))$([System.Guid]::NewGuid().ToString('N').Substring(0, 8))"
    # Only allow letters, numbers, hyphens, dashes; must start with letter/number, 3-120 chars
    $name = $base -replace '[^A-Za-z0-9-]', ''
    if ($name.Length -gt 120) { $name = $name.Substring(0,120) }
    if ($name.Length -lt 3) { $name = $name.PadRight(3, '0') }
    if ($name[0] -notmatch '[A-Za-z0-9]') { $name = "a$name" }
    return $name
}
$CREDENTIAL_NAME = New-FederatedCredentialName
$ISSUER = "https://token.actions.githubusercontent.com"
$SUBJECT = 'repo:' + $REPO_NAME + ':ref:refs/heads/main'
Write-Host "DEBUG: SUBJECT will be: $SUBJECT"
$AUDIENCE = "api://AzureADTokenExchange"

# Check for existing federated credentials with the same subject
Write-Host "Checking for existing federated credentials..."
$existingCreds = az identity federated-credential list --identity-name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query "[?subject=='$SUBJECT']" -o json | ConvertFrom-Json
if ($existingCreds -and $existingCreds.Count -gt 0) {
    Write-Host "Found existing federated credentials. Removing them..."
    foreach ($cred in $existingCreds) {
        Write-Host "Removing credential: $($cred.name)"
        az identity federated-credential delete --identity-name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --name $cred.name 
        if ($LASTEXITCODE -ne 0) { Exit-OnError "Failed to delete existing federated credential: $($cred.name)" }
    }
    Write-Host "Waiting for credential deletion to complete..."
    Start-Sleep -Seconds 10

    # Verify that the credentials were actually deleted
    Write-Host "Verifying credential deletion..."
    $remainingCreds = az identity federated-credential list --identity-name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query "[?subject=='$SUBJECT']" -o json | ConvertFrom-Json
    if ($remainingCreds -and $remainingCreds.Count -gt 0) {
        Write-Host "Warning: Some credentials were not deleted. Attempting to force delete..." -ForegroundColor Yellow
        foreach ($cred in $remainingCreds) {
            Write-Host "Force removing credential: $($cred.name)"
            az identity federated-credential delete --identity-name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --name $cred.name --yes
            if ($LASTEXITCODE -ne 0) { 
                Write-Host "Warning: Could not force delete credential: $($cred.name)" -ForegroundColor Yellow
            }
        }
        Start-Sleep -Seconds 10
    }
}

# Create the federated credential
Write-Host "Creating new federated credential..."
$maxAttempts = 3
$attempt = 1
$success = $false

while (-not $success -and $attempt -le $maxAttempts) {
    try {
        az identity federated-credential create `
            --name $CREDENTIAL_NAME `
            --identity-name $MANAGED_IDENTITY_NAME `
            --resource-group $RESOURCE_GROUP `
            --issuer $ISSUER `
            --subject "$SUBJECT" `
            --audiences $AUDIENCE | Out-Null
        
        if ($LASTEXITCODE -eq 0) {
            $success = $true
            Write-Host "Federated credential created successfully." -ForegroundColor Green
        } else {
            throw "Failed to create federated credential"
        }
    }
    catch {
        if ($attempt -eq $maxAttempts) {
            Exit-OnError "Failed to create federated credential after $maxAttempts attempts. Error: $_"
        }
        Write-Host "Attempt $attempt failed. Retrying in 10 seconds..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        $attempt++
    }
}

# Get the managed identity's client ID, subscription ID, and tenant ID
$MANAGED_IDENTITY_CLIENTID = az identity show --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query 'clientId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get managed identity client ID.' }
$TENANTID = az account show --query 'tenantId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get tenant ID.' }

# Assign the Owner role to the managed identity at the resource group scope
Write-Host "Assigning Owner role to managed identity..."
$IDENTITY_OBJECT_ID = az identity show --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query 'principalId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get managed identity principal ID.' }

Invoke-WithRetry -ScriptBlock {
    az role assignment create --assignee $IDENTITY_OBJECT_ID --role "Owner" --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Failed to assign Owner role." }
} -MaxAttempts 3 -DelaySeconds 30

# Output the variables for GitHub secrets
Write-Host ""
Write-Host ""
Write-Host "=============================================================="
Write-Host "Setting up GitHub repository secrets..."
Write-Host "=============================================================="

# Check if user is logged into GitHub CLI
Write-Host "Checking GitHub CLI authentication status..."
try {
    $ghAuth = gh auth status 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "You need to authenticate with GitHub CLI first." -ForegroundColor Yellow
        Write-Host "Please use your personal GitHub account (not an Enterprise Managed User/EMU account) for authentication." -ForegroundColor Yellow
        gh auth login
        if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to authenticate with GitHub CLI.' }
    }
} catch {
    Write-Host "Error: GitHub CLI is not properly installed or not in PATH. Please restart your PowerShell session and run the script again." -ForegroundColor Red
    exit 1
}

# Set GitHub secrets
Write-Host "Setting MANAGED_IDENTITY_CLIENTID secret..."
try {
    gh secret set MANAGED_IDENTITY_CLIENTID --body $MANAGED_IDENTITY_CLIENTID --repo $REPO_NAME
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to set MANAGED_IDENTITY_CLIENTID secret.' }
} catch {
    Write-Host "Error: Failed to set MANAGED_IDENTITY_CLIENTID secret. Please check if GitHub CLI is properly installed and you're authenticated." -ForegroundColor Red
    exit 1
}

Write-Host "Setting SUBSCRIPTIONID secret..."
try {
    gh secret set SUBSCRIPTIONID --body $SUBSCRIPTION_ID --repo $REPO_NAME
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to set SUBSCRIPTIONID secret.' }
} catch {
    Write-Host "Error: Failed to set SUBSCRIPTIONID secret. Please check if GitHub CLI is properly installed and you're authenticated." -ForegroundColor Red
    exit 1
}

Write-Host "Setting TENANTID secret..."
try {
    gh secret set TENANTID --body $TENANTID --repo $REPO_NAME
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to set TENANTID secret.' }
} catch {
    Write-Host "Error: Failed to set TENANTID secret. Please check if GitHub CLI is properly installed and you're authenticated." -ForegroundColor Red
    exit 1
}

Write-Host "Setting RESOURCE_GROUP_NAME secret..."
try {
    gh secret set RESOURCE_GROUP_NAME --body $RESOURCE_GROUP --repo $REPO_NAME
    if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to set RESOURCE_GROUP_NAME secret.' }
} catch {
    Write-Host "Error: Failed to set RESOURCE_GROUP_NAME secret. Please check if GitHub CLI is properly installed and you're authenticated." -ForegroundColor Red
    exit 1
}

Write-Host "All GitHub secrets have been set successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Resource group ensured, managed identity ensured, federated credential created, Owner role assigned to the managed identity, and GitHub secrets configured." 

# Inform user about the trigger-workflows script
Write-Host ""
Write-Host "=============================================================="
Write-Host "To trigger GitHub Actions workflows and start the deployment process:" -ForegroundColor Cyan
Write-Host "Run the trigger-workflows.ps1 script in this directory." -ForegroundColor Cyan
Write-Host "This script will help you trigger all or specific workflows." -ForegroundColor Cyan
Write-Host "==============================================================" 