using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("StartupScenarios")]
    public class StartupScenarioTests
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
        [Category("HangOnStartup")]
        public async Task TestHangOnStartupScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Testing Hang on Startup scenario...");
            
            try
            {
                // Set a timeout of 30 seconds for the request
                _httpClient.Timeout = TimeSpan.FromSeconds(30);
                
                // Attempt to access the default page
                var response = await _httpClient.GetAsync(_baseUrl);
                
                // If we get here, the request didn't hang as expected
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Unexpected response received:");
                TestContext.Progress.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
                TestContext.Progress.WriteLine($"Response Content: {await response.Content.ReadAsStringAsync()}");
                Assert.Fail("The request should have timed out due to the app hanging on startup");
            }
            catch (TaskCanceledException)
            {
                // This is the expected behavior - the request should time out
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Request timed out as expected");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Unexpected exception: {ex.Message}");
                TestContext.Progress.WriteLine($"Exception Type: {ex.GetType().FullName}");
                TestContext.Progress.WriteLine($"Stack Trace: {ex.StackTrace}");
                Assert.Fail($"Unexpected exception: {ex.Message}");
            }
        }

        [Test]
        [Category("CrashOnStartup")]
        public async Task TestCrashOnStartupScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Testing Crash on Startup scenario...");
            
            try
            {
                // Attempt to access the default page
                var response = await _httpClient.GetAsync(_baseUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response received:");
                TestContext.Progress.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
                TestContext.Progress.WriteLine($"Response Content: {responseContent}");
                
                // The app should return a 500 error due to crashing on startup
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError), 
                    $"Expected 500 Internal Server Error, but got {(int)response.StatusCode} {response.StatusCode}");
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Received 500 error as expected");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Unexpected exception: {ex.Message}");
                TestContext.Progress.WriteLine($"Exception Type: {ex.GetType().FullName}");
                TestContext.Progress.WriteLine($"Stack Trace: {ex.StackTrace}");
                Assert.Fail($"Unexpected exception: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }
    }
} 