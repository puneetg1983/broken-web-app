<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StackOverflow1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Crash.StackOverflow1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Stack Overflow Scenario 1</title>
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
            color: #dc3545;
            font-weight: bold;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Stack Overflow Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a stack overflow by using infinite recursion.</p>
                <p>Click the button below to trigger the stack overflow.</p>
            </div>
            
            <asp:Button  CssClass="button" ID="btnTriggerActual" runat="server" Text="Trigger Actual Scenario" OnClick="btnTriggerActual_Click" />
            
            <div class="warning">
                <p>Warning: This will crash the application. Make sure you have saved any important work before proceeding.</p>
            </div>
        </div>
    </form>
</body>
</html> 