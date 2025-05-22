using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    public class ArmMetricsHelper : IDisposable
    {
        private const int DEFAULT_METRICS_WAIT_SECONDS = 30;
        private const int DEFAULT_RESTART_WAIT_SECONDS = 30;
        private const int DEFAULT_WARMUP_RETRIES = 10;
        private const int DEFAULT_WARMUP_RETRY_DELAY_SECONDS = 10;
        private const int MINIMUM_METRIC_INCREASE_PERCENTAGE = 20;

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _armClient;
        private readonly string _subscriptionId;
        private readonly string _resourceGroup;
        private readonly string _appServiceName;
        private readonly string _appServicePlanName;

        public static async Task<ArmMetricsHelper> CreateAsync(string baseUrl, string subscriptionId, string resourceGroup, string appServiceName)
        {
            var azPath = FindAzureCliPath();
            if (string.IsNullOrEmpty(azPath))
            {
                throw new Exception("Azure CLI not found. Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows");
            }

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Using Azure CLI from: {azPath}");
            var accessToken = await GetAzureAccessToken(azPath);
            var appServicePlanName = await GetAppServicePlanName(subscriptionId, resourceGroup, appServiceName, accessToken);
            
            return new ArmMetricsHelper(baseUrl, subscriptionId, resourceGroup, appServiceName, appServicePlanName, accessToken);
        }

        private ArmMetricsHelper(string baseUrl, string subscriptionId, string resourceGroup, string appServiceName, string appServicePlanName, string accessToken)
        {
            _baseUrl = baseUrl;
            _subscriptionId = subscriptionId;
            _resourceGroup = resourceGroup;
            _appServiceName = appServiceName;
            _appServicePlanName = appServicePlanName;

            _httpClient = CreateHttpClient();
            _armClient = CreateArmClient(accessToken);
        }

        public static bool ShouldRunTests()
        {
            return Environment.GetEnvironmentVariable("RUN_ARM_METRICS_TESTS_LOCALLY")?.ToLower() == "true";
        }

        private static string FindAzureCliPath()
        {
            string[] possibleAzPaths = new[]
            {
                @"C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin\az.cmd",
                @"C:\Program Files (x86)\Microsoft SDKs\Azure\CLI2\wbin\az.cmd",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Python\Python*\Scripts\az.cmd"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Python\Python*\Scripts\az.exe")
            };

            return possibleAzPaths.FirstOrDefault(File.Exists);
        }

        private static async Task<string> GetAzureAccessToken(string azPath)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting Azure access token...");
            var psi = new ProcessStartInfo
            {
                FileName = azPath,
                Arguments = "account get-access-token --query accessToken -o tsv",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        throw new Exception("Failed to start Azure CLI process");
                    }

                    var accessToken = (await process.StandardOutput.ReadToEndAsync()).Trim();
                    process.WaitForExit();
                    
                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"Azure CLI process exited with code {process.ExitCode}");
                    }

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new Exception("Failed to get access token from Azure CLI");
                    }
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Successfully obtained Azure access token");
                    return accessToken;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get access token: {ex.Message}");
            }
        }

        private static async Task<string> GetAppServicePlanName(string subscriptionId, string resourceGroup, string appServiceName, string accessToken)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting app service plan name...");
            var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.Web/sites/{appServiceName}?api-version=2021-02-01";
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get app service details. Status code: {response.StatusCode}");
                }

                var json = JObject.Parse(content);
                var serverFarmId = json["properties"]?["serverFarmId"]?.Value<string>();
                
                if (string.IsNullOrEmpty(serverFarmId))
                {
                    throw new Exception("Could not find serverFarmId in app service details");
                }

                var appServicePlanName = serverFarmId.Split('/').Last();
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Found app service plan: {appServicePlanName}");
                return appServicePlanName;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            client.Timeout = TimeSpan.FromMinutes(2);
            return client;
        }

        private HttpClient CreateArmClient(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.Timeout = TimeSpan.FromMinutes(2);
            return client;
        }

        public async Task EnsureAppIsRunning()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Making warmup request to ensure app is running...");
            int maxRetries = 5;
            int retryDelaySeconds = 50;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                if (await TryWarmupRequest(attempt, maxRetries))
                {
                    return;
                }
                
                if (attempt < maxRetries)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {retryDelaySeconds} seconds before next warmup attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }
        }

        private async Task<bool> TryWarmupRequest(int attempt, int maxRetries)
        {
            try
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warmup attempt {attempt} of {maxRetries}");
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warmup request succeeded with status code {(int)response.StatusCode} {response.StatusCode}");
                    return true;
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warmup request returned status code {(int)response.StatusCode} {response.StatusCode}");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warmup request failed: {ex.GetType().Name}: {ex.Message}");
            }

            if (attempt == maxRetries)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] WARNING: App may not be ready after {maxRetries} attempts. Tests may fail.");
            }
            return false;
        }

        public async Task RestartWebApp()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Restarting web app...");
            var restartUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroup}/providers/Microsoft.Web/sites/{_appServiceName}/restart?api-version=2021-02-01";
            var restartResponse = await _armClient.PostAsync(restartUrl, null);
            
            if (!restartResponse.IsSuccessStatusCode)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warning: Failed to restart web app. Status code: {restartResponse.StatusCode}");
                return;
            }

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Web app restart initiated successfully");
            await WaitForAppRestart();
        }

        private async Task WaitForAppRestart()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting for web app to restart and warm up...");
            await Task.Delay(TimeSpan.FromSeconds(DEFAULT_RESTART_WAIT_SECONDS));
            
            for (int attempt = 1; attempt <= DEFAULT_WARMUP_RETRIES; attempt++)
            {
                if (await TryWarmupRequest(attempt, DEFAULT_WARMUP_RETRIES))
                {
                    return;
                }
                
                if (attempt < DEFAULT_WARMUP_RETRIES)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {DEFAULT_WARMUP_RETRY_DELAY_SECONDS} seconds before next warmup attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(DEFAULT_WARMUP_RETRY_DELAY_SECONDS));
                }
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warning: Web app may not be fully ready after restart");
        }

        public async Task<HttpResponseMessage> TriggerScenarioWithResponse(string scenarioPath)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering scenario: {scenarioPath}");
            var response = await _httpClient.GetAsync($"{_baseUrl}{scenarioPath}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Scenario response status code: {(int)response.StatusCode} {response.StatusCode}");
            return response;
        }

        public async Task<double?> GetInstanceMetricValue(string metricName, string aggregation = "Average", int intervalMinutes = 5)
        {
            const int maxRetries = 5;
            const int retryDelayMinutes = 1;
            const int expectedRecords = 5; // We expect 5 minutes worth of data
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                var (startTime, endTime) = GetMetricTimeRange(intervalMinutes);
                var url = BuildMetricsUrl(metricName, aggregation, startTime, endTime);
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Fetching metrics for {metricName} (Attempt {attempt}/{maxRetries})...");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Time range: {startTime:yyyy-MM-ddTHH:mm:ssZ} to {endTime:yyyy-MM-ddTHH:mm:ssZ}");
                
                try
                {
                    var response = await _armClient.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warning: Failed to get metrics. Status code: {response.StatusCode}");
                        if (attempt < maxRetries)
                        {
                            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Retrying in {retryDelayMinutes} minute...");
                            await Task.Delay(TimeSpan.FromMinutes(retryDelayMinutes));
                            continue;
                        }
                        return null;
                    }

                    var json = JObject.Parse(content);
                    var timeseries = json["value"]?[0]?["timeseries"]?[0]?["data"];
                    
                    if (timeseries == null || !timeseries.HasValues)
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warning: No metric data available for {metricName}");
                        if (attempt < maxRetries)
                        {
                            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Retrying in {retryDelayMinutes} minute...");
                            await Task.Delay(TimeSpan.FromMinutes(retryDelayMinutes));
                            continue;
                        }
                        return null;
                    }

                    // Print metrics in a table format
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Metric values for {metricName}:");
                    TestContext.Progress.WriteLine("┌─────────────────────┬────────────┐");
                    TestContext.Progress.WriteLine("│ Timestamp           │ Value      │");
                    TestContext.Progress.WriteLine("├─────────────────────┼────────────┤");
                    
                    var validDataPoints = new List<(string timestamp, double value)>();
                    foreach (var dataPoint in timeseries)
                    {
                        var timestamp = dataPoint["timeStamp"]?.Value<string>();
                        var average = dataPoint["average"]?.Value<double>();
                        if (average.HasValue)
                        {
                            validDataPoints.Add((timestamp, average.Value));
                            TestContext.Progress.WriteLine($"│ {timestamp,-19} │ {average.Value,10:F2} │");
                        }
                    }
                    TestContext.Progress.WriteLine("└─────────────────────┴────────────┘");

                    // Check if we have all expected records
                    if (validDataPoints.Count < expectedRecords)
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Warning: Only got {validDataPoints.Count} records, expected {expectedRecords}. Retrying...");
                        if (attempt < maxRetries)
                        {
                            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Retrying in {retryDelayMinutes} minute...");
                            await Task.Delay(TimeSpan.FromMinutes(retryDelayMinutes));
                            continue;
                        }
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Failed to get all {expectedRecords} records after {maxRetries} attempts");
                        return null;
                    }

                    // Get the average of the last 3 values
                    var lastThreeValues = validDataPoints.Skip(Math.Max(0, validDataPoints.Count - 3)).Select(x => x.value).ToList();
                    var averageValue = lastThreeValues.Average();
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Retrieved {metricName} value (average of last 3): {averageValue:F2}");
                    return averageValue;
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Error getting metrics: {ex.Message}");
                    if (attempt < maxRetries)
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Retrying in {retryDelayMinutes} minute...");
                        await Task.Delay(TimeSpan.FromMinutes(retryDelayMinutes));
                        continue;
                    }
                    return null;
                }
            }

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Failed to get {metricName} metrics after {maxRetries} attempts");
            return null;
        }

        private (DateTime startTime, DateTime endTime) GetMetricTimeRange(int intervalMinutes)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddMinutes(-intervalMinutes);
            return (startTime, endTime);
        }

        private string BuildMetricsUrl(string metricName, string aggregation, DateTime startTime, DateTime endTime)
        {
            return $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroup}/providers/Microsoft.Web/sites/{_appServiceName}/providers/Microsoft.Insights/metrics" +
                   $"?api-version=2021-05-01" +
                   $"&metricnames={metricName}" +
                   $"&aggregation={aggregation}" +
                   $"&interval=PT1M" +
                   $"&timespan={startTime:yyyy-MM-ddTHH:mm:ssZ}/{endTime:yyyy-MM-ddTHH:mm:ssZ}";
        }

        public async Task<(double baseline, double after)> RunResourceIntensiveScenario(ScenarioInfo scenario)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting {scenario.MetricName} scenario test...");
            
            var baseline = await GetBaselineMetrics(scenario.MetricName);
            await RunScenario(scenario);
            var after = await GetAfterMetrics(scenario.MetricName);
            
            LogMetricComparison(scenario.MetricName, baseline, after);
            return (baseline, after);
        }

        private async Task<double> GetBaselineMetrics(string metricName)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting baseline {metricName} metrics...");
            var baseline = await GetInstanceMetricValue(metricName);
            if (!baseline.HasValue)
            {
                throw new Exception($"Failed to get baseline {metricName} metrics");
            }
            if (metricName == "CpuTime")
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline {metricName}: {baseline.Value:F2} seconds");
            }
            else if (metricName == "PrivateBytes")
            {
                var baselineMB = baseline.Value / (1024 * 1024);
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline {metricName}: {baselineMB:F2}MB");
            }
            return baseline.Value;
        }

        private async Task RunScenario(ScenarioInfo scenario)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting the {scenario.MetricName} page...");
            var getResponse = await TriggerScenarioWithResponse(scenario.Path);
            if (!getResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get {scenario.MetricName} page");
            }
            
            for (int i = 0; i < scenario.Iterations; i++)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Running {scenario.MetricName} scenario iteration {i + 1} of {scenario.Iterations}");
                var response = await TriggerScenarioWithResponse(scenario.Path);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to trigger {scenario.MetricName} scenario");
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {scenario.DelayBetweenIterationsSeconds} seconds before next iteration...");
                await Task.Delay(TimeSpan.FromSeconds(scenario.DelayBetweenIterationsSeconds));
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {scenario.WaitForMetricsMinutes} minute for {scenario.MetricName} metrics to be collected...");
            await Task.Delay(TimeSpan.FromMinutes(scenario.WaitForMetricsMinutes));
        }

        private async Task<double> GetAfterMetrics(string metricName)
        {
            var after = await GetInstanceMetricValue(metricName);
            if (!after.HasValue)
            {
                throw new Exception($"Failed to get {metricName} metrics after scenario");
            }
            return after.Value;
        }

        private void LogMetricComparison(string metricName, double baseline, double after)
        {
            if (metricName == "CpuTime")
            {
                var increaseSeconds = after - baseline;
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} after scenario: {after:F2} seconds");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} increase: {increaseSeconds:F2} seconds");
            }
            else if (metricName == "PrivateBytes")
            {
                var afterMB = after / (1024 * 1024);
                var baselineMB = baseline / (1024 * 1024);
                var increaseMB = afterMB - baselineMB;
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} after scenario: {afterMB:F2}MB");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} increase: {increaseMB:F2}MB");
            }
        }

        public void VerifyMetricIncrease(double after, double baseline, string metricType)
        {
            if (metricType == "CPU")
            {
                // For CPU Time, we expect an increase in seconds
                if (after <= baseline)
                {
                    throw new Exception($"{metricType} time should increase after running the high {metricType.ToLower()} scenario");
                }
                var increaseSeconds = after - baseline;
                if (increaseSeconds <= 5) // Expect at least 5 seconds increase
                {
                    throw new Exception($"{metricType} time should increase by at least 5 seconds");
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High {metricType} scenario test completed successfully. CPU Time increase: {increaseSeconds:F2} seconds");
            }
            else if (metricType == "Memory")
            {
                // For Private Bytes, we expect an increase in bytes
                if (after <= baseline)
                {
                    throw new Exception($"{metricType} usage should increase after running the high {metricType.ToLower()} scenario");
                }
                var increaseBytes = after - baseline;
                var increaseMB = increaseBytes / (1024 * 1024);
                if (increaseMB <= 10) // Expect at least 10MB increase
                {
                    throw new Exception($"{metricType} usage should increase by at least 10MB");
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High {metricType} scenario test completed successfully. Memory increase: {increaseMB:F2}MB");
            }
        }

        public void VerifyHttp500Response(HttpResponseMessage response, string content)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception($"Expected HTTP 500 status code, but got {(int)response.StatusCode} {response.StatusCode}");
            }
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] HTTP 500 scenario test completed successfully");
        }

        public void VerifySlowResponse(double responseTimeMs)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            if (responseTimeMs <= 1000)
            {
                throw new Exception($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Slow Response scenario test completed successfully");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _armClient?.Dispose();
        }
    }
} 