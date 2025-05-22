<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Http500_2Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Http500.Http500_2Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTTP 500 - Null Reference Exception</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>HTTP 500 - Null Reference Exception</h1>
        <asp:Label ID="lblStatus" runat="server" Text="Simulating null reference exception..." />
    </form>
</body>
</html> 