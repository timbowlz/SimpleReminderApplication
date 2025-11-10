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
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            string description = txtTaskDescription.Text;
            DateTime dueDate = dateTimePicker1.Value;
            TaskItem newTask = new TaskItem(description, dueDate);

            // 3. Save the object to the database
            try
            {
                _taskRepository.AddTask(newTask);

                // 4. If saving was successful, create the UI control (View)
                TodoItemControl newControl = new TodoItemControl(newTask);

                // 5. Add the UI control to the screen
                pnlToDo.Controls.Add(newControl);

                // Clear input fields
                txtTaskDescription.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add task: {ex.Message}");
            }
        }
    }
    
}
