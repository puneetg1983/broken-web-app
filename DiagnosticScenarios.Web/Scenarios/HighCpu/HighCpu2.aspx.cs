using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu2 : Page
    {
        private static readonly object _lock = new object();
        private static int _threadCount = 0;
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate thread contention...";
            }
        }

        protected void btnStartContention_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartContention.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Create multiple threads that will compete for the lock
                for (int i = 0; i < 10; i++)
                {
                    Thread thread = new Thread(ContentionWorker);
                    thread.Start();
                    _threadCount++;
                }

                lblStatus.Text = $"Started {_threadCount} threads competing for lock. CPU usage will increase.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartContention.Enabled = true;
                progress.Visible = false;
            }
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

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }
    }
} 