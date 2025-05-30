# PowerShell script to automate Azure setup for diagnostic scenarios

function Exit-OnError {
    param([string]$Message)
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

# Prompt for required parameters
$SUBSCRIPTION_ID = Read-Host "Enter your Azure Subscription ID"
$RESOURCE_GROUP = Read-Host "Enter your Resource Group name (will be created if not existing)"
$MANAGED_IDENTITY_NAME = Read-Host "Enter your Managed Identity name (will be created if not existing)"
do {
    $REPO_NAME = Read-Host "Enter your cloned GitHub repo name (e.g. yourgithubaccount/broken-web-app)"
    if ([string]::IsNullOrWhiteSpace($REPO_NAME)) {
        Write-Host "Repository name cannot be empty. Please enter a value like 'yourgithubaccount/broken-web-app'." -ForegroundColor Yellow
    }
} while ([string]::IsNullOrWhiteSpace($REPO_NAME))
Write-Host "DEBUG: REPO_NAME entered: '$REPO_NAME'"

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

# Create the federated credential
az identity federated-credential create `
    --name $CREDENTIAL_NAME `
    --identity-name $MANAGED_IDENTITY_NAME `
    --resource-group $RESOURCE_GROUP `
    --issuer $ISSUER `
    --subject "$SUBJECT" `
    --audiences $AUDIENCE | Out-Null
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to create federated credential.' }

# Get the managed identity's client ID, subscription ID, and tenant ID
$MANAGED_IDENTITY_CLIENTID = az identity show --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query 'clientId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get managed identity client ID.' }
$TENANTID = az account show --query 'tenantId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get tenant ID.' }

# Assign the Owner role to the managed identity at the resource group scope
$IDENTITY_OBJECT_ID = az identity show --name $MANAGED_IDENTITY_NAME --resource-group $RESOURCE_GROUP --query 'principalId' -o tsv
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to get managed identity principal ID.' }
az role assignment create --assignee $IDENTITY_OBJECT_ID --role "Owner" --scope "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP" | Out-Null
if ($LASTEXITCODE -ne 0) { Exit-OnError 'Failed to assign Owner role.' }

# Output the variables for GitHub secrets
Write-Host ""
Write-Host ""
Write-Host "=============================================================="
Write-Host "Add the following values as GitHub repository secrets:"
Write-Host "=============================================================="
Write-Host "MANAGED_IDENTITY_CLIENTID=$MANAGED_IDENTITY_CLIENTID"
Write-Host "SUBSCRIPTIONID=$SUBSCRIPTION_ID"
Write-Host "TENANTID=$TENANTID"
Write-Host "RESOURCE_GROUP_NAME=$RESOURCE_GROUP"
Write-Host "=============================================================="
Write-Host "To add these secrets, go to your GitHub repository, then navigate to:"
Write-Host "Settings -> Secrets and variables -> Actions -> New repository secret"
Write-Host ""
Write-Host ""
Write-Host "Resource group ensured, managed identity ensured, federated credential created, and Owner role assigned to the managed identity." 