using System;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool2 : Page
    {
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate connection timeout...";
            }
        }

        protected void btnConnectionTimeout_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnectionTimeout.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Start multiple threads to attempt connections simultaneously
                for (int i = 0; i < 10; i++)
                {
                    Thread thread = new Thread(() =>
                    {
                        try
                        {
                            // Attempt to connect to a non-existent server with a short timeout
                            string connectionString = "Server=NonExistentServer;Database=master;Trusted_Connection=True;Connection Timeout=5;";
                            
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open(); // This will timeout after 5 seconds
                            }
                        }
                        catch (SqlException ex)
                        {
                            // Log the timeout
                            System.Diagnostics.Debug.WriteLine($"Connection timeout: {ex.Message}");
                        }
                    });

                    thread.Start();
                }

                lblStatus.Text = "Started connection timeout scenario. Multiple connection attempts will timeout.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnConnectionTimeout.Enabled = true;
                progress.Visible = false;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }
    }
} 