using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory3Actual : System.Web.UI.Page
    {
        private static List<string> _largeStrings = new List<string>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start string concatenation
            Thread thread = new Thread(ConcatenateStrings);
            thread.Start();

            // Let it run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;
        }

        private void ConcatenateStrings()
        {
            while (_isRunning)
            {
                try
                {
                    // Create a large string through concatenation
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < 1000000; i++)
                    {
                        sb.Append("This is a long string that will be concatenated multiple times to create memory pressure. ");
                    }
                    _largeStrings.Add(sb.ToString());

                    // Small delay to allow memory pressure to build up
                    Thread.Sleep(100);
                }
                catch (OutOfMemoryException)
                {
                    // If we run out of memory, clear some strings and continue
                    if (_largeStrings.Count > 0)
                    {
                        _largeStrings.RemoveAt(0);
                    }
                }
            }
        }
    }
} 