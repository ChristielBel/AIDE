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
    public partial class AddClientForm : Form
    {
        public NpgsqlConnection con;
        public AddClientForm(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Client (name, address, phone) VALUES (:name, :address, :phone)", con);
                command.Parameters.AddWithValue("name", textBoxName.Text);
                command.Parameters.AddWithValue("address", textBoxAddress.Text);
                command.Parameters.AddWithValue("phone", textBoxPhone.Text);
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
