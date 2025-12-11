namespace TskMgr
{
    public enum TaskPriority
    {
        Low,
        Normal,
        High,
    }

    public enum TaskStatus
    {
        NotStart,
        Proccessing,
        Completed
    }
   
    public class TaskManager
    {
        private ITaskStorage storage;
        private StorageType currentStorageType;

        public ITaskStorage taskStorage => storage;

        public TaskManager(StorageType storageType = StorageType.Sqlite, string storagePath = "storage") 
        {
            currentStorageType = storageType;
            storage = StorageFactory.CreateStorage(storageType, storagePath);
        }

        public void SwitchStorage(StorageType newStorageType, string newPath = null)
        {
            if (newStorageType == currentStorageType) return;

            try
            {
                // Сохраняем текущие задачи
                var currentTasks = new Dictionary<int, Task>(storage.tasks);

                // Создаем новое хранилище
                var newStorage = StorageFactory.CreateStorage(newStorageType, newPath);

                // Переносим задачи в новое хранилище
                foreach (var task in currentTasks)
                {
                    newStorage.AddTask(task.Value);
                }

                // Обновляем ссылки
                storage = newStorage;
                currentStorageType = newStorageType;

                Console.WriteLine($"Хранилище изменено на: {newStorageType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при смене хранилища: {ex.Message}");
                throw;
            }
        }

        public void AddTask(Task task)
        {
            taskStorage.AddTask(task);
            taskStorage.Save();
        }

        public void AddTask(string name, string description, TaskPriority priority, TaskStatus status, DateTime? deadline)
        {
            Task task = new Task(name, description, priority, status, DateTime.Now, deadline);
            taskStorage.AddTask(task);
            taskStorage.Save();
        }

        public void RemoveTask(string name) 
        {
            taskStorage.tasks.Remove(GetTaskIdByMame(name));
            taskStorage.Save();
        }
        public void RemoveTask(int id)
        {
            taskStorage.DeleteTask(id);
            taskStorage.Save();
        }

        public Task GetTask(string name = null, int id = -1) 
        {
            if (name != null || id >= 0)
            {
                if(taskStorage.tasks.TryGetValue(id, out var task))
                {
                    return task; 
                }

                return taskStorage.tasks[GetTaskIdByMame(name)];
            }
            else
            {
                return null;
            }
        }

        public int GetTaskIdByMame(string name)
        {
            foreach (var task in taskStorage.tasks) 
            {
                if(task.Value.Name == name)
                {
                    return task.Key; 
                }
            }
            return -1;
        }
    }
}
