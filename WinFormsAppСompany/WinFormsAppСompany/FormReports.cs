using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsAppСompany
{
    public partial class FormReports : Form
    {
        private readonly NpgsqlConnection _connection;
        private DataGridView _dataGridView;
        private MenuStrip _menuStrip;

        public FormReports(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            InitializeComponents();
            LoadReports();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление отчетами";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            _menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var actionMenu = new ToolStripMenuItem("Действия");

            var refreshItem = new ToolStripMenuItem("Обновить", null, (s, e) => LoadReports());
            var addItem = new ToolStripMenuItem("Добавить отчет", null, AddReport);
            var viewExpensesItem = new ToolStripMenuItem("Просмотр расходов", null, ViewExpenses);
            var deleteItem = new ToolStripMenuItem("Удалить", null, DeleteReport);
            var exitItem = new ToolStripMenuItem("Выход", null, (s, e) => this.Close());

            actionMenu.DropDownItems.AddRange(new[] { refreshItem, addItem, viewExpensesItem, deleteItem });
            fileMenu.DropDownItems.Add(exitItem);

            _menuStrip.Items.AddRange(new[] { fileMenu, actionMenu });

            this.Controls.Add(_dataGridView);
            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;

            _dataGridView.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) ViewExpenses(s, e);
            };
        }

        private void LoadReports()
        {
            try
            {
                using (var cmd = new NpgsqlCommand(
                    @"SELECT r.id, e.name AS employee, a.date AS advance_date, 
                             a.sum AS advance_sum, r.date AS report_date, 
                             r.sum AS report_sum, a.id AS advance_id
                      FROM reports r
                      JOIN advances a ON r.advance_id = a.id
                      JOIN employees e ON a.employee_id = e.employee_id
                      ORDER BY r.date DESC", _connection))
                {
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _dataGridView.DataSource = dt;

                    if (_dataGridView.Columns.Contains("report_date"))
                    {
                        _dataGridView.Columns["report_date"].HeaderText = "Дата отчета";
                        _dataGridView.Columns["report_date"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }
                    if (_dataGridView.Columns.Contains("advance_date"))
                    {
                        _dataGridView.Columns["advance_date"].HeaderText = "Дата аванса";
                        _dataGridView.Columns["advance_date"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }
                    if (_dataGridView.Columns.Contains("advance_sum"))
                    {
                        _dataGridView.Columns["advance_sum"].HeaderText = "Сумма аванса";
                        _dataGridView.Columns["advance_sum"].DefaultCellStyle.Format = "N2";
                    }
                    if (_dataGridView.Columns.Contains("report_sum"))
                    {
                        _dataGridView.Columns["report_sum"].HeaderText = "Сумма отчета";
                        _dataGridView.Columns["report_sum"].DefaultCellStyle.Format = "N2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчетов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddReport(object sender, EventArgs e)
        {
            var form = new AddReportForm(_connection);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadReports();
            }
        }

        private void ViewExpenses(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчет для просмотра расходов!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _dataGridView.SelectedRows[0];
            var reportId = Convert.ToInt32(row.Cells["id"].Value);
            var employeeName = row.Cells["employee"].Value.ToString();
            var reportSum = Convert.ToDecimal(row.Cells["report_sum"].Value);

            var form = new ExpensesForm(_connection, reportId);
            form.ShowDialog();
        }

        private void DeleteReport(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите отчет для удаления!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _dataGridView.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["id"].Value);
            var advanceId = Convert.ToInt32(row.Cells["advance_id"].Value);

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный отчет? Все связанные расходы также будут удалены.",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Удаляем все расходы по отчету
                        using (var cmd = new NpgsqlCommand(
                            "DELETE FROM expenses WHERE report_id = @report_id", _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@report_id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Удаляем сам отчет
                        using (var cmd = new NpgsqlCommand(
                            "DELETE FROM reports WHERE id = @id", _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Сбрасываем флаг reported в авансе
                        using (var cmd = new NpgsqlCommand(
                            "UPDATE advances SET reported = FALSE WHERE id = @advance_id", _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@advance_id", advanceId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        LoadReports();
                        MessageBox.Show("Отчет успешно удален!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при удалении отчета: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при начале транзакции: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}