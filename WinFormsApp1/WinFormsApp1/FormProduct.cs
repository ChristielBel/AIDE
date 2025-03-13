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
    }
}
