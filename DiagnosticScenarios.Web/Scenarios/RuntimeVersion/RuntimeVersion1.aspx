<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RuntimeVersion1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.RuntimeVersion.RuntimeVersion1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Incompatible Runtime Version Scenario</title>
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
            <h1>Incompatible Runtime Version Scenario</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates an application running on an incompatible .NET runtime version by attempting to use features from a newer .NET version.</p>
                <p>Click the button below to start the simulation. The application will attempt to use features that are not available in the current runtime version.</p>
            </div>

            <asp:Button ID="btnTriggerError" runat="server" Text="Start Runtime Version Simulation" 
                CssClass="button" OnClick="btnTriggerError_Click" />
            
            <asp:Label ID="lblStatus" runat="server" CssClass="status"></asp:Label>
            
            <div id="progress" runat="server" class="progress"></div>
        </div>
    </form>
</body>
</html> 