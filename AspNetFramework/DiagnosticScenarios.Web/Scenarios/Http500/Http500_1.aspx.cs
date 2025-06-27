using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate database error...";
            }
        }

        protected void btnDatabaseError_Click(object sender, EventArgs e)
        {
            Response.Redirect("Http500_1Actual.aspx");
        }
    }
} 