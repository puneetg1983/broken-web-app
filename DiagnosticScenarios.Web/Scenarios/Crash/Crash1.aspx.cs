using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate crash...";
            }
        }

        protected void btnStartCrash_Click(object sender, EventArgs e)
        {
            Response.Redirect("Crash1Actual.aspx");
        }
    }
} 