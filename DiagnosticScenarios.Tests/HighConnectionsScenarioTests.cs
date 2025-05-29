using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Linq;
using DiagnosticScenarios.Tests.Helpers;
using System.Collections.Generic;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("HighConnections")]
    public class HighConnectionsScenarioTests
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public HighConnectionsScenarioTests()
        {
            if (!ArmMetricsHelper.ShouldRunTests("HighConnections"))
            {
                Assert.Ignore("Skipping HighConnectionsScenarioTests tests. Set RUN_SPECIALIZED_TESTS=HighConnections to run them locally.");
            }
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? 
                throw new Exception("WEBAPP_URL environment variable is not set");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Wait for the app to be ready
            await WaitForAppReady();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task HighConnectionsScenario_ShouldIncreaseConnectionCount()
        {
            TestContext.Progress.WriteLine("Starting HighConnectionsScenario_ShouldIncreaseConnectionCount test");
            TestContext.Progress.WriteLine($"Using base URL: {_baseUrl}");

            // Get initial metrics
            TestContext.Progress.WriteLine("Getting initial metrics...");
            var initialMetrics = await GetMetrics();
            var initialTotalConnections = GetTotalServicePointConnections(initialMetrics);
            TestContext.Progress.WriteLine($"Initial total connection count: {initialTotalConnections}");

            // First get the page with the button
            TestContext.Progress.WriteLine("Accessing initial page (HighConnections1.aspx)...");
            var response = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1.aspx");
            TestContext.Progress.WriteLine($"Initial page response status: {response.StatusCode}");
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to get the initial page");

            // Now trigger the actual scenario
            TestContext.Progress.WriteLine("Triggering high connections scenario (HighConnections1Actual.aspx)...");
            response = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1Actual.aspx");
            TestContext.Progress.WriteLine($"Scenario page response status: {response.StatusCode}");
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger high connections scenario");

            // Wait for connections to be established
            TestContext.Progress.WriteLine("Waiting 10 seconds for connections to be established...");
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Get metrics after scenario
            TestContext.Progress.WriteLine("Getting metrics after scenario...");
            var afterMetrics = await GetMetrics();
            var afterTotalConnections = GetTotalServicePointConnections(afterMetrics);
            TestContext.Progress.WriteLine($"After scenario total connection count: {afterTotalConnections}");

            // Verify connection count increased
            TestContext.Progress.WriteLine($"Comparing connection counts - Initial: {initialTotalConnections}, After: {afterTotalConnections}");
            Assert.That(afterTotalConnections, Is.GreaterThan(initialTotalConnections), 
                $"Connection count should increase. Initial: {initialTotalConnections}, After: {afterTotalConnections}");
            
            TestContext.Progress.WriteLine("Test completed successfully");
        }

        private int GetTotalServicePointConnections(ProcessMetrics metrics)
        {
            return metrics.ServicePointConnections.ServicePoints.Sum(sp => sp.TotalConnections);
        }

        private async Task<ProcessMetrics> GetMetrics()
        {
            TestContext.Progress.WriteLine("Attempting to get process metrics...");
            var response = await _httpClient.GetAsync($"{_baseUrl}/ProcessMetrics.aspx");
            TestContext.Progress.WriteLine($"ProcessMetrics response status: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var error = $"Failed to get process metrics. Status code: {response.StatusCode}";
                TestContext.Progress.WriteLine(error);
                throw new Exception(error);
            }

            var content = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"ProcessMetrics response content: {content}");
            
            try
            {
                var metrics = JsonConvert.DeserializeObject<ProcessMetrics>(content);
                TestContext.Progress.WriteLine($"Successfully deserialized ProcessMetrics");
                return metrics;
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Failed to deserialize ProcessMetrics: {ex.Message}");
                throw;
            }
        }

        private async Task WaitForAppReady()
        {
            const int maxRetries = 10;
            const int delaySeconds = 5;

            TestContext.Progress.WriteLine("Waiting for app to be ready...");
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} of {maxRetries} to connect to {_baseUrl}");
                    var response = await _httpClient.GetAsync(_baseUrl);
                    TestContext.Progress.WriteLine($"Attempt {i + 1} response status: {response.StatusCode}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        TestContext.Progress.WriteLine("App is ready!");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
                }

                if (i < maxRetries - 1)
                {
                    TestContext.Progress.WriteLine($"Waiting {delaySeconds} seconds before next attempt...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            var error = $"App not ready after {maxRetries} attempts";
            TestContext.Progress.WriteLine(error);
            throw new Exception(error);
        }
    }

    public class ProcessMetrics
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string MachineName { get; set; }
        public double CpuTime { get; set; }
        public long PrivateBytes { get; set; }
        public long WorkingSet { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public double ProcessUptimeMinutes { get; set; }
        public ServicePointConnections ServicePointConnections { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ServicePointConnections
    {
        public int ServicePointCount { get; set; }
        public int DefaultConnectionLimit { get; set; }
        public List<ServicePointInfo> ServicePoints { get; set; }
    }

    public class ServicePointInfo
    {
        public string Address { get; set; }
        public int ConnectionLimit { get; set; }
        public int CurrentConnections { get; set; }
        public int ConnectionGroupCount { get; set; }
        public int TotalConnections { get; set; }
    }
} 