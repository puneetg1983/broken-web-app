using System;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.OutOfMemory
{
    public partial class OutOfMemory1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Initialize the page
            }
        }
    }
} 