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
    [Category("HttpResponse")]
    public class HttpResponseScenarioTests
    {
        private const int DEFAULT_METRICS_WAIT_SECONDS = 30;
        private const int DEFAULT_RESTART_WAIT_SECONDS = 30;
        private const int DEFAULT_WARMUP_RETRIES = 10;
        private const int DEFAULT_WARMUP_RETRY_DELAY_SECONDS = 10;

        private ArmMetricsHelper _helper;
        private string _baseUrl;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (!ArmMetricsHelper.ShouldRunTests("HttpResponse"))
            {
                Assert.Ignore("Skipping ARM metrics tests. Set RUN_SPECIALIZED_TESTS=true to run them locally.");
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
            TestContext.Progress.WriteLine($"  SUBSCRIPTION_ID: {ArmMetricsHelper.GetSubscriptionId()}");
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

        [Test]
        [Order(1)]
        public async Task TestHttp500DatabaseConnectionScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting HTTP 500 Database Connection scenario test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_1Actual.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        [Test]
        [Order(2)]
        public async Task TestHttp500FileAccessScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting HTTP 500 File Access scenario test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_2Actual.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        [Test]
        [Order(3)]
        public async Task TestHttp500ConfigurationScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting HTTP 500 Configuration scenario test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_3Actual.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        [Test]
        [Order(4)]
        public async Task TestHttp500InvalidConnectionStringScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting HTTP 500 Invalid Connection String scenario test...");
            
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/Http500/Http500_4Actual.aspx");
            var responseContent = await response.Content.ReadAsStringAsync();
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response content: {responseContent}");
            
            _helper.VerifyHttp500Response(response, responseContent);
        }

        [Test]
        [Order(5)]
        public async Task TestBasicSlowResponseScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Basic Slow Response scenario test...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowResponse/SlowResponse1Actual.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger basic slow response scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Basic Slow Response scenario test completed successfully");
        }

        [Test]
        [Order(6)]
        public async Task TestComplexSlowResponseScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Complex Slow Response scenario test...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowResponse/SlowResponse2Actual.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger complex slow response scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Complex Slow Response scenario test completed successfully");
        }

        [Test]
        [Order(7)]
        public async Task TestSlowApiDependencyScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Slow API Dependency scenario test...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowDependency/SlowDependency1Actual.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger slow API dependency scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Slow API Dependency scenario test completed successfully");
        }

        [Test]
        [Order(8)]
        public async Task TestSlowFileSystemDependencyScenario()
        {
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Starting Slow File System Dependency scenario test...");
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _helper.TriggerScenarioWithResponse("/Scenarios/SlowDependency/SlowDependency2Actual.aspx");
            stopwatch.Stop();
            
            Assert.That(response.IsSuccessStatusCode, Is.True, "Failed to trigger slow file system dependency scenario");
            
            var responseTimeMs = stopwatch.ElapsedMilliseconds;
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Response Time: {responseTimeMs}ms");
            
            if (responseTimeMs <= 1000)
            {
                Assert.Fail($"Response time should be above 1000ms, but was {responseTimeMs}ms");
            }
            
            TestContext.Progress.WriteLine($"[{DateTime.UtcNow}] Slow File System Dependency scenario test completed successfully");
        }
    }
} 