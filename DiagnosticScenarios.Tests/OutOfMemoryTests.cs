using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("OutOfMemory")]
    public class OutOfMemoryTests : ProcessMetricsBase
    {
        protected override string GetTestCategory() => "OutOfMemory";

        private ProcessMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("OutOfMemory"))
            {
                Assert.Ignore("Skipping OutOfMemoryTests. Set RUN_SPECIALIZED_TESTS=OutOfMemory to run them locally.");
            }
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _helper = new ProcessMetricsHelper(_baseUrl);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _helper?.Dispose();
        }

        [Test]
        public async Task TestOutOfMemoryScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting baseline memory usage...");
            var baseline = await GetProcessMetrics();
            long baselineMemory = baseline.PrivateBytes;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline memory usage: {baselineMemory} bytes");

            // Trigger the out of memory scenario 10 times
            for (int i = 0; i < 3; i++)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering OutOfMemory1Actual.aspx (iteration {i + 1}/10)...");
                var response = await _helper.TriggerScenarioWithResponse("/Scenarios/OutOfMemory/OutOfMemory1Actual.aspx");

                // Wait for memory to be released
                await Task.Delay(TimeSpan.FromSeconds(30));
            }

            // Check for OutOfMemoryException only after the last response
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Checking for OutOfMemoryException after all iterations...");
            var lastResponse = await _helper.TriggerScenarioWithResponse("/Scenarios/OutOfMemory/OutOfMemory1Actual.aspx");
            Assert.That(lastResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError), "Expected InternalServerError");
            var content = await lastResponse.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("Exception of type 'System.OutOfMemoryException' was thrown"), "Response should contain - Exception of type 'System.OutOfMemoryException' was thrown");

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting memory usage after scenario...");
            var after = await GetProcessMetrics();
            long afterMemory = after.PrivateBytes;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Memory usage after scenario: {afterMemory} bytes");

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Memory difference: {afterMemory - baselineMemory} bytes");
        }
    }
} 