using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Net;

namespace DiagnosticScenariosCore.Web.Controllers
{
    public class ProcessMetricsController : Controller
    {
        private readonly ILogger<ProcessMetricsController> _logger;

        public ProcessMetricsController(ILogger<ProcessMetricsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/ProcessMetrics/api")]
        public IActionResult Get()
        {
            try
            {
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

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting process metrics");
                return StatusCode(500, new { error = ex.Message });
            }
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
                _logger.LogWarning(ex, "Error getting ServicePoint stats");
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
                _logger.LogWarning(ex, "Error getting ServicePoint info");
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