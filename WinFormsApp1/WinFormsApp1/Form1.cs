using Npgsql;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public NpgsqlConnection con;
        public Form1()
        {
            InitializeComponent();
            MyLoad();
        }

        public void MyLoad()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            con = new NpgsqlConnection("Server = localhost; Port = 5432; UserID = postgres; Password= postpass; Database = belikova");
            con.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormProduct fp = new FormProduct(con);
            fp.ShowDialog();
        }
    }
}