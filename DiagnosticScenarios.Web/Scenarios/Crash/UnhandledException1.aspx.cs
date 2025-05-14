using System;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class UnhandledException1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No special initialization needed
        }

        protected void btnTriggerException_Click(object sender, EventArgs e)
        {
            // Trigger an unhandled exception by accessing a null reference
            string nullString = null;
            int length = nullString.Length; // This will throw a NullReferenceException
        }
    }
} 