<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Http500_4.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Http500.Http500_4" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTTP 500 Scenario 4 - Invalid Connection String</title>
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
            <h1>HTTP 500 Scenario 4 - Invalid Connection String</h1>
            <p class="description">
                This scenario demonstrates an HTTP 500 error caused by an invalid connection string in web.config.
                Clicking the button will attempt to read and use an invalid connection string,
                resulting in a configuration error and an HTTP 500 response.
            </p>
            
            <asp:Button ID="btnInvalidConnectionString" runat="server" Text="Simulate Invalid Connection String Error" 
                CssClass="button" OnClick="btnStartError_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate invalid connection string error..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating invalid connection string error... This will cause an HTTP 500 error.
            </div>
        </div>
    </form>
</body>
</html> 