using System;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class StackOverflow1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CauseStackOverflow();
        }

        private void CauseStackOverflow()
        {
            // Recursive call to cause stack overflow
            CauseStackOverflow();
        }
    }
} 