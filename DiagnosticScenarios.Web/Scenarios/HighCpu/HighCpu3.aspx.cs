using System;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu3 : Page
    {
        private static readonly object _lockA = new object();
        private static readonly object _lockB = new object();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate deadlock...";
            }
        }

        protected void btnStartHighCpu_Click(object sender, EventArgs e)
        {
            Response.Redirect("HighCpu3Actual.aspx");
        }
    }
}