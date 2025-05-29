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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJEZWZhdWx0IFBhZ2UgVGVzdCIgSWQ9ImRlZmF1bHQtdGVzdCIgT3duZXI9IiIgUHJpb3JpdHk9IjAiIEVuYWJsZWQ9IlRydWUiIENzc1Byb2plY3RTdHJ1Y3R1cmU9IiIgQ3NzSXRlcmF0aW9uPSIiIFRpbWVvdXQ9IjMwIiBXb3JrSXRlbUlkcz0iIiB4bWxucz0iaHR0cDovL21pY3Jvc29mdC5jb20vc2NoZW1hcy9WaXN1YWxTdHVkaW8vVGVhbVRlc3QvMjAxMCIgRGVzY3JpcHRpb249IiIgQ3JlZGVudGlhbFVzZXJOYW1lPSIiIENyZWRlbnRpYWxQYXNzd29yZD0iIiBQcmVBdXRoZW50aWNhdGU9IlRydWUiIFByb3h5PSJkZWZhdWx0IiBTdG9wT25FcnJvcj0iRmFsc2UiIFJlY29yZGVkUmVzdWx0RmlsZT0iIiBSZXN1bHRzTG9jYWxlPSIiPjxJdGVtcz48UmVxdWVzdCBNZXRob2Q9IkdFVCIgR3VpZD0iYTVmMTAxMjYtZTRjZC01NzBkLTk2MWMtY2VhNDM5YjljNDg0IiBWZXJzaW9uPSIxLjEiIFVybD0iJHt3ZWJBcHBVcmx9L2RlZmF1bHQuYXNweCIgVGhpbmtUaW1lPSIwIiBUaW1lb3V0PSIzMDAiIFBhcnNlRGVwZW5kZW50UmVxdWVzdHM9IkZhbHNlIiBGb2xsb3dSZWRpcmVjdHM9IlRydWUiIFJlY29yZFJlc3VsdD0iVHJ1ZSIgQ2FjaGU9IkZhbHNlIiBSZXNwb25zZVRpbWVHb2FsPSIwIiBFbmNvZGluZz0idXRmLTgiIEV4cGVjdGVkSHR0cFN0YXR1c0NvZGU9IjIwMCIgRXhwZWN0ZWRSZXNwb25zZVVybD0iIiBSZXBvcnRpbmdOYW1lPSIiIElnbm9yZUh0dHBTdGF0dXNDb2RlPSJGYWxzZSIgLz48L0l0ZW1zPjwvV2ViVGVzdD4='
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIERhdGFiYXNlIENvbm5lY3Rpb24gVGVzdCIgSWQ9Imh0dHA1MDAtZGItdGVzdCIgT3duZXI9IiIgUHJpb3JpdHk9IjAiIEVuYWJsZWQ9IlRydWUiIENzc1Byb2plY3RTdHJ1Y3R1cmU9IiIgQ3NzSXRlcmF0aW9uPSIiIFRpbWVvdXQ9IjMwIiBXb3JrSXRlbUlkcz0iIiB4bWxucz0iaHR0cDovL21pY3Jvc29mdC5jb20vc2NoZW1hcy9WaXN1YWxTdHVkaW8vVGVhbVRlc3QvMjAxMCIgRGVzY3JpcHRpb249IiIgQ3JlZGVudGlhbFVzZXJOYW1lPSIiIENyZWRlbnRpYWxQYXNzd29yZD0iIiBQcmVBdXRoZW50aWNhdGU9IlRydWUiIFByb3h5PSJkZWZhdWx0IiBTdG9wT25FcnJvcj0iRmFsc2UiIFJlY29yZGVkUmVzdWx0RmlsZT0iIiBSZXN1bHRzTG9jYWxlPSIiPjxJdGVtcz48UmVxdWVzdCBNZXRob2Q9IkdFVCIgR3VpZD0iYjVmMTAxMjYtZTRjZC01NzBkLTk2MWMtY2VhNDM5YjljNDg1IiBWZXJzaW9uPSIxLjEiIFVybD0iJHt3ZWJBcHBVcmx9L1NjZW5hcmlvcy9IdHRwNTAwL0h0dHA1MDBfMUFjdHVhbC5hc3B4IiBUaGlua1RpbWU9IjAiIFRpbWVvdXQ9IjMwMCIgUGFyc2VEZXBlbmRlbnRSZXF1ZXN0cz0iRmFsc2UiIEZvbGxvd1JlZGlyZWN0cz0iVHJ1ZSIgUmVjb3JkUmVzdWx0PSJUcnVlIiBDYWNoZT0iRmFsc2UiIFJlc3BvbnNlVGltZUdvYWw9IjAiIEVuY29kaW5nPSJ1dGYtOCIgRXhwZWN0ZWRIdHRwU3RhdHVzQ29kZT0iNTAwIiBFeHBlY3RlZFJlc3BvbnNlVXJsPSIiIFJlcG9ydGluZ05hbWU9IiIgSWdub3JlSHR0cFN0YXR1c0NvZGU9IkZhbHNlIiAvPjwvSXRlbXM+PC9XZWJUZXN0Pg=='
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIEZpbGUgQWNjZXNzIFRlc3QiIElkPSJodHRwNTAwLWZpbGUtdGVzdCIgT3duZXI9IiIgUHJpb3JpdHk9IjAiIEVuYWJsZWQ9IlRydWUiIENzc1Byb2plY3RTdHJ1Y3R1cmU9IiIgQ3NzSXRlcmF0aW9uPSIiIFRpbWVvdXQ9IjMwIiBXb3JrSXRlbUlkcz0iIiB4bWxucz0iaHR0cDovL21pY3Jvc29mdC5jb20vc2NoZW1hcy9WaXN1YWxTdHVkaW8vVGVhbVRlc3QvMjAxMCIgRGVzY3JpcHRpb249IiIgQ3JlZGVudGlhbFVzZXJOYW1lPSIiIENyZWRlbnRpYWxQYXNzd29yZD0iIiBQcmVBdXRoZW50aWNhdGU9IlRydWUiIFByb3h5PSJkZWZhdWx0IiBTdG9wT25FcnJvcj0iRmFsc2UiIFJlY29yZGVkUmVzdWx0RmlsZT0iIiBSZXN1bHRzTG9jYWxlPSIiPjxJdGVtcz48UmVxdWVzdCBNZXRob2Q9IkdFVCIgR3VpZD0iYzVmMTAxMjYtZTRjZC01NzBkLTk2MWMtY2VhNDM5YjljNDg2IiBWZXJzaW9uPSIxLjEiIFVybD0iJHt3ZWJBcHBVcmx9L1NjZW5hcmlvcy9IdHRwNTAwL0h0dHA1MDBfMkFjdHVhbC5hc3B4IiBUaGlua1RpbWU9IjAiIFRpbWVvdXQ9IjMwMCIgUGFyc2VEZXBlbmRlbnRSZXF1ZXN0cz0iRmFsc2UiIEZvbGxvd1JlZGlyZWN0cz0iVHJ1ZSIgUmVjb3JkUmVzdWx0PSJUcnVlIiBDYWNoZT0iRmFsc2UiIFJlc3BvbnNlVGltZUdvYWw9IjAiIEVuY29kaW5nPSJ1dGYtOCIgRXhwZWN0ZWRIdHRwU3RhdHVzQ29kZT0iNTAwIiBFeHBlY3RlZFJlc3BvbnNlVXJsPSIiIFJlcG9ydGluZ05hbWU9IiIgSWdub3JlSHR0cFN0YXR1c0NvZGU9IkZhbHNlIiAvPjwvSXRlbXM+PC9XZWJUZXN0Pg=='
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIENvbmZpZ3VyYXRpb24gVGVzdCIgSWQ9Imh0dHA1MDAtY29uZmlnLXRlc3QiIE93bmVyPSIiIFByaW9yaXR5PSIwIiBFbmFibGVkPSJUcnVlIiBDc3NQcm9qZWN0U3RydWN0dXJlPSIiIENzc0l0ZXJhdGlvbj0iIiBUaW1lb3V0PSIzMCIgV29ya0l0ZW1JZHM9IiIgeG1sbnM9Imh0dHA6Ly9taWNyb3NvZnQuY29tL3NjaGVtYXMvVmlzdWFsU3R1ZGlvL1RlYW1UZXN0LzIwMTAiIERlc2NyaXB0aW9uPSIiIENyZWRlbnRpYWxVc2VyTmFtZT0iIiBDcmVkZW50aWFsUGFzc3dvcmQ9IiIgUHJlQXV0aGVudGljYXRlPSJUcnVlIiBQcm94eT0iZGVmYXVsdCIgU3RvcE9uRXJyb3I9IkZhbHNlIiBSZWNvcmRlZFJlc3VsdEZpbGU9IiIgUmVzdWx0c0xvY2FsZT0iIj48SXRlbXM+PFJlcXVlc3QgTWV0aG9kPSJHRVQiIEd1aWQ9ImQ1ZjEwMTI2LWU0Y2QtNTcwZC05NjFjLWNlYTQzOWI5YzQ4NyIgVmVyc2lvbj0iMS4xIiBVcmw9IiR7d2ViQXBwVXJsfS9TY2VuYXJpb3MvSHR0cDUwMC9IdHRwNTAwXzNBY3R1YWwuYXNweCIgVGhpbmtUaW1lPSIwIiBUaW1lb3V0PSIzMDAiIFBhcnNlRGVwZW5kZW50UmVxdWVzdHM9IkZhbHNlIiBGb2xsb3dSZWRpcmVjdHM9IlRydWUiIFJlY29yZFJlc3VsdD0iVHJ1ZSIgQ2FjaGU9IkZhbHNlIiBSZXNwb25zZVRpbWVHb2FsPSIwIiBFbmNvZGluZz0idXRmLTgiIEV4cGVjdGVkSHR0cFN0YXR1c0NvZGU9IjUwMCIgRXhwZWN0ZWRSZXNwb25zZVVybD0iIiBSZXBvcnRpbmdOYW1lPSIiIElnbm9yZUh0dHBTdGF0dXNDb2RlPSJGYWxzZSIgLz48L0l0ZW1zPjwvV2ViVGVzdD4='
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIENvbm5lY3Rpb24gU3RyaW5nIFRlc3QiIElkPSJodHRwNTAwLWNvbm4tdGVzdCIgT3duZXI9IiIgUHJpb3JpdHk9IjAiIEVuYWJsZWQ9IlRydWUiIENzc1Byb2plY3RTdHJ1Y3R1cmU9IiIgQ3NzSXRlcmF0aW9uPSIiIFRpbWVvdXQ9IjMwIiBXb3JrSXRlbUlkcz0iIiB4bWxucz0iaHR0cDovL21pY3Jvc29mdC5jb20vc2NoZW1hcy9WaXN1YWxTdHVkaW8vVGVhbVRlc3QvMjAxMCIgRGVzY3JpcHRpb249IiIgQ3JlZGVudGlhbFVzZXJOYW1lPSIiIENyZWRlbnRpYWxQYXNzd29yZD0iIiBQcmVBdXRoZW50aWNhdGU9IlRydWUiIFByb3h5PSJkZWZhdWx0IiBTdG9wT25FcnJvcj0iRmFsc2UiIFJlY29yZGVkUmVzdWx0RmlsZT0iIiBSZXN1bHRzTG9jYWxlPSIiPjxJdGVtcz48UmVxdWVzdCBNZXRob2Q9IkdFVCIgR3VpZD0iZTVmMTAxMjYtZTRjZC01NzBkLTk2MWMtY2VhNDM5YjljNDg4IiBWZXJzaW9uPSIxLjEiIFVybD0iJHt3ZWJBcHBVcmx9L1NjZW5hcmlvcy9IdHRwNTAwL0h0dHA1MDBfNEFjdHVhbC5hc3B4IiBUaGlua1RpbWU9IjAiIFRpbWVvdXQ9IjMwMCIgUGFyc2VEZXBlbmRlbnRSZXF1ZXN0cz0iRmFsc2UiIEZvbGxvd1JlZGlyZWN0cz0iVHJ1ZSIgUmVjb3JkUmVzdWx0PSJUcnVlIiBDYWNoZT0iRmFsc2UiIFJlc3BvbnNlVGltZUdvYWw9IjAiIEVuY29kaW5nPSJ1dGYtOCIgRXhwZWN0ZWRIdHRwU3RhdHVzQ29kZT0iNTAwIiBFeHBlY3RlZFJlc3BvbnNlVXJsPSIiIFJlcG9ydGluZ05hbWU9IiIgSWdub3JlSHR0cFN0YXR1c0NvZGU9IkZhbHNlIiAvPjwvSXRlbXM+PC9XZWJUZXN0Pg=='
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIFBhZ2UgMSBUZXN0IiBJZD0iaHR0cDUwMC1wYWdlMS10ZXN0IiBPd25lcj0iIiBQcmlvcml0eT0iMCIgRW5hYmxlZD0iVHJ1ZSIgQ3NzUHJvamVjdFN0cnVjdHVyZT0iIiBDc3NJdGVyYXRpb249IiIgVGltZW91dD0iMzAiIFdvcmtJdGVtSWRzPSIiIHhtbG5zPSJodHRwOi8vbWljcm9zb2Z0LmNvbS9zY2hlbWFzL1Zpc3VhbFN0dWRpby9UZWFtVGVzdC8yMDEwIiBEZXNjcmlwdGlvbj0iIiBDcmVkZW50aWFsVXNlck5hbWU9IiIgQ3JlZGVudGlhbFBhc3N3b3JkPSIiIFByZUF1dGhlbnRpY2F0ZT0iVHJ1ZSIgUHJveHk9ImRlZmF1bHQiIFN0b3BPbkVycm9yPSJGYWxzZSIgUmVjb3JkZWRSZXN1bHRGaWxlPSIiIFJlc3VsdHNMb2NhbGU9IiI+PEl0ZW1zPjxSZXF1ZXN0IE1ldGhvZD0iR0VUIiBHdWlkPSJmNWYxMDEyNi1lNGNkLTU3MGQtOTYxYy1jZWE0MzliOWM0ODkiIFZlcnNpb249IjEuMSIgVXJsPSIke3dlYkFwcFVybH0vU2NlbmFyaW9zL0h0dHA1MDAvSHR0cDUwMF8xLmFzcHgiIFRoaW5rVGltZT0iMCIgVGltZW91dD0iMzAwIiBQYXJzZURlcGVuZGVudFJlcXVlc3RzPSJGYWxzZSIgRm9sbG93UmVkaXJlY3RzPSJUcnVlIiBSZWNvcmRSZXN1bHQ9IlRydWUiIENhY2hlPSJGYWxzZSIgUmVzcG9uc2VUaW1lR29hbD0iMCIgRW5jb2Rpbmc9InV0Zi04IiBFeHBlY3RlZEh0dHBTdGF0dXNDb2RlPSIyMDAiIEV4cGVjdGVkUmVzcG9uc2VVcmw9IiIgUmVwb3J0aW5nTmFtZT0iIiBJZ25vcmVIdHRwU3RhdHVzQ29kZT0iRmFsc2UiIC8+PC9JdGVtcz48L1dlYlRlc3Q+'
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIFBhZ2UgMiBUZXN0IiBJZD0iaHR0cDUwMC1wYWdlMi10ZXN0IiBPd25lcj0iIiBQcmlvcml0eT0iMCIgRW5hYmxlZD0iVHJ1ZSIgQ3NzUHJvamVjdFN0cnVjdHVyZT0iIiBDc3NJdGVyYXRpb249IiIgVGltZW91dD0iMzAiIFdvcmtJdGVtSWRzPSIiIHhtbG5zPSJodHRwOi8vbWljcm9zb2Z0LmNvbS9zY2hlbWFzL1Zpc3VhbFN0dWRpby9UZWFtVGVzdC8yMDEwIiBEZXNjcmlwdGlvbj0iIiBDcmVkZW50aWFsVXNlck5hbWU9IiIgQ3JlZGVudGlhbFBhc3N3b3JkPSIiIFByZUF1dGhlbnRpY2F0ZT0iVHJ1ZSIgUHJveHk9ImRlZmF1bHQiIFN0b3BPbkVycm9yPSJGYWxzZSIgUmVjb3JkZWRSZXN1bHRGaWxlPSIiIFJlc3VsdHNMb2NhbGU9IiI+PEl0ZW1zPjxSZXF1ZXN0IE1ldGhvZD0iR0VUIiBHdWlkPSJnNWYxMDEyNi1lNGNkLTU3MGQtOTYxYy1jZWE0MzliOWM0OTAiIFZlcnNpb249IjEuMSIgVXJsPSIke3dlYkFwcFVybH0vU2NlbmFyaW9zL0h0dHA1MDAvSHR0cDUwMF8yLmFzcHgiIFRoaW5rVGltZT0iMCIgVGltZW91dD0iMzAwIiBQYXJzZURlcGVuZGVudFJlcXVlc3RzPSJGYWxzZSIgRm9sbG93UmVkaXJlY3RzPSJUcnVlIiBSZWNvcmRSZXN1bHQ9IlRydWUiIENhY2hlPSJGYWxzZSIgUmVzcG9uc2VUaW1lR29hbD0iMCIgRW5jb2Rpbmc9InV0Zi04IiBFeHBlY3RlZEh0dHBTdGF0dXNDb2RlPSIyMDAiIEV4cGVjdGVkUmVzcG9uc2VVcmw9IiIgUmVwb3J0aW5nTmFtZT0iIiBJZ25vcmVIdHRwU3RhdHVzQ29kZT0iRmFsc2UiIC8+PC9JdGVtcz48L1dlYlRlc3Q+'
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIFBhZ2UgMyBUZXN0IiBJZD0iaHR0cDUwMC1wYWdlMy10ZXN0IiBPd25lcj0iIiBQcmlvcml0eT0iMCIgRW5hYmxlZD0iVHJ1ZSIgQ3NzUHJvamVjdFN0cnVjdHVyZT0iIiBDc3NJdGVyYXRpb249IiIgVGltZW91dD0iMzAiIFdvcmtJdGVtSWRzPSIiIHhtbG5zPSJodHRwOi8vbWljcm9zb2Z0LmNvbS9zY2hlbWFzL1Zpc3VhbFN0dWRpby9UZWFtVGVzdC8yMDEwIiBEZXNjcmlwdGlvbj0iIiBDcmVkZW50aWFsVXNlck5hbWU9IiIgQ3JlZGVudGlhbFBhc3N3b3JkPSIiIFByZUF1dGhlbnRpY2F0ZT0iVHJ1ZSIgUHJveHk9ImRlZmF1bHQiIFN0b3BPbkVycm9yPSJGYWxzZSIgUmVjb3JkZWRSZXN1bHRGaWxlPSIiIFJlc3VsdHNMb2NhbGU9IiI+PEl0ZW1zPjxSZXF1ZXN0IE1ldGhvZD0iR0VUIiBHdWlkPSJoNWYxMDEyNi1lNGNkLTU3MGQtOTYxYy1jZWE0MzliOWM0OTEiIFZlcnNpb249IjEuMSIgVXJsPSIke3dlYkFwcFVybH0vU2NlbmFyaW9zL0h0dHA1MDAvSHR0cDUwMF8zLmFzcHgiIFRoaW5rVGltZT0iMCIgVGltZW91dD0iMzAwIiBQYXJzZURlcGVuZGVudFJlcXVlc3RzPSJGYWxzZSIgRm9sbG93UmVkaXJlY3RzPSJUcnVlIiBSZWNvcmRSZXN1bHQ9IlRydWUiIENhY2hlPSJGYWxzZSIgUmVzcG9uc2VUaW1lR29hbD0iMCIgRW5jb2Rpbmc9InV0Zi04IiBFeHBlY3RlZEh0dHBTdGF0dXNDb2RlPSIyMDAiIEV4cGVjdGVkUmVzcG9uc2VVcmw9IiIgUmVwb3J0aW5nTmFtZT0iIiBJZ25vcmVIdHRwU3RhdHVzQ29kZT0iRmFsc2UiIC8+PC9JdGVtcz48L1dlYlRlc3Q+'
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
      WebTest: 'PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48V2ViVGVzdCBOYW1lPSJIVFRQNTAwIFBhZ2UgNCBUZXN0IiBJZD0iaHR0cDUwMC1wYWdlNC10ZXN0IiBPd25lcj0iIiBQcmlvcml0eT0iMCIgRW5hYmxlZD0iVHJ1ZSIgQ3NzUHJvamVjdFN0cnVjdHVyZT0iIiBDc3NJdGVyYXRpb249IiIgVGltZW91dD0iMzAiIFdvcmtJdGVtSWRzPSIiIHhtbG5zPSJodHRwOi8vbWljcm9zb2Z0LmNvbS9zY2hlbWFzL1Zpc3VhbFN0dWRpby9UZWFtVGVzdC8yMDEwIiBEZXNjcmlwdGlvbj0iIiBDcmVkZW50aWFsVXNlck5hbWU9IiIgQ3JlZGVudGlhbFBhc3N3b3JkPSIiIFByZUF1dGhlbnRpY2F0ZT0iVHJ1ZSIgUHJveHk9ImRlZmF1bHQiIFN0b3BPbkVycm9yPSJGYWxzZSIgUmVjb3JkZWRSZXN1bHRGaWxlPSIiIFJlc3VsdHNMb2NhbGU9IiI+PEl0ZW1zPjxSZXF1ZXN0IE1ldGhvZD0iR0VUIiBHdWlkPSJpNWYxMDEyNi1lNGNkLTU3MGQtOTYxYy1jZWE0MzliOWM0OTIiIFZlcnNpb249IjEuMSIgVXJsPSIke3dlYkFwcFVybH0vU2NlbmFyaW9zL0h0dHA1MDAvSHR0cDUwMF80LmFzcHgiIFRoaW5rVGltZT0iMCIgVGltZW91dD0iMzAwIiBQYXJzZURlcGVuZGVudFJlcXVlc3RzPSJGYWxzZSIgRm9sbG93UmVkaXJlY3RzPSJUcnVlIiBSZWNvcmRSZXN1bHQ9IlRydWUiIENhY2hlPSJGYWxzZSIgUmVzcG9uc2VUaW1lR29hbD0iMCIgRW5jb2Rpbmc9InV0Zi04IiBFeHBlY3RlZEh0dHBTdGF0dXNDb2RlPSIyMDAiIEV4cGVjdGVkUmVzcG9uc2VVcmw9IiIgUmVwb3J0aW5nTmFtZT0iIiBJZ25vcmVIdHRwU3RhdHVzQ29kZT0iRmFsc2UiIC8+PC9JdGVtcz48L1dlYlRlc3Q+'
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
