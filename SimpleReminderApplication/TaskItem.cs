using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReminderApplication
{
    internal class TaskItem
    {
        public Guid Id { get; set; }              // Unique ID
        public string Description { get; set; }   // Task description
        public DateTime DueDate { get; set; }     // Due date/time
        public TaskStatus Status { get; set; }    // ToDo, InProgress, Completed

        public TaskItem(string description, DateTime dueDate)
        {
            Id = Guid.NewGuid();
            Description = description;
            DueDate = dueDate;
            Status = TaskStatus.ToDo;
        }

        public override string ToString()
        {
            return $"{Description} (Due {DueDate:yyyy-MM-dd}) - {Status}";
        }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Completed
    }
}
