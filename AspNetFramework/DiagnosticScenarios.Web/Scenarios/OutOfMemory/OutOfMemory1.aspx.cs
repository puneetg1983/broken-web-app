using System;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.OutOfMemory
{
    public partial class OutOfMemory1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate out of memory scenario...";
            }
        }

        protected void btnOutOfMemory_Click(object sender, EventArgs e)
        {
            Response.Redirect("OutofMemory1Actual.aspx");
        }
    }
} 