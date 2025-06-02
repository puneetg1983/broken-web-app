using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("OutOfMemory")]
    public class OutOfMemoryTests
    {
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
            var baseline = await _helper.GetMetrics();
            long baselineMemory = baseline.PrivateBytes;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline memory usage: {baselineMemory} bytes");

            // Trigger the out of memory scenario 10 times
            for (int i = 0; i < 10; i++)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering OutOfMemory1Actual.aspx (iteration {i + 1}/10)...");
                var response = await _helper.TriggerScenarioWithResponse("/Scenarios/OutOfMemory/OutOfMemory1Actual.aspx");

                // Wait for memory to be released
                await Task.Delay(TimeSpan.FromSeconds(30));
            }

            // Check for OutOfMemoryException only after the last response
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Checking for OutOfMemoryException after all iterations...");
            var lastResponse = await _helper.TriggerScenarioWithResponse("/Scenarios/OutOfMemory/OutOfMemory1Actual.aspx");
            Assert.That(lastResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.InternalServerError), "Expected an OutOfMemoryException to be thrown.");
            var content = await lastResponse.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("OutOfMemoryException"), "Response should indicate an OutOfMemoryException was thrown.");

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting memory usage after scenario...");
            var after = await _helper.GetMetrics();
            long afterMemory = after.PrivateBytes;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Memory usage after scenario: {afterMemory} bytes");

            Assert.Greater(afterMemory, baselineMemory, "Memory usage should increase after running the out of memory scenario.");
        }
    }
} 