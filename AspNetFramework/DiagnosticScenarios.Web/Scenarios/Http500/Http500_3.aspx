<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Http500_3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Http500.Http500_3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTTP 500 Scenario 3 - Configuration Error</title>
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
            <h1>HTTP 500 Scenario 3 - Configuration Error</h1>
            <p class="description">
                This scenario demonstrates an HTTP 500 error caused by a configuration issue.
                Clicking the button will attempt to access a configuration setting that doesn't exist,
                resulting in a configuration error and an HTTP 500 response.
            </p>
            
            <asp:Button ID="btnConfigError" runat="server" Text="Simulate Configuration Error" 
                CssClass="button" OnClick="btnStartError_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate configuration error..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating configuration error... This will cause an HTTP 500 error.
            </div>
        </div>
    </form>
</body>
</html> 