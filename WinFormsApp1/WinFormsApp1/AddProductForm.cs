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
    public partial class AddProductForm : Form
    {
        public NpgsqlConnection con;

        public AddProductForm(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Product (name, ed) VALUES (:name, :ed)", con);
                command.Parameters.AddWithValue("name", textBoxName.Text);
                command.Parameters.AddWithValue("ed", textBoxEd.Text);
                command.ExecuteNonQuery();
                Close();
            }
            catch (Exception ee) { MessageBox.Show("ААААААААААААА"); }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
