using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("InvalidZip")]
    public class InvalidZipScenarioTests
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("InvalidZip"))
            {
                Assert.Ignore("Skipping InvalidZipScenarioTests tests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
            }
            _httpClient = new HttpClient();
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL");
            
            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("WEBAPP_URL environment variable is not set");
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task TestInvalidZipScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Invalid Zip scenario test...");
            
            const int maxRetries = 5;
            const int retryDelayMs = 2000; // 2 seconds between retries
            var retryCount = 0;
            Exception lastException = null;

            while (retryCount < maxRetries)
            {
                try
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Attempt {retryCount + 1} of {maxRetries}");
                    
                    var response = await _httpClient.GetAsync(_baseUrl);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Status Code: {response.StatusCode}");
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Content: {responseContent}");
                    
                    // The app should fail to start due to invalid zip
                    Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.ServiceUnavailable), 
                        "Expected ServiceUnavailable (503) status code due to invalid zip deployment");
                    
                    // Verify error message indicates deployment issue
                    Assert.That(responseContent, Does.Contain("The service is unavailable").IgnoreCase, 
                        "Response should contain 'The service is unavailable' message");
                    
                    // If we get here, the test passed
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Attempt {retryCount + 1} failed: {ex.Message}");
                    retryCount++;
                    
                    if (retryCount < maxRetries)
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Waiting {retryDelayMs}ms before next attempt...");
                        await Task.Delay(retryDelayMs);
                    }
                }
            }

            // If we get here, all retries failed
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] All {maxRetries} attempts failed");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Last exception: {lastException}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Last exception type: {lastException?.GetType().FullName}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Last exception stack trace: {lastException?.StackTrace}");
            throw lastException ?? new Exception("All retry attempts failed without capturing an exception");
        }
    }
} 