<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario 2 - Thread Contention</title>
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
            <h1>High CPU Scenario 2 - Thread Contention</h1>
            <p class="description">
                This scenario demonstrates thread contention by creating multiple threads that compete for the same lock.
                Each thread will try to acquire a lock, perform some work, and then release it, causing high CPU usage
                due to thread context switching and lock contention.
            </p>
            
            <asp:Button ID="btnStartContention" runat="server" Text="Start Thread Contention" 
                CssClass="button" OnClick="btnStartContention_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate thread contention..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Creating thread contention... This will cause high CPU usage.
            </div>
        </div>
    </form>
</body>
</html> 