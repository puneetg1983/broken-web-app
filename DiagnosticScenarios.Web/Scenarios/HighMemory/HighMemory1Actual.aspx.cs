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
            // Allocate 100MB of memory in 1MB chunks
            for (int i = 0; i < 100; i++)
            {
                _memoryLeak.Add(new byte[1024 * 1024]); // 1MB
                Thread.Sleep(100); // Small delay to make the allocation visible
            }
        }
    }
} 