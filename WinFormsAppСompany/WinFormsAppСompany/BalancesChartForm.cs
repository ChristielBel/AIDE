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
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace WinFormsAppСompany
{
    public partial class BalancesChartForm : Form
    {
        private Chart chart;
        private ComboBox employeeComboBox;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private Button buildButton;
        private NpgsqlConnection connection;

        public BalancesChartForm(NpgsqlConnection conn)
        {
            connection = conn;
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            this.Text = "График остатков подотчетных средств";
            this.Size = new Size(800, 600);

            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                AutoScroll = true 
            };


            employeeComboBox = new ComboBox { Location = new Point(120, 20), Width = 250 };
            fromDatePicker = new DateTimePicker { Location = new Point(120, 50), Width = 120 };
            toDatePicker = new DateTimePicker { Location = new Point(300, 50), Width = 120 };
            buildButton = new Button { Text = "Построить", Location = new Point(450, 50), Width = 100 };

            buildButton.Click += BuildButton_Click;

            panel.Controls.Add(new Label { Text = "Сотрудник:", Location = new Point(20, 20) });
            panel.Controls.Add(employeeComboBox);
            panel.Controls.Add(new Label { Text = "Период с:", Location = new Point(20, 50) });
            panel.Controls.Add(fromDatePicker);
            panel.Controls.Add(new Label { Text = "по:", Location = new Point(20, 50) });
            panel.Controls.Add(toDatePicker);
            panel.Controls.Add(buildButton);

            chart = new Chart { Dock = DockStyle.Fill };
            chart.ChartAreas.Add(new ChartArea());

            this.Controls.Add(chart);
            this.Controls.Add(panel);
        }

        private void LoadEmployees()
        {
            var cmd = new NpgsqlCommand("SELECT employee_id, name FROM employees", connection);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                employeeComboBox.Items.Add(new KeyValuePair<int, string>(
                    reader.GetInt32(0), reader.GetString(1)));
            }

            reader.Close();
            employeeComboBox.DisplayMember = "Value";
            employeeComboBox.ValueMember = "Key";
        }

        private void BuildButton_Click(object sender, EventArgs e)
        {
            if (employeeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника!");
                return;
            }

            var employeeId = ((KeyValuePair<int, string>)employeeComboBox.SelectedItem).Key;
            var fromDate = fromDatePicker.Value;
            var toDate = toDatePicker.Value;

            var cmd = new NpgsqlCommand(@"
        SELECT month, end_balance 
        FROM balances 
        WHERE employee_id = @employeeId 
          AND month BETWEEN @fromMonth AND @toMonth
        ORDER BY month", connection);

            cmd.Parameters.AddWithValue("@employeeId", employeeId);
            cmd.Parameters.AddWithValue("@fromMonth", fromDate.ToString("yyyy-MM"));
            cmd.Parameters.AddWithValue("@toMonth", toDate.ToString("yyyy-MM"));

            var reader = cmd.ExecuteReader();

            chart.Series.Clear();
            chart.Titles.Clear();

            var series = new Series
            {
                Name = "Остаток",
                ChartType = SeriesChartType.Spline,
                XValueType = ChartValueType.DateTime,
                BorderWidth = 3,
                Color = Color.MediumSlateBlue,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                MarkerColor = Color.DarkBlue,
            };

            while (reader.Read())
            {
                string monthStr = reader.GetString(0); 
                decimal balance = reader.GetDecimal(1); 

                try
                {
                    DateTime date = DateTime.ParseExact(monthStr + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    series.Points.AddXY(date, balance);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при парсинге даты: {monthStr}\n{ex.Message}");
                }
            }

            reader.Close();
            chart.Series.Add(series);

            var area = chart.ChartAreas[0];
            area.AxisX.Title = "Месяц";
            area.AxisX.LabelStyle.Format = "yyyy-MM";
            area.AxisX.Interval = 1;
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.MajorGrid.LineColor = Color.LightGray;

            area.AxisY.Title = "Остаток";
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.LabelStyle.Format = "N0"; 

            chart.Titles.Add(new Title
            {
                Text = $"Остаток подотчетных средств для {((KeyValuePair<int, string>)employeeComboBox.SelectedItem).Value}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray,
                Docking = Docking.Top,
                Alignment = ContentAlignment.MiddleCenter
            });

            if (chart.Legends.Count == 0)
                chart.Legends.Add(new Legend());

            chart.Legends[0].Docking = Docking.Bottom;
            chart.Legends[0].Alignment = StringAlignment.Center;
        }
    }
}
