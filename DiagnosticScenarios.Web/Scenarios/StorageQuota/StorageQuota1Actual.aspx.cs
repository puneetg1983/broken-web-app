using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.StorageQuota
{
    public partial class StorageQuota1Actual : System.Web.UI.Page
    {
        private const int FileSizeMB = 10; // Size of each file in MB
        private const int BufferSize = 1024 * 1024; // 1MB buffer
        private static readonly string TempPath = Path.GetTempPath();
        private static readonly string FilePrefix = "StorageQuotaTest_";

        protected void Page_Load(object sender, EventArgs e)
        {
            int fileCount = 0;
            try
            {
                lblStatus.Text = "Starting storage quota exceeded simulation...";
                Response.Flush();

                // Create files until we hit the quota
                while (true)
                {
                    string fileName = Path.Combine(TempPath, $"{FilePrefix}{fileCount}.dat");
                    CreateLargeFile(fileName, FileSizeMB);
                    fileCount++;

                    lblStatus.Text = $"Created {fileCount} files ({fileCount * FileSizeMB} MB total)...";
                    Response.Flush();
                }
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("There is not enough space on the disk"))
                {
                    // Throw a new exception with the status message to ensure proper error propagation
                    throw new Exception($"Storage quota exceeded! Created {fileCount} files before running out of space.", ex);
                }
                else
                {
                    throw new Exception($"Error: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        private void CreateLargeFile(string filePath, int sizeMB)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[BufferSize];
                for (int i = 0; i < sizeMB; i++)
                {
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
} 