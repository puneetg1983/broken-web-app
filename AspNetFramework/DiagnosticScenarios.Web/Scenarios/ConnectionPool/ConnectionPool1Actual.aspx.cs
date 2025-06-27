using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool1Actual : System.Web.UI.Page
    {
        private static List<SqlConnection> _connections = new List<SqlConnection>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start connection pool exhaustion
            Thread thread = new Thread(ExhaustConnectionPool);
            thread.Start();

            // Let it run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;
        }

        private void ExhaustConnectionPool()
        {
            while (_isRunning)
            {
                try
                {
                    // Create a new connection
                    var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=password;");
                    connection.Open();
                    _connections.Add(connection);

                    // Small delay to allow pool to fill up
                    Thread.Sleep(100);
                }
                catch (SqlException)
                {
                    // If we can't create more connections, clear some and continue
                    if (_connections.Count > 0)
                    {
                        var connection = _connections[0];
                        _connections.RemoveAt(0);
                        connection.Dispose();
                    }
                }
            }

            // Cleanup
            foreach (var connection in _connections)
            {
                connection.Dispose();
            }
            _connections.Clear();
        }
    }
} 