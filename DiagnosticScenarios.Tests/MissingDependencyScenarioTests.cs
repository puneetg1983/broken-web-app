using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("MissingDependency")]
    public class MissingDependencyScenarioTests
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests("MissingDependency"))
            {
                Assert.Ignore("Skipping MissingDependencyScenarioTests tests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
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
        public async Task TestMissingDependencyScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Missing Dependency scenario test...");
            
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Status Code: {response.StatusCode}");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Content: {responseContent}");
                
                // The app should fail to start due to missing dependency
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError), 
                    "Expected InternalServerError (500) status code due to missing dependency");
                
                // Verify error message indicates assembly loading issue
                Assert.That(responseContent, Does.Contain("Could not load file or assembly").IgnoreCase, 
                    "Response should contain assembly loading error message");
                
                // Verify specific dependency error
                Assert.That(responseContent, Does.Contain("MissingDependency").IgnoreCase, 
                    "Response should mention the missing dependency");
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