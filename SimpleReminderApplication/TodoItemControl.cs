using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleReminderApplication
{
    public partial class TodoItemControl : UserControl
    {
        // The TaskItem is now the single source of truth
        private TaskItem _task;
        private TaskRepository _taskRepository;
        private Action _refreshListCallback;

        /// <summary>
        /// Gets or sets the data object for this control.
        /// Setting this property will automatically update the UI.
        /// </summary>
        internal TaskItem Task
        {
            get { return _task; }
            set
            {
                _task = value;
                UpdateUI(); // Update all UI elements when the task is set
            }
        }

        // Default constructor for the designer
        public TodoItemControl()
        {
            InitializeComponent();
        }

        // New constructor to initialize the control with a TaskItem
        internal TodoItemControl(TaskItem task, TaskRepository repository, Action refreshList) : this()
        {
            this.Task = task;
            this._taskRepository = repository;
            this._refreshListCallback = refreshList;
        }

        /// <summary>
        /// Updates all UI elements based on the current state of the _task object.
        /// </summary>
        private void UpdateUI()
        {
            if (_task == null) return;

            // Update text
            lblDescription.Text = _task.Description;
            lblDueDate.Text = $"Due: {_task.DueDate:yyyy-MM-dd}";

            // Update styling based on due date
            if (_task.DueDate.Date == DateTime.Today)
            {
                this.BackColor = Color.FromArgb(247, 217, 213);
                this.ForeColor = Color.FromArgb(109, 53, 51);
                // Bring "due today" tasks to the top if the parent is a FlowLayoutPanel
                this.Parent?.Controls.SetChildIndex(this, 0);
            }
            else
            {
                // IMPORTANT: Always reset to default if the condition isn't met
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
            }

            // Update styling based on status (e.g., strikeout for completed)
            if (_task.Status == TaskStatus.Completed)
            {
                lblDescription.Font = new Font(lblDescription.Font, FontStyle.Strikeout);
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
            }
            else
            {
                lblDescription.Font = new Font(lblDescription.Font, FontStyle.Regular);
            }
        }

        #region Proxy Properties

        // These properties now "proxy" the underlying _task object.
        // This ensures the AddTaskForm still works as expected.

        public string Description
        {
            get { return _task?.Description; }
            set
            {
                if (_task != null)
                {
                    _task.Description = value;
                    UpdateUI(); // Refresh UI
                }
            }
        }

        public DateTime DueDate
        {
            get { return _task?.DueDate ?? DateTime.MinValue; }
            set
            {
                if (_task != null)
                {
                    _task.DueDate = value;
                    UpdateUI(); // Refresh UI
                }
            }
        }

        public TaskStatus Status
        {
            get { return _task?.Status ?? TaskStatus.ToDo; }
            set
            {
                if (_task != null)
                {
                    _task.Status = value;
                    UpdateUI(); // Refresh UI
                }
            }
        }

        #endregion

        private void btnMenu_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            // Enabled the status-changing menu items
            menu.Items.Add("Move to To Do", null, MoveToToDo);
            menu.Items.Add("Move to In Progress", null, MoveToInProgress);
            menu.Items.Add("Move to Completed", null, MoveToCompleted);
            menu.Items.Add(new ToolStripSeparator()); // Added for clarity
            menu.Items.Add("Edit Task", null, EditTask);
            menu.Items.Add("Delete Task", null, DeleteTask);

            menu.Show(btnMenu, new Point(0, btnMenu.Height));
        }

        #region Menu Item Handlers

        // From your TodoItemControl.cs
        private void EditTask(object sender, EventArgs e)
        {
            using (AddTaskForm editForm = new AddTaskForm(Description, DueDate))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // 1. Update the local task object's properties
                    _task.Description = editForm.TaskDescription;
                    _task.DueDate = editForm.TaskDueDate;

                    try
                    {
                        // 2. Save the updated task to the database
                        _taskRepository.UpdateTask(this.Task);

                        // 3. CALL THE MAIN REFRESH CALLBACK
                        // This tells MainWindow to call LoadAllTasks()
                        _refreshListCallback?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to update task: {ex.Message}");
                    }
                }
            }
        }

        private void DeleteTask(object sender, EventArgs e)
        {
            _taskRepository.DeleteTask(this.Task.Id);
            // 3. Remove the UI control from the panel
            this.Parent.Controls.Remove(this);
            this.Dispose(); // Clean up resources
        }


        // ... (EditTask and DeleteTask) ...

        // New handlers for status changes
        private void MoveToToDo(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.ToDo; // Update the task object
            try
            {
                _taskRepository.UpdateTask(_task); // Save change to database
                _refreshListCallback?.Invoke();    // Refresh the main UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }

        private void MoveToInProgress(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.InProgress; // Update the task object
            try
            {
                _taskRepository.UpdateTask(_task); // Save change to database
                _refreshListCallback?.Invoke();    // Refresh the main UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }

        private void MoveToCompleted(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.Completed; // Update the task object
            try
            {
                _taskRepository.UpdateTask(_task); // Save change to database
                _refreshListCallback?.Invoke();    // Refresh the main UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }

        #endregion
    }
}