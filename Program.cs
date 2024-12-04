using System;
using System.Collections.Generic;
using System.Linq;
using WeeklyCalendar.Data;
using WeeklyCalendar.Models;

namespace WeeklyCalendar
{
    class Program
    {
        private static DatabaseManager _dbManager = new DatabaseManager();
        private static int _currentYear = DateTime.Now.Year;
        private static int _currentWeek = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

        static void Main(string[] args)
        {
            while (true)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddNewActivity();
                        break;
                    case "2":
                        ViewActivities();
                        break;
                    case "3":
                        EditActivity();
                        break;
                    case "4":
                        DeleteActivity();
                        break;
                    case "5":
                        NavigateWeeks();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== Weekly Calendar - Week {_currentWeek}/{_currentYear} ===");
            Console.WriteLine("1. Add new activity");
            Console.WriteLine("2. View activities");
            Console.WriteLine("3. Edit activity");
            Console.WriteLine("4. Delete activity");
            Console.WriteLine("5. Change week");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
        }

        static void AddNewActivity()
        {
            Console.Clear();
            var weekStart = System.Globalization.ISOWeek.ToDateTime(_currentYear, _currentWeek, DayOfWeek.Monday);
            var dates = Enumerable.Range(0, 7).Select(days => weekStart.AddDays(days)).ToList();

            Console.WriteLine("Select day:");
            for (int i = 0; i < dates.Count; i++)
            {
                var date = dates[i];
                Console.WriteLine($"{i + 1}. {date.DayOfWeek} ({date.ToShortDateString()})");
            }

            if (!int.TryParse(Console.ReadLine(), out int dayChoice) || dayChoice < 1 || dayChoice > 7)
            {
                Console.WriteLine("Invalid choice. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var selectedDate = dates[dayChoice - 1];

            Console.WriteLine("Enter activity description: ");
            var description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            _dbManager.AddActivity(selectedDate.DayOfWeek.ToString(), description);
            Console.WriteLine("Activity added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewActivities()
        {
            Console.Clear();
            var activities = _dbManager.GetActivitiesByWeek(_currentYear, _currentWeek);
            
            // Get dates for the current week
            var weekStart = System.Globalization.ISOWeek.ToDateTime(_currentYear, _currentWeek, DayOfWeek.Monday);
            var dates = Enumerable.Range(0, 7).Select(days => weekStart.AddDays(days)).ToList();

            for (int i = 0; i < 7; i++)
            {
                var date = dates[i];
                var day = date.DayOfWeek.ToString();
                Console.WriteLine($"\n=== {day} ({date.ToShortDateString()}) ===");
                var dayActivities = activities.Where(a => a.DayOfWeek == day).ToList();
                
                if (!dayActivities.Any())
                {
                    Console.WriteLine("No activities recorded");
                }
                else
                {
                    foreach (var activity in dayActivities)
                    {
                        Console.WriteLine($"[ID: {activity.Id}] - {activity.Description} (Added: {activity.Date.ToShortDateString()})");
                    }
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void EditActivity()
        {
            Console.Clear();
            ViewActivities();
            
            Console.WriteLine("\nEnter the ID of the activity you want to edit (or press Enter to cancel): ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input)) return;
            
            if (int.TryParse(input, out int id))
            {
                var activity = _dbManager.GetActivityById(id);
                if (activity != null)
                {
                    Console.WriteLine($"Current description: {activity.Description}");
                    Console.WriteLine("Enter new description: ");
                    var newDescription = Console.ReadLine();
                    
                    if (!string.IsNullOrWhiteSpace(newDescription))
                    {
                        _dbManager.UpdateActivity(id, newDescription);
                        Console.WriteLine("Activity updated successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Invalid description. Update cancelled.");
                    }
                }
                else
                {
                    Console.WriteLine("Activity not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeleteActivity()
        {
            Console.Clear();
            ViewActivities();
            
            Console.WriteLine("\nEnter the ID of the activity you want to delete (or press Enter to cancel): ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input)) return;
            
            if (int.TryParse(input, out int id))
            {
                var activity = _dbManager.GetActivityById(id);
                if (activity != null)
                {
                    Console.WriteLine($"Are you sure you want to delete this activity?");
                    Console.WriteLine($"[{activity.DayOfWeek}] {activity.Description}");
                    Console.Write("Type 'yes' to confirm: ");
                    
                    if (Console.ReadLine()?.ToLower() == "yes")
                    {
                        _dbManager.DeleteActivity(id);
                        Console.WriteLine("Activity deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                else
                {
                    Console.WriteLine("Activity not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void NavigateWeeks()
        {
            Console.Clear();
            Console.WriteLine("1. Previous week");
            Console.WriteLine("2. Next week");
            Console.WriteLine("3. Go to specific week");
            Console.WriteLine("4. Return to current week");
            Console.WriteLine("5. Back to main menu");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var prev = _dbManager.GetPreviousWeek(_currentYear, _currentWeek);
                    _currentYear = prev.Year;
                    _currentWeek = prev.Week;
                    break;
                case "2":
                    var next = _dbManager.GetNextWeek(_currentYear, _currentWeek);
                    _currentYear = next.Year;
                    _currentWeek = next.Week;
                    break;
                case "3":
                    Console.Write("Enter week number (1-52): ");
                    if (int.TryParse(Console.ReadLine(), out int week) && week >= 1 && week <= 52)
                    {
                        _currentWeek = week;
                    }
                    break;
                case "4":
                    _currentYear = DateTime.Now.Year;
                    _currentWeek = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
                    break;
            }
        }
    }
} 