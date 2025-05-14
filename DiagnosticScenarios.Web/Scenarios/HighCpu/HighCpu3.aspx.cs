using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu3 : Page
    {
        private static readonly object _lockA = new object();
        private static readonly object _lockB = new object();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate deadlock...";
            }
        }

        protected void btnStartDeadlock_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartDeadlock.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Start two threads that will deadlock
                Thread thread1 = new Thread(DeadlockWorker1);
                Thread thread2 = new Thread(DeadlockWorker2);

                thread1.Start();
                thread2.Start();

                lblStatus.Text = "Started deadlock scenario. Two threads are now deadlocked.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartDeadlock.Enabled = true;
                progress.Visible = false;
            }
        }

        private void DeadlockWorker1()
        {
            while (_isRunning)
            {
                try
                {
                    lock (_lockA)
                    {
                        // Simulate some work
                        Thread.Sleep(100);

                        // Try to acquire lock B while holding lock A
                        lock (_lockB)
                        {
                            // This will never be reached due to deadlock
                            for (int i = 0; i < 1000000; i++)
                            {
                                Math.Sqrt(i);
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private void DeadlockWorker2()
        {
            while (_isRunning)
            {
                try
                {
                    lock (_lockB)
                    {
                        // Simulate some work
                        Thread.Sleep(100);

                        // Try to acquire lock A while holding lock B
                        lock (_lockA)
                        {
                            // This will never be reached due to deadlock
                            for (int i = 0; i < 1000000; i++)
                            {
                                Math.Sqrt(i);
                            }
                        }
                    }
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