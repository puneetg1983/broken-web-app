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
            if (!ArmMetricsHelper.ShouldRunTests("StorageQuota"))
            {
                Assert.Ignore("Skipping StorageQuotaScenarioTests tests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
            }
            _client = new HttpClient();
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "http://localhost:5000";
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task TestStorageQuotaScenario()
        {
            // Arrange
            var url = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1.aspx";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True, $"Failed to load page. Status code: {response.StatusCode}");
            Assert.That(content, Does.Contain("Storage Quota Exceeded Scenario"));

            // Trigger the error
            var errorUrl = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1Actual.aspx";
            var errorResponse = await _client.GetAsync(errorUrl);
            var errorContent = await errorResponse.Content.ReadAsStringAsync();

            // Assert error response
            Assert.That(errorResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
            Assert.That(errorContent, Does.Contain("Storage quota exceeded").IgnoreCase);
        }

        [Test]
        public async Task TestStorageQuotaScenarioWithRetry()
        {
            // Arrange
            var url = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1.aspx";
            var maxRetries = 3;
            var retryDelay = TimeSpan.FromSeconds(2);

            // Act & Assert
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await _client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    Assert.That(response.IsSuccessStatusCode, Is.True, $"Failed to load page. Status code: {response.StatusCode}");
                    Assert.That(content, Does.Contain("Storage Quota Exceeded Scenario"));

                    // Trigger the error
                    var errorUrl = $"{_baseUrl}/Scenarios/StorageQuota/StorageQuota1Actual.aspx";
                    var errorResponse = await _client.GetAsync(errorUrl);
                    var errorContent = await errorResponse.Content.ReadAsStringAsync();

                    // Assert error response
                    Assert.That(errorResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError));
                    Assert.That(errorContent, Does.Contain("Storage quota exceeded").IgnoreCase);
                    return;
                }
                catch (Exception ex) when (i < maxRetries - 1)
                {
                    TestContext.Progress.WriteLine($"Attempt {i + 1} failed: {ex.Message}");
                    await Task.Delay(retryDelay);
                }
            }

            Assert.Fail("All retry attempts failed");
        }
    }
} 