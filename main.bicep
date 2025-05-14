@description('The name of the App Service')
param appServiceName string

@description('The location of the resources')
param location string = resourceGroup().location

@description('The pricing tier of the App Service Plan')
param pricingTier string = 'P1v2'

@description('The name of the Application Insights resource')
param appInsightsName string = '${appServiceName}-insights'

// Deploy network resources
module network 'network.bicep' = {
  name: 'network'
  params: {
    appServiceName: appServiceName
    location: location
  }
}

// Deploy App Service resources
module appService 'app-service.bicep' = {
  name: 'appService'
  params: {
    appServiceName: appServiceName
    location: location
    pricingTier: pricingTier
    appInsightsName: appInsightsName
  }
  dependsOn: [
    network
  ]
}

// Output the App Service URL
output appServiceUrl string = appService.outputs.appServiceUrl 