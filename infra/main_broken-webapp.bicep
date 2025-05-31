param appServiceName string
param appInsightsName string = '${appServiceName}-insights'

var appServicePlanName = appServiceName
var location = resourceGroup().location
var webAppUrl = 'https://${webApp.properties.defaultHostName}'

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
  kind: 'app'
  properties: {
    reserved: false
  }
}

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

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPINSIGHTS_PROFILERFEATURE_VERSION'
          value: '1.0.0'
        }
        {
          name: 'APPINSIGHTS_SNAPSHOTFEATURE_VERSION'
          value: '1.0.0'
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'DiagnosticServices_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'recommended'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_PreemptSdk'
          value: 'disabled'
        }
      ]
    }
  }
}

resource appInsightsWebTest 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-default'
    Name: '${appServiceName}-webtest-default'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: webAppUrl
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500'
    Name: '${appServiceName}-webtest-500'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_1Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500_1 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500-1'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500-1'
    Name: '${appServiceName}-webtest-500-1'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_1Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500_2 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500-2'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500-2'
    Name: '${appServiceName}-webtest-500-2'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_2Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500_3 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500-3'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500-3'
    Name: '${appServiceName}-webtest-500-3'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_3Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500_4 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500-4'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500-4'
    Name: '${appServiceName}-webtest-500-4'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_4Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTest500_5 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-500-5'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-500-5'
    Name: '${appServiceName}-webtest-500-5'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'standard'
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
      {
        Id: 'us-va-ash-azr'
      }
      {
        Id: 'us-ca-sjc-azr'
      }
      {
        Id: 'emea-gb-db3-azr'
      }
      {
        Id: 'emea-nl-ams-azr'
      }
    ]
    RetryEnabled: true
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_5Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}



