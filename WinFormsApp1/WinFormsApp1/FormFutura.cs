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
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            Update();
            Update2();
        }

        private void Update()
        {
            string sql = @"SELECT futura.ID, futura.data, futura.idClient, client.name 
               FROM futura 
               JOIN client ON futura.idClient = client.id";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
        }

        private void Update2()
        {
            if (dataGridView1.CurrentRow == null)
                return;

            if (!int.TryParse(dataGridView1.CurrentRow.Cells["id"].Value.ToString(), out int id))
                return;

            string sql = @"SELECT futurainfo.ID, futurainfo.idproduct, futurainfo.quantity, futurainfo.price, futura.data 
                   FROM futurainfo 
                   JOIN futura ON futurainfo.IDfutura = futura.ID 
                   WHERE futura.ID = @id";

            using (var command = new NpgsqlCommand(sql, con))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var adapter = new NpgsqlDataAdapter(command))
                {
                    ds2.Reset();
                    adapter.Fill(ds2);
                    dt2 = ds2.Tables[0];
                    dataGridView2.DataSource = dt2;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            Update2();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFuturaForm f = new AddFuturaForm(con);
            f.ShowDialog();
            Update();
            Update2();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            DateTime date = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["data"].Value);
            int clientId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["idClient"].Value); 
            AddFuturaForm f = new AddFuturaForm(con, id, date, clientId);
            f.ShowDialog();
            Update();
            Update2();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите накладную для удаления.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Вы уверены, что хотите удалить выбранную накладную?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    int futuraId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

                    string deleteFuturaInfoSql = "DELETE FROM futurainfo WHERE idFutura = @id";
                    using (var cmd1 = new NpgsqlCommand(deleteFuturaInfoSql, con))
                    {
                        cmd1.Parameters.AddWithValue("@id", futuraId);
                        cmd1.ExecuteNonQuery();
                    }
                   
                    string deleteFuturaSql = "DELETE FROM futura WHERE id = @id";
                    using (var cmd2 = new NpgsqlCommand(deleteFuturaSql, con))
                    {
                        cmd2.Parameters.AddWithValue("@id", futuraId);
                        cmd2.ExecuteNonQuery();
                    }

                    MessageBox.Show("Накладная успешно удалена.");
                    Update();  
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
