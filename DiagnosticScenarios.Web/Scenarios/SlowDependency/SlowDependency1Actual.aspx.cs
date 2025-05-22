using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Simulate a slow API call
                using (var client = new HttpClient())
                {
                    // Set a timeout of 30 seconds
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Make a request to a non-existent endpoint
                    var response = client.GetAsync("http://non-existent-api.com/slow-endpoint").Result;
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"API error: {ex.Message}");
                throw;
            }
        }
    }
} 