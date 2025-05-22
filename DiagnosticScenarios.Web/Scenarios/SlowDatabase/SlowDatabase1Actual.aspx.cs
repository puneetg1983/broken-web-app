using System;
using System.Data.SqlClient;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowDatabase
{
    public partial class SlowDatabase1Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Simulate a slow database query
                using (var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=password;"))
                {
                    connection.Open();

                    // Create a command that will take a long time to execute
                    using (var command = new SqlCommand("WAITFOR DELAY '00:00:05'; SELECT * FROM NonExistentTable", connection))
                    {
                        // This will timeout after 30 seconds
                        command.CommandTimeout = 30;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                throw;
            }
        }
    }
} 