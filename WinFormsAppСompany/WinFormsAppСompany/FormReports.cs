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

namespace WinFormsAppСompany
{
    public partial class FormReports : Form
    {
        private DataGridView dataGridView;
        private NpgsqlConnection connection;
        private MenuStrip menuStrip;

        public FormReports(NpgsqlConnection conn)
        {
            connection = conn;
            InitializeComponents();
            LoadReports();
        }

        private void InitializeComponents()
        {
            this.Text = "Отчеты по подотчетным средствам";
            this.Size = new Size(800, 600);

            menuStrip = new MenuStrip();

            var fileMenuItem = new ToolStripMenuItem("Файл");
            var editMenuItem = new ToolStripMenuItem("Действия");

            var addItem = new ToolStripMenuItem("Добавить", null, AddItem_Click);
            var editItem = new ToolStripMenuItem("Изменить", null, EditItem_Click);
            var deleteItem = new ToolStripMenuItem("Удалить", null, DeleteItem_Click);
            var exitItem = new ToolStripMenuItem("Выход", null, (s, e) => this.Close());

            editMenuItem.DropDownItems.AddRange(new[] { addItem, editItem, deleteItem });
            fileMenuItem.DropDownItems.Add(exitItem);

            menuStrip.Items.AddRange(new[] { fileMenuItem, editMenuItem });

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(dataGridView);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;

        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView.Rows[e.RowIndex];
                int reportId = Convert.ToInt32(row.Cells["id"].Value);

                var expensesForm = new ExpensesForm(connection, reportId);
                expensesForm.ShowDialog(); 
            }
        }


        private void LoadReports()
        {
            var cmd = new NpgsqlCommand(@"
            SELECT r.id, e.name, a.date AS advance_date, a.sum AS advance_sum, 
                   r.date AS report_date, r.sum AS report_sum
            FROM reports r
            JOIN advances a ON r.advance_id = a.id
            JOIN employees e ON a.employee_id = e.employee_id", connection);

            var adapter = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
        }

        private void AddItem_Click(object sender, EventArgs e)
        {
            var form = new AddReportForm(connection);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadReports();
            }
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчёт для редактирования!");
                return;
            }

            var row = dataGridView.SelectedRows[0];
            int reportId = (int)row.Cells["id"].Value;
            DateTime reportDate = Convert.ToDateTime(row.Cells["report_date"].Value);
            decimal reportSum = Convert.ToDecimal(row.Cells["report_sum"].Value);

            var form = new EditReportForm(connection, reportId, reportDate, reportSum);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadReports();
            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчёт для удаления!");
                return;
            }

            var row = dataGridView.SelectedRows[0];
            int id = (int)row.Cells["id"].Value;

            if (MessageBox.Show("Удалить выбранный отчёт?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var cmd = new NpgsqlCommand("DELETE FROM reports WHERE id = @id", connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                LoadReports();
                MessageBox.Show("Отчёт удален!");
            }
        }
    }
}
