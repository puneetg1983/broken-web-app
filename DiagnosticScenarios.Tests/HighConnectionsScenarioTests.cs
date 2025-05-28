using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;

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
            // Get initial metrics
            var initialMetrics = await GetMetrics();
            var initialConnections = initialMetrics.TcpConnections.TotalConnections;

            // Trigger the high connections scenario
            var response = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1.aspx");
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger high connections scenario");

            // Wait for connections to be established
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Get metrics after scenario
            var afterMetrics = await GetMetrics();
            var afterConnections = afterMetrics.TcpConnections.TotalConnections;

            // Verify connection count increased
            Assert.That(afterConnections, Is.GreaterThan(initialConnections), 
                $"Connection count should increase. Initial: {initialConnections}, After: {afterConnections}");
        }

        private async Task<ProcessMetrics> GetMetrics()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/ProcessMetrics.aspx");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get process metrics. Status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProcessMetrics>(content);
        }

        private async Task WaitForAppReady()
        {
            const int maxRetries = 10;
            const int delaySeconds = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(_baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
                }

                if (i < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            throw new Exception($"App not ready after {maxRetries} attempts");
        }
    }
} 