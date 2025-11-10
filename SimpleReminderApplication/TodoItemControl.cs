using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleReminderApplication
{
    public partial class TodoItemControl : UserControl
    {
        // The TaskItem is now the single source of truth
        private TaskItem _task;

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
        internal TodoItemControl(TaskItem task) : this()
        {
            this.Task = task;
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
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
            }

            // Update styling based on status (e.g., strikeout for completed)
            if (_task.Status == TaskStatus.Completed)
            {
                lblDescription.Font = new Font(lblDescription.Font, FontStyle.Strikeout);
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
            // This 'using' block calls the exact form you just posted
            using (AddTaskForm editForm = new AddTaskForm(Description, DueDate))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    Description = editForm.TaskDescription;
                    DueDate = editForm.TaskDueDate;
                }
            }
        }

        private void DeleteTask(object sender, EventArgs e)
        {
            // This will remove the UI control.
            // Note: You'll still need to delete the TaskItem from your main list/database.
            this.Parent.Controls.Remove(this);
            this.Dispose(); // Clean up resources
        }

        // New handlers for status changes
        private void MoveToToDo(object sender, EventArgs e)
        {
            this.Status = TaskStatus.ToDo;
        }

        private void MoveToInProgress(object sender, EventArgs e)
        {
            this.Status = TaskStatus.InProgress;
        }

        private void MoveToCompleted(object sender, EventArgs e)
        {
            this.Status = TaskStatus.Completed;
        }

        #endregion
    }
}