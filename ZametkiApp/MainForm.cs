using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace CalendarApp
{
    public partial class MainForm : Form
    {
        // Классы данных
        private class Profile
        {
            public string Name { get; set; }
            public List<Note> Notes { get; set; } = new List<Note>();
            public List<Reminder> Reminders { get; set; } = new List<Reminder>();
        }

        private class Note
        {
            public string Text { get; set; }
            public override string ToString() => Text.Length > 30 ? Text.Substring(0, 30) + "..." : Text;
        }

        private class Reminder
        {
            public string Text { get; set; }
            public DateTime Time { get; set; }
            public bool IsTriggered { get; set; } = false; // Новое поле, пометка срабатывания

            public override string ToString() => $"{Time:yyyy-MM-dd HH:mm} - {Text}";
        }

        private List<Profile> profiles = new List<Profile>();
        private Profile currentProfile = null;

        private SoundPlayer alertSound;
        private System.Threading.Timer checkReminderTimer;
        private readonly object reminderLock = new object();

        private readonly string saveFilePath;

        public MainForm()
        {
            InitializeComponent();

            // Путь для сохранения данных
            saveFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CalendarApp",
                "profiles.json");

            // Прозрачный фон контролов
            SetupControlsTransparency(this.Controls);

            // Загрузка профилей из файла или создание дефолтного
            LoadProfiles();

            // Загрузка звука alert.wav
            try
            {
                alertSound = new SoundPlayer("alert.wav");
                alertSound.Load();
            }
            catch
            {
                alertSound = null;
            }

            numericUpDownYear.Value = DateTime.Now.Year;
            numericUpDownMonth.Value = DateTime.Now.Month;
            ShowCalendar((int)numericUpDownYear.Value, (int)numericUpDownMonth.Value);

            // Таймер проверки напоминаний каждую секунду
            checkReminderTimer = new System.Threading.Timer(CheckReminders, null, 0, 1000);

            // Подписка на событие отрисовки элементов списка напоминаний
            listBoxReminders.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxReminders.DrawItem += ListBoxReminders_DrawItem;

            // Установка текста профиля на "не найден"
            comboBoxProfiles.Items.Add("не найден");
            comboBoxProfiles.SelectedIndex = 0;
        }

        private void SetupControlsTransparency(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Label || control is Panel)
                {
                    control.BackColor = Color.Transparent;
                }
                else if (control is ListBox || control is TextBox || control is ComboBox)
                {
                    control.BackColor = Color.FromArgb(230, 240, 255);
                }

                if (control.HasChildren)
                    SetupControlsTransparency(control.Controls);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.LightBlue,
                Color.SlateBlue,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void LoadProfiles()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    profiles = JsonSerializer.Deserialize<List<Profile>>(json);
                }
            }
            catch
            {
                profiles = new List<Profile>();
            }

            if (profiles == null || profiles.Count == 0)
            {
                // Создаем профиль с именем "не найден"
                profiles = new List<Profile> { new Profile() { Name = "не найден" } };
            }

            comboBoxProfiles.Items.Clear();
            foreach (var profile in profiles)
            {
                comboBoxProfiles.Items.Add(profile.Name);
            }

            // Устанавливаем выбранный профиль на "не найден"
            comboBoxProfiles.SelectedIndex = 0;
            currentProfile = profiles[0]; // Устанавливаем текущий профиль
            RefreshNotes();
            RefreshReminders();
        }

        private void SaveProfiles()
        {
            try
            {
                string folder = Path.GetDirectoryName(saveFilePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveFilePath, json);
            }
            catch
            {
                // Ошибки записи
            }
        }

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProfiles.SelectedIndex < 0) return;
            currentProfile = profiles[comboBoxProfiles.SelectedIndex];
            RefreshNotes();
            RefreshReminders();
        }

        private void btnAddProfile_Click(object sender, EventArgs e)
        {
            string profileName = Prompt.ShowDialog("Введите имя нового профиля:", "Новый профиль");
            if (string.IsNullOrWhiteSpace(profileName)) return;

            foreach (var p in profiles)
            {
                if (p.Name.Equals(profileName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Профиль с таким именем уже существует.");
                    return;
                }
            }

            var newProfile = new Profile() { Name = profileName };
            profiles.Add(newProfile);
            comboBoxProfiles.Items.Add(profileName);
            comboBoxProfiles.SelectedIndex = comboBoxProfiles.Items.Count - 1;

            SaveProfiles();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;

            if (MessageBox.Show($"Удалить профиль \"{currentProfile.Name}\"?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int idx = comboBoxProfiles.SelectedIndex;
                profiles.RemoveAt(idx);
                comboBoxProfiles.Items.RemoveAt(idx);

                if (comboBoxProfiles.Items.Count > 0)
                {
                    comboBoxProfiles.SelectedIndex = 0;
                    currentProfile = profiles[0];
                }
                else
                {
                    currentProfile = null;
                }

                RefreshNotes();
                RefreshReminders();

                SaveProfiles();
            }
        }

        private void RefreshNotes()
        {
            listBoxNotes.Items.Clear();
            if (currentProfile == null)
            {
                MessageBox.Show("Нельзя добавить заметку, пока не войдете в профиль.");
                return;
            }

            for (int i = 0; i < currentProfile.Notes.Count; i++)
            {
                listBoxNotes.Items.Add($"{i + 1}. {currentProfile.Notes[i]}");
            }
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Нельзя добавить заметку, пока не войдете в профиль.");
                return;
            }

            string text = textBoxNoteText.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Текст заметки не может быть пустым.");
                return;
            }

            currentProfile.Notes.Add(new Note() { Text = text });
            textBoxNoteText.Clear();
            RefreshNotes();

            SaveProfiles();
        }

        private void btnDeleteNote_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;
            if (listBoxNotes.SelectedIndex < 0) return;

            currentProfile.Notes.RemoveAt(listBoxNotes.SelectedIndex);
            RefreshNotes();

            SaveProfiles();
        }

        private void RefreshReminders()
        {
            listBoxReminders.Items.Clear();
            if (currentProfile == null)
            {
                MessageBox.Show("Нельзя добавить напоминание, пока не войдете в профиль.");
                return;
            }

            for (int i = 0; i < currentProfile.Reminders.Count; i++)
            {
                listBoxReminders.Items.Add($"{i + 1}. {currentProfile.Reminders[i]}");
            }
        }

        private void btnAddReminder_Click(object sender, EventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Нельзя добавить напоминание, пока не войдете в профиль.");
                return;
            }

            string text = textBoxReminderText.Text.Trim();
            DateTime time = dateTimePickerReminder.Value;

            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Текст напоминания не может быть пустым.");
                return;
            }

            if (time <= DateTime.Now)
            {
                MessageBox.Show("Время напоминания должно быть в будущем.");
                return;
            }

            currentProfile.Reminders.Add(new Reminder() { Text = text, Time = time });
            textBoxReminderText.Clear();
            RefreshReminders();

            SaveProfiles();
        }

        private void btnDeleteReminder_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;
            if (listBoxReminders.SelectedIndex < 0) return;

            currentProfile.Reminders.RemoveAt(listBoxReminders.SelectedIndex);
            RefreshReminders();

            SaveProfiles();
        }

        // Отрисовка элементов списка с учётом цвета для сработавших
        private void ListBoxReminders_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || currentProfile == null) return;

            e.DrawBackground();

            bool isTriggered = false;

            if (e.Index < currentProfile.Reminders.Count)
            {
                isTriggered = currentProfile.Reminders[e.Index].IsTriggered;
            }

            Color textColor = isTriggered ? Color.Red : e.ForeColor;

            using (SolidBrush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(listBoxReminders.Items[e.Index].ToString(),
                    e.Font,
                    brush,
                    e.Bounds,
                    StringFormat.GenericDefault);
            }

            e.DrawFocusRectangle();
        }

        // Проверка напоминаний — помечаем сработавшие, не удаляем
        private void CheckReminders(object state)
        {
            if (currentProfile == null) return;

            List<Reminder> remindersToTrigger = new List<Reminder>();

            lock (reminderLock)
            {
                DateTime now = DateTime.Now;

                foreach (var reminder in currentProfile.Reminders)
                {
                    if (!reminder.IsTriggered && reminder.Time <= now)
                    {
                        remindersToTrigger.Add(reminder);
                    }
                }

                foreach (var reminder in remindersToTrigger)
                {
                    reminder.IsTriggered = true; // Пометка срабатывания
                }
            }

            if (remindersToTrigger.Count > 0)
            {
                this.Invoke(new Action(() =>
                {
                    RefreshReminders();
                    SaveProfiles();

                    foreach (var reminder in remindersToTrigger)
                    {
                        if (alertSound != null)
                        {
                            try { alertSound.Play(); }
                            catch { }
                        }

                        MessageBox.Show(reminder.Text, $"Напоминание {reminder.Time:yyyy-MM-dd HH:mm}", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }));
            }
        }

        private void btnShowCalendar_Click(object sender, EventArgs e)
        {
            ShowCalendar((int)numericUpDownYear.Value, (int)numericUpDownMonth.Value);
        }

        private void ShowCalendar(int year, int month)
        {
            tableLayoutPanelCalendar.Controls.Clear();

            string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            for (int i = 0; i < 7; i++)
            {
                var label = new Label
                {
                    Text = dayNames[i],
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.DarkBlue
                };
                tableLayoutPanelCalendar.Controls.Add(label, i, 0);
            }

            DateTime firstDay = new DateTime(year, month, 1);
            int startDay = ((int)firstDay.DayOfWeek + 6) % 7; // Понедельник = 0
            int daysInMonth = DateTime.DaysInMonth(year, month);

            int row = 1, col = startDay;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var dayLabel = new Label
                {
                    Text = day.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(1),
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.LightYellow
                };

                tableLayoutPanelCalendar.Controls.Add(dayLabel, col, row);

                col++;
                if (col >= 7)
                {
                    col = 0;
                    row++;
                }
            }
        }
    }

    // Вспомогательный класс для ввода текста (Prompt)
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 10, Top = 10, Text = text, Width = 360 };
            TextBox textBox = new TextBox() { Left = 10, Top = 40, Width = 360 };
            Button confirmation = new Button() { Text = "ОК", Left = 300, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
