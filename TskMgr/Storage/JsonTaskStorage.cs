using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TskMgr
{
    public class JsonTaskStorage : ITaskStorage
    {
        public Dictionary<int, Task> tasks { get; private set; }
        private string storagePath;

        public JsonTaskStorage(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appFolder = Path.Combine(appDataPath, "TaskManager");

                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }

                storagePath = Path.Combine(appFolder, "tasks.json");
            }
            else
            {
                storagePath = path + ".json";
            }

            tasks = new Dictionary<int, Task>();
            Load();
        }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(storagePath, json);
                Console.WriteLine($"Данные сохранены в JSON: {storagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения JSON: {ex.Message}");
                throw;
            }
        }

        public void Load()
        {
            try
            {
                if (File.Exists(storagePath))
                {
                    string json = File.ReadAllText(storagePath);
                    var loadedTasks = JsonConvert.DeserializeObject<Dictionary<int, Task>>(json);

                    if (loadedTasks != null)
                    {
                        tasks = loadedTasks;
                        Console.WriteLine($"Загружено {tasks.Count} задач из JSON");
                    }
                    else
                    {
                        tasks = new Dictionary<int, Task>();
                        Console.WriteLine("JSON файл пуст, создан новый список задач");
                    }
                }
                else
                {
                    tasks = new Dictionary<int, Task>();
                    Console.WriteLine($"JSON файл не найден. Создан новый список задач. Путь: {storagePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки JSON: {ex.Message}");
                tasks = new Dictionary<int, Task>();
            }
        }

        public void AddTask(Task task)
        {
            int newId = GetNextId();
            tasks.Add(newId, task);
            Save();
        }

        public void UpdateTask(int id, Task task)
        {
            if (tasks.ContainsKey(id))
            {
                tasks[id] = task;
                Save();
            }
        }

        public void DeleteTask(int id)
        {
            if (tasks.ContainsKey(id))
            {
                tasks.Remove(id);
                Save();
            }
        }

        public void Clear()
        {
            tasks.Clear();
            Save();
        }

        private int GetNextId()
        {
            if (tasks.Count == 0) return 1;
            return tasks.Keys.Max() + 1;
        }
    }
}