using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleReminderApplication
{
    public partial class TodoItemControl : UserControl
    {
        private TaskItem _task;
        private TaskRepository _taskRepository;
        private Action _refreshListCallback;

        internal TaskItem Task
        {
            get { return _task; }
            set
            {
                _task = value;
                UpdateUI();
            }
        }

        public TodoItemControl()
        {
            InitializeComponent();
        }

        internal TodoItemControl(TaskItem task, TaskRepository repository, Action refreshList) : this()
        {
            this.Task = task;
            this._taskRepository = repository;
            this._refreshListCallback = refreshList;
        }

        private void UpdateUI()
        {
            if (_task == null) return;

            lblDescription.Text = _task.Description;
            lblDueDate.Text = $"Due: {_task.DueDate:yyyy-MM-dd}";

            if (_task.DueDate.Date <= DateTime.Today)
            {
                this.BackColor = Color.FromArgb(247, 217, 213);
                this.ForeColor = Color.FromArgb(109, 53, 51);
                this.Parent?.Controls.SetChildIndex(this, 0);
            }
            else
            {
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
            }

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

        public string Description
        {
            get { return _task?.Description; }
            set
            {
                if (_task != null)
                {
                    _task.Description = value;
                    UpdateUI();
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
                    UpdateUI();
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
                    UpdateUI();
                }
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Move to To Do", null, MoveToToDo);
            menu.Items.Add("Move to In Progress", null, MoveToInProgress);
            menu.Items.Add("Move to Completed", null, MoveToCompleted);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Edit Task", null, EditTask);
            menu.Items.Add("Delete Task", null, DeleteTask);

            menu.Show(btnMenu, new Point(0, btnMenu.Height));
        }

        private void EditTask(object sender, EventArgs e)
        {
            using (TaskEditorForm editForm = new TaskEditorForm(Description, DueDate))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _task.Description = editForm.TaskDescription;
                    _task.DueDate = editForm.TaskDueDate;

                    try
                    {
                        _taskRepository.UpdateTask(this.Task);
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
            try
            {
                _taskRepository.DeleteTask(this.Task.Id);
                _refreshListCallback?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete task: {ex.Message}");
            }
        }


        private void MoveToToDo(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.ToDo;
            try
            {
                _taskRepository.UpdateTask(_task);
                _refreshListCallback?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }

        private void MoveToInProgress(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.InProgress;
            try
            {
                _taskRepository.UpdateTask(_task);
                _refreshListCallback?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }

        private void MoveToCompleted(object sender, EventArgs e)
        {
            _task.Status = TaskStatus.Completed;
            try
            {
                _taskRepository.UpdateTask(_task);
                _refreshListCallback?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to move task: {ex.Message}");
            }
        }
    }
}