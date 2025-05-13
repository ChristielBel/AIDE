using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsAppСompany
{
    public partial class FormAdvances : Form
    {
        private readonly NpgsqlConnection _connection;
        private DataGridView _dataGridView;
        private MenuStrip _menuStrip;

        public FormAdvances(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            InitializeComponents();
            LoadAdvances();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление авансами";
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

            var refreshItem = new ToolStripMenuItem("Обновить", null, (s, e) => LoadAdvances());
            var addItem = new ToolStripMenuItem("Добавить аванс", null, AddAdvance);
            var editItem = new ToolStripMenuItem("Редактировать", null, EditAdvance);
            var deleteItem = new ToolStripMenuItem("Удалить", null, DeleteAdvance);
            var exitItem = new ToolStripMenuItem("Выход", null, (s, e) => this.Close());

            actionMenu.DropDownItems.AddRange(new[] { refreshItem, addItem, editItem, deleteItem });
            fileMenu.DropDownItems.Add(exitItem);

            _menuStrip.Items.AddRange(new[] { fileMenu, actionMenu });

            this.Controls.Add(_dataGridView);
            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
        }

        private void LoadAdvances()
        {
            try
            {
                using (var cmd = new NpgsqlCommand(
                    @"SELECT a.id, e.name AS employee, a.date, a.sum, a.reported 
                      FROM advances a
                      JOIN employees e ON a.employee_id = e.employee_id
                      ORDER BY a.date DESC", _connection))
                {
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _dataGridView.DataSource = dt;

                    if (_dataGridView.Columns.Contains("reported"))
                    {
                        _dataGridView.Columns["reported"].HeaderText = "Отчитано";
                    }
                    if (_dataGridView.Columns.Contains("date"))
                    {
                        _dataGridView.Columns["date"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }
                    if (_dataGridView.Columns.Contains("sum"))
                    {
                        _dataGridView.Columns["sum"].DefaultCellStyle.Format = "N2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке авансов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddAdvance(object sender, EventArgs e)
        {
            var form = new AddEditAdvanceForm(_connection);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadAdvances();
            }
        }

        private void EditAdvance(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите аванс для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _dataGridView.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["id"].Value);
            var employeeName = row.Cells["employee"].Value.ToString();
            var date = Convert.ToDateTime(row.Cells["date"].Value);
            var sum = Convert.ToDecimal(row.Cells["sum"].Value);
            var reported = Convert.ToBoolean(row.Cells["reported"].Value);

            if (reported)
            {
                MessageBox.Show("Нельзя редактировать аванс, по которому уже есть отчет!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var form = new AddEditAdvanceForm(_connection, id, employeeName, date, sum);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadAdvances();
            }
        }

        private void DeleteAdvance(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите аванс для удаления!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _dataGridView.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["id"].Value);
            var reported = Convert.ToBoolean(row.Cells["reported"].Value);

            if (reported)
            {
                MessageBox.Show("Нельзя удалить аванс, по которому уже есть отчет!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный аванс?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (var cmd = new NpgsqlCommand("DELETE FROM advances WHERE id = @id", _connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadAdvances();
                    MessageBox.Show("Аванс успешно удален!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении аванса: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}