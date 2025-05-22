using System;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowResponse
{
    public partial class SlowResponse2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Simulate slow database operations
            SimulateDatabaseOperations();
        }

        private void SimulateDatabaseOperations()
        {
            // Simulate database connection delay
            Thread.Sleep(1000);

            // Simulate complex query execution
            for (int i = 0; i < 5; i++)
            {
                // Simulate query execution time
                Thread.Sleep(500);

                // Simulate data processing
                ProcessQueryResults();
            }
        }

        private void ProcessQueryResults()
        {
            // Simulate processing of query results
            for (int i = 0; i < 1000; i++)
            {
                // Simulate data transformation
                double result = Math.Sqrt(i) * Math.Sin(i);
            }
        }
    }
} 