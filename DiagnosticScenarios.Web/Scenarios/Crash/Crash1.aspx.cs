using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate crash...";
            }
        }

        protected void btnCrash_Click(object sender, EventArgs e)
        {
            try
            {
                btnCrash.Enabled = false;
                progress.Visible = true;

                // Start a background thread that will throw an unhandled exception
                Thread thread = new Thread(() =>
                {
                    // Simulate some work
                    Thread.Sleep(1000);

                    // Throw an unhandled exception
                    throw new InvalidOperationException("This is an unhandled exception that will crash the application.");
                });

                thread.Start();

                lblStatus.Text = "Started crash scenario. The application will crash in a few seconds.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnCrash.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 