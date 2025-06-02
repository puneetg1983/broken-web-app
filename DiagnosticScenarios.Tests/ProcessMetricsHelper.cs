using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DiagnosticScenarios.Tests
{
    public class ProcessMetricsHelper : IDisposable
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private const int DEFAULT_RESTART_WAIT_SECONDS = 30;
        private const int DEFAULT_WARMUP_RETRIES = 10;
        private const int DEFAULT_WARMUP_RETRY_DELAY_SECONDS = 10;

        public ProcessMetricsHelper(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        public static bool ShouldRunTests(string category)
        {
            var runTests = Environment.GetEnvironmentVariable("RUN_SPECIALIZED_TESTS");
            if (string.IsNullOrEmpty(runTests))
            {
                return false;
            }

            return runTests.Equals(category, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ProcessMetrics> GetMetrics()
        {
            const int maxRetries = 5;
            const int retryDelaySeconds = 10;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting process metrics (attempt {attempt} of {maxRetries})...");
                    var response = await _httpClient.GetAsync($"{_baseUrl}/ProcessMetrics.aspx");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ProcessMetrics>(content);
                    }
                    
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Failed to get process metrics. Status code: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Error getting process metrics: {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {retryDelaySeconds} seconds before next attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                }
            }

            throw new Exception($"Failed to get process metrics after {maxRetries} attempts");
        }

        public async Task RestartWebApp()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Restarting web app...");
            var response = await _httpClient.GetAsync($"{_baseUrl}/RestartWebApp.aspx");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to restart web app. Status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);
            
            if (result.status.ToString() != "success")
            {
                throw new Exception($"Failed to restart web app: {result.message}");
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
            var fullUrl = $"{_baseUrl}{scenario.Path}";
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting the {scenario.MetricName} page with Url {fullUrl}...");
            
            const int maxRetries = 3;
            const int initialDelayMs = 5000;
            
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    var getResponse = await _httpClient.GetAsync(fullUrl);
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to get {scenario.MetricName} page with Url {fullUrl}");
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
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Running {scenario.MetricName} scenario with Url {fullUrl} iteration {i + 1} of {scenario.Iterations}");
                
                for (int retry = 0; retry < maxRetries; retry++)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(fullUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to trigger {scenario.MetricName} scenario with Url {fullUrl}");
                        }
                        break; // Success, exit retry loop
                    }
                    catch (TaskCanceledException) when (retry < maxRetries - 1)
                    {
                        var delayMs = initialDelayMs * (int)Math.Pow(2, retry);
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Url {fullUrl} Request timed out, retrying in {delayMs}ms (attempt {retry + 1} of {maxRetries})...");
                        await Task.Delay(delayMs);
                    }
                }
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {scenario.DelayBetweenIterationsSeconds} seconds to trigger {scenario.MetricName} scenario with Url {fullUrl} before next iteration...");
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

        public void VerifyMetricIncrease(ProcessMetrics after, ProcessMetrics baseline, ScenarioInfo scenarioInfo)
        {
            string metricType = scenarioInfo.MetricName;

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

        public void VerifyHttp500Response(HttpResponseMessage response, string content)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception($"Expected HTTP 500 status code, but got {(int)response.StatusCode} {response.StatusCode}");
            }
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] HTTP 500 scenario test completed successfully");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
} 