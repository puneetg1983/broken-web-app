<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Crash1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Crash.Crash1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crash Scenario 1 - Unhandled Exception</title>
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
            <h1>Crash Scenario 1 - Unhandled Exception</h1>
            <p class="description">
                This scenario demonstrates an unhandled exception that will crash the application.
                Clicking the button will throw an unhandled exception in a background thread,
                which will cause the application to crash.
            </p>
            
            <asp:Button ID="btnCrash" runat="server" Text="Crash Application" 
                CssClass="button" OnClick="btnCrash_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate crash..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Crashing application... This will cause an unhandled exception.
            </div>
        </div>
    </form>
</body>
</html> 