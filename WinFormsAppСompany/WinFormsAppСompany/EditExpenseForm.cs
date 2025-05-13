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
    public partial class EditExpenseForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private readonly int _reportId;
        private readonly int? _expenseId;

        private TextBox _categoryTextBox, _quantityTextBox, _sumTextBox;

        public EditExpenseForm(NpgsqlConnection conn, int reportId, int? expenseId = null)
        {
            _connection = conn ?? throw new ArgumentNullException(nameof(conn));
            _reportId = reportId;
            _expenseId = expenseId;

            InitializeComponents();

            if (_expenseId.HasValue)
            {
                LoadExpenseData().ConfigureAwait(false);
            }
        }

        private void InitializeComponents()
        {
            this.Text = _expenseId.HasValue ? "Редактирование расхода" : "Добавление расхода";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            _categoryTextBox = new TextBox { Dock = DockStyle.Fill };
            _quantityTextBox = new TextBox { Dock = DockStyle.Fill };
            _sumTextBox = new TextBox { Dock = DockStyle.Fill };

            panel.Controls.Add(new Label { Text = "Категория:", AutoSize = true }, 0, 0);
            panel.Controls.Add(_categoryTextBox, 1, 0);
            panel.Controls.Add(new Label { Text = "Количество:", AutoSize = true }, 0, 1);
            panel.Controls.Add(_quantityTextBox, 1, 1);
            panel.Controls.Add(new Label { Text = "Сумма:", AutoSize = true }, 0, 2);
            panel.Controls.Add(_sumTextBox, 1, 2);

            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            var saveButton = new Button { Text = "Сохранить", Dock = DockStyle.Right };
            var cancelButton = new Button { Text = "Отмена", Dock = DockStyle.Right };

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            this.Controls.Add(panel);
            this.Controls.Add(buttonPanel);
        }

        private async Task LoadExpenseData()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();

                await using (var cmd = new NpgsqlCommand(
                    "SELECT category, quantity, sum FROM expenses WHERE id = @id", _connection))
                {
                    cmd.Parameters.AddWithValue("@id", _expenseId.Value);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _categoryTextBox.Text = reader.GetString(0);
                            _quantityTextBox.Text = reader.GetDecimal(1).ToString();
                            _sumTextBox.Text = reader.GetDecimal(2).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных расхода: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(_quantityTextBox.Text, out decimal quantity) ||
                !decimal.TryParse(_sumTextBox.Text, out decimal sum))
            {
                MessageBox.Show("Введите корректные числовые значения!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_connection.State != ConnectionState.Open)
                    await _connection.OpenAsync();

                if (_expenseId.HasValue)
                {
                    // Обновление существующего расхода
                    await using (var cmd = new NpgsqlCommand(
                        "UPDATE expenses SET category = @category, quantity = @quantity, sum = @sum " +
                        "WHERE id = @id", _connection))
                    {
                        cmd.Parameters.AddWithValue("@category", _categoryTextBox.Text);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@sum", sum);
                        cmd.Parameters.AddWithValue("@id", _expenseId.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    // Добавление нового расхода
                    await using (var cmd = new NpgsqlCommand(
                        "INSERT INTO expenses (report_id, category, quantity, sum) " +
                        "VALUES (@report_id, @category, @quantity, @sum)", _connection))
                    {
                        cmd.Parameters.AddWithValue("@report_id", _reportId);
                        cmd.Parameters.AddWithValue("@category", _categoryTextBox.Text);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@sum", sum);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении расхода: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
