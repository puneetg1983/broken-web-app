using System;
using System.Data.SqlClient;
using System.Configuration;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_4Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Attempt to read and use an invalid connection string
            // This will throw a ConfigurationErrorsException
            string connectionString = ConfigurationManager.ConnectionStrings["InvalidConnectionString"].ConnectionString;
            
            // Attempt to use the connection string
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
            }
        }
    }
} 