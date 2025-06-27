using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Threading;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory2 : Page
    {
        // Static list to hold event handlers - this is the leak!
        private static readonly List<EventHandler> _leakedHandlers = new List<EventHandler>();
        private static int _handlerCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate event handler leak...";
            }
        }

        protected void btnStartHighMemory_Click(object sender, EventArgs e)
        {
            Response.Redirect("HighMemory2Actual.aspx");
        }

        protected void btnStartLeak_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartLeak.Enabled = false;
                progress.Visible = true;

                // Create a new event handler that will never be unsubscribed
                EventHandler handler = (s, args) =>
                {
                    // Create a large string to consume memory
                    string largeString = new string('x', 1024 * 1024); // 1MB string
                    System.Diagnostics.Debug.WriteLine($"Handler {_handlerCount} executed with {largeString.Length} bytes");
                };

                // Add the handler to our static list - this is the leak!
                _leakedHandlers.Add(handler);
                _handlerCount++;

                // Subscribe to a static event
                StaticEventSource.SomeEvent += handler;

                lblStatus.Text = $"Created event handler leak #{_handlerCount}. Memory will grow with each click.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartLeak.Enabled = true;
                progress.Visible = false;
            }
        }
    }

    // Static class to hold our event
    public static class StaticEventSource
    {
        public static event EventHandler SomeEvent;

        public static void RaiseEvent()
        {
            SomeEvent?.Invoke(null, EventArgs.Empty);
        }
    }
} 