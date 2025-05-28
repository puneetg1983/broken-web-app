using System;

namespace DiagnosticScenarios.Web.Scenarios.RuntimeVersion
{
    public partial class RuntimeVersion1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate incompatible runtime version. Click the button to start.";
            }
        }

        protected void btnTriggerError_Click(object sender, EventArgs e)
        {
            Response.Redirect("RuntimeVersion1Actual.aspx");
        }
    }
} 