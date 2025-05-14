<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Deadlock1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Deadlock.Deadlock1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Deadlock Scenario 1</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            line-height: 1.6;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
        }
        .description {
            background-color: #f8f9fa;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
        }
        .button {
            padding: 10px 20px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }
        .button:hover {
            background-color: #c82333;
        }
        .progress {
            margin-top: 20px;
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Deadlock Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a deadlock condition by creating two threads that acquire locks in different orders.</p>
                <p>Click the button below to start the deadlock simulation.</p>
            </div>

            <asp:Button ID="btnStartDeadlock" runat="server" Text="Start Deadlock Simulation" 
                       CssClass="button" OnClick="btnStartDeadlock_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
            
            <div id="progress" class="progress" runat="server">
                <p>Simulating deadlock... This may take a while.</p>
            </div>
        </div>
    </form>
</body>
</html> 