using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DiagnosticScenarios.Web.Scenarios.HighConnections
{
    public partial class HighConnections1Actual : System.Web.UI.Page
    {
        private static readonly List<TcpClient> _activeConnections = new List<TcpClient>();
        private const int TargetConnections = 2000;
        private const int ConnectionTimeout = 5000; // 5 seconds
        private static readonly object _lock = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Starting to create outbound connections...";
                Task.Run(() => CreateConnections());
            }
        }

        private async Task CreateConnections()
        {
            try
            {
                // Create connections in batches to avoid overwhelming the system
                const int batchSize = 100;
                int totalCreated = 0;

                while (totalCreated < TargetConnections)
                {
                    int currentBatch = Math.Min(batchSize, TargetConnections - totalCreated);
                    var tasks = new List<Task>();

                    for (int i = 0; i < currentBatch; i++)
                    {
                        tasks.Add(CreateSingleConnection());
                    }

                    await Task.WhenAll(tasks);
                    totalCreated += currentBatch;

                    // Update status
                    UpdateStatus($"Created {totalCreated} outbound connections out of {TargetConnections}");

                    // Small delay between batches
                    await Task.Delay(1000);
                }

                UpdateStatus($"Successfully created {totalCreated} outbound connections. Connections will be maintained for 5 minutes.");
                
                // Keep connections alive for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5));

                // Clean up connections
                CleanupConnections();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error creating connections: {ex.Message}");
            }
        }

        private async Task CreateSingleConnection()
        {
            try
            {
                var client = new TcpClient();
                var connectTask = client.ConnectAsync("localhost", 80);
                
                if (await Task.WhenAny(connectTask, Task.Delay(ConnectionTimeout)) == connectTask)
                {
                    lock (_lock)
                    {
                        _activeConnections.Add(client);
                    }
                }
                else
                {
                    client.Dispose();
                }
            }
            catch
            {
                // Ignore individual connection failures
            }
        }

        private void CleanupConnections()
        {
            lock (_lock)
            {
                foreach (var client in _activeConnections)
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
                _activeConnections.Clear();
            }
        }

        private void UpdateStatus(string message)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Write($"<script>document.getElementById('lblStatus').innerHTML = '{message}';</script>");
                HttpContext.Current.Response.Flush();
            }
        }
    }
} 