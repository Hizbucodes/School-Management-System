namespace SchoolManagementSystem.API.Dtos
{
    public class TimeTableDto
    {
        public Guid Id { get; set; }
        public string CourseName { get; set; } // Flattened from Course entity
        public string TeacherName { get; set; } // Flattened from Teacher/User
        public string Day { get; set; } // Convert DayOfWeek to String
        public string StartTime { get; set; } // Convert TimeSpan to string (e.g., "08:00")
        public string EndTime { get; set; }
        public string RoomNumber { get; set; }
    }
}
