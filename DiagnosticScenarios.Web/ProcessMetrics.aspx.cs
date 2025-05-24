using System;
using System.Diagnostics;
using System.Web;
using System.Web.Script.Serialization;

namespace DiagnosticScenarios.Web
{
    public partial class ProcessMetrics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var process = Process.GetCurrentProcess();
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
                Timestamp = DateTime.UtcNow
            };

            var serializer = new JavaScriptSerializer();
            Response.Write(serializer.Serialize(metrics));
            Response.End();
        }
    }
} 