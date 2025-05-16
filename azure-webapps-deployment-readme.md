# Azure Web Apps Deployment Script

This script automates the creation of 10 Azure Web Apps with GitHub Actions CI/CD setup, similar to the existing app `broken-webapp-aspnet-fmfzf8fdakanh8gm.canadacentral-01.azurewebsites.net`. Each app is named based on a specific diagnostic scenario it represents.

## Prerequisites

Before running the script, ensure you have:

1. Azure CLI installed and configured
2. PowerShell 7+ (for cross-platform support)
3. Git installed and configured
4. Access to the Azure subscription where the existing app is deployed
5. Contributor or Owner permissions on the resource group
6. Existing GitHub workflow file for the app in `.github/workflows/main_broken-webapp-aspnet.yml`

## Running the Script

1. Make sure you're in the repository root directory
2. Run the script:

```powershell
./create-azure-webapps.ps1
```

3. When prompted, log in to your Azure account
4. The script will:
   - Get details of the existing web app
   - Create 10 new Azure Web Apps with scenario-based names (e.g., slowresponse, highmemory, highcpu)
   - Set up GitHub Actions integration
   - Create workflow YML files for each app, copying the exact authentication configuration from the existing workflow

## Scenario-Based Web Apps

The script creates the following named web apps:

1. broken-webapp-aspnet-slowresponse
2. broken-webapp-aspnet-highmemory
3. broken-webapp-aspnet-highcpu
4. broken-webapp-aspnet-deadlock
5. broken-webapp-aspnet-databasetimeout
6. broken-webapp-aspnet-outofmemory
7. broken-webapp-aspnet-threadleak
8. broken-webapp-aspnet-connectionleak
9. broken-webapp-aspnet-invalidoperation
10. broken-webapp-aspnet-uncaughtexception

## After Running the Script

1. Commit and push the new workflow files to your repository:

```bash
git add .github/workflows/*.yml
git commit -m "Add workflow files for scenario-based web apps"
git push
```

2. GitHub Actions will automatically deploy your code to the new web apps once the workflows are pushed

## Troubleshooting

- If any step fails, the script will provide an error message
- Check Azure Portal for web app creation status
- Check GitHub Actions for deployment status
- Reference the generated `azure-webapps-deployment-details.json` file for all created resources

## Important Notes

- Each app has its own GitHub Actions workflow file
- The script preserves the exact authentication configuration from the existing workflow file
- No changes are made to the authentication section of the workflow files
- App names are descriptive of different diagnostic scenarios you might want to test 