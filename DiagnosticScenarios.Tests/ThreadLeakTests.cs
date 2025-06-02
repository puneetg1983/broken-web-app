using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("ThreadLeak")]
    public class ThreadLeakTests
    {
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
            var baseline = await _helper.GetMetrics();
            int baselineThreads = baseline.ThreadCount;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Baseline thread count: {baselineThreads}");

            // Trigger the thread leak scenario
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Triggering ThreadLeak1Actual.aspx...");
            await _helper.TriggerScenarioWithResponse("/Scenarios/ThreadLeak/ThreadLeak1Actual.aspx");

            // Wait for threads to leak
            await Task.Delay(TimeSpan.FromSeconds(30));

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Getting thread count after scenario...");
            var after = await _helper.GetMetrics();
            int afterThreads = after.ThreadCount;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Thread count after scenario: {afterThreads}");

            Assert.Greater(afterThreads, baselineThreads, "Thread count should increase after running the thread leak scenario.");
        }
    }
} 