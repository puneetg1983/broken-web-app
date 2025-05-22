using System;
using System.Collections.Generic;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash3Actual : System.Web.UI.Page
    {
        private static List<byte[]> _memoryBlocks = new List<byte[]>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start a background thread that will consume memory
            Thread thread = new Thread(ConsumeMemory);
            thread.Start();

            // Let it run for a short time
            Thread.Sleep(1000);
        }

        private void ConsumeMemory()
        {
            // Simulate some work
            Thread.Sleep(500);

            try
            {
                while (_isRunning)
                {
                    // Allocate a large block of memory (100MB)
                    byte[] block = new byte[100 * 1024 * 1024];
                    _memoryBlocks.Add(block);

                    // Small delay to allow memory pressure to build
                    Thread.Sleep(100);
                }
            }
            catch (OutOfMemoryException)
            {
                // Expected exception
                throw;
            }
        }
    }
} 