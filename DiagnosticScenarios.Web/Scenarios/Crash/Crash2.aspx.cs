using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash2 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate stack overflow...";
            }
        }

        protected void btnStackOverflow_Click(object sender, EventArgs e)
        {
            try
            {
                btnStackOverflow.Enabled = false;
                progress.Visible = true;

                // Start a background thread to cause stack overflow
                Thread thread = new Thread(() =>
                {
                    // Start infinite recursion
                    RecursiveMethod(0);
                });

                thread.Start();

                lblStatus.Text = "Started stack overflow scenario. The application will crash in a few seconds.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStackOverflow.Enabled = true;
                progress.Visible = false;
            }
        }

        private void RecursiveMethod(int depth)
        {
            // Create a large array on the stack
            int[] array = new int[1000];

            // Fill the array to use more stack space
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }

            // Recursive call that will eventually cause stack overflow
            RecursiveMethod(depth + 1);
        }
    }
} 