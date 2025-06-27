<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighMemory2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighMemory.HighMemory2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Memory Scenario 2 - Event Handler Leak</title>
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
            <h1>High Memory Scenario 2 - Event Handler Leak</h1>
            <p class="description">
                This scenario demonstrates a common memory leak pattern where event handlers are not properly unsubscribed.
                Each time you click the button, it creates a new event handler that is never cleaned up, causing memory to grow
                until the application pool is recycled.
            </p>
            
            <asp:Button ID="btnStartLeak" runat="server" Text="Create Event Handler Leak" 
                CssClass="button" OnClick="btnStartLeak_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate event handler leak..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Creating event handler leak... This will consume memory.
            </div>
        </div>
    </form>
</body>
</html> 