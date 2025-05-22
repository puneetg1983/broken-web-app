using System;
using System.Collections.Generic;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory2Actual : System.Web.UI.Page
    {
        private static List<byte[]> _largeArrays = new List<byte[]>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start allocating large objects
            Thread thread = new Thread(AllocateLargeObjects);
            thread.Start();

            // Let it run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;
        }

        private void AllocateLargeObjects()
        {
            while (_isRunning)
            {
                try
                {
                    // Allocate a 100MB array
                    byte[] largeArray = new byte[100 * 1024 * 1024];
                    _largeArrays.Add(largeArray);

                    // Small delay to allow memory pressure to build up
                    Thread.Sleep(100);
                }
                catch (OutOfMemoryException)
                {
                    // If we run out of memory, clear some arrays and continue
                    if (_largeArrays.Count > 0)
                    {
                        _largeArrays.RemoveAt(0);
                    }
                }
            }
        }
    }
} 