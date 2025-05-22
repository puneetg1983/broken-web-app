<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Http500_1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Http500.Http500_1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTTP 500 Scenario - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>HTTP 500 Scenario - Actual</h1>
            <p>This page will trigger an HTTP 500 error by attempting to connect to a non-existent database.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Triggering HTTP 500 error..."></asp:Label>
        </div>
    </form>
</body>
</html> 