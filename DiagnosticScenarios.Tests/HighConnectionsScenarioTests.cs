using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("HighConnections")]
    public class HighConnectionsScenarioTests
    {
        private string _baseUrl;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task TestHighConnectionsScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Connections scenario test...");

            // First, verify the main page loads
            var mainPageResponse = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1.aspx");
            Assert.That(mainPageResponse.IsSuccessStatusCode, Is.True, "Main page should load successfully");

            // Get initial metrics
            var initialMetrics = await GetMetrics();
            var initialOutboundConnections = initialMetrics.TcpConnections.Outgoing;

            // Trigger the scenario
            var scenarioResponse = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighConnections/HighConnections1Actual.aspx");
            Assert.That(scenarioResponse.IsSuccessStatusCode, Is.True, "Scenario page should load successfully");

            // Wait for connections to be established
            await Task.Delay(TimeSpan.FromSeconds(30));

            // Get metrics after connections are established
            var finalMetrics = await GetMetrics();
            var finalOutboundConnections = finalMetrics.TcpConnections.Outgoing;

            // Verify the number of outbound connections increased significantly
            Assert.That(finalOutboundConnections, Is.GreaterThan(initialOutboundConnections), 
                "Number of outbound connections should increase");
            Assert.That(finalOutboundConnections, Is.GreaterThan(1000), 
                "Should have more than 1000 outbound connections");

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Initial outbound connections: {initialOutboundConnections}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Final outbound connections: {finalOutboundConnections}");
        }

        private async Task<ProcessMetrics> GetMetrics()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/ProcessMetrics.aspx");
            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ProcessMetrics>(content);
        }

        private class ProcessMetrics
        {
            public TcpConnectionInfo TcpConnections { get; set; }

            public class TcpConnectionInfo
            {
                public int Outgoing { get; set; }
            }
        }
    }
} 