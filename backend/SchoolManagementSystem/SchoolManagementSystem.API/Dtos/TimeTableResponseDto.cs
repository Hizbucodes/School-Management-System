namespace SchoolManagementSystem.API.Dtos
{
    public class TimeTableResponseDto
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public DayOfWeek Day { get; set; }
        public string DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RoomNumber { get; set; }
        public int Duration { get; set; }
    }
}
