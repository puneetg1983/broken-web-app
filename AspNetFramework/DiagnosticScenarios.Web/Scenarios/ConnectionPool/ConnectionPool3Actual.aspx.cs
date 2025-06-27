using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool3Actual : System.Web.UI.Page
    {
        private static List<SqlConnection> _connections = new List<SqlConnection>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start connection pool timeout scenario
            Thread thread = new Thread(SimulateTimeouts);
            thread.Start();

            // Let it run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;

            // Clean up connections
            foreach (var connection in _connections)
            {
                try
                {
                    connection.Dispose();
                }
                catch { }
            }
        }

        private void SimulateTimeouts()
        {
            while (_isRunning)
            {
                try
                {
                    // Create a connection with a very short timeout
                    var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=password;Connection Timeout=1;");
                    connection.Open();
                    _connections.Add(connection);

                    // Hold the connection for a while to simulate long-running operations
                    Thread.Sleep(5000);
                }
                catch (SqlException)
                {
                    // If we get a timeout, wait a bit and try again
                    Thread.Sleep(1000);
                }
            }
        }
    }
} 