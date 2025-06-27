using System;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.MissingDependency
{
    public partial class MissingDependency1Actual : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This will throw a FileNotFoundException because the assembly doesn't exist
            // The assembly name is intentionally misspelled to ensure it's not found
            var assembly = System.Reflection.Assembly.Load("MissingDependency.Assembly");
            var type = assembly.GetType("MissingDependency.Class");
            var instance = Activator.CreateInstance(type);
        }
    }
} 