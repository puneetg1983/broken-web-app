using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("HighMemory")]
    public class HighMemoryTests
    {
        private ArmMetricsHelper _helper;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests("HighMemory"))
            {
                Assert.Ignore("Skipping HighMemoryTests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
            }

            var baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _helper = await ArmMetricsHelper.CreateAsync(
                baseUrl,
                ArmMetricsHelper.GetSubscriptionId(),
                Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME"),
                Environment.GetEnvironmentVariable("APP_SERVICE_NAME")
            );
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _helper?.Dispose();
        }

        private async Task EnsureFreshProcess()
        {
            await _helper.RestartWebApp();
            await Task.Delay(TimeSpan.FromSeconds(10)); // Wait for app to stabilize
        }

        [Test]
        [Order(1)]
        public async Task TestHighMemoryLeakScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory Leak scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighMemory/HighMemory1Actual.aspx",
                MetricName = "PrivateBytes",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 15,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "Memory");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(2)]
        public async Task TestHighMemoryLargeObjectScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory Large Object scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighMemory/HighMemory2Actual.aspx",
                MetricName = "PrivateBytes",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 15,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "Memory");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(3)]
        public async Task TestHighMemoryStringBuilderScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory StringBuilder scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighMemory/HighMemory3Actual.aspx",
                MetricName = "PrivateBytes",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 15,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "Memory");
            await _helper.RestartWebApp();
        }
    }
} 