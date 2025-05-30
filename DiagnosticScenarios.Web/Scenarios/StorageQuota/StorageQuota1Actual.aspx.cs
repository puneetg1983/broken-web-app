using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.StorageQuota
{
    public partial class StorageQuota1Actual : System.Web.UI.Page
    {
        private const int FileSizeMB = 100; // Size of each file in MB
        private const int BufferSize = 5 * 1024 * 1024; // 5MB buffer
        private static readonly string TempPath = Path.GetTempPath();
        private static readonly string FilePrefix = "StorageQuotaTest_";
        private const int ConcurrentTasks = 20; // Number of concurrent file creation tasks

        protected void Page_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Starting storage quota exceeded simulation...";

            var task = new PageAsyncTask(async (ct) =>
            {
                await CreateFilesUntilQuotaExceeded();
            });

            RegisterAsyncTask(task);
            ExecuteRegisteredAsyncTasks();
        }

        private async Task CreateFilesUntilQuotaExceeded()
        {
            int fileCount = 0;
            var tasks = new List<Task>();

            while (true)
            {
                // Create a batch of concurrent tasks
                for (int i = 0; i < ConcurrentTasks; i++)
                {
                    string fileName = Path.Combine(TempPath, $"{FilePrefix}{fileCount}.dat");
                    tasks.Add(CreateLargeFileAsync(fileName, FileSizeMB));
                    fileCount++;
                }

                // Wait for all tasks in the current batch to complete
                await Task.WhenAll(tasks);
                tasks.Clear();

                lblStatus.Text = $"Created {fileCount} files ({fileCount * FileSizeMB} MB total)...";
            }
        }

        private async Task CreateLargeFileAsync(string filePath, int sizeMB)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                byte[] buffer = new byte[BufferSize];
                for (int i = 0; i < sizeMB; i++)
                {
                    await fs.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }
    }
} 