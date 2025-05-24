using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CalendarApp
{
    public class Day
    {
        public DateTime Date { get; }
        public Day(int year, int month, int day)
        {
            try { Date = new DateTime(year, month, day); }
            catch (ArgumentOutOfRangeException e)
            { throw new ArgumentException($"Неверная дата: {e.Message}"); }
        }
        public string Weekday => Date.ToString("dddd", new CultureInfo("ru-RU"));
        public bool IsWeekend => Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        public string MonthName => Date.ToString("MMMM", new CultureInfo("ru-RU"));
        public int DayOfYear => Date.DayOfYear;
    }

    public class CalendarParser
    {
        private readonly DateTime _start, _end;
        public CalendarParser(DateTime start, DateTime end)
        {
            if (start > end) throw new ArgumentException("Начальная дата должна быть раньше конечной");
            _start = start; _end = end;
        }
        public IEnumerable<Day> GetMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int d = 1; d <= days; d++)
                yield return new Day(year, month, d);
        }
        public IEnumerable<Day> IterateDays()
        {
            for (var cur = _start; cur <= _end; cur = cur.AddDays(1))
                yield return new Day(cur.Year, cur.Month, cur.Day);
        }
    }

    public class Note
    {
        public DateTime At { get; set; } = DateTime.Now;
        public string Text { get; set; }
        public bool Triggered { get; set; } = false;
    }

    public class Profile
    {
        public string Name { get; set; }
        public List<Note> Reminders { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
    }

    class Program
    {
        const string ProfilesDir = "profiles";
        static List<Profile> profiles = new();
        static Profile current;

        static void Main()
        {
            LoadProfiles();
            if (!profiles.Any())
            {
                Console.Write("Создайте имя профиля: ");
                current = new Profile { Name = Console.ReadLine() ?? "Default" };
                profiles.Add(current);
                SaveProfile(current);
            }
            else
            {
                current = profiles[0];
            }

            while (true)
            {
                CheckReminders();
                Console.WriteLine($"\nТекущий профиль: {current.Name}");
                Console.WriteLine("1. Вывести календарь (месяц)");
                Console.WriteLine("2. Работа с напоминаниями");
                Console.WriteLine("3. Работа с заметками");
                Console.WriteLine("4. Профили");
                Console.WriteLine("5. Выход");
                Console.Write("Выберите пункт: ");
                var opt = Console.ReadLine();
                switch (opt)
                {
                    case "1": ShowCalendar(); break;
                    case "2": TestReminders(); break;
                    case "3": TestNotesFunc(); break;
                    case "4": ManageProfiles(); break;
                    case "5": return;
                    default: Console.WriteLine("Неверный выбор"); break;
                }
            }
        }

        static void LoadProfiles()
        {
            if (!Directory.Exists(ProfilesDir))
                Directory.CreateDirectory(ProfilesDir);
            profiles.Clear();
            foreach (var file in Directory.GetFiles(ProfilesDir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var p = JsonSerializer.Deserialize<Profile>(json);
                    if (p != null)
                        profiles.Add(p);
                }
                catch { }
            }
        }

        static void SaveProfile(Profile p)
        {
            if (!Directory.Exists(ProfilesDir))
                Directory.CreateDirectory(ProfilesDir);
            var safeName = string.Join("_",
                p.Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            var path = Path.Combine(ProfilesDir, safeName + ".json");
            var json = JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        static void CheckReminders()
        {
            var now = DateTime.Now;
            var toTrigger = current.Reminders.Where(r => !r.Triggered && r.At <= now).ToList();
            foreach (var r in toTrigger)
            {
                try { Console.Beep(800, 500); } catch { }
                Console.WriteLine($"НАПОМИНАНИЕ: [{r.At:yyyy-MM-dd HH:mm}] {r.Text}");
                r.Triggered = true;
                SaveProfile(current);
            }
        }

        static void ShowCalendar()
        {
            Console.Write("Год: "); int y = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Месяц (1-12): "); int m = int.Parse(Console.ReadLine() ?? "0");
            var parser = new CalendarParser(new DateTime(y, 1, 1), new DateTime(y, 12, 31));
            Console.WriteLine($"\n   {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)} {y}");
            Console.WriteLine("Пн Вт Ср Чт Пт Сб Вс");
            int firstDay = (int)new DateTime(y, m, 1).DayOfWeek;
            if (firstDay == 0) firstDay = 7;
            for (int i = 1; i < firstDay; i++) Console.Write("   ");
            foreach (var d in parser.GetMonth(y, m))
            {
                Console.Write($"{d.Date.Day,2} ");
                if ((int)d.Date.DayOfWeek == 0) Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void TestReminders()
        {
            Console.WriteLine("1. Показать напоминания\n2. Добавить напоминание\n3. Удалить напоминание");
            var choose = Console.ReadLine();
            if (choose == "1") ShowNotes(current.Reminders);
            else if (choose == "2")
            {
                Console.Write("Дата и время (yyyy-MM-dd HH:mm): ");
                var str = Console.ReadLine();
                Console.Write("Сообщение: ");
                var msg = Console.ReadLine();
                if (DateTime.TryParseExact(str, "yyyy-MM-dd HH:mm", null,
                    DateTimeStyles.None, out var dt))
                {
                    current.Reminders.Add(new Note { At = dt, Text = msg });
                    Console.WriteLine("Напоминание добавлено.");
                    SaveProfile(current);
                }
                else Console.WriteLine("Неверный формат даты.");
            }
            else if (choose == "3")
            {
                ShowNotes(current.Reminders);
                Console.Write("Индекс для удаления: ");
                if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= current.Reminders.Count)
                {
                    current.Reminders.RemoveAt(idx - 1);
                    Console.WriteLine("Удалено.");
                    SaveProfile(current);
                }
                else Console.WriteLine("Неверный индекс.");
            }
        }

        static void TestNotesFunc()
        {
            Console.WriteLine("1. Показать заметки\n2. Добавить заметку\n3. Удалить заметку\n4. Изменить заметку");
            var choose = Console.ReadLine();
            if (choose == "1") ShowNotes(current.Notes);
            else if (choose == "2")
            {
                Console.Write("Текст заметки: ");
                var txt = Console.ReadLine();
                current.Notes.Add(new Note { Text = txt });
                Console.WriteLine("Заметка добавлена.");
                SaveProfile(current);
            }
            else if (choose == "3")
            {
                ShowNotes(current.Notes);
                Console.Write("Индекс для удаления: ");
                if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= current.Notes.Count)
                {
                    current.Notes.RemoveAt(idx - 1);
                    Console.WriteLine("Удалено.");
                    SaveProfile(current);
                }
                else Console.WriteLine("Неверный индекс.");
            }
            else if (choose == "4")
            {
                ShowNotes(current.Notes);
                Console.Write("Индекс для редактирования: ");
                if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= current.Notes.Count)
                {
                    Console.Write("Новый текст: ");
                    current.Notes[idx - 1].Text = Console.ReadLine();
                    current.Notes[idx - 1].At = DateTime.Now;
                    Console.WriteLine("Изменено.");
                    SaveProfile(current);
                }
                else Console.WriteLine("Неверный индекс.");
            }
        }

        static void ShowNotes(List<Note> list)
        {
            Console.WriteLine();
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1}. {list[i].At:yyyy-MM-dd HH:mm} - {list[i].Text}");
        }

        static void ManageProfiles()
        {
            Console.WriteLine("Профили:");
            for (int i = 0; i < profiles.Count; i++)
                Console.WriteLine($"{i + 1}. {profiles[i].Name}");
            Console.WriteLine("a - добавить, s - выбрать, d - удалить");
            var cmd = Console.ReadLine();
            if (cmd == "a")
            {
                Console.Write("Имя нового профиля: ");
                var nm = Console.ReadLine();
                var p = new Profile { Name = nm };
                profiles.Add(p);
                current = p;
                SaveProfile(p);
                Console.WriteLine("Профиль добавлен.");
            }
            else if (cmd == "s")
            {
                Console.Write("Номер профиля: ");
                if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= profiles.Count)
                {
                    current = profiles[idx - 1];
                    Console.WriteLine($"Выбран профиль: {current.Name}");
                }
                else Console.WriteLine("Неверный номер.");
            }
            else if (cmd == "d")
            {
                Console.Write("Номер для удаления: ");
                if (int.TryParse(Console.ReadLine(), out var idx) && idx > 0 && idx <= profiles.Count)
                {
                    var toRem = profiles[idx - 1];
                    profiles.RemoveAt(idx - 1);
                    var safe = string.Join("_", toRem.Name
                        .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
                    var path = Path.Combine(ProfilesDir, safe + ".json");
                    if (File.Exists(path)) File.Delete(path);
                    current = profiles.FirstOrDefault() ?? toRem;
                    Console.WriteLine($"Профиль «{toRem.Name}» удалён.");
                }
                else Console.WriteLine("Неверный номер.");
            }
        }
    }
}
