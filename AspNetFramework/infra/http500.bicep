param appServiceName string
param appServicePlanName string
param logAnalyticsName string

var location = resourceGroup().location
var webAppUrl = 'https://${webApp.properties.defaultHostName}'
var appInsightsName = '${appServiceName}-insights'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: logAnalyticsName
}

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

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'windows'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      alwaysOn: true
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
      netFrameworkVersion: 'v4.8'
    }
  }
}

resource appInsightsWebTestHttp500_1 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-1'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-1'
    Name: '${appServiceName}-webtest-http500-1'
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
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_1.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestHttp500_1Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-1-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-1-actual'
    Name: '${appServiceName}-webtest-http500-1-actual'
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

resource appInsightsWebTestHttp500_2 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-2'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-2'
    Name: '${appServiceName}-webtest-http500-2'
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
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_2.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestHttp500_2Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-2-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-2-actual'
    Name: '${appServiceName}-webtest-http500-2-actual'
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

resource appInsightsWebTestHttp500_3 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-3'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-3'
    Name: '${appServiceName}-webtest-http500-3'
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
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_3.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestHttp500_3Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-3-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-3-actual'
    Name: '${appServiceName}-webtest-http500-3-actual'
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

resource appInsightsWebTestHttp500_4 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-http500-4'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-http500-4'
    Name: '${appServiceName}-webtest-http500-4'
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

resource appInsightsWebTestServerErrors1 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-1'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-1'
    Name: '${appServiceName}-webtest-servererrors-1'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors1.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestServerErrors1Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-1-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-1-actual'
    Name: '${appServiceName}-webtest-servererrors-1-actual'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors1Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestServerErrors2 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-2'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-2'
    Name: '${appServiceName}-webtest-servererrors-2'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors2.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestServerErrors2Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-2-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-2-actual'
    Name: '${appServiceName}-webtest-servererrors-2-actual'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors2Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestServerErrors3 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-3'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-3'
    Name: '${appServiceName}-webtest-servererrors-3'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors3.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
}

resource appInsightsWebTestServerErrors3Actual 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appServiceName}-webtest-servererrors-3-actual'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appServiceName}-webtest-servererrors-3-actual'
    Name: '${appServiceName}-webtest-servererrors-3-actual'
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
      RequestUrl: '${webAppUrl}/Scenarios/ServerErrors/ServerErrors3Actual.aspx'
    }
    ValidationRules: {
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 100
    }
  }
} 
