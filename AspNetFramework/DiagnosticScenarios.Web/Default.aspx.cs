using System;
using System.Diagnostics;
using System.Web.UI;

namespace DiagnosticScenarios.Web
{
    public partial class Default : Page
    {
        public string MachineName { get; private set; }
        public string InstanceId { get; private set; }
        public string Environment { get; private set; }
        public int ProcessId { get; private set; }
        public DateTime StartTime { get; private set; }
        public string ProcessBitness { get; private set; }
        public string Hostname { get; private set; }
        public string Region { get; private set; }
        public string AppServicePlan { get; private set; }
        public string AppServiceName { get; private set; }
        public string AppServiceSlot { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Azure App Service environment variables
                MachineName = System.Environment.GetEnvironmentVariable("COMPUTERNAME") ?? System.Environment.MachineName;
                InstanceId = System.Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID") ?? "Not available";
                Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? 
                            System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ?? "Production";
                ProcessId = Process.GetCurrentProcess().Id;
                StartTime = Process.GetCurrentProcess().StartTime;
                ProcessBitness = System.Environment.Is64BitProcess ? "64-bit" : "32-bit";
                Hostname = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") ?? Request.Url.Host;
                
                // Additional Azure App Service information
                Region = System.Environment.GetEnvironmentVariable("REGION_NAME") ?? "Not available";
                AppServicePlan = System.Environment.GetEnvironmentVariable("WEBSITE_SKU") ?? "Not available";
                AppServiceName = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") ?? "Not available";
                AppServiceSlot = System.Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? "production";
            }
        }
    }
} 