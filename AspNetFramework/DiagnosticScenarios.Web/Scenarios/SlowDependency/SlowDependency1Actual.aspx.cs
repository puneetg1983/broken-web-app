using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                // Simulate a slow API call
                using (var client = new HttpClient())
                {
                    // Set a timeout of 30 seconds
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Make a request to httpbin.org's delay endpoint
                    // This will make the server wait for 5 seconds before responding
                    var response = client.GetAsync("https://httpbin.org/delay/5").Result;
                    response.EnsureSuccessStatusCode();
                }
                sw.Stop();
                Response.Write($"<div>SlowDependency1Actual executed successfully. Elapsed: {sw.ElapsedMilliseconds} ms</div>");
            }
            catch (HttpRequestException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"API error: {ex.Message}");
                sw.Stop();
                Response.Write($"<div style='color:red'>API error: {ex.Message}<br/>Elapsed: {sw.ElapsedMilliseconds} ms</div>");
            }
            catch (Exception ex)
            {
                sw.Stop();
                Response.Write($"<div style='color:red'>Exception: {ex.Message}<br/>{ex.StackTrace}<br/>Elapsed: {sw.ElapsedMilliseconds} ms</div>");
            }
        }
    }
} 