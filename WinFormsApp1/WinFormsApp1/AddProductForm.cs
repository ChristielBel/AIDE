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
        int id;

        public AddProductForm(NpgsqlConnection con, int id = -1)
        {
            InitializeComponent();
            this.con = con;
            this.id = id;
        }

        public AddProductForm(NpgsqlConnection con, int id, string nameP, string ed)
        {
            InitializeComponent();
            textBoxName.Text = nameP;
            textBoxEd.Text = ed;
            this.con = con;
            this.id = id;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (id == -1)
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Product (name, ed) VALUES (:name, :ed)", con);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("ed", textBoxEd.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch { }
            }
            else
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("UPDATE Product SET name = :name, ed = :ed WHERE ID = :id", con);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("ed", textBoxEd.Text);
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
