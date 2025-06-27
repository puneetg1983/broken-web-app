<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighConnections1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighConnections.HighConnections1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Outbound Connections Scenario</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .container { max-width: 800px; margin: 0 auto; }
        .description { margin: 20px 0; padding: 15px; background-color: #f8f9fa; border-radius: 5px; }
        .button { padding: 10px 20px; background-color: #007bff; color: white; border: none; border-radius: 5px; cursor: pointer; }
        .button:hover { background-color: #0056b3; }
        .status { margin-top: 20px; padding: 10px; border-radius: 5px; }
        .progress { margin-top: 20px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>High Outbound Connections Scenario</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a high number of outbound TCP connections (close to 2000) by creating multiple connections to various endpoints.</p>
                <p>Click the button below to start the simulation. The application will create multiple outbound connections and maintain them for a period of time.</p>
            </div>

            <asp:Button ID="btnTriggerConnections" runat="server" Text="Start Connection Simulation" 
                CssClass="button" OnClick="btnTriggerConnections_Click" />
            
            <asp:Label ID="lblStatus" runat="server" CssClass="status"></asp:Label>
            
            <div id="progress" runat="server" class="progress"></div>
        </div>
    </form>
</body>
</html> 