using Microsoft.AspNetCore.Mvc;

namespace DiagnosticScenariosCore.Web.Controllers
{
    public class ScenariosController : Controller
    {
        private readonly ILogger<ScenariosController> _logger;

        public ScenariosController(ILogger<ScenariosController> logger)
        {
            _logger = logger;
        }

        // Crash Scenarios
        public IActionResult Crash1()
        {
            return View("Crash/Crash1");
        }

        public IActionResult Crash1Actual()
        {
            // Start a background thread that will throw an unhandled exception
            var thread = new Thread(ThrowUnhandledException);
            thread.Start();

            // Let it run for a short time
            Thread.Sleep(1000);

            return View("Crash/Crash1Actual");
        }

        public IActionResult Crash2()
        {
            return View("Crash/Crash2");
        }

        public IActionResult Crash2Actual()
        {
            // Simulate a different type of crash
            var thread = new Thread(() =>
            {
                Thread.Sleep(500);
                throw new DivideByZeroException("Division by zero crash simulation");
            });
            thread.Start();

            Thread.Sleep(1000);
            return View("Crash/Crash2Actual");
        }

        public IActionResult StackOverflow1()
        {
            return View("Crash/StackOverflow1");
        }

        public IActionResult StackOverflow1Actual()
        {
            // Simulate stack overflow
            var thread = new Thread(CauseStackOverflow);
            thread.Start();

            Thread.Sleep(1000);
            return View("Crash/StackOverflow1Actual");
        }

        public IActionResult UnhandledException1()
        {
            return View("Crash/UnhandledException1");
        }

        public IActionResult UnhandledException1Actual()
        {
            // Simulate unhandled exception
            var thread = new Thread(() =>
            {
                Thread.Sleep(500);
                throw new InvalidOperationException("Unhandled exception simulation");
            });
            thread.Start();

            Thread.Sleep(1000);
            return View("Crash/UnhandledException1Actual");
        }

        // High CPU Scenarios
        public IActionResult HighCpu1()
        {
            return View("HighCpu/HighCpu1");
        }

        public IActionResult HighCpu1Actual()
        {
            RunCpuIntensiveOperations();
            return View("HighCpu/HighCpu1Actual");
        }

        public IActionResult HighCpu2()
        {
            return View("HighCpu/HighCpu2");
        }

        public IActionResult HighCpu2Actual()
        {
            RunCpuIntensiveOperations();
            return View("HighCpu/HighCpu2Actual");
        }

        public IActionResult HighCpu3()
        {
            return View("HighCpu/HighCpu3");
        }

        public IActionResult HighCpu3Actual()
        {
            RunCpuIntensiveOperations();
            return View("HighCpu/HighCpu3Actual");
        }

        // High Memory Scenarios
        public IActionResult HighMemory1()
        {
            return View("HighMemory/HighMemory1");
        }

        public IActionResult HighMemory1Actual()
        {
            ConsumeMemory();
            return View("HighMemory/HighMemory1Actual");
        }

        public IActionResult HighMemory2()
        {
            return View("HighMemory/HighMemory2");
        }

        public IActionResult HighMemory2Actual()
        {
            ConsumeMemory();
            return View("HighMemory/HighMemory2Actual");
        }

        public IActionResult HighMemory3()
        {
            return View("HighMemory/HighMemory3");
        }

        public IActionResult HighMemory3Actual()
        {
            ConsumeMemory();
            return View("HighMemory/HighMemory3Actual");
        }

        // High Connections Scenarios
        public IActionResult HighConnections1()
        {
            return View("HighConnections/HighConnections1");
        }

        public IActionResult HighConnections1Actual()
        {
            // Simulate high connection usage
            return View("HighConnections/HighConnections1Actual");
        }

        // Deadlock Scenarios
        public IActionResult Deadlock1()
        {
            return View("Deadlock/Deadlock1");
        }

        public IActionResult Deadlock1Actual()
        {
            CreateDeadlock();
            return View("Deadlock/Deadlock1Actual");
        }

        // HTTP 500 Scenarios
        public IActionResult Http500_1()
        {
            return View("Http500/Http500_1");
        }

        public IActionResult Http500_1Actual()
        {
            throw new InvalidOperationException("Simulated HTTP 500 error");
        }

        public IActionResult Http500_2()
        {
            return View("Http500/Http500_2");
        }

        public IActionResult Http500_2Actual()
        {
            throw new ArgumentException("Simulated HTTP 500 error - Argument Exception");
        }

        public IActionResult Http500_3()
        {
            return View("Http500/Http500_3");
        }

        public IActionResult Http500_3Actual()
        {
            throw new NullReferenceException("Simulated HTTP 500 error - Null Reference");
        }

        public IActionResult Http500_4()
        {
            return View("Http500/Http500_4");
        }

        public IActionResult Http500_4Actual()
        {
            throw new IndexOutOfRangeException("Simulated HTTP 500 error - Index Out of Range");
        }

        // Slow Response Scenarios
        public IActionResult SlowResponse1()
        {
            return View("SlowResponse/SlowResponse1");
        }

        public IActionResult SlowResponse1Actual()
        {
            Thread.Sleep(30000); // 30 seconds
            return View("SlowResponse/SlowResponse1Actual");
        }

        public IActionResult SlowResponse2()
        {
            return View("SlowResponse/SlowResponse2");
        }

        public IActionResult SlowResponse2Actual()
        {
            Thread.Sleep(60000); // 60 seconds
            return View("SlowResponse/SlowResponse2Actual");
        }

        public IActionResult SlowResponse3()
        {
            return View("SlowResponse/SlowResponse3");
        }

        public IActionResult SlowResponse3Actual()
        {
            Thread.Sleep(120000); // 2 minutes
            return View("SlowResponse/SlowResponse3Actual");
        }

        // Slow Database Scenarios
        public IActionResult SlowDatabase1()
        {
            return View("SlowDatabase/SlowDatabase1");
        }

        public IActionResult SlowDatabase1Actual()
        {
            // Simulate slow database operation
            Thread.Sleep(45000); // 45 seconds
            return View("SlowDatabase/SlowDatabase1Actual");
        }

        public IActionResult SlowDatabase2()
        {
            return View("SlowDatabase/SlowDatabase2");
        }

        public IActionResult SlowDatabase2Actual()
        {
            // Simulate slow database operation
            Thread.Sleep(90000); // 90 seconds
            return View("SlowDatabase/SlowDatabase2Actual");
        }

        // Slow Dependency Scenarios
        public IActionResult SlowDependency1()
        {
            return View("SlowDependency/SlowDependency1");
        }

        public IActionResult SlowDependency1Actual()
        {
            // Simulate slow external dependency
            Thread.Sleep(40000); // 40 seconds
            return View("SlowDependency/SlowDependency1Actual");
        }

        public IActionResult SlowDependency2()
        {
            return View("SlowDependency/SlowDependency2");
        }

        public IActionResult SlowDependency2Actual()
        {
            // Simulate slow external dependency
            Thread.Sleep(80000); // 80 seconds
            return View("SlowDependency/SlowDependency2Actual");
        }

        // Missing Dependency Scenarios
        public IActionResult MissingDependency1()
        {
            return View("MissingDependency/MissingDependency1");
        }

        public IActionResult MissingDependency1Actual()
        {
            // Simulate missing dependency
            throw new DllNotFoundException("Required dependency not found");
        }

        // Out of Memory Scenarios
        public IActionResult OutOfMemory1()
        {
            return View("OutOfMemory/OutOfMemory1");
        }

        public IActionResult OutOfMemory1Actual()
        {
            ConsumeAllMemory();
            return View("OutOfMemory/OutOfMemory1Actual");
        }

        // Thread Leak Scenarios
        public IActionResult ThreadLeak1()
        {
            return View("ThreadLeak/ThreadLeak1");
        }

        public IActionResult ThreadLeak1Actual()
        {
            CreateThreadLeak();
            return View("ThreadLeak/ThreadLeak1Actual");
        }

        // Connection Pool Scenarios
        public IActionResult ConnectionPool1()
        {
            return View("ConnectionPool/ConnectionPool1");
        }

        public IActionResult ConnectionPool1Actual()
        {
            // Simulate connection pool exhaustion
            return View("ConnectionPool/ConnectionPool1Actual");
        }

        public IActionResult ConnectionPool2()
        {
            return View("ConnectionPool/ConnectionPool2");
        }

        public IActionResult ConnectionPool2Actual()
        {
            // Simulate connection pool exhaustion
            return View("ConnectionPool/ConnectionPool2Actual");
        }

        public IActionResult ConnectionPool3()
        {
            return View("ConnectionPool/ConnectionPool3");
        }

        public IActionResult ConnectionPool3Actual()
        {
            // Simulate connection pool exhaustion
            return View("ConnectionPool/ConnectionPool3Actual");
        }

        // Storage Quota Scenarios
        public IActionResult StorageQuota1()
        {
            return View("StorageQuota/StorageQuota1");
        }

        public IActionResult StorageQuota1Actual()
        {
            // Simulate storage quota exceeded
            return View("StorageQuota/StorageQuota1Actual");
        }

        public IActionResult StorageQuotaCleanup()
        {
            // Simulate cleanup
            return View("StorageQuota/StorageQuotaCleanup");
        }

        // Runtime Version Scenarios
        public IActionResult RuntimeVersion1()
        {
            return View("RuntimeVersion/RuntimeVersion1");
        }

        public IActionResult RuntimeVersion1Actual()
        {
            // Simulate runtime version issues
            return View("RuntimeVersion/RuntimeVersion1Actual");
        }

        // Private helper methods
        private void ThrowUnhandledException()
        {
            Thread.Sleep(500);
            throw new InvalidOperationException("This is an unhandled exception that will crash the application.");
        }

        private void CauseStackOverflow()
        {            
            CauseStackOverflow(); // Recursive call to cause stack overflow
        }

        private void RunCpuIntensiveOperations()
        {
            var threads = new Thread[Environment.ProcessorCount * 2];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var endTime = DateTime.Now.AddSeconds(60);
                    while (DateTime.Now < endTime)
                    {
                        for (int j = 0; j < 10000000; j++)
                        {
                            var result = Math.Sqrt(j) * Math.Sin(j) * Math.Cos(j) * Math.Tan(j);
                            result = Math.Pow(result, 2) + Math.Log(Math.Abs(result) + 1);
                            
                            if (result > 1000000)
                            {
                                result = Math.Sqrt(result);
                            }
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private void ConsumeMemory()
        {
            var memoryList = new List<byte[]>();
            try
            {
                while (true)
                {
                    memoryList.Add(new byte[1024 * 1024]); // 1MB chunks
                    Thread.Sleep(100);
                }
            }
            catch (OutOfMemoryException)
            {
                // Expected when memory is exhausted
            }
        }

        private void CreateDeadlock()
        {
            var lock1 = new object();
            var lock2 = new object();

            var thread1 = new Thread(() =>
            {
                lock (lock1)
                {
                    Thread.Sleep(1000);
                    lock (lock2)
                    {
                        // This will never be reached due to deadlock
                    }
                }
            });

            var thread2 = new Thread(() =>
            {
                lock (lock2)
                {
                    Thread.Sleep(1000);
                    lock (lock1)
                    {
                        // This will never be reached due to deadlock
                    }
                }
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
        }

        private void ConsumeAllMemory()
        {
            var memoryList = new List<byte[]>();
            try
            {
                while (true)
                {
                    memoryList.Add(new byte[1024 * 1024 * 100]); // 100MB chunks
                }
            }
            catch (OutOfMemoryException)
            {
                // Expected when memory is exhausted
            }
        }

        private void CreateThreadLeak()
        {
            for (int i = 0; i < 1000; i++)
            {
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        // Thread never exits
                    }
                });
                thread.Start();
            }
        }
    }
} 