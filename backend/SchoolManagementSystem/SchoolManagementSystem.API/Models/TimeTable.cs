namespace SchoolManagementSystem.API.Models
{
    public class TimeTable
    {
        public Guid Id { get; set; }

        public Guid ClassId { get; set; }
        public Class Class { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string RoomNumber { get; set; }
    }
}
