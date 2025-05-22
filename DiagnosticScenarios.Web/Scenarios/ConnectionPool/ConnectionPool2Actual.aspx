<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConnectionPool2Actual.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.ConnectionPool.ConnectionPool2Actual" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Connection Pool - Leaks</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Connection Pool - Leaks</h1>
        <asp:Label ID="lblStatus" runat="server" Text="Running connection pool leak scenario..." />
 