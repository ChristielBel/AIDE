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
    public partial class AddEmployeeForm : Form
    {
        private TextBox nameTextBox;
        private TextBox departmentTextBox;
        private Button saveButton;
        private Button cancelButton;
        private NpgsqlConnection connection;

        public AddEmployeeForm(NpgsqlConnection conn)
        {
            connection = conn;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Добавить сотрудника";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            nameTextBox = new TextBox { Location = new Point(120, 20), Width = 250 };
            departmentTextBox = new TextBox { Location = new Point(120, 50), Width = 250 };
            saveButton = new Button { Text = "Сохранить", Location = new Point(120, 100) };
            cancelButton = new Button { Text = "Отмена", Location = new Point(220, 100) };

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(new Label { Text = "ФИО:", Location = new Point(20, 20) });
            this.Controls.Add(nameTextBox);
            this.Controls.Add(new Label { Text = "Отдел:", Location = new Point(20, 50) });
            this.Controls.Add(departmentTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = saveButton;
            this.CancelButton = cancelButton;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника!");
                return;
            }

            var result = MessageBox.Show(
                "Вы уверены, что хотите добавить нового сотрудника?",
                "Подтверждение добавления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            var cmd = new NpgsqlCommand(
                "INSERT INTO employees (name, department) VALUES (@name, @department)",
                connection);

            cmd.Parameters.AddWithValue("@name", nameTextBox.Text);
            cmd.Parameters.AddWithValue("@department", departmentTextBox.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Сотрудник успешно добавлен!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
