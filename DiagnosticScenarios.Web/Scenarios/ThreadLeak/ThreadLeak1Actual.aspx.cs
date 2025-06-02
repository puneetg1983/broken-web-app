using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.ThreadLeak
{
    public partial class ThreadLeak1Actual : Page
    {
        private static readonly List<Thread> _leakedThreads = new List<Thread>();
        private static bool _isRunning = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                StartThreadLeak();
            }
        }

        private void StartThreadLeak()
        {
            try
            {
                _isRunning = true;
                int threadCount = 0;

                // Create multiple threads that will never terminate
                for (int i = 0; i < 50; i++)
                {
                    Thread thread = new Thread(() =>
                    {
                        while (_isRunning)
                        {
                            // Simulate some work
                            Thread.Sleep(1000);
                        }
                    });

                    thread.Start();
                    _leakedThreads.Add(thread);
                    threadCount++;
                }

                lblStatus.Text = $"Started {threadCount} threads that will never terminate.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _isRunning = false;
            base.OnUnload(e);
        }
    }
} 