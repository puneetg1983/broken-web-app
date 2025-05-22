using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool1 : Page
    {
        private static readonly List<SqlConnection> _leakedConnections = new List<SqlConnection>();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate connection leak...";
            }
        }

        protected void btnStartConnectionPoolExhaustion_Click(object sender, EventArgs e)
        {
            Response.Redirect("ConnectionPool1Actual.aspx");
        }

        protected void btnStartConnectionPool_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartConnectionPool.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Start a background thread to leak connections
                Thread thread = new Thread(() =>
                {
                    int connectionCount = 0;
                    string connectionString = "Server=localhost;Database=master;Trusted_Connection=True;";

                    while (_isRunning)
                    {
                        try
                        {
                            // Create a new connection but don't dispose it
                            SqlConnection connection = new SqlConnection(connectionString);
                            connection.Open();

                            // Add to our list to prevent GC from collecting it
                            _leakedConnections.Add(connection);
                            connectionCount++;

                            // Update status
                            lblStatus.Text = $"Leaked {connectionCount} connections. Connection pool will be exhausted soon.";
                        }
                        catch (SqlException ex)
                        {
                            lblStatus.Text = $"Connection pool exhausted! Error: {ex.Message}";
                            break;
                        }
                    }
                });

                thread.Start();

                lblStatus.Text = "Started connection leak scenario. The connection pool will be exhausted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartConnectionPool.Enabled = true;
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