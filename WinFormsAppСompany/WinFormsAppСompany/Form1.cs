using System.Drawing;
using Npgsql;

namespace WinFormsAppСompany
{
    public partial class Form1 : Form
    {
        public NpgsqlConnection connection;
        public Form1()
        {
            InitializeComponent();
            Connection();
        }

        public void Connection()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            connection = new NpgsqlConnection("Server = localhost; Port = 5432; UserID = postgres; Password = postpass; Database = belikova");
            connection.Open();
        }

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void balancesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void advancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormAdvances(connection);
            form.ShowDialog();
        }

        private void списокСотрудниковToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var form = new FormEmployees(connection);
            form.ShowDialog();
        }

        private void просмотрОтчетовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormReports(connection);
            form.ShowDialog();
        }

        private void графикОстатковToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new BalancesChartForm(connection);
            form.ShowDialog();
        }

        private void отчетПоОстаткамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ReportsExpensesForm(connection);
            form.ShowDialog();
        }
    }
}
