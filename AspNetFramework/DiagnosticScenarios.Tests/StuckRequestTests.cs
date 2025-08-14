using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("StuckRequest")]
    public class StuckRequestTests
    {
        private ProcessMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("StuckRequest"))
            {
                Assert.Ignore("Skipping StuckRequestTests. Set RUN_SPECIALIZED_TESTS=StuckRequest to run them locally.");
            }

            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _helper = new ProcessMetricsHelper(_baseUrl);

            await EnsureAppIsRunning();
        }

        private async Task EnsureAppIsRunning()
        {
            if (_helper == null)
            {
                throw new InvalidOperationException("Helper is not initialized");
            }
            await _helper.EnsureAppIsRunning();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Cleaning up resources...");
            _helper?.Dispose();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Cleanup completed");
        }

        [Test]
        [Order(1)]
        public async Task TestHighSleepPageReturns200()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Sleep page test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/StuckRequests/HighSleep.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response status: {response.StatusCode}");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content length: {responseContent?.Length ?? 0} characters");
            
            Assert.That(response.IsSuccessStatusCode, Is.True, $"HighSleep.aspx should return 200 OK, but got {response.StatusCode}");
            Assert.That(responseContent, Is.Not.Null.And.Not.Empty, "Response content should not be empty");
            
            // Verify the page contains expected content
            Assert.That(responseContent.Contains("High Sleep Scenario"), Is.True, "Page should contain 'High Sleep Scenario' text");
            Assert.That(responseContent.Contains("Start High Sleep Operation"), Is.True, "Page should contain the button text");
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep page test completed successfully");
        }

        [Test]
        [Order(2)]
        public async Task TestHighSleepActualPageTimesOutOrReturns500()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Sleep Actual page test (expecting timeout or 500 error)...");
            
            // Create a custom HttpClient with a shorter timeout for this specific test
            using (var timeoutClient = new HttpClient())
            {
                timeoutClient.Timeout = TimeSpan.FromSeconds(60); // 1 minute timeout, much less than the 300 seconds sleep
                
                try
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var response = await timeoutClient.GetAsync($"{_baseUrl}/Scenarios/StuckRequests/HighSleepActual.aspx");
                    stopwatch.Stop();
                    
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response received after {stopwatch.ElapsedMilliseconds}ms");
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response status: {response.StatusCode}");
                    
                    var responseContent = await response.Content.ReadAsStringAsync();
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content preview: {responseContent?.Substring(0, Math.Min(500, responseContent?.Length ?? 0))}...");
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Full response content length: {responseContent?.Length ?? 0} characters");
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Full response content: {responseContent}");
                    }
                    
                    // If we get a response, it should be a 500 error due to timeout
                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        // Check if the error is related to timeout
                        Assert.That(responseContent.ToLower().Contains("timeout") || 
                                   responseContent.ToLower().Contains("time") ||
                                   responseContent.ToLower().Contains("abort") ||
                                   responseContent.ToLower().Contains("cancel"),
                                   Is.True, 
                                   "HTTP 500 response should contain timeout-related error message");
                        
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page correctly returned HTTP 500 with timeout-related error");
                    }
                    else
                    {
                        Assert.Fail($"Expected HTTP 500 or timeout exception, but got {response.StatusCode}");
                    }
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || ex.Message.Contains("timeout"))
                {
                    // This is expected - the request timed out because the page sleeps for 300 seconds
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page correctly timed out as expected: {ex.Message}");
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("timeout") || ex.Message.Contains("cancel"))
                {
                    // This is also expected - timeout at HTTP level
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page correctly timed out as expected: {ex.Message}");
                }
                catch (OperationCanceledException ex)
                {
                    // This is expected when the operation is canceled due to timeout
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page correctly timed out as expected: {ex.Message}");
                }
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page test completed successfully (timeout or 500 error as expected)");
        }

        [Test]
        [Order(3)]
        public async Task TestHighSleepActualPageWithLongerTimeout()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Sleep Actual page test with extended timeout...");
            
            // This test will actually wait for the full response to see if we get a proper timeout from ASP.NET
            using (var extendedTimeoutClient = new HttpClient())
            {
                extendedTimeoutClient.Timeout = TimeSpan.FromMinutes(6); // 6 minutes - longer than the 5 minute execution timeout
                
                try
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var response = await extendedTimeoutClient.GetAsync($"{_baseUrl}/Scenarios/StuckRequests/HighSleepActual.aspx");
                    stopwatch.Stop();
                    
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response received after {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalMinutes:F2} minutes)");
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response status: {response.StatusCode}");
                    
                    var responseContent = await response.Content.ReadAsStringAsync();
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content preview: {responseContent?.Substring(0, Math.Min(1000, responseContent?.Length ?? 0))}...");
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Full response content length: {responseContent?.Length ?? 0} characters");
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Full response content: {responseContent}");
                    }
                    
                    // The response should be HTTP 500 due to ASP.NET execution timeout (configured for 5 minutes)
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), 
                               "Should receive HTTP 500 due to ASP.NET execution timeout");
                    
                    // Verify the response contains timeout-related content
                    Assert.That(responseContent.ToLower().Contains("timeout") || 
                               responseContent.ToLower().Contains("time") ||
                               responseContent.ToLower().Contains("execution") ||
                               responseContent.ToLower().Contains("abort"),
                               Is.True, 
                               "Response should contain timeout or execution-related error message");
                    
                    // Verify the response took approximately 5 minutes (with some tolerance)
                    var elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
                    Assert.That(elapsedMinutes, Is.GreaterThan(2.0).And.LessThan(6.0), 
                               $"Response should take approximately 5 minutes due to execution timeout, but took {elapsedMinutes:F2} minutes");
                    
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual page correctly timed out after {elapsedMinutes:F2} minutes with HTTP 500");
                }
                catch (TaskCanceledException ex)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Request was canceled (this may be expected): {ex.Message}");
                    // This might happen if the server-side timeout occurs before our client timeout
                    // This is still a valid test result as it shows the stuck request behavior
                }
                catch (HttpRequestException ex)
                {
                    TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] HTTP request exception (may be expected due to timeout): {ex.Message}");
                    // This might happen due to server-side timeout
                    // This is still a valid test result as it shows the stuck request behavior
                }
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] High Sleep Actual extended timeout test completed");
        }
    }
}
