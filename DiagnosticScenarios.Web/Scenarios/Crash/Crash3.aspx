<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Crash3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.Crash.Crash3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crash Scenario 3 - Out of Memory</title>
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
            <h1>Crash Scenario 3 - Out of Memory</h1>
            <p class="description">
                This scenario demonstrates an out-of-memory exception by allocating large arrays
                until the system runs out of memory. Clicking the button will start allocating
                memory until the application crashes.
            </p>
            
            <asp:Button ID="btnOutOfMemory" runat="server" Text="Cause Out of Memory" 
                CssClass="button" OnClick="btnOutOfMemory_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate out of memory..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Causing out of memory... This will crash the application.
            </div>
        </div>
    </form>
</body>
</html> 