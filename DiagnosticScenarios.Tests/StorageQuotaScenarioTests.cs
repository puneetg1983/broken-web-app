using System;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("StorageQuota")]
    public class StorageQuotaScenarioTests
    {
        private HttpClient _client;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            TestContext.Progress.WriteLine("Setting up StorageQuotaScenarioTests...");
            if (!ProcessMetricsHelper.ShouldRunTests("StorageQuota"))
            {
                TestContext.Progress.WriteLine("Skipping tests as RUN_SPECIALIZED_TESTS is not set to StorageQuota");
                Assert.Ignore("Skipping StorageQuotaScenarioTests tests. Set RUN_SPECIALIZED_TESTS=StorageQuota to run them locally.");
            }
            _client = new HttpClient();
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "http://localhost:5000";
            TestContext.Progress.WriteLine($"Using base URL: {_baseUrl}");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task TestStorageQuotaScenario()
        {
            TestContext.Progress.WriteLine("Starting TestStorageQuotaScenario test");
            
            // Arrange
            var url = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1.aspx";
            TestContext.Progress.WriteLine($"Attempting to access URL: {url}");

            // Act
            var response = await _client.GetAsync(url);
            TestContext.Progress.WriteLine($"Initial page response status: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"Initial page content length: {content.Length} characters");

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True, $"Failed to load page. Status code: {response.StatusCode}");
            Assert.That(content, Does.Contain("Storage Quota Exceeded Scenario"));

            // Make 10 requests to trigger the error
            var errorUrl = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1Actual.aspx";
            HttpResponseMessage lastErrorResponse = null;
            string lastErrorContent = null;

            for (int i = 0; i < 10; i++)
            {
                TestContext.Progress.WriteLine($"Making request {i + 1} of 10 to error URL: {errorUrl}");
                lastErrorResponse = await _client.GetAsync(errorUrl);
                lastErrorContent = await lastErrorResponse.Content.ReadAsStringAsync();
                TestContext.Progress.WriteLine($"Request {i + 1} response status: {lastErrorResponse.StatusCode}");
                TestContext.Progress.WriteLine($"Request {i + 1} content length: {lastErrorContent.Length} characters");
            }

            // Assert the 10th response
            Assert.That(lastErrorResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError), 
                $"Expected 500 status code on 10th request, got {lastErrorResponse.StatusCode}");
            Assert.That(lastErrorContent, Does.Contain("There is not enough space on the disk").IgnoreCase, 
                "Expected 'There is not enough space on the disk' in the response content");
            
            TestContext.Progress.WriteLine("Test completed successfully");
        }
    }
}
 