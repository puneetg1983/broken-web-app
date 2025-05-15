<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DiagnosticScenarios.Web.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Diagnostic Scenarios</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            line-height: 1.6;
        }
        .scenario-group {
            margin-bottom: 20px;
            padding: 15px;
            border: 1px solid #ddd;
            border-radius: 5px;
        }
        h1 {
            color: #333;
        }
        h2 {
            color: #666;
            margin-top: 0;
        }
        a {
            display: block;
            margin: 5px 0;
            color: #0066cc;
            text-decoration: none;
        }
        a:hover {
            text-decoration: underline;
        }
        .status {
            font-size: 0.8em;
            color: #666;
            margin-left: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Diagnostic Scenarios</h1>
        
        <div class="scenario-group">
            <h2>High CPU Scenarios</h2>
            <a href="Scenarios/HighCpu/HighCpu1.aspx">High CPU Scenario 1 - Infinite Loop</a>
            <a href="Scenarios/HighCpu/HighCpu2.aspx">High CPU Scenario 2 - Thread Contention</a>
            <a href="Scenarios/HighCpu/HighCpu3.aspx">High CPU Scenario 3 - Deadlock</a>
        </div>

        <div class="scenario-group">
            <h2>High Memory Scenarios</h2>
            <a href="Scenarios/HighMemory/HighMemory1.aspx">High Memory Scenario 1 - Memory Leak</a>
            <a href="Scenarios/HighMemory/HighMemory2.aspx">High Memory Scenario 2 - Event Handler Leak</a>
            <a href="Scenarios/HighMemory/HighMemory3.aspx">High Memory Scenario 3 - LOH Fragmentation</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Response Scenarios</h2>
            <a href="Scenarios/SlowResponse/SlowResponse1.aspx">Slow Response Scenario 1 - Thread Sleep</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Dependency Call Scenarios</h2>
            <a href="Scenarios/SlowDependency/SlowDependency1.aspx">Slow Dependency Scenario 1 - External API Delay</a>
            <a href="Scenarios/SlowDependency/SlowDependency2.aspx">Slow Dependency Scenario 2 - Timeout</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Database Call Scenarios</h2>
            <a href="Scenarios/SlowDatabase/SlowDatabase1.aspx">Slow Database Scenario 1 - Long Query</a>
            <a href="Scenarios/SlowDatabase/SlowDatabase2.aspx">Slow Database Scenario 2 - Connection Pool</a>
        </div>

        <div class="scenario-group">
            <h2>Crashing Scenarios</h2>
            <a href="Scenarios/Crash/UnhandledException1.aspx">Unhandled Exception Scenario 1 - Background Thread</a>
            <a href="Scenarios/Crash/StackOverflow1.aspx">Stack Overflow Scenario 1 - Recursion</a>
            <a href="Scenarios/Crash/Crash1.aspx">Crash Scenario 1 - Null Reference</a>
            <a href="Scenarios/Crash/Crash2.aspx">Crash Scenario 2 - Invalid Operation</a>
            <a href="Scenarios/Crash/Crash3.aspx">Crash Scenario 3 - Out of Memory</a>
        </div>

        <div class="scenario-group">
            <h2>HTTP 500 Error Scenarios</h2>
            <a href="Scenarios/Http500/Http500_1.aspx">HTTP 500 Scenario 1 - Database Connection Error</a>
            <a href="Scenarios/Http500/Http500_2.aspx">HTTP 500 Scenario 2 - File Access Error</a>
            <a href="Scenarios/Http500/Http500_3.aspx">HTTP 500 Scenario 3 - Configuration Error</a>
        </div>

        <div class="scenario-group">
            <h2>Connection Pool Scenarios</h2>
            <a href="Scenarios/ConnectionPool/ConnectionPool1.aspx">Connection Pool Scenario 1 - Exhaustion</a>
            <a href="Scenarios/ConnectionPool/ConnectionPool2.aspx">Connection Pool Scenario 2 - Timeout</a>
            <a href="Scenarios/ConnectionPool/ConnectionPool3.aspx">Connection Pool Scenario 3 - Leak</a>
        </div>

        <div class="scenario-group">
            <h2>Deadlock Scenarios</h2>
            <a href="Scenarios/Deadlock/Deadlock1.aspx">Deadlock Scenario 1 - Classic Deadlock</a>
        </div>
    </form>
</body>
</html> 