using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.StorageQuota
{
    public partial class StorageQuota1Actual : System.Web.UI.Page
    {
        private const int FileSizeMB = 1000; // Size of each file in MB
        private const int BufferSize = 5 * 1024 * 1024; // 5MB buffer
        private static readonly string TempPath = Path.GetTempPath();
        private static readonly string FilePrefix = "StorageQuotaTest_";
        private const int ConcurrentTasks = 100; // Number of concurrent file creation tasks

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateFilesUntilQuotaExceeded();
        }

        private void CreateFilesUntilQuotaExceeded()
        {
            int fileCount = 0;

            // Create a batch of concurrent tasks
            for (int i = 0; i < ConcurrentTasks; i++)
            {
                string fileName = Path.Combine(TempPath, $"{FilePrefix}{fileCount}.dat");
                CreateLargeFileAsync(fileName, FileSizeMB);
            }
        }

        private void CreateLargeFileAsync(string filePath, int sizeMB)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
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