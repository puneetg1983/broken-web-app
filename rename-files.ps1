# Rename workflow files
Get-ChildItem -Path ".github/workflows" -Filter "main_broken-*.yml" | ForEach-Object {
    $newName = $_.Name -replace "main_", ""
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

# Rename bicep files
Get-ChildItem -Path "." -Filter "main_broken-*.bicep" | ForEach-Object {
    $newName = $_.Name -replace "main_", ""
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

Get-ChildItem -Path "infra" -Filter "main_broken-*.bicep" | ForEach-Object {
    $newName = $_.Name -replace "main_", ""
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

# Rename JSON files
Get-ChildItem -Path "infra" -Filter "main_broken-*.json" | ForEach-Object {
    $newName = $_.Name -replace "main_", ""
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

# Update references in workflow files
Get-ChildItem -Path ".github/workflows" -Filter "broken-*.yml" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace "main_broken-", "broken-"
    Set-Content -Path $_.FullName -Value $content -Force
}

# Rename bicep files
Get-ChildItem -Path "infra" -Filter "broken-webapp-aspnet-*.bicep" | ForEach-Object {
    $newName = $_.Name -replace "broken-webapp-aspnet-", ''
    Rename-Item -Path $_.FullName -NewName $newName -Force
}
Get-ChildItem -Path "infra" -Filter "main_broken-webapp-aspnet-*.bicep" | ForEach-Object {
    $newName = $_.Name -replace "main_broken-webapp-aspnet-", ''
    Rename-Item -Path $_.FullName -NewName $newName -Force
}

# Update bicep references in all workflow YAML files
Get-ChildItem -Path ".github/workflows" -Filter "*.yml" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace "template: ./infra/broken-webapp-aspnet-", "template: ./infra/"
    $content = $content -replace "template: ./infra/main_broken-webapp-aspnet-", "template: ./infra/"
    Set-Content -Path $_.FullName -Value $content -Force
} 