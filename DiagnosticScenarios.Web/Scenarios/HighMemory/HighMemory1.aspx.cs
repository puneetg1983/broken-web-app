using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory1 : System.Web.UI.Page
    {
        private static readonly List<byte[]> _memoryHolders = new List<byte[]>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to start memory-intensive operation.";
            }
        }

        protected void btnStartHighMemory_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Starting memory-intensive operation...";
            
            // Start the memory-intensive operation asynchronously
            Task.Run(() => PerformMemoryIntensiveOperation());
            
            lblStatus.Text = "Memory-intensive operation started. Check Task Manager to see memory usage.";
        }

        private void PerformMemoryIntensiveOperation()
        {
            try
            {
                // Allocate large arrays and keep them in memory
                for (int i = 0; i < 100; i++)
                {
                    // Allocate 100MB of memory
                    byte[] largeArray = new byte[100 * 1024 * 1024];
                    
                    // Fill the array with some data to ensure it's not optimized away
                    for (int j = 0; j < largeArray.Length; j++)
                    {
                        largeArray[j] = (byte)(j % 256);
                    }
                    
                    // Keep the array in memory
                    _memoryHolders.Add(largeArray);
                    
                    // Add a small delay to make the memory growth visible
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                System.Diagnostics.Debug.WriteLine($"Error in memory-intensive operation: {ex.Message}");
            }
        }
    }
} 