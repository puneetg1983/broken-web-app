<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThreadLeak1.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ThreadLeak.ThreadLeak1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Thread Leak Scenario 1</title>
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
            <h1>Thread Leak Scenario 1</h1>
            
            <div class="description">
                <h2>Description</h2>
                <p>This scenario simulates a thread leak by creating threads that never terminate.</p>
                <p>Click the button below to start the thread leak scenario.</p>
            </div>

            <asp:Button ID="btnStartThreadLeak" runat="server" Text="Start Thread Leak" 
                       CssClass="button" OnClick="btnStartThreadLeak_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="" />
        </div>
    </form>
</body>
</html> 