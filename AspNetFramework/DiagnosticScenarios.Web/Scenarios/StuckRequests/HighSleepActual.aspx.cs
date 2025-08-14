using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.StuckRequests
{
    public partial class HighSleepActual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Simulate a stuck request by sleeping for 300 seconds (5 minutes)
            Thread.Sleep(300000);
        }
    }
}
