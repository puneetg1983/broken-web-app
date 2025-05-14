<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighCpu1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighCpu.HighCpu1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High CPU Scenario 1</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>High CPU Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates high CPU usage by performing intensive mathematical calculations in a tight loop.</p>
                <p>Click the button below to start the CPU-intensive operation.</p>
            </div>

            <asp:Button ID="btnStartHighCpu" runat="server" Text="Start High CPU Operation" 
                       CssClass="button" OnClick="btnStartHighCpu_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
        </div>
    </form>
</body>
</html> 