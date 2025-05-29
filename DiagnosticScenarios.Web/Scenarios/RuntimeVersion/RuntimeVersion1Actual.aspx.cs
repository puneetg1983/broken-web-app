using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiagnosticScenarios.Web.Scenarios.RuntimeVersion
{
    public partial class RuntimeVersion1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Attempt to use features from .NET 6.0
                // This will fail on .NET Framework 4.8
                var assembly = Assembly.Load("System.Runtime.CompilerServices.Unsafe");
                var type = assembly.GetType("System.Runtime.CompilerServices.Unsafe");
                
                // Try to use a method that doesn't exist in .NET Framework 4.8
                var method = type.GetMethod("AddByteOffset");
                if (method == null)
                {
                    throw new MissingMethodException("Method AddByteOffset not found");
                }

                
            }
            catch (Exception)
            {
                // This is the expected behavior
                throw; // Re-throw to ensure the application fails
            }
        }
    }
} 