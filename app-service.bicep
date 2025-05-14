@description('The name of the App Service')
param appServiceName string

@description('The location of the resources')
param location string = resourceGroup().location

@description('The pricing tier of the App Service')
param appServiceSku string = 'S1'

@description('The name of the Application Insights resource')
param appInsightsName string = '${appServiceName}-insights'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: '${appServiceName}-plan'
  location: location
  sku: {
    name: appServiceSku
  }
  kind: 'windows'
  properties: {
    reserved: true
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// App Service
resource appService 'Microsoft.Web/sites@2021-02-01' = {
  name: appServiceName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v4.8'
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Output the App Service URL
output appServiceUrl string = 'https://${appService.properties.defaultHostName}' 