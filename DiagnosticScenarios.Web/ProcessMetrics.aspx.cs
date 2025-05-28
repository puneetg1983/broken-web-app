using System;
using System.Diagnostics;
using System.Web;
using System.Web.Script.Serialization;
using System.Net.NetworkInformation;
using System.Linq;

namespace DiagnosticScenarios.Web
{
    public partial class ProcessMetrics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var process = Process.GetCurrentProcess();
            var tcpStats = GetTcpConnectionStats(process.Id);

            var metrics = new
            {
                ProcessId = process.Id,
                ProcessName = process.ProcessName,
                MachineName = Environment.MachineName,
                CpuTime = process.TotalProcessorTime.TotalSeconds,
                PrivateBytes = process.PrivateMemorySize64,
                WorkingSet = process.WorkingSet64,
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount,
                ProcessUptimeMinutes = (DateTime.UtcNow - process.StartTime).TotalMinutes,
                TcpConnections = new
                {
                    Total = tcpStats.TotalConnections,
                    Incoming = tcpStats.IncomingConnections,
                    Outgoing = tcpStats.OutgoingConnections,
                    Established = tcpStats.EstablishedConnections,
                    TimeWait = tcpStats.TimeWaitConnections,
                    CloseWait = tcpStats.CloseWaitConnections
                },
                Timestamp = DateTime.UtcNow
            };

            var serializer = new JavaScriptSerializer();
            Response.Write(serializer.Serialize(metrics));
            Response.End();
        }

        private TcpConnectionStats GetTcpConnectionStats(int processId)
        {
            var stats = new TcpConnectionStats();
            try
            {
                var connections = IPGlobalProperties.GetIPGlobalProperties()
                    .GetActiveTcpConnections()
                    .Where(c => c.ProcessId == processId);

                foreach (var conn in connections)
                {
                    stats.TotalConnections++;
                    
                    // Determine if connection is incoming or outgoing
                    if (conn.LocalEndPoint.Port < conn.RemoteEndPoint.Port)
                    {
                        stats.OutgoingConnections++;
                    }
                    else
                    {
                        stats.IncomingConnections++;
                    }

                    // Track connection states
                    switch (conn.State)
                    {
                        case TcpState.Established:
                            stats.EstablishedConnections++;
                            break;
                        case TcpState.TimeWait:
                            stats.TimeWaitConnections++;
                            break;
                        case TcpState.CloseWait:
                            stats.CloseWaitConnections++;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - we want to return partial metrics
                System.Diagnostics.Debug.WriteLine($"Error getting TCP stats: {ex.Message}");
            }

            return stats;
        }

        private class TcpConnectionStats
        {
            public int TotalConnections { get; set; }
            public int IncomingConnections { get; set; }
            public int OutgoingConnections { get; set; }
            public int EstablishedConnections { get; set; }
            public int TimeWaitConnections { get; set; }
            public int CloseWaitConnections { get; set; }
        }
    }
} 