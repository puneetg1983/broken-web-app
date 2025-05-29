using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.HighConnections
{
    public partial class HighConnections1Actual : System.Web.UI.Page
    {
        private static readonly List<WebClient> _activeConnections = new List<WebClient>();
        private const int TargetConnections = 2000;
        private const int ConnectionTimeout = 5000; // 5 seconds
        private static readonly object _lock = new object();
        private static readonly string[] _endpoints = new[]
        {
            "http://www.microsoft.com",
            "http://www.google.com",
            "http://www.amazon.com",
            "http://www.github.com",
            "http://www.azure.com"
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(async state =>
            {
                try
                {
                    await CreateConnectionsAsync();
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Error: {ex.Message}");
                }
            });
        }

        private async Task CreateConnectionsAsync()
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
                        tasks.Add(Task.Run(()=> { CreateSingleConnection(); }));
                    }

                    await Task.WhenAll(tasks);
                    totalCreated += currentBatch;

                    // Update status
                    UpdateStatus($"Created {totalCreated} outbound connections out of {TargetConnections}");

                    // Small delay between batches
                    await Task.Delay(5);
                }

                UpdateStatus($"Successfully created {totalCreated} outbound connections. Connections will be maintained for 5 minutes.");
                
                // await Task.Delay(TimeSpan.FromSeconds(1));

                // Clean up connections
                // CleanupConnections();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error creating connections: {ex.Message}");
            }
        }

        private void CreateSingleConnection()
        {
            try
            {
                var endpoint = _endpoints[new Random().Next(_endpoints.Length)];
                var client = new WebClient();                
                client.OpenRead(new Uri(endpoint));               
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