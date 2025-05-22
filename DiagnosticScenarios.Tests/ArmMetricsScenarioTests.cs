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

            await InitializeEnvironmentVariables();
            await InitializeHelper();
            await EnsureAppIsRunning();
        }

        private async Task InitializeEnvironmentVariables()
        {
            Console.WriteLine($"[{DateTime.Now}] Initializing environment variables...");
            _baseUrl = Environment.GetEnvironmentVariable("WEBAPP_URL") ?? "https://localhost:44300";
            var subscriptionId = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
            var resourceGroup = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME");
            var appServiceName = Environment.GetEnvironmentVariable("APP_SERVICE_NAME");

            LogEnvironmentVariables();

            if (string.IsNullOrEmpty(subscriptionId) || string.IsNullOrEmpty(resourceGroup) || string.IsNullOrEmpty(appServiceName))
            {
                Assert.Fail("Required environment variables are not set: SUBSCRIPTION_ID, RESOURCE_GROUP_NAME, APP_SERVICE_NAME");
            }
        }

        private void LogEnvironmentVariables()
        {
            Console.WriteLine($"[{DateTime.Now}] Environment variables:");
            Console.WriteLine($"  WEBAPP_URL: {_baseUrl}");
            Console.WriteLine($"  SUBSCRIPTION_ID: {Environment.GetEnvironmentVariable("SUBSCRIPTION_ID")}");
            Console.WriteLine($"  RESOURCE_GROUP_NAME: {Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME")}");
            Console.WriteLine($"  APP_SERVICE_NAME: {Environment.GetEnvironmentVariable("APP_SERVICE_NAME")}");
        }

        private async Task InitializeHelper()
        {
            try
            {
                _helper = await ArmMetricsHelper.CreateAsync(
                    _baseUrl,
                    Environment.GetEnvironmentVariable("SUBSCRIPTION_ID"),
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
            await _helper.EnsureAppIsRunning();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Console.WriteLine($"[{DateTime.Now}] Cleaning up resources...");
            _helper?.Dispose();
            Console.WriteLine($"[{DateTime.Now}] Cleanup completed");
        }

        [TearDown]
        public async Task TestCleanup()
        {
            Console.WriteLine($"[{DateTime.Now}] Running test cleanup...");
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Error during test cleanup: {ex.Message}");
            }
            Console.WriteLine($"[{DateTime.Now}] Test cleanup completed");
        }

        [Test]
        [Order(1)]
        public async Task TestHighCpuScenario()
        {
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighCpu/HighCpu1.aspx",
                ButtonId = "btnHighCpu",
                ButtonText = "Simulate High CPU",
                MetricName = "CpuPercentage",
                Iterations = 5,
                DelayBetweenIterationsSeconds = 5,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "CPU");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(2)]
        public async Task TestHighMemoryScenario()
        {
            var scenario = new ScenarioInfo
            {
                Path = "/Scenarios/HighMemory/HighMemory1.aspx",
                ButtonId = "btnMemoryLeak",
                ButtonText = "Simulate Memory Leak",
                MetricName = "MemoryPercentage",
                Iterations = 7,
                DelayBetweenIterationsSeconds = 15,
                WaitForMetricsMinutes = 2
            };

            var (baseline, after) = await _helper.RunResourceIntensiveScenario(scenario);
            _helper.VerifyMetricIncrease(after, baseline, "Memory");
            await _helper.RestartWebApp();
        }

        [Test]
        [Order(3)]
        public async Task TestHttp500Scenario()
        {
            Console.WriteLine($"[{DateTime.Now}] Starting HTTP 500 scenario test...");
            
            // First get the page to extract form data
            var getResponse = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_1.aspx");
            if (!getResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get HTTP 500 page");
            }
            
            var content = await getResponse.Content.ReadAsStringAsync();
            var formData = new Dictionary<string, string>
            {
                { "__VIEWSTATE", ExtractField(content, "__VIEWSTATE") },
                { "__VIEWSTATEGENERATOR", ExtractField(content, "__VIEWSTATEGENERATOR") },
                { "__EVENTVALIDATION", ExtractField(content, "__EVENTVALIDATION") },
                { "btnHttp500", "Simulate HTTP 500" }
            };
            
            // Click the button to trigger the 500 error
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_1.aspx", true, formData);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[{DateTime.Now}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        private string ExtractField(string content, string fieldId)
        {
            var match = Regex.Match(content, $@"id=""{fieldId}"" value=""([^""]+)""");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        [Test]
        [Order(4)]
        public async Task TestSlowResponseScenario()
        {
            Console.WriteLine($"[{DateTime.Now}] Starting Slow Response scenario test...");
            
            // Measure response time directly
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowResponse/SlowResponse1.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger slow response scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"[{DateTime.Now}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            Console.WriteLine($"[{DateTime.Now}] Slow Response scenario test completed successfully");
        }
    }
} 