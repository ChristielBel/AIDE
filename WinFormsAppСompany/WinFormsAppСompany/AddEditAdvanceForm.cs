using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsAppСompany
{
    public partial class AddEditAdvanceForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private readonly int? _advanceId;

        private ComboBox _employeeComboBox;
        private DateTimePicker _datePicker;
        private TextBox _sumTextBox;

        public AddEditAdvanceForm(NpgsqlConnection connection,
                                int? advanceId = null,
                                string employeeName = null,
                                DateTime? date = null,
                                decimal? sum = null)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _advanceId = advanceId;

            InitializeComponents();

            if (_advanceId.HasValue)
            {
                this.Text = "Редактирование аванса";
                _datePicker.Value = date ?? DateTime.Now;
                _sumTextBox.Text = sum?.ToString("N2") ?? "";

                _employeeComboBox.Items.Add(employeeName);
                _employeeComboBox.SelectedIndex = 0;
                _employeeComboBox.Enabled = false;
            }
            else
            {
                this.Text = "Добавление аванса";
                LoadEmployees();
            }
        }

        private void InitializeComponents()
        {
            this.ClientSize = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10),
                AutoSize = true
            };

            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); 
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  

            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _employeeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _datePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short
            };

            _sumTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Введите сумму"
            };

            mainTable.Controls.Add(new Label { Text = "Сотрудник:", AutoSize = true }, 0, 0);
            mainTable.Controls.Add(_employeeComboBox, 1, 0);

            mainTable.Controls.Add(new Label { Text = "Дата:", AutoSize = true }, 0, 1);
            mainTable.Controls.Add(_datePicker, 1, 1);

            mainTable.Controls.Add(new Label { Text = "Сумма:", AutoSize = true }, 0, 2);
            mainTable.Controls.Add(_sumTextBox, 1, 2);

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(0, 5, 0, 0)
            };

            var saveButton = new Button
            {
                Text = "Сохранить",
                DialogResult = DialogResult.OK,
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Right
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Size = new Size(80, 30),
                Anchor = AnchorStyles.Right,
                Margin = new Padding(5, 0, 0, 0)
            };

            saveButton.Click += SaveButton_Click;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            saveButton.Location = new Point(
                buttonPanel.Width - saveButton.Width - cancelButton.Width - 5,
                (buttonPanel.Height - saveButton.Height) / 2);

            cancelButton.Location = new Point(
                buttonPanel.Width - cancelButton.Width,
                (buttonPanel.Height - cancelButton.Height) / 2);

            this.Controls.Add(mainTable);
            this.Controls.Add(buttonPanel);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private void LoadEmployees()
        {
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT employee_id, name FROM employees ORDER BY name", _connection))
                using (var reader = cmd.ExecuteReader())
                {
                    var employees = new List<EmployeeItem>();

                    while (reader.Read())
                    {
                        employees.Add(new EmployeeItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    _employeeComboBox.DataSource = employees;
                    _employeeComboBox.DisplayMember = "Name";
                    _employeeComboBox.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_employeeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(_sumTextBox.Text, out decimal sum) || sum <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var employeeId = ((EmployeeItem)_employeeComboBox.SelectedItem).Id;

                if (_advanceId.HasValue)
                {
                    // Редактирование существующего аванса
                    using (var cmd = new NpgsqlCommand(
                        "UPDATE advances SET date = @date, sum = @sum WHERE id = @id", _connection))
                    {
                        cmd.Parameters.AddWithValue("@date", _datePicker.Value);
                        cmd.Parameters.AddWithValue("@sum", sum);
                        cmd.Parameters.AddWithValue("@id", _advanceId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Добавление нового аванса
                    using (var cmd = new NpgsqlCommand(
                        "INSERT INTO advances (employee_id, date, sum) VALUES (@employee_id, @date, @sum)", _connection))
                    {
                        cmd.Parameters.AddWithValue("@employee_id", employeeId);
                        cmd.Parameters.AddWithValue("@date", _datePicker.Value);
                        cmd.Parameters.AddWithValue("@sum", sum);
                        cmd.ExecuteNonQuery();
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении аванса: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class EmployeeItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}