using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [NonParallelizable]
    [Category("ResourceMetrics")]
    public class ResourceMetricsTests
    {
        private ProcessMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public void Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests("ResourceMetrics"))
            {
                Assert.Ignore("Skipping ResourceMetrics tests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
            }
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            _helper = new ProcessMetricsHelper(_baseUrl);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _helper?.Dispose();
        }

        private async Task EnsureFreshProcess()
        {
            var metrics = await _helper.GetMetrics();
            if (metrics.ProcessUptimeMinutes < 1)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Process is fresh (uptime: {metrics.ProcessUptimeMinutes:F2} minutes)");
                return;
            }

            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Process uptime is {metrics.ProcessUptimeMinutes:F2} minutes, restarting web app...");
            await _helper.RestartWebApp();
            await Task.Delay(TimeSpan.FromSeconds(30)); // Wait for restart to complete
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

        [Test]
        [Order(4)]
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
        [Order(5)]
        public async Task TestHighMemoryEventHandlerLeakScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory Event Handler Leak scenario test...");
            
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
        [Order(6)]
        public async Task TestHighMemoryLohFragmentationScenario()
        {
            await EnsureFreshProcess();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory LOH Fragmentation scenario test...");
            
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