using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [NonParallelizable]
    [Category("CrashTests")]
    public class CrashTests: ProcessMetricsBase
    {
        protected override string GetTestCategory() => "CrashTests";

        private ProcessMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ProcessMetricsHelper.ShouldRunTests("CrashTests"))
            {
                Assert.Ignore("Skipping CrashTests tests. Set RUN_SPECIALIZED_TESTS='CrashTests' to run them locally.");
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
        [Order(1)]
        public async Task TestUnhandledExceptionCrash()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Unhandled Exception Crash test...");
            
            // Get initial process ID
            var initialMetrics = await GetProcessMetrics();
            var initialProcessId = initialMetrics.ProcessId;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Initial Process ID: {initialProcessId}");

            // Trigger the crash
            await _helper.TriggerScenarioWithResponse("/Scenarios/Crash/Crash1Actual.aspx");

            // Wait for process to restart
            await Task.Delay(TimeSpan.FromSeconds(60));

            // Get new process ID
            var newMetrics = await GetProcessMetrics();
            var newProcessId = newMetrics.ProcessId;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] New Process ID: {newProcessId}");

            // Verify process ID has changed
            Assert.That(newProcessId, Is.Not.EqualTo(initialProcessId), "Process ID should change after crash");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Unhandled Exception Crash test completed successfully");
        }

        [Test]
        [Order(2)]
        public async Task TestStackOverflowCrash()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Stack Overflow Crash test...");
            
            // Get initial process ID
            var initialMetrics = await GetProcessMetrics();
            var initialProcessId = initialMetrics.ProcessId;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Initial Process ID: {initialProcessId}");

            // Trigger the crash
            await _helper.TriggerScenarioWithResponse("/Scenarios/Crash/Crash2Actual.aspx");

            // Wait for process to restart
            await Task.Delay(TimeSpan.FromSeconds(60));

            // Get new process ID
            var newMetrics = await GetProcessMetrics();
            var newProcessId = newMetrics.ProcessId;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] New Process ID: {newProcessId}");

            // Verify process ID has changed
            Assert.That(newProcessId, Is.Not.EqualTo(initialProcessId), "Process ID should change after crash");
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Stack Overflow Crash test completed successfully");
        }
    }
} 