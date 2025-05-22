using System;
using System.IO;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowDependency
{
    public partial class SlowDependency2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Simulate a slow file system operation
                string tempPath = Path.GetTempPath();
                string testFile = Path.Combine(tempPath, "slow_file_test.txt");

                // Create a large file
                using (var writer = new StreamWriter(testFile))
                {
                    // Write a large amount of data
                    for (int i = 0; i < 1000000; i++)
                    {
                        writer.WriteLine($"Line {i}: This is a test line to simulate slow file system operations.");
                    }
                }

                // Simulate slow file reading
                using (var reader = new StreamReader(testFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Process each line slowly
                        Thread.Sleep(1);
                    }
                }

                // Clean up
                File.Delete(testFile);
            }
            catch (IOException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"File system error: {ex.Message}");
                throw;
            }
        }
    }
} 