using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    public partial class CashierDashboard : BaseForm
    {
        public CashierDashboard()
        {
            InitializeComponent();
            InitializeTitleBar(closeButton, titleBar, titleLabel);
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                LogInForm login = new LogInForm();
                login.Show();
                this.Close();
            }
        }
    }
}
