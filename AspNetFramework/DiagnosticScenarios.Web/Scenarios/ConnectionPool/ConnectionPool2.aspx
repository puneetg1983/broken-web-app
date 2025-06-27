<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnectionPool2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ConnectionPool.ConnectionPool2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connection Pool Scenario 2 - Connection Timeout</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 800px; margin: 0 auto; }
        .description { margin-bottom: 20px; color: #666; }
        .button { 
            padding: 10px 20px; 
            background-color: #007bff; 
            color: white; 
            border: none; 
            cursor: pointer; 
        }
        .button:hover { background-color: #0056b3; }
        .progress { 
            display: none; 
            margin-top: 20px; 
            padding: 10px; 
            background-color: #f8f9fa; 
            border: 1px solid #ddd; 
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Connection Pool Scenario 2 - Connection Timeout</h1>
            <p class="description">
                This scenario demonstrates a connection timeout by attempting to connect to a non-existent
                database server with a short timeout period. Clicking the button will attempt to establish
                multiple connections simultaneously, causing connection timeouts.
            </p>
            
            <asp:Button ID="btnConnectionTimeout" runat="server" Text="Simulate Connection Timeout" 
                CssClass="button" OnClick="btnConnectionTimeout_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate connection timeout..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating connection timeout... This will cause connection timeouts.
            </div>
        </div>
    </form>
</body>
</html> 