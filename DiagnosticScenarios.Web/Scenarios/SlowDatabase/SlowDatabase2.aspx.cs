using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.SlowDatabase
{
    public partial class SlowDatabase2 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate database deadlock...";
            }
        }

        protected async void btnStartDeadlock_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartDeadlock.Enabled = false;
                progress.Visible = true;
                lblStatus.Text = "Starting deadlock simulation...";

                // Connection string to a non-existent SQL Server
                // This will simulate a deadlock error
                string connectionString = "Server=non-existent-server;Database=NonExistentDB;User Id=sa;Password=wrongpassword;Connection Timeout=30;";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    lblStatus.Text = "Attempting to create deadlock...";
                    
                    // This will fail with a deadlock error
                    await connection.OpenAsync();
                    
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = @"
                            BEGIN TRANSACTION;
                            UPDATE Table1 SET Column1 = 'Value1' WHERE Id = 1;
                            WAITFOR DELAY '00:00:05';
                            UPDATE Table2 SET Column1 = 'Value2' WHERE Id = 1;
                            COMMIT;";
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Deadlock error number
                {
                    lblStatus.Text = "Deadlock detected! SQL Server chose this transaction as the deadlock victim.";
                }
                else
                {
                    lblStatus.Text = $"Database Error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartDeadlock.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 