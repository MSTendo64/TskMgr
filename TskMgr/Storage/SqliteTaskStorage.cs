using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace TskMgr
{
    public class SqliteTaskStorage : ITaskStorage
    {
        public Dictionary<int, Task> tasks { get; private set; }
        private string connectionString;
        private string databasePath;

        public SqliteTaskStorage(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appFolder = Path.Combine(appDataPath, "TaskManager");

                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }

                databasePath = Path.Combine(appFolder, "tasks.db");
            }
            else
            {
                databasePath = path + ".db";
            }

            connectionString = $"Data Source={databasePath};Version=3;";
            tasks = new Dictionary<int, Task>();

            InitializeDatabase();
            Load();
        }

        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(databasePath))
                {
                    SQLiteConnection.CreateFile(databasePath);
                }

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Priority INTEGER NOT NULL,
                        Status INTEGER NOT NULL,
                        CreateDate TEXT NOT NULL,
                        DeadLine TEXT
                    )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Создаем индекс для быстрого поиска
                    string createIndexQuery = @"
                    CREATE INDEX IF NOT EXISTS idx_tasks_name ON Tasks(Name);
                    CREATE INDEX IF NOT EXISTS idx_tasks_priority ON Tasks(Priority);
                    CREATE INDEX IF NOT EXISTS idx_tasks_status ON Tasks(Status);";

                    using (var command = new SQLiteCommand(createIndexQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"База данных SQLite создана/подключена: {databasePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации SQLite: {ex.Message}");
                throw;
            }
        }

        public void Save()
        {
            // В SQLite каждая операция сохраняется автоматически
            // Этот метод может быть пустым или использоваться для batch операций
            Console.WriteLine("Данные сохранены в SQLite");
        }

        public void Load()
        {
            try
            {
                tasks.Clear();

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM Tasks ORDER BY Id";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var task = new Task(
                                reader["Name"].ToString(),
                                reader["Description"].ToString(),
                                (TaskPriority)Convert.ToInt32(reader["Priority"]),
                                (TaskStatus)Convert.ToInt32(reader["Status"]),
                                DateTime.Parse(reader["CreateDate"].ToString()),
                                reader["DeadLine"] != DBNull.Value ? DateTime.Parse(reader["DeadLine"].ToString()) : (DateTime?)null
                            );

                            int id = Convert.ToInt32(reader["Id"]);
                            tasks.Add(id, task);
                        }
                    }
                }

                Console.WriteLine($"Загружено {tasks.Count} задач из SQLite");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки из SQLite: {ex.Message}");
                tasks = new Dictionary<int, Task>();
            }
        }

        public void AddTask(Task task)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                    INSERT INTO Tasks (Name, Description, Priority, Status, CreateDate, DeadLine)
                    VALUES (@Name, @Description, @Priority, @Status, @CreateDate, @DeadLine);
                    SELECT last_insert_rowid();";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", task.Name);
                        command.Parameters.AddWithValue("@Description", task.Description);
                        command.Parameters.AddWithValue("@Priority", (int)task.Priority);
                        command.Parameters.AddWithValue("@Status", (int)task.Status);
                        command.Parameters.AddWithValue("@CreateDate", task.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@DeadLine", task.DeadLine.HasValue ?
                            task.DeadLine.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);

                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        tasks.Add(newId, task);
                    }
                }

                Console.WriteLine($"Задача добавлена в SQLite с ID: {tasks.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления задачи в SQLite: {ex.Message}");
                throw;
            }
        }

        public void UpdateTask(int id, Task task)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string updateQuery = @"
                    UPDATE Tasks 
                    SET Name = @Name, 
                        Description = @Description, 
                        Priority = @Priority, 
                        Status = @Status, 
                        DeadLine = @DeadLine
                    WHERE Id = @Id";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", task.Name);
                        command.Parameters.AddWithValue("@Description", task.Description);
                        command.Parameters.AddWithValue("@Priority", (int)task.Priority);
                        command.Parameters.AddWithValue("@Status", (int)task.Status);
                        command.Parameters.AddWithValue("@DeadLine", task.DeadLine.HasValue ?
                            task.DeadLine.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            tasks[id] = task;
                            Console.WriteLine($"Задача {id} обновлена в SQLite");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления задачи в SQLite: {ex.Message}");
                throw;
            }
        }

        public void DeleteTask(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Tasks WHERE Id = @Id";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            tasks.Remove(id);
                            Console.WriteLine($"Задача {id} удалена из SQLite");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления задачи из SQLite: {ex.Message}");
                throw;
            }
        }

        public void Clear()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Tasks";
                    string resetIdQuery = "DELETE FROM sqlite_sequence WHERE name='Tasks'";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SQLiteCommand(resetIdQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    tasks.Clear();
                    Console.WriteLine("Все задачи удалены из SQLite");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка очистки SQLite: {ex.Message}");
                throw;
            }
        }

        // Дополнительные методы для SQLite
        public List<Task> SearchTasks(string keyword)
        {
            var results = new List<Task>();

            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string searchQuery = @"
                    SELECT * FROM Tasks 
                    WHERE Name LIKE @Keyword 
                       OR Description LIKE @Keyword
                    ORDER BY Id";

                    using (var command = new SQLiteCommand(searchQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new Task(
                                    reader["Name"].ToString(),
                                    reader["Description"].ToString(),
                                    (TaskPriority)Convert.ToInt32(reader["Priority"]),
                                    (TaskStatus)Convert.ToInt32(reader["Status"]),
                                    DateTime.Parse(reader["CreateDate"].ToString()),
                                    reader["DeadLine"] != DBNull.Value ? DateTime.Parse(reader["DeadLine"].ToString()) : (DateTime?)null
                                );

                                results.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска в SQLite: {ex.Message}");
            }

            return results;
        }
    }
}