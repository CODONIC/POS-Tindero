using Npgsql;
using POS.Cashier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace POS
{
    public partial class CashierDashboard : BaseForm
    {
        private string _username;
        private string _companyName;
        private string _companyId;

        public CashierDashboard(string username, string companyName)
        {
            InitializeComponent();
            InitializeTitleBar(closeButton, titleBar, titleLabel);
            _username = username;
            _companyName = companyName;
            lblCashierName.Text = $"{_username} | Cashier";
            titleLabel.Text = $"{_companyName} ";
            _companyId = GetCompanyId(_companyName);
        }


        // ─── Resolve company name to ID ───────────────────────────────────────────

        private string GetCompanyId(string companyName)
        {
            try
            {
                using (var conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id FROM public.companies WHERE LOWER(name) = LOWER(@name) LIMIT 1";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", companyName);
                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resolving company:\n{ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // ─── Load Products ────────────────────────────────────────────────────────

        private async void CashierDashboard_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            if (string.IsNullOrEmpty(_companyId)) return;

            try
            {
                await using var conn = DatabaseService.GetConnection();
                await conn.OpenAsync();

                string sql = @"
                SELECT p.product_code, p.product_name, p.price, p.quantity
                FROM products p
                WHERE p.company_id = @companyId
                ORDER BY p.product_name";

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("companyId", NpgsqlTypes.NpgsqlDbType.Uuid, Guid.Parse(_companyId));

                await using var reader = await cmd.ExecuteReaderAsync();

                var table = new DataTable();
                table.Load(reader);

                dgvProducts.Invoke((Action)(() =>
                {
                    dgvProducts.DataSource = table;
                    dgvProducts.Columns["product_code"].HeaderText = "Product Code";
                    dgvProducts.Columns["product_name"].HeaderText = "Product Name";
                    dgvProducts.Columns["price"].HeaderText = "Price";
                    dgvProducts.Columns["quantity"].HeaderText = "Quantity";
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Buttons ──────────────────────────────────────────────────────────────

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                LogInForm login = new LogInForm();
                login.Show();
                this.Close();
            }
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {
            DiscountFrm discount = new DiscountFrm(_username, _companyName);
            discount.Show();
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            PaymentFrm payment = new PaymentFrm(_username, _companyName);
            payment.Show();
        }
    }
}
