 using Microsoft.Data.Sqlite;
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
    }
} 
