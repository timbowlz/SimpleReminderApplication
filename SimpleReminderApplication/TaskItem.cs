using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReminderApplication
{
    internal class TaskItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskStatus Status { get; set; }

        public TaskItem(string description, DateTime dueDate)
        {
            Id = Guid.NewGuid();
            Description = description;
            DueDate = dueDate;
            Status = TaskStatus.ToDo;
        }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed
    }
}
