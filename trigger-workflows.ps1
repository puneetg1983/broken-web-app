# PowerShell script to trigger GitHub Actions workflows

param(
    [Parameter()]
    [string]$RepoName
)

# Function to handle errors
function Exit-OnError {
    param($Message)
    Write-Host $Message -ForegroundColor Red
    exit 1
}

# If no repository name provided, prompt for it
if (-not $RepoName) {
    Write-Host "Please enter your GitHub repository URL (e.g., https://github.com/username/repo or https://github.com/username/repo.git):" -ForegroundColor Cyan
    $RepoUrl = Read-Host
    
    # Extract repository name from URL
    if ($RepoUrl -match 'github\.com/([^/]+/[^/]+?)(?:\.git)?$') {
        $RepoName = $matches[1]
    } else {
        Exit-OnError "Invalid GitHub repository URL format. Please use format: https://github.com/username/repo or https://github.com/username/repo.git"
    }
}

Write-Host "Using repository: $RepoName" -ForegroundColor Yellow

# Get all workflow files
Write-Host "Finding GitHub Actions workflows..." -ForegroundColor Yellow
try {
    $workflowsJson = gh api repos/$RepoName/actions/workflows
    if (-not $workflowsJson) {
        Exit-OnError "Could not find any workflow files. Please check your repository structure."
    }

    # Convert the JSON to PowerShell objects and filter for YAML files
    $workflowList = $workflowsJson | ConvertFrom-Json | Select-Object -ExpandProperty workflows | 
        Where-Object { $_.path -match '\.(yml|yaml)$' }

    if (-not $workflowList -or $workflowList.Count -eq 0) {
        Exit-OnError "No workflow files found. Please check your repository structure."
    }

    Write-Host "Found $($workflowList.Count) workflow(s):" -ForegroundColor Cyan
    foreach ($workflow in $workflowList) {
        Write-Host "- $($workflow.name) ($($workflow.path))" -ForegroundColor Cyan
    }

    # Ask user if they want to trigger all workflows or select specific ones
    Write-Host "`nWould you like to:" -ForegroundColor Yellow
    Write-Host "1. Trigger all workflows"
    Write-Host "2. Select specific workflows to trigger"
    $choice = Read-Host "Enter your choice (1 or 2)"

    if ($choice -eq "1") {
        Write-Host "`nTriggering all workflows..." -ForegroundColor Yellow
        foreach ($workflow in $workflowList) {
            Write-Host "Triggering workflow: $($workflow.name)..." -ForegroundColor Yellow
            gh workflow run $workflow.id --repo $RepoName
            if ($LASTEXITCODE -ne 0) { 
                Write-Host "Failed to trigger workflow: $($workflow.name). Please check your repository permissions." -ForegroundColor Red
                continue
            }
            Write-Host "Successfully triggered workflow: $($workflow.name)" -ForegroundColor Green
        }
    } else {
        Write-Host "`nSelect workflows to trigger (enter numbers separated by commas):" -ForegroundColor Yellow
        for ($i = 0; $i -lt $workflowList.Count; $i++) {
            Write-Host "$($i + 1). $($workflowList[$i].name) ($($workflowList[$i].path))"
        }
        
        $selected = Read-Host "Enter workflow numbers (e.g., 1,3,4)"
        $selectedIndices = $selected -split ',' | ForEach-Object { [int]$_ - 1 }
        
        foreach ($index in $selectedIndices) {
            if ($index -ge 0 -and $index -lt $workflowList.Count) {
                $workflow = $workflowList[$index]
                Write-Host "Triggering workflow: $($workflow.name)..." -ForegroundColor Yellow
                gh workflow run $workflow.id --repo $RepoName
                if ($LASTEXITCODE -ne 0) { 
                    Write-Host "Failed to trigger workflow: $($workflow.name). Please check your repository permissions." -ForegroundColor Red
                    continue
                }
                Write-Host "Successfully triggered workflow: $($workflow.name)" -ForegroundColor Green
            }
        }
    }

    Write-Host "`nAll selected workflows have been triggered!" -ForegroundColor Green
    Write-Host "You can monitor the workflow progress at: https://github.com/$RepoName/actions" -ForegroundColor Cyan
} catch {
    Write-Host "Error triggering GitHub Actions workflows: $_" -ForegroundColor Red
    Write-Host "You can manually trigger the workflows from the Actions tab in your GitHub repository." -ForegroundColor Yellow
} 