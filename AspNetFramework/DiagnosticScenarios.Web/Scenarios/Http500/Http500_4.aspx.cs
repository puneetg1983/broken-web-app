using System;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_4 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate invalid connection string error...";
            }
        }

        protected void btnStartError_Click(object sender, EventArgs e)
        {
            Response.Redirect("Http500_4Actual.aspx");
        }
    }
} 