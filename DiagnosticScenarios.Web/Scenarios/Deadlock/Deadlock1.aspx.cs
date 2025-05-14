using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.Deadlock
{
    public partial class Deadlock1 : System.Web.UI.Page
    {
        private static readonly object _lock1 = new object();
        private static readonly object _lock2 = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start deadlock simulation.";
            }
        }

        protected void btnStartDeadlock_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Starting deadlock simulation...";
            progress.Visible = true;
            
            // Start the deadlock simulation
            Task.Run(() => PerformDeadlockSimulation());
            
            lblStatus.Text = "Deadlock simulation started. Check Application Insights for deadlock detection.";
        }

        private void PerformDeadlockSimulation()
        {
            try
            {
                // Create two tasks that will deadlock
                var task1 = Task.Run(() =>
                {
                    lock (_lock1)
                    {
                        Thread.Sleep(100); // Simulate some work
                        lock (_lock2)
                        {
                            // This will never be reached due to deadlock
                            lblStatus.Text = "Task 1 completed";
                        }
                    }
                });

                var task2 = Task.Run(() =>
                {
                    lock (_lock2)
                    {
                        Thread.Sleep(100); // Simulate some work
                        lock (_lock1)
                        {
                            // This will never be reached due to deadlock
                            lblStatus.Text = "Task 2 completed";
                        }
                    }
                });

                // Wait for both tasks to complete (they won't)
                Task.WaitAll(task1, task2);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                progress.Visible = false;
            }
        }
    }
} 