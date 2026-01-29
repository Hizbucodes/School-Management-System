namespace SchoolManagementSystem.API.Dtos
{
    public class AttendanceReportDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime Date { get; set; }
        public int TotalStudents { get; set; }
        public int PresentStudents { get; set; }
        public int AbsentStudents { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceResponseDto> Records { get; set; } = new();
    }
}
