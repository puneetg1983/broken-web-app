<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighMemory3.aspx.cs" Inherits="DiagnosticScenarios.Web.Scenarios.HighMemory.HighMemory3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>High Memory Scenario 3 - LOH Fragmentation</title>
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
            <h1>High Memory Scenario 3 - Large Object Heap Fragmentation</h1>
            <p class="description">
                This scenario demonstrates Large Object Heap (LOH) fragmentation by allocating and releasing
                large objects of different sizes. This creates holes in the LOH that can't be compacted,
                leading to out of memory exceptions even when there appears to be enough free memory.
            </p>
            
            <asp:Button ID="btnStartFragmentation" runat="server" Text="Start LOH Fragmentation" 
                CssClass="button" OnClick="btnStartFragmentation_Click" />
            
            <asp:Label ID="lblStatus" runat="server" Text="Ready to simulate LOH fragmentation..."></asp:Label>
            
            <div id="progress" runat="server" class="progress">
                Creating LOH fragmentation... This will consume memory.
            </div>
        </div>
    </form>
</body>
</html> 