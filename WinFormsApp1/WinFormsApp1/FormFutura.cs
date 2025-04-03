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

    public partial class FormFutura : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        DataTable dt2 = new DataTable();
        DataSet ds2 = new DataSet();
        public FormFutura(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
            Update();
            Update2();
        }

        private void Update()
        {
            string sql = "SELECT futura.ID, futura.data, client.name FROM futura JOIN client ON futura.IDclient = client.ID GROUP BY futura.ID, futura.data, client.name";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
        }

        private void Update2()
        {
            string sql = "SELECT futura.ID, futurainfo.ID, futurainfo.idproduct, futurainfo.quantity, futurainfo.quantity, futurainfo.price, futura.data FROM futurainfo JOIN futura ON futurainfo.IDfutura = futura.ID WHERE futura.ID = :id GROUP BY futurainfo.ID, futura.data, futura.ID";
            int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;
            if (id != 0) {
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
                ds2.Reset();
                da.Fill(ds2);
                dt2 = ds2.Tables[0];
                dataGridView2.DataSource = dt2;
            }
          
        }
    }
}
