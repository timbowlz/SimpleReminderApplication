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
    // 1. Open your existing AddTaskForm
    // We pass default values for a new task.
    using (AddTaskForm form = new AddTaskForm("", DateTime.Now))
    {
        // 2. Check if the user clicked "Done"
        if (form.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // 3. Create a new TaskItem object
                TaskItem newTask = new TaskItem(form.TaskDescription, form.TaskDueDate);
                
                // 4. Save the new task to the database
                _taskRepository.AddTask(newTask);

                // 5. Add the task to the correct UI panel
                TodoItemControl taskControl = new TodoItemControl(newTask, _taskRepository, LoadAllTasks);
                pnlToDo.Controls.Add(taskControl);
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
            // 1. Ask the user for confirmation
            var confirmResult = MessageBox.Show("Are you sure you want to clear all completed tasks?",
                                             "Confirm Clear",
                                             MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // 2. Call the repository to delete the tasks from the database
                    _taskRepository.DeleteAllCompletedTasks();

                    // 3. Refresh the UI by calling your existing LoadAllTasks method
                    LoadAllTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to clear completed tasks: {ex.Message}");
                }
            }
        }

        internal void LoadAllTasks()
        {
            try
            {
                // 1. Get all tasks from the repository
                List<TaskItem> tasks = _taskRepository.GetAllTasks();

                // 2. Clear any existing controls (in case you add a refresh button later)
                pnlToDo.Controls.Clear();
                pnlInProgress.Controls.Clear();
                pnlCompleted.Controls.Clear();

                // 3. Add each task to the correct panel
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
            // Create the UI control, passing in the task and the repository
            TodoItemControl taskControl = new TodoItemControl(task, _taskRepository, LoadAllTasks);

            // Add the control to the correct panel based on its status
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
