<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowDependency1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowDependency.SlowDependency1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Dependency - API Call</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Slow Dependency - API Call</h1>
        <asp:Label ID="lblStatus" runat="server" Text="Running slow API call scenario..." />
    </form>
</body>
</html> 