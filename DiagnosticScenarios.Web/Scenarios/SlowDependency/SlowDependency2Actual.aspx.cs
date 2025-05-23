using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                // Simulate a slow file system operation
                string tempPath = Path.GetTempPath();
                string testFile = Path.Combine(tempPath, "slow_file_test.txt");

                // Create a file with some content
                File.WriteAllText(testFile, "Test content");

                // Simulate slow file operations
                for (int i = 0; i < 10; i++)
                {
                    // Read the file
                    string content = File.ReadAllText(testFile);
                    
                    // Simulate processing
                    Thread.Sleep(500);
                    
                    // Write back to the file
                    File.WriteAllText(testFile, content + $"\nIteration {i}");
                }

                // Clean up
                File.Delete(testFile);
                sw.Stop();
                Response.Write($"<div>SlowDependency2Actual executed successfully. Elapsed: {sw.ElapsedMilliseconds} ms</div>");
            }
            catch (Exception ex)
            {
                sw.Stop();
                Response.Write($"<div style='color:red'>Exception: {ex.Message}<br/>{ex.StackTrace}<br/>Elapsed: {sw.ElapsedMilliseconds} ms</div>");
            }
        }
    }
} 