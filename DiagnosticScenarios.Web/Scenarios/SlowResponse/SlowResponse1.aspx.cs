using System;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start slow response operation.";
            }
        }

        protected void btnStartSlowResponse_Click(object sender, EventArgs e)
        {
            Response.Redirect("SlowResponse1Actual.aspx");
        }
    }
} 