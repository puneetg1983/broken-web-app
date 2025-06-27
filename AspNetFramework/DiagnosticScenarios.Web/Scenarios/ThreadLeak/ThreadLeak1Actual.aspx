<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThreadLeak1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ThreadLeak.ThreadLeak1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Thread Leak - Running</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Thread Leak Scenario</h1>
        <asp:Label ID="lblStatus" runat="server" Text="Running thread leak scenario..." />
    </form>
</body>
</html> 