using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Simulate a slow response by waiting for 2 seconds
            Thread.Sleep(2000);
        }
    }
} 