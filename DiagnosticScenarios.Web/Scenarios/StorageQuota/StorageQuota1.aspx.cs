using System;

namespace DiagnosticScenarios.Web.Scenarios.StorageQuota
{
    public partial class StorageQuota1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate storage quota exceeded scenario.";
            }
        }

        protected void btnTriggerError_Click(object sender, EventArgs e)
        {
            Response.Redirect("StorageQuota1Actual.aspx");
        }
    }
} 