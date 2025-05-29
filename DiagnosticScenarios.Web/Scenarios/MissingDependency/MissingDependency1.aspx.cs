using System;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.MissingDependency
{
    public partial class MissingDependency1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {        
        }

        protected void btnTriggerError_Click(object sender, EventArgs e)
        {
            Response.Redirect("MissingDependency1Actual.aspx");
        }
    }
} 