#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using ThreadingTimer = System.Threading.Timer;

namespace CalendarApp
{
    public partial class MainForm : Form
    {
        // --- Модели данных ---
        private class Profile
        {
            public string Name { get; set; } = string.Empty;
            public List<Note> Notes { get; set; } = new();
            public List<Reminder> Reminders { get; set; } = new();
        }

        private class Note
        {
            public string Text { get; set; } = string.Empty;
            public override string ToString() =>
                Text.Length > 30 ? Text.Substring(0, 30) + "…" : Text;
        }

        private class Reminder
        {
            public string Text { get; set; } = string.Empty;
            public DateTime Time { get; set; }
            public bool IsTriggered { get; set; } = false;
            public override string ToString() => $"{Time:yyyy-MM-dd HH:mm} – {Text}";
        }

        // --- Поля формы ---
        private readonly List<Profile> profiles = new();
        private Profile? currentProfile;

        private SoundPlayer? alertSound;
        private ThreadingTimer? checkReminderTimer;
        private readonly object reminderLock = new();

        public MainForm()
        {
            InitializeComponent();
            // Инициализация после Designer
            SetupControlsTransparency(Controls);
            LoadProfiles();

            // Попытка загрузить звук
            try { alertSound = new SoundPlayer("alert.wav"); alertSound.Load(); }
            catch { alertSound = null; }

            // Календарь: год и месяц
            numericUpDownYear.Value = DateTime.Today.Year;
            comboBoxMonth.SelectedIndex = DateTime.Today.Month - 1;
            ShowCalendar((int)numericUpDownYear.Value, comboBoxMonth.SelectedIndex + 1);

            // Таймер напоминаний
            checkReminderTimer = new ThreadingTimer(CheckReminders, null, 0, 1000);

            // Подсветка напоминаний
            listBoxReminders.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxReminders.DrawItem += ListBoxReminders_DrawItem;

            // Смена вкладок
            tabControlMain.SelectedIndexChanged += TabControlMain_SelectedIndexChanged;
            TabControlMain_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private string ProfilesFolder =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CalendarApp",
                "Profiles"
            );

        private void LoadProfiles()
        {
            var dir = ProfilesFolder;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            profiles.Clear();
            var files = Directory.GetFiles(dir, "*.json");
            if (files.Length == 0)
            {
                string name;
                do
                {
                    name = Prompt.ShowDialog(
                        "Профилей не найдено. Введите имя нового профиля:",
                        "Создать профиль"
                    ).Trim();
                } while (string.IsNullOrEmpty(name));

                profiles.Add(new Profile { Name = name });
                SaveProfiles();
            }
            else
            {
                foreach (var f in files)
                {
                    try
                    {
                        var p = JsonSerializer.Deserialize<Profile>(File.ReadAllText(f));
                        if (p != null) profiles.Add(p);
                    }
                    catch { }
                }
            }

            comboBoxProfiles.Items.Clear();
            foreach (var p in profiles) comboBoxProfiles.Items.Add(p.Name);

            comboBoxProfiles.SelectedIndex = 0;
            currentProfile = profiles[0];
            RefreshNotes();
            RefreshReminders();
        }

        private void SaveProfiles()
        {
            var dir = ProfilesFolder;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var live = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in profiles)
            {
                var safe = SanitizeFileName(p.Name) + ".json";
                var path = Path.Combine(dir, safe);
                live.Add(path);
                File.WriteAllText(path,
                    JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true }));
            }

            foreach (var f in Directory.GetFiles(dir, "*.json"))
                if (!live.Contains(f))
                    File.Delete(f);
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private void SetupControlsTransparency(Control.ControlCollection ctrls)
        {
            foreach (Control ctl in ctrls)
            {
                if (ctl is Label or Panel) ctl.BackColor = Color.Transparent;
                else if (ctl is ListBox or TextBox or ComboBox)
                    ctl.BackColor = Color.FromArgb(230, 240, 255);

                if (ctl.HasChildren) SetupControlsTransparency(ctl.Controls);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(
                ClientRectangle,
                Color.LightBlue,
                Color.SlateBlue,
                LinearGradientMode.Vertical
            );
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idx = comboBoxProfiles.SelectedIndex;
            if (idx < 0 || idx >= profiles.Count) return;
            currentProfile = profiles[idx];
            RefreshNotes();
            RefreshReminders();
        }

        private void btnAddProfile_Click(object sender, EventArgs e)
        {
            var name = Prompt.ShowDialog("Введите имя нового профиля:", "Новый профиль").Trim();
            if (string.IsNullOrEmpty(name)) return;
            if (profiles.Exists(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Профиль с таким именем уже существует.");
                return;
            }
            profiles.Add(new Profile { Name = name });
            comboBoxProfiles.Items.Add(name);
            comboBoxProfiles.SelectedIndex = comboBoxProfiles.Items.Count - 1;
            SaveProfiles();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (currentProfile == null) return;
            if (MessageBox.Show(
                    $"Удалить профиль «{currentProfile.Name}»?",
                    "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var idx = comboBoxProfiles.SelectedIndex;
                profiles.RemoveAt(idx);
                comboBoxProfiles.Items.RemoveAt(idx);
                if (profiles.Count > 0)
                {
                    comboBoxProfiles.SelectedIndex = 0;
                    currentProfile = profiles[0];
                }
                else { LoadProfiles(); return; }
                RefreshNotes();
                RefreshReminders();
                SaveProfiles();
            }
        }

        private void RefreshNotes()
        {
            listBoxNotes.Items.Clear();
            if (currentProfile == null) return;
            for (int i = 0; i < currentProfile.Notes.Count; i++)
                listBoxNotes.Items.Add($"{i + 1}. {currentProfile.Notes[i]}");
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Сначала выберите или создайте профиль.");
                return;
            }
            var txt = textBoxNoteText.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                MessageBox.Show("Текст заметки не может быть пустым.");
                return;
            }
            currentProfile.Notes.Add(new Note { Text = txt });
            textBoxNoteText.Clear();
            RefreshNotes();
            SaveProfiles();
        }

        private void btnDeleteNote_Click(object sender, EventArgs e)
        {
            if (currentProfile == null || listBoxNotes.SelectedIndex < 0) return;
            currentProfile.Notes.RemoveAt(listBoxNotes.SelectedIndex);
            RefreshNotes();
            SaveProfiles();
        }

        private void RefreshReminders()
        {
            listBoxReminders.Items.Clear();
            if (currentProfile == null) return;
            for (int i = 0; i < currentProfile.Reminders.Count; i++)
                listBoxReminders.Items.Add($"{i + 1}. {currentProfile.Reminders[i]}");
        }

        private void btnAddReminder_Click(object sender, EventArgs e)
        {
            if (currentProfile == null)
            {
                MessageBox.Show("Сначала выберите или создайте профиль.");
                return;
            }
            var txt = textBoxReminderText.Text.Trim();
            var tm = dateTimePickerReminder.Value;
            if (string.IsNullOrEmpty(txt))
            {
                MessageBox.Show("Текст напоминания не может быть пустым.");
                return;
            }
            if (tm <= DateTime.Now)
            {
                MessageBox.Show("Время напоминания должно быть в будущем.");
                return;
            }
            currentProfile.Reminders.Add(new Reminder { Text = txt, Time = tm });
            textBoxReminderText.Clear();
            RefreshReminders();
            SaveProfiles();
        }

        private void btnDeleteReminder_Click(object sender, EventArgs e)
        {
            if (currentProfile == null || listBoxReminders.SelectedIndex < 0) return;
            currentProfile.Reminders.RemoveAt(listBoxReminders.SelectedIndex);
            RefreshReminders();
            SaveProfiles();
        }

        private void ListBoxReminders_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || currentProfile == null) return;
            e.DrawBackground();
            bool trig = currentProfile.Reminders[e.Index].IsTriggered;
            var clr = trig ? Color.Red : e.ForeColor;
            var txt = listBoxReminders.Items[e.Index]?.ToString() ?? "";
            using var br = new SolidBrush(clr);
            var fnt = e.Font ?? Font;
            e.Graphics.DrawString(txt, fnt, br, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void CheckReminders(object? state)
        {
            if (currentProfile == null) return;
            var toTrig = new List<Reminder>();
            lock (reminderLock)
            {
                var now = DateTime.Now;
                foreach (var r in currentProfile.Reminders)
                    if (!r.IsTriggered && r.Time <= now) toTrig.Add(r);
                foreach (var r in toTrig) r.IsTriggered = true;
            }
            if (toTrig.Count == 0) return;
            Invoke(() =>
            {
                RefreshReminders();
                SaveProfiles();
                foreach (var r in toTrig)
                {
                    alertSound?.Play();
                    MessageBox.Show(
                        r.Text,
                        $"Напоминание {r.Time:yyyy-MM-dd HH:mm}",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            });
        }

        private void btnShowCalendar_Click(object sender, EventArgs e)
        {
            ShowCalendar((int)numericUpDownYear.Value, comboBoxMonth.SelectedIndex + 1);
        }

        private void ShowCalendar(int year, int month)
        {
            tableLayoutPanelCalendar.Controls.Clear();
            string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            for (int i = 0; i < 7; i++)
            {
                var hdr = new Label
                {
                    Text = dayNames[i],
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.DarkBlue
                };
                tableLayoutPanelCalendar.Controls.Add(hdr, i, 0);
            }

            var first = new DateTime(year, month, 1);
            int startCol = ((int)first.DayOfWeek + 6) % 7;
            int daysCount = DateTime.DaysInMonth(year, month);
            int row = 1, col = startCol;
            for (int d = 1; d <= daysCount; d++)
            {
                var lbl = new Label
                {
                    Text = d.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(1),
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.LightYellow
                };
                // подсветка сегодняшнего дня
                if (year == DateTime.Today.Year && month == DateTime.Today.Month && d == DateTime.Today.Day)
                {
                    lbl.BackColor = Color.Orange;
                    lbl.Font = new Font(lbl.Font, FontStyle.Bold);
                }
                tableLayoutPanelCalendar.Controls.Add(lbl, col, row);
                if (++col >= 7) { col = 0; row++; }
            }
        }

        private void PanelCalendarHeader_Resize(object? sender, EventArgs e)
        {
            // центрируем
            int totalW = numericUpDownYear.Width + comboBoxMonth.Width + btnShowCalendar.Width + 20;
            int startX = (panelCalendarHeader.ClientSize.Width - totalW) / 2;
            int y = (panelCalendarHeader.ClientSize.Height - numericUpDownYear.Height) / 2;
            numericUpDownYear.Location = new Point(startX, y);
            comboBoxMonth.Location = new Point(numericUpDownYear.Right + 10, y);
            btnShowCalendar.Location = new Point(comboBoxMonth.Right + 10, y);
        }

        private void TabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isCal = tabControlMain.SelectedTab == tabPageCalendar;
            tableLayoutPanelActions.Visible = !isCal;

            if (tabControlMain.SelectedTab == tabPageNotes)
            {
                buttonAdd.Text = "Добавить заметку";
                buttonDelete.Text = "Удалить заметку";
            }
            else if (tabControlMain.SelectedTab == tabPageReminders)
            {
                buttonAdd.Text = "Добавить напоминание";
                buttonDelete.Text = "Удалить напоминание";
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageNotes)
                btnAddNote_Click(sender, e);
            else if (tabControlMain.SelectedTab == tabPageReminders)
                btnAddReminder_Click(sender, e);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageNotes)
                btnDeleteNote_Click(sender, e);
            else if (tabControlMain.SelectedTab == tabPageReminders)
                btnDeleteReminder_Click(sender, e);
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            using var prompt = new Form
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent
            };
            var lbl = new Label { Left = 10, Top = 10, Text = text, Width = 360 };
            var tb = new TextBox { Left = 10, Top = 40, Width = 360 };
            var btn = new Button { Text = "ОК", Left = 300, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            btn.Click += (_, _) => prompt.Close();

            prompt.Controls.Add(lbl);
            prompt.Controls.Add(tb);
            prompt.Controls.Add(btn);
            prompt.AcceptButton = btn;

            return prompt.ShowDialog() == DialogResult.OK ? tb.Text : string.Empty;
        }
    }
}
