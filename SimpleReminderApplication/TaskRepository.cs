using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SimpleReminderApplication
{
    internal class TaskRepository
    {
        private string _connectionString = "Server=VENN\\SQLEXPRESS;Database=RemindersDB;Integrated Security=True;TrustServerCertificate=True;";

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

        public void UpdateTask(TaskItem task)
        {
            // Use 'using' blocks to ensure connections are closed
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // The UPDATE command - finds the row by Id
                string sql = "UPDATE Tasks SET Description = @Description, DueDate = @DueDate, Status = @Status WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add parameters for all fields
                    cmd.Parameters.AddWithValue("@Description", task.Description);
                    cmd.Parameters.AddWithValue("@DueDate", task.DueDate);
                    cmd.Parameters.AddWithValue("@Status", (int)task.Status);
                    cmd.Parameters.AddWithValue("@Id", task.Id); // The WHERE clause

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Run the command
                }
            }
        }

        public void DeleteTask(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM Tasks WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add the Id parameter
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Run the delete command
                }
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id, Description, DueDate, Status FROM Tasks";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create task from database data
                            TaskItem task = new TaskItem(
                                reader.GetString(reader.GetOrdinal("Description")),
                                reader.GetDateTime(reader.GetOrdinal("DueDate"))
                            )
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Status = (TaskStatus)reader.GetInt32(reader.GetOrdinal("Status"))
                            };
                            tasks.Add(task);
                        }
                    }
                }
            }
            return tasks;
        }

        public void DeleteAllCompletedTasks()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // We delete where Status = 2 (which is TaskStatus.Completed)
                string sql = "DELETE FROM Tasks WHERE Status = @Status";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Use the enum value to ensure it's always correct
                    cmd.Parameters.AddWithValue("@Status", (int)TaskStatus.Completed);

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Run the delete command
                }
            }
        }
    }
}