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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Diagnostic Scenarios</h1>
        
        <div class="scenario-group">
            <h2>High CPU Scenarios</h2>
            <a href="Scenarios/HighCpu/HighCpu1.aspx">High CPU Scenario 1</a>
            <a href="Scenarios/HighCpu/HighCpu2.aspx">High CPU Scenario 2</a>
            <a href="Scenarios/HighCpu/HighCpu3.aspx">High CPU Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>High Memory Scenarios</h2>
            <a href="Scenarios/HighMemory/HighMemory1.aspx">High Memory Scenario 1</a>
            <a href="Scenarios/HighMemory/HighMemory2.aspx">High Memory Scenario 2</a>
            <a href="Scenarios/HighMemory/HighMemory3.aspx">High Memory Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Response Scenarios</h2>
            <a href="Scenarios/SlowResponse/SlowResponse1.aspx">Slow Response Scenario 1</a>
            <a href="Scenarios/SlowResponse/SlowResponse2.aspx">Slow Response Scenario 2</a>
            <a href="Scenarios/SlowResponse/SlowResponse3.aspx">Slow Response Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Dependency Call Scenarios</h2>
            <a href="Scenarios/SlowDependency/SlowDependency1.aspx">Slow Dependency Scenario 1</a>
            <a href="Scenarios/SlowDependency/SlowDependency2.aspx">Slow Dependency Scenario 2</a>
            <a href="Scenarios/SlowDependency/SlowDependency3.aspx">Slow Dependency Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Database Call Scenarios</h2>
            <a href="Scenarios/SlowDatabase/SlowDatabase1.aspx">Slow Database Scenario 1</a>
            <a href="Scenarios/SlowDatabase/SlowDatabase2.aspx">Slow Database Scenario 2</a>
            <a href="Scenarios/SlowDatabase/SlowDatabase3.aspx">Slow Database Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>Crashing Scenarios</h2>
            <a href="Scenarios/Crash/UnhandledException1.aspx">Unhandled Exception Scenario 1</a>
            <a href="Scenarios/Crash/UnhandledException2.aspx">Unhandled Exception Scenario 2</a>
            <a href="Scenarios/Crash/StackOverflow1.aspx">Stack Overflow Scenario 1</a>
            <a href="Scenarios/Crash/StackOverflow2.aspx">Stack Overflow Scenario 2</a>
        </div>

        <div class="scenario-group">
            <h2>HTTP 500 Error Scenarios</h2>
            <a href="Scenarios/Http500/Http500_1.aspx">HTTP 500 Scenario 1</a>
            <a href="Scenarios/Http500/Http500_2.aspx">HTTP 500 Scenario 2</a>
            <a href="Scenarios/Http500/Http500_3.aspx">HTTP 500 Scenario 3</a>
        </div>

        <div class="scenario-group">
            <h2>Connection Pool Scenarios</h2>
            <a href="Scenarios/ConnectionPool/ConnectionPool1.aspx">Connection Pool Exhaustion Scenario 1</a>
        </div>

        <div class="scenario-group">
            <h2>Deadlock Scenarios</h2>
            <a href="Scenarios/Deadlock/Deadlock1.aspx">Deadlock Scenario 1</a>
        </div>
    </form>
</body>
</html> 