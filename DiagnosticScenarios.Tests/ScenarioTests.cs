using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    public class ScenarioTests
    {
        private string _baseUrl;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void Setup()
        {
            // Use local URL when running in development, Azure URL when running in CI/CD
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? 
                      "https://localhost:44300"; // Default to local development URL

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        // Helper method to check response and provide detailed error information
        private async Task AssertSuccessfulResponse(HttpResponseMessage response, string scenarioName)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Fail($"{scenarioName} failed: Status Code: {(int)response.StatusCode} {response.StatusCode}, " +
                            $"Response Content: {content.Substring(0, Math.Min(500, content.Length))}");
            }
            
            Assert.That(response.IsSuccessStatusCode, Is.True, $"{scenarioName} failed");
        }

        [Test]
        public async Task TestHighCpuScenarios()
        {
            // These pages are interactive and require button clicks to start the simulation
            // We'll just verify the pages load successfully
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu3.aspx");

            await AssertSuccessfulResponse(response1, "High CPU Scenario 1");
            await AssertSuccessfulResponse(response2, "High CPU Scenario 2");
            await AssertSuccessfulResponse(response3, "High CPU Scenario 3");
        }

        [Test]
        public async Task TestHighMemoryScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory3.aspx");

            await AssertSuccessfulResponse(response1, "High Memory Scenario 1");
            await AssertSuccessfulResponse(response2, "High Memory Scenario 2");
            await AssertSuccessfulResponse(response3, "High Memory Scenario 3");
        }

        [Test]
        public async Task TestSlowResponseScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowResponse/SlowResponse1.aspx");
            await AssertSuccessfulResponse(response1, "Slow Response Scenario 1");
        }

        [Test]
        public async Task TestSlowDependencyScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency2.aspx");

            await AssertSuccessfulResponse(response1, "Slow Dependency Scenario 1");
            await AssertSuccessfulResponse(response2, "Slow Dependency Scenario 2");
        }

        [Test]
        public async Task TestSlowDatabaseScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase2.aspx");

            await AssertSuccessfulResponse(response1, "Slow Database Scenario 1");
            await AssertSuccessfulResponse(response2, "Slow Database Scenario 2");
        }

        [Test]
        public async Task TestCrashScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/UnhandledException1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/StackOverflow1.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash1.aspx");
            var response4 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash2.aspx");
            var response5 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash3.aspx");

            await AssertSuccessfulResponse(response1, "Unhandled Exception Scenario 1");
            await AssertSuccessfulResponse(response2, "Stack Overflow Scenario 1");
            await AssertSuccessfulResponse(response3, "Crash Scenario 1");
            await AssertSuccessfulResponse(response4, "Crash Scenario 2");
            await AssertSuccessfulResponse(response5, "Crash Scenario 3");
        }

        [Test]
        public async Task TestHttp500Scenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_3.aspx");

            await AssertSuccessfulResponse(response1, "HTTP 500 Scenario 1");
            await AssertSuccessfulResponse(response2, "HTTP 500 Scenario 2");
            await AssertSuccessfulResponse(response3, "HTTP 500 Scenario 3");
        }

        [Test]
        public async Task TestConnectionPoolScenarios()
        {
            // These pages are interactive and require a button click to start the simulation
            // We'll just verify the pages load successfully
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool3.aspx");
            
            await AssertSuccessfulResponse(response1, "Connection Pool Scenario 1");
            await AssertSuccessfulResponse(response2, "Connection Pool Scenario 2");
            await AssertSuccessfulResponse(response3, "Connection Pool Scenario 3");
        }

        [Test]
        public async Task TestDeadlockScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Deadlock/Deadlock1.aspx");
            await AssertSuccessfulResponse(response1, "Deadlock Scenario 1");
        }
    }
} 