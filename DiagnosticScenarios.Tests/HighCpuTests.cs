using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [Category("HighCpu")]
    public class HighCpuTests
    {
        private ArmMetricsHelper _helper;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests("HighCpu"))
            {
                Assert.Ignore("Skipping HighCpuTests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
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
        public async Task TestHighCpuInfiniteLoopScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High CPU Infinite Loop scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighCpu/HighCpu1Actual.aspx",
                MetricName = "CpuTime",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 5,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "CPU");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(2)]
        public async Task TestHighCpuThreadContentionScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High CPU Thread Contention scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighCpu/HighCpu2Actual.aspx",
                MetricName = "CpuTime",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 5,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "CPU");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(3)]
        public async Task TestHighCpuComplexRegexScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High CPU Complex Regex scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighCpu/HighCpu3Actual.aspx",
                MetricName = "CpuTime",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 5,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "CPU");
            await _helper.RestartWebApp();
        }
    }
} 