<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Threading" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // Log the restart request
            System.Diagnostics.Trace.WriteLine($"[{DateTime.UtcNow}] Restart requested via RestartWebApp.aspx");
            
            // Start a new thread to handle the exit
            // This ensures the response is sent back to the client before the app exits
            Thread exitThread = new Thread(() =>
            {
                // Give a small delay to ensure the response is sent
                Thread.Sleep(1000);
                Environment.Exit(0);
            });
            
            exitThread.Start();
            
            // Send success response
            Response.ContentType = "application/json";
            Response.Write("{\"status\":\"success\",\"message\":\"Restart initiated\"}");
        }
        catch (Exception ex)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = 500;
            Response.Write($"{{\"status\":\"error\",\"message\":\"{ex.Message}\"}}");
        }
    }
</script> 