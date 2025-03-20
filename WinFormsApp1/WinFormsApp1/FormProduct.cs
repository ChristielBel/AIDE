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
    public partial class FormProduct : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        public FormProduct(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
            Update();
        }

        public void Update()
        {
            String sql = "Select * from Product";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Номер";
            dataGridView1.Columns[1].HeaderText = "Наименование";
            dataGridView1.Columns[2].HeaderText = "Ед.измерения";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        //добавить
        private void ljfbmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddProductForm f = new AddProductForm(con);
            f.ShowDialog();
            Update();
        }

        //удалить
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Product WHERE ID = :id", con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                Update();
            }
            catch (Exception ex) { }
        }
    }
}
