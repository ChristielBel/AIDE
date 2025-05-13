using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace WinFormsAppСompany
{
    public partial class AddReportForm : Form
    {
        private class EmployeeItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name;
        }

        private class AdvanceItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        private readonly NpgsqlConnection _connection;
        private ComboBox _employeeComboBox;
        private ComboBox _advanceComboBox;
        private DateTimePicker _reportDatePicker;
        private TextBox _sumTextBox;

        public AddReportForm(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            InitializeComponents();
            LoadEmployeesAsync();
        }

        private void InitializeComponents()
        {
            Text = "Добавить отчёт";
            Size = new System.Drawing.Size(500, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;

            _employeeComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(150, 20),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _advanceComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(150, 60),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            _reportDatePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(150, 100),
                Width = 300
            };

            _sumTextBox = new TextBox
            {
                Location = new System.Drawing.Point(150, 140),
                Width = 300
            };

            var saveButton = new Button
            {
                Text = "Сохранить",
                Location = new System.Drawing.Point(150, 200)
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new System.Drawing.Point(270, 200)
            };

            _employeeComboBox.SelectedIndexChanged += EmployeeComboBox_SelectedIndexChanged;
            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => Close();

            Controls.Add(new Label { Text = "Сотрудник:", Location = new System.Drawing.Point(20, 20) });
            Controls.Add(_employeeComboBox);
            Controls.Add(new Label { Text = "Аванс:", Location = new System.Drawing.Point(20, 60) });
            Controls.Add(_advanceComboBox);
            Controls.Add(new Label { Text = "Дата отчёта:", Location = new System.Drawing.Point(20, 100) });
            Controls.Add(_reportDatePicker);
            Controls.Add(new Label { Text = "Сумма:", Location = new System.Drawing.Point(20, 140) });
            Controls.Add(_sumTextBox);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
        }

        private bool _isLoadingEmployees = false;


        private async void LoadEmployeesAsync()
        {
            try
            {
                _isLoadingEmployees = true;
                _employeeComboBox.Enabled = false;

                var employees = new List<EmployeeItem>();

                await using (var cmd = new NpgsqlCommand("SELECT employee_id, name FROM employees ORDER BY name", _connection))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new EmployeeItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }

                _employeeComboBox.DataSource = employees;
                _employeeComboBox.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}");
            }
            finally
            {
                _isLoadingEmployees = false;
            }
        }

        private async void EmployeeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoadingEmployees) return;

            if (_employeeComboBox.SelectedItem is EmployeeItem employee)
            {
                await LoadAdvancesAsync(employee.Id);
            }
        }


        private async Task LoadAdvancesAsync(int employeeId)
        {
            try
            {
                _advanceComboBox.Enabled = false;
                _advanceComboBox.DataSource = null;
                _advanceComboBox.Items.Clear();

                await using (var cmd = new NpgsqlCommand(
                    "SELECT id, date, sum FROM advances WHERE employee_id = @id AND reported = FALSE ORDER BY date DESC",
                    _connection))
                {
                    cmd.Parameters.AddWithValue("@id", employeeId);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var advances = new List<AdvanceItem>();

                        while (await reader.ReadAsync())
                        {
                            advances.Add(new AdvanceItem
                            {
                                Id = reader.GetInt32(0),
                                Description = $"#{reader.GetInt32(0)} от {reader.GetDateTime(1):dd.MM.yyyy} на {reader.GetDecimal(2):C}"
                            });
                        }

                        if (advances.Count == 0)
                        {
                            _advanceComboBox.Items.Add("Нет доступных авансов");
                            _advanceComboBox.SelectedIndex = 0;
                        }
                        else
                        {
                            _advanceComboBox.DataSource = advances;
                            _advanceComboBox.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке авансов: {ex.Message}");
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (_advanceComboBox.SelectedItem is not AdvanceItem advance)
            {
                MessageBox.Show("Выберите аванс!");
                return;
            }

            if (!decimal.TryParse(_sumTextBox.Text, out decimal sum) || sum <= 0)
            {
                MessageBox.Show("Введите корректную сумму!");
                return;
            }

            try
            {
                await using (var transaction = await _connection.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Добавляем отчет
                        await using (var cmd = new NpgsqlCommand(
                            "INSERT INTO reports (advance_id, date, sum) VALUES (@advance_id, @date, @sum)",
                            _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@advance_id", advance.Id);
                            cmd.Parameters.AddWithValue("@date", _reportDatePicker.Value);
                            cmd.Parameters.AddWithValue("@sum", sum);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // 2. Обновляем флаг reported в авансе
                        await using (var cmd = new NpgsqlCommand(
                            "UPDATE advances SET reported = TRUE WHERE id = @id",
                            _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", advance.Id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // 3. Обновляем баланс сотрудника
                        var employeeId = ((EmployeeItem)_employeeComboBox.SelectedItem).Id;
                        await using (var cmd = new NpgsqlCommand(
                            "UPDATE employees SET balance = balance - @sum WHERE employee_id = @employee_id",
                            _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@sum", sum);
                            cmd.Parameters.AddWithValue("@employee_id", employeeId);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                        MessageBox.Show("Отчёт успешно добавлен!");
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        MessageBox.Show($"Ошибка при сохранении отчета: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при начале транзакции: {ex.Message}");
            }
        }
    }
}