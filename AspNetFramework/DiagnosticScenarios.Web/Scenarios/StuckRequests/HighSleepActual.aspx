<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighSleepActual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.StuckRequests.HighSleepActual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Sleep Scenario - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>High Sleep Scenario - Actual</h1>
            <p>This page will simulate a stuck request by sleeping for 300 seconds (5 minutes).</p>
            <p>The request will appear to hang and not complete during this time.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Simulating stuck request... This will take 5 minutes."></asp:Label>
        </div>
    </form>
</body>
</html>
