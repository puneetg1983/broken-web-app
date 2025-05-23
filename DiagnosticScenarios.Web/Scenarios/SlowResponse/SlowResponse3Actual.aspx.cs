using System;
using System.Threading.Tasks;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse3Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Simulate multiple long-running operations
            Task.Run(async () =>
            {
                try
                {
                    // First operation - 5 seconds
                    await Task.Delay(5000);
                    
                    // Second operation - 3 seconds
                    await Task.Delay(3000);
                    
                    // Third operation - 2 seconds
                    await Task.Delay(2000);
                    
                    // Update status
                    lblStatus.Text = "All operations completed successfully!";
                }
                catch (Exception ex)
                {
                    lblStatus.Text = $"Error occurred: {ex.Message}";
                }
            });
        }
    }
} 