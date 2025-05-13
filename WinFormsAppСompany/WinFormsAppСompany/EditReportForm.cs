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
    public partial class EditReportForm : Form
    {
        private DateTimePicker reportDatePicker;
        private TextBox sumTextBox;
        private Button saveButton, cancelButton;
        private NpgsqlConnection connection;
        private int reportId;

        public EditReportForm(NpgsqlConnection conn, int id, DateTime date, decimal sum)
        {
            connection = conn;
            reportId = id;
            InitializeComponents();

            reportDatePicker.Value = date;
            sumTextBox.Text = sum.ToString();
        }

        private void InitializeComponents()
        {
            this.Text = "Редактировать отчёт";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            reportDatePicker = new DateTimePicker { Location = new Point(120, 20), Width = 200 };
            sumTextBox = new TextBox { Location = new Point(120, 50), Width = 200 };

            saveButton = new Button { Text = "Сохранить", Location = new Point(120, 100) };
            cancelButton = new Button { Text = "Отмена", Location = new Point(220, 100) };

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => this.Close();

            this.Controls.Add(new Label { Text = "Дата отчёта:", Location = new Point(20, 20) });
            this.Controls.Add(reportDatePicker);
            this.Controls.Add(new Label { Text = "Сумма:", Location = new Point(20, 50) });
            this.Controls.Add(sumTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(sumTextBox.Text, out decimal sum))
            {
                MessageBox.Show("Введите корректную сумму!");
                return;
            }

            var cmd = new NpgsqlCommand("UPDATE reports SET date = @date, sum = @sum WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@date", reportDatePicker.Value);
            cmd.Parameters.AddWithValue("@sum", sum);
            cmd.Parameters.AddWithValue("@id", reportId);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Отчёт обновлён!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
