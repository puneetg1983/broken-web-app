<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageQuota1Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.StorageQuota.StorageQuota1Actual" Async="true" AsyncTimeout="90" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Storage Quota Exceeded - In Progress</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 800px; margin: 0 auto; }
        .status { margin-top: 20px; padding: 10px; border-radius: 5px; }
        .progress { margin-top: 20px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Storage Quota Exceeded Simulation</h1>
            <asp:Label ID="lblStatus" runat="server" CssClass="status"></asp:Label>
            <div id="progress" runat="server" class="progress"></div>
        </div>
    </form>
</body>
</html> 