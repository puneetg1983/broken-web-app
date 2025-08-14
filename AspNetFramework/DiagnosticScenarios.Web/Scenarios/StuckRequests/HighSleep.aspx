<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighSleep.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.StuckRequests.HighSleep" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Sleep Scenario</title>
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
        .warning {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>High Sleep Scenario</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a stuck request by creating a page that sleeps for an extended period (300 seconds).</p>
                <p>This is useful for testing scenarios where requests get stuck and don't complete, potentially leading to thread pool exhaustion.</p>
            </div>

            <div class="warning">
                <strong>Warning:</strong> This operation will create a request that will not complete for 5 minutes (300 seconds). 
                Use this carefully in testing environments only.
            </div>

            <asp:Button ID="btnStartHighSleep" runat="server" Text="Start High Sleep Operation" 
                       CssClass="button" OnClick="btnStartHighSleep_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
        </div>
    </form>
</body>
</html>
