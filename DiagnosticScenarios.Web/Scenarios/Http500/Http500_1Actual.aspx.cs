using System;
using System.Data.SqlClient;

namespace DiagnosticScenarios.Web.Scenarios.Http500
{
    public partial class Http500_1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Attempt to connect to a non-existent database
                using (var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=wrongpassword;"))
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow to trigger HTTP 500
                System.Diagnostics.Debug.WriteLine($"Database connection error: {ex.Message}");
                throw;
            }
        }
    }
} 