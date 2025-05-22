using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu2Actual : System.Web.UI.Page
    {
        private static readonly object _lock = new object();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Create multiple threads that will compete for the lock
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(ContentionWorker);
                thread.Start();
            }

            // Let the threads run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;
        }

        private void ContentionWorker()
        {
            while (_isRunning)
            {
                try
                {
                    // Try to acquire the lock
                    lock (_lock)
                    {
                        // Simulate some work
                        for (int i = 0; i < 1000000; i++)
                        {
                            Math.Sqrt(i);
                        }
                    }

                    // Small delay to allow other threads to compete
                    Thread.Sleep(10);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }
    }
} 