using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    public partial class AddFuturaForm : Form
    {
        public NpgsqlConnection con;
        int id;

        public AddFuturaForm(NpgsqlConnection con, int id = -1)
        {
            InitializeComponent();
            this.con = con;
            this.id = id;
            LoadClients();
        }

        public AddFuturaForm(NpgsqlConnection con, int id, DateTime date, int clientId)
        {
            InitializeComponent();
            this.con = con;
            this.id = id;
            LoadClients();
            dateTimePicker1.Value = date;
            comboBox1.SelectedValue = clientId;
        }

        private void LoadClients()
        {
            string sql = "SELECT id, name FROM client";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";
        }

        public int GetSelectedClientId()
        {
            return (int)comboBox1.SelectedValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int clientId = Convert.ToInt32(comboBox1.SelectedValue);
                DateTime date = dateTimePicker1.Value;

                if (id == -1) 
                {
                    string sql = "INSERT INTO futura (idClient, data) VALUES (@idClient, @data)";
                    using (var cmd = new NpgsqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@idClient", clientId);
                        cmd.Parameters.AddWithValue("@data", date);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Новая накладная добавлена.");
                }
                else
                {
                    string sql = "UPDATE futura SET idClient = @idClient, data = @data WHERE id = @id";
                    using (var cmd = new NpgsqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@idClient", clientId);
                        cmd.Parameters.AddWithValue("@data", date);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Накладная обновлена.");
                }

                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
