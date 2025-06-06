# Diagnostic Scenarios for Azure App Service - Windows Apps

This codebase provides a collection of intentionally broken or problematic web application scenarios designed to help you test and validate diagnostics and troubleshooting tools for Azure App Service. It includes:

* A variety of web app scenarios that simulate real-world failures and performance issues
* Automated pipelines that deploy these apps and execute tests
* Infrastructure-as-Code (IaC) using Bicep templates for consistent Azure resource provisioning

# Setup Instructions

## What does setup.ps1 do?

The `setup.ps1` PowerShell script automates the following:

* Ensures your Azure resource group exists (creates it if needed)
* Ensures a managed identity exists (creates it if needed)
* Sets up a federated credential for GitHub Actions OIDC
* Assigns the Owner role to the managed identity on the resource group

## Prerequisites

Before running `setup.ps1`, keep the following information handy:

* Your Azure Subscription ID
* The name of your Azure Resource Group (will be created if not existing)
* The name you want for your Managed Identity (will be created if not existing)
* Your cloned GitHub repo name (e.g. yourgithubaccount/broken-web-app)

## Getting Started

1. Fork this repository:  
   * Click the "Fork" button in the top-right corner of this repository's page  
   * Select your GitHub account as the destination for the fork  
   * Wait for the forking process to complete  
   * You'll be redirected to your forked copy of the repository
2. Run the setup script in PowerShell:  
      ```
      Invoke-RestMethod -Uri 'https://raw.githubusercontent.com/puneetg1983/broken-web-app/refs/heads/main/setup.ps1' -OutFile 'setup.ps1'  
      .\setup.ps1
      ```
3. The script will:
   - Show you what actions it will perform and ask for your consent
   - Install any missing required tools (WinGet, Azure CLI, GitHub CLI)
   - Prompt you for necessary Azure information
   - Ask for your GitHub repository URL
   - Automatically set up all required GitHub secrets using GitHub CLI

   Note: If you're not already authenticated with GitHub CLI, the script will prompt you to log in during the process.

## Trigger GitHub Actions workflows

To trigger GitHub Actions workflows, run the trigger script in PowerShell:
   ```
   Invoke-RestMethod -Uri 'https://raw.githubusercontent.com/puneetg1983/broken-web-app/refs/heads/main/trigger-workflows.ps1' -OutFile 'trigger-workflows.ps1'
   .\trigger-workflows.ps1
   ```
The trigger script will:
- Find all available workflows in your repository
- Let you choose between triggering all workflows or selecting specific ones
- Show you the progress of triggered workflows
- You can run this script anytime to trigger workflows or check their status

## Scenarios

For details on different scenarios created by this repository, see [scenarios.md](scenarios.md). 

