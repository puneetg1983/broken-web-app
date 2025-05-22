<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowDependency2Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowDependency.SlowDependency2Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Dependency - File System</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Slow Dependency - File System</h1>
        <asp:Label ID="lblStatus" runat="server" Text="Running slow file system operation scenario..." />
    </form>
</body>
</html> 