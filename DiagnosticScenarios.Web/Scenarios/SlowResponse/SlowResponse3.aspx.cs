using System;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnStartSlowResponse_Click(object sender, EventArgs e)
        {
            Response.Redirect("SlowResponse3Actual.aspx");
        }
    }
} 