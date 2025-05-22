using System;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start CPU-intensive operation.";
            }
        }

        protected void btnStartHighCpu_Click(object sender, EventArgs e)
        {
            Response.Redirect("HighCpu1Actual.aspx");
        }
    }
} 