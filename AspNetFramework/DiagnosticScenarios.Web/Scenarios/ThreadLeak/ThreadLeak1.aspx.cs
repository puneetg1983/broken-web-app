using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.ThreadLeak
{
    public partial class ThreadLeak1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate thread leak...";
            }
        }

        protected void btnStartThreadLeak_Click(object sender, EventArgs e)
        {
            Response.Redirect("ThreadLeak1Actual.aspx");
        }
    }
} 