using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleReminderApplication
{
    public partial class MainWindow : Form
    {
        private TaskRepository _taskRepository;
        public MainWindow()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
            _taskRepository = new TaskRepository();
            LoadAllTasks();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

private void btnAdd_Click(object sender, EventArgs e)
{
    using (TaskEditorForm form = new TaskEditorForm("", DateTime.Now))
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            try
            {
                TaskItem newTask = new TaskItem(form.TaskDescription, form.TaskDueDate);
                _taskRepository.AddTask(newTask);
                LoadAllTasks();
                    }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add task: {ex.Message}");
            }
        }
    }
}

        private void btnClearCompleted_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to clear all completed tasks?", "Confirm Clear", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    _taskRepository.DeleteAllCompletedTasks();
                    LoadAllTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to clear completed tasks: {ex.Message}");
                }
            }
        }

        private void LoadAllTasks()
        {
            try
            {
                List<TaskItem> tasks = _taskRepository.GetAllTasks();
                pnlToDo.Controls.Clear();
                pnlInProgress.Controls.Clear();
                pnlCompleted.Controls.Clear();

                var sortedTasks = tasks.OrderBy(t => t.DueDate).ToList();
                foreach (var task in sortedTasks)
                {
                    AddTaskToPanel(task);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load tasks: {ex.Message}");
            }
        }

        private void AddTaskToPanel(TaskItem task)
        {
            TodoItemControl taskControl = new TodoItemControl(task, _taskRepository, LoadAllTasks);

            switch (task.Status)
            {
                case TaskStatus.ToDo:
                    pnlToDo.Controls.Add(taskControl);
                    break;
                case TaskStatus.InProgress:
                    pnlInProgress.Controls.Add(taskControl);
                    break;
                case TaskStatus.Completed:
                    pnlCompleted.Controls.Add(taskControl);
                    break;
            }
        }
    }
    
}
