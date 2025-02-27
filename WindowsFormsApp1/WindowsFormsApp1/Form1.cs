using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<User> users = new List<User>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string city = comboBox1.SelectedItem.ToString();
            string gender = "";
            if (radioButton1.Checked) gender = radioButton1.Text;
            else if (radioButton2.Checked) gender = radioButton2.Text;
            string sport = "";
            for(int i = 0;i < checkedListBox1.CheckedItems.Count; i++)
            {
                sport += checkedListBox1.CheckedItems[i].ToString() + " ";
            }

            User user = new User(name, city, gender, sport);

            users.Add(user);

            richTextBox1.Clear();

            for(int i = 0; i < users.Count; i++)
            {
                richTextBox1.AppendText("User " + (i + 1) + Environment.NewLine + users[i].toString() + Environment.NewLine);
            }

            MessageBox.Show("You have successfully added a user!");
        }

        private void buttonExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            String filename = ofd.FileName;

            Microsoft.Office.Interop.Excel.Application excelObj = new Microsoft.Office.Interop.Excel.Application();
            excelObj.Visible = true;

            Workbook wb = excelObj.Workbooks.Open(filename, 0, false, 5, "","",false, XlPlatform.xlWindows, "",true,false,0,true,false,false);

            Worksheet wsh = wb.Sheets[1];

            wsh.Cells[1, 1] = "Name";
            wsh.Cells[1, 2] = "City";
            wsh.Cells[1, 3] = "Gender";
            wsh.Cells[1, 4] = "Sport";

            for(int i = 0; i < users.Count; i++)
            {
                wsh.Cells[i + 2, 1] = users[i].Name;
                wsh.Cells[i + 2, 2] = users[i].City;
                wsh.Cells[i + 2, 3] = users[i].Gender;
                wsh.Cells[i + 2, 4] = users[i].Sport;
            }

            wb.Save();

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
