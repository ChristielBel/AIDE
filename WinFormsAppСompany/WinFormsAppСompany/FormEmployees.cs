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
    public partial class FormEmployees : Form
    {
        private DataGridView dataGridView;
        private NpgsqlConnection connection;
        private MenuStrip menuStrip;

        public FormEmployees(NpgsqlConnection conn)
        {
            connection = conn;
            InitializeComponents();
            LoadEmployees();
        }

        private void InitializeComponents()
        {
            this.Text = "Список сотрудников";
            this.Size = new Size(800, 600);

            menuStrip = new MenuStrip();

            var fileMenuItem = new ToolStripMenuItem("Файл");
            var editMenuItem = new ToolStripMenuItem("Действия");

            var addItem = new ToolStripMenuItem("Добавить", null, AddItem_Click);
            var editItem = new ToolStripMenuItem("Изменить", null, EditItem_Click);
            var deleteItem = new ToolStripMenuItem("Удалить", null, DeleteItem_Click);
            var exitItem = new ToolStripMenuItem("Выход", null, ExitItem_Click);

            editMenuItem.DropDownItems.AddRange(new[] { addItem, editItem, deleteItem });
            fileMenuItem.DropDownItems.Add(exitItem);

            menuStrip.Items.AddRange(new[] { fileMenuItem, editMenuItem });

            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(dataGridView);
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private void LoadEmployees()
        {
            var cmd = new NpgsqlCommand("SELECT employee_id, name, department, balance FROM employees", connection);
            var adapter = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            dataGridView.DataSource = dt;
        }

        private void AddItem_Click(object sender, EventArgs e)
        {
            var form = new AddEmployeeForm(connection);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadEmployees(); 
            }
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!");
                return;
            }

            var row = dataGridView.SelectedRows[0];
            var id = (int)row.Cells["employee_id"].Value;
            var name = row.Cells["name"].Value.ToString();
            var department = row.Cells["department"].Value.ToString();

            var form = new EditEmployeeForm(connection, id, name, department);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadEmployees(); 
            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для удаления!");
                return;
            }

            var row = dataGridView.SelectedRows[0];
            var id = (int)row.Cells["employee_id"].Value;
            var name = row.Cells["name"].Value.ToString();

            if (MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {name}?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var cmd = new NpgsqlCommand("DELETE FROM employees WHERE employee_id = @id", connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadEmployees();
                    MessageBox.Show("Сотрудник удален!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class EditEmployeeForm : Form
    {
        private TextBox nameTextBox;
        private TextBox departmentTextBox;
        private Button saveButton;
        private Button cancelButton;
        private NpgsqlConnection connection;
        private int employeeId;

        public EditEmployeeForm(NpgsqlConnection conn, int id, string name, string department)
        {
            connection = conn;
            employeeId = id;
            InitializeComponents();
            nameTextBox.Text = name;
            departmentTextBox.Text = department;
        }

        private void InitializeComponents()
        {
            this.Text = "Редактировать сотрудника";
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
                "Вы уверены, что хотите сохранить изменения?",
                "Подтверждение изменений",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            var cmd = new NpgsqlCommand(
                "UPDATE employees SET name = @name, department = @department WHERE employee_id = @id",
                connection);

            cmd.Parameters.AddWithValue("@name", nameTextBox.Text);
            cmd.Parameters.AddWithValue("@department", departmentTextBox.Text);
            cmd.Parameters.AddWithValue("@id", employeeId);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Изменения успешно сохранены!");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
