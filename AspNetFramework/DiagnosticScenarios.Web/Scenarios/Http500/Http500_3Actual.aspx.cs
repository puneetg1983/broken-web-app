using System;
using System.Configuration;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_3Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Attempt to access a non-existent configuration setting
                string nonExistentSetting = ConfigurationManager.AppSettings["NonExistentSetting"];
                
                // This will throw a ConfigurationErrorsException
                if (string.IsNullOrEmpty(nonExistentSetting))
                {
                    throw new ConfigurationErrorsException("Required configuration setting 'NonExistentSetting' is missing.");
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow to trigger HTTP 500
                System.Diagnostics.Debug.WriteLine($"Configuration error: {ex.Message}");
                Response.StatusCode = 500;
                Response.StatusDescription = "Internal Server Error";
                Response.Write($"<h1>Internal Server Error</h1><p>Configuration Error: {ex.Message}</p>");
                Response.End();
            }
        }
    }
} 