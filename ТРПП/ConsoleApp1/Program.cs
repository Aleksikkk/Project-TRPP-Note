using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

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
        public IEnumerable<Day> IterateDays() { for (var cur = _start; cur <= _end; cur = cur.AddDays(1)) yield return new Day(cur.Year, cur.Month, cur.Day); }
    }

    // Напоминание
    public class Reminder
    {
        public DateTime At { get; set; }
        public string Message { get; set; }
        public bool Triggered { get; set; } = false;
    }

    // Заметка
    public class Note
    {
        public DateTime Created { get; } = DateTime.Now;
        public string Text { get; set; }
    }

    // Профиль
    public class Profile
    {
        public string Name { get; set; }
        public List<Reminder> Reminders { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
    }

    class Program
    {
        static List<Profile> profiles = new();
        static Profile current;
        static DateTime? fakeNow = null;

        static DateTime Now => fakeNow ?? DateTime.Now;

        static void Main()
        {
            LoadProfiles();
            if (!profiles.Any()) { current = new Profile { Name = "Default" }; profiles.Add(current); }
            else current = profiles[0];

            while (true)
            {
                CheckReminders();

                Console.WriteLine($"\nТекущий профиль: {current.Name}");
                Console.WriteLine("1. Вывести календарь (месяц)");
                Console.WriteLine("2. Добавить напоминание");
                Console.WriteLine("3. Добавить заметку");
                Console.WriteLine("4. Показать заметки");
                Console.WriteLine("5. Управление профилями");
                Console.WriteLine("6. Эмуляция времени");
                Console.WriteLine("7. Выход");
                Console.Write("Выберите пункт: "); var opt = Console.ReadLine();

                switch (opt)
                {
                    case "1": ShowCalendar(); break;
                    case "2": AddReminder(); break;
                    case "3": AddNote(); break;
                    case "4": ShowNotes(); break;
                    case "5": ManageProfiles(); break;
                    case "6": EmulateTime(); break;
                    case "7": SaveProfiles(); return;
                    default: Console.WriteLine("Неверный выбор"); break;
                }
            }
        }

        static void CheckReminders()
        {
            var now = Now;
            var toTrigger = current.Reminders.Where(r => !r.Triggered && r.At <= now).ToList();
            foreach (var r in toTrigger)
            {
                // Бипка(звуковой сигнал)
                try { Console.Beep(800, 500); } catch { /* ничего */ }
                Console.WriteLine($" НАПОМИНАНИЕ: [{r.At:yyyy-MM-dd HH:mm}] {r.Message}");
                r.Triggered = true;
            }
        }

        static void ShowCalendar()
        {
            Console.Write("Год: "); int y = int.Parse(Console.ReadLine() ?? "");
            Console.Write("Месяц (1-12): "); int m = int.Parse(Console.ReadLine() ?? "");
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

        static void AddReminder()
        {
            Console.Write("Дата и время (yyyy-MM-dd HH:mm): "); var str = Console.ReadLine();
            Console.Write("Сообщение: "); var msg = Console.ReadLine();
            if (DateTime.TryParseExact(str, "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out var dt))
            {
                current.Reminders.Add(new Reminder { At = dt, Message = msg });
                Console.WriteLine("Напоминание добавлено.");
            }
            else Console.WriteLine("Неверный формат даты.");
        }

        static void AddNote()
        {
            Console.Write("Текст заметки: "); var txt = Console.ReadLine();
            current.Notes.Add(new Note { Text = txt });
            Console.WriteLine("Заметка сохранена.");
        }

        static void ShowNotes()
        {
            Console.WriteLine("\nТекущие заметки:");
            foreach (var note in current.Notes)
                Console.WriteLine($"{note.Created:yyyy-MM-dd HH:mm} - {note.Text}");
        }

        static void ManageProfiles()
        {
            Console.WriteLine("Профили:");
            for (int i = 0; i < profiles.Count; i++) Console.WriteLine($"{i + 1}. {profiles[i].Name}");
            Console.WriteLine("a - добавить профиль, s - выбрать профиль, e - экспорт профилей");
            var cmd = Console.ReadLine();
            if (cmd == "a")
            {
                Console.Write("Имя нового профиля: "); var nm = Console.ReadLine();
                profiles.Add(new Profile { Name = nm }); Console.WriteLine("Профиль добавлен.");
            }
            else if (cmd == "s")
            {
                Console.Write("Номер профиля: "); int idx = int.Parse(Console.ReadLine() ?? "");
                if (idx > 0 && idx <= profiles.Count) { current = profiles[idx - 1]; Console.WriteLine("Профиль выбран."); }
            }
            else if (cmd == "e")
            {
                var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("profiles_export.json", json);
                Console.WriteLine("Профили экспортированы в profiles_export.json");
            }
        }

        static void EmulateTime()
        {
            Console.WriteLine($"Текущее время: {Now:yyyy-MM-dd HH:mm:ss}");
            Console.Write("Введите эмулируемое время (yyyy-MM-dd HH:mm) или пустую строку для сброса: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                fakeNow = null;
                Console.WriteLine("Эмуляция времени сброшена.");
            }
            else if (DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out var dt))
            {
                fakeNow = dt;
                Console.WriteLine($"Эмулируемое время установлено: {fakeNow:yyyy-MM-dd HH:mm}");
            }
            else Console.WriteLine("Неверный формат даты.");
        }

        static void LoadProfiles()
        {
            if (File.Exists("profiles_export.json"))
            {
                var json = File.ReadAllText("profiles_export.json");
                profiles = JsonSerializer.Deserialize<List<Profile>>(json) ?? new();
            }
        }

        static void SaveProfiles()
        {
            var json = JsonSerializer.Serialize(profiles);
            File.WriteAllText("profiles_export.json", json);
        }
    }
}
