<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario 3 - Deadlock</title>
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
            <h1>High CPU Scenario 3 - Deadlock</h1>
            <p class="description">
                This scenario demonstrates a classic deadlock situation where two threads are waiting for each other's locks.
                Thread 1 holds lock A and waits for lock B, while Thread 2 holds lock B and waits for lock A.
                This will cause high CPU usage as the threads spin while waiting for the locks.
            </p>
            
            <asp:Button ID="btnStartDeadlock" runat="server" Text="Start Deadlock" 
                CssClass="button" OnClick="btnStartDeadlock_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate deadlock..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Creating deadlock... This will cause high CPU usage.
            </div>
        </div>
    </form>
</body>
</html> 