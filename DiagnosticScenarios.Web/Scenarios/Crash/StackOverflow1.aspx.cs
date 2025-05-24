using System;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class StackOverflow1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No special initialization needed
        }

        protected void btnTriggerActual_Click(object sender, EventArgs e)
        {
            Response.Redirect("StackOverflow1Actual.aspx");
        }
    }
} 