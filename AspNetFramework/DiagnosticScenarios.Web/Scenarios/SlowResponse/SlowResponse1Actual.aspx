<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowResponse1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowResponse.SlowResponse1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Response Scenario - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Slow Response Scenario - Actual</h1>
            <p>This page will simulate a slow response by waiting for 2 seconds.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Simulating slow response..."></asp:Label>
        </div>
    </form>
</body>
</html> 