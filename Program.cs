using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace WeeklyCalendar
{
    class Program
    {
        private static DatabaseManager _dbManager = new DatabaseManager();

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
            Console.WriteLine("=== Weekly Calendar ===");
            Console.WriteLine("1. Add new activity");
            Console.WriteLine("2. View activities");
            Console.WriteLine("3. Edit activity");
            Console.WriteLine("4. Delete activity");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
        }

        static void AddNewActivity()
        {
            Console.Clear();
            Console.WriteLine("Enter day of week (Monday-Sunday): ");
            var day = Console.ReadLine();

            Console.WriteLine("Enter activity description: ");
            var description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(day) || string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            _dbManager.AddActivity(day, description);
            Console.WriteLine("Activity added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        static void ViewActivities()
        {
            Console.Clear();
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            foreach (var day in days)
            {
                Console.WriteLine($"\n=== {day} ===");
                var activities = _dbManager.GetActivitiesByDay(day);
                
                if (activities.Count == 0)
                {
                    Console.WriteLine("No activities recorded");
                }
                else
                {
                    foreach (var activity in activities)
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
    }
} 