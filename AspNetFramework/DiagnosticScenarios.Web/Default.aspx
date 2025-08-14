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
        .system-info {
            margin-bottom: 30px;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 5px;
            border: 1px solid #ddd;
        }
        .system-info table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
        }
        .system-info th, .system-info td {
            padding: 8px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }
        .system-info th {
            background-color: #f1f1f1;
            font-weight: bold;
        }
        .system-info tr:hover {
            background-color: #f5f5f5;
        }
        .azure-info {
            background-color: #0072C6;
            color: white;
            padding: 2px 6px;
            border-radius: 3px;
            font-size: 0.8em;
            margin-left: 5px;
        }
    </style>
    <script type="text/javascript">
        function updateUptime() {
            var startTime = new Date('<%= StartTime.ToString("yyyy-MM-dd HH:mm:ss") %>');
            var now = new Date();
            var diff = now - startTime;
            
            var seconds = Math.floor(diff / 1000);
            var minutes = Math.floor(seconds / 60);
            var hours = Math.floor(minutes / 60);
            var days = Math.floor(hours / 24);
            
            var uptimeText = '';
            if (days > 0) {
                uptimeText += days + ' day' + (days > 1 ? 's' : '') + ' ';
            }
            if (hours % 24 > 0) {
                uptimeText += (hours % 24) + ' hour' + ((hours % 24) > 1 ? 's' : '') + ' ';
            }
            if (minutes % 60 > 0) {
                uptimeText += (minutes % 60) + ' minute' + ((minutes % 60) > 1 ? 's' : '') + ' ';
            }
            if (seconds % 60 > 0) {
                uptimeText += (seconds % 60) + ' second' + ((seconds % 60) > 1 ? 's' : '') + ' ';
            }
            
            document.getElementById('uptime').textContent = uptimeText;
        }
        
        // Update uptime every second
        setInterval(updateUptime, 1000);
        // Initial update
        updateUptime();
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Diagnostic Scenarios</h1>
        
        <div class="system-info">
            <h2>System Information <span class="azure-info">Azure App Service</span></h2>
            <table>
                <tr>
                    <th>Property</th>
                    <th>Value</th>
                </tr>
                <tr>
                    <td>App Service Name</td>
                    <td><%= AppServiceName %></td>
                </tr>
                <tr>
                    <td>App Service Plan</td>
                    <td><%= AppServicePlan %></td>
                </tr>
                <tr>
                    <td>Region</td>
                    <td><%= Region %></td>
                </tr>
                <tr>
                    <td>Deployment Slot</td>
                    <td><%= AppServiceSlot %></td>
                </tr>
                <tr>
                    <td>Instance ID</td>
                    <td><%= InstanceId %></td>
                </tr>
                <tr>
                    <td>Machine Name</td>
                    <td><%= MachineName %></td>
                </tr>
                <tr>
                    <td>Environment</td>
                    <td><%= Environment %></td>
                </tr>
                <tr>
                    <td>Process ID</td>
                    <td><%= ProcessId %></td>
                </tr>
                <tr>
                    <td>Uptime</td>
                    <td id="uptime"></td>
                </tr>
                <tr>
                    <td>Process Bitness</td>
                    <td><%= ProcessBitness %></td>
                </tr>
                <tr>
                    <td>Hostname</td>
                    <td><%= Hostname %></td>
                </tr>
            </table>
        </div>
        
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
            <a href="Scenarios/SlowResponse/SlowResponse2.aspx">Slow Response Scenario 2 - Long Running Task</a>
            <a href="Scenarios/SlowResponse/SlowResponse3.aspx">Slow Response Scenario 3 - Async Delay</a>
        </div>

        <div class="scenario-group">
            <h2>Slow Dependency Call Scenarios</h2>
            <a href="Scenarios/SlowDependency/SlowDependency1.aspx">Slow Dependency Scenario 1 - External API Delay</a>
            <a href="Scenarios/SlowDependency/SlowDependency2.aspx">Slow Dependency Scenario 2 - Timeout</a>
            <a href="Scenarios/SlowDependency/SlowDependency3.aspx">Slow Dependency Scenario 3 - Retry Logic</a>
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

        <div class="scenario-group">
            <h2>Storage Quota Scenarios</h2>
            <a href="Scenarios/StorageQuota/StorageQuota1.aspx">Storage Quota Scenario 1 - Disk Space Exhaustion</a>
        </div>

        <div class="scenario-group">
            <h2>High Connections Scenarios</h2>
            <a href="Scenarios/HighConnections/HighConnections1.aspx">High Connections Scenario 1 - Connection Pool Exhaustion</a>
        </div>

        <div class="scenario-group">
            <h2>Stuck Requests Scenarios</h2>
            <a href="Scenarios/StuckRequests/HighSleep.aspx">High Sleep Scenario - Stuck Request Simulation (300 seconds, 500.121 scenario)</a>
        </div>

        <div class="scenario-group">
            <h2>Runtime Version Scenarios</h2>
            <a href="Scenarios/RuntimeVersion/RuntimeVersion1.aspx">Runtime Version Scenario 1 - Version Mismatch</a>
        </div>

        <div class="scenario-group">
            <h2>Missing Dependency Scenarios</h2>
            <a href="Scenarios/MissingDependency/MissingDependency1.aspx">Missing Dependency Scenario 1 - Assembly Not Found</a>
        </div>
    </form>
</body>
</html> 