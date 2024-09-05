using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace toDoList
{
    public class DatabaseHelper
    {
        private readonly string connectionString = "Data Source=ToDoListDB.db;Version=3;";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                string createTaskTableQuery = @"
                    CREATE TABLE IF NOT EXISTS TableTask (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Description TEXT,
                    Type INTEGER,
                    StartDate TEXT,
                    EndDate TEXT
                )";

                string createBlackoutDatesTableQuery = @"
                    CREATE TABLE IF NOT EXISTS BlackoutDates (
                    BlackoutDate TEXT PRIMARY KEY
                )";

                SQLiteCommand createTaskTableCommand = new(createTaskTableQuery, connection);
                createTaskTableCommand.ExecuteNonQuery();

                SQLiteCommand createBlackoutDatesTableCommand = new(createBlackoutDatesTableQuery, connection);
                createBlackoutDatesTableCommand.ExecuteNonQuery();
            }
        }

        public void InsertTask(Task task)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "INSERT INTO TableTask (Title, Description, Type, StartDate, EndDate) VALUES (@Title, @Description, @Type, @StartDate, @EndDate)";
                SQLiteCommand command = new(query, connection);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@Type", (int)task.Type);
                command.Parameters.AddWithValue("@StartDate", task.StartDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@EndDate", task.EndDate.ToString("yyyy-MM-dd"));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public List<Task> GetTask()
        {
            List<Task> tasks = [];

            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "SELECT Title, Description, Type, StartDate, EndDate FROM TableTask";
                SQLiteCommand command = new(query, connection);

                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new Task
                    {
                        Title = reader["Title"].ToString(),
                        Description = reader["Description"].ToString(),
                        Type = (TaskType)reader.GetInt32(reader.GetOrdinal("Type")),
                        StartDate = DateTime.Parse(reader["StartDate"].ToString()).Date,
                        EndDate = DateTime.Parse(reader["EndDate"].ToString()).Date,
                    });
                }
            }

            return tasks;
        }

        public void DeleteTask(Task task)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "DELETE FROM TableTask WHERE Title = @Title AND Description = @Description AND Type = @Type AND StartDate = @StartDate AND EndDate = @EndDate";
                SQLiteCommand command = new(query, connection);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@Type", (int)task.Type);
                command.Parameters.AddWithValue("@StartDate", task.StartDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@EndDate", task.EndDate.ToString("yyyy-MM-dd"));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void InsertBlackoutDate(DateTime date)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "INSERT INTO BlackoutDates (BlackoutDate) VALUES (@BlackoutDate)";
                SQLiteCommand command = new(query, connection);
                command.Parameters.AddWithValue("@BlackoutDate", date.ToString("yyyy-MM-dd"));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteBlackoutDate(DateTime date)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "DELETE FROM BlackoutDates WHERE BlackoutDate = @BlackoutDate";
                SQLiteCommand command = new(query, connection);
                command.Parameters.AddWithValue("@BlackoutDate", date.ToString("yyyy-MM-dd"));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<DateTime> GetBlackoutDates()
        {
            List<DateTime> dates = new();

            using (SQLiteConnection connection = new(connectionString))
            {
                string query = "SELECT BlackoutDate FROM BlackoutDates";
                SQLiteCommand command = new(query, connection);

                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dates.Add(DateTime.Parse(reader["BlackoutDate"].ToString()));
                }
            }

            return dates;
        }
    }
}
