using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Start a background thread that will cause a stack overflow
            Thread thread = new Thread(CauseStackOverflow);
            thread.Start();

            // Let it run for a short time
            Thread.Sleep(1000);
        }

        private void CauseStackOverflow()
        {
            // Simulate some work
            Thread.Sleep(500);

            // Call recursive method that will cause stack overflow
            RecursiveMethod(0);
        }

        private void RecursiveMethod(int depth)
        {
            // Create a large array on the stack
            byte[] buffer = new byte[1024];

            // Recursive call without termination condition
            RecursiveMethod(depth + 1);
        }
    }
} 