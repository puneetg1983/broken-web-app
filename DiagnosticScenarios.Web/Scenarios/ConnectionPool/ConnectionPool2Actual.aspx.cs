using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.ConnectionPool
{
    public partial class ConnectionPool2Actual : System.Web.UI.Page
    {
        private static List<SqlConnection> _leakedConnections = new List<SqlConnection>();
        private static bool _isRunning = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start connection pool leak
            Thread thread = new Thread(LeakConnections);
            thread.Start();

            // Let it run for 30 seconds
            Thread.Sleep(30000);
            _isRunning = false;
        }

        private void LeakConnections()
        {
            while (_isRunning)
            {
                try
                {
                    // Create a new connection but don't dispose it
                    var connection = new SqlConnection("Server=localhost;Database=NonExistentDB;User Id=sa;Password=password;");
                    connection.Open();
                    _leakedConnections.Add(connection);

                    // Small delay to allow leaks to accumulate
                    Thread.Sleep(100);
                }
                catch (SqlException)
                {
                    // If we can't create more connections, wait a bit and try again
                    Thread.Sleep(1000);
                }
            }
        }
    }
} 