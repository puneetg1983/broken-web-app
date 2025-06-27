using System;
using System.Diagnostics;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
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
            var servicePointStats = GetServicePointStats();

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
                ServicePointConnections = new
                {
                    ServicePointCount = servicePointStats.ServicePointCount,
                    DefaultConnectionLimit = ServicePointManager.DefaultConnectionLimit,
                    ServicePoints = servicePointStats.ServicePoints
                },
                Timestamp = DateTime.UtcNow
            };

            var serializer = new JavaScriptSerializer();
            Response.Write(serializer.Serialize(metrics));
            Response.End();
        }

        private ServicePointStats GetServicePointStats()
        {
            var stats = new ServicePointStats();
            try
            {
                var tableField = typeof(ServicePointManager).GetField("s_ServicePointTable", 
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                
                if (tableField != null)
                {
                    var table = (Hashtable)tableField.GetValue(null);
                    var keys = table.Keys.Cast<object>().ToList();
                    stats.ServicePointCount = keys.Count;

                    foreach (var key in keys)
                    {
                        var val = ((WeakReference)table[key]);
                        if (val?.Target == null) continue;

                        var servicePoint = val.Target as ServicePoint;
                        if (servicePoint == null) continue;

                        var servicePointInfo = GetServicePointInfo(servicePoint);
                        if (servicePointInfo != null)
                        {
                            stats.ServicePoints.Add(servicePointInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting ServicePoint stats: {ex.Message}");
            }

            return stats;
        }

        private ServicePointInfo GetServicePointInfo(ServicePoint sp)
        {
            try
            {
                var spType = sp.GetType();
                var privateOrPublicInstanceField = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                var connectionGroupField = spType.GetField("m_ConnectionGroupList", privateOrPublicInstanceField);
                
                if (connectionGroupField == null) return null;

                var value = (Hashtable)connectionGroupField.GetValue(sp);
                var connectionGroups = value.Keys.Cast<object>().ToList();
                var totalConnections = 0;

                foreach (var key in connectionGroups)
                {
                    var connectionGroup = value[key];
                    var groupType = connectionGroup.GetType();
                    var listField = groupType.GetField("m_ConnectionList", privateOrPublicInstanceField);
                    
                    if (listField != null)
                    {
                        var listValue = (ArrayList)listField.GetValue(connectionGroup);
                        totalConnections += listValue.Count;
                    }
                }

                return new ServicePointInfo
                {
                    Address = sp.Address.ToString(),
                    ConnectionLimit = sp.ConnectionLimit,
                    CurrentConnections = sp.CurrentConnections,
                    ConnectionGroupCount = connectionGroups.Count,
                    TotalConnections = totalConnections
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting ServicePoint info: {ex.Message}");
                return null;
            }
        }

        private class ServicePointStats
        {
            public int ServicePointCount { get; set; }
            public List<ServicePointInfo> ServicePoints { get; set; } = new List<ServicePointInfo>();
        }

        private class ServicePointInfo
        {
            public string Address { get; set; }
            public int ConnectionLimit { get; set; }
            public int CurrentConnections { get; set; }
            public int ConnectionGroupCount { get; set; }
            public int TotalConnections { get; set; }
        }
    }
} 