using System;
using System.IO;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_2 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate file access error...";
            }
        }

        protected void btnFileError_Click(object sender, EventArgs e)
        {
            try
            {
                btnFileError.Enabled = false;
                progress.Visible = true;

                // Attempt to access a file in a restricted directory
                string restrictedPath = @"C:\Windows\System32\config\SAM";
                
                using (FileStream fs = File.Open(restrictedPath, FileMode.Open, FileAccess.Read))
                {
                    // This will never be reached due to access denied
                    byte[] buffer = new byte[1024];
                    fs.Read(buffer, 0, buffer.Length);
                }

                lblStatus.Text = "File access successful (this should not happen)";
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"File access error: {ex.Message}");
                
                // Throw a new exception to cause HTTP 500
                throw new Exception("Access to the requested file was denied. Please contact your system administrator.", ex);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnFileError.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 