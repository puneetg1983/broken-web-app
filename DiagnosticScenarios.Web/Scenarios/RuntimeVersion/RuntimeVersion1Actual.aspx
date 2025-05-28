<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RuntimeVersion1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.RuntimeVersion.RuntimeVersion1Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Incompatible Runtime Version - Active</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 800px; margin: 0 auto; }
        .status { margin-top: 20px; padding: 10px; border-radius: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Incompatible Runtime Version - Active</h1>
            <asp:Label ID="lblStatus" runat="server" CssClass="status"></asp:Label>
        </div>
    </form>
</body>
</html> 