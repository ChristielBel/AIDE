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

namespace WinFormsApp1
{
    public partial class FormClient : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        public FormClient(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
            Update();
        }

        public void Update()
        {
            String sql = "Select * from Client";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Номер";
            dataGridView1.Columns[1].HeaderText = "Имя";
            dataGridView1.Columns[2].HeaderText = "Адрес";
            dataGridView1.Columns[3].HeaderText = "Номер телефона";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        // добавить
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClientForm f = new AddClientForm(con);
            f.ShowDialog();
            Update();
        }

        // удалить
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Client WHERE ID = :id", con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                Update();
            }
            catch (Exception ex) { }
        }

        // изменить
        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
            string name = (string)dataGridView1.CurrentRow.Cells["name"].Value;
            string address = (string)dataGridView1.CurrentRow.Cells["address"].Value;
            string phone = (string)dataGridView1.CurrentRow.Cells["phone"].Value;
            AddClientForm f = new AddClientForm(con, id, name, address, phone);
            f.ShowDialog();
            Update();
        }
    }
}
