using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Run CPU-intensive operations for 30 seconds
            RunCpuIntensiveOperations();
        }

        private void RunCpuIntensiveOperations()
        {
            // Create multiple threads to maximize CPU usage
            var threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    var endTime = DateTime.Now.AddSeconds(30);
                    while (DateTime.Now < endTime)
                    {
                        // Perform CPU-intensive calculations
                        for (int j = 0; j < 1000000; j++)
                        {
                            Math.Sqrt(j) * Math.Sin(j) * Math.Cos(j);
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