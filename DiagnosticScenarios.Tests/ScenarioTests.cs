using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    public class ScenarioTests
    {
        private string _baseUrl;
        private HttpClient _httpClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Use local URL when running in development, Azure URL when running in CI/CD
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300"; // Default to local development URL

            Console.WriteLine($"[{DateTime.Now}] Using base URL: {_baseUrl}");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DiagnosticScenarios.Tests");
            // Increase timeout for problematic scenarios
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
            
            // Warmup request to ensure the app is running
            await EnsureAppIsRunning();
        }

        // Make a warmup request to ensure the app is running
        private async Task EnsureAppIsRunning()
        {
            Console.WriteLine($"[{DateTime.Now}] Making warmup request to ensure app is running...");
            int maxRetries = 5;
            int retryDelaySeconds = 50;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now}] Warmup attempt {attempt} of {maxRetries}");
                    var response = await _httpClient.GetAsync(_baseUrl);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[{DateTime.Now}] Warmup request succeeded with status code {(int)response.StatusCode} {response.StatusCode}");
                        return;
                    }
                    
                    Console.WriteLine($"[{DateTime.Now}] Warmup request returned status code {(int)response.StatusCode} {response.StatusCode}");
                    
                    if (attempt == maxRetries)
                    {
                        Console.WriteLine($"[{DateTime.Now}] WARNING: App may not be ready after {maxRetries} attempts. Tests may fail.");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now}] Warmup request failed: {ex.GetType().Name}: {ex.Message}");
                    
                    if (attempt == maxRetries)
                    {
                        Console.WriteLine($"[{DateTime.Now}] WARNING: Could not reach the app after {maxRetries} attempts. Tests may fail.");
                        break;
                    }
                }
                
                Console.WriteLine($"[{DateTime.Now}] Waiting {retryDelaySeconds} seconds before next warmup attempt...");
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
            }
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
                Console.WriteLine($"[{DateTime.Now}] ERROR: {scenarioName} failed with status code {(int)response.StatusCode} {response.StatusCode}");
                Assert.Fail($"{scenarioName} failed: Status Code: {(int)response.StatusCode} {response.StatusCode}, " +
                            $"Response Content: {content.Substring(0, Math.Min(500, content.Length))}");
            }
            
            Console.WriteLine($"[{DateTime.Now}] SUCCESS: {scenarioName} returned {(int)response.StatusCode} {response.StatusCode}");
            Assert.That(response.IsSuccessStatusCode, Is.True, $"{scenarioName} failed");
        }

        // Helper method to retry HTTP requests that might time out
        private async Task<HttpResponseMessage> GetWithRetryAsync(string url, string scenarioName, int maxRetries = 3)
        {
            Console.WriteLine($"[{DateTime.Now}] Testing URL for {scenarioName}: {url}");
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now}] Attempt {attempt} for {scenarioName}");
                    var response = await _httpClient.GetAsync(url);
                    return response;
                }
                catch (TaskCanceledException)
                {
                    if (attempt == maxRetries)
                        throw;
                    
                    Console.WriteLine($"[{DateTime.Now}] Request timed out for {scenarioName}, retrying...");
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
                catch (HttpRequestException ex)
                {
                    if (attempt == maxRetries)
                        throw;
                    
                    Console.WriteLine($"[{DateTime.Now}] HTTP request error for {scenarioName}: {ex.Message}, retrying...");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            
            throw new InvalidOperationException("Should not reach here due to retry logic");
        }

        [Test]
        public async Task TestHighCpuScenarios()
        {
            // These pages are interactive and require button clicks to start the simulation
            // We'll just verify the pages load successfully
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu1.aspx", "High CPU Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu2.aspx", "High CPU Scenario 2");
            var response3 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighCpu/HighCpu3.aspx", "High CPU Scenario 3");

            await AssertSuccessfulResponse(response1, "High CPU Scenario 1");
            await AssertSuccessfulResponse(response2, "High CPU Scenario 2");
            await AssertSuccessfulResponse(response3, "High CPU Scenario 3");
        }

        [Test]
        public async Task TestHighMemoryScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory1.aspx", "High Memory Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory2.aspx", "High Memory Scenario 2");
            var response3 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/HighMemory/HighMemory3.aspx", "High Memory Scenario 3");

            await AssertSuccessfulResponse(response1, "High Memory Scenario 1");
            await AssertSuccessfulResponse(response2, "High Memory Scenario 2");
            await AssertSuccessfulResponse(response3, "High Memory Scenario 3");
        }

        [Test]
        public async Task TestSlowResponseScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/SlowResponse/SlowResponse1.aspx", "Slow Response Scenario 1");
            await AssertSuccessfulResponse(response1, "Slow Response Scenario 1");
        }

        [Test]
        public async Task TestSlowDependencyScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency1.aspx", "Slow Dependency Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/SlowDependency/SlowDependency2.aspx", "Slow Dependency Scenario 2");

            await AssertSuccessfulResponse(response1, "Slow Dependency Scenario 1");
            await AssertSuccessfulResponse(response2, "Slow Dependency Scenario 2");
        }

        [Test]
        public async Task TestSlowDatabaseScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase1.aspx", "Slow Database Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/SlowDatabase/SlowDatabase2.aspx", "Slow Database Scenario 2");

            await AssertSuccessfulResponse(response1, "Slow Database Scenario 1");
            await AssertSuccessfulResponse(response2, "Slow Database Scenario 2");
        }

        [Test]
        public async Task TestCrashScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Crash/UnhandledException1.aspx", "Unhandled Exception Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Crash/StackOverflow1.aspx", "Stack Overflow Scenario 1");
            var response3 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Crash/Crash1.aspx", "Crash Scenario 1");
            var response4 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Crash/Crash2.aspx", "Crash Scenario 2");
            var response5 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Crash/Crash3.aspx", "Crash Scenario 3");

            await AssertSuccessfulResponse(response1, "Unhandled Exception Scenario 1");
            await AssertSuccessfulResponse(response2, "Stack Overflow Scenario 1");
            await AssertSuccessfulResponse(response3, "Crash Scenario 1");
            await AssertSuccessfulResponse(response4, "Crash Scenario 2");
            await AssertSuccessfulResponse(response5, "Crash Scenario 3");
        }

        [Test]
        public async Task TestHttp500Scenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Http500/Http500_1.aspx", "HTTP 500 Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Http500/Http500_2.aspx", "HTTP 500 Scenario 2");
            var response3 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Http500/Http500_3.aspx", "HTTP 500 Scenario 3");

            await AssertSuccessfulResponse(response1, "HTTP 500 Scenario 1");
            await AssertSuccessfulResponse(response2, "HTTP 500 Scenario 2");
            await AssertSuccessfulResponse(response3, "HTTP 500 Scenario 3");
        }

        [Test]
        public async Task TestConnectionPoolScenarios()
        {
            // These pages are interactive and require a button click to start the simulation
            // We'll just verify the pages load successfully
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool1.aspx", "Connection Pool Scenario 1");
            var response2 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool2.aspx", "Connection Pool Scenario 2");
            var response3 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/ConnectionPool/ConnectionPool3.aspx", "Connection Pool Scenario 3");
            
            await AssertSuccessfulResponse(response1, "Connection Pool Scenario 1");
            await AssertSuccessfulResponse(response2, "Connection Pool Scenario 2");
            await AssertSuccessfulResponse(response3, "Connection Pool Scenario 3");
        }

        [Test]
        public async Task TestDeadlockScenarios()
        {
            var response1 = await GetWithRetryAsync($"{_baseUrl}/Scenarios/Deadlock/Deadlock1.aspx", "Deadlock Scenario 1");
            await AssertSuccessfulResponse(response1, "Deadlock Scenario 1");
        }
    }
} 