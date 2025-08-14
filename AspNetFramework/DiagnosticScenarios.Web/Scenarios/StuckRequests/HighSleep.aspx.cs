using System;

namespace DiagnosticScenarios.Web.Scenarios.StuckRequests
{
    public partial class HighSleep : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start high sleep operation. Warning: This will create a stuck request for 300 seconds.";
            }
        }

        protected void btnStartHighSleep_Click(object sender, EventArgs e)
        {
            Response.Redirect("HighSleepActual.aspx");
        }
    }
}
