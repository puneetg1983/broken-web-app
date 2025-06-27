<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario 3 - Complex Regex</title>
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
            <h1>High CPU Scenario 3 - Complex Regex</h1>
            <p class="description">
                This scenario demonstrates high CPU usage through complex regular expression operations.
                It will perform multiple regex matches with nested patterns and backtracking,
                causing significant CPU utilization while still returning a response.
            </p>
            
            <asp:Button ID="btnStartHighCpu" runat="server" Text="Start High CPU" 
                CssClass="button" OnClick="btnStartHighCpu_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate high CPU usage..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Running complex regex operations... This will cause high CPU usage.
            </div>
        </div>
    </form>
</body>
</html> 