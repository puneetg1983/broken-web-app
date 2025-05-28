using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DiagnosticScenarios.Tests
{
    public class ProcessMetricsHelper : IDisposable
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _armClient;
        private readonly string _subscriptionId;
        private readonly string _resourceGroup;
        private readonly string _appServiceName;
        private const int DEFAULT_RESTART_WAIT_SECONDS = 30;
        private const int DEFAULT_WARMUP_RETRIES = 10;
        private const int DEFAULT_WARMUP_RETRY_DELAY_SECONDS = 10;

        public ProcessMetricsHelper(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);

            // Initialize ARM client for web app restart
            _subscriptionId = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID") ?? 
                throw new Exception("SUBSCRIPTION_ID environment variable is not set");
            _resourceGroup = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME") ?? 
                throw new Exception("RESOURCE_GROUP_NAME environment variable is not set");
            _appServiceName = Environment.GetEnvironmentVariable("APP_SERVICE_NAME") ?? 
                throw new Exception("APP_SERVICE_NAME environment variable is not set");

            _armClient = new HttpClient();
            _armClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _armClient.Timeout = TimeSpan.FromMinutes(2);

            // Get access token and set it in the ARM client
            var accessToken = GetAzureAccessToken().GetAwaiter().GetResult();
            _armClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }

        private async Task<string> GetAzureAccessToken()
        {
            var azPath = FindAzureCliPath();
            if (string.IsNullOrEmpty(azPath))
            {
                throw new Exception("Azure CLI not found. Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows");
            }

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Using Azure CLI from: {azPath}");
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

                    return accessToken;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get access token: {ex.Message}");
            }
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

        public async Task<ProcessMetrics> GetMetrics()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/ProcessMetrics.aspx");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get process metrics. Status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            return new ProcessMetrics
            {
                ProcessId = json["ProcessId"].Value<int>(),
                ProcessName = json["ProcessName"].Value<string>(),
                CpuTime = json["CpuTime"].Value<double>(),
                PrivateBytes = json["PrivateBytes"].Value<long>(),
                WorkingSet = json["WorkingSet"].Value<long>(),
                ThreadCount = json["ThreadCount"].Value<int>(),
                HandleCount = json["HandleCount"].Value<int>(),
                Timestamp = json["Timestamp"].Value<DateTime>()
            };
        }

        public async Task RestartWebApp()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Restarting web app...");
            var restartUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{_resourceGroup}/providers/Microsoft.Web/sites/{_appServiceName}/restart?api-version=2021-02-01";
            var restartResponse = await _armClient.PostAsync(restartUrl, null);
            
            if (!restartResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to restart web app. Status code: {restartResponse.StatusCode}");
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

        public async Task<(ProcessMetrics baseline, ProcessMetrics after)> RunResourceIntensiveScenario(ScenarioInfo scenario)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting {scenario.MetricName} scenario test...");
            
            var baseline = await GetMetrics();
            await RunScenario(scenario);
            var after = await GetMetrics();
            
            LogMetricComparison(scenario.MetricName, baseline, after);
            return (baseline, after);
        }

        private async Task RunScenario(ScenarioInfo scenario)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting the {scenario.MetricName} page with Url {scenario.Path}...");
            
            const int maxRetries = 3;
            const int initialDelayMs = 5000;
            
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var getResponse = await _httpClient.GetAsync($"{_baseUrl}{scenario.Path}");
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to get {scenario.MetricName} page with Url {_baseUrl}{scenario.Path}");
                    }
                    break; // Success, exit retry loop
                }
                catch (TaskCanceledException) when (retry < maxRetries - 1)
                {
                    var delayMs = initialDelayMs * (int)Math.Pow(2, retry);
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Request timed out, retrying in {delayMs}ms (attempt {retry + 1} of {maxRetries})...");
                    await Task.Delay(delayMs);
                }
            }
            
            for (int i = 0; i < scenario.Iterations; i++)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Running {scenario.MetricName} scenario with Url {_baseUrl}{scenario.Path} iteration {i + 1} of {scenario.Iterations}");
                
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync($"{_baseUrl}{scenario.Path}");
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to trigger {scenario.MetricName} scenario with Url {_baseUrl}{scenario.Path}");
                        }
                        break; // Success, exit retry loop
                    }
                    catch (TaskCanceledException) when (retry < maxRetries - 1)
                    {
                        var delayMs = initialDelayMs * (int)Math.Pow(2, retry);
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Url {_baseUrl}{scenario.Path} Request timed out, retrying in {delayMs}ms (attempt {retry + 1} of {maxRetries})...");
                        await Task.Delay(delayMs);
                    }
                }
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {scenario.DelayBetweenIterationsSeconds} seconds to trigger {scenario.MetricName} scenario with Url {_baseUrl}{scenario.Path} before next iteration...");
                await Task.Delay(TimeSpan.FromSeconds(scenario.DelayBetweenIterationsSeconds));
            }
        }

        private void LogMetricComparison(string metricName, ProcessMetrics baseline, ProcessMetrics after)
        {
            if (metricName == "CpuTime")
            {
                var increaseSeconds = after.CpuTime - baseline.CpuTime;
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} after scenario: {after.CpuTime:F2} seconds");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} increase: {increaseSeconds:F2} seconds");
            }
            else if (metricName == "PrivateBytes")
            {
                var afterMB = after.PrivateBytes / (1024.0 * 1024.0);
                var baselineMB = baseline.PrivateBytes / (1024.0 * 1024.0);
                var increaseMB = afterMB - baselineMB;
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} after scenario: {afterMB:F2}MB");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] {metricName} increase: {increaseMB:F2}MB");
            }
        }

        public void VerifyMetricIncrease(ProcessMetrics after, ProcessMetrics baseline, string metricType)
        {
            if (metricType == "CPU")
            {
                // For CPU Time, we expect an increase in seconds
                if (after.CpuTime <= baseline.CpuTime)
                {
                    throw new Exception($"{metricType} time should increase after running the high {metricType.ToLower()} scenario");
                }
                var increaseSeconds = after.CpuTime - baseline.CpuTime;
                if (increaseSeconds <= 5) // Expect at least 5 seconds increase
                {
                    throw new Exception($"{metricType} time should increase by at least 5 seconds");
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High {metricType} scenario test completed successfully. CPU Time increase: {increaseSeconds:F2} seconds");
            }
            else if (metricType == "Memory")
            {
                // For Private Bytes, we expect an increase in bytes
                if (after.PrivateBytes <= baseline.PrivateBytes)
                {
                    throw new Exception($"{metricType} usage should increase after running the high {metricType.ToLower()} scenario");
                }
                var increaseBytes = after.PrivateBytes - baseline.PrivateBytes;
                var increaseMB = increaseBytes / (1024.0 * 1024.0);
                if (increaseMB <= 10) // Expect at least 10MB increase
                {
                    throw new Exception($"{metricType} usage should increase by at least 10MB");
                }
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High {metricType} scenario test completed successfully. Memory increase: {increaseMB:F2}MB");
            }
        }

        public async Task<HttpResponseMessage> TriggerScenarioWithResponse(string path)
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering scenario at path: {path}");
            var response = await _httpClient.GetAsync($"{_baseUrl}{path}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response status code: {response.StatusCode}");
            return response;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _armClient?.Dispose();
        }
    }

    public class ProcessMetrics
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public double CpuTime { get; set; }
        public long PrivateBytes { get; set; }
        public long WorkingSet { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 