using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.SlowDatabase
{
    public partial class SlowDatabase1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate slow database connection...";
            }
        }

        protected async void btnStartSlowDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartSlowDatabase.Enabled = false;
                progress.Visible = true;
                lblStatus.Text = "Attempting to connect to database...";

                // Connection string to a non-existent SQL Server
                string connectionString = "Server=non-existent-server;Database=NonExistentDB;User Id=sa;Password=wrongpassword;Connection Timeout=30;";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    lblStatus.Text = "Opening database connection...";
                    await connection.OpenAsync(); // This will timeout after 30 seconds
                }
            }
            catch (SqlException ex)
            {
                lblStatus.Text = $"Database Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartSlowDatabase.Enabled = true;
                progress.Visible = false;
            }
        }

        protected void btnStartSlowQuery_Click(object sender, EventArgs e)
        {
            Response.Redirect("SlowDatabase1Actual.aspx");
        }
    }
} 