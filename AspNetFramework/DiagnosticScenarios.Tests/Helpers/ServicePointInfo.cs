namespace DiagnosticScenarios.Tests.Helpers
{

    public class ServicePointInfo
    {
        public ServicePointInfo()
        {
        }

        public string Address { get; set; }
        public int ConnectionLimit { get; set; }
        public int CurrentConnections { get; set; }
        public int ConnectionGroupCount { get; set; }
        public int TotalConnections { get; set; }
    }
} 