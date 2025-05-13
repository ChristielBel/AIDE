using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;
using Excel = Microsoft.Office.Interop.Excel;

namespace WinFormsAppСompany
{
    public partial class ReportsExpensesForm : Form
    {
        private readonly NpgsqlConnection _connection;
        private DataGridView _dataGridView;
        private Button _exportButton;
        private DateTimePicker _dateFromPicker;
        private DateTimePicker _dateToPicker;
        private ComboBox _employeeComboBox;

        public ReportsExpensesForm(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            this.Text = "Отчет по авансам и расходам";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);

            var mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 80)); 
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); 
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); 

            var filterPanel = new Panel { Dock = DockStyle.Fill };

            var filterTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                Padding = new Padding(5),
                AutoSize = true
            };

            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            filterTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            _employeeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _dateFromPicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short
            };

            _dateToPicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short
            };

            var filterButton = new Button
            {
                Text = "Сформировать отчет",
                Dock = DockStyle.Fill,
                Height = 30
            };
            filterButton.Click += FilterButton_Click;

            filterTable.Controls.Add(new Label { Text = "Сотрудник:", AutoSize = true }, 0, 0);
            filterTable.Controls.Add(_employeeComboBox, 1, 0);
            filterTable.Controls.Add(new Label { Text = "Период с:", AutoSize = true }, 2, 0);
            filterTable.Controls.Add(_dateFromPicker, 3, 0);
            filterTable.Controls.Add(filterButton, 4, 0);

            filterTable.Controls.Add(new Label(), 0, 1); 
            filterTable.Controls.Add(new Label(), 1, 1); 
            filterTable.Controls.Add(new Label { Text = "по:", AutoSize = true }, 2, 1);
            filterTable.Controls.Add(_dateToPicker, 3, 1);
            filterTable.Controls.Add(new Label(), 4, 1); 

            filterPanel.Controls.Add(filterTable);

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D
            };

            _exportButton = new Button
            {
                Text = "Экспорт в Excel",
                Dock = DockStyle.Fill,
                Height = 40
            };
            _exportButton.Click += ExportButton_Click;

            mainTable.Controls.Add(filterPanel, 0, 0);
            mainTable.Controls.Add(_dataGridView, 0, 1);
            mainTable.Controls.Add(_exportButton, 0, 2);

            this.Controls.Add(mainTable);

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

                    var allEmployeesRow = dt.NewRow();
                    allEmployeesRow["employee_id"] = -1;
                    allEmployeesRow["name"] = "Все сотрудники";
                    dt.Rows.InsertAt(allEmployeesRow, 0);

                    _employeeComboBox.SelectedIndex = 0;
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
                        r.id AS report_id,
                        e.name AS employee_name,
                        a.date AS advance_date,
                        a.sum AS advance_sum,
                        r.date AS report_date,
                        r.sum AS report_sum,
                        exp.category AS expense_category,
                        exp.quantity AS expense_quantity,
                        exp.sum AS expense_sum
                    FROM reports r
                    JOIN advances a ON r.advance_id = a.id
                    JOIN employees e ON a.employee_id = e.employee_id
                    JOIN expenses exp ON exp.report_id = r.id
                    WHERE r.date BETWEEN @date_from AND @date_to";

                var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@date_from", _dateFromPicker.Value.Date),
                    new NpgsqlParameter("@date_to", _dateToPicker.Value.Date)
                };

                if (_employeeComboBox.SelectedValue != null &&
                    (int)_employeeComboBox.SelectedValue != -1)
                {
                    query += " AND a.employee_id = @employee_id";
                    parameters.Add(new NpgsqlParameter("@employee_id", _employeeComboBox.SelectedValue));
                }

                query += " ORDER BY e.name, r.date, exp.category";

                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    _dataGridView.DataSource = dt;

                    FormatDataGridViewColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridViewColumns()
        {
            if (_dataGridView.Columns.Contains("advance_sum"))
            {
                _dataGridView.Columns["advance_sum"].HeaderText = "Сумма аванса";
                _dataGridView.Columns["advance_sum"].DefaultCellStyle.Format = "N2";
                _dataGridView.Columns["advance_sum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (_dataGridView.Columns.Contains("report_sum"))
            {
                _dataGridView.Columns["report_sum"].HeaderText = "Сумма отчета";
                _dataGridView.Columns["report_sum"].DefaultCellStyle.Format = "N2";
                _dataGridView.Columns["report_sum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (_dataGridView.Columns.Contains("expense_sum"))
            {
                _dataGridView.Columns["expense_sum"].HeaderText = "Сумма расхода";
                _dataGridView.Columns["expense_sum"].DefaultCellStyle.Format = "N2";
                _dataGridView.Columns["expense_sum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (_dataGridView.Columns.Contains("expense_quantity"))
            {
                _dataGridView.Columns["expense_quantity"].HeaderText = "Количество";
                _dataGridView.Columns["expense_quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (_dataGridView.Columns.Contains("advance_date"))
            {
                _dataGridView.Columns["advance_date"].HeaderText = "Дата аванса";
                _dataGridView.Columns["advance_date"].DefaultCellStyle.Format = "dd.MM.yyyy";
            }

            if (_dataGridView.Columns.Contains("report_date"))
            {
                _dataGridView.Columns["report_date"].HeaderText = "Дата отчета";
                _dataGridView.Columns["report_date"].DefaultCellStyle.Format = "dd.MM.yyyy";
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
                saveFileDialog.FileName = $"Отчет по авансам и расходам {DateTime.Now:yyyy-MM-dd}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excelApp = null;
                    Excel.Workbook workbook = null;
                    Excel.Worksheet worksheet = null;

                    try
                    {
                        excelApp = new Excel.Application();
                        excelApp.Visible = false;

                        workbook = excelApp.Workbooks.Add();
                        worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                        worksheet.Name = "Авансы и расходы";

                        for (int i = 0; i < _dataGridView.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1] = _dataGridView.Columns[i].HeaderText;
                        }

                        var dataTable = (DataTable)_dataGridView.DataSource;
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1] = dataTable.Rows[row][col];
                            }
                        }

                        Excel.Range headerRange = worksheet.Range[
                            worksheet.Cells[1, 1],
                            worksheet.Cells[1, _dataGridView.Columns.Count]];
                        headerRange.Font.Bold = true;
                        headerRange.Interior.Color = Color.LightGray.ToArgb();

                        FormatExcelColumns(worksheet, dataTable);

                        worksheet.Columns.AutoFit();

                        AddSummary(worksheet, dataTable);

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

        private void FormatExcelColumns(Excel.Worksheet worksheet, DataTable dataTable)
        {
            try
            {
                FormatColumnAsDate(worksheet, dataTable, "advance_date");
                FormatColumnAsDate(worksheet, dataTable, "report_date");

                string[] moneyColumns = { "advance_sum", "report_sum", "expense_sum" };
                foreach (var colName in moneyColumns)
                {
                    if (_dataGridView.Columns.Contains(colName))
                    {
                        FormatColumnAsCurrency(worksheet, dataTable, colName);
                    }
                }

                if (_dataGridView.Columns.Contains("expense_quantity"))
                {
                    FormatColumnAsNumber(worksheet, dataTable, "expense_quantity");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при форматировании столбцов Excel: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatColumnAsDate(Excel.Worksheet worksheet, DataTable dataTable, string columnName)
        {
            if (!_dataGridView.Columns.Contains(columnName)) return;

            int colIndex = _dataGridView.Columns[columnName].Index + 1;
            Excel.Range range = worksheet.Range[
                worksheet.Cells[2, colIndex],
                worksheet.Cells[dataTable.Rows.Count + 1, colIndex]];

            range.NumberFormat = "dd.MM.yyyy";
            range.EntireColumn.AutoFit();
        }

        private void FormatColumnAsCurrency(Excel.Worksheet worksheet, DataTable dataTable, string columnName)
        {
            if (!_dataGridView.Columns.Contains(columnName)) return;

            int colIndex = _dataGridView.Columns[columnName].Index + 1;
            Excel.Range range = worksheet.Range[
                worksheet.Cells[2, colIndex],
                worksheet.Cells[dataTable.Rows.Count + 1, colIndex]];

            range.NumberFormat = "#,##0.00";
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.EntireColumn.AutoFit();
        }

        private void FormatColumnAsNumber(Excel.Worksheet worksheet, DataTable dataTable, string columnName)
        {
            if (!_dataGridView.Columns.Contains(columnName)) return;

            int colIndex = _dataGridView.Columns[columnName].Index + 1;
            Excel.Range range = worksheet.Range[
                worksheet.Cells[2, colIndex],
                worksheet.Cells[dataTable.Rows.Count + 1, colIndex]];

            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            range.EntireColumn.AutoFit();
        }

        private void AddSummary(Excel.Worksheet worksheet, DataTable dataTable)
        {
            try
            {
                int lastRow = dataTable.Rows.Count + 2;

                AddSummaryRow(worksheet, dataTable, "advance_sum", lastRow, "Итого авансы:");
                AddSummaryRow(worksheet, dataTable, "report_sum", lastRow + 1, "Итого отчеты:");
                AddSummaryRow(worksheet, dataTable, "expense_sum", lastRow + 2, "Итого расходы:");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении итогов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSummaryRow(Excel.Worksheet worksheet, DataTable dataTable, string columnName, int row, string label)
        {
            if (!_dataGridView.Columns.Contains(columnName)) return;

            int colIndex = _dataGridView.Columns[columnName].Index + 1;

            Excel.Range labelCell = worksheet.Cells[row, colIndex - 1];
            labelCell.Value = label;
            labelCell.Font.Bold = true;

            Excel.Range sumCell = worksheet.Cells[row, colIndex];
            sumCell.Formula = $"=SUM({worksheet.Cells[2, colIndex].Address}:{worksheet.Cells[row - 1, colIndex].Address})";

            sumCell.NumberFormat = "#,##0.00";
            sumCell.Font.Bold = true;
            sumCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
        }
    }
}