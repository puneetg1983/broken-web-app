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
            // Allocate 500MB of memory in 10MB chunks
            for (int i = 0; i < 50; i++)
            {
                _memoryLeak.Add(new byte[10 * 1024 * 1024]); // 10MB
                // Fill the array with data to prevent optimization
                for (int j = 0; j < _memoryLeak[i].Length; j++)
                {
                    _memoryLeak[i][j] = (byte)(j % 256);
                }
                Thread.Sleep(100); // Small delay to make the allocation visible
            }
        }
    }
} 