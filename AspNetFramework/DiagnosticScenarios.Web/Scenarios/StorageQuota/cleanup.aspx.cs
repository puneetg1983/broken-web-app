using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.StorageQuota
{
    public partial class cleanup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string tempPath = Path.GetTempPath();
                var datFiles = Directory.GetFiles(tempPath, "*.dat");
                int deletedCount = 0;

                foreach (var file in datFiles)
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with other files
                        System.Diagnostics.Debug.WriteLine($"Error deleting file {file}: {ex.Message}");
                    }
                }

                Response.Write($"Successfully deleted {deletedCount} .dat files from {tempPath}");
            }
            catch (Exception ex)
            {
                Response.Write($"Error during cleanup: {ex.Message}");
            }
        }
    }
} 