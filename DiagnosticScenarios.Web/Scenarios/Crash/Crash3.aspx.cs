using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.Crash
{
    public partial class Crash3 : Page
    {
        private static readonly List<byte[]> _memoryHog = new List<byte[]>();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate out of memory...";
            }
        }

        protected void btnOutOfMemory_Click(object sender, EventArgs e)
        {
            try
            {
                btnOutOfMemory.Enabled = false;
                progress.Visible = true;
                _isRunning = true;

                // Start a background thread to allocate memory
                Thread thread = new Thread(() =>
                {
                    int allocationCount = 0;
                    const int allocationSize = 100 * 1024 * 1024; // 100MB

                    while (_isRunning)
                    {
                        try
                        {
                            // Allocate a large array
                            byte[] largeArray = new byte[allocationSize];
                            
                            // Fill the array with data to ensure it's actually allocated
                            for (int i = 0; i < allocationSize; i++)
                            {
                                largeArray[i] = (byte)(i % 256);
                            }

                            // Add to our list to prevent GC from collecting it
                            _memoryHog.Add(largeArray);
                            allocationCount++;

                            // Update status
                            lblStatus.Text = $"Allocated {allocationCount} * 100MB = {allocationCount * 100}MB of memory";
                        }
                        catch (OutOfMemoryException)
                        {
                            lblStatus.Text = "Out of memory exception occurred!";
                            break;
                        }
                    }
                });

                thread.Start();

                lblStatus.Text = "Started out of memory scenario. The application will crash when memory is exhausted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnOutOfMemory.Enabled = true;
                progress.Visible = false;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }
    }
} 