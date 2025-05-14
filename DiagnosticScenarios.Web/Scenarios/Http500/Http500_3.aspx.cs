using System;
using System.Configuration;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_3 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate configuration error...";
            }
        }

        protected void btnConfigError_Click(object sender, EventArgs e)
        {
            try
            {
                btnConfigError.Enabled = false;
                progress.Visible = true;

                // Attempt to access a non-existent configuration setting
                string nonExistentSetting = ConfigurationManager.AppSettings["NonExistentSetting"];
                
                if (string.IsNullOrEmpty(nonExistentSetting))
                {
                    // This is expected, but we'll throw an exception to simulate a configuration error
                    throw new ConfigurationErrorsException("Required configuration setting 'NonExistentSetting' is missing.");
                }

                lblStatus.Text = "Configuration access successful (this should not happen)";
            }
            catch (ConfigurationErrorsException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Configuration error: {ex.Message}");
                
                // Throw a new exception to cause HTTP 500
                throw new Exception("The application configuration is invalid. Please contact your system administrator.", ex);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnConfigError.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 