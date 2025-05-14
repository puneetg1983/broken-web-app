using System;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class StackOverflow1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No special initialization needed
        }

        protected void btnTriggerStackOverflow_Click(object sender, EventArgs e)
        {
            // Trigger a stack overflow by using infinite recursion
            RecursiveMethod(0);
        }

        private void RecursiveMethod(int depth)
        {
            // This will eventually cause a stack overflow
            RecursiveMethod(depth + 1);
        }
    }
} 