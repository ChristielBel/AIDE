using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string city = comboBox1.ToString();
            string gender = "";
            if (radioButton1.Checked) gender = radioButton1.Text;
            else if (radioButton2.Checked) gender = radioButton2.Text;
            string sport = "";
            for(int i = 0;i < checkedListBox1.CheckedItems.Count; i++)
            {
                sport += checkedListBox1.CheckedItems[i].ToString() + " ";
            }

            User user = new User(name, city, gender, sport);

            richTextBox1.Text = user.toString();
        }
    }

    class User
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Sport { get; set; }

        public User(string name, string city, string gender, string sport)
        {
            Name = name;
            City = city;
            Gender = gender;
            Sport = sport;
        }

        public string toString()
        {
            return "Name: " + Name + "\nCity: " + City + "\nGender: " + Gender + "\nSport: " + Sport;
        }
    }
}
