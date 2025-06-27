<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Crash2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Crash.Crash2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crash Scenario 2 - Stack Overflow</title>
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
            <h1>Crash Scenario 2 - Stack Overflow</h1>
            <p class="description">
                This scenario demonstrates a stack overflow exception by using infinite recursion.
                Clicking the button will start a recursive method that will eventually cause
                a stack overflow and crash the application.
            </p>
            
            <asp:Button ID="btnStackOverflow" runat="server" Text="Cause Stack Overflow" 
                CssClass="button" OnClick="btnStackOverflow_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate stack overflow..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Causing stack overflow... This will crash the application.
            </div>
        </div>
    </form>
</body>
</html> 