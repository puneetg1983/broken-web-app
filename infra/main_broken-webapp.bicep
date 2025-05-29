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
      ]
    }
  }
}

resource defaultPageTest 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-default-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-default-test'
    Name: 'Default Page Test'
    Description: 'Tests the default.aspx page'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/default.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/default.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500Test1 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-db-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-db-test'
    Name: 'HTTP500 Database Connection Test'
    Description: 'Tests the Database Connection Error scenario'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_1Actual.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_1Actual.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 500
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500Test2 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-file-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-file-test'
    Name: 'HTTP500 File Access Test'
    Description: 'Tests the File Access Error scenario'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_2Actual.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_2Actual.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 500
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500Test3 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-config-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-config-test'
    Name: 'HTTP500 Configuration Test'
    Description: 'Tests the Configuration Error scenario'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_3Actual.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_3Actual.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 500
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500Test4 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-conn-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-conn-test'
    Name: 'HTTP500 Connection String Test'
    Description: 'Tests the Invalid Connection String scenario'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_4Actual.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_4Actual.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 500
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500PageTest1 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-page1-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-page1-test'
    Name: 'HTTP500 Page 1 Test'
    Description: 'Tests the HTTP500_1.aspx page'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_1.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_1.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500PageTest2 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-page2-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-page2-test'
    Name: 'HTTP500 Page 2 Test'
    Description: 'Tests the HTTP500_2.aspx page'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_2.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_2.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500PageTest3 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-page3-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-page3-test'
    Name: 'HTTP500 Page 3 Test'
    Description: 'Tests the HTTP500_3.aspx page'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_3.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_3.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
}

resource http500PageTest4 'Microsoft.Insights/webtests@2022-06-15' = {
  name: '${appInsightsName}-http500-page4-test'
  kind: 'ping'
  location: location
  tags: {
    'hidden-link:${appInsights.id}': 'Resource'
  }
  properties: {
    SyntheticMonitorId: '${appInsightsName}-http500-page4-test'
    Name: 'HTTP500 Page 4 Test'
    Description: 'Tests the HTTP500_4.aspx page'
    Enabled: true
    Frequency: 300
    Timeout: 30
    Kind: 'ping'
    RetryEnabled: true
    Locations: [
      {
        Id: 'us-fl-mia-edge'
      }
    ]
    Configuration: {
      WebTest: '${webAppUrl}/Scenarios/Http500/Http500_4.aspx'
    }
    Request: {
      RequestUrl: '${webAppUrl}/Scenarios/Http500/Http500_4.aspx'
      HttpVerb: 'GET'
      ParseDependentRequests: false
      FollowRedirects: true
      Headers: []
      RequestBody: null
    }
    ValidationRules: {
      ExpectedHttpStatusCode: 200
      IgnoreHttpStatusCode: false
      ContentValidation: null
      SSLCheck: true
      SSLCertRemainingLifetimeCheck: 7
    }
  }
} 
