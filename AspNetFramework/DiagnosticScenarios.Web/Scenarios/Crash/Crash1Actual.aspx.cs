using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Start a background thread that will throw an unhandled exception
            Thread thread = new Thread(ThrowUnhandledException);
            thread.Start();

            // Let it run for a short time
            Thread.Sleep(1000);
        }

        private void ThrowUnhandledException()
        {
            // Simulate some work
            Thread.Sleep(500);

            // Throw an unhandled exception
            throw new InvalidOperationException("This is an unhandled exception that will crash the application.");
        }
    }
} 