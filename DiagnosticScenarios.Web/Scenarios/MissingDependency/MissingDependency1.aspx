<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MissingDependency1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.MissingDependency.MissingDependency1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Missing Dependency Scenario</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Missing Dependency Scenario</h1>
            <p class="description">
                This scenario demonstrates an application failure due to a missing NuGet package dependency.
                Clicking the button will attempt to use a class from a missing dependency,
                resulting in a runtime error.
            </p>
            
            <asp:Button ID="btnTriggerError" runat="server" Text="Trigger Missing Dependency Error" 
                CssClass="button" OnClick="btnTriggerError_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate missing dependency error..."></asp:Label>
        </div>
    </form>
</body>
</html> 