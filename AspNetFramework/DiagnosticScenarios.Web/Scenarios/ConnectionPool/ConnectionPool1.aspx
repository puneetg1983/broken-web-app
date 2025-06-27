<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnectionPool1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ConnectionPool.ConnectionPool1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connection Pool Exhaustion Scenario 1</title>
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
            <h1>Connection Pool Exhaustion Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates connection pool exhaustion by creating multiple concurrent database connections without properly disposing them.</p>
                <p>Click the button below to start the connection pool exhaustion simulation.</p>
            </div>

            <asp:Button ID="btnStartConnectionPool" runat="server" Text="Start Connection Pool Exhaustion" 
                       CssClass="button" OnClick="btnStartConnectionPool_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
            
            <div id="progress" class="progress" runat="server">
                <p>Creating database connections... This may take a while.</p>
            </div>
        </div>
    </form>
</body>
</html> 