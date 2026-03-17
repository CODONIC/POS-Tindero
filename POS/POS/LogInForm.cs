using Supabase;
using System;
namespace POS
{
    public partial class LogIn_Form : Form
    {
        private Supabase.Client supabase;
        public LogIn_Form()
        {
            InitializeComponent();
            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            try
            {
                var url = "https://tvmxjtgypuimgbshtpbf.supabase.co"; 
                var key = "sb_publishable_Fv1POfF2Fy0X4uAN9ec1mw_ZXlpLm6K"; 

                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true,
                    AutoRefreshToken = true
                };

                supabase = new Supabase.Client(url, key, options);
                await supabase.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize Supabase: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Point mouseOffset;
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOffset = new Point(-e.X, -e.Y);
                titleBar.MouseMove += TitleBar_MouseMove;
                titleBar.MouseUp += TitleBar_MouseUp;
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            titleBar.MouseMove -= TitleBar_MouseMove;
            titleBar.MouseUp -= TitleBar_MouseUp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Activate();
            this.Focus();
        }

        private async void btnSignInClick(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                var userCheck = await supabase
                    .From<Users>()
                    .Select("*")
                    .Where(u => u.Username == username)
                    .Get();

                if (userCheck.Models.Count == 0)
                {
                    if ((username == "Username" || string.IsNullOrEmpty(username)) &&
                        (password == "Password" || string.IsNullOrEmpty(password)))
                    {
                        MessageBox.Show("Please enter username and password.", "Login Failed",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (username == "Username" || string.IsNullOrEmpty(username))
                    {
                        MessageBox.Show("Please enter a username.", "Login Failed",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (password == "Password" || string.IsNullOrEmpty(password))
                    {
                        MessageBox.Show("Please enter a password.", "Login Failed",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MessageBox.Show("Invalid username & password.", "Login Failed",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Text = "Username";
                    txtUsername.InnerForeColor = Color.Gray;
                    txtPassword.IsPasswordField = false;
                    txtPassword.Text = "Password";
                    txtPassword.InnerForeColor = Color.Gray;
                    return;
                }

                // Username exists, now check password
                var result = await supabase
                    .From<Users>()
                    .Select("*")
                    .Where(u => u.Username == username && u.Password == password)
                    .Get();

                if (result.Models.Count > 0)
                {
                    // Successful login
                    var user = result.Models.First();
                    MessageBox.Show("Login Successful!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // open main POS form
                    // MainForm main = new MainForm();
                    // main.Show();
                    // this.Hide();
                }
                else
                {
                    if (password == "Password" || string.IsNullOrEmpty(password))
                    {
                        MessageBox.Show("Please enter a password.", "Login Failed",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    MessageBox.Show("Invalid password.", "Login Failed",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtPassword.IsPasswordField = false;
                    txtPassword.Text = "Password";
                    txtPassword.InnerForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
                txtUsername.InnerForeColor = Color.Black;
            }
        }

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                txtUsername.Text = "Username";
                txtUsername.InnerForeColor = Color.Gray;
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = "";
                txtPassword.InnerForeColor = Color.Black;
                txtPassword.IsPasswordField = true; 
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if(txtPassword.Text == "")
            {
                txtPassword.Text = "Password";
                txtPassword.InnerForeColor = Color.Gray;
                txtPassword.IsPasswordField = false;
            }
        }
    }
}