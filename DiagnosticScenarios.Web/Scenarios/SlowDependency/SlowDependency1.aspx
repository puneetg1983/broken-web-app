<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowDependency1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowDependency.SlowDependency1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Dependency Call Scenario 1</title>
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
            <h1>Slow Dependency Call Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a slow dependency call by introducing artificial delays in the external service call.</p>
                <p>Click the button below to start the slow dependency call operation.</p>
            </div>

            <asp:Button ID="btnStartSlowDependency" runat="server" Text="Start Slow Dependency Call" 
                       CssClass="button" OnClick="btnStartSlowDependency_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
            
            <div id="progress" class="progress" runat="server">
                <p>Calling external service... This may take a while.</p>
            </div>
        </div>
    </form>
</body>
</html> 