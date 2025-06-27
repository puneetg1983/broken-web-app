using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Diagnostics;

namespace DiagnosticScenarios.Web.Scenarios.ThreadLeak
{
    public partial class ThreadLeak1Actual : Page
    {
        private static readonly List<Thread> _leakedThreads = new List<Thread>();
        private static bool _isRunning = false;
        private static readonly object _lock = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Start thread creation synchronously to ensure it completes
            StartThreadLeak();
        }

        private void StartThreadLeak()
        {
            try
            {
                _isRunning = true;
                int threadCount = 0;

                // Create multiple threads that will never terminate
                for (int i = 0; i < 10; i++)
                {
                    Thread thread = new Thread(() =>
                    {
                        try
                        {
                            while (_isRunning)
                            {
                                // Simulate some work
                                Thread.Sleep(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Thread error: {ex.Message}");
                        }
                    });

                    thread.IsBackground = false; // Make threads foreground threads
                    thread.Start();
                    
                    lock (_lock)
                    {
                        _leakedThreads.Add(thread);
                        threadCount = _leakedThreads.Count;
                    }

                    Debug.WriteLine($"Created thread {i + 1}, total threads: {threadCount}");
                }

                lblStatus.Text = $"Started {threadCount} threads that will never terminate.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                Debug.WriteLine($"Error creating threads: {ex.Message}");
            }
        }
    }
} 