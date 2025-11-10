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

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        private void btnClearCompleted_Click(object sender, EventArgs e)
        {

        }
    }
    
}
