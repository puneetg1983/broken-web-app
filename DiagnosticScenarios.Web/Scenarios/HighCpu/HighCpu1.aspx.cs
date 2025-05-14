using System;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start CPU-intensive operation.";
            }
        }

        protected void btnStartHighCpu_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Starting CPU-intensive operation...";
            
            // Start the CPU-intensive operation asynchronously
            Task.Run(() => PerformCpuIntensiveOperation());
            
            lblStatus.Text = "CPU-intensive operation started. Check Task Manager to see CPU usage.";
        }

        private void PerformCpuIntensiveOperation()
        {
            try
            {
                // Perform CPU-intensive calculations
                for (int i = 0; i < 1000000; i++)
                {
                    // Complex mathematical operations
                    double result = 0;
                    for (int j = 0; j < 1000; j++)
                    {
                        result += Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2));
                        result *= Math.Sin(result) * Math.Cos(result);
                        result = Math.Log(Math.Abs(result) + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                System.Diagnostics.Debug.WriteLine($"Error in CPU-intensive operation: {ex.Message}");
            }
        }
    }
} 