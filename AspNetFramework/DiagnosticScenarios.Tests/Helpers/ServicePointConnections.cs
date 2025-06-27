using System.Collections.Generic;

namespace DiagnosticScenarios.Tests.Helpers
{
    public class ServicePointConnections
    {
        public int ServicePointCount { get; set; }
        public int DefaultConnectionLimit { get; set; }
        public List<ServicePointInfo> ServicePoints { get; set; }
    }
} 