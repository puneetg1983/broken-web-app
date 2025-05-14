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
            try
            {
                btnDatabaseError.Enabled = false;
                progress.Visible = true;

                // Attempt to connect to a non-existent database server
                string connectionString = "Server=NonExistentServer;Database=NonExistentDB;User Id=sa;Password=password;";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // This will throw an exception
                }

                lblStatus.Text = "Database connection successful (this should not happen)";
            }
            catch (SqlException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Database connection error: {ex.Message}");
                
                // Throw a new exception to cause HTTP 500
                throw new Exception("Failed to connect to the database server. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnDatabaseError.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 