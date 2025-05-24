using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessLargeDataset();
        }

        private void ProcessLargeDataset()
        {
            try
            {
                // Create a large dataset
                var data = new List<int>();
                for (int i = 0; i < 1000000; i++)
                {
                    data.Add(i);
                }

                // Perform complex operations on the dataset
                var result = data
                    .Where(x => x % 2 == 0)
                    .Select(x => (long)x * x)
                    .OrderByDescending(x => x)
                    .Take(1000)
                    .Sum();

                // Simulate additional processing time - increased to 3 seconds to ensure consistent response time
                System.Threading.Thread.Sleep(3000);

                lblStatus.Text = $"Long-running task completed. Processed {data.Count} items and calculated sum: {result}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error during long-running task: {ex.Message}";
            }
        }
    }
} 