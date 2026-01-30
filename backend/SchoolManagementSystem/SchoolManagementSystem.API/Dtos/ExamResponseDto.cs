namespace SchoolManagementSystem.API.Dtos
{
    public class ExamResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int TotalMarks { get; set; }
        public int Duration { get; set; } // in minutes
        public int RegisteredStudents { get; set; }
    }
}
