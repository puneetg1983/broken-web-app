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
            if (!ArmMetricsHelper.ShouldRunTests("StorageQuota"))
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

            // Trigger the error
            var errorUrl = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1Actual.aspx";
            TestContext.Progress.WriteLine($"Attempting to access error URL: {errorUrl}");
            var errorResponse = await _client.GetAsync(errorUrl);
            TestContext.Progress.WriteLine($"Error page response status: {errorResponse.StatusCode}");
            var errorContent = await errorResponse.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"Error page content length: {errorContent.Length} characters");

            // Assert error response
            Assert.That(errorResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
            Assert.That(errorContent, Does.Contain("Storage quota exceeded").IgnoreCase);
            
            TestContext.Progress.WriteLine("Test completed successfully");
        }

        [Test]
        public async Task TestStorageQuotaScenarioWithRetry()
        {
            TestContext.Progress.WriteLine("Starting TestStorageQuotaScenarioWithRetry test");
            
            // Arrange
            var url = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1.aspx";
            var maxRetries = 3;
            var retryDelay = TimeSpan.FromSeconds(2);
            TestContext.Progress.WriteLine($"Using URL: {url} with {maxRetries} retries and {retryDelay.TotalSeconds}s delay");

            // Act & Assert
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} of {maxRetries}");
                    TestContext.Progress.WriteLine($"Accessing URL: {url}");
                    
                    var response = await _client.GetAsync(url);
                    TestContext.Progress.WriteLine($"Response status: {response.StatusCode}");
                    var content = await response.Content.ReadAsStringAsync();
                    TestContext.Progress.WriteLine($"Content length: {content.Length} characters");

                    Assert.That(response.IsSuccessStatusCode, Is.True, $"Failed to load page. Status code: {response.StatusCode}");
                    Assert.That(content, Does.Contain("Storage Quota Exceeded Scenario"));

                    // Trigger the error
                    var errorUrl = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1Actual.aspx";
                    TestContext.Progress.WriteLine($"Accessing error URL: {errorUrl}");
                    var errorResponse = await _client.GetAsync(errorUrl);
                    TestContext.Progress.WriteLine($"Error response status: {errorResponse.StatusCode}");
                    var errorContent = await errorResponse.Content.ReadAsStringAsync();
                    TestContext.Progress.WriteLine($"Error content length: {errorContent.Length} characters");

                    // Assert error response
                    Assert.That(errorResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
                    Assert.That(errorContent, Does.Contain("Storage quota exceeded").IgnoreCase);
                    
                    TestContext.Progress.WriteLine("Test completed successfully");
                    return;
                }
                catch (Exception ex) when (i < maxRetries - 1)
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} failed with error: {ex.Message}");
                    TestContext.Progress.WriteLine($"Stack trace: {ex.StackTrace}");
                    TestContext.Progress.WriteLine($"Waiting {retryDelay.TotalSeconds} seconds before next attempt...");
                    await Task.Delay(retryDelay);
                }
            }

            TestContext.Progress.WriteLine("All retry attempts failed");
            Assert.Fail("All retry attempts failed");
        }
    }
} 