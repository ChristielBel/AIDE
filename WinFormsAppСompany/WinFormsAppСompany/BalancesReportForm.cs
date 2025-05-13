using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;
using Excel = Microsoft.Office.Interop.Excel;

namespace WinFormsAppСompany
{
    public partial class BalancesReportForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private DataGridView _dataGridView;
        private Button _exportButton;
        private DateTimePicker _dateFromPicker;
        private DateTimePicker _dateToPicker;
        private ComboBox _employeeComboBox;

        public BalancesReportForm(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            this.Text = "Отчет по остаткам подотчетных средств";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Панель фильтров
            var filterPanel = new Panel { Dock = DockStyle.Top, Height = 100 };

            _employeeComboBox = new ComboBox
            {
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(120, 10)
            };

            _dateFromPicker = new DateTimePicker
            {
                Width = 120,
                Location = new Point(120, 40),
                Format = DateTimePickerFormat.Short
            };

            _dateToPicker = new DateTimePicker
            {
                Width = 120,
                Location = new Point(300, 40),
                Format = DateTimePickerFormat.Short
            };

            var filterButton = new Button
            {
                Text = "Применить фильтры",
                Location = new Point(450, 10),
                AutoSize = true
            };

            filterButton.Click += FilterButton_Click;

            filterPanel.Controls.Add(new Label { Text = "Сотрудник:", Location = new Point(10, 12) });
            filterPanel.Controls.Add(_employeeComboBox);
            filterPanel.Controls.Add(new Label { Text = "Период с:", Location = new Point(10, 42) });
            filterPanel.Controls.Add(_dateFromPicker);
            filterPanel.Controls.Add(new Label { Text = "по:", Location = new Point(250, 42) });
            filterPanel.Controls.Add(_dateToPicker);
            filterPanel.Controls.Add(filterButton);

            // DataGridView
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            // Кнопка экспорта
            _exportButton = new Button
            {
                Text = "Экспорт в Excel",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            _exportButton.Click += ExportButton_Click;

            this.Controls.Add(_dataGridView);
            this.Controls.Add(_exportButton);
            this.Controls.Add(filterPanel);

            // Установка дат по умолчанию
            _dateFromPicker.Value = DateTime.Now.AddMonths(-1);
            _dateToPicker.Value = DateTime.Now;
        }

        private void LoadEmployees()
        {
            try
            {
                using (var cmd = new NpgsqlCommand("SELECT employee_id, name FROM employees ORDER BY name", _connection))
                {
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _employeeComboBox.DataSource = dt;
                    _employeeComboBox.DisplayMember = "name";
                    _employeeComboBox.ValueMember = "employee_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            try
            {
                var query = @"
                    SELECT 
                        e.name AS Сотрудник,
                        b.month AS Месяц,
                        b.end_balance AS Остаток
                    FROM balances b
                    JOIN employees e ON b.employee_id = e.employee_id
                    WHERE 1=1";

                var parameters = new List<NpgsqlParameter>();

                if (_employeeComboBox.SelectedValue != null)
                {
                    query += " AND b.employee_id = @employee_id";
                    parameters.Add(new NpgsqlParameter("@employee_id", _employeeComboBox.SelectedValue));
                }

                query += " AND b.month BETWEEN @from_month AND @to_month";
                parameters.Add(new NpgsqlParameter("@from_month", _dateFromPicker.Value.ToString("yyyy-MM")));
                parameters.Add(new NpgsqlParameter("@to_month", _dateToPicker.Value.ToString("yyyy-MM")));

                query += " ORDER BY e.name, b.month";

                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _dataGridView.DataSource = dt;

                    // Форматирование столбцов
                    if (_dataGridView.Columns.Contains("Остаток"))
                    {
                        _dataGridView.Columns["Остаток"].DefaultCellStyle.Format = "N2";
                        _dataGridView.Columns["Остаток"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (_dataGridView.DataSource == null || ((DataTable)_dataGridView.DataSource).Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"Отчет по остаткам на {DateTime.Now:yyyy-MM-dd}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excelApp = null;
                    Excel.Workbook workbook = null;
                    Excel.Worksheet worksheet = null;

                    try
                    {
                        // Создаем экземпляр Excel
                        excelApp = new Excel.Application();
                        excelApp.Visible = false;

                        // Создаем новую книгу
                        workbook = excelApp.Workbooks.Add();
                        worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                        worksheet.Name = "Остатки";

                        // Заголовки
                        for (int i = 0; i < _dataGridView.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1] = _dataGridView.Columns[i].HeaderText;
                        }

                        // Данные
                        var dataTable = (DataTable)_dataGridView.DataSource;
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1] = dataTable.Rows[row][col];
                            }
                        }

                        // Форматирование
                        Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, _dataGridView.Columns.Count]];
                        headerRange.Font.Bold = true;
                        headerRange.Interior.Color = Color.LightGray.ToArgb();

                        // Форматирование столбца с остатками
                        if (_dataGridView.Columns.Contains("Остаток"))
                        {
                            int columnIndex = _dataGridView.Columns["Остаток"].Index + 1;
                            Excel.Range sumRange = worksheet.Range[
                                worksheet.Cells[2, columnIndex],
                                worksheet.Cells[dataTable.Rows.Count + 1, columnIndex]];
                            sumRange.NumberFormat = "#,##0.00";
                            sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        }

                        // Автоподбор ширины столбцов
                        worksheet.Columns.AutoFit();

                        // Сохранение файла
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно экспортирован в Excel!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Освобождение ресурсов
                        if (workbook != null)
                        {
                            workbook.Close(false);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        }

                        if (excelApp != null)
                        {
                            excelApp.Quit();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        }

                        worksheet = null;
                        workbook = null;
                        excelApp = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }
        }
    }
}