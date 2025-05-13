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
    public partial class ExpensesForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private readonly int _reportId;
        private DataGridView _expensesGrid;

        public ExpensesForm(NpgsqlConnection conn, int reportId)
        {
            _connection = conn ?? throw new ArgumentNullException(nameof(conn));
            _reportId = reportId;

            InitializeComponents();
            LoadExpenses();
        }

        private void InitializeComponents()
        {
            this.Text = $"Детализация расходов (Отчет #{_reportId})";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            _expensesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _expensesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "id",
                HeaderText = "ID",
                Visible = false
            });

            _expensesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                DataPropertyName = "category",
                HeaderText = "Категория",
                Width = 200
            });

            _expensesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                DataPropertyName = "quantity",
                HeaderText = "Количество"
            });

            _expensesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sum",
                DataPropertyName = "sum",
                HeaderText = "Сумма"
            });

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            var addButton = new Button { Text = "Добавить", Dock = DockStyle.Left, Width = 100 };
            var editButton = new Button { Text = "Редактировать", Dock = DockStyle.Left, Width = 100 };
            var deleteButton = new Button { Text = "Удалить", Dock = DockStyle.Left, Width = 100 };
            var closeButton = new Button { Text = "Закрыть", Dock = DockStyle.Right, Width = 100 };

            addButton.Click += AddButton_Click;
            editButton.Click += EditButton_Click;
            deleteButton.Click += DeleteButton_Click;
            closeButton.Click += (s, e) => this.Close();

            panel.Controls.Add(addButton);
            panel.Controls.Add(editButton);
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(closeButton);

            this.Controls.Add(_expensesGrid);
            this.Controls.Add(panel);
        }

        private async void LoadExpenses()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                    "SELECT id, category, quantity, sum FROM expenses WHERE report_id = @report_id",
                    _connection))
                {
                    cmd.Parameters.AddWithValue("@report_id", _reportId);

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _expensesGrid.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке расходов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var form = new EditExpenseForm(_connection, _reportId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadExpenses();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (_expensesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите расход для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var expenseId = (int)_expensesGrid.SelectedRows[0].Cells["Id"].Value;
            var form = new EditExpenseForm(_connection, _reportId, expenseId);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadExpenses();
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_expensesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите расход для удаления!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var expenseId = (int)_expensesGrid.SelectedRows[0].Cells["Id"].Value;

            if (MessageBox.Show("Удалить выбранный расход?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                    "DELETE FROM expenses WHERE id = @id", _connection))
                {
                    cmd.Parameters.AddWithValue("@id", expenseId);
                    await cmd.ExecuteNonQueryAsync();
                    LoadExpenses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении расхода: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
