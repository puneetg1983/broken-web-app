using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DiagnosticScenarios.Web.Scenarios.HighMemory
{
    public partial class HighMemory3 : Page
    {
        // Static list to hold our large objects
        private static readonly List<byte[]> _largeObjects = new List<byte[]>();
        private static int _iterationCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = "Ready to simulate LOH fragmentation...";
            }
        }

        protected void btnStartFragmentation_Click(object sender, EventArgs e)
        {
            try
            {
                btnStartFragmentation.Enabled = false;
                progress.Visible = true;

                // Create objects of different sizes to fragment the LOH
                for (int i = 0; i < 10; i++)
                {
                    // Create objects of different sizes (all > 85KB to go to LOH)
                    int size = 85 * 1024 + (i * 1024); // Start at 85KB and increase by 1KB each time
                    byte[] largeObject = new byte[size];
                    
                    // Fill with some data
                    for (int j = 0; j < size; j++)
                    {
                        largeObject[j] = (byte)(j % 256);
                    }

                    _largeObjects.Add(largeObject);
                }

                // Remove every other object to create fragmentation
                for (int i = _largeObjects.Count - 1; i >= 0; i -= 2)
                {
                    _largeObjects.RemoveAt(i);
                }

                // Force GC to see the fragmentation
                GC.Collect();
                GC.WaitForPendingFinalizers();

                _iterationCount++;
                lblStatus.Text = $"Created LOH fragmentation iteration #{_iterationCount}. " +
                                $"Memory is fragmented with {_largeObjects.Count} objects remaining.";
            }
            catch (OutOfMemoryException)
            {
                lblStatus.Text = "Out of memory! The LOH is too fragmented to allocate new objects.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnStartFragmentation.Enabled = true;
                progress.Visible = false;
            }
        }
    }
} 