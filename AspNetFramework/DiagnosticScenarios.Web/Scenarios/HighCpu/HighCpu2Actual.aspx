<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu2Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu2Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario 2 - Thread Contention - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>High CPU Scenario 2 - Thread Contention - Actual</h1>
            <p>This page will trigger high CPU usage through thread contention.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Running thread contention scenario..."></asp:Label>
        </div>
    </form>
</body>
</html> 