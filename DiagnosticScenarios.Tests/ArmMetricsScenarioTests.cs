using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiagnosticScenarios.Tests
{
    [TestFixture]
    [NonParallelizable]
    [Category("ArmMetrics")]
    public class ArmMetricsScenarioTests
    {
        private const int DEFAULT_METRICS_WAIT_SECONDS = 30;
        private const int DEFAULT_RESTART_WAIT_SECONDS = 30;
        private const int DEFAULT_WARMUP_RETRIES = 10;
        private const int DEFAULT_WARMUP_RETRY_DELAY_SECONDS = 10;
        private const int MINIMUM_METRIC_INCREASE_PERCENTAGE = 20;

        private ArmMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests())
            {
                Assert.Ignore("Skipping ARM metrics tests. Set RUN_ARM_METRICS_TESTS_LOCALLY=true to run them locally.");
            }

            InitializeEnvironmentVariables();
            await InitializeHelper();
            await EnsureAppIsRunning();
        }

        private void InitializeEnvironmentVariables()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Initializing environment variables...");
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            var subscriptionId = ArmMetricsHelper.GetSubscriptionId();
            var resourceGroup = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME");
            var appServiceName = Environment.GetEnvironmentVariable("APP_SERVICE_NAME");

            LogEnvironmentVariables();

            if (string.IsNullOrEmpty(resourceGroup) || string.IsNullOrEmpty(appServiceName))
            {
                Assert.Fail("Required environment variables are not set: RESOURCE_GROUP_NAME, APP_SERVICE_NAME");
            }
        }

        private void LogEnvironmentVariables()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Environment variables:");
            TestContext.Progress.WriteLine($"  WEBAPP_URL: {_baseUrl}");
            TestContext.Progress.WriteLine($"  AZURE_SUBSCRIPTION_ID: {ArmMetricsHelper.GetSubscriptionId()}");
            TestContext.Progress.WriteLine($"  RESOURCE_GROUP_NAME: {Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME")}");
            TestContext.Progress.WriteLine($"  APP_SERVICE_NAME: {Environment.GetEnvironmentVariable("APP_SERVICE_NAME")}");
        }

        private async Task InitializeHelper()
        {
            try
            {
                _helper = await ArmMetricsHelper.CreateAsync(
                    _baseUrl,
                    ArmMetricsHelper.GetSubscriptionId(),
                    Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME"),
                    Environment.GetEnvironmentVariable("APP_SERVICE_NAME")
                );
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to initialize helper: {ex.Message}");
            }
        }

        private async Task EnsureAppIsRunning()
        {
            if (_helper == null)
            {
                throw new InvalidOperationException("Helper is not initialized");
            }
            await _helper.EnsureAppIsRunning();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Cleaning up resources...");
            _helper?.Dispose();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Cleanup completed");
        }

        [TearDown]
        public async Task TestCleanup()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Running test cleanup...");
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Error during test cleanup: {ex.Message}");
            }
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Test cleanup completed");
        }

        [Test]
        [Order(1)]
        public async Task TestHighCpuScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High CPU scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighCpu/HighCpu1Actual.aspx",
                MetricName = "CpuTime",
                Iterations = 5,
                DelayBetweenIterationsSeconds = 5,
                WaitForMetricsMinutes = 1
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "CPU");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(2)]
        public async Task TestHighMemoryScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting High Memory scenario test...");
            
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighMemory/HighMemory1Actual.aspx",
                MetricName = "PrivateBytes",
                Iterations = 3,
                DelayBetweenIterationsSeconds = 15,
                WaitForMetricsMinutes = 1
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "Memory");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(3)]
        public async Task TestHttp500Scenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting HTTP 500 scenario test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_1Actual.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        [Test]
        [Order(4)]
        public async Task TestSlowResponseScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Slow Response scenario test...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowResponse/SlowResponse1Actual.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger slow response scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Slow Response scenario test completed successfully");
        }
    }
} 