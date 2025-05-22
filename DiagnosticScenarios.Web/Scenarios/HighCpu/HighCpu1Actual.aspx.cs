using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Run CPU-intensive operations for 60 seconds
            RunCpuIntensiveOperations();
        }

        private void RunCpuIntensiveOperations()
        {
            // Create multiple threads to maximize CPU usage
            var threads = new Thread[Environment.ProcessorCount * 2]; // Double the number of threads
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var endTime = DateTime.Now.AddSeconds(60); // Run for 60 seconds
                    while (DateTime.Now < endTime)
                    {
                        // Perform more CPU-intensive calculations
                        for (int j = 0; j < 10000000; j++) // Increased iterations
                        {
                            // More complex calculations
                            var result = Math.Sqrt(j) * Math.Sin(j) * Math.Cos(j) * Math.Tan(j);
                            result = Math.Pow(result, 2) + Math.Log(Math.Abs(result) + 1);
                            
                            // Use the result to prevent compiler optimization
                            if (result > 1000000)
                            {
                                result = Math.Sqrt(result);
                            }
                        }
                    }
                });
                threads[i].Start();
            }

            // Wait for all threads to complete
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
} 