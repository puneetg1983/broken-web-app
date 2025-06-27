<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnectionPool3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ConnectionPool.ConnectionPool3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connection Pool Scenario 3 - Pool Exhaustion</title>
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
            <h1>Connection Pool Scenario 3 - Pool Exhaustion</h1>
            <p class="description">
                This scenario demonstrates connection pool exhaustion by creating multiple long-running
                database operations that hold connections open. Clicking the button will start multiple
                threads that each hold a connection open for an extended period, eventually exhausting
                the connection pool.
            </p>
            
            <asp:Button ID="btnExhaustPool" runat="server" Text="Start Pool Exhaustion" 
                CssClass="button" OnClick="btnExhaustPool_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate pool exhaustion..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Simulating pool exhaustion... This will exhaust the connection pool.
            </div>
        </div>
    </form>
</body>
</html> 