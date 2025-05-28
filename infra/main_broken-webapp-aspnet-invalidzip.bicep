param appServiceName string
var appServicePlanName = appServiceName
var location = resourceGroup().location

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
  kind: 'windows'
  properties: {
    reserved: false
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'windows'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'WEBAPP_INVALID_ZIP'
          value: 'true'
        }, {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }, {
          name: 'WEBSITE_RUN_FROM_PACKAGE_URL'
          value: 'https://github.com/Azure-Samples/dotnet-framework-azure-web-app/archive/refs/heads/main.zip'
        }
      ]
      netFrameworkVersion: 'v4.8'
    }
  }
} 
