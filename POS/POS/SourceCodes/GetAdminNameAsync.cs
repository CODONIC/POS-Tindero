using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.SourceCodes
{
    public static class GetAdminNameAsync
    {
        public static async Task<string> GetAdminName(string username)
        {
            try
            {
                await using var conn = DatabaseService.GetConnection();
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(
                    "SELECT name FROM admins WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("username", username);

                var result = await cmd.ExecuteScalarAsync();
                return result?.ToString() ?? username;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching admin name: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return username;
            }
        }
    }
}