<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighMemory1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighMemory.HighMemory1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Memory Scenario - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>High Memory Scenario - Actual</h1>
            <p>This page will trigger high memory usage by allocating large arrays.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Running high memory scenario..."></asp:Label>
        </div>
    </form>
</body>
</html> 