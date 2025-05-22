# Set environment variables for ARM metrics tests
$env:WEBAPP_URL = "https://broken-webapp-aspnet-fmfzf8fdakanh8gm.canadacentral-01.azurewebsites.net"
$env:SUBSCRIPTION_ID = "6b6db65f-680e-4650-b97d-e82ed6a0f583"
$env:RESOURCE_GROUP_NAME = "broken-web-apps"
$env:APP_SERVICE_NAME = "broken-webapp-aspnet"
$env:RUN_ARM_METRICS_TESTS_LOCALLY = "true"

# Check if user is logged into Azure
$account = az account show 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Please log in to Azure first using 'az login'"
    exit 1
}

# Check if we're on the correct subscription
$currentSub = (az account show --query id -o tsv)
if ($currentSub -ne $env:SUBSCRIPTION_ID) {
    Write-Host "Switching to subscription $($env:SUBSCRIPTION_ID)..."
    az account set --subscription $env:SUBSCRIPTION_ID
}

# Find MSBuild
$msbuildPath = $null
$possiblePaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $msbuildPath = $path
        break
    }
}

if ($null -eq $msbuildPath) {
    Write-Host "Could not find MSBuild. Please ensure Visual Studio is installed."
    exit 1
}

Write-Host "Using MSBuild from: $msbuildPath"

# Restore NuGet packages
Write-Host "Restoring NuGet packages..."
& $msbuildPath DiagnosticScenarios.sln /t:Restore /p:Configuration=Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to restore NuGet packages"
    exit 1
}

# Build the solution
Write-Host "Building solution..."
& $msbuildPath DiagnosticScenarios.sln /p:Configuration=Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to build solution"
    exit 1
}

# Run the tests
Write-Host "Running tests..."
$testDll = "DiagnosticScenarios.Tests\bin\Release\net48\DiagnosticScenarios.Tests.dll"
if (-not (Test-Path $testDll)) {
    Write-Host "Test DLL not found at: $testDll"
    exit 1
}

# Find NUnit Console Runner
$nunitConsole = $null
$possibleNUnitPaths = @(
    "packages\NUnit.ConsoleRunner.3.20.0\tools\nunit3-console.exe",
    "packages\NUnit.ConsoleRunner\*\tools\nunit3-console.exe",
    "..\packages\NUnit.ConsoleRunner\*\tools\nunit3-console.exe"
)

foreach ($path in $possibleNUnitPaths) {
    if (Test-Path $path) {
        $nunitConsole = $path
        break
    }
}

if ($null -eq $nunitConsole) {
    Write-Host "NUnit Console Runner not found. Installing..."
    nuget install NUnit.ConsoleRunner -Version 3.20.0 -OutputDirectory packages
    $nunitConsole = "packages\NUnit.ConsoleRunner.3.20.0\tools\nunit3-console.exe"
    
    if (-not (Test-Path $nunitConsole)) {
        Write-Host "Failed to find NUnit Console Runner after installation"
        exit 1
    }
}

Write-Host "Using NUnit Console Runner from: $nunitConsole"
& $nunitConsole $testDll --where "cat==ArmMetrics" --noheader --noresult 