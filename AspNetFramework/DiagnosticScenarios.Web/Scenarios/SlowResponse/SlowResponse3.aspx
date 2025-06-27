<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SlowResponse3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.SlowResponse.SlowResponse3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Slow Response Scenario 3</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Slow Response Scenario 3</h1>
            <p>This scenario demonstrates a slow response due to multiple long-running operations.</p>
            <asp:Label ID="lblStatus" runat="server" Text="Ready to start long-running task."></asp:Label>
            <br /><br />
            <asp:Button ID="btnStartLongTask" runat="server" Text="Start Long Task" OnClick="btnStartLongTask_Click" />
        </div>
    </form>
</body>
</html> 