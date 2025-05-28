using System;
using System.Configuration;
using System.Threading;

namespace DiagnosticScenarios.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Check for hanging on startup
            string hangOnStartup = ConfigurationManager.AppSettings["WEBAPP_HUNG_ON_STARTUP"];
            if (!string.IsNullOrEmpty(hangOnStartup) && hangOnStartup.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                // Hang indefinitely
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }

            // Check for crash on startup
            string crashOnStartup = ConfigurationManager.AppSettings["WEBAPP_CRASH_ON_STARTUP"];
            if (!string.IsNullOrEmpty(crashOnStartup) && crashOnStartup.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                // Throw an unhandled exception
                throw new InvalidOperationException("Application configured to crash on startup");
            }
        }
    }
} 