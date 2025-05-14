using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency1 : Page
    {
        private static readonly HttpClient client = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate slow dependency call...";
            }
        }

        protected async void btnStartSlowDependency_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartSlowDependency.Enabled = false;
                progress.Visible = true;
                lblStatus.Text = "Starting slow dependency call...";

                // Using HTTPBIN.org's delay endpoint to simulate slow response
                // This will make the server wait for 5 seconds before responding
                string url = "https://httpbin.org/delay/5";
                
                lblStatus.Text = "Calling external service...";
                var response = await client.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "External service call completed successfully.";
                }
                else
                {
                    lblStatus.Text = $"External service call failed with status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartSlowDependency.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 