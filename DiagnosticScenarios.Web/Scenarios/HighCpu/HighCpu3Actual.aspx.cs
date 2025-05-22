using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.HighCpu
{
    public partial class HighCpu3Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Start recursive calculations in parallel
            Parallel.For(0, 10, i =>
            {
                RecursiveCalculation(30); // Start with depth 30
            });
        }

        private void RecursiveCalculation(int depth)
        {
            if (depth <= 0)
                return;

            // Perform some CPU-intensive calculations
            double result = 0;
            for (int i = 0; i < 1000000; i++)
            {
                result += Math.Sqrt(i) * Math.Sin(i);
            }

            // Recursive calls
            RecursiveCalculation(depth - 1);
            RecursiveCalculation(depth - 1);
        }
    }
} 