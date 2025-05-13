namespace WinFormsAppСompany
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            menuStrip2 = new MenuStrip();
            employeesToolStripMenuItem = new ToolStripMenuItem();
            списокСотрудниковToolStripMenuItem = new ToolStripMenuItem();
            advancesToolStripMenuItem = new ToolStripMenuItem();
            выдатьАвансToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            просмотрОтчетовToolStripMenuItem = new ToolStripMenuItem();
            balancesToolStripMenuItem = new ToolStripMenuItem();
            отчетПоОстаткамToolStripMenuItem = new ToolStripMenuItem();
            графикОстатковToolStripMenuItem = new ToolStripMenuItem();
            menuStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Location = new Point(0, 24);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            menuStrip2.Items.AddRange(new ToolStripItem[] { employeesToolStripMenuItem, advancesToolStripMenuItem, reportsToolStripMenuItem, balancesToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Size = new Size(800, 24);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // employeesToolStripMenuItem
            // 
            employeesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { списокСотрудниковToolStripMenuItem });
            employeesToolStripMenuItem.Name = "employeesToolStripMenuItem";
            employeesToolStripMenuItem.Size = new Size(85, 20);
            employeesToolStripMenuItem.Text = "Сотрудники";
            employeesToolStripMenuItem.Click += employeesToolStripMenuItem_Click;
            // 
            // списокСотрудниковToolStripMenuItem
            // 
            списокСотрудниковToolStripMenuItem.Name = "списокСотрудниковToolStripMenuItem";
            списокСотрудниковToolStripMenuItem.Size = new Size(188, 22);
            списокСотрудниковToolStripMenuItem.Text = "Список сотрудников";
            списокСотрудниковToolStripMenuItem.Click += списокСотрудниковToolStripMenuItem_Click_1;
            // 
            // advancesToolStripMenuItem
            // 
            advancesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { выдатьАвансToolStripMenuItem });
            advancesToolStripMenuItem.Name = "advancesToolStripMenuItem";
            advancesToolStripMenuItem.Size = new Size(61, 20);
            advancesToolStripMenuItem.Text = "Авансы";
            advancesToolStripMenuItem.Click += advancesToolStripMenuItem_Click;
            // 
            // выдатьАвансToolStripMenuItem
            // 
            выдатьАвансToolStripMenuItem.Name = "выдатьАвансToolStripMenuItem";
            выдатьАвансToolStripMenuItem.Size = new Size(178, 22);
            выдатьАвансToolStripMenuItem.Text = "Просмотр авансов";
            выдатьАвансToolStripMenuItem.Click += advancesToolStripMenuItem_Click;
            // 
            // reportsToolStripMenuItem
            // 
            reportsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { просмотрОтчетовToolStripMenuItem });
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(123, 20);
            reportsToolStripMenuItem.Text = "Отчеты о расходах";
            reportsToolStripMenuItem.Click += reportsToolStripMenuItem_Click;
            // 
            // просмотрОтчетовToolStripMenuItem
            // 
            просмотрОтчетовToolStripMenuItem.Name = "просмотрОтчетовToolStripMenuItem";
            просмотрОтчетовToolStripMenuItem.Size = new Size(177, 22);
            просмотрОтчетовToolStripMenuItem.Text = "Просмотр отчетов";
            просмотрОтчетовToolStripMenuItem.Click += просмотрОтчетовToolStripMenuItem_Click;
            // 
            // balancesToolStripMenuItem
            // 
            balancesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { отчетПоОстаткамToolStripMenuItem, графикОстатковToolStripMenuItem });
            balancesToolStripMenuItem.Name = "balancesToolStripMenuItem";
            balancesToolStripMenuItem.Size = new Size(63, 20);
            balancesToolStripMenuItem.Text = "Остаток";
            balancesToolStripMenuItem.Click += balancesToolStripMenuItem_Click;
            // 
            // отчетПоОстаткамToolStripMenuItem
            // 
            отчетПоОстаткамToolStripMenuItem.Name = "отчетПоОстаткамToolStripMenuItem";
            отчетПоОстаткамToolStripMenuItem.Size = new Size(180, 22);
            отчетПоОстаткамToolStripMenuItem.Text = "Отчет по остаткам";
            отчетПоОстаткамToolStripMenuItem.Click += отчетПоОстаткамToolStripMenuItem_Click;
            // 
            // графикОстатковToolStripMenuItem
            // 
            графикОстатковToolStripMenuItem.Name = "графикОстатковToolStripMenuItem";
            графикОстатковToolStripMenuItem.Size = new Size(180, 22);
            графикОстатковToolStripMenuItem.Text = "График остатков";
            графикОстатковToolStripMenuItem.Click += графикОстатковToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuStrip1);
            Controls.Add(menuStrip2);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem employeesToolStripMenuItem;
        private ToolStripMenuItem списокСотрудниковToolStripMenuItem;
        private ToolStripMenuItem advancesToolStripMenuItem;
        private ToolStripMenuItem reportsToolStripMenuItem;
        private ToolStripMenuItem balancesToolStripMenuItem;
        private ToolStripMenuItem выдатьАвансToolStripMenuItem;
        private ToolStripMenuItem просмотрОтчетовToolStripMenuItem;
        private ToolStripMenuItem отчетПоОстаткамToolStripMenuItem;
        private ToolStripMenuItem графикОстатковToolStripMenuItem;
    }
}
