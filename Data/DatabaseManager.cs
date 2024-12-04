using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WeeklyCalendar.Models;

namespace WeeklyCalendar.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=calendar.db");
    }

    public class DatabaseManager
    {
        private readonly DatabaseContext _context;

        public DatabaseManager()
        {
            _context = new DatabaseContext();
            _context.Database.EnsureCreated();
        }

        public void AddActivity(string dayOfWeek, string description)
        {
            var activity = new Activity
            {
                DayOfWeek = dayOfWeek,
                Description = description,
                Date = DateTime.Now
            };

            _context.Activities.Add(activity);
            _context.SaveChanges();
        }

        public List<Activity> GetActivitiesByDay(string dayOfWeek)
        {
            return _context.Activities
                .Where(a => a.DayOfWeek == dayOfWeek)
                .OrderBy(a => a.Date)
                .ToList();
        }

        public void DeleteActivity(int id)
        {
            var activity = _context.Activities.Find(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                _context.SaveChanges();
            }
        }

        public void UpdateActivity(int id, string description)
        {
            var activity = _context.Activities.Find(id);
            if (activity != null)
            {
                activity.Description = description;
                _context.SaveChanges();
            }
        }

        public Activity? GetActivityById(int id)
        {
            return _context.Activities.Find(id);
        }

        public List<Activity> GetActivitiesByWeek(int year, int weekNumber)
        {
            return _context.Activities
                .AsEnumerable()
                .Where(a => System.Globalization.ISOWeek.GetWeekOfYear(a.Date) == weekNumber 
                           && a.Date.Year == year)
                .OrderBy(a => a.Date)
                .ToList();
        }

        public (int Year, int Week) GetPreviousWeek(int year, int week)
        {
            var date = FirstDateOfWeek(year, week).AddDays(-7);
            return (date.Year, GetWeekNumber(date));
        }

        public (int Year, int Week) GetNextWeek(int year, int week)
        {
            var date = FirstDateOfWeek(year, week).AddDays(7);
            return (date.Year, GetWeekNumber(date));
        }

        private DateTime FirstDateOfWeek(int year, int weekNumber)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            var firstThursday = jan1.AddDays(daysOffset);
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstThursday, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekNumber;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        private int GetWeekNumber(DateTime date)
        {
            return System.Globalization.ISOWeek.GetWeekOfYear(date);
        }
    }
} 
