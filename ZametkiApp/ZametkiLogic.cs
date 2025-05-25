using System;
using System.Collections.Generic;
using System.Globalization;

namespace CalendarApp
{
    public class Day
    {
        public DateTime Date { get; set; }
        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;
        public string DayOfWeekStr => Date.ToString("dddd", CultureInfo.CurrentCulture);
    }

    public class Note
    {
        public DateTime At { get; set; }
        public string Text { get; set; }
        public bool Triggered { get; set; } = false;
    }

    public class Profile
    {
        public string Name { get; set; } = "";
        public List<Note> Notes { get; set; } = new List<Note>();
        public List<Note> Reminders { get; set; } = new List<Note>();
    }

    public class CalendarParser
    {
        DateTime from;
        DateTime to;

        public CalendarParser(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public IEnumerable<Day> GetMonth(int year, int month)
        {
            var daysCount = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysCount; day++)
            {
                var dt = new DateTime(year, month, day);
                if (dt >= from && dt <= to)
                    yield return new Day() { Date = dt };
            }
        }
    }
}
