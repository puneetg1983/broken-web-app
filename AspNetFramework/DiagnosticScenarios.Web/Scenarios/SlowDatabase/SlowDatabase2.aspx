<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowDatabase2.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowDatabase.SlowDatabase2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Database Call Scenario 2 - Deadlock</title>
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
            <h1>Slow Database Call Scenario 2 - Deadlock</h1>
            <p class="description">
                This scenario simulates a database deadlock by attempting to update the same records in different orders
                from multiple transactions. This will cause SQL Server to detect a deadlock and choose a victim.
            </p>
            
            <asp:Button ID="btnStartDeadlock" runat="server" Text="Start Deadlock Test" 
                CssClass="button" OnClick="btnStartDeadlock_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate deadlock..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating database deadlock... This may take a moment.
            </div>
        </div>
    </form>
</body>
</html> 