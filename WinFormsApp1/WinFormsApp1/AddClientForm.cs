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
        int id;
        public AddClientForm(NpgsqlConnection con, int id = -1)
        { 
            InitializeComponent();
            this.con = con;
            this.id = id;
        }

        public AddClientForm(NpgsqlConnection con, int id, string name, string address, string phone)
        {
            InitializeComponent();
            textBoxName.Text = name;
            textBoxAddress.Text = address;
            textBoxPhone.Text = phone;
            this.con = con;
            this.id = id;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (id == -1)
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
            else
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("UPDATE Client SET name = :name, address = :address, phone = :phone WHERE ID = :id", con);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("address", textBoxAddress.Text);
                    command.Parameters.AddWithValue("phone", textBoxPhone.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch { }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
