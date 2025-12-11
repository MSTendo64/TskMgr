using System;
using System.Collections.Generic;
using System.Text;

namespace TskMgr
{
    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeadLine { get; set; }

        public Task(string Name, string Description, TaskPriority Priority,
            TaskStatus Status, DateTime CreateDate, DateTime? DeadLine = null)
        {
            this.Name = Name;
            this.Description = Description;
            this.Priority = Priority;
            this.Status = Status;
            this.CreateDate = CreateDate;
            this.DeadLine = DeadLine;
        }
    }
}
