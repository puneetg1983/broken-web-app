# Diagnostic Scenarios Web Application

This web application demonstrates various diagnostic scenarios that can occur in ASP.NET applications. Each scenario is designed to help developers understand and debug common issues in web applications.

## Scenarios

### High CPU Scenarios

1. **High CPU Scenario 1 - Infinite Loop**
   - Demonstrates CPU usage spike caused by an infinite loop
   - Creates multiple threads that perform CPU-intensive calculations
   - Reference: Common performance issue in web applications

2. **High CPU Scenario 2 - Thread Contention**
   - Shows how thread contention can cause high CPU usage
   - Multiple threads compete for the same lock, causing context switching
   - Reference: Common threading issue in multi-threaded applications

3. **High CPU Scenario 3 - Deadlock**
   - Demonstrates a classic deadlock situation
   - Two threads wait for each other's locks, causing CPU spinning
   - Reference: Common deadlock pattern in concurrent applications

### High Memory Scenarios

1. **High Memory Scenario 1 - Memory Leak**
   - Demonstrates a memory leak through static collections
   - Objects are added to a static list and never removed
   - Reference: Common memory leak pattern in web applications

2. **High Memory Scenario 2 - Event Handler Leak**
   - Shows memory leak caused by not unsubscribing event handlers
   - Event handlers are added but never removed
   - Reference: Tess Ferrandez's blog on memory leaks

3. **High Memory Scenario 3 - LOH Fragmentation**
   - Demonstrates Large Object Heap fragmentation
   - Creates and removes large objects, causing memory fragmentation
   - Reference: Tess Ferrandez's blog on LOH fragmentation

### Crash Scenarios

1. **Crash Scenario 1 - Unhandled Exception**
   - Demonstrates application crash due to unhandled exception
   - Throws an exception in a background thread
   - Reference: Common crash pattern in web applications

2. **Crash Scenario 2 - Stack Overflow**
   - Shows stack overflow exception through infinite recursion
   - Creates large arrays on the stack in recursive calls
   - Reference: Common stack overflow pattern

3. **Crash Scenario 3 - Out of Memory**
   - Demonstrates out of memory exception
   - Allocates large arrays until memory is exhausted
   - Reference: Common memory exhaustion pattern

### HTTP 500 Scenarios

1. **HTTP 500 Scenario 1 - Database Connection Error**
   - Shows HTTP 500 error due to database connection failure
   - Attempts to connect to a non-existent database server
   - Reference: Common database connectivity issue

2. **HTTP 500 Scenario 2 - File Access Error**
   - Demonstrates HTTP 500 error due to file access failure
   - Attempts to access a restricted system file
   - Reference: Common file access permission issue

3. **HTTP 500 Scenario 3 - Configuration Error**
   - Shows HTTP 500 error due to missing configuration
   - Attempts to access a non-existent configuration setting
   - Reference: Common configuration management issue

### Connection Pool Scenarios

1. **Connection Pool Scenario 1 - Connection Leak**
   - Demonstrates connection pool exhaustion through leaks
   - Creates and doesn't dispose of database connections
   - Reference: Common connection leak pattern

2. **Connection Pool Scenario 2 - Connection Timeout**
   - Shows connection timeout issues
   - Attempts to connect to a non-existent server with short timeout
   - Reference: Common connection timeout pattern

3. **Connection Pool Scenario 3 - Pool Exhaustion**
   - Demonstrates connection pool exhaustion
   - Holds multiple connections open for long-running operations
   - Reference: Common connection pool management issue

## References

- Tess Ferrandez's Blog: [ASP.NET Memory Leaks](https://blogs.msdn.microsoft.com/tess/)
- Microsoft Documentation: [ASP.NET Performance](https://docs.microsoft.com/en-us/aspnet/performance)
- SQL Server Documentation: [Connection Pooling](https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-connection-pooling)

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the application
4. Navigate to the scenarios you want to test

## Note

These scenarios are designed for educational purposes and should not be used in production environments. They demonstrate common issues that can occur in web applications and how to diagnose them.

## Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8
- IIS Express (included with Visual Studio)
- Azure subscription (for deployment)

## Running Locally

1. Clone the repository
2. Open the solution in Visual Studio
3. Build the solution
4. Press F5 to run the application
5. Navigate to the desired scenario page

## Deployment to Azure

### Using GitHub Actions

#### Prerequisites
1. A GitHub account (create one at [github.com](https://github.com) if you don't have one)
2. An Azure account with an active subscription
3. Basic knowledge of Git (you can learn at [git-scm.com](https://git-scm.com))

#### Setting Up GitHub Repository
1. Fork this repository:
   - Click the "Fork" button in the top-right corner of this GitHub page
   - This creates your own copy of the repository

2. Clone your forked repository:
   ```bash
   git clone https://github.com/YOUR-USERNAME/broken-web-app.git
   cd broken-web-app
   ```

#### Configuring GitHub Secrets
1. Go to your forked repository on GitHub
2. Click on "Settings" tab
3. In the left sidebar, click on "Secrets and variables" → "Actions"
4. Click on "New repository secret"
5. Add the following secrets one by one:

   a. `AZURE_CREDENTIALS`:
   - Create an Azure service principal:
     ```bash
     az ad sp create-for-rbac --name "myApp" --role contributor \
                             --scopes /subscriptions/{subscription-id} \
                             --sdk-auth
     ```
   - Copy the entire JSON output and paste it as the secret value

   b. `AZURE_SUBSCRIPTION_ID`:
   - Get your subscription ID:
     ```bash
     az account show --query id -o tsv
     ```
   - Copy the output and paste it as the secret value

   c. `AZURE_RESOURCE_GROUP`:
   - Create a resource group in Azure:
     ```bash
     az group create --name myResourceGroup --location eastus
     ```
   - Use the resource group name as the secret value

   d. `AZURE_WEBAPP_NAME`:
   - Choose a unique name for your web app (e.g., "my-diagnostic-app")
   - This name must be globally unique across all of Azure
   - Use this name as the secret value

   e. `AZURE_LOCATION`:
   - Choose an Azure region (e.g., "eastus", "westeurope")
   - Common values:
     - `eastus` (East US)
     - `westeurope` (West Europe)
     - `southeastasia` (Southeast Asia)
   - Use the region name as the secret value

#### Understanding the Deployment Process
The GitHub Actions workflow will automatically:
1. Build the application:
   - Compiles the .NET code
   - Creates the deployment package

2. Deploy the infrastructure:
   - Uses Azure Bicep templates to create:
     - Azure Web App
     - Required networking components
     - Security configurations

3. Deploy the application:
   - Uploads the compiled application
   - Configures the web app settings
   - Starts the application

#### Monitoring the Deployment
1. Go to your repository on GitHub
2. Click on the "Actions" tab
3. You'll see the deployment workflow running
4. Click on the workflow to see detailed progress
5. Green checkmarks indicate successful steps
6. Red X marks indicate failed steps (click to see error details)

#### Troubleshooting Common Issues
1. **Workflow fails to start**:
   - Check if all required secrets are configured
   - Verify your Azure credentials are correct

2. **Build fails**:
   - Check the build logs in the Actions tab
   - Verify your code compiles locally

3. **Deployment fails**:
   - Check Azure portal for resource creation status
   - Verify your Azure subscription has sufficient permissions

4. **Web App not accessible**:
   - Check the deployment logs
   - Verify the web app is running in Azure portal

### Manual Deployment

1. Publish the application from Visual Studio
2. Create an Azure App Service
3. Deploy the published files to the App Service

## Infrastructure as Code

The application includes Azure Bicep templates for infrastructure deployment:

- `main.bicep`: Main template for resource deployment
- `app-service.bicep`: App Service specific configuration
- `network.bicep`: Network security and configuration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Security

This application is designed for testing and development purposes only. Do not deploy to production environments without proper security review and modifications. 