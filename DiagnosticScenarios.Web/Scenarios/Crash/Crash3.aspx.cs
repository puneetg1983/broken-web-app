using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash3 : Page
    {
        private static readonly List<byte[]> _memoryHog = new List<byte[]>();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate out of memory...";
            }
        }

        protected void btnStartOutOfMemory_Click(object sender, EventArgs e)
        {
            Response.Redirect("Crash3Actual.aspx");
        }

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }
    }
} 