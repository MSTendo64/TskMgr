using System.Collections.Generic;

namespace TskMgr
{
    public interface ITaskStorage
    {
        Dictionary<int, Task> tasks { get; }
        void Save();
        void Load();
        void AddTask(Task task);
        void UpdateTask(int id, Task task);
        void DeleteTask(int id);
        void Clear();
    }
}