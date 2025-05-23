using System;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start long-running task.";
            }
        }

        protected void btnStartLongTask_Click(object sender, EventArgs e)
        {
            Response.Redirect("SlowResponse3Actual.aspx");
        }
    }
} 