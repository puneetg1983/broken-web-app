<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowResponse3Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowResponse.SlowResponse3Actual" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Response Scenario 3 - Processing</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Processing Multiple Long-Running Operations</h1>
            <p>This page simulates multiple long-running operations that will take some time to complete.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Processing..."></asp:Label>
        </div>
    </form>
</body>
</html> 