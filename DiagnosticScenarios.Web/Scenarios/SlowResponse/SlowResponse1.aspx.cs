using System;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start slow response operation.";
            }
        }

        protected void btnStartSlowResponse_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Starting slow response operation...";
            progress.Visible = true;
            
            // Start the slow response operation
            PerformSlowResponseOperation();
        }

        private void PerformSlowResponseOperation()
        {
            try
            {
                // Simulate a slow response by introducing delays
                for (int i = 0; i < 10; i++)
                {
                    // Simulate some work
                    System.Threading.Thread.Sleep(1000); // 1 second delay
                    
                    // Update status
                    lblStatus.Text = $"Processing step {i + 1} of 10...";
                }
                
                // Final delay to simulate completion
                System.Threading.Thread.Sleep(2000);
                
                lblStatus.Text = "Slow response operation completed.";
                progress.Visible = false;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                progress.Visible = false;
            }
        }
    }
} 