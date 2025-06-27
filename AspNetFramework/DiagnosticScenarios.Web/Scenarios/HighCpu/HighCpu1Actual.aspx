<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>High CPU Scenario - Actual</h1>
            <p>This page will trigger high CPU usage for 30 seconds.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Running high CPU scenario..."></asp:Label>
        </div>
    </form>
</body>
</html> 