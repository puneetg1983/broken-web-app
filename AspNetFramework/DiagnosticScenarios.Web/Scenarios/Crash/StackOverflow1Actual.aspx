<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StackOverflow1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Crash.StackOverflow1Actual" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stack Overflow Crash - Actual</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Stack Overflow Crash (Actual)</h2>
            <asp:Label ID="lblStatus" runat="server" Text="Triggering stack overflow..."></asp:Label>
        </div>
    </form>
</body>
</html> 