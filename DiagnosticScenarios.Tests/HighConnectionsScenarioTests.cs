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
    public class HighConnectionsScenarioTests : ProcessMetricsBase
    {
        protected override string GetTestCategory() => "HighConnections";

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private ProcessMetricsHelper _helper;

        public HighConnectionsScenarioTests()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("HighConnections"))
            {
                Assert.Ignore("Skipping HighConnectionsScenarioTests tests. Set RUN_SPECIALIZED_TESTS=HighConnections to run them locally.");
            }
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? 
                throw new Exception("WEBAPP_URL environment variable is not set");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _helper = new ProcessMetricsHelper(_baseUrl);
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
            _helper?.Dispose();
        }

        [Test]
        public async Task HighConnectionsScenario_ShouldIncreaseConnectionCount()
        {
            TestContext.Progress.WriteLine("Starting HighConnectionsScenario_ShouldIncreaseConnectionCount test");
            TestContext.Progress.WriteLine($"Using base URL: {_baseUrl}");

            // Restart the web app to ensure a clean state
            TestContext.Progress.WriteLine("Restarting web app to ensure clean state...");
            await _helper.RestartWebApp();

            // Get initial metrics
            TestContext.Progress.WriteLine("Getting initial metrics...");
            var initialMetrics = await GetProcessMetrics();
            var initialTotalConnections = GetTotalServicePointConnections(initialMetrics);
            TestContext.Progress.WriteLine($"Initial total connection count: {initialTotalConnections}");

            // First get the page with the button
            TestContext.Progress.WriteLine("Accessing initial page (HighConnections1.aspx)...");
            var response = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1.aspx");
            TestContext.Progress.WriteLine($"Initial page response status: {response.StatusCode}");
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to get the initial page");

            // Make multiple calls to trigger the scenario
            const int numberOfCalls = 5;
            TestContext.Progress.WriteLine($"Making {numberOfCalls} calls to HighConnections1Actual.aspx...");
            
            for (int i = 0; i < numberOfCalls; i++)
            {
                TestContext.Progress.WriteLine($"Making call {i + 1} of {numberOfCalls}...");
                response = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1Actual.aspx");
                TestContext.Progress.WriteLine($"Call {i + 1} response status: {response.StatusCode}");
                Assert.That(response.IsSuccessStatusCode, Is.True, $"Failed to trigger high connections scenario on call {i + 1}");
            }

            // Wait for connections to be established
            TestContext.Progress.WriteLine("Waiting 30 seconds for connections to be established...");
            await Task.Delay(TimeSpan.FromSeconds(30));

            // Get metrics after scenario
            TestContext.Progress.WriteLine("Getting metrics after scenario...");
            var afterMetrics = await GetProcessMetrics();
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