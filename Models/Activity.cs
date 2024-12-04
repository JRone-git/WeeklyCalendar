namespace WeeklyCalendar.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public required string DayOfWeek { get; set; }
        public required string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
 
