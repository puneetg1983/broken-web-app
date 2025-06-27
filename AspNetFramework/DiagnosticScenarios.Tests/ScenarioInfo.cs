namespace DiagnosticScenarios.Tests
{
    public class ScenarioInfo
    {
        public string Path { get; set; }
        public string MetricName { get; set; }
        public int Iterations { get; set; }
        public int DelayBetweenIterationsSeconds { get; set; }
        public int WaitForMetricsMinutes { get; set; }
    }
} 