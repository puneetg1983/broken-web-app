using System;

namespace DiagnosticScenarios.Web.Scenarios.RuntimeVersion
{
    public partial class RuntimeVersion1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnTriggerError_Click(object sender, EventArgs e)
        {
            Response.Redirect("RuntimeVersion1Actual.aspx");
        }
    }
} 