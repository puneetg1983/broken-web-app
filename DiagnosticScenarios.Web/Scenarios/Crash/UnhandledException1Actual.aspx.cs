using System;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class UnhandledException1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new InvalidOperationException("This is an unhandled exception from the Actual page.");
        }
    }
} 