using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            Console.WriteLine($"[{DateTime.Now}] Using Azure CLI from: {azPath}");
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
            Console.WriteLine($"[{DateTime.Now}] Getting Azure access token...");
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
                    Console.WriteLine($"[{DateTime.Now}] Successfully obtained Azure access token");
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
            Console.WriteLine($"[{DateTime.Now}] Getting app service plan name...");
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
                Console.WriteLine($"[{DateTime.Now}] Found app service plan: {appServicePlanName}");
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
            Console.WriteLine($"[{DateTime.Now}] Making warmup request to ensure app is running...");
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
                    Console.WriteLine($"[{DateTime.Now}] Waiting {retryDelaySeconds} seconds before next warmup attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }
        }

        private async Task<bool> TryWarmupRequest(int attempt, int maxRetries)
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}] Warmup attempt {attempt} of {maxRetries}");
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[{DateTime.Now}] Warmup request succeeded with status code {(int)response.StatusCode} {response.StatusCode}");
                    return true;
                }
                Console.WriteLine($"[{DateTime.Now}] Warmup request returned status code {(int)response.StatusCode} {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Warmup request failed: {ex.GetType().Name}: {ex.Message}");
            }

            if (attempt == maxRetries)
            {
                Console.WriteLine($"[{DateTime.Now}] WARNING: App may not be ready after {maxRetries} attempts. Tests may fail.");
            }
            return false;
        }

        public async Task RestartWebApp()
        {
            Console.WriteLine($"[{DateTime.Now}] Restarting web app...");
            var restartUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroup}/providers/Microsoft.Web/sites/{_appServiceName}/restart?api-version=2021-02-01";
            var restartResponse = await _armClient.PostAsync(restartUrl, null);
            
            if (!restartResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"[{DateTime.Now}] Warning: Failed to restart web app. Status code: {restartResponse.StatusCode}");
                return;
            }

            Console.WriteLine($"[{DateTime.Now}] Web app restart initiated successfully");
            await WaitForAppRestart();
        }

        private async Task WaitForAppRestart()
        {
            Console.WriteLine($"[{DateTime.Now}] Waiting for web app to restart and warm up...");
            await Task.Delay(TimeSpan.FromSeconds(DEFAULT_RESTART_WAIT_SECONDS));
            
            for (int attempt = 1; attempt <= DEFAULT_WARMUP_RETRIES; attempt++)
            {
                if (await TryWarmupRequest(attempt, DEFAULT_WARMUP_RETRIES))
                {
                    return;
                }
                
                if (attempt < DEFAULT_WARMUP_RETRIES)
                {
                    Console.WriteLine($"[{DateTime.Now}] Waiting {DEFAULT_WARMUP_RETRY_DELAY_SECONDS} seconds before next warmup attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(DEFAULT_WARMUP_RETRY_DELAY_SECONDS));
                }
            }
            
            Console.WriteLine($"[{DateTime.Now}] Warning: Web app may not be fully ready after restart");
        }

        public async Task<HttpResponseMessage> TriggerScenarioWithResponse(string scenarioPath, bool isPost = false, Dictionary<string, string> formData = null)
        {
            Console.WriteLine($"[{DateTime.Now}] Triggering scenario: {scenarioPath}");
            HttpResponseMessage response;
            
            if (isPost && formData != null)
            {
                var content = new FormUrlEncodedContent(formData);
                response = await _httpClient.PostAsync($"{_baseUrl}{scenarioPath}", content);
            }
            else
            {
                response = await _httpClient.GetAsync($"{_baseUrl}{scenarioPath}");
            }
            
            Console.WriteLine($"[{DateTime.Now}] Scenario response status code: {(int)response.StatusCode} {response.StatusCode}");
            return response;
        }

        public async Task<double?> GetInstanceMetricValue(string metricName, string aggregation = "Average", int intervalMinutes = 5)
        {
            var (startTime, endTime) = GetMetricTimeRange(intervalMinutes);
            var url = BuildMetricsUrl(metricName, aggregation, startTime, endTime);
            
            Console.WriteLine($"[{DateTime.Now}] Fetching metrics for {metricName}...");
            Console.WriteLine($"[{DateTime.Now}] Time range: {startTime:yyyy-MM-ddTHH:mm:ssZ} to {endTime:yyyy-MM-ddTHH:mm:ssZ}");
            
            try
            {
                var response = await _armClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[{DateTime.Now}] Metrics response: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[{DateTime.Now}] Warning: Failed to get metrics. Status code: {response.StatusCode}");
                    return null;
                }

                return ExtractMetricValue(content, metricName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Error getting metrics: {ex.Message}");
                return null;
            }
        }

        private (DateTime startTime, DateTime endTime) GetMetricTimeRange(int intervalMinutes)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddMinutes(-intervalMinutes);
            return (startTime, endTime);
        }

        private string BuildMetricsUrl(string metricName, string aggregation, DateTime startTime, DateTime endTime)
        {
            return $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroup}/providers/Microsoft.Web/serverfarms/{_appServicePlanName}/providers/Microsoft.Insights/metrics" +
                   $"?api-version=2021-05-01" +
                   $"&metricnames={metricName}" +
                   $"&aggregation={aggregation}" +
                   $"&interval=PT1M" +
                   $"&timespan={startTime:yyyy-MM-ddTHH:mm:ssZ}/{endTime:yyyy-MM-ddTHH:mm:ssZ}";
        }

        private double? ExtractMetricValue(string content, string metricName)
        {
            var json = JObject.Parse(content);
            var timeseries = json["value"]?[0]?["timeseries"]?[0]?["data"];
            
            if (timeseries == null || !timeseries.HasValues)
            {
                Console.WriteLine($"[{DateTime.Now}] Warning: No metric data available for {metricName}");
                return null;
            }

            foreach (var dataPoint in timeseries)
            {
                var average = dataPoint["average"]?.Value<double>();
                if (average.HasValue)
                {
                    Console.WriteLine($"[{DateTime.Now}] Retrieved {metricName} value: {average.Value}");
                    return average.Value;
                }
            }

            Console.WriteLine($"[{DateTime.Now}] Warning: No valid average values found for {metricName}");
            return null;
        }

        public async Task<(double baseline, double after)> RunResourceIntensiveScenario(ScenarioInfo scenario)
        {
            Console.WriteLine($"[{DateTime.Now}] Starting {scenario.MetricName} scenario test...");
            
            var baseline = await GetBaselineMetrics(scenario.MetricName);
            await RunScenario(scenario);
            var after = await GetAfterMetrics(scenario.MetricName);
            
            LogMetricComparison(scenario.MetricName, baseline, after);
            return (baseline, after);
        }

        private async Task<double> GetBaselineMetrics(string metricName)
        {
            Console.WriteLine($"[{DateTime.Now}] Getting baseline {metricName} metrics...");
            var baseline = await GetInstanceMetricValue(metricName);
            if (!baseline.HasValue)
            {
                throw new Exception($"Failed to get baseline {metricName} metrics");
            }
            Console.WriteLine($"[{DateTime.Now}] Baseline {metricName} Percentage: {baseline.Value}%");
            return baseline.Value;
        }

        private async Task RunScenario(ScenarioInfo scenario)
        {
            Console.WriteLine($"[{DateTime.Now}] Getting the {scenario.MetricName} page...");
            var getResponse = await TriggerScenarioWithResponse(scenario.Path);
            if (!getResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get {scenario.MetricName} page");
            }
            
            var content = await getResponse.Content.ReadAsStringAsync();
            var formData = ExtractFormData(content, scenario.ButtonId, scenario.ButtonText);
            
            for (int i = 0; i < scenario.Iterations; i++)
            {
                Console.WriteLine($"[{DateTime.Now}] Running {scenario.MetricName} scenario iteration {i + 1} of {scenario.Iterations}");
                var response = await TriggerScenarioWithResponse(scenario.Path, true, formData);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to trigger {scenario.MetricName} scenario");
                }
                await Task.Delay(TimeSpan.FromSeconds(scenario.DelayBetweenIterationsSeconds));
            }
            
            Console.WriteLine($"[{DateTime.Now}] Waiting for {scenario.MetricName} metrics to be collected...");
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
            Console.WriteLine($"[{DateTime.Now}] {metricName} Percentage after scenario: {after}%");
            Console.WriteLine($"[{DateTime.Now}] {metricName} Percentage increase: {after - baseline}%");
        }

        private Dictionary<string, string> ExtractFormData(string content, string buttonId, string buttonText)
        {
            return new Dictionary<string, string>
            {
                { "__VIEWSTATE", ExtractField(content, "__VIEWSTATE") },
                { "__VIEWSTATEGENERATOR", ExtractField(content, "__VIEWSTATEGENERATOR") },
                { "__EVENTVALIDATION", ExtractField(content, "__EVENTVALIDATION") },
                { buttonId, buttonText }
            };
        }

        private string ExtractField(string content, string fieldId)
        {
            var match = Regex.Match(content, $@"id=""{fieldId}"" value=""([^""]+)""");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public void VerifyMetricIncrease(double after, double baseline, string metricType)
        {
            if (after <= baseline)
            {
                throw new Exception($"{metricType} usage should increase after running the high {metricType.ToLower()} scenario");
            }
            if (after - baseline <= MINIMUM_METRIC_INCREASE_PERCENTAGE)
            {
                throw new Exception($"{metricType} usage should increase by at least {MINIMUM_METRIC_INCREASE_PERCENTAGE}%");
            }
            Console.WriteLine($"[{DateTime.Now}] High {metricType} scenario test completed successfully");
        }

        public void VerifyHttp500Response(HttpResponseMessage response, string content)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception($"Expected HTTP 500 status code, but got {(int)response.StatusCode} {response.StatusCode}");
            }
            Console.WriteLine($"[{DateTime.Now}] HTTP 500 scenario test completed successfully");
        }

        public void VerifySlowResponse(double responseTimeDays)
        {
            var responseTimeMs = responseTimeDays * 24 * 60 * 60 * 1000;
            Console.WriteLine($"[{DateTime.Now}] Response Time: {responseTimeMs}ms (raw value: {responseTimeDays} days)");
            if (responseTimeMs <= 1000)
            {
                throw new Exception("Response time should be above 1000ms");
            }
            Console.WriteLine($"[{DateTime.Now}] Slow Response scenario test completed successfully");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _armClient?.Dispose();
        }
    }
} 