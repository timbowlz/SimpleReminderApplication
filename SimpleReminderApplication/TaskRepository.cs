    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Configuration;

    namespace SimpleReminderApplication
    {
        internal class TaskRepository
        {
            private string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            public void AddTask(TaskItem task)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        string sql = "INSERT INTO Tasks (Id, Description, DueDate, Status) VALUES (@Id, @Description, @DueDate, @Status)";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", task.Id);
                            cmd.Parameters.AddWithValue("@Description", task.Description);
                            cmd.Parameters.AddWithValue("@DueDate", task.DueDate);
                            cmd.Parameters.AddWithValue("@Status", (int)task.Status);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to add task to repository.", ex);
                }
            }

            public void UpdateTask(TaskItem task)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        string sql = "UPDATE Tasks SET Description = @Description, DueDate = @DueDate, Status = @Status WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Description", task.Description);
                            cmd.Parameters.AddWithValue("@DueDate", task.DueDate);
                            cmd.Parameters.AddWithValue("@Status", (int)task.Status);
                            cmd.Parameters.AddWithValue("@Id", task.Id);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update task in repository.", ex);
                }
            }

            public void DeleteTask(Guid id)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        string sql = "DELETE FROM Tasks WHERE Id = @Id";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to delete task from repository.", ex);
                }
            }

            public List<TaskItem> GetAllTasks()
            {
                try
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
                catch (Exception ex)
                {
                    throw new Exception("Failed to retrieve tasks from repository.", ex);
                }
            }

            public void DeleteAllCompletedTasks()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(_connectionString))
                    {
                        string sql = "DELETE FROM Tasks WHERE Status = @Status";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Status", (int)TaskStatus.Completed);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to delete completed tasks from repository.", ex);
                }
            }
        }
    }