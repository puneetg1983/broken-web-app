<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Http500_1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Http500.Http500_1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTTP 500 Scenario 1 - Database Connection Error</title>
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
            <h1>HTTP 500 Scenario 1 - Database Connection Error</h1>
            <p class="description">
                This scenario demonstrates an HTTP 500 error caused by a database connection failure.
                Clicking the button will attempt to connect to a non-existent database server,
                resulting in a connection error and an HTTP 500 response.
            </p>
            
            <asp:Button ID="btnDatabaseError" runat="server" Text="Simulate Database Error" 
                CssClass="button" OnClick="btnDatabaseError_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate database error..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating database connection error... This will cause an HTTP 500 error.
            </div>
        </div>
    </form>
</body>
</html> 