using System;
using System.Collections.Generic;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory1Actual : System.Web.UI.Page
    {
        private static readonly List<byte[]> _memoryLeak = new List<byte[]>();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Allocate 500MB of memory in 5MB chunks with more gradual allocation
                for (int i = 0; i < 50; i++)
                {
                    try
                    {
                        _memoryLeak.Add(new byte[5 * 1024 * 1024]); // 5MB
                        // Fill the array with data to prevent optimization
                        for (int j = 0; j < _memoryLeak[i].Length; j += 1024) // Fill in chunks to reduce CPU usage
                        {
                            _memoryLeak[i][j] = (byte)(j % 256);
                        }
                        Thread.Sleep(50); // Smaller delay between allocations
                    }
                    catch (OutOfMemoryException)
                    {
                        // If we run out of memory, break the loop
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it
                System.Diagnostics.Debug.WriteLine($"Memory allocation error: {ex.Message}");
            }
        }
    }
} 