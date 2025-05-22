using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace DiagnosticScenarios.Web.Scenarios.Deadlock
{
    public partial class Deadlock1Actual : System.Web.UI.Page
    {
        private static readonly string ConnectionString = "Server=localhost;Database=TestDB;Trusted_Connection=True;";
        private static readonly object Lock1 = new object();
        private static readonly object Lock2 = new object();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Create two tasks that will deadlock
                var task1 = Task.Run(() => DeadlockTask1());
                var task2 = Task.Run(() => DeadlockTask2());

                // Wait for both tasks to complete or timeout
                Task.WaitAll(new[] { task1, task2 }, TimeSpan.FromSeconds(30));
            }
            catch (AggregateException ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Deadlock error: {ex.InnerException?.Message}");
                throw;
            }
        }

        private void DeadlockTask1()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First update table A
                        using (var command = new SqlCommand("UPDATE TableA SET Value = Value + 1 WHERE Id = 1", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Simulate some work
                        Thread.Sleep(1000);

                        // Then try to update table B
                        using (var command = new SqlCommand("UPDATE TableB SET Value = Value + 1 WHERE Id = 1", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void DeadlockTask2()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First update table B
                        using (var command = new SqlCommand("UPDATE TableB SET Value = Value + 1 WHERE Id = 1", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // Simulate some work
                        Thread.Sleep(1000);

                        // Then try to update table A
                        using (var command = new SqlCommand("UPDATE TableA SET Value = Value + 1 WHERE Id = 1", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
} 