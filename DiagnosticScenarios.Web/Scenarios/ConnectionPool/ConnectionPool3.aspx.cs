using System;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool3 : Page
    {
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate pool exhaustion...";
            }
        }

        protected void btnExhaustPool_Click(object sender, EventArgs e)
        {
            try
            {
                btnExhaustPool.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Start multiple threads to hold connections open
                for (int i = 0; i < 20; i++)
                {
                    Thread thread = new Thread(() =>
                    {
                        try
                        {
                            // Create a connection and hold it open
                            string connectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
                            
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                // Simulate a long-running operation
                                while (_isRunning)
                                {
                                    using (SqlCommand command = new SqlCommand("WAITFOR DELAY '00:00:01'", connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            // Log the error
                            System.Diagnostics.Debug.WriteLine($"Connection error: {ex.Message}");
                        }
                    });

                    thread.Start();
                }

                lblStatus.Text = "Started pool exhaustion scenario. The connection pool will be exhausted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnExhaustPool.Enabled = true;
                progress.Visible = false;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }

        protected void btnStartConnectionPoolTimeout_Click(object sender, EventArgs e)
        {
            Response.Redirect("ConnectionPool3Actual.aspx");
        }
    }
} 