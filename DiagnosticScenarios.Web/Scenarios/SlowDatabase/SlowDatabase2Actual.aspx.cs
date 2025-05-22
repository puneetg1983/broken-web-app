using System;
using System.Data.SqlClient;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.SlowDatabase
{
    public partial class SlowDatabase2Actual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Simulate a slow database transaction
                using (var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=password;"))
                {
                    connection.Open();

                    // Start a transaction
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Create a command that will take a long time to execute
                            using (var command = new SqlCommand("WAITFOR DELAY '00:00:05'; UPDATE NonExistentTable SET Column1 = 'Value'", connection, transaction))
                            {
                                // This will timeout after 30 seconds
                                command.CommandTimeout = 30;
                                command.ExecuteNonQuery();
                            }

                            // Simulate more work in the transaction
                            Thread.Sleep(2000);

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch
                        {
                            // Rollback on error
                            transaction.Rollback();
                            throw;
                        }
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