<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowDependency2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowDependency.SlowDependency2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Dependency Call Scenario 2 - Timeout</title>
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
            <h1>Slow Dependency Call Scenario 2 - Timeout</h1>
            <p class="description">
                This scenario simulates a timeout when calling an external service. It uses HTTPBIN.org's delay endpoint
                with a timeout that will be exceeded, demonstrating how to handle timeouts in external service calls.
            </p>
            
            <asp:Button ID="btnStartTimeout" runat="server" Text="Start Timeout Test" 
                CssClass="button" OnClick="btnStartTimeout_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate timeout..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Calling external service with timeout... This may take a moment.
            </div>
        </div>
    </form>
</body>
</html> 