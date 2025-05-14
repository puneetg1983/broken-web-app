using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency2 : Page
    {
        private static readonly HttpClient client = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate timeout...";
            }
        }

        protected async void btnStartTimeout_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartTimeout.Enabled = false;
                progress.Visible = true;
                lblStatus.Text = "Starting timeout test...";

                // Using HTTPBIN.org's delay endpoint with a 10-second delay
                // but our client will timeout after 3 seconds
                string url = "https://httpbin.org/delay/10";
                
                // Set a short timeout
                client.Timeout = TimeSpan.FromSeconds(3);
                
                lblStatus.Text = "Calling external service (will timeout in 3 seconds)...";
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
            catch (TaskCanceledException)
            {
                lblStatus.Text = "Request timed out after 3 seconds.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartTimeout.Enabled = true;
                progress.Visible = false;
                // Reset timeout to default
                client.Timeout = TimeSpan.FromSeconds(100);
            }
        }
    }
} 