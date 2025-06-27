<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowResponse2Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowResponse.SlowResponse2Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Long-Running Task in Progress</title>
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
        .progress {
            margin-top: 20px;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Long-Running Task in Progress</h1>
            <div class="progress">
                <asp:Label ID="lblStatus" runat="server" Text="Processing large dataset..." />
            </div>
        </div>
    </form>
</body>
</html> 