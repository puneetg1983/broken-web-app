using System;

namespace DiagnosticScenarios.Tests.Helpers
{
    public class ProcessMetrics
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string MachineName { get; set; }
        public double CpuTime { get; set; }
        public long PrivateBytes { get; set; }
        public long WorkingSet { get; set; }
        public int ThreadCount { get; set; }
        public int HandleCount { get; set; }
        public double ProcessUptimeMinutes { get; set; }
        public ServicePointConnections ServicePointConnections { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 