using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("ThreadLeak")]
    public class ThreadLeakTests : ProcessMetricsBase
    {
        protected override string GetTestCategory() => "ThreadLeak";

        private ProcessMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("ThreadLeak"))
            {
                Assert.Ignore("Skipping ThreadLeakTests. Set RUN_SPECIALIZED_TESTS=ThreadLeak to run them locally.");
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
        public async Task TestThreadLeakScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting baseline thread count...");
            var baseline = await GetProcessMetrics();
            int baselineThreads = baseline.ThreadCount;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline thread count: {baselineThreads}");

            // Trigger the thread leak scenario multiple times
            const int numberOfRequests = 15;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering ThreadLeak1Actual.aspx {numberOfRequests} times...");
            
            for (int i = 0; i < numberOfRequests; i++)
            {
                await _helper.TriggerScenarioWithResponse("/Scenarios/ThreadLeak/ThreadLeak1Actual.aspx");
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Completed request {i + 1} of {numberOfRequests}");
                // Small delay between requests to avoid overwhelming the server
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // Get final thread count
            var after = await GetProcessMetrics();
            int afterThreads = after.ThreadCount;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Final thread count: {afterThreads}");

            // Verify thread count increased
            Assert.That(afterThreads, Is.GreaterThan(baselineThreads), 
                $"Thread count should increase. Initial: {baselineThreads}, After: {afterThreads}");
        }
    }
} 