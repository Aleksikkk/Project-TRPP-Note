namespace CalendarApp
{
    partial class MainForm
    {
        private ComboBox comboBoxProfiles;
        private Button btnAddProfile, btnDeleteProfile;
        private TabControl tabControlMain;

        private TabPage tabPageNotes;
        private Panel panelNotesList, panelNoteInput;
        private ListBox listBoxNotes;
        private Label labelNoteInput;
        private TextBox textBoxNoteText;

        private TabPage tabPageReminders;
        private Panel panelRemindersList, panelReminderInput;
        private ListBox listBoxReminders;
        private Label labelReminderInput;
        private DateTimePicker dateTimePickerReminder;
        private TextBox textBoxReminderText;

        private TabPage tabPageCalendar;
        private Panel panelCalendarHeader;
        private NumericUpDown numericUpDownYear;
        private ComboBox comboBoxMonth;
        private Button btnShowCalendar;
        private TableLayoutPanel tableLayoutPanelCalendar;

        private TableLayoutPanel tableLayoutPanelActions;
        private Button buttonAdd, buttonDelete;

        private void InitializeComponent()
        {
            comboBoxProfiles = new ComboBox();
            btnAddProfile = new Button();
            btnDeleteProfile = new Button();
            tabControlMain = new TabControl();

            // Notes
            tabPageNotes = new TabPage();
            panelNotesList = new Panel();
            listBoxNotes = new ListBox();
            panelNoteInput = new Panel();
            labelNoteInput = new Label();
            textBoxNoteText = new TextBox();

            // Reminders
            tabPageReminders = new TabPage();
            panelRemindersList = new Panel();
            listBoxReminders = new ListBox();
            panelReminderInput = new Panel();
            labelReminderInput = new Label();
            dateTimePickerReminder = new DateTimePicker();
            textBoxReminderText = new TextBox();

            // Calendar
            tabPageCalendar = new TabPage();
            panelCalendarHeader = new Panel();
            numericUpDownYear = new NumericUpDown();
            comboBoxMonth = new ComboBox();
            btnShowCalendar = new Button();
            tableLayoutPanelCalendar = new TableLayoutPanel();

            // Actions
            tableLayoutPanelActions = new TableLayoutPanel();
            buttonAdd = new Button();
            buttonDelete = new Button();

            ((System.ComponentModel.ISupportInitialize)numericUpDownYear).BeginInit();
            SuspendLayout();

            // comboBoxProfiles
            comboBoxProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProfiles.Location = new Point(15, 15);
            comboBoxProfiles.Size = new Size(280, 24);
            comboBoxProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxProfiles.SelectedIndexChanged += comboBoxProfiles_SelectedIndexChanged;

            // btnAddProfile
            btnAddProfile.Text = "Добавить профиль";
            btnAddProfile.Location = new Point(310, 13);
            btnAddProfile.Size = new Size(140, 28);
            btnAddProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddProfile.Click += btnAddProfile_Click;

            // btnDeleteProfile
            btnDeleteProfile.Text = "Удалить профиль";
            btnDeleteProfile.Location = new Point(460, 13);
            btnDeleteProfile.Size = new Size(140, 28);
            btnDeleteProfile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteProfile.Click += btnDeleteProfile_Click;

            // tabControlMain
            tabControlMain.Location = new Point(15, 50);
            tabControlMain.Size = new Size(760, 550);
            tabControlMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // ==== Notes Tab ====
            tabPageNotes.Text = "Заметки";
            panelNotesList.Dock = DockStyle.Fill;
            panelNotesList.Controls.Add(listBoxNotes);
            listBoxNotes.Dock = DockStyle.Fill;
            listBoxNotes.BackColor = SystemColors.Window;

            panelNoteInput.Dock = DockStyle.Bottom;
            panelNoteInput.Height = 140; // чуть больше для метки
            panelNoteInput.Padding = new Padding(10, 5, 10, 5);
            panelNoteInput.Controls.Add(textBoxNoteText);
            panelNoteInput.Controls.Add(labelNoteInput);

            labelNoteInput.Text = "Поле ввода";
            labelNoteInput.Dock = DockStyle.Top;
            labelNoteInput.Height = 20;
            labelNoteInput.TextAlign = ContentAlignment.MiddleLeft;
            labelNoteInput.Margin = new Padding(0, 0, 0, 5);

            textBoxNoteText.Dock = DockStyle.Fill;
            textBoxNoteText.Multiline = true;
            textBoxNoteText.BackColor = SystemColors.Window;

            tabPageNotes.Controls.Add(panelNotesList);
            tabPageNotes.Controls.Add(panelNoteInput);

            // ==== Reminders Tab ====
            tabPageReminders.Text = "Напоминания";
            panelRemindersList.Dock = DockStyle.Fill;
            panelRemindersList.Padding = new Padding(0, 0, 0, 20);
            panelRemindersList.Controls.Add(listBoxReminders);
            listBoxReminders.Dock = DockStyle.Fill;
            listBoxReminders.BackColor = SystemColors.Window;

            panelReminderInput.Dock = DockStyle.Bottom;
            panelReminderInput.Height = 220; // увеличено для метки и отступов
            panelReminderInput.Padding = new Padding(10, 5, 10, 5);
            panelReminderInput.Controls.Clear();
            // Добавляем в порядке: текстовое поле, затем picker, затем метку сверху
            panelReminderInput.Controls.Clear();
            // Сначала текст, затем метка, затем выбор даты (док-топ)
            panelReminderInput.Controls.Add(textBoxReminderText);
            panelReminderInput.Controls.Add(labelReminderInput);
            panelReminderInput.Controls.Add(dateTimePickerReminder);

            labelReminderInput.Text = "Поле ввода";
            labelReminderInput.Dock = DockStyle.Top;
            labelReminderInput.Height = 20;
            labelReminderInput.TextAlign = ContentAlignment.MiddleLeft;
            labelReminderInput.Margin = new Padding(0, 0, 0, 10);

            // Настройка DateTimePicker сверху
            dateTimePickerReminder.Format = DateTimePickerFormat.Custom;
            dateTimePickerReminder.CustomFormat = "yyyy-MM-dd HH:mm";
            dateTimePickerReminder.ShowUpDown = true;
            dateTimePickerReminder.Dock = DockStyle.Top;
            dateTimePickerReminder.Margin = new Padding(0, 0, 0, 10);

            textBoxReminderText.Dock = DockStyle.Fill;
            textBoxReminderText.Multiline = true;
            textBoxReminderText.BackColor = SystemColors.Window;

            tabPageReminders.Controls.Add(panelRemindersList);
            tabPageReminders.Controls.Add(panelReminderInput);

            // ==== Calendar Tab ====
            tabPageCalendar.Text = "Календарь";
            panelCalendarHeader.Dock = DockStyle.Top;
            panelCalendarHeader.Height = 50;
            panelCalendarHeader.Padding = new Padding(10);
            panelCalendarHeader.Controls.Add(numericUpDownYear);
            panelCalendarHeader.Controls.Add(comboBoxMonth);
            panelCalendarHeader.Controls.Add(btnShowCalendar);
            panelCalendarHeader.Resize += PanelCalendarHeader_Resize;

            numericUpDownYear.Minimum = 1900;
            numericUpDownYear.Maximum = 2100;
            numericUpDownYear.Value = DateTime.Today.Year;
            numericUpDownYear.Size = new Size(80, 22);
            numericUpDownYear.Anchor = AnchorStyles.None;

            comboBoxMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMonth.Items.AddRange(new object[]
            {
                "Январь","Февраль","Март","Апрель","Май","Июнь",
                "Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь"
            });
            comboBoxMonth.SelectedIndex = DateTime.Today.Month - 1;
            comboBoxMonth.Size = new Size(120, 24);
            comboBoxMonth.Anchor = AnchorStyles.None;

            btnShowCalendar.Text = "Показать календарь";
            btnShowCalendar.Size = new Size(140, 24);
            btnShowCalendar.Anchor = AnchorStyles.None;
            btnShowCalendar.Click += btnShowCalendar_Click;

            tableLayoutPanelCalendar.Dock = DockStyle.Fill;
            tableLayoutPanelCalendar.ColumnCount = 7;
            tableLayoutPanelCalendar.RowCount = 7;
            for (int i = 0; i < 7; i++)
                tableLayoutPanelCalendar.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 14.2857F));
            for (int i = 0; i < 7; i++)
                tableLayoutPanelCalendar.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 14.2857F));

            tabPageCalendar.Controls.Add(tableLayoutPanelCalendar);
            tabPageCalendar.Controls.Add(panelCalendarHeader);

            // ==== Actions panel ====
            tableLayoutPanelActions.ColumnCount = 2;
            tableLayoutPanelActions.RowCount = 1;
            tableLayoutPanelActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelActions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelActions.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            tableLayoutPanelActions.Dock = DockStyle.Bottom;
            tableLayoutPanelActions.Height = 45;
            tableLayoutPanelActions.Controls.Add(buttonAdd, 0, 0);
            tableLayoutPanelActions.Controls.Add(buttonDelete, 1, 0);

            buttonAdd.Text = "Добавить заметку";
            buttonAdd.Dock = DockStyle.Fill;
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Text = "Удалить заметку";
            buttonDelete.Dock = DockStyle.Fill;
            buttonDelete.Click += buttonDelete_Click;

            // Final assembly
            tabControlMain.Controls.Add(tabPageNotes);
            tabControlMain.Controls.Add(tabPageReminders);
            tabControlMain.Controls.Add(tabPageCalendar);

            Controls.Add(tableLayoutPanelActions);
            Controls.Add(tabControlMain);
            Controls.Add(btnDeleteProfile);
            Controls.Add(btnAddProfile);
            Controls.Add(comboBoxProfiles);

            ClientSize = new Size(800, 650);
            Name = "MainForm";
            Text = "CalendarApp";
            BackColor = SystemColors.Control;

            ((System.ComponentModel.ISupportInitialize)numericUpDownYear).EndInit();
            ResumeLayout(false);
        }
    }
}
