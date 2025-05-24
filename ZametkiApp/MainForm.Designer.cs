namespace CalendarApp
{
    partial class MainForm
    {
        private System.Windows.Forms.ComboBox comboBoxProfiles;
        private System.Windows.Forms.Button btnAddProfile;
        private System.Windows.Forms.Button btnDeleteProfile;

        private System.Windows.Forms.TabControl tabControlMain;

        private System.Windows.Forms.TabPage tabPageNotes;
        private System.Windows.Forms.ListBox listBoxNotes;
        private System.Windows.Forms.TextBox textBoxNoteText;
        private System.Windows.Forms.Button btnAddNote;
        private System.Windows.Forms.Button btnDeleteNote;

        private System.Windows.Forms.TabPage tabPageReminders;
        private System.Windows.Forms.ListBox listBoxReminders;
        private System.Windows.Forms.TextBox textBoxReminderText;
        private System.Windows.Forms.DateTimePicker dateTimePickerReminder;
        private System.Windows.Forms.Button btnAddReminder;
        private System.Windows.Forms.Button btnDeleteReminder;

        private System.Windows.Forms.TabPage tabPageCalendar;
        private System.Windows.Forms.NumericUpDown numericUpDownYear;
        private System.Windows.Forms.NumericUpDown numericUpDownMonth;
        private System.Windows.Forms.Button btnShowCalendar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCalendar;

        private void InitializeComponent()
        {
            this.comboBoxProfiles = new System.Windows.Forms.ComboBox();
            this.btnAddProfile = new System.Windows.Forms.Button();
            this.btnDeleteProfile = new System.Windows.Forms.Button();

            this.tabControlMain = new System.Windows.Forms.TabControl();

            this.tabPageNotes = new System.Windows.Forms.TabPage();
            this.listBoxNotes = new System.Windows.Forms.ListBox();
            this.textBoxNoteText = new System.Windows.Forms.TextBox();
            this.btnAddNote = new System.Windows.Forms.Button();
            this.btnDeleteNote = new System.Windows.Forms.Button();

            this.tabPageReminders = new System.Windows.Forms.TabPage();
            this.listBoxReminders = new System.Windows.Forms.ListBox();
            this.textBoxReminderText = new System.Windows.Forms.TextBox();
            this.dateTimePickerReminder = new System.Windows.Forms.DateTimePicker();
            this.btnAddReminder = new System.Windows.Forms.Button();
            this.btnDeleteReminder = new System.Windows.Forms.Button();

            this.tabPageCalendar = new System.Windows.Forms.TabPage();
            this.numericUpDownYear = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMonth = new System.Windows.Forms.NumericUpDown();
            this.btnShowCalendar = new System.Windows.Forms.Button();
            this.tableLayoutPanelCalendar = new System.Windows.Forms.TableLayoutPanel();

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonth)).BeginInit();

            this.SuspendLayout();

            // comboBoxProfiles
            this.comboBoxProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProfiles.FormattingEnabled = true;
            this.comboBoxProfiles.Location = new System.Drawing.Point(15, 15);
            this.comboBoxProfiles.Name = "comboBoxProfiles";
            this.comboBoxProfiles.Size = new System.Drawing.Size(280, 24);
            this.comboBoxProfiles.TabIndex = 0;
            this.comboBoxProfiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxProfiles_SelectedIndexChanged);
            this.comboBoxProfiles.BackColor = System.Drawing.SystemColors.Window; // системный цвет для полей ввода

            // btnAddProfile
            this.btnAddProfile.Location = new System.Drawing.Point(310, 13);
            this.btnAddProfile.Name = "btnAddProfile";
            this.btnAddProfile.Size = new System.Drawing.Size(140, 28);
            this.btnAddProfile.TabIndex = 1;
            this.btnAddProfile.Text = "Добавить профиль";
            this.btnAddProfile.UseVisualStyleBackColor = true;
            this.btnAddProfile.Click += new System.EventHandler(this.btnAddProfile_Click);

            // btnDeleteProfile
            this.btnDeleteProfile.Location = new System.Drawing.Point(460, 13);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Size = new System.Drawing.Size(140, 28);
            this.btnDeleteProfile.TabIndex = 2;
            this.btnDeleteProfile.Text = "Удалить профиль";
            this.btnDeleteProfile.UseVisualStyleBackColor = true;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);

            // tabControlMain
            this.tabControlMain.Location = new System.Drawing.Point(15, 50);
            this.tabControlMain.Size = new System.Drawing.Size(760, 580);
            this.tabControlMain.TabIndex = 3;

            // tabPageNotes
            this.tabPageNotes.Text = "Заметки";
            this.tabPageNotes.UseVisualStyleBackColor = true;
            this.tabPageNotes.BackColor = System.Drawing.SystemColors.Control; // системный цвет фона

            // listBoxNotes
            this.listBoxNotes.Location = new System.Drawing.Point(10, 10);
            this.listBoxNotes.Size = new System.Drawing.Size(730, 350);
            this.listBoxNotes.TabIndex = 0;
            this.listBoxNotes.BackColor = System.Drawing.SystemColors.Window;

            // textBoxNoteText
            this.textBoxNoteText.Location = new System.Drawing.Point(10, 370);
            this.textBoxNoteText.Multiline = true;
            this.textBoxNoteText.Size = new System.Drawing.Size(730, 90);
            this.textBoxNoteText.TabIndex = 1;
            this.textBoxNoteText.BackColor = System.Drawing.SystemColors.Window;

            // btnAddNote
            this.btnAddNote.Location = new System.Drawing.Point(10, 470);
            this.btnAddNote.Size = new System.Drawing.Size(355, 35);
            this.btnAddNote.Text = "Добавить заметку";
            this.btnAddNote.UseVisualStyleBackColor = true;
            this.btnAddNote.Click += new System.EventHandler(this.btnAddNote_Click);

            // btnDeleteNote
            this.btnDeleteNote.Location = new System.Drawing.Point(385, 470);
            this.btnDeleteNote.Size = new System.Drawing.Size(355, 35);
            this.btnDeleteNote.Text = "Удалить заметку";
            this.btnDeleteNote.UseVisualStyleBackColor = true;
            this.btnDeleteNote.Click += new System.EventHandler(this.btnDeleteNote_Click);

            this.tabPageNotes.Controls.Add(this.listBoxNotes);
            this.tabPageNotes.Controls.Add(this.textBoxNoteText);
            this.tabPageNotes.Controls.Add(this.btnAddNote);
            this.tabPageNotes.Controls.Add(this.btnDeleteNote);

            // tabPageReminders
            this.tabPageReminders.Text = "Напоминания";
            this.tabPageReminders.UseVisualStyleBackColor = true;
            this.tabPageReminders.BackColor = System.Drawing.SystemColors.Control;

            // listBoxReminders
            this.listBoxReminders.Location = new System.Drawing.Point(10, 10);
            this.listBoxReminders.Size = new System.Drawing.Size(730, 350);
            this.listBoxReminders.TabIndex = 0;
            this.listBoxReminders.BackColor = System.Drawing.SystemColors.Window;

            // textBoxReminderText
            this.textBoxReminderText.Location = new System.Drawing.Point(10, 370);
            this.textBoxReminderText.Multiline = true;
            this.textBoxReminderText.Size = new System.Drawing.Size(730, 70);
            this.textBoxReminderText.TabIndex = 1;
            this.textBoxReminderText.BackColor = System.Drawing.SystemColors.Window;

            // dateTimePickerReminder
            this.dateTimePickerReminder.Location = new System.Drawing.Point(10, 450);
            this.dateTimePickerReminder.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerReminder.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePickerReminder.ShowUpDown = true;
            this.dateTimePickerReminder.Size = new System.Drawing.Size(280, 22);
            this.dateTimePickerReminder.TabIndex = 2;

            // btnAddReminder
            this.btnAddReminder.Location = new System.Drawing.Point(300, 445);
            this.btnAddReminder.Size = new System.Drawing.Size(220, 35);
            this.btnAddReminder.Text = "Добавить напоминание";
            this.btnAddReminder.UseVisualStyleBackColor = true;
            this.btnAddReminder.Click += new System.EventHandler(this.btnAddReminder_Click);

            // btnDeleteReminder
            this.btnDeleteReminder.Location = new System.Drawing.Point(530, 445);
            this.btnDeleteReminder.Size = new System.Drawing.Size(210, 35);
            this.btnDeleteReminder.Text = "Удалить напоминание";
            this.btnDeleteReminder.UseVisualStyleBackColor = true;
            this.btnDeleteReminder.Click += new System.EventHandler(this.btnDeleteReminder_Click);

            this.tabPageReminders.Controls.Add(this.listBoxReminders);
            this.tabPageReminders.Controls.Add(this.textBoxReminderText);
            this.tabPageReminders.Controls.Add(this.dateTimePickerReminder);
            this.tabPageReminders.Controls.Add(this.btnAddReminder);
            this.tabPageReminders.Controls.Add(this.btnDeleteReminder);

            // tabPageCalendar
            this.tabPageCalendar.Text = "Календарь";
            this.tabPageCalendar.UseVisualStyleBackColor = true;
            this.tabPageCalendar.BackColor = System.Drawing.SystemColors.Control;

            // numericUpDownYear
            this.numericUpDownYear.Location = new System.Drawing.Point(10, 10);
            this.numericUpDownYear.Minimum = 1900;
            this.numericUpDownYear.Maximum = 2100;
            this.numericUpDownYear.Value = 2024;
            this.numericUpDownYear.Size = new System.Drawing.Size(140, 22);
            this.numericUpDownYear.TabIndex = 0;

            // numericUpDownMonth
            this.numericUpDownMonth.Location = new System.Drawing.Point(160, 10);
            this.numericUpDownMonth.Minimum = 1;
            this.numericUpDownMonth.Maximum = 12;
            this.numericUpDownMonth.Value = 5;
            this.numericUpDownMonth.Size = new System.Drawing.Size(100, 22);
            this.numericUpDownMonth.TabIndex = 1;

            // btnShowCalendar
            this.btnShowCalendar.Location = new System.Drawing.Point(270, 7);
            this.btnShowCalendar.Size = new System.Drawing.Size(150, 28);
            this.btnShowCalendar.Text = "Показать календарь";
            this.btnShowCalendar.UseVisualStyleBackColor = true;
            this.btnShowCalendar.Click += new System.EventHandler(this.btnShowCalendar_Click);

            // tableLayoutPanelCalendar
            this.tableLayoutPanelCalendar.Location = new System.Drawing.Point(10, 45);
            this.tableLayoutPanelCalendar.Size = new System.Drawing.Size(740, 500);
            this.tableLayoutPanelCalendar.ColumnCount = 7;
            this.tableLayoutPanelCalendar.RowCount = 7; // 1 row for headers + up to 6 weeks
            for (int i = 0; i < 7; i++)
                this.tableLayoutPanelCalendar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            for (int i = 0; i < 7; i++)
                this.tableLayoutPanelCalendar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));

            this.tabPageCalendar.Controls.Add(this.numericUpDownYear);
            this.tabPageCalendar.Controls.Add(this.numericUpDownMonth);
            this.tabPageCalendar.Controls.Add(this.btnShowCalendar);
            this.tabPageCalendar.Controls.Add(this.tableLayoutPanelCalendar);

            // Add tabs
            this.tabControlMain.Controls.Add(this.tabPageNotes);
            this.tabControlMain.Controls.Add(this.tabPageReminders);
            this.tabControlMain.Controls.Add(this.tabPageCalendar);

            // MainForm
            this.ClientSize = new System.Drawing.Size(800, 650);
            this.Controls.Add(this.comboBoxProfiles);
            this.Controls.Add(this.btnAddProfile);
            this.Controls.Add(this.btnDeleteProfile);
            this.Controls.Add(this.tabControlMain);

            this.Name = "MainForm";
            this.Text = "CalendarApp";

            // Устанавливаем системный цвет фона формы
            this.BackColor = System.Drawing.SystemColors.Control;

            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMonth)).EndInit();

            this.ResumeLayout(false);
        }
    }
}
