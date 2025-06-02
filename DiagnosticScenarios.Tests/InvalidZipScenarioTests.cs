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
            
            try
            {
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
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Exception occurred: {ex}");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Exception type: {ex.GetType().FullName}");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 