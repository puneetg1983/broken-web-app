using System;
using System.Collections.Generic;

namespace DiagnosticScenarios.Web.Scenarios.OutOfMemory
{
    public partial class OutOfMemory1Actual : System.Web.UI.Page
    {
        private static List<byte[]> _memoryBlocks = new List<byte[]>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Continuously allocate memory until we get OutOfMemoryException
            while (true)
            {
                // Allocate a large block of memory (100MB)
                byte[] block = new byte[100 * 1024 * 1024];
                _memoryBlocks.Add(block);
            }
        }
    }
}