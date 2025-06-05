using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using DiagnosticScenarios.Tests.Helpers;

namespace DiagnosticScenarios.Tests
{
    public abstract class ProcessMetricsBase
    {
        protected readonly string _processMetricsBaseUrl;
        protected readonly HttpClient _httpClientProcessMetrics;
        protected readonly ProcessMetricsHelper _baseHelper;

        protected ProcessMetricsBase()
        {
            if (!ProcessMetricsHelper.ShouldRunTests(GetTestCategory()))
            {
                Assert.Ignore($"Skipping {GetTestCategory()} tests. Set RUN_SPECIALIZED_TESTS={GetTestCategory()} to run them locally.");
            }

            _processMetricsBaseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _httpClientProcessMetrics = new HttpClient();
            _httpClientProcessMetrics.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _baseHelper = new ProcessMetricsHelper(_processMetricsBaseUrl);
        }

        protected abstract string GetTestCategory();

        [OneTimeTearDown]
        public void ProcessMetricsBaseCleanup()
        {
            _httpClientProcessMetrics?.Dispose();
            _baseHelper?.Dispose();
        }

        protected async Task<ProcessMetrics> GetProcessMetrics()
        {
            const int maxRetries = 5;
            const int retryDelaySeconds = 10;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting process metrics (attempt {attempt} of {maxRetries})...");
                    var response = await _httpClientProcessMetrics.GetAsync($"{_processMetricsBaseUrl}/ProcessMetrics.aspx");
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

        protected async Task EnsureFreshProcess()
        {
            await _baseHelper.RestartWebApp();
            await Task.Delay(TimeSpan.FromSeconds(10)); // Wait for app to stabilize
        }
    }
} 