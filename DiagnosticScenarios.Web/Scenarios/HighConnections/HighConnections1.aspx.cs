using System;

namespace DiagnosticScenarios.Web.Scenarios.HighConnections
{
    public partial class HighConnections1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate high outbound connections. Click the button to start.";
            }
        }

        protected void btnTriggerConnections_Click(object sender, EventArgs e)
        {
            Response.Redirect("HighConnections1Actual.aspx");
        }
    }
} 