using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Web
{
    [TestFixture]
    public class ScenarioTests
    {
        private string _baseUrl;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? 
                      throw new InvalidOperationException("WEBAPP_URL environment variable is not set. Please set it in your GitHub repository secrets.");
            _httpClient = new HttpClient();
        }

        [Test]
        public async Task TestHighCpuScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu3.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "High CPU Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "High CPU Scenario 2 failed");
            Assert.That(response3.IsSuccessStatusCode, Is.True, "High CPU Scenario 3 failed");
        }

        [Test]
        public async Task TestHighMemoryScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory3.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "High Memory Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "High Memory Scenario 2 failed");
            Assert.That(response3.IsSuccessStatusCode, Is.True, "High Memory Scenario 3 failed");
        }

        [Test]
        public async Task TestSlowResponseScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowResponse/SlowResponse1.aspx");
            Assert.That(response1.IsSuccessStatusCode, Is.True, "Slow Response Scenario 1 failed");
        }

        [Test]
        public async Task TestSlowDependencyScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency2.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "Slow Dependency Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "Slow Dependency Scenario 2 failed");
        }

        [Test]
        public async Task TestSlowDatabaseScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase2.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "Slow Database Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "Slow Database Scenario 2 failed");
        }

        [Test]
        public async Task TestCrashScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/UnhandledException1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/StackOverflow1.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash1.aspx");
            var response4 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash2.aspx");
            var response5 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Crash/Crash3.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "Crash Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "Crash Scenario 2 failed");
            Assert.That(response3.IsSuccessStatusCode, Is.True, "Crash Scenario 3 failed");
            Assert.That(response4.IsSuccessStatusCode, Is.True, "Crash Scenario 4 failed");
            Assert.That(response5.IsSuccessStatusCode, Is.True, "Crash Scenario 5 failed");
        }

        [Test]
        public async Task TestHttp500Scenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Http500/Http500_3.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "HTTP 500 Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "HTTP 500 Scenario 2 failed");
            Assert.That(response3.IsSuccessStatusCode, Is.True, "HTTP 500 Scenario 3 failed");
        }

        [Test]
        public async Task TestConnectionPoolScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool1.aspx");
            var response2 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool2.aspx");
            var response3 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool3.aspx");

            Assert.That(response1.IsSuccessStatusCode, Is.True, "Connection Pool Scenario 1 failed");
            Assert.That(response2.IsSuccessStatusCode, Is.True, "Connection Pool Scenario 2 failed");
            Assert.That(response3.IsSuccessStatusCode, Is.True, "Connection Pool Scenario 3 failed");
        }

        [Test]
        public async Task TestDeadlockScenarios()
        {
            var response1 = await _httpClient.GetAsync($"{_baseUrl}/Scenarios/Deadlock/Deadlock1.aspx");
            Assert.That(response1.IsSuccessStatusCode, Is.True, "Deadlock Scenario 1 failed");
        }
    }
} 