using System;
using System.Data;
using System.Data.SqlClient;

namespace SimpleReminderApplication
{
    internal class TaskRepository
    {
        // Replace this with your actual connection string
        private string _connectionString = "Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;TrustServerCertificate=True;";

        public TaskRepository()
        {
            // You would also create your 'Tasks' table in SSMS here
            // with columns: Id (uniqueidentifier), Description (nvarchar), DueDate (datetime), Status (int)
        }

        /// <summary>
        /// Adds a new TaskItem to the database.
        /// </summary>
        public void AddTask(TaskItem task)
        {
            // Use 'using' blocks to ensure connections are closed
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // ALWAYS use parameters to prevent SQL Injection
                string sql = "INSERT INTO Tasks (Id, Description, DueDate, Status) VALUES (@Id, @Description, @DueDate, @Status)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add parameters
                    cmd.Parameters.AddWithValue("@Id", task.Id);
                    cmd.Parameters.AddWithValue("@Description", task.Description);
                    cmd.Parameters.AddWithValue("@DueDate", task.DueDate);
                    cmd.Parameters.AddWithValue("@Status", (int)task.Status); // Store enum as an integer

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Run the command
                }
            }
        }

        // You will add other functions here...
        // public void UpdateTask(TaskItem task) { ... }
        // public void DeleteTask(Guid id) { ... }
        // public List<TaskItem> GetAllTasks() { ... }
    }
}